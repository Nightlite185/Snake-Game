namespace SnakeGame.Model
{
    public struct Coords(int row, int col)
    {
        public int Row { get; set; } = row;
        public int Col { get; set; } = col;
        public override readonly string ToString() 
            => $"Row = {Row}, Col = {Col}";
    }
}