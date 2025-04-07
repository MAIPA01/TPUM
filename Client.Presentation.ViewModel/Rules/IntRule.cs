using System.Globalization;
using System.Windows.Controls;

namespace TPUM.Client.Presentation.ViewModel.Rules
{
    public class IntRule : ValidationRule
    {
        public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
        {
            if (value == null) return ValidationResult.ValidResult;

            return int.TryParse(value.ToString(), out _) ? ValidationResult.ValidResult
                : new ValidationResult(false, "Value was not any integer");
        }
    }
}