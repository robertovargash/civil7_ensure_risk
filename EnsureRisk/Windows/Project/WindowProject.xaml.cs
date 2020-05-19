using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public partial class WindowProject : Window, INotifyPropertyChanged
    {
        private string _project;
        public string Project { get { return _project; } set { _project = value; OnPropertyChanged("Project"); } }
        public DataRow DrProject { get; set; }
        public string Operation { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public WindowProject()
        {
            InitializeComponent();
            TxtProject.DataContext = this;
        }

        
        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            DrProject[DT_Project.PROJECT_NAME] = TxtProject.Text;
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
                    TxtProject.Text = DrProject[DT_Project.PROJECT_NAME].ToString();
                }
                catch (Exception ex)
                {
                    MostrarErrorDialog(ex.Message);
                }
            }
        }

        private void TxtKeyword_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Project = TxtProject.Text;
        }
    }
}
