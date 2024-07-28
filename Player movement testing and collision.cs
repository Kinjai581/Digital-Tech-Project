using System.Drawing;

namespace Player_test
{
    public partial class Form1 : Form
    {
        bool moveUp, moveDown, moveLeft, moveRight;
        int speed = 10;
        Point lastPosition;

        int[,] tileMap = {
            { 1, 0, 0, 1 },
            { 0, 0, 0, 1 },
            { 1, 1, 0, 0 }
        }; // 0 -> the tile is not there, 1 -> a tile is present

        //Work more on setting up the physical tile map for the rooms

        int tileSize = 100;

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Image = Image.FromFile("Images/Player.png"); // Upload a player sprite into the picture box widget
            this.KeyPreview = true;
            timer1.Start();
            lastPosition = pictureBox1.Location;
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

        public void DrawTileMap(Graphics g)
        {

            int x, y;
            for (y = 0; y < tileMap.GetLength(0); y++)
            {
                for (x = 0; x < tileMap.GetLength(1); x++)
                {
                    if (tileMap[y, x] == 1)
                    {
                        using (SolidBrush brush = new SolidBrush(Color.Black))
                        {
                            g.FillRectangle(brush, x * tileSize, y * tileSize, tileSize, tileSize);
                        }
                    }

                }
            } // Make the collision detector for these black rectangles

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Define the amount of pixels to move the PictureBox.
            int moveAmount = 7;




            // Check which key was pressed and move the PictureBox accordingly.
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    moveUp = true;
                    break;
                case Keys.S:
                case Keys.Down:
                    moveDown = true;
                    break;
                case Keys.A:
                case Keys.Left:
                    moveLeft = true;
                    break;
                case Keys.D:
                case Keys.Right:
                    moveRight = true;
                    break;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private bool IsColliding(PictureBox pb1, PictureBox pb2)
        {
            return pb1.Bounds.IntersectsWith(pb2.Bounds);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lastPosition = pictureBox1.Location;




            if (moveUp && pictureBox1.Top > 0)
            {
                pictureBox1.Top -= speed;
            }
            if (moveDown && pictureBox1.Bottom < this.ClientSize.Height)
            {
                pictureBox1.Top += speed;
            }
            if (moveLeft && pictureBox1.Left > 0)
            {
                pictureBox1.Left -= speed;
            }
            if (moveRight && pictureBox1.Right < this.ClientSize.Width)
            {
                pictureBox1.Left += speed;
            }

            if (IsColliding(pictureBox1, pictureBox2))
            {
                //MessageBox.Show("The player collided");
                pictureBox1.Location = lastPosition;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Up:
                    moveUp = false;
                    break;
                case Keys.S:
                case Keys.Down:
                    moveDown = false;
                    break;
                case Keys.A:
                case Keys.Left:
                    moveLeft = false;
                    break;
                case Keys.D:
                case Keys.Right:
                    moveRight = false;
                    break;
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using (Graphics g = e.Graphics)
            {
                DrawTileMap(g);
            }
        }
    }
}
