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
    /// Interaction logic for WindowCopyPasteMoveConfirm.xaml
    /// </summary>
    public partial class WindowCopyPasteMoveConfirm : Window
    {
        public int OptionSelected { get; set; }

        public WindowCopyPasteMoveConfirm(string Text)
        {
            InitializeComponent();
            TextShow.Text = Text;
        }

        private void BtnAllData_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected = 0;
            DialogResult = true;
        }

        private void BtnNothing_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected = 2;
            DialogResult = true;
        }

        private void BtnWBS_Click(object sender, RoutedEventArgs e)
        {
            OptionSelected = 1;
            DialogResult = true;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
