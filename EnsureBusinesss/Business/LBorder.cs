using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnsureBusinesss.Business
{
    public class LBorder:Border
    {

        public string Risk
        {
            set;
            get;
        }

        public int IDRisk
        {
            set;
            get;
        }
        public LBorder():base()
        {

        }
    }
}
