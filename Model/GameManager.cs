namespace SnakeGame.Model
{
    public class GameManager
    {
        public GameManager(Settings cfg, GameState GS)
        {
            InitFields(cfg);
            InitGameObjects(cfg, GS);
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

        // food
        private int FoodSpawningFrequency;
        private int MaxActiveFoods;
        #endregion

        #region Game Objects
        public GameGrid Grid { get; private set; } = null!; // STFU ROSLYN ITS NEVER NULL IN .CTOR
        public Snake Snake { get; private set; } = null!; // same here
        public GameState State { get; private set; } = null!;
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

            while (State.Current == GameStates.Running && Snake.Alive)
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
        private void SpawnRandomFood()
        {
            var emptySquares = Grid.Where(x => !x.HasSnake && !x.HasFood).ToArray();

            if (emptySquares.Length == 0) return; // quick return if the grid is already full.

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
                int when Score > MaxSnakeLength => throw new Exception($"snake's length cannot exceed grid's size. current state - {State.Current}, score = {Score}, snake's length = {Snake.CurrentLength}"),
                int when Score == MaxScore && State.Current == GameStates.Running => true,

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
        private void InitFields(Settings cfg)
        {
            MaxSnakeLength = cfg.Grid.Rows * cfg.Grid.Columns;
            MaxScore = MaxSnakeLength - cfg.Snake.StartingLength;

            FoodSpawningFrequency = InvertFoodFreq(cfg.Food.FoodSpawnFreq);
            
            MaxActiveFoods = cfg.Food.MaxActiveFoods;
            TickLength = SpeedToTick(cfg.Snake.Speed);
        }
        private static int InvertFoodFreq(int wrong) => Math.Max(10 - wrong, 1);
        private static int SpeedToTick(int speed) => 550 - (speed * 5);
        /// <summary> Determines snake's starting Coords and sets first QueuedDirection as a side effect. </summary>
        /// <returns> starting Coords </returns>
        private Coords GetFirstHeadPos(Settings.GridSettings g, int startingLength)
        {
            if (startingLength <= (g.Rows - 2)) // prioritizing vertical spawn
            {
                QueuedDirection = Direction.Up;
                return new Coords(g.Rows - 1 - startingLength, g.Columns / 2); // starting at bottom middle, going up
            }

            else if (startingLength <= (g.Columns - 2))
            {
                QueuedDirection = Direction.Right;
                return new Coords(g.Rows / 2, startingLength); // starting at middle left, going right
            }

            // this should never happen bc of settings validation, but just in case
            else throw new ArgumentOutOfRangeException(nameof(startingLength), "Grid is too small for given snake's starting length.");
        }
        private void InitGameObjects(Settings cfg, GameState gs)
        {
            Coords startingPos = GetFirstHeadPos(cfg.Grid, cfg.Snake.StartingLength);

            State = gs;
            Grid = new(cfg.Grid);
            Snake = new(cfg.Snake, startingPos, QueuedDirection);
            FoodPool = new(cfg.Food);
        }
        #endregion
    }
}