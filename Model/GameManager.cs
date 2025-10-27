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
        private FoodPool FoodPool { get; set; } = new(maxCapacity: 8);
        #endregion

        #region main management methods 
        public void MoveSnake(Direction newDirection) // LINKED LIST IS A MUSTTT. MAKE ITTTT PLSSS LOLLL
        {
            if (Math.Abs(Snake.Head.Facing - newDirection) == 2) // if u turn in opposite direction - 死ねええええ!!!!!
            {
                Snake.Die();
                GameState.Lose();
            }
            
            var tail = Snake.Tail; // save tail
            var oldHead = Snake.Head; // save old head

            var tailSquare = Grid[(tail.Y, tail.X)]
                ?? throw new IndexOutOfRangeException($"tail's coords are out of grid's bounds. tail's Y = {tail.Y}, and X= {tail.X}");
            
            tailSquare.ClearSnake();

            PopAndGlue(tail);
            var newHeadSquare = Grid.GetNextSquare((oldHead.X, oldHead.Y), newDirection, out (int X, int Y) newCoords);

            if (newHeadSquare == null)
            {
                Snake.Die();
                GameState.Lose(); // gotta wrap this Lose into a bigger method later that handles losing and objects cleanup.
                return;
            }

            Snake.Head.Facing = newDirection;
            Snake.Head.X = oldHead.X + newCoords.X;
            Snake.Head.Y = oldHead.Y + newCoords.Y;

            newHeadSquare.AddSnake(Snake.Head);
        }
        public void RunGame()
        {
            throw new NotImplementedException();

            // gotta handle collisions before updating the grid with new snake pos (check them)

            // also remember to update grid EVERY TIME SNAKE EATS FOOD
        }
        private void LoseGame()
        {
            GameState.Lose();
            
        }
        public bool CheckForCollisions(Direction newDirection)
        {
            GameGrid.Square? square = Grid.GetNextSquare((Snake.Head.X, Snake.Head.Y), newDirection, out _);

            return square?.HasSnake ?? true;
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
        private void PopAndGlue(Snake.SnakeSegment tail) // TO DO: MOVE THIS TO SNAKE'S METHODS SINCE IT SHOULD MANAGE THAT ON ITS OWN. 
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
            
        #endregion
    }
}