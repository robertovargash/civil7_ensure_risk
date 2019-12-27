using DataMapping.Data;
using System;
using System.Data;
using System.Windows;


namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowWBSChild.xaml
    /// </summary>
    public partial class WindowWBSChild : Window
    {
        public DataRow DrWBS { get; set; }
        public DataRow DrWBS_Structure { get; set; }
        public WindowWBSChild()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DrWBS_Structure[DT_WBS_STRUCTURE.ID_CHILD] = DrWBS[DT_WBS.ID_WBS];
                DrWBS_Structure[DT_WBS_STRUCTURE.CHILD] = TextName.Text;
                DrWBS_Structure[DT_WBS_STRUCTURE.CNIVEL] = TextLevel.Text;
                DrWBS[DT_WBS.WBS_NAME] = TextName.Text;
                DrWBS[DT_WBS.NIVEL] = TextLevel.Text;
                DialogResult = true;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
