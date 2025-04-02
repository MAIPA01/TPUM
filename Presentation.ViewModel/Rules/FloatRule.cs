using System.Globalization;
using System.Windows.Controls;

namespace TPUM.Presentation.ViewModel.Rules
{
    public class FloatRule : ValidationRule
    {
        public override ValidationResult Validate(object? value, CultureInfo cultureInfo)
        {
            if (value == null) return ValidationResult.ValidResult;

            return float.TryParse(value.ToString(), out _) ? ValidationResult.ValidResult
                : new ValidationResult(false, "Value was not any float");
        }
    }
}