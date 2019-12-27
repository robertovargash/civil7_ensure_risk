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
        private String stringFormat;

        public String StringFormat
        {
            get { return stringFormat; }
            set { stringFormat = value; }
        }

        private int decimals;

        public int Decimals
        {
            get { return decimals; }
            set { decimals = value; }
        }

        public DecimalUIConverterParams(String format, int fractionalDigits)
        {
            stringFormat = format;
            decimals = fractionalDigits;
        }
    }

    public class DecimalUIConverter: IValueConverter
    {
        public DecimalUIConverter()
        {
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (parameter is DecimalUIConverterParams paramValue)
                {
                    return String.Format(CultureInfo.CurrentUICulture, paramValue.StringFormat, Math.Round((Decimal)value, paramValue.Decimals, MidpointRounding.AwayFromZero));
                }
                else
                {
                    return value;
                }
            }
            catch (Exception ex)
            {
               return  new WindowMessageOK(ex.Message).ShowDialog();
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                Decimal valueResult;
                if (Decimal.TryParse(value.ToString(), NumberStyles.Number, CultureInfo.CurrentUICulture, out valueResult))
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
