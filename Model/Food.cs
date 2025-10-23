namespace SnakeGame.Model
{
    internal struct Food
    {
        public int? X { get; private set; } 
        public int? Y { get; private set; } 

        public void Reset()
        {
            this.X = null;
            this.Y = null;
        }
    }
}

