using System.Collections;
using System.Drawing;

namespace SnakeGame.Model
{
    public class GameGrid(int rows, int cols) : IEnumerable<GameGrid.Square>
    {
        private readonly int RowCount = rows;
        private readonly int ColCount = cols;
        private readonly Square[,] grid = new Square[rows, cols];
        public void ClearGrid() => Array.Clear(grid);
        public Square? GetNextSquare((int X, int Y) coords, Direction direction, out (int X, int Y) newCoords) // returns null if its a wall.
        { // this method could return bool signalizing success, and have an out param that returns the actual square, since its nullable.
            newCoords = direction switch
            {
                Direction.Up => (coords.X, coords.Y - 1),
                Direction.Down => (coords.X, coords.Y + 1),
                Direction.Right => (coords.X + 1, coords.Y),
                Direction.Left => (coords.X - 1, coords.Y),

                _ => throw new Exception($"Unexpected value of {nameof(direction)} - '{direction}'")
            };

            return this[newCoords];
        }

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
            public Snake.SnakeSegment? SnakeContents { get; set; }
            public Food? FoodContents { get; set; }
            public bool HasSnake => SnakeContents != null;
            public bool HasFood => FoodContents != null;
            public Food? TakeFood()
            {
                var food = FoodContents;
                FoodContents = null;
                
                return food;
            }
            public void ClearSnake() => SnakeContents = null;
            public void AddFood(Food food) => FoodContents = food;
            public void AddSnake(Snake.SnakeSegment snake) => SnakeContents = snake;
        }
    }
}