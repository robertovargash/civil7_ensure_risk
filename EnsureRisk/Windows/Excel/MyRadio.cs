using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EnsureRisk.Windows
{
    public class MyRadio:RadioButton
    {
        public int MyValue { get; set; }
        public MyRadio():base()
        {

        }
    }
}
