namespace SnakeGame.Model
{
    public class GameManager
    {
        public GameManager(Settings cfg)
        {
            InitGameObjects(cfg);
            UpdateOptions(cfg);

            State = new();
        }
        #region ViewModel public API
        public event Action? OnScoreChange;
        public event Action? OnIteration;
        public event Action<int>? GotFinalScore;
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
        public Direction QueuedDirection { get; set; }
        #endregion
        
        #region private fields
        // general
        private int TickLength;
        private int MaxScore;

        // snake
        private int MaxSnakeLength;

        //Food
        private int FoodSpawningFrequency;
        private int MaxActiveFoods;
        #endregion

        #region Game Objects
        public GameGrid Grid { get; private set; } = null!; // STFU ROSLYN ITS NEVER NULL IN .CTOR
        public Snake Snake { get; private set; } = null!; // same here
        public GameState State { get; init; } 
        public FoodPool FoodPool { get; private set; } = null!; // same here
        private static readonly Random rand = new();
        #endregion

        #region main management methods
        private bool SafelyMoveSnake(Direction newDirection) // the bool returned means whether it survived this move.
        {
            if (Math.Abs(Snake.Head.Facing - newDirection) == 2) // if u turn in opposite direction - 死ねええええ!!!!!
                return false;

            var tailSquare = Grid[Snake.TailPos]
                ?? throw new IndexOutOfRangeException($"tail's coords are out of grid's bounds. Their values: {Snake.TailPos}");
            
            tailSquare.ClearSnake();

            var newHeadSquare = Grid.GetNextSquare(Snake.HeadPos, newDirection);

            if (newHeadSquare == null || newHeadSquare.HasSnake) // null here means wall collision --> game over.
                return false;

            else
            {
                Snake.Move(newHeadSquare.Coords, newDirection); // actually moving the snake
                newHeadSquare.AddSnake(Snake.Head); // updating the grid
                return true;
            }
        }
        public async Task RunGameAsync()
        {
            AddFirstSnakeToGrid();

            State.Start();
            int i = 0;
            OnIteration?.Invoke(); // first canvas initialization (before game loop)

            while (State.CurrentState == GameStates.Running && Snake.Alive)
            {
                i++;

                Direction inputDirection = QueuedDirection;

                EatIfHasFood();

                if (SafelyMoveSnake(inputDirection) == false) // * if snake didnt make it ;(
                {
                    LoseGame();
                    return;
                }

                if (CheckForWin())
                {
                    State.Win();

                    if (Score > 0) 
                        GotFinalScore?.Invoke(Score);
                }

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

            if (Score > 0)
                GotFinalScore?.Invoke(Score);
        }
        public void RestartGame(Settings cfg)
        {
            State.Restart();

            if (Snake.Alive && State.CurrentState == GameStates.Running)
                Snake.Die();

            InitGameObjects(cfg);

            this.QueuedDirection = cfg.Snake.StartingDirection;
            
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
                ?? throw new IndexOutOfRangeException($"snake's head cannot be inside a wall. Current HeadPos: {Snake.HeadPos}");
            
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
                int when Score > MaxSnakeLength => throw new Exception($"snake's length cannot exceed grid's size. current state - {State.CurrentState}, score = {Score}, snake's length = {Snake.CurrentLength}"),
                int when Score == MaxScore && State.CurrentState == GameStates.Running => true,

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
        private void UpdateOptions(Settings cfg)
        {
            QueuedDirection = cfg.Snake.StartingDirection;
            MaxSnakeLength = cfg.Grid.Rows * cfg.Grid.Columns;
            MaxScore = MaxSnakeLength - cfg.Snake.StartingLength;

            FoodSpawningFrequency = cfg.General.FoodSpawningFrequency;
            
            MaxActiveFoods = cfg.General.MaxActiveFoods;
            TickLength = cfg.General.TickLength;
        }
        private void InitGameObjects(Settings cfg)
        {
            UpdateOptions(cfg);

            Coords snakeStartingCoords = new(cfg.Grid.Rows / 2, cfg.Grid.Columns / 2);
            
            Grid = new(cfg.Grid);
            Snake = new(cfg.Snake, snakeStartingCoords);
            FoodPool = new(cfg.General);
        }
        #endregion
    }
}