using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using EnsureRisk.Windows;

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

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (decimal.TryParse(value.ToString(), NumberStyles.Number, CultureInfo.CurrentUICulture, out decimal valueResult))
                    return valueResult;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
