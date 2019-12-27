using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataMapping.Data;
using EnsureBusinesss;
using EnsureRisk.Resources;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowProject.xaml
    /// </summary>
    public partial class WindowProject : Window
    {
        public DataRow DrProject { get; set; }
        public string Operation { get; set; }

        public WindowProject()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DrProject[DT_Project.PROJECT_NAME] = txtKeyword.Text;
            DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Operation == General.UPDATE)
            {
                try
                {
                    txtKeyword.Text = DrProject[DT_Project.PROJECT_NAME].ToString();
                }
                catch (Exception ex)
                {
                    new WindowMessageOK(ex.Message).ShowDialog();
                }
            }
        }
    }
}
