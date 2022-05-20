using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace XmlXslxProject.UI.ValidationRules
{
    internal class DecimalValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            decimal result;

            if (!string.IsNullOrWhiteSpace(value.ToString()) && !decimal.TryParse(value.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))
            {
                return new ValidationResult(false, "You must input a valid integer");
            }

            return new ValidationResult(true, null);
        }
    }
}


