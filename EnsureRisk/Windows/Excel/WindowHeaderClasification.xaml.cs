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
using DataMapping.Data;
using EnsureRisk.Classess;
using MaterialDesignThemes.Wpf;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowHeaderClasification.xaml
    /// </summary>
    public partial class WindowHeaderClasification : Window
    {
        public List<HeaderExcelContent> MyList { get; set; }
        public List<MyExcelButton> BtnList { get; set; }
        public List<int> ChipList { get; set; }
        public DataSet MyDataset { get; set; }
        public bool IsCustom { get; set; }
        public DataView DataVieww { get; set; }
        public DataTable MyTable { get; set; }

        private const string ID_CLASIFICATION = "idClasification";
        private const string HEADER = "Header";
        private const string TYPE = "Type";

        public WindowHeaderClasification()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                chkIsCustom.IsChecked = true;
                IsCustom = true;
                BtnList = new List<MyExcelButton>();
                MyTable = new DataTable();
                MyTable.Columns.Add(HEADER, typeof(string));
                MyTable.Columns.Add(ID_CLASIFICATION, typeof(int));
                MyTable.Columns.Add(TYPE, typeof(string));
                foreach (var item in MyList)
                {
                    DataRow dr = MyTable.NewRow();
                    dr[HEADER] = item.MyContent;
                    dr[ID_CLASIFICATION] = 0;
                    dr[TYPE] = "Select...";
                    MyTable.Rows.Add(dr);
                }
                DataVieww= new DataView(MyTable);
                dgHeaders.ItemsSource = DataVieww;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        private void Mck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowMultiRadio wmr = new WindowMultiRadio();
                if (wmr.ShowDialog() == true)
                {
                    if (dgHeaders.SelectedIndex >= 0)
                    {                        
                        DataVieww[dgHeaders.SelectedIndex].Row[ID_CLASIFICATION] = wmr.ValueSelected;
                        DataVieww[dgHeaders.SelectedIndex].Row[TYPE] = wmr.ContentSelected;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }            
        }

        private void Chip_DeleteClick(object sender, RoutedEventArgs e)
        {
            try
            {
                ((MyExcelButton)((StackPanel)((Chip)sender).Parent).Children[0]).Visibility = Visibility.Visible;
                ((MyExcelButton)((StackPanel)((Chip)sender).Parent).Children[0]).MyValue = 0;
                ((StackPanel)((Chip)sender).Parent).Children.RemoveAt(1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                for (int i = 0; i < MyTable.Rows.Count; i++)
                {
                    MyList[i].IdClasification = (int)MyTable.Rows[i][ID_CLASIFICATION];
                }
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }           
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void chkIsCustom_Unchecked(object sender, RoutedEventArgs e)
        {
            IsCustom = false;
        }

        private void chkIsCustom_Checked(object sender, RoutedEventArgs e)
        {
            IsCustom = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dgHeaders.SelectedIndex >= 0)
            {
                DataVieww[dgHeaders.SelectedIndex].Row["idClasification"] = 0;
                DataVieww[dgHeaders.SelectedIndex].Row["Type"] = "Select...";
            }
        }

        private void dgHeaders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //if (dgHeaders.SelectedIndex >= 0)
                //{
                //    WindowMultiRadio wmr = new WindowMultiRadio();
                //    if (wmr.ShowDialog() == true)
                //    {
                //        DataVieww[dgHeaders.SelectedIndex].Row[ID_CLASIFICATION] = wmr.ValueSelected;
                //        DataVieww[dgHeaders.SelectedIndex].Row[TYPE] = wmr.ContentSelected;
                //    }
                //}                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgHeaders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgHeaders.SelectedIndex >= 0)
            {
                WindowMultiRadio wmr = new WindowMultiRadio();
                if (wmr.ShowDialog() == true)
                {
                    DataVieww[dgHeaders.SelectedIndex].Row[ID_CLASIFICATION] = wmr.ValueSelected;
                    DataVieww[dgHeaders.SelectedIndex].Row[TYPE] = wmr.ContentSelected;
                }
            }
        }
    }
}
