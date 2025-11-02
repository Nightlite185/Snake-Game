namespace SnakeGame.Model
{
    public struct Coords(int row, int col) : IEquatable<Coords>
    {
        public int Row { get; set; } = row;
        public int Col { get; set; } = col;

        public readonly bool Equals(Coords other)
            => this.Row == other.Row && this.Col == other.Col;

        public override readonly string ToString() 
            => $"Row = {Row}, Col = {Col}";
    }
}