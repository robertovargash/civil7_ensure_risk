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
using System.Windows.Controls.Primitives;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowSingleSelection.xaml
    /// </summary>
    public partial class WindowSingleSelection : Window, INotifyPropertyChanged
    {
        public DataRow RowSelected { get; set; }
        public DataTable Dt { get; set; }
        public DataView Dv { get; set; }
        public string[] DcolumToShowAlias { get; set; }
        public string[] DcolumToShow { get; set; }
        public string ColumnToFilter { get; set; }
        public bool IsMultiple { get; set; }
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public WindowSingleSelection()
        {
            InitializeComponent();
            ChangeLanguage();
            txtFilterRisk.DataContext = this;
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
                for (int i = 0; i < DcolumToShow.Count(); i++)
                {
                    dgSelection.Columns.Add(new DataGridTextColumn { Header = DcolumToShowAlias[i], Binding = new Binding(DcolumToShow[i]), IsReadOnly = true });
                }
                if (!Dt.Columns.Contains("Is_Selected"))
                {
                    Dt.Columns.Add("Is_Selected", typeof(bool));
                }
                foreach (DataRow item in Dt.Rows)
                {
                    item["Is_Selected"] = false;
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
            if (Dv.Table.Select("Is_Selected = 1").Any())
            {
                RowSelected = Dv.Table.Select("Is_Selected = 1").First();
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
                RowSelected = ((DataRowView)dgSelection.SelectedItems[0]).Row;
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
        
        private void Button_UnSelect(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.Source is Button checkboxx)
                {
                    if (checkboxx.DataContext is DataRowView rowView)
                    {
                        rowView.Row["Is_Selected"] = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Button_Select(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (DataRow item in Dv.Table.Rows)
                {
                    if ((bool)item["Is_Selected"])
                    {
                        item["Is_Selected"] = false;
                    }
                }
                if (e.Source is Button checkboxx)
                {
                    if (checkboxx.DataContext is DataRowView rowView)
                    {
                        rowView.Row["Is_Selected"] = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void dgSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                foreach (DataRow item in Dv.Table.Rows)
                {
                    if ((bool)item["Is_Selected"])
                    {
                        item["Is_Selected"] = false;
                    }
                }
                foreach (var item in dgSelection.SelectedItems)
                {
                    if (item is DataRowView thedataRowView)
                    {
                        thedataRowView.Row["Is_Selected"] = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
    }
}