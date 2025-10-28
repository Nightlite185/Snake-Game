namespace SnakeGame.Model
{
    public class GameManager()
    {
        #region const definitions
        // grid
        private const int gridRows = 20;
        private const int gridColumns = 30;

        // snake
        private const int StartingLength = 3; // snake throws if this is greater than MaxLength const
        private const int MaxSnakeLength = gridRows * gridColumns; // placeholder - depends on the size of the grid
        private const Direction StartingDirection = Direction.Up;
        private static readonly (int X, int Y) startingCoords = (10, 10);

        //Food Pool
        private const int FoodPoolMaxCapacity = 8;
        #endregion

        #region constructing game objects
        public GameGrid Grid { get; init; } = new(gridRows, gridColumns);
        private Snake Snake { get; set; } = new(StartingLength, StartingDirection, startingCoords, MaxSnakeLength);
        public GameState GameState { get; init; } = new();
        private FoodPool FoodPool { get; set; } = new(FoodPoolMaxCapacity);
        #endregion

        #region main management methods 
        public void SafelyMoveSnake(Direction newDirection)
        {
            if (Math.Abs(Snake.Head.Facing - newDirection) == 2) // if u turn in opposite direction - 死ねええええ!!!!!
            {
                Snake.Die();
                GameState.Lose();
            }
            
            var tailSquare = Grid[Snake.TailPos]
                ?? throw new IndexOutOfRangeException($"tail's coords are out of grid's bounds. Their values: {Snake.TailPos}");
            
            tailSquare.ClearSnake();

            var newHeadSquare = Grid.GetNextSquare(Snake.HeadPos, newDirection);

            if (newHeadSquare == null) // null here means wall collision --> game over.
                LoseGame();

            else
            {
                Snake.Move(newHeadSquare.Coords, newDirection); // actually moving the snake
                newHeadSquare.AddSnake(Snake.Head); // updating the grid
            }
                
        }
        public void RunGame()
        {
            throw new NotImplementedException();

            // gotta handle collisions before updating the grid with new snake pos (check them)

            // also remember to update grid EVERY TIME SNAKE EATS FOOD
        }
        private void LoseGame()
        {
            Snake.Die();
            GameState.Lose();
            // to add more later
        }
        public void SpawnRandomFood()
        {
            var emptySquares = Grid.Where(x => !x.HasSnake && !x.HasFood);

            if (!emptySquares.Any())
                throw new InvalidOperationException($"Cannot spawn food when grid is full. Current state - {GameState.CurrentState}");
                
            var rand = new Random();
            (int row, int col) randomCoords = (rand.Next(gridRows - 1), rand.Next(gridColumns - 1));

            var food = FoodPool.Pop(randomCoords);

            var foodSquare = Grid[randomCoords] ??
                throw new IndexOutOfRangeException($"cannot spawn food inside a wall. The coords given are: row= {randomCoords.row}, col= {randomCoords.col}");

            foodSquare.AddFood(food);
        }
        private void EatIfHasFood() 
        {
            Food food;
            var here = Grid[(Snake.Head.Y, Snake.Head.X)]
                ?? throw new IndexOutOfRangeException($"snake's head cannot be inside a wall, fix me. X = {Snake.Head.X}, Y= {Snake.Head.Y}");
            
            if (!here.HasFood) return;

            food = here.TakeFood()!;
            Snake.Eat();
            FoodPool.ReturnToPool(food);
            // increment the game score here (this for later)
        }
        #endregion main management methods

        #region private helper methods

        private bool CheckForWin() 
            => Snake.CurrentLength switch
            {
                > MaxSnakeLength => throw new Exception($"snake's length cannot exceed grid's size, fix me. current state - {GameState.CurrentState}"),
                MaxSnakeLength => true,

                _ => false
            };
            
        #endregion
    }
}