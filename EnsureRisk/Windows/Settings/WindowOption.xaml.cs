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
using EnsureRisk.Pages;
using EnsureRisk.Resources;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowOption.xaml
    /// </summary>
    public partial class WindowOption : Window
    {
        public WindowOption()
        {
            InitializeComponent();
            ChangeLanguage();
        }

        public void ChangeLanguage()
        {
            TextEnviroment.Text = StringResources.ListEnviroment;
            textSetting.Text = StringResources.ListServerSetting;
            Title = StringResources.SettingsMenu;
        }
        private void BtnSplit_Click(object sender, RoutedEventArgs e)
        {
            if (BtnSplit.Content == FindResource("Right"))
            {
                columnLeft.Width = new GridLength(200);
                BtnSplit.Content = FindResource("Left");
            }
            else
            {
                columnLeft.Width = new GridLength(40);
                BtnSplit.Content = FindResource("Right");
            }
        }

        private void lvConnection_MouseEnter(object sender, MouseEventArgs e)
        {
            FrameNavigation.Content = new SetConnectionPage();
        }

        private void lvEnviroment_MouseEnter(object sender, MouseEventArgs e)
        {
            FrameNavigation.Content = new ThemePage();
        }

       
    }
}
