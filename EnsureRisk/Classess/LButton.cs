using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EnsureRisk.Classess
{
    public class LButton: Button
    {
        public MyLayoutDocument MyOwner { get; set; }
        public LButton():base()
        {

        }
    }
}
