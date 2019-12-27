using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EnsureRisk.Classess
{
    public class MyGrid: Grid
    {
        public int ID_Diagram { get; set; }
        public MyGrid():base()
        {

        }

        public void SetTheRow(System.Windows.UIElement element, int row)
        {
            SetRow(element, row);
        }
    }
}
