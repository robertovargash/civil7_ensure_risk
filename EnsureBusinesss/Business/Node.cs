using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Petzold.Media2D;

namespace EnsureBusinesss.Business
{
    public class Node : Grid
    {
        public int ID { get; set; }
        public int ParentId { get; set; }

        public bool IsCM { get; set; }
        public TextBlock RiskName { get; set; }
        public Border MyBorder { get; set; }
        public TextBlock AcumD { get; set; }
        public TextBlock ValAcum { get; set; }
        public Color Colorr { get; set; }
        public Point MidBottomPoint { get; set; }
        public Point MidTopPoint { get; set; }
        public ArrowLine LineaArriba { get; set; }
        public Node Padre { get; set; }
        //public ArrowLine LineaAbajo { get; set; }

        //public ArrowLine LineaHorizontal { get; set; }
        public Node(Grid panel, Point location, string Risk, string AcumValue, string AcumDamage, Color color)
        {
            panel.Children.Add(this);
            //LineaHorizontal = new ArrowLine();
            LineaArriba = new ArrowLine();
            //LineaAbajo = new ArrowLine();

            SetZIndex(this, 1);
            Colorr = color;
            RenderTransformOrigin = new Point(0, 0);
            Margin = new Thickness(location.X, location.Y, 0, 0);
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Width = 150;
            Height = 90;
            MyBorder = new Border
            {
                CornerRadius = new CornerRadius(20),
                Background = new SolidColorBrush(color),
                Name = "BorderNode"

            };
            Children.Add(MyBorder);
            Grid gridBorder = new Grid() { Name = "ContenedorGrids" };
            MyBorder.Child = gridBorder;
            RowDefinition row = new RowDefinition
            {
                Height = new GridLength(30)
            };
            RowDefinition row2 = new RowDefinition
            {
                Height = new GridLength(30)
            };
            RowDefinition row3 = new RowDefinition
            {
                Height = new GridLength(30)
            };

            gridBorder.RowDefinitions.Add(row);
            gridBorder.RowDefinitions.Add(row2);
            gridBorder.RowDefinitions.Add(row3);

            Grid gridRisk = new Grid();
            Grid gridAcumV = new Grid();
            Grid gridAD = new Grid();

            var margin = new Thickness(10, 0, 0, 0);
            if (Risk.Length > 20)
            {
                Risk = Risk.Substring(0, 17) + " ...";
            }
            RiskName = new TextBlock() { Text = Risk, VerticalAlignment = VerticalAlignment.Center, Margin = margin, Foreground = new SolidColorBrush(Colors.Black) };
            AcumD = new TextBlock() { Text = AcumDamage, VerticalAlignment = VerticalAlignment.Center, Margin = margin, Foreground = new SolidColorBrush(Colors.Black) };
            ValAcum = new TextBlock() { Text = AcumValue, VerticalAlignment = VerticalAlignment.Center, Margin = margin, Foreground = new SolidColorBrush(Colors.Black) };

            gridRisk.Children.Add(RiskName);
            gridAD.Children.Add(AcumD);
            gridAcumV.Children.Add(ValAcum);

            gridBorder.Children.Add(gridRisk);
            gridBorder.Children.Add(gridAD);
            gridBorder.Children.Add(gridAcumV);

            SetRow(gridRisk, 0);

            SetRow(gridAcumV, 1);

            SetRow(gridAD, 2);
            MidBottomPoint = new Point(Margin.Left + (Width / 2), Margin.Top + Height);

            MidTopPoint = new Point(Margin.Left + (Width / 2), Margin.Top);
        }
    }
}
