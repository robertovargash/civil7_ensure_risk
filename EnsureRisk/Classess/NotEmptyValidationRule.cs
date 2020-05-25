using System.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

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

    public class NegativeValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            DataRowView course = ((BindingExpression)value).DataItem as DataRowView;

            if (course.Row["value"] is decimal)
            {
                if ((decimal)course.Row["value"] < 0)
                {
                    return new ValidationResult(false, "Value can't be negative");
                }
                else
                {
                    return ValidationResult.ValidResult;
                }                
            }
            return new ValidationResult(false, "Insert a number");
        }
    }

    public class NegativePlusValidation : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {

            DataRowView course = ((BindingExpression)value).DataItem as DataRowView;

            if (course.Row["probability"] is decimal)
            {
                if ((decimal)course.Row["probability"] < 0)
                {
                    return new ValidationResult(false, "Probability can't be negative");
                }
                else
                {
                    if ((decimal)course.Row["probability"] < 0 || (decimal)course.Row["probability"] > 100)
                    {
                        return new ValidationResult(false, "Probability must be between 0 and 100");
                    }
                    return ValidationResult.ValidResult;
                }
            }
            return new ValidationResult(false, "Insert a number");
        }
    }
}
