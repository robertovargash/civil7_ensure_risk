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
using DataMapping.Data;
using EnsureBusinesss;
using EnsureRisk.Resources;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowSelectTopRisk.xaml
    /// </summary>
    public partial class WindowSelectTopRisk : Window
    {
        public DataRow Drow { get; set; }
        public DataTable TopRiskTable { get; set; }
        public WindowSelectTopRisk()
        {
            InitializeComponent();
            ChangeLanguage();
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }
        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextTopRisk, StringResources.TopRiskText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextUnit, StringResources.UnitText);
            BtnCancel.Content = StringResources.CancelButton;
            Title = StringResources.SelectTopRiskTitle;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceTopRiskController.WebServiceTopRisk ws = new ServiceTopRiskController.WebServiceTopRisk();
                DataTable topCodif = ws.GetAllTopRisk().Tables[DT_Damage.TopRisk_TABLA].Copy();
                WindowSelection frmSelection = new WindowSelection
                {
                    //dt = topCodif,
                    Dt = General.DeleteExists(topCodif, TopRiskTable, DT_Damage.ID_COLUMNA),
                    DcolumToShow = new string[] { DT_Damage.TOP_RISK_COLUMN },
                    DcolumToShowAlias = new string[] { "Damage" },
                    ColumnToFilter = DT_Damage.TOP_RISK_COLUMN,

                    Title = "Damages"
                };
                frmSelection.P.FilterString = "Damage";
                if (frmSelection.Dt.Rows.Count == 1)
                {
                    Drow[DT_Diagram_Damages.ID_DAMAGE] = frmSelection.Dt.Rows[0][DT_Damage.ID_COLUMNA];
                    Drow[DT_Diagram_Damages.COLOR] = frmSelection.Dt.Rows[0][DT_Damage.COLORID_COLUMNA];
                    TextTopRisk.Text = frmSelection.Dt.Rows[0][DT_Diagram_Damages.DAMAGE].ToString();
                }
                else
                {
                    frmSelection.ShowDialog();
                    if (frmSelection.DialogResult == true)
                    {
                        if (frmSelection.RowsSelected.Count() > 0)
                        {
                            Drow[DT_Diagram_Damages.ID_DAMAGE] = frmSelection.RowsSelected[0][DT_Damage.ID_COLUMNA];
                            Drow[DT_Diagram_Damages.COLOR] = frmSelection.RowsSelected[0][DT_Damage.COLORID_COLUMNA];
                            TextTopRisk.Text = frmSelection.RowsSelected[0][DT_Diagram_Damages.DAMAGE].ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }

        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TextTopRisk.Text != "")
                {
                    Drow[DT_Diagram_Damages.DAMAGE] = TextTopRisk.Text;
                    Drow[DT_Diagram_Damages.UM] = TextUnit.Text;
                    this.DialogResult = true;
                }
                else
                {
                    MostrarErrorDialog("Please, select a Top Risk!!!");
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }

        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; Close();
        }
    }
}
