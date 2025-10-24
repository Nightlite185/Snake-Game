namespace SnakeGame.Model
{
    public class GameManager()
    {
        // grid constants definitions
        private const int gridRows = 400;
        private const int gridColumns = 400;

        // snake constants definitions
        private const int StartingLength = 3;
        private const Direction StartingDirection = Direction.Up;
        private static readonly (int X, int Y) startingCoords = (50, 50);

        //constructing game objects
        private GameGrid Grid { get; init; } = new(gridRows, gridColumns);
        private Snake Snake { get; init; } = new(StartingLength, StartingDirection, startingCoords);
    }
}