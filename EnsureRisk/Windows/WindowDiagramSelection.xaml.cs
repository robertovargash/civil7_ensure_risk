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
using System.Windows.Shapes;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowDiagramSelection.xaml
    /// </summary>
    public partial class WindowDiagramSelection : Window
    {
        public DiagramSelection Selection { get; set; }
        public WindowDiagramSelection()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void RbCreate_Checked(object sender, RoutedEventArgs e)
        {
            Selection = DiagramSelection.Create;
            DialogResult = true;
        }

        private void RbClone_Checked(object sender, RoutedEventArgs e)
        {
            Selection = DiagramSelection.Clone;
            DialogResult = true;
        }

        private void RbImport_Checked(object sender, RoutedEventArgs e)
        {
            Selection = DiagramSelection.Import;
            DialogResult = true;
        }
    }
}
