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
    /// Interaction logic for WindowMultiRadio.xaml
    /// </summary>
    public partial class WindowMultiRadio : Window
    {
        public int ValueSelected { get; set; }
        public string ContentSelected { get; set; }
        public WindowMultiRadio()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            ValueSelected = BuscarElementomarcado();
            ContentSelected = BuscarStrElementomarcado();
            DialogResult = true;
        }

        public int BuscarElementomarcado()
        {
            int position = -1;
            foreach (var item in gridRadioButton.Children)
            {
                if (((MyRadio)item).IsChecked == true)
                {
                    position = ((MyRadio)item).MyValue;
                    break;
                }
            }
            return position;
        }

        public string BuscarStrElementomarcado()
        {
            string content = "Nothing";
            foreach (var item in gridRadioButton.Children)
            {
                if (((MyRadio)item).IsChecked == true)
                {
                    content = ((MyRadio)item).Content.ToString();
                    break;
                }
            }
            return content;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void MyRadio_Checked(object sender, RoutedEventArgs e)
        {
            BtnOK_Click(sender, e);
        }
    }
}
