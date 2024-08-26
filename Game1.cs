using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

// TODO: Need to add the window for the battle menu when the enemy touches the player - this is the priority for tommorow
//Pathfinding AI works, but the a star algorithm is not used. i need to incorporate it so that the enemy/s can find ways around to get to the user once it detects the player. Make A Star pass in the tilemap so that the enemy can find a path around to get to the player
// TODO: Enemies should have unique positions for each room they're in
// TODO: The player should be able to move back and forth through the trigger zones without fail. When the player enters the trigger zone, they will spawn one row of tile in front of the trigger zone they just came from if they are on tiles 2 to 7 and they will spawn one row of tiles behind the trigger zone if the tiles are 11 to 16 (this is because the player is going back through the rooms)
// TODO: The enemy should patrol throughout the map and not just one section unless there are multiple enemies in a room (multiple enemies walking about would be too difficult to complete game)

namespace MonoGame_Files
{


    public class Game1 : Game
    {

        private Dictionary<string, int[,]> roomTileMaps;
        private string currentRoom;
        int[,] tileMap;
        int tileSize = 80;

        int Player_speed = 2;
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

        private Dictionary<string, Vector2> roomEnemyPositions;

        int playerSize = 65;

        private Direction direction;
        private Vector2 previousPlayerPosition;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private readonly Dictionary<int, string> tileTriggerZones = new Dictionary<int, string>
        {
           { 2, "Room2" }, // Tile value 2 transitions to Room2
           { 3, "Room3" }, // Tile value 3 transitions to Room3
           { 4, "Room4" }, // Tile value 4 transitions to Room4
           { 5, "Room5" }, // Tile value 5 transitions to Room5
           { 6, "Room6" }, // Tile value 6 transitions to Room6
           { 7, "Room7" }, // Tile value 7 transitions to Room7
           { 11, "Room1" }, // Tile value 11 transitions back to Room1
           { 12, "Room2" }, // Tile value 12 transitions back to Room2
           { 13, "Room3" }, // Tile value 13 transitions back to Room3
           { 14, "Room4" }, // Tile value 14 transitions back to Room4
           { 15, "Room5" },  // Tile value 15 transitions back to Room5 
           { 16, "Room6"} // Tile value 16 transitions back to Room6
        };

        public string[,] roomConnections = { //CURRENT_ROOM, LEFT, RIGHT, UP, DOWN -> For each row in array
            { "Room1", "", "", "Room2", ""}, // Each row represents the connections of each rooms. First index is the current room
            { "Room2", "" , "Room3" , "", "Room1"},
            { "Room3", "Room2", "" , "Room4", ""},
            { "Room4", "" , "" , "Room5", "Room3"},
            { "Room5", "" , "" , "Room6", "Room4"},
            { "Room6", "Room7" , "" , "", "Room5"},
            {"Room7", "", "Room6", "", "" }
            // Incorportate this logic into HandleRoomTranstition().
        };

        string targetRoom;
        public enum Direction 
        {
            None,
            Up,
            Down,
            Right,
            Left
        }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            roomTileMaps = new Dictionary<string, int[,]>();
            // 0 --> no tile, 1 --> a tile is present
            roomTileMaps.Add("Room1", new int[,] { // Starting Room
                { 1, 1, 1, 1, 2, 2, 2, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // 2 is to go to room 2
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1 }

            });

            roomTileMaps.Add("Room2", new int[,] { // Monster room 
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, // 11 is to go back down to room 1
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // 3 is to go to room 3
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3 },
                { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 3 },
                { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 3 },
                { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 11, 11, 11, 1, 1, 1, 1},

            });

            roomTileMaps.Add("Room3", new int[,] { // Chance room
                { 1, 1, 1, 1, 4, 4, 4, 1, 1, 1, 1 },// 4 is to go up to room 4
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // 12 is to go back to room 2
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }

            });

            roomTileMaps.Add("Room4", new int[,] { // Multiple monster room
                { 1, 1, 1, 1, 5, 5, 5, 1, 1, 1, 1 }, // 5 is to go up to room 5
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // 12 is to go back down to room 3
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 13, 13, 13, 1, 1, 1, 1 }
            });

            roomTileMaps.Add("Room5", new int[,] {
                { 1, 1, 1, 1, 6, 6, 6, 1, 1, 1, 1 }, // 6 is to go up to room 6
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // 13 is to go down to room 4
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 0, 0, 0, 0, 1, 1, 1 },
                { 1, 1, 1, 1, 14, 14, 14, 1, 1, 1, 1 }
            });

            roomTileMaps.Add("Room6", new int[,] { // Chest room
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }, // 7 is to go left to the boss/ending room
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }, // 14 is to go down to room 5
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 15, 15, 15, 1, 1, 1, 1 }
            });

            roomTileMaps.Add("Room7", new int[,] { //Boss and Ending Room
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
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
            /*
            private void InitializePatrolPoints() Do this later
            {
                patrolPoints.Clear();
                int mapWidth = tileMap.GetLength(1) * tileSize;
                int mapHeight = tileMap.GetLength(0) * tileSize;

                patrolPoints.Add(new Vector2(100, 200));
                patrolPoints.Add(new Vector2(mapWidth - 100, 200));
                patrolPoints.Add(new Vector2(mapWidth - 100, mapHeight - 200));
                patrolPoints.Add(new Vector2(100, mapHeight - 200));
            }
            */

            currentPatrolIndex = 0;

            roomEnemyPositions = new Dictionary<string, Vector2>
            {
                {"Room1", new Vector2() },
                { "Room2", new Vector2(100, 200) },
                { "Room4", new Vector2(150, 250) },

            };

            //if (roomEnemyPositions.ContainsKey(currentRoom))
            //{
            //Enemy_position = roomEnemyPositions[currentRoom];
            //}

            previousPlayerPosition = Player_position;


            int mapWidth = tileMap.GetLength(1) * tileSize;
            int mapHeight = tileMap.GetLength(0) * tileSize;

            _graphics.PreferredBackBufferWidth = mapWidth;
            _graphics.PreferredBackBufferHeight = mapHeight;
            _graphics.ApplyChanges();


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
            Vector2 previousEnemyPosition = Enemy_position;
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

                Patrol();
            }

            HandleRoomTransition();

            // Check for player touching the enemy to trigger battle menu
            if (IsPlayerTouchingEnemy())
            {
                // Trigger battle menu Bailey made
                // Example: ShowBattleMenu();
            }



            base.Update(gameTime);
        }

        private bool IsColliding(Vector2 position, int width, int height)
        {
            int leftTile = (int)(position.X / tileSize);
            int rightTile = (int)((position.X + width) / tileSize);
            int topTile = (int)(position.Y / tileSize);
            int bottomTile = (int)((position.Y + height) / tileSize);

            for (int x = leftTile; x <= rightTile; x++)
            {
                for (int y = topTile; y <= bottomTile; y++)
                {
                    if (x >= 0 && x < tileMap.GetLength(1) && y >= 0 && y < tileMap.GetLength(0))
                    {
                        if (tileMap[y, x] == 1) // 1 indicates a solid tile
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void Patrol()
        {
            if (patrolPoints.Count > 0)
            {
                Vector2 targetPoint = patrolPoints[currentPatrolIndex];
                UpdateEnemyPosition(targetPoint);

                if (Vector2.Distance(Enemy_position, targetPoint) < Enemy_speed)
                {
                    Enemy_position = targetPoint;
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
                }
            }
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


        // Make the enemy follow AStar Pathfinding to find alternative paths to the player


        private bool IsInTriggerZone(Vector2 playerPosition, out Direction direction)
        {
            int x = (int)(playerPosition.X / tileSize);
            int y = (int)(playerPosition.Y / tileSize);
            direction = Direction.None;

            if (x >= 0 && x < tileMap.GetLength(1) && y >= 0 && y < tileMap.GetLength(0))
            {
                int tileValue = tileMap[y, x];

                if (tileTriggerZones.ContainsKey(tileValue))
                {
                    // Determine the direction based on tile value (e.g., 2 for Room2, 11 for Room1)
                    //if (tileValue >= 2 && tileValue <= 7)
                    //{
                    //direction = Direction.Up;
                    //}


                    //else if (tileValue >= 11 && tileValue <= 16)
                    //{
                    //direction = Direction.Down;
                    //}

                    Vector2 position = GetTileIndexAtPosition(playerPosition);
                    if (tileValue >= 2 && tileValue <= 16)
                    {
                        if (y == 0 && (x >= 4 && x <= 6))
                        {
                            direction = Direction.Up;
                        }
                        else if (y == 11 && (y >= 4 && y <= 6))
                        {
                            direction = Direction.Down;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        private bool IsPlayerTouchingEnemy()
        {
            Rectangle playerRect = new Rectangle((int)Player_position.X, (int)Player_position.Y, playerSize, playerSize);
            Rectangle enemyRect = new Rectangle((int)Enemy_position.X, (int)Enemy_position.Y, enemySize, enemySize);

            return playerRect.Intersects(enemyRect); // triggers battle menu
        }

        private int GetTileValueAtPosition(Vector2 playerPosition)
        {
            int x = (int)(playerPosition.X / tileSize);
            int y = (int)(playerPosition.Y / tileSize);

            if (x >= 0 && x < tileMap.GetLength(1) && y >= 0 && y < tileMap.GetLength(0))
            {
                return tileMap[y, x];
            }
            return 0;
        }

        private Vector2 GetTileIndexAtPosition(Vector2 playerPosition)
        {
            int x = (int)(playerPosition.X / tileSize);
            int y = (int)(playerPosition.Y / tileSize);

            if (x >= 0 && x < tileMap.GetLength(1) && y >= 0 && y < tileMap.GetLength(0))
            {
                return new Vector2(x, y);
            }
            return new Vector2 (0, 0);
        }



        private void HandleRoomTransition()
        {
            // Check if the player is in a trigger zone
            if (IsInTriggerZone(Player_position, out Direction direction))
            {
                // Get the tile value at the player's position
                int tileValue = GetTileValueAtPosition(Player_position);

                if (tileTriggerZones.TryGetValue(tileValue, out string targetRoomName))
                {
                    if (targetRoomName != currentRoom)
                    {
                        LoadNewRoom(direction, targetRoomName);
                    }
                }
            }

        }

      

        private void LoadNewRoom(Direction direction, string newRoomName)
        {
            if (roomTileMaps.ContainsKey(newRoomName))
            {
                tileMap = roomTileMaps[newRoomName];
                currentRoom = newRoomName;

                // Adjust player position based on direction
                Vector2 newSpawnPosition = GetValidSpawnPosition(direction);

                if (direction == Direction.Up)
                {
                    newSpawnPosition = new Vector2(500, 700);
                }

                else if (direction == Direction.Down)
                {
                    newSpawnPosition = new Vector2(500, 100);
                }

                else if(direction == Direction.Left)
                {
                    newSpawnPosition = new Vector2(750, 500);
                }

                if (newSpawnPosition != Vector2.Zero)
                {
                    Player_position = newSpawnPosition;
                    
                }

                // Reset other variables related to the room, like enemy positions
                if (roomEnemyPositions.ContainsKey(newRoomName))
                {
                    Enemy_position = roomEnemyPositions[newRoomName];
                }

                // Optionally adjust the window size to fit the new tilemap
                int mapWidth = tileMap.GetLength(1) * tileSize;
                int mapHeight = tileMap.GetLength(0) * tileSize;
                _graphics.PreferredBackBufferWidth = mapWidth;
                _graphics.PreferredBackBufferHeight = mapHeight;
                _graphics.ApplyChanges();
            }
        }

        private Vector2 GetValidSpawnPosition(Direction direction)
        {
            // Default spawn position in case we can't find a valid one
            Vector2 spawnPosition = Vector2.Zero;

            // Get the position of the trigger zone tile
            int triggerTileX = (int)(Player_position.X / tileSize);
            int triggerTileY = (int)(Player_position.Y / tileSize);

            // Adjust spawn position based on direction
            switch (direction)
            {
                case Direction.Up:
                    spawnPosition = new Vector2(triggerTileX * tileSize, (triggerTileY - 1) * tileSize);
                    break;
                case Direction.Down:
                    spawnPosition = new Vector2(triggerTileX * tileSize, (triggerTileY + 1) * tileSize);
                    break;

                case Direction.Left:
                    spawnPosition = new Vector2(triggerTileX * tileSize, (triggerTileY) * tileSize); // Make sure this is correct with logic
                    break;
                case Direction.Right:

                    break;
                    
            }

            // Ensure spawn position is within bounds and not on a solid tile
            if (IsWithinBounds(spawnPosition) && !IsSolidTile(spawnPosition))
            {
                return spawnPosition;
            }
            else
            {
                // Return a fallback position if the calculated position is invalid
                return FindFallbackSpawnPosition();
            }
        }

        private bool IsWithinBounds(Vector2 position)
        {
            int x = (int)(position.X / tileSize);
            int y = (int)(position.Y / tileSize);
            return x >= 0 && x < tileMap.GetLength(1) && y >= 0 && y < tileMap.GetLength(0);
        }

        private bool IsSolidTile(Vector2 position)
        {
            int x = (int)(position.X / tileSize);
            int y = (int)(position.Y / tileSize);
            return tileMap[y, x] == 1;
        }

        private Vector2 FindFallbackSpawnPosition()
        {
            // This method finds a fallback spawn position that is guaranteed to be valid
            for (int y = 0; y < tileMap.GetLength(0); y++)
            {
                for (int x = 0; x < tileMap.GetLength(1); x++)
                {
                    Vector2 position = new Vector2(x * tileSize, y * tileSize);
                    if (!IsSolidTile(position))
                    {
                        return position;
                    }
                }
            }

            // If no valid position found, return a default position
            return new Vector2(0, 0);
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

                    else if (tileMap[y, x] >= 2 && tileMap[y, x] <= 16)
                    {
                        _spriteBatch.Draw(TileMap_texture, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), Color.LightGoldenrodYellow);
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
            if (currentRoom == "Room2" || currentRoom == "Room4") // Add or statements for rooms without any enemies
            {
                _spriteBatch.Draw(Enemy_texture, new Rectangle((int)Enemy_position.X, (int)Enemy_position.Y, enemySize, enemySize), Color.Green); // Enemy dimensions
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        // Get the A Star working so that the enemy can go around the tiles to reach the player
        /*
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
    */
    }
    




}











    

       



    

       

