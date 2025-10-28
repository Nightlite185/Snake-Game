using System.Drawing;

namespace SnakeGame.Model
{
    public enum Direction { Up = 1, Right = 2, Down = 3, Left = 4}
    public class Snake
    {
        public Snake(int startingLength, Direction direction, (int X, int Y) startingCoords, int maxLength)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(startingLength, 2); // cannot be less than two bc we are adding head first, outside of the loop.
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startingLength, maxLength);

            Body.AddLast(new SnakeSegment(startingCoords.X, startingCoords.Y, direction, isHead: true)); // adding head first

            Grow(times: startingLength - 1);
        }
        
        public int CurrentLength { get => Body.Count; init {} }
        public SnakeSegment Head => Body.First!.Value;
        public SnakeSegment Tail => Body.Last!.Value;
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
                    Direction.Left => new(tail.X + 1, tail.Y, tail.Facing), 
                    Direction.Right => new(tail.X - 1, tail.Y, tail.Facing),
                    Direction.Up => new(tail.X, tail.Y + 1, tail.Facing),   
                    Direction.Down => new(tail.X, tail.Y - 1, tail.Facing), 

                    _ => throw new Exception($"Sth went wrong, tail's Direction's value is {tail.Facing}")
                };

                this.Body.AddLast(newSegment);
            }
        }
        public void Eat()
        {
            if (!Alive) throw new InvalidOperationException("can't eat if you're dead bro");

            Grow(times: 1);
        }
        public void Die()
        {
            Alive = false;
            Body.Clear();
            OnDeath?.Invoke(this);
        }
        public event Action<Snake>? OnDeath;
        public class SnakeSegment(int x, int y, Direction direction, bool isHead = false)
        {
            public int X { get; set; } = x;
            public int Y { get; set; } = y;
            public Direction Facing { get; set; } = direction;
            public bool IsBent { get; set; } = false;
            public bool IsHead { get; set; } = isHead;
        }
    }
}