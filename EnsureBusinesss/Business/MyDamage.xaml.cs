using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnsureBusinesss.Business
{
    /// <summary>
    /// Interaction logic for MyDamage.xaml
    /// </summary>
    public partial class MyDamage : UserControl
    {
        public Color Colorr { get; set; }
        public Grid Container { get; set; }
        public decimal ID_TopRisk { get; set; }
        public string UM { get; set; }
        public TextBlock CalculateValue { get => CalculatedValue; set => CalculatedValue = value; }
        public TextBlock ExpecteDamage { get => ExpectedDamage; set => ExpectedDamage = value; }
        public TextBlock HeadeValue { get => HeaderValue; set => HeaderValue = value; }
        public TextBlock RisksDamageValue { get => RisksDamage; set => RisksDamage=value; }
        public TextBlock CounterMesuareDamageValue { get => CounterMesuareDamage; set => CounterMesuareDamage = value; }

        public MyDamage(Grid panel, Point location, string HeaderText, string CalculatedValueText, string ExpectedValueText, string RisksDamageText, string CounterMesuareDamageText, Color color, decimal ID_TopRisk)
        {
            InitializeComponent();
            panel.Children.Add(this);
            this.Margin = new Thickness(location.X, location.Y, 0, 0);
            this.Colorr = color;
            this.Container = panel;
            myBorder.Background = new SolidColorBrush(color);
            HeaderValue.Text = HeaderText;
            ExpectedDamage.Text = ExpectedValueText;
            RisksDamage.Text = RisksDamageText;
            CounterMesuareDamage.Text = CounterMesuareDamageText;
            CalculatedValue.Text = CalculatedValueText;
            this.ID_TopRisk = ID_TopRisk;
            Width = 250;
            Height = 150;
            this.RenderTransformOrigin = new Point(0, 0);
        }
    }
}