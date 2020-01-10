using DataMapping.Data;
using System;
using System.Data;
using System.Windows;
using EnsureBusinesss;
using EnsureRisk.Resources;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowWBS.xaml
    /// </summary>
    public partial class WindowWBS : Window
    {
        public string Operation { get; set; }
        public DataRow DrWBS { get; set; }
        public int IdProject { get; set; }
        public DataTable WBS_Structure { get; set; }
        public DataTable WBS_Encoder { get; set; }
        public DataView DvWBS { get; set; }
        public WindowWBS()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Operation == General.UPDATE)
                {
                    TextName.Text = DrWBS[DT_WBS.WBS_NAME].ToString();
                    TextLevel.Text = DrWBS[DT_WBS.NIVEL].ToString();
                }
                if (TextName.Text == "")
                {
                    BtnAdd.IsEnabled = false;
                }
                DvWBS = WBS_Structure.DefaultView;
                DvWBS.RowFilter = DT_WBS_STRUCTURE.ID_FATHER + " = " + DrWBS[DT_WBS.ID_WBS];
                dgWBS.ItemsSource = DvWBS;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Operation == General.INSERT)
                {
                    DrWBS[DT_WBS.WBS_NAME] = TextName.Text;
                    DrWBS[DT_WBS.NIVEL] = TextLevel.Text;
                    DrWBS[DT_WBS.IDPROJECT] = IdProject;
                }
                if (Operation == General.UPDATE)
                {
                    DrWBS[DT_WBS.IDPROJECT] = IdProject;
                    WBS_Encoder.Rows.Find(DrWBS[DT_WBS.ID_WBS])[DT_WBS.WBS_NAME] = TextName.Text;
                }               
                
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            int count = DvWBS.Count + 1;
            WindowWBSChild wBSChild = new WindowWBSChild
            {
                DrWBS_Structure = WBS_Structure.NewRow(),
                DrWBS = WBS_Encoder.NewRow(),
                IdProject = IdProject
            };
            wBSChild.TextLevel.Text = TextLevel.Text + "." + count;
            wBSChild.DrWBS_Structure[DT_WBS_STRUCTURE.ID_FATHER] = DrWBS[DT_WBS.ID_WBS];
            wBSChild.DrWBS_Structure[DT_WBS_STRUCTURE.FATHER] = DrWBS[DT_WBS.WBS_NAME];
            if (wBSChild.ShowDialog() == true)
            {                
                WBS_Encoder.Rows.Add(wBSChild.DrWBS);
                WBS_Structure.Rows.Add(wBSChild.DrWBS_Structure);
            }
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgWBS.SelectedIndex > 0)
                {
                    if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + WBS_Structure.Rows[dgWBS.SelectedIndex][DT_WBS_STRUCTURE.CHILD] + "]?").ShowDialog() == true)
                        WBS_Structure.Rows[dgWBS.SelectedIndex].Delete();
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void TextName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            try
            {
                if (TextName.Text.Trim() == "")
                {
                    BtnAdd.IsEnabled = false;
                }
                else
                {
                    BtnAdd.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }            
        }
    }
}
