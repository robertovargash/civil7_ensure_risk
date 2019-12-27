using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EnsureRisk.Classess
{
    public class LTextBlock:TextBlock
    {
        public MyLayoutDocument MyOwner { get; set; }
        public LTextBlock():base()
        {

        }
    }
}
