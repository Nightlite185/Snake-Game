using System.Drawing;

namespace SnakeGame.Model
{
    public class GameGrid(int rows, int cols)
    {
        //public int Rows { get; } = rows;
        //public int Columns { get; } = cols;

        private readonly Square[,] grid = new Square[rows, cols];
        public void ClearGrid() => Array.Clear(grid);

        public Square this[int x, int y] // custom indexer
        {
            get => grid[y, x];
            set => grid[y, x] = value;
        }

        public class Square(object? item = null)
        {
            public Rectangle Visual { get; set; }
            public object? Contents { get; private set; } = item;
            public bool IsEmpty => Contents == null;
            public void ClearContents() => Contents = null;
            public void SetContents(object item) => Contents = item;
        }
    }
}