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

namespace EnsureRisk
{
    /// <summary>
    /// Interaction logic for PanelWBS.xaml
    /// </summary>
    public partial class PanelWBS : StackPanel
    {
        public static readonly DependencyProperty TextoProperty =
            DependencyProperty.Register("Texto", typeof(string), typeof(PanelWBS), new FrameworkPropertyMetadata("Nada"));

        public string Texto
        {
            set { SetValue(TextoProperty, value); }
            get { return (string)GetValue(TextoProperty); }
        }
        public PanelWBS()
        {
            InitializeComponent();
        }
    }
}
