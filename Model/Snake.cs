namespace SnakeGame.Model
{
    class Snake(int startingLength)
    {

        private const int maxLength = 50; // this is a placeholder, to change later. It depends on the number of squares in the grid.

        private int StartingLength = startingLength <= 0
            ? throw new ArgumentOutOfRangeException("Length needs to be higher than 0")

               : startingLength >= maxLength
                   ? throw new ArgumentOutOfRangeException($"Length needs to be lower than {maxLength}")

            : startingLength;

        public int CurrentLength { get; };

        public Segment[] GetSnake { get; set; } = [];

        private struct Segment
        {
            int? X { get; set; }
            int? Y { get; set; }
            bool IsBent { get; set; }
        }

        private struct Head : Segment { }

    }

    
}
