using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EnsureRisk.Classess
{
    public class GridPaint : MyGrid
    {
        public MyLayoutDocument MyOwner { get; set; }
        public GridPaint(): base()
        {
            Background = new SolidColorBrush(Colors.LightGoldenrodYellow);
            //ScaleTransform scale = new ScaleTransform() { ScaleX = 0.01, ScaleY = 0.01  };
            //TransformGroup grp = new TransformGroup();
            //grp.Children.Add(scale);
            //LayoutTransform = grp;
            //Hacer los eventos
            TextBox textName = new TextBox() { Visibility = Visibility.Hidden /*Hacer eventos*/ };
            this.Children.Add(textName);
        }
    }
}
