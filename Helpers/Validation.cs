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
        public static ValidationResult Int(int input, (double min, double max) bounds)
        {
            //if (input == null || !int.TryParse(input, out int value))
            //    return ValidationResult.NullOrEmpty;

            if (input < bounds.min)
                return ValidationResult.ValueTooLow;

            else if (input > bounds.max)
                return ValidationResult.ValueTooHigh;

            return ValidationResult.Valid;
        }
    }
}
