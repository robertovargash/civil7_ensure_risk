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

namespace EnsureRisk.DialogContent
{
    /// <summary>
    /// Interaction logic for LoginDialogContent.xaml
    /// </summary>
    public partial class LoginDialogContent : UserControl
    {
        public LoginDialogContent()
        {
            InitializeComponent();
        }

        private void TextUser_GotFocus(object sender, RoutedEventArgs e)
        {
            TextPasword.SelectAll();
        }

        private void TextPasword_GotFocus(object sender, RoutedEventArgs e)
        {
            TextUser.SelectAll();
        }
    }
}
