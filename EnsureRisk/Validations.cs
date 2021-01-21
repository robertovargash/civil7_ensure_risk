using System.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System;
using System.Windows;
using System.Windows.Input;
using EnsureBusinesss;
using System.Text.RegularExpressions;

namespace EnsureRisk
{
    class Validations
    {
    }

    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString()) ? new ValidationResult(false, "Field is required.") : ValidationResult.ValidResult;
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

    public class DamageValueValidation : ValidationRule
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

    public class ProbabilityValidation : ValidationRule
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

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }

            return (bValue) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as Visibility? == TrueValue;
        }
    }

    public class OpositeBoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; } = Visibility.Visible;
        public Visibility FalseValue { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }

            return (bValue) ? FalseValue : TrueValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as Visibility? == FalseValue;
        }
    }

    public class BoolToCursorConverter : IValueConverter
    {
        public Cursor TrueValue { get; set; } = CursorHelper.FromByteArray(Properties.Resources.HandOpen);
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                return ((bool)value == true) ? TrueValue : Cursors.Arrow;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Cursor cursor)
                return cursor == TrueValue;
            return false;
        }
    }

    public class BoolToIconConverter : IValueConverter
    {

        public ContentControl TrueValue { get; set; } = new ContentControl { Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.CursorDefaultOutline } };
        public ContentControl FalseValue { get; set; } = new ContentControl { Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.Hand } };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }

            return (bValue) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as ContentControl == TrueValue;
        }
    }

    public class BoolToCheckConverter : IValueConverter
    {
        public ContentControl TrueValue { get; set; } = new ContentControl { Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.CheckboxMarkedOutline } };
        public ContentControl FalseValue { get; set; } = new ContentControl { Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.CloseBoxOutline } };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }

            return (bValue) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as ContentControl == TrueValue;
        }
    }

    public class BoolToCMProbabilityConverter : IValueConverter
    {
        public string TrueValue { get; set; } = "Risk Reduction (%)";
        public string FalseValue { get; set; } = "Probability (%)";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }

            return (bValue) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as string == TrueValue;
        }
    }

    public class BoolToCMProbabilityShortConverter : IValueConverter
    {
        public string TrueValue { get; set; } = "R.Red. (%)";
        public string FalseValue { get; set; } = "Prob. (%)";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool bValue = false;
            if (value is bool)
            {
                bValue = (bool)value;
            }

            return (bValue) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value as string == TrueValue;
        }
    }
}
