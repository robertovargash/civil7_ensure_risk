using System.Globalization;
using System.Windows.Controls;

namespace EnsureRisk.Classess
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString()) ? new ValidationResult(false, "Field is required."): ValidationResult.ValidResult;
        }
    }

    public class IsCheckedValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value is bool && (bool)value)
            {
                return ValidationResult.ValidResult;
            }
            return new ValidationResult(false, "Option must be checked");
        }
    }
}
