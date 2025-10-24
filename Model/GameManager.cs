namespace SnakeGame.Model
{
    public class GameManager()
    {
        #region const definitions
        // grid
        private const int gridRows = 400;
        private const int gridColumns = 400;

        // snake
        private const int StartingLength = 3;
        private const Direction StartingDirection = Direction.Up;
        private static readonly (int X, int Y) startingCoords = (50, 50);
        #endregion

        //constructing game objects
        public GameGrid Grid { get; init; } = new(gridRows, gridColumns);
        private Snake Snake { get; set; } = new(StartingLength, StartingDirection, startingCoords);
        private GameState GameState { get; init; } = new();
        
        public void MoveSnake(Direction newDirection)
        {
            if (Math.Abs(Snake.Head.Facing - newDirection) == 2) // if u turn in opposite direction - 死ねええええ!!!!!
            {
                Snake.Die();
                GameState.Lose();
            }

            var tail = Snake.Tail; // save tail
            Snake.Body.RemoveAt(Snake.CurrentLength - 1); // pop it from body

            Snake.Head.IsHead = false; // update old head's flag
            Snake.Body.Insert(0, tail); // and glue it to the front
            Snake.Head.IsHead = true; // and update new head's (old tail's) flag
        }
    }
}