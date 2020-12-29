using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace EnsureRisk.Classess
{
    public class DamageText:TextBox
    {
        public static readonly DependencyProperty DamageColumnProperty =
           DependencyProperty.Register("DamageColumn", typeof(string), typeof(DamageText));

        public string DamageColumn { set { SetValue(DamageColumnProperty, value); } get { return (string)GetValue(DamageColumnProperty); } }

        public DamageText(): base()
        {

        }
    }
}
