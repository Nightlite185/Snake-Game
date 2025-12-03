namespace SnakeGame.Helpers
{
    public enum ValidationResult
    { 
        Valid,
        ValueTooLow,
        ValueTooHigh,
        NullOrEmpty,
    }
    public static class Validation
    {
        public static ValidationResult String(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return ValidationResult.NullOrEmpty;

            return ValidationResult.Valid;
        }
        public static ValidationResult Int(int input, int MinValue, int MaxValue)
        {
            if (input < MinValue)
                return ValidationResult.ValueTooLow;

            else if (input > MaxValue)
                return ValidationResult.ValueTooHigh;

            return ValidationResult.Valid;
        }
    }
}
