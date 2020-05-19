using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowSaveAs.xaml
    /// </summary>
    public partial class WindowSaveAs : Window, INotifyPropertyChanged
    {
        private string _name;
        public string DiagramName { get { return _name; } set { _name = value; OnPropertyChanged("DName"); } }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public WindowSaveAs()
        {
            InitializeComponent();
            TextName.DataContext = this;
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

        private void TextName_TextChanged(object sender, TextChangedEventArgs e)
        {
            DiagramName = TextName.Text;
        }
    }
}
