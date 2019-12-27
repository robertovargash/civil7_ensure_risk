using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnsureRisk.Classess
{
    public class ScrollViewerDiagram : ScrollViewer
    {
        public MyLayoutDocument MyOwner { get; set; }
        public int ID_Diagram { get; set; }
        public ScrollViewerDiagram():base()
        {
            
        }
        public void AddHijo(LGrid grid)
        {
            AddChild(grid);
        }
    }
}
