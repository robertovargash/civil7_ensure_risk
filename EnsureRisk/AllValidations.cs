using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EnsureRisk
{
    public class AllValidations : ValidationRule
    {
        public decimal MinProb { get; set; }
        public decimal MaxProb { get; set; }
        public AllValidations()
        {

        }

        public override ValidationResult Validate(object value,CultureInfo cultureInfo)
        {
            decimal proba = 0;

            try
            {
                if (((string)value).Length > 0)
                    proba = Convert.ToDecimal(value,cultureInfo);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, "Illegal characters or " + e.Message);
            }

            if (proba < 0 || proba > 1)
            {
                return new ValidationResult(false,
                  "Please enter a number in the range: 0 - 1");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
}
