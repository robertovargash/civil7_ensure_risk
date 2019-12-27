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
    /// Interaction logic for WindowExcelClasification.xaml
    /// </summary>
    public partial class WindowExcelClasification : Window
    {
        public bool Custom { get; set; }
        public WindowExcelClasification()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            Custom = cbSelection.SelectedIndex == 0;
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
