using DataMapping.Data;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for WindowRenameGroup.xaml
    /// </summary>
    public partial class WindowRenameGroup : Window
    {
        public DataRow DrGroup { get; set; }

        public WindowRenameGroup()
        {
            InitializeComponent();
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DrGroup[DT_Groupe.GROUPE_NAME] = txtGroup.Text;
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                txtGroup.Text = DrGroup[DT_Groupe.GROUPE_NAME].ToString();
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
    }
}
