using System.Globalization;
using System.Windows.Controls;

namespace SnakeGame.Helpers
{
    public class IntValidation : ValidationRule
    {
        public string? TooLowMessage { get; set; } = null;
        public string? TooHighMessage { get; set; } = null;
        public int Min { get; set; } = 1; // if not given -> default 1.
        public int? Max { get; set; } // def null (not to allow huge nums if forgot to provide max) -> we throw if null.
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string? text = value as string;
            
            if (string.IsNullOrEmpty(text))
                return new ValidationResult(false, null); // case when I want quiet reject + reverting UI to previous val

            if (!int.TryParse(text, out int parsedNum))
                return new ValidationResult(false, "input value has to be a number.");

            if (!Max.HasValue) throw new ArgumentNullException(nameof(Max));

            if (parsedNum < Min)
                return new ValidationResult(false, TooLowMessage ?? $"input value has to be higher than {Min}.");

            if (parsedNum > Max)
                return new ValidationResult(false, TooHighMessage ?? $"input value has to be lower than {Max}.");

            return ValidationResult.ValidResult;
        }
    }
}
