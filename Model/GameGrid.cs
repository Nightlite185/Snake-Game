using System.Collections;
using System.Drawing;

namespace SnakeGame.Model
{
    public class GameGrid(int rows, int cols) : IEnumerable<GameGrid.Square>
    {
        private readonly Square[,] grid = new Square[rows, cols];
        public void ClearGrid() => Array.Clear(grid);

        public IEnumerator<Square> GetEnumerator()
        {
            foreach (var square in grid)
                yield return square;
        }
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Square this[(int row, int col) coords] // custom indexer
        {
            get => grid[coords.row, coords.col];
            set => grid[coords.row, coords.col] = value;
        }

        public class Square
        {
            public Rectangle Visual { get; set; }
            public Snake.SnakeSegment? SnakeContents { get; set; }
            public Food? FoodContents { get; set; }
            public bool HasSnake => SnakeContents != null;
            public bool HasFood => FoodContents != null;
            public void ClearFood() => FoodContents = null;
            public void ClearSnake() => SnakeContents = null;
            public void AddFood(Food food) => FoodContents = food;
            public void AddSnake(Snake.SnakeSegment snake) => SnakeContents = snake;
        }
    }
}