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

            Body.Add(new SnakeSegment(startingCoords.X, startingCoords.Y, direction, isHead: true)); // adding head first

            Grow(times: startingLength - 1);
        }
        
        public int CurrentLength { get => Body.Count; init {} }
        public SnakeSegment Head => Body[0];
        public SnakeSegment Tail => Body.Last();
        public List<SnakeSegment> Body { get; set; } = [];
        public bool Alive { get; private set; } = true;
        
        private void Grow(int times)
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

                this.Body.Add(newSegment);
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