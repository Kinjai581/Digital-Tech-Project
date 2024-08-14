using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

//Pathfinding AI works, but some refinements could be made
// Enemies have unique positions for each room they're in
// Sort out room connections and positions after discussing how much rooms should be in the game
// The player should be able to move back and forth through the rooms
// The enemy should patrol throughout the map and not just one section unless there are multiple enemies in a room (multiple enemies walking about would be too difficult to complete game)
// Need to add the window for the battle menu when the enemy touches the player
namespace MonoGame_Files
{


    public class Game1 : Game
    {

        private Dictionary<string, int[,]> roomTileMaps;
        private string currentRoom;
        int[,] tileMap; // 0 -> the tile is not there, 1 -> a tile is present, 2 -> trigger zone to room
        int tileSize = 80;

        int Player_speed = 3;
        int Enemy_speed = 1; 

        Texture2D TileMap_texture;
        Texture2D Player_texture;
        Vector2 Player_position;

        Texture2D Enemy_texture;
        Vector2 Enemy_position;
        
        private List<Vector2> patrolPoints;
        private int currentPatrolIndex;
        private bool isPatrolling = true;


        int enemySize = 50;
        int playerSize = 65;

        private Direction direction;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 880;
            _graphics.PreferredBackBufferHeight = 880;
            _graphics.ApplyChanges();

            roomTileMaps = new Dictionary<string, int[,]>();
            // 0 --> no tile, 1 --> a tile is present, 2--> trigger zones to go to the next room
            roomTileMaps.Add("Room1", new int[,] { // Starting Room
                { 1, 1, 1, 1, 1, 2, 2, 2, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1 } 
            });

             roomTileMaps.Add("Room2", new int[,] {
                { 1, 1, 1, 1, 2, 2, 2, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 },
                { 2, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 2 },
                { 2, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 2 },
                { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 2, 2, 2, 1, 1, 1, 1, 1 }
            });

            roomTileMaps.Add("Room3", new int[,] { // Change this tile map according to Bailey's rooms
                { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
                { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1, 1 }
            });

            roomTileMaps.Add("Room4", new int[,] {
                { 1, 1, 1, 1, 2, 2, 2, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 },
                { 2, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 2 },
                { 2, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 2 },
                { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 },
                { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 2, 2, 2, 1, 1, 1, 1, 1 }
            });

            currentRoom = "Room1";
            tileMap = roomTileMaps[currentRoom];

            patrolPoints = new List<Vector2> // Make the enemy patrol the entire map and not just one section of it
            {
                new Vector2(100, 200),
                new Vector2(400, 200),
                new Vector2(400, 500),
                new Vector2(100, 500)
            };

            currentPatrolIndex = 0;

            base.Initialize();
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TileMap_texture = new Texture2D(GraphicsDevice, 1, 1);
            TileMap_texture.SetData(new[] { Color.Gray });

            Player_texture = new Texture2D(GraphicsDevice, 1, 1);
            Player_texture.SetData(new[] { Color.Black });

            Enemy_texture = new Texture2D(GraphicsDevice, 1, 1);
            Enemy_texture.SetData(new[] { Color.Green });

            Player_position = new Vector2(400, 800);
            Enemy_position = new Vector2(100, 190);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

            // Player Movement
            KeyboardState currentKeyboardState = Keyboard.GetState();
            Vector2 newPosition = Player_position;

            if (currentKeyboardState.IsKeyDown(Keys.W) || currentKeyboardState.IsKeyDown(Keys.Up))
            {
                newPosition.Y -= Player_speed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.A) || currentKeyboardState.IsKeyDown(Keys.Left))
            {
                newPosition.X -= Player_speed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.S) || currentKeyboardState.IsKeyDown(Keys.Down))
            {
                newPosition.Y += Player_speed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.D) || currentKeyboardState.IsKeyDown(Keys.Right))
            {
                newPosition.X += Player_speed;
            }

            // Check for collisions before updating the player position
            if (!IsColliding(newPosition, playerSize, playerSize))
            {
                Player_position = newPosition;
            }

            // Enemy AI Logic
            float detectionRange = 200f; // range within which the enemy detects the player

            if (Vector2.Distance(Player_position, Enemy_position) < detectionRange)
            {
                // Player is within detection range, so chase the player
                isPatrolling = false;
                UpdateEnemyPosition(Player_position);
            }
            else
            {
                // Player is out of detection range, so patrol
                isPatrolling = true;

                if (patrolPoints.Count > 0)
                {
                    Vector2 targetPoint = patrolPoints[currentPatrolIndex];
                    UpdateEnemyPosition(targetPoint);

                    // To prevent overshooting the patrol point
                    if (Vector2.Distance(Enemy_position, targetPoint) < Enemy_speed)
                    {
                        Enemy_position = targetPoint;
                        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count; // Move to the next patrol point
                    }
                }
            }

            // Room transition logic
            if (IsInTriggerZone(Player_position, out Direction direction))
            {
                HandleRoomTransition();
            }

            // Check for player touching the enemy to trigger battle menu
            if (IsPlayerTouchingEnemy())
            {
                // Trigger battle menu
                // Example: ShowBattleMenu();
            }

            base.Update(gameTime);
        }

        private bool IsColliding(Vector2 position, int width, int height)
        {
            int startX = (int)(position.X / tileSize);
            int startY = (int)(position.Y / tileSize);
            int endX = (int)((position.X + width) / tileSize);
            int endY = (int)((position.Y + height) / tileSize);

            if (startX < 0 || endX >= tileMap.GetLength(1) || startY < 0 || endY >= tileMap.GetLength(0))
            {
                return true; // Outside bounds, treat as collision
            }

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    if (tileMap[y, x] == 1)
                    {
                        float tileLeft = x * tileSize;
                        float tileRight = tileLeft + tileSize;
                        float tileTop = y * tileSize;
                        float tileBottom = tileTop + tileSize;

                        if (position.X + width > tileLeft && position.X < tileRight &&
                            position.Y + height > tileTop && position.Y < tileBottom)
                        {
                            return true;
                        }
                    }
                }
            }

            return false; // No collision
        }

        private void UpdateEnemyPosition(Vector2 targetPosition)
        {
            Vector2 direction = targetPosition - Enemy_position;
            float distance = direction.Length();

            if (distance > 0)
            {
                direction.Normalize();
                Vector2 movement = direction * Enemy_speed;

                // Calculate potential new position
                Vector2 newEnemyPosition = Enemy_position + movement;

                // Check for collisions before updating position
                if (!IsColliding(newEnemyPosition, enemySize, enemySize))
                {
                    Enemy_position = newEnemyPosition;
                }
            }
        }

        public enum Direction // If required
        {
            Up,
            Down,
            Right,
            Left 
        }


        private bool IsInTriggerZone(Vector2 playerPosition, out Direction direction)
        {
            int x = (int)(playerPosition.X / tileSize);
            int y = (int)(playerPosition.Y / tileSize);

            if (tileMap[y, x] == 2)
            {
                if (y == 0)
                {
                    direction = Direction.Up;
                    return true;
                }
                else if (y == tileMap.GetLength(0) - 1)
                {
                    direction = Direction.Down;
                    return true;
                }
                else if (x == 0)
                {
                    direction = Direction.Left;
                    return true;
                }
                else if (x == tileMap.GetLength(1) - 1)
                {
                    direction = Direction.Right;
                    return true;
                }
            }

            direction = default;
            return false;
        }

        private bool IsPlayerTouchingEnemy()
        {
            Rectangle playerRect = new Rectangle((int)Player_position.X, (int)Player_position.Y, playerSize, playerSize);
            Rectangle enemyRect = new Rectangle((int)Enemy_position.X, (int)Enemy_position.Y, enemySize, enemySize);

            return playerRect.Intersects(enemyRect); // triggers battle menu
        }

       private void HandleRoomTransition()
        { // Will need to map out the rooms properly in order to do this
            // Transition based on player position and current room
            if (currentRoom == "Room1" && direction == Direction.Up)
            {
                LoadNewRoom(Direction.Up, "Room2");
            }
            else if (currentRoom == "Room2")
            {
                if (direction == Direction.Down)
                {
                    LoadNewRoom(Direction.Down, "Room1");
                }
                else if (direction == Direction.Up)
                {
                    LoadNewRoom(Direction.Up, "Room3");
                }
            }
            // Add additional room transitions here based on our design
        }

        private void LoadNewRoom(Direction direction, string newRoomName)
        {
            if (roomTileMaps.ContainsKey(newRoomName))
            {
                tileMap = roomTileMaps[newRoomName];
                currentRoom = newRoomName;

                // Set player position based on direction of transition
                switch (direction)
                {
                    case Direction.Up:
                        Player_position = new Vector2(Player_position.X, tileMap.GetLength(0) * tileSize - playerSize);
                        break;
                    case Direction.Down:
                        Player_position = new Vector2(Player_position.X, 0);
                        break;
                    case Direction.Left:
                        Player_position = new Vector2(tileMap.GetLength(1) * tileSize - playerSize, Player_position.Y);
                        break;
                    case Direction.Right:
                        Player_position = new Vector2(0, Player_position.Y);
                        break;
                }

                // Adjust enemy position if needed (this may need customization)
                Enemy_position = new Vector2(100, 190); // Example position for all rooms, adjust accordingly
            }
            else
            {
                Console.WriteLine($"Room {newRoomName} does not exist!");
            }
        }
        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray); // Background color

            _spriteBatch.Begin();

            for (int y = 0; y < tileMap.GetLength(0); y++)
            {
                for (int x = 0; x < tileMap.GetLength(1); x++)
                {
                    if (tileMap[y, x] == 1)
                    {
                        _spriteBatch.Draw(TileMap_texture, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), Color.White);
                    }

                    // Optional: Draw tile boundaries for debugging
                    //if (tileMap[y, x] == 1)
                    //{
                       // _spriteBatch.Draw(TileMap_texture, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), Color.Red * 0.5f);
                    //}
                }
            }

            _spriteBatch.Draw(Player_texture, new Rectangle((int)Player_position.X, (int)Player_position.Y, playerSize, playerSize), Color.White); // Player dimensions

            // Ensure the enemy is drawn correctly based on the room
            if (currentRoom != "Room1") // Add or statements for rooms without any enemies
            {
                _spriteBatch.Draw(Enemy_texture, new Rectangle((int)Enemy_position.X, (int)Enemy_position.Y, enemySize, enemySize), Color.Green); // Enemy dimensions
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        private List<Vector2> AStar(Vector2 startWorld, Vector2 goalWorld)
        {
            Vector2 start = new Vector2((int)(startWorld.X / tileSize), (int)(startWorld.Y / tileSize));
            Vector2 goal = new Vector2((int)(goalWorld.X / tileSize), (int)(goalWorld.Y / tileSize));

            var openList = new List<Node>();
            var closedList = new HashSet<Vector2>();

            Node startNode = new Node(start, null, 0, Heuristic(start, goal));
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = openList.OrderBy(node => node.F).First();

                if (currentNode.Position == goal)
                {
                    return ReconstructPath(currentNode);
                }

                openList.Remove(currentNode);
                closedList.Add(currentNode.Position);

                foreach (var neighbor in GetNeighbors(currentNode.Position))
                {
                    if (closedList.Contains(neighbor) || IsColliding(new Vector2(neighbor.X * tileSize, neighbor.Y * tileSize), playerSize, playerSize))
                        continue;

                    float tentativeG = currentNode.G + Vector2.Distance(currentNode.Position, neighbor);

                    Node neighborNode = openList.FirstOrDefault(node => node.Position == neighbor);
                    if (neighborNode == null)
                    {
                        neighborNode = new Node(neighbor, currentNode, tentativeG, Heuristic(neighbor, goal));
                        openList.Add(neighborNode);
                    }
                    else if (tentativeG < neighborNode.G)
                    {
                        neighborNode.Parent = currentNode;
                        neighborNode.G = tentativeG;
                    }
                }
            }

            return new List<Vector2>(); // Return an empty path if no path is found
        }

        private float Heuristic(Vector2 a, Vector2 b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y); // Manhattan distance
        }

        private List<Vector2> GetNeighbors(Vector2 position)
        {
            List<Vector2> neighbors = new List<Vector2>
            {
                new Vector2(position.X + 1, position.Y),
                new Vector2(position.X - 1, position.Y),
                new Vector2(position.X, position.Y + 1),
                new Vector2(position.X, position.Y - 1)
            };

            return neighbors;
        }

        private List<Vector2> ReconstructPath(Node node)
        {
            List<Vector2> path = new List<Vector2>();
            while (node != null)
            {
                path.Add(new Vector2(node.Position.X * tileSize, node.Position.Y * tileSize));
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }
    }

    
    
       
}

    

       



    

       

