using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame_Files;
using System;
using System.Collections.Generic;
using System.Linq;

//Pathfinding AI works, but is a bit choppy (enemy sometimes spazzes out) and the enemy can pass straight through a tile to find the player
//Need to add the game classes for the other windoes (rooms and battle menu)
namespace MonoGame_Files
{


    public class Game1 : Game
    {
        int[,] tileMap = {
            { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 1, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
            { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
            { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0 },
            { 1, 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 0 }
        }; // 0 -> the tile is not there, 1 -> a tile is present. 12, 9
        int tileSize = 80;

        int Player_speed = 3;
        int Enemy_speed = 1; 

        Texture2D TileMap_texture;
        Texture2D Player_texture;
        Vector2 Player_position;

        Texture2D Enemy_texture;
        Vector2 Enemy_position;
        Vector2 Enemy_position_default;


        int enemySize = 50;

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
            Enemy_position = new Vector2(800, 190);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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

            if (!IsColliding(newPosition) &&
                !IsColliding(new Vector2(newPosition.X + tileSize - 1, newPosition.Y)) &&
                !IsColliding(new Vector2(newPosition.X, newPosition.Y + tileSize - 1)) &&
                !IsColliding(new Vector2(newPosition.X + tileSize - 1, newPosition.Y + tileSize - 1))) // this only works if the tiles and player are the same size
            {
                Player_position = newPosition;
            }

            Vector2 enemyNewPosition = Enemy_position;
            
            if (Player_position.X < Enemy_position.X)
            {
                enemyNewPosition.X -= Enemy_speed;
            }
            if (Player_position.X > Enemy_position.X)
            {
                enemyNewPosition.X += Enemy_speed;
            }

            if (Player_position.Y < Enemy_position.Y)
            {
                enemyNewPosition.Y -= Enemy_speed;
            }
            if (Player_position.Y > Enemy_position.Y)
            {
                enemyNewPosition.Y += Enemy_speed;
            }
            

            List<Vector2> path = AStar(Enemy_position, Player_position);

            if (path.Count > 1) // Check if a path exists
            {
                // Move enemy towards the next position in the path
                Vector2 nextPosition = path[1];
                Vector2 direction = Vector2.Normalize(nextPosition - Enemy_position) * Enemy_speed;
                Enemy_position += direction;
            }

            if (!IsColliding(enemyNewPosition) &&
                !IsColliding(new Vector2(enemyNewPosition.X + enemySize - 1, enemyNewPosition.Y)) &&
                !IsColliding(new Vector2(enemyNewPosition.X, enemyNewPosition.Y + enemySize - 1)) &&
                !IsColliding(new Vector2(enemyNewPosition.X + enemySize - 1, enemyNewPosition.Y + enemySize - 1))) 
            {
                Enemy_position = enemyNewPosition;
            } 

            base.Update(gameTime);
        }

        private bool IsColliding(Vector2 position)
        {
            int x = (int)(position.X / tileSize);
            int y = (int)(position.Y / tileSize);

            if (x < 0 || x >= tileMap.GetLength(1) || y < 0 || y >= tileMap.GetLength(0))
            {
                return true; // Outside bounds, treat as collision
            }

            return tileMap[y, x] == 1;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            for (int y = 0; y < tileMap.GetLength(0); y++)
            {
                for (int x = 0; x < tileMap.GetLength(1); x++)
                {
                    if (tileMap[y, x] == 1)
                    {
                        _spriteBatch.Draw(TileMap_texture, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), Color.White);
                    }
                }
            }

            _spriteBatch.Draw(Player_texture, new Rectangle((int)Player_position.X, (int)Player_position.Y, tileSize, tileSize), Color.White);
            _spriteBatch.Draw(Enemy_texture, new Rectangle((int)Enemy_position.X, (int)Enemy_position.Y, enemySize, enemySize), Color.Green);

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
                    if (closedList.Contains(neighbor) || IsColliding(new Vector2(neighbor.X * tileSize, neighbor.Y * tileSize)))
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

    

       

