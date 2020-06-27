using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EnsureRisk.Classess
{
    /// <summary>
    /// Interaction logic for MyTreeViewItem.xaml
    /// </summary>
    public partial class MyTreeViewItem : TreeViewItem, INotifyPropertyChanged
    {
        private int myID;
        public int MyID { get { return myID; } set { myID = value; OnPropertyChanged("MyID"); } }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public MyTreeViewItem()
        {
            InitializeComponent();
        }

        public void EnableBtns(bool isEnable)
        {
            BtnEdit.IsEnabled = BtnDelete.IsEnabled = isEnable;
        }
    }
}
