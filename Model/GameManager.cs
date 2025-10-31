namespace SnakeGame.Model
{
    public class GameManager()
    {
        #region ViewModel public API
        public event Action? OnScoreChanged;
        public event Action? OnIterationEnd;
        public int Score
        { 
            get;
            private set
            {
                field = value switch
                {
                    > MaxScore => throw new ArgumentOutOfRangeException("Score cannot be higher than the grid's square count."),
                    < 0 => throw new ArgumentOutOfRangeException("Score cannot be lower than 0"),

                    _ => value
                };

                OnScoreChanged?.Invoke();
            }
        }
        public Direction QueuedDirection { get; set; } = StartingDirection;
        #endregion
        
        #region const definitions
        public const int TickLength = 200;
        private const int MaxScore = MaxSnakeLength - StartingLength;
        // grid
        public const int gridRows = 20;
        public const int gridColumns = 20;

        // snake
        private const int StartingLength = 3; // snake throws if this is greater than MaxLength const
        private const int MaxSnakeLength = gridRows * gridColumns;

        private const Direction StartingDirection = Direction.Up;
        private static readonly Coords startingCoords = new(10, 10);

        //Food
        private const int FoodPoolMaxCapacity = 8;
        private const int FoodSpawningFrequency = 1;
        #endregion

        #region constructing game objects
        public GameGrid Grid { get; init; } = new(gridRows, gridColumns);
        public Snake Snake { get; set; } = new(StartingLength, StartingDirection, startingCoords, MaxSnakeLength);
        public GameState State { get; init; } = new();
        public FoodPool FoodPool { get; init; } = new(FoodPoolMaxCapacity);
        #endregion

        #region main management methods
        public void SafelyMoveSnake(Direction newDirection)
        {
            if (Math.Abs(Snake.Head.Facing - newDirection) == 2) // if u turn in opposite direction - 死ねええええ!!!!!
            {
                Snake.Die();
                State.Lose();
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
        public async Task RunGameAsync()
        {
            State.Start();
            int i = 0;
            OnIterationEnd?.Invoke();

            while (State.CurrentState == GameStates.Running)
            {
                i++;

                Direction inputDirection = QueuedDirection;

                EatIfHasFood();

                SafelyMoveSnake(inputDirection);

                if (CheckForWin())
                    WinGame();

                if (i % FoodSpawningFrequency == 0)
                    SpawnRandomFood();

                OnIterationEnd?.Invoke();

                await Task.Delay(TickLength);
            }
        }
        private void LoseGame()
        {
            Snake.Die();
            State.Lose();
            Score = 0;
            // to add more later
        }
        private void WinGame()
        {
            State.Win();
            // to add more later on
        }
        public void SpawnRandomFood()
        {
            var emptySquares = Grid.Where(x => !x.HasSnake && !x.HasFood);

            if (!emptySquares.Any())
                throw new InvalidOperationException($"Cannot spawn food when grid is full. Current state - {State.CurrentState}");
                
            var rand = new Random();
            Coords randomCoords = new(rand.Next(gridRows - 1), rand.Next(gridColumns - 1));

            var foodSquare = Grid[randomCoords] ??
                throw new IndexOutOfRangeException($"cannot spawn food inside a wall. The coords given are: {randomCoords}");
            
            var food = FoodPool.Get(randomCoords);

            foodSquare.AddFood(food);
        }
        private void EatIfHasFood() 
        {
            Food food;
            var here = Grid[Snake.HeadPos]
                ?? throw new IndexOutOfRangeException($"snake's head cannot be inside a wall, fix me. Current HeadPos: {Snake.HeadPos}");
            
            if (!here.HasFood) return;

            food = here.TakeFood()!;
            Snake.Eat();
            FoodPool.ReturnToPool(food);

            var tailSquare = Grid[Snake.TailPos]
                ?? throw new IndexOutOfRangeException($"Snake's tail is out of bounds. Its coords: {Snake.TailPos}");
            
            tailSquare.AddSnake(Snake.Tail); // updating the grid with freshly grown tail.

            this.Score++;
        }
        #endregion main management methods

        #region private helper methods
        
        private bool CheckForWin() 
            => Score switch
            {
                > MaxSnakeLength => throw new Exception($"snake's length cannot exceed grid's size. current state - {State.CurrentState}, score = {Score}, snake's length = {Snake.CurrentLength}"),
                MaxScore when State.CurrentState == GameStates.Running => true,

                _ => false
            };
            
        #endregion
    }
}