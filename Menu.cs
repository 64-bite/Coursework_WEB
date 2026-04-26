using System;
using System.Windows;

namespace Snake_game
{
    public partial class MainWindow : Window
    {
        private int snakeSkinIndex = 0;
        private readonly int maxSnakeSkins = 6;
        private int foodSkinIndex = 0;
        private readonly int maxFoodSkins = 6;
        public static bool IsFoodRandom { get; set; } = false;
        public static bool isBlindMode { get; set; } = false;
        public static int CurrentFoodCount { get; private set; } = 1;
        private readonly int maxFoodCount = 5;
        private readonly int minFoodCount = 1;
        private int mapSizeIndex = 1;
        private readonly int[] availableMapSizes = { 10, 15, 20, 25 };
        private void OpenMenuButton_Click(object sender, RoutedEventArgs e)
        {
            bool isMenuOpen = SkinSelectorMenu.Visibility == Visibility.Visible;
            SkinSelectorMenu.Visibility = isMenuOpen ? Visibility.Collapsed : Visibility.Visible;
            OpenMenuButton.Content = isMenuOpen ? "MENU" : "APPLY";
        }
        private int CycleIndex(int currentIndex, int step, int max) => (currentIndex + step + max) % max;
        private void BtnSnakeUp_Click(object sender, RoutedEventArgs e) { snakeSkinIndex = CycleIndex(snakeSkinIndex, -1, maxSnakeSkins); UpdateMenuDisplay(); }
        private void BtnSnakeDown_Click(object sender, RoutedEventArgs e) { snakeSkinIndex = CycleIndex(snakeSkinIndex, 1, maxSnakeSkins); UpdateMenuDisplay(); }
        private void BtnFoodUp_Click(object sender, RoutedEventArgs e) { foodSkinIndex = CycleIndex(foodSkinIndex, -1, maxFoodSkins); UpdateMenuDisplay(); }
        private void BtnFoodDown_Click(object sender, RoutedEventArgs e) { foodSkinIndex = CycleIndex(foodSkinIndex, 1, maxFoodSkins); UpdateMenuDisplay(); }
        private void BtnMapUp_Click(object sender, RoutedEventArgs e) { mapSizeIndex = CycleIndex(mapSizeIndex, -1, availableMapSizes.Length); ApplyMapSizeChange(); UpdateMenuDisplay(); }
        private void BtnMapDown_Click(object sender, RoutedEventArgs e) { mapSizeIndex = CycleIndex(mapSizeIndex, 1, availableMapSizes.Length); ApplyMapSizeChange(); UpdateMenuDisplay(); }
        private void BtnFoodCountUp_Click(object sender, RoutedEventArgs e) { CurrentFoodCount = Math.Clamp(CurrentFoodCount + 1, minFoodCount, maxFoodCount); UpdateMenuDisplay(); }
        private void BtnFoodCountDown_Click(object sender, RoutedEventArgs e) { CurrentFoodCount = Math.Clamp(CurrentFoodCount - 1, minFoodCount, maxFoodCount); UpdateMenuDisplay(); }
        private void BtnToggleBlindMode_Click(object sender, RoutedEventArgs e)
        {
            isBlindMode = !isBlindMode;
            BtnToggleBlindMode.Content = isBlindMode ? "BLIND" : "NORMAL";
            if (!gameRunning && !gameState.GameOver) Draw();
        }

        private void ApplyMapSizeChange()
        {
            rows = availableMapSizes[mapSizeIndex];
            cols = availableMapSizes[mapSizeIndex];
            GameGrid.Children.Clear();
            FogGrid.Children.Clear();
            gridImages = SetupGrid();
            gameState = new GameState(rows, cols);
        }

        private void ApplyFruitSkinChange()
        {
            IsFoodRandom = (foodSkinIndex == 5);
            Images.colorFruit = foodSkinIndex;

            if (FoodImageDisplay != null)
            {
                FoodImageDisplay.Visibility = Visibility.Visible;
                FoodImageDisplay.Source = IsFoodRandom ? Images.Rand_NoBorder : Images.ColorOfFood(1);
            }
            gridValToImage[GridValue.Food] = IsFoodRandom ? Images.Rand_NoBorder : Images.ColorOfFood(0);
        }
        private void UpdateMenuDisplay()
        {
            Images.colorBody = snakeSkinIndex;
            Images.colorHead = snakeSkinIndex;
            if (SnakeImageDisplay != null) SnakeImageDisplay.Source = Images.ColorOfHead(1);
            gridValToImage[GridValue.Snake] = Images.ColorOfBody();
            ApplyFruitSkinChange();
            if (FoodCountText != null) FoodCountText.Text = CurrentFoodCount.ToString();
            if (MapSizeText != null) MapSizeText.Text = $"{rows}x{cols}";
            if (!gameRunning && !gameState.GameOver) Draw();
        }
    }
}