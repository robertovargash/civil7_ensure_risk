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

        public WindowHeaderClasification()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnList = new List<MyExcelButton>();
                ListBox listBox = new ListBox();
                foreach (var item in MyList)
                {
                    var margin = new Thickness(10, 10, 10, 10);
                    StackPanel stk = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = margin,
                        HorizontalAlignment = HorizontalAlignment.Right
                    };
                    TextBlock texting = new TextBlock
                    {
                        Text = item.MyContent,
                        Margin = margin,
                        Width = 125
                    };
                    MyExcelButton btnExcel = new MyExcelButton
                    {
                        Content = "...",
                        Style = BtnOK.Style
                    };
                    btnExcel.Click += Mck_Click;
                    BtnList.Add(btnExcel);
                    stk.Children.Add(texting);
                    stk.Children.Add(btnExcel);
                    listBox.Items.Add(stk);
                }
                TheStackPanel.Children.Add(listBox);
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
                    ((MyExcelButton)sender).MyValue = wmr.ValueSelected;
                    Chip chip = new Chip()
                    {
                        Content = wmr.ContentSelected,
                        Icon = wmr.ContentSelected.ToString().ToCharArray()[0].ToString().ToUpper(),
                        IsDeletable = true
                    };
                    chip.DeleteClick += Chip_DeleteClick;
                    ((StackPanel)((MyExcelButton)sender).Parent).Children.Add(chip);
                    ((MyExcelButton)sender).Visibility = Visibility.Collapsed;
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
                ((MyExcelButton)((StackPanel)((Chip)sender).Parent).Children[1]).Visibility = Visibility.Visible;
                ((MyExcelButton)((StackPanel)((Chip)sender).Parent).Children[1]).MyValue = 0;
                ((StackPanel)((Chip)sender).Parent).Children.RemoveAt(2);
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
                for (int i = 0; i < BtnList.Count; i++)
                {
                    MyList[i].IdClasification = BtnList[i].MyValue;
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
    }
}
