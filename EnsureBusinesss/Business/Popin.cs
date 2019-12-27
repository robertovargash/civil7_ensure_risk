using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace EnsureBusinesss.Business
{
    public class Popin : Grid
    {
        public TextBlock RiskName { get; set; }
        public TextBlock Probability { get; set; }
        public TextBlock EL { get; set; }
        public TextBlock Values { get; set; }
        public TextBlock ED { get; set; }
        public TextBlock ValAcum { get; set; }

        public Popin(Grid panel, Point location, string Risk, string Prob, string ExpLike, string ValueI, string Acum, string EDi)
        {
            panel.Children.Add(this);
            SetZIndex(this, 1);

            RenderTransformOrigin = new Point(0, 0);
            Margin = new Thickness(location.X, location.Y, 0, 0);
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            //Height = 208;
            Border border = new Border
            {
                CornerRadius = new CornerRadius(20),
                Background = new SolidColorBrush(Colors.Bisque)
            };
            Children.Add(border);
            Grid gridBorder = new Grid() { Name = "ContenedorGrids" };
            border.Child = gridBorder;
            RowDefinition row = new RowDefinition
            {
                //Height = new GridLength(35),
            };
            RowDefinition row2 = new RowDefinition
            {
                Height = new GridLength(35)
            };
            RowDefinition row3 = new RowDefinition
            {
                Height = new GridLength(35)
            };
            RowDefinition row4 = new RowDefinition
            {
                Height = new GridLength(35)
            };
            RowDefinition row5 = new RowDefinition
            {
                Height = new GridLength(35)
            };
            RowDefinition row6 = new RowDefinition
            {
                Height = new GridLength(35)
            };

            gridBorder.RowDefinitions.Add(row);
            gridBorder.RowDefinitions.Add(row2);
            gridBorder.RowDefinitions.Add(row3);
            gridBorder.RowDefinitions.Add(row4);
            gridBorder.RowDefinitions.Add(row5);
            gridBorder.RowDefinitions.Add(row6);

            Grid gridRisk = new Grid() { MaxWidth = 200};
            Grid gridPro = new Grid();
            Grid gridEL = new Grid();
            Grid gridValue = new Grid();
            Grid gridAcum = new Grid();
            Grid gridED = new Grid();

            var margin = new Thickness(10, 0, 10, 0);
            
            RiskName = new TextBlock() { Text = Risk + "  ", TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, Margin = margin, Foreground = new SolidColorBrush(Colors.Black) };
            Probability = new TextBlock() { Text = Prob, VerticalAlignment = VerticalAlignment.Center, Margin = margin, Foreground = new SolidColorBrush(Colors.Black) };
            EL = new TextBlock() { Text = ExpLike, VerticalAlignment = VerticalAlignment.Center, Margin = margin, Foreground = new SolidColorBrush(Colors.Black) };
            Values = new TextBlock() { Text = ValueI, VerticalAlignment = VerticalAlignment.Center, Margin = margin, Foreground = new SolidColorBrush(Colors.Black) };
            ValAcum = new TextBlock() { Text = Acum, VerticalAlignment = VerticalAlignment.Center, Margin = margin, Foreground = new SolidColorBrush(Colors.Black) };
            ED = new TextBlock() { Text = EDi, VerticalAlignment = VerticalAlignment.Center, Margin = margin, Foreground = new SolidColorBrush(Colors.Black) };

            gridRisk.Children.Add(RiskName);
            gridPro.Children.Add(Probability);
            gridEL.Children.Add(EL);
            gridValue.Children.Add(Values);
            gridAcum.Children.Add(ValAcum);
            gridED.Children.Add(ED);

            gridBorder.Children.Add(gridRisk);
            gridBorder.Children.Add(gridPro);
            gridBorder.Children.Add(gridEL);
            gridBorder.Children.Add(gridValue);
            gridBorder.Children.Add(gridAcum);
            gridBorder.Children.Add(gridED);

            SetRow(gridRisk, 0);
            SetRow(gridPro, 1);
            SetRow(gridEL, 2);
            SetRow(gridValue, 3);
            SetRow(gridAcum, 4);
            SetRow(gridED, 5);

        }

        public Popin(Grid panel, Point location, string Risk, string Prob, string ValueI)
        {
            panel.Children.Add(this);
            SetZIndex(this, 1);

            RenderTransformOrigin = new Point(0, 0);
            Margin = new Thickness(location.X, location.Y, 0, 0);
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Border border = new Border
            {
                CornerRadius = new CornerRadius(20),
                Background = new SolidColorBrush(Colors.Bisque)
            };
            Children.Add(border);
            Grid gridBorder = new Grid() { Name = "ContenedorGrids" };
            border.Child = gridBorder;
            RowDefinition row = new RowDefinition
            {
                //Height = new GridLength(36)
            };
            RowDefinition row2 = new RowDefinition
            {
                Height = new GridLength(36)
            };
            RowDefinition row3 = new RowDefinition
            {
                Height = new GridLength(36)
            };

            gridBorder.RowDefinitions.Add(row);
            gridBorder.RowDefinitions.Add(row2);
            gridBorder.RowDefinitions.Add(row3);

            Grid gridRisk = new Grid() { MaxWidth = 200 };
            Grid gridPro = new Grid();
            Grid gridValue = new Grid();

            var margin = new Thickness(10, 0, 10, 0);

            RiskName = new TextBlock() { Text = Risk + "  ", TextWrapping = TextWrapping.Wrap, VerticalAlignment = VerticalAlignment.Center, Margin = margin, Foreground = new SolidColorBrush(Colors.Black)};
            Probability = new TextBlock() { Text = Prob, VerticalAlignment = VerticalAlignment.Center, Margin = margin, Foreground = new SolidColorBrush(Colors.Black) };
            Values = new TextBlock() { Text = ValueI, VerticalAlignment = VerticalAlignment.Center, Margin = margin, Foreground = new SolidColorBrush(Colors.Black) };

            gridRisk.Children.Add(RiskName);
            gridPro.Children.Add(Probability);
            gridValue.Children.Add(Values);

            gridBorder.Children.Add(gridRisk);
            gridBorder.Children.Add(gridPro);
            gridBorder.Children.Add(gridValue);

            SetRow(gridRisk, 0);
            SetRow(gridPro, 1);
            SetRow(gridValue, 2);

        }

    }
}
