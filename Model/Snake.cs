namespace SnakeGame.Model
{
    public enum Direction { Up = 1, Right = 2, Down = 3, Left = 4}
    public class Snake
    {
        public Snake(int startingLength, Direction direction, Coords startingCoords, int maxLength)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(startingLength, 2); // cannot be less than two bc we are adding head first, outside of the loop.
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startingLength, maxLength);

            Body.AddFirst(new SnakeSegment(startingCoords, direction)); // adding head first

            GrowBy(times: startingLength - 1);
        }
        
        public int CurrentLength { get => Body.Count; init {} }
        public SnakeSegment Head => Body.First!.Value;
        public Coords HeadPos => Head.Coords;
        public SnakeSegment Tail => Body.Last!.Value;
        public Coords TailPos => Tail.Coords;
        public LinkedList<SnakeSegment> Body { get; set; } = [];
        public bool Alive { get; private set; } = true;
        
        public void Move(Coords newHeadCoords, Direction newDirection)
        {
            var tail = this.Tail; // save tail's ref

            Body.RemoveLast(); // pop tail from the body
            Body.AddFirst(tail); // glue it to the front

            // updating head with new data
            Head.Coords = newHeadCoords;
            Head.Facing = newDirection;
        }

        private void GrowBy(int times)
        {
            for (int i = 0; i < times; i++)
            {
                SnakeSegment tail = this.Tail; // assigning tail to new var for reusing, its a waste to call Last() more than once.

                // calculating new segment's coords after growing
                SnakeSegment newSegment = tail.Facing switch
                {
                    Direction.Left  => new ( new (TailPos.Row, TailPos.Col + 1), tail.Facing ), 
                    Direction.Right => new ( new (TailPos.Row, TailPos.Col - 1), tail.Facing ),
                    Direction.Up    => new ( new (TailPos.Row + 1, TailPos.Col), tail.Facing ),   
                    Direction.Down  => new ( new (TailPos.Row - 1, TailPos.Col), tail.Facing ), 

                    _ => throw new Exception($"Sth went wrong, tail's Direction's value is {tail.Facing}")
                };

                this.Body.AddLast(newSegment);
            }
        }
        public void Eat()
        {
            if (!Alive) throw new InvalidOperationException("can't eat if you're dead bro");

            GrowBy(times: 1);
        }
        public void Die()
        {
            Alive = false;
            Body.Clear();
            OnDeath?.Invoke(this);
        }
        public event Action<Snake>? OnDeath;
        public class SnakeSegment(Coords coords, Direction direction)
        {
            public Coords Coords { get; set; } = coords;
            public Direction Facing { get; set; } = direction;

        }
    }
}