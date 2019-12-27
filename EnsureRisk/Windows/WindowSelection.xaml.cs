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

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowSelection.xaml
    /// </summary>
    public partial class WindowSelection : Window
    {

        public DataRow[] RowsSelected { get; set; }
        public DataTable dt { get; set; }
        public string[] dcolumToShowAlias { get; set; }
        public string[] dcolumToShow { get; set; }
        public WindowSelection()
        {
            InitializeComponent();
            ChangeLanguage();
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
                for (int i = 0; i < dcolumToShow.Count(); i++)
                {
                    DataGridTextColumn column = new DataGridTextColumn();
                    column.Header = dcolumToShowAlias[i];
                    column.Binding = new Binding(dcolumToShow[i]);
                    column.IsReadOnly = true;
                    //column.
                    dgSelection.Columns.Add(column);
                }
                dgSelection.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (dgSelection.SelectedItems.Count >= 0)
            {
                RowsSelected = new DataRow[dgSelection.SelectedItems.Count];
                int index = 0;
                for (int i = 0; i < dgSelection.Items.Count; i++)
                {
                    if (dgSelection.SelectedItems.Count > index)
                    {
                        if (dgSelection.SelectedItems[index].Equals(dgSelection.Items[i]))
                        {
                            RowsSelected[index] = dt.Rows[i];
                            index++;
                        }
                    }
                }

                DialogResult = true;
            }
            else
            {
                new WindowMessageOK("No selected").ShowDialog(); ;
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
                RowsSelected = new DataRow[1];
                RowsSelected[0] = dt.Rows[dgSelection.SelectedIndex];
                DialogResult = true;
            }
            else
            {
                new WindowMessageOK("No selected").ShowDialog(); ;
            }
        }
    }
}
