using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using EnsureRisk.Windows;
using System.Windows.Controls;
using EnsureRisk.Classess;

namespace EnsureRisk.DataBinding
{
    public struct DecimalUIConverterParams
    {
        private string stringFormat;

        public string StringFormat
        {
            get { return stringFormat; }
            set { stringFormat = value; }
        }

        public int Decimals { get; set; }

        public DecimalUIConverterParams(String format, int fractionalDigits)
        {
            stringFormat = format;
            Decimals = fractionalDigits;
        }
    }

    public class DecimalUIConverter: IValueConverter
    {
        public DecimalUIConverter()
        {
        }

        public static readonly DecimalUIConverter Instance = new DecimalUIConverter();
        public static readonly DecimalUIConverterParams ConverterParams = new DecimalUIConverterParams(Properties.Settings.Default.DecimalsStringFormat, Properties.Settings.Default.DecimalFractionalDigits);
        public static readonly CultureInfo CultureInfo = CultureInfo.CurrentUICulture;
        public static readonly string DecimalStringFormat = Properties.Settings.Default.DecimalsStringFormat;
        public static readonly int DecimalFractionalDigits = Properties.Settings.Default.DecimalFractionalDigits;
        public static Collection<ValidationRule> collection = new Collection<ValidationRule>() { new NegativePlusValidation() { ValidationStep = ValidationStep.UpdatedValue} };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is decimal && (decimal)value == 0)
                {
                    return "-";
                }
                else
                {
                    if (value.ToString() == "")
                    {
                        return "-";
                    }
                    else
                    {
                        if (parameter is DecimalUIConverterParams paramValue)
                        {
                            return string.Format(CultureInfo.CurrentUICulture, paramValue.StringFormat, Math.Round((decimal)value, paramValue.Decimals, MidpointRounding.AwayFromZero));
                        }
                        else
                        {
                            return value;
                        }
                    }
                    
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value.ToString() == "-" || value.ToString() == "")
                {
                    return 0;
                }
                else
                {
                    if (decimal.TryParse(value.ToString(), NumberStyles.Number, CultureInfo.CurrentUICulture, out decimal valueResult))
                        return valueResult;
                    else
                        return 0;
                }               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
