using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TPUM.Presentation.ViewModel.Rules
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
