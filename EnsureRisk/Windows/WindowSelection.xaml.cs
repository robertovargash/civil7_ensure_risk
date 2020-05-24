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
using System.Data;
using EnsureBusinesss;
using EnsureRisk.Resources;
using System.ComponentModel;

namespace EnsureRisk.Windows
{
    public class ThisOne : INotifyPropertyChanged
    {
        string filterName = "None";

        public string FilterString
        {
            get { return filterName; }
            set
            {
                filterName = value;
                OnPropertyChanged("FilterString");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new
                PropertyChangedEventArgs(property));
        }
    }
    /// <summary>
    /// Interaction logic for WindowSelection.xaml
    /// </summary>
    public partial class WindowSelection : Window
    {
        public string FilterString { get; set; }
        public List<DataRow> RowsSelected { get; set; }
        public DataTable Dt { get; set; }
        public DataView Dv { get; set; }
        public string[] DcolumToShowAlias { get; set; }
        public string[] DcolumToShow { get; set; }
        public string ColumnToFilter { get; set; }
        public ThisOne P { get; set; }
        public bool IsMultiple { get; set; }

        public WindowSelection()
        {
            InitializeComponent();
            ChangeLanguage();
            P = new ThisOne();
            txtFilterRisk.DataContext = P;
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        public void ChangeLanguage()
        {
            BtnSelect.Content = StringResources.SelectButton;
            BtnCancel.Content = StringResources.CancelButton;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dgSelection.AutoGenerateColumns = false;
                //dgSelection.columns
                for (int i = 0; i < DcolumToShow.Count(); i++)
                {
                    DataGridTextColumn column = new DataGridTextColumn();
                    column.Header = DcolumToShowAlias[i];
                    column.Binding = new Binding(DcolumToShow[i]);
                    column.IsReadOnly = true;
                    //column.
                    dgSelection.Columns.Add(column);
                }
                Dv = new DataView(Dt);
                dgSelection.ItemsSource = Dv;

            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (dgSelection.SelectedItems.Count >= 0)
            {
                RowsSelected = new List<DataRow>();
                foreach (DataRowView item in dgSelection.SelectedItems)
                {
                    RowsSelected.Add(item.Row);
                }
                DialogResult = true;
            }
            else
            {
                MostrarErrorDialog("No selected"); ;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void dgSelection_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgSelection.SelectedItems.Count > 0)
            {
                RowsSelected = new List<DataRow>();
                foreach (DataRowView item in dgSelection.SelectedItems)
                {
                    RowsSelected.Add(item.Row);
                }
                DialogResult = true;
            }
            else
            {
                MostrarErrorDialog("No selected"); ;
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            //txtFilterRisk.Clear();
        }

        private void TxtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Dv.RowFilter = ColumnToFilter + " like '%" + txtFilterRisk.Text + "%'";

            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void btnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            txtFilterRisk.Clear();
        }
    }
}
