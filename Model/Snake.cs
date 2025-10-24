namespace SnakeGame.Model
{
    public enum Direction { Left, Right, Up, Down }
    class Snake
    {
        private const int maxLength = 50; // this is a placeholder, to change later. It depends on the number of squares in the grid.
        public Snake(int startingLength, Direction direction, (int X, int Y) startingCoords)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(startingLength, 2); // cannot be less than two bc we are adding head first, outside of the loop.
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startingLength, maxLength);

            SnakeSegments.Add(new Head(startingCoords.X, startingCoords.Y, direction));

            for (int i = 1; i < startingLength - 1; i++)
            {
                //setting X next coord depending on the direction
                int nextX = direction switch
                {
                    Direction.Left => startingCoords.X + i, 
                    Direction.Right => startingCoords.X - i, 

                    _ => startingCoords.X
                };
                
                //setting Y next coord depending on the direction
                int nextY = direction switch
                {
                    Direction.Up => startingCoords.Y - i, 
                    Direction.Down => startingCoords.Y + i, 

                    _ => startingCoords.X
                };

                SnakeSegments.Add(new Segment(nextX, nextY, direction));
            }
            CurrentLength = startingLength;
        }
        
        public int CurrentLength { get => SnakeSegments.Count; init {} }
        public Head GetHead { get => (Head)SnakeSegments[0]; set {} }
        public Segment GetTail { get => SnakeSegments.Last(); set{}}
        public List<Segment> SnakeSegments { get; set; } = [];
        public bool Alive { get; private set; } = true;
        

        public void Eat()
        {
            if (!Alive) throw new InvalidOperationException("can't eat if you're dead bro");

            Segment tail = GetTail; // assigning tail to new var for reusing, its a waste to call Last() more than once.

            // calculating new segment's coords after growing
            Segment newSegment = tail.Facing switch
            {
                Direction.Left => new(tail.X + 1, tail.Y, tail.Facing),
                Direction.Right => new(tail.X - 1, tail.Y, tail.Facing),
                Direction.Up => new(tail.X, tail.Y - 1, tail.Facing),
                Direction.Down => new(tail.X, tail.Y + 1, tail.Facing),

                _ => throw new Exception("Sth went wrong, tail's Direction enum was neither of it's values - probably null")
            };

            SnakeSegments.Add(newSegment);
        }
        public void Die()
        {
            Alive = false;
            SnakeSegments.Clear();
            OnDeath?.Invoke(this);
        }
        public event Action<Snake>? OnDeath;
        public class Segment(int x, int y, Direction direction)
        {
            public int X { get; set; } = x;
            public int Y { get; set; } = y;
            public Direction Facing { get; set; } = direction;
            public bool IsBent { get; set; } = false;
        }
        public class Head(int x, int y, Direction direction) : Segment(x, y, direction); // head cannot be bent so maybe do sth ab it in the future.
    }                                                                                    // instead of blindly extending the Head class with Segment.
}