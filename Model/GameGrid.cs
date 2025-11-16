using System.Collections;

namespace SnakeGame.Model
{
    public class GameGrid : IEnumerable<GameGrid.Square>
    {
        public GameGrid(Settings.GridSettings cfg)
        {
            RowCount = cfg.Rows;
            ColCount = cfg.Columns;

            grid = new Square[RowCount, ColCount];
            
            InitializeGrid();
        }
        private readonly int RowCount;
        private readonly int ColCount;
        private readonly Square[,] grid;
        public void ClearGrid() => Array.Clear(grid);
        public Square? GetNextSquare(Coords coords, Direction direction) // returns null if its a wall.
        { // this method could return bool signalizing success, and have an out param that returns the actual square, since its nullable.
            coords = direction switch
            {
                Direction.Up => new(coords.Row - 1, coords.Col),
                Direction.Down => new(coords.Row + 1, coords.Col),
                Direction.Right => new(coords.Row, coords.Col + 1),
                Direction.Left => new(coords.Row, coords.Col - 1),

                _ => throw new Exception($"Unexpected value of {nameof(direction)} - '{direction}'")
            };

            return this[coords];
        }
        private void InitializeGrid()
        {
            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColCount; col++)
                {
                    grid[row, col] = new Square(new Coords(row, col));
                }
            }
        }
        public IEnumerator<Square> GetEnumerator()
        {
            foreach (var square in grid)
                yield return square;
        }
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Square? this[Coords coords] // custom indexer, returns null if its a wall in getter, throws in setter.
        {
            get
            {
               if (coords.Row < 0 || coords.Col < 0 || coords.Row >= RowCount || coords.Col >= ColCount)
                    return null; // null means wall, so its meaningful.
               
                return grid[coords.Row, coords.Col];
            }
            set
            {
                if (coords.Row < 0 || coords.Col < 0 || coords.Row >= RowCount || coords.Col >= ColCount)
                    throw new IndexOutOfRangeException($"Given coords are not inside the grid's bounds, fix this. current coords: {coords}");
                
                grid[coords.Row, coords.Col] = value!;
            }
        }

        public class Square(Coords coords)
        {
            public Coords Coords { get; init; } = coords;
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