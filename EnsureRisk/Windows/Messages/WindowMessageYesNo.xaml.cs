using EnsureRisk.Resources;
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
    /// Interaction logic for WindowMessageYesNo.xaml
    /// </summary>
    public partial class WindowMessageYesNo : Window
    {
        public WindowMessageYesNo(string Text)
        {
            InitializeComponent();
            TextShow.Text = Text;
            ChangeLanguage();
        }

        public void ChangeLanguage()
        {
            BtnOK.Content = StringResources.YesButton;
            BtnCancel.Content = StringResources.NoButton;
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
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
