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
    /// Interaction logic for WindowSaveAs.xaml
    /// </summary>
    public partial class WindowSaveAs : Window
    {
        public string DiagramName { get; set; }
        public WindowSaveAs()
        {
            InitializeComponent();
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TextName.Text = DiagramName;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DiagramName = TextName.Text;
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
