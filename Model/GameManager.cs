namespace SnakeGame.Model
{
    public class GameManager()
    {
        #region ViewModel public API
        public event Action? OnScoreChange;
        public event Action? OnIteration;
        public int Score
        { 
            get;
            private set
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxScore);
                ArgumentOutOfRangeException.ThrowIfNegative(value);

                field = value;

                OnScoreChange?.Invoke();
            }
        }
        public Direction QueuedDirection { get; set; } = StartingDirection;
        #endregion
        
        #region const definitions
        public const int TickLength = 300;
        private const int MaxScore = MaxSnakeLength - StartingLength;
        // grid
        public const int gridRows = 15;
        public const int gridColumns = 15;

        // snake
        private const int StartingLength = 3; // snake throws if this is greater than MaxLength const
        private const int MaxSnakeLength = gridRows * gridColumns;

        private const Direction StartingDirection = Direction.Up;
        private static readonly Coords startingCoords = new(gridRows/2, gridColumns/2); // exactly in the middle

        //Food
        private const int FoodPoolMaxCapacity = 8;
        private const int FoodSpawningFrequency = 3;
        private const int MaxActiveFoods = 7;
        #endregion

        #region constructing game objects
        public GameGrid Grid { get; private set; } = new(gridRows, gridColumns);
        public Snake Snake { get; set; } = new(StartingLength, StartingDirection, startingCoords, MaxSnakeLength);
        public GameState State { get; init; } = new();
        public FoodPool FoodPool { get; private set; } = new(FoodPoolMaxCapacity, MaxActiveFoods);
        private static readonly Random rand = new();
        #endregion

        #region main management methods
        private void SafelyMoveSnake(Direction newDirection)
        {
            if (Math.Abs(Snake.Head.Facing - newDirection) == 2) // if u turn in opposite direction - 死ねええええ!!!!!
            {
                Snake.Die();
                State.Lose();
                return;
            }

            var tailSquare = Grid[Snake.TailPos]
                ?? throw new IndexOutOfRangeException($"tail's coords are out of grid's bounds. Their values: {Snake.TailPos}");
            
            tailSquare.ClearSnake();

            var newHeadSquare = Grid.GetNextSquare(Snake.HeadPos, newDirection);

            if (newHeadSquare == null || newHeadSquare.HasSnake) // null here means wall collision --> game over.
                LoseGame();

            else
            {
                Snake.Move(newHeadSquare.Coords, newDirection); // actually moving the snake
                newHeadSquare.AddSnake(Snake.Head); // updating the grid
            }
        }
        public async Task RunGameAsync()
        {
            AddFirstSnakeToGrid();

            State.Start();
            int i = 0;
            OnIteration?.Invoke();

            while (State.CurrentState == GameStates.Running && Snake.Alive)
            {
                i++;

                Direction inputDirection = QueuedDirection;

                EatIfHasFood();

                SafelyMoveSnake(inputDirection);

                if (CheckForWin())
                    State.Win();

                if (i % FoodSpawningFrequency == 0 && FoodPool.ActiveCount < MaxActiveFoods)
                    SpawnRandomFood();

                OnIteration?.Invoke();

                await Task.Delay(TickLength);
            }
        }
        private void LoseGame()
        {
            Snake.Die();
            State.Lose();
        }
        public void RestartGame()
        {
            State.Restart();

            if (Snake.Alive && State.CurrentState == GameStates.Running)
                Snake.Die();

            RestartGameObjects();

            this.QueuedDirection = StartingDirection;

            Score = 0;
        }
        private void SpawnRandomFood()
        {
            var emptySquares = Grid.Where(x => !x.HasSnake && !x.HasFood).ToArray();

            if (emptySquares.Length == 0)
                throw new InvalidOperationException($"Cannot spawn food when grid is full. Current state - {State.CurrentState}");

            GameGrid.Square randomSquare = emptySquares[rand.Next(emptySquares.Length)];
            
            var food = FoodPool.Get(randomSquare.Coords);

            randomSquare.AddFood(food);
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
        private void AddFirstSnakeToGrid()
        {
            foreach (var seg in Snake.Body)
            {
                var segSquare = Grid[seg.Coords]
                    ?? throw new IndexOutOfRangeException("square that has a snake segment is null or out of bounds.");

                segSquare.AddSnake(seg);
            }
        }
        private void RestartGameObjects() // this can be reused later in a proper constructor if I wanna later switch from primary to normal one.
        {
            Grid = new(gridRows, gridColumns);
            Snake = new(StartingLength, StartingDirection, startingCoords, MaxSnakeLength);
            FoodPool = new(FoodPoolMaxCapacity, MaxActiveFoods);
        }
        #endregion
    }

    public class ScoreEntry(int score, string nick)
    {
        public int Score { get; set; } = score;
        public string Nickname { get; set; } = nick;
        public DateTime Time { get; set; } = DateTime.Now;
    }
}