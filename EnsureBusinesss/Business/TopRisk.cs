using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
//using System.Drawing;

namespace EnsureBusinesss
{
    public class TopRisk : Grid
    {
        public Label CalculatedValue { get; set; }
        public Label ExpectedDamage { get; set; }
        public Color Colorr { get; set; }
        public Grid Container { get; set; }
        public int ID_TopRisk { get; set; }
        public string UM { get; set; }
        public TopRisk(Grid panel, Point location, string CalculatedValueText, string ExpectedValueText, Color color, int ID_TopRisk, string UM)
        {
            panel.Children.Add(this);
            SetZIndex(this, 1);
            this.CalculatedValue = new Label();
            this.ExpectedDamage = new Label();
            this.RenderTransformOrigin = new Point(0, 0);
            this.Margin = new Thickness(location.X, location.Y, 0, 0);
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            //this.Background = new SolidColorBrush(color);
            this.Height = 60;
            this.Width = 250;
            //this.CornerRadius = 9;
            this.Colorr = color;
            this.Container = panel;
            Grid gridContanerLabels = new Grid();
            gridContanerLabels.Children.Add(CalculatedValue);
            gridContanerLabels.Children.Add(ExpectedDamage);
            Border myBorder = new Border
            {
                Background = new SolidColorBrush(color),
                CornerRadius = new CornerRadius(15),
                Child = gridContanerLabels
            };
            Children.Add(myBorder);
            //Children.Add(CalculatedValue);
            //Children.Add(ExpectedDamage);

            CalculatedValue.RenderTransformOrigin = new Point(0, 0.5);
            ExpectedDamage.RenderTransformOrigin = new Point(0, 0.5);

            //CalculatedValue.Margin = new Thickness(location.X + 5, location.Y + 10,0,0);
            //ExpectedDamage.Margin = new Thickness(location.X + 5, location.Y + 30, 0, 0);

            CalculatedValue.Margin = new Thickness(5, 10, 0, 0);
            ExpectedDamage.Margin = new Thickness(5, 30, 0, 0);

            //ver como cambiar el color negro este
            CalculatedValue.Foreground = new SolidColorBrush(Colors.Black);
            ExpectedDamage.Foreground = new SolidColorBrush(Colors.Black);

            CalculatedValue.Content = CalculatedValueText;
            ExpectedDamage.Content = ExpectedValueText;

            this.ID_TopRisk = ID_TopRisk;
            this.UM = UM;
        }
    }
}
