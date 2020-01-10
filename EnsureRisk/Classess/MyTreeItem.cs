using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EnsureRisk.Classess
{
    public class MyTreeItem: TreeViewItem
    {
        public int MyID { get; set; }

        public MyTreeItem(): base()
        {

        }
    }
}
