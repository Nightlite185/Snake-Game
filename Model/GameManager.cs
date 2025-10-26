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
        private static readonly (int X, int Y) startingCoords = (50, 50);

        //Food Pool
        private const int FoodPoolMaxCapacity = 8;
        #endregion

        #region constructing game objects
        public GameGrid Grid { get; init; } = new(gridRows, gridColumns);
        private Snake Snake { get; set; } = new(StartingLength, StartingDirection, startingCoords, MaxSnakeLength);
        public GameState GameState { get; init; } = new();
        private FoodPool FoodPool { get; set; } = new(maxCapacity: 8);
        #endregion

        #region main management methods 
        public void MoveSnake(Direction newDirection) // to rethink later if linked list is not a bad idea here
        {
            if (Math.Abs(Snake.Head.Facing - newDirection) == 2) // if u turn in opposite direction - 死ねええええ!!!!!
            {
                Snake.Die();
                GameState.Lose();
            }
            
            var tail = Snake.Tail; // save tail
            var oldHead = Snake.Head; // save old head

            Grid[(tail.Y, tail.X)].ClearSnake();

            PopAndGlue(tail);
            var (newX, newY) = GetNextSquare((oldHead.X, oldHead.Y), newDirection);

            Snake.Head.Facing = newDirection;
            Snake.Head.X = oldHead.X + newX;
            Snake.Head.Y = oldHead.Y + newY;

            Grid[(Snake.Head.Y, Snake.Head.X)].AddSnake(Snake.Head);
        }
        public void RunGame()
        {
            throw new NotImplementedException();

            // gotta handle collisions before updating the grid with new snake pos (check them)

            // also remember to update grid EVERY TIME SNAKE EATS FOOD
        }
        public bool CheckForCollisions(Direction newDirection)
        {
            (int nextX, int nextY) = GetNextSquare((Snake.Head.X, Snake.Head.Y), newDirection);

            return Grid[(nextY, nextX)].HasSnake;
        }
        public void SpawnRandomFood()
        {
            var emptySquares = Grid.Where(x => !x.HasSnake && !x.HasFood);

            if (!emptySquares.Any())
                throw new InvalidOperationException($"Cannot spawn food when grid is full. Current state - {GameState.CurrentState}");
                
            var rand = new Random();
            (int row, int col) randomCoords = (rand.Next(), rand.Next());

            var food = FoodPool.PopAndAssign(randomCoords);

            Grid[randomCoords].AddFood(food);
        }
        private void EatIfHasFood()
        {
            Food food;
            var here = Grid[(Snake.Head.Y, Snake.Head.X)];

            if (!here.HasFood) return;   

            food = here.FoodEaten()!;
            FoodPool.ReturnToPool(food);
        }
        #endregion main management methods

        #region private helper methods
        private void PopAndGlue(Snake.SnakeSegment tail)
        {
            Snake.Body.RemoveAt(Snake.CurrentLength - 1); // pop tail from the body
            Snake.Head.IsHead = false; // update old head's flag

            Snake.Body.Insert(0, tail); // glue it to the front
            Snake.Head.IsHead = true; // and update new head's (old tail's) flag
        }
        private bool CheckForWin() 
            => Snake.CurrentLength switch
            {
                > MaxSnakeLength => throw new Exception($"snake's length cannot exceed grid's size, fix me. current state - {GameState.CurrentState}"),
                MaxSnakeLength => true,

                _ => false
            };
        private static (int newX, int newY) GetNextSquare((int X, int Y) coords, Direction direction)
            => direction switch
            {
                Direction.Up => (coords.X, coords.Y -1),
                Direction.Down => (coords.X, coords.Y +1),
                Direction.Right => (coords.X +1, coords.Y),
                Direction.Left => (coords.X -1, coords.Y),

                _ => throw new Exception($"Something unexpected happened, {nameof(direction)}'s value is {direction}")
            };
            
        #endregion
    }
}