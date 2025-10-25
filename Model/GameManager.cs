namespace SnakeGame.Model
{
    public class GameManager()
    {
        #region const definitions
        // grid
        private const int gridRows = 400;
        private const int gridColumns = 400;

        // snake
        private const int StartingLength = 3; // snake throws if this is greater than MaxLength const
        private const int MaxLength = 50; // placeholder - depends on the size of the grid
        private const Direction StartingDirection = Direction.Up;
        private static readonly (int X, int Y) startingCoords = (50, 50);
        #endregion

        #region constructing game objects
        public GameGrid Grid { get; init; } = new(gridRows, gridColumns);
        private Snake Snake { get; set; } = new(StartingLength, StartingDirection, startingCoords, MaxLength);
        public GameState GameState { get; init; } = new();
        #endregion

        #region main management methods 
        public void MoveSnake(Direction newDirection) // to rethink later if linked list is not a bad idea here
        {
            if (Math.Abs(Snake.Head.Facing - newDirection) == 2) // if u turn in opposite direction - 死ねええええ!!!!!
            {
                Snake.Die();
                GameState.Lose();
            }

            var oldHead = Snake.Head; // save old head

            PopAndGlue();
            UpdateCoordsAndDirection(newDirection, oldHead);
            UpdateGrid();
        }
        public void Run()
        {
            throw new NotImplementedException();
        }
        public void CheckForCollisions()
        {
            throw new NotImplementedException("");
        }

        #endregion main management methods

        #region MoveSnake() method helpers
        private void PopAndGlue()
        {
            var tail = Snake.Tail; // save tail

            Snake.Body.RemoveAt(Snake.CurrentLength - 1); // pop it from the body
            Snake.Head.IsHead = false; // update old head's flag

            Snake.Body.Insert(0, tail); // glue it to the front
            Snake.Head.IsHead = true; // and update new head's (old tail's) flag
        }
        private void UpdateCoordsAndDirection(Direction newDirection, Snake.SnakeSegment oldHead)
        {
            Snake.Head.Facing = newDirection;

            var (newX, newY) = newDirection switch
            {
                Direction.Up => (0, -1),
                Direction.Down => (0, +1),
                Direction.Right => (+1, 0),
                Direction.Left => (-1, 0),

                _ => throw new Exception($"Something unexpected happened, {nameof(newDirection)}'s value is {newDirection}")
            };

            Snake.Head.X = oldHead.X + newX;
            Snake.Head.Y = oldHead.Y + newY;
        }
        private void UpdateGrid()
        {
            throw new NotImplementedException();
        }
        
        #endregion
    }
}