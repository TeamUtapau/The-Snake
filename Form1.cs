using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        private List<Circle> Snake = new List<Circle>();
        private Circle food = new Circle();
        List<Point> listOfPoints = new List<Point>();
        PictureBox newBrick = new PictureBox();

        public Form1()
        {
            
            InitializeComponent();

            //Set settings to default
            new Settings();

            //Set game speed and start timer
            gameTimer.Interval = 1000 / Settings.Speed;
            gameTimer.Tick += UpdateScreen;
            gameTimer.Start();

            //Start New game
            StartGame();
        }

        private void StartGame()
        {
            
            lblGameOver.Visible = false;

            //Set settings to default
            new Settings();

            //Create new player object
            Snake.Clear();
            Circle head = new Circle {X = 10, Y = 5};
            Snake.Add(head);
            


            lblScore.Text = Settings.Score.ToString();
            GenerateFood();

        }

        //Place random food object
        private void GenerateFood()
        {
            int maxXPos = pbCanvas.Size.Width / Settings.Width;
            int maxYPos = pbCanvas.Size.Height / Settings.Height;

           
            bool tooCloseToBricks = new bool ();
            int m;
            int n;
            
            do
            {
                Random random = new Random();
                m = random.Next(0, maxXPos);
                n = random.Next(0, maxYPos);
                
                for (int i = 0; i < listOfPoints.Count; i++)
                {
                    int distanceToBricks = (m * Settings.Height - listOfPoints[i].X) * (m * Settings.Height - listOfPoints[i].X) + (n*Settings.Width - listOfPoints[i].Y) * (n*Settings.Width - listOfPoints[i].Y);

                    if (distanceToBricks < 10000)
                    {
                        tooCloseToBricks = true;
                        break;
                    }
                    else
                    {
                        tooCloseToBricks = false;
                    }
                }                
            }
            while(tooCloseToBricks);

            food = new Circle {X = m, Y = n};
        }


        private void UpdateScreen(object sender, EventArgs e)
        {
            //Check for Game Over
            if (Settings.GameOver)
            {
                //Check if Enter is pressed
                if (Input.KeyPressed(Keys.Enter))
                {
                    StartGame();
                }
            }
            else
            {
                if (Input.KeyPressed(Keys.Right) && Settings.direction != Direction.Left)
                    Settings.direction = Direction.Right;
                else if (Input.KeyPressed(Keys.Left) && Settings.direction != Direction.Right)
                    Settings.direction = Direction.Left;
                else if (Input.KeyPressed(Keys.Up) && Settings.direction != Direction.Down)
                    Settings.direction = Direction.Up;
                else if (Input.KeyPressed(Keys.Down) && Settings.direction != Direction.Up)
                    Settings.direction = Direction.Down;

                MovePlayer();
            }

            pbCanvas.Invalidate();

        }

        private void pbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            if (!Settings.GameOver)
            {
                //Set colour of snake

                //Draw snake
                for (int i = 0; i < Snake.Count; i++)
                {
                    Brush snakeColour;
                    if (i == 0)
                        snakeColour = Brushes.Black;     //Draw head
                    else
                        snakeColour = Brushes.Green;    //Rest of body

                    //Draw snake
                    canvas.FillEllipse(snakeColour,
                        new Rectangle(Snake[i].X * Settings.Width,
                                      Snake[i].Y * Settings.Height,
                                      Settings.Width, Settings.Height));


                    //Draw Food
                    canvas.FillEllipse(Brushes.Red,
                        new Rectangle(food.X * Settings.Width,
                             food.Y * Settings.Height, Settings.Width, Settings.Height));

                }
            }
            else
            {
                string gameOver = "Game over \nYour final score is: " + Settings.Score + "\nPress Enter to try again";
                lblGameOver.Text = gameOver;
                lblGameOver.Visible = true;
            }
        }


        private void MovePlayer()
        {
            for (int i = Snake.Count - 1; i >= 0; i--)
            {
                //Move head
                if (i == 0)
                {
                    switch (Settings.direction)
                    {
                        case Direction.Right:
                            Snake[i].X++;
                            break;
                        case Direction.Left:
                            Snake[i].X--;
                            break;
                        case Direction.Up:
                            Snake[i].Y--;
                            break;
                        case Direction.Down:
                            Snake[i].Y++;
                            break;
                    }


                    //Get maximum X and Y Pos
                    int maxXPos = pbCanvas.Size.Width / Settings.Width;
                    int maxYPos = pbCanvas.Size.Height / Settings.Height;

                    //Detect collission with game borders.
                    if (Snake[i].X < 0 || Snake[i].Y < 0
                        || Snake[i].X >= maxXPos || Snake[i].Y >= maxYPos)
                    {
                        Die();
                    }


                    //Detect collission with body
                    for (int j = 1; j < Snake.Count; j++)
                    {
                        if (Snake[i].X == Snake[j].X &&
                           Snake[i].Y == Snake[j].Y)
                        {
                            Die();
                        }
                    }

                    //Detect collision with food piece
                    if (Snake[0].X == food.X && Snake[0].Y == food.Y)
                    {
                        Eat();

                                 

                    }

                    //Detect collision with barrier   
                    for (int j = 0; j < listOfPoints.Count; j++)
                    {

                        Rectangle myRectangle = new Rectangle(listOfPoints[j].X - 25, listOfPoints[j].Y - 25, 67, 26);



                        if (myRectangle.Contains(Snake[0].X * Settings.Width, Snake[0].Y * Settings.Height))
                        {

                            Die();

                        }
                    }

                }
                else
                {
                    //Move body
                    Snake[i].X = Snake[i - 1].X;
                    Snake[i].Y = Snake[i - 1].Y;
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, true);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            Input.ChangeState(e.KeyCode, false);
        }

        private void Eat()
        {
            
            //Add circle to body
            Circle circle = new Circle
            {
                X = Snake[Snake.Count - 1].X,
                Y = Snake[Snake.Count - 1].Y,        
                
            };
            Snake.Add(circle);

            

            //Update Score
            Settings.Score += Settings.Points;
            

            lblScore.Text = Settings.Score.ToString();
            gameTimer.Interval = 100;            
            GenerateFood();
            


            if (Settings.Score % 200 == 0)
            {
                bool tooCloseToFood = new bool();
                bool tooCloseToOtherBricks = new bool();
                bool tooCloseToSnakeBody = new bool();


                PictureBox newBrick = new PictureBox();
                Random rnd = new Random();

                int maxX = pbCanvas.Size.Width - 100;
                int maxY = pbCanvas.Size.Height - 100;

                Point p1;


                do
                {
                    int xCoordinate = rnd.Next(13, maxX);
                    int yCoordinate = rnd.Next(13, maxY);
                    p1 = new Point(xCoordinate, yCoordinate);

                    int distanceToFood = (p1.X - food.X * Settings.Height) * (p1.X - food.X * Settings.Height) + (p1.Y - food.Y * Settings.Height) * (p1.Y - food.Y * Settings.Height);

                    if (distanceToFood < 10000)
                    {
                        tooCloseToFood = true;
                    }
                    else
                    {
                        tooCloseToFood = false;
                    }

                    for (int i = 0; i < listOfPoints.Count; i++)
                    {
                        int distanceToOtherBricks = (p1.X - listOfPoints[i].X) * (p1.X - listOfPoints[i].X) + (p1.Y - listOfPoints[i].Y) * (p1.Y - listOfPoints[i].Y);

                        if (distanceToOtherBricks < 10000)
                        {
                            tooCloseToOtherBricks = true;
                            break;

                        }
                        else
                        {
                            tooCloseToOtherBricks = false;
                        }
                    }

                    for (int i = 0; i < Snake.Count; i++)
                    {
                        int distanceToSnakeBody = (p1.X - Snake[i].X*Settings.Width) * (p1.X - Snake[i].X*Settings.Width) + (p1.Y - Snake[i].Y*Settings.Height) * (p1.Y - Snake[i].Y*Settings.Height);
                        
                        if (distanceToSnakeBody<10000)
                        {
                            tooCloseToSnakeBody = true;
                            break;
                        }
                        else
                        {
                            tooCloseToSnakeBody = false;
                        }
                    }

                }
                while (tooCloseToFood || tooCloseToOtherBricks || tooCloseToSnakeBody);


                newBrick.Location = p1;
                listOfPoints.Add(p1);

                Controls.Add(newBrick);


                newBrick.BackColor = Color.Brown;
                newBrick.BorderStyle = BorderStyle.FixedSingle;            
                newBrick.Show();
                newBrick.Height = 23;
                newBrick.Width = 55;

                newBrick.BringToFront();
            }


        }

        private void Die()
        {
            Settings.GameOver = true;
            listOfPoints.Clear();
            pbCanvas.BringToFront();
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
