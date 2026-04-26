using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Snake_game
{
    public partial class MainWindow : Window
    {
        private int rows = 15, cols = 15;
        private Image[,] gridImages;
        private Image[,] fogImages;
        private ImageSource[,] activeFoodSkins;
        private GameState gameState;
        private bool gameRunning;
        private int currentDelay = 120;

        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, null },
            { GridValue.Food, null }
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            { Direction.Up, 0 }, { Direction.Right, 90 },
            { Direction.Down, 180 }, { Direction.Left, 270 }
        };

        public MainWindow()
        {
            InitializeComponent();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
            UpdateMenuDisplay();
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();

            if (gameState.Score == rows * cols - 1) await ShowVictory();
            else await ShowGameOver();

            gameState = new GameState(rows, cols);
        }

        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible || SkinSelectorMenu.Visibility == Visibility.Visible)
                e.Handled = true;

            if (!gameRunning && SkinSelectorMenu.Visibility != Visibility.Visible)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver || SkinSelectorMenu.Visibility == Visibility.Visible) return;

            switch (e.Key)
            {
                case Key.Up: gameState.ChangeDirection(Direction.Up); break;
                case Key.Down: gameState.ChangeDirection(Direction.Down); break;
                case Key.Left: gameState.ChangeDirection(Direction.Left); break;
                case Key.Right: gameState.ChangeDirection(Direction.Right); break;
            }
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            currentDelay = (int)e.NewValue;
            if (SpeedValueText != null) SpeedValueText.Text = $"{currentDelay} ms";
        }

        private void ApplyFruitCount()
        {
            int maxAttempts = 10;
            int attempts = 0;
            while (gameState.FoodCount() < CurrentFoodCount && attempts < maxAttempts)
            {
                int countBefore = gameState.FoodCount();
                gameState.AddFood();
                if (gameState.FoodCount() == countBefore) break;
                attempts++;
            }
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                ApplyFruitCount();
                await Task.Delay(currentDelay);
                gameState.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            activeFoodSkins = new ImageSource[rows, cols];
            fogImages = new Image[rows, cols];

            GameGrid.Rows = rows; GameGrid.Columns = cols;
            FogGrid.Rows = rows; FogGrid.Columns = cols;

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image img = new Image { Source = Images.Empty, RenderTransformOrigin = new Point(0.5, 0.5) };
                    images[r, c] = img;
                    GameGrid.Children.Add(img);

                    Image fogImg = new Image { Stretch = Stretch.Fill, Visibility = Visibility.Hidden };
                    fogImages[r, c] = fogImg;
                    FogGrid.Children.Add(fogImg);
                }
            }
            return images;
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            DrawFog();
            ScoreText.Text = isBlindMode ? $"Blind Score {gameState.Score}" : $"Score {gameState.Score}";
        }

        private void DrawGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    Image imgControl = gridImages[r, c];

                    if (gridVal == GridValue.Food)
                    {
                        if (IsFoodRandom)
                        {
                            if (activeFoodSkins[r, c] == null)
                            {
                                Images.SetRandomFoodSkin();
                                activeFoodSkins[r, c] = Images.ColorOfFood(0);
                            }
                            imgControl.Source = activeFoodSkins[r, c];
                        }
                        else imgControl.Source = gridValToImage[GridValue.Food];
                    }
                    else
                    {
                        activeFoodSkins[r, c] = null;
                        imgControl.Source = gridValToImage[gridVal];
                    }
                    imgControl.RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.ColorOfHead(0);
            image.RenderTransform = new RotateTransform(dirToRotation[gameState.Dir]);
        }

        private void DrawFog()
        {
            if (!isBlindMode || gameState.GameOver)
            {
                for (int r = 0; r < rows; r++)
                    for (int c = 0; c < cols; c++)
                        fogImages[r, c].Visibility = Visibility.Hidden;
                return;
            }

            Position head = gameState.HeadPosition();
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    int distance = Math.Max(Math.Abs(r - head.Row), Math.Abs(c - head.Col));
                    fogImages[r, c].Visibility = Visibility.Visible;

                    if (distance <= 1) fogImages[r, c].Visibility = Visibility.Hidden;
                    else if (distance <= 3) fogImages[r, c].Source = Images.FogLight;
                    else fogImages[r, c].Source = Images.FogBlack;
                }
            }
        }

        private async Task DrawDeadSnake()
        {
            var positions = new List<Position>(gameState.SnakePositions());
            for (int i = 0; i < positions.Count; i++)
            {
                gridImages[positions[i].Row, positions[i].Col].Source = (i == 0) ? Images.ColorOfDeadHead() : Images.ColorOfDeadBody();
                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver() { await DrawDeadSnake(); Overlay.Visibility = Visibility.Visible; OverlayText.Text = "Game Over!\nPRESS ANY KEY TO START"; }
        private async Task ShowVictory() { Overlay.Visibility = Visibility.Visible; OverlayText.Text = "Victory!\nPRESS ANY KEY TO START"; await Task.Delay(100); }
    }
}