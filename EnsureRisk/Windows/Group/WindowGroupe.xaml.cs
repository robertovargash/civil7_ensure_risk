using System;
using System.Linq;
using System.Windows;
using System.Data;
using DataMapping.Data;
using EnsureBusinesss;
using EnsureRisk.Resources;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowGroupe.xaml
    /// </summary>
    public partial class WindowGroupe : Window
    {
        public decimal IdGroup = -100;
        public string GroupName { get; set; }
        public DataTable DT_Groups { get; set; }
        public WindowGroupe()
        {
            InitializeComponent();
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GroupName = TextGroup.Text;
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //using (GroupService.WebServiceGroupe ws = new GroupService.WebServiceGroupe())
                //{
                //    DataTable topCodif = ws.GetAllGroupes().Tables[DT_Groupe.TABLE_NAME].Copy();
                    
                //}
                WindowSingleSelection frmSelection = new WindowSingleSelection
                {
                    Dt = DT_Groups,
                    DcolumToShow = new string[] { DT_Groupe.GROUPE_NAME },
                    ColumnToFilter = DT_Groupe.GROUPE_NAME,
                    DcolumToShowAlias = new string[] { DT_Groupe.GROUPE_NAME },
                    Title = "Groups"
                };
                frmSelection.ShowDialog();
                if (frmSelection.DialogResult == true)
                {//Arreglado por roberto pues estaba  tomando el primer elemento
                    IdGroup = (decimal)frmSelection.RowSelected[DT_Groupe.ID_GROUPE];
                    TextGroup.Text = frmSelection.RowSelected[DT_Groupe.GROUPE_NAME].ToString();
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
    }
}
