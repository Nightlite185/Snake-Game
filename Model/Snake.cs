namespace SnakeGame.Model
{
    public enum Direction { Up = 1, Right = 2, Down = 3, Left = 4 }
    public class Snake
    {
        public Snake(Settings.SnakeSettings cfg, Coords startingCoords, Direction startingDirection)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(cfg.StartingLength, 2); // cannot be less than two bc we are adding head first, outside of the loop.

            Alive = true;
            Body.AddFirst(new SnakeSegment(startingCoords, startingDirection)); // adding head first

            GrowBy(times: cfg.StartingLength - 1);
        }
        public int CurrentLength { get => Body.Count; init {} }
        public SnakeSegment Head => Body.First!.Value;
        public Coords HeadPos => Head.Coords;
        public SnakeSegment Tail => Body.Last!.Value;
        public Coords TailPos => Tail.Coords;
        public LinkedList<SnakeSegment> Body { get; set; } = [];
        public bool Alive { get; private set; }
        
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
                SnakeSegment tail = this.Tail; // assigning tail to new var for reusing, its a waste to call Last() more than once

                // calculating new segment's coords after growing
                Coords coords = tail.Facing switch
                {
                    Direction.Left => new(tail.Coords.Row, tail.Coords.Col + 1),
                    Direction.Right => new(tail.Coords.Row, tail.Coords.Col - 1),
                    Direction.Up => new(tail.Coords.Row + 1, tail.Coords.Col),
                    Direction.Down => new(tail.Coords.Row - 1, tail.Coords.Col),
                    _ => throw new Exception($"Unexpected tail's direction: {tail.Facing}")
                };

                SnakeSegment newSegment = new(coords, tail.Facing);

                Body.AddLast(newSegment);
            }
        }
        public void Eat()
        {
            if (!Alive) throw new InvalidOperationException("can't eat if you're dead bro");

            GrowBy(times: 1);
        }

        public void Die() => Alive = (Alive == true)
            ? false
            : throw new InvalidOperationException("can't die if you're already dead bro");
        public class SnakeSegment(Coords coords, Direction direction)
        {
            public Coords Coords { get; set; } = coords;
            public Direction Facing { get; set; } = direction;
        }
    }
}