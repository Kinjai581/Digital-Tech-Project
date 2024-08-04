using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Player_test_on_mono
{
    // Update collisions

    public class Game1 : Game
    {
        int[,] tileMap = {
            { 1, 0, 0, 1, 0, 0, 1 },
            { 0, 0, 0, 1, 1, 1, 0 },
            { 1, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 1, 1, 0, 0 },
            { 0, 1, 1, 0, 0, 1, 1 }
        }; // 0 -> the tile is not there, 1 -> a tile is present
        int tileSize = 90;

        int speed = 10;

        Texture2D TileMap_texture;
        Texture2D Player_texture;
        Vector2 Player_position;

        Texture2D Enemy_texture;
        Vector2 Enemy_position;

        // Make the enemy box follow the player


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
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            TileMap_texture = new Texture2D(GraphicsDevice, 1, 1);
            TileMap_texture.SetData(new[] { Color.Red });

            Player_texture = new Texture2D(GraphicsDevice, 1, 1);
            Player_texture.SetData(new[] { Color.Black });

            Enemy_texture = new Texture2D (GraphicsDevice, 1, 1);
            Enemy_texture.SetData(new[] { Color.Green });

            Player_position = new Vector2(100,20);
            Enemy_position = new Vector2 (100,100);


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //KeyboardState previousKeyboardState;
            KeyboardState currentKeyboardState = Keyboard.GetState();
            Vector2 newPosition = Player_position;
            Vector2 newPosition2 = Enemy_position;

            
            

            if (currentKeyboardState.IsKeyDown(Keys.W) || currentKeyboardState.IsKeyDown(Keys.Up))
            {
                newPosition.Y -= speed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.A) || currentKeyboardState.IsKeyDown(Keys.Left))
            {
                newPosition.X -= speed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.S) || currentKeyboardState.IsKeyDown(Keys.Down))
            {
                newPosition.Y += speed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.D) || currentKeyboardState.IsKeyDown(Keys.Right))
            {
                newPosition.X += speed;
            }

            if (!IsColliding(newPosition) &&
                !IsColliding(new Vector2(newPosition.X + tileSize - 1, newPosition.Y)) &&
                !IsColliding(new Vector2(newPosition.X, newPosition.Y + tileSize - 1)) &&
                !IsColliding(new Vector2(newPosition.X + tileSize - 1, newPosition.Y + tileSize - 1))) // this only works if the tiles and player are the same size
            {
                Player_position = newPosition;
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

            // TODO: Add your drawing code here

            _spriteBatch.Begin();
            
            int x, y;
            for (y = 0; y < tileMap.GetLength(0); y++)
            {
                for (x = 0; x < tileMap.GetLength(1); x++)
                {
                    if (tileMap[y, x] == 1)
                    {
                        _spriteBatch.Draw(TileMap_texture, new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize), Color.White);
                    }

                }
            }
            _spriteBatch.Draw(Player_texture, new Rectangle((int) Player_position.X, (int) Player_position.Y, tileSize, tileSize), Color.White);
            _spriteBatch.Draw(Enemy_texture, new Rectangle((int)Enemy_position.X, (int)Enemy_position.Y, tileSize, tileSize), Color.Green);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
