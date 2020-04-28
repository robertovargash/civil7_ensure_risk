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

namespace EnsureRisk.Classess
{
    /// <summary>
    /// Interaction logic for PopWindow.xaml
    /// </summary>
    public partial class PopWindow : UserControl
    {
        public static readonly DependencyProperty PointProperty =
            DependencyProperty.Register("Point",
                typeof(Point), typeof(PopWindow),
                new FrameworkPropertyMetadata(new Point(),
                        FrameworkPropertyMetadataOptions.AffectsRender));

        public Point Point
        {
            set { SetValue(PointProperty, value); }
            get { return (Point)GetValue(PointProperty); }
        }
        public PopWindow()
        {
            InitializeComponent();            
        }
    }
}
