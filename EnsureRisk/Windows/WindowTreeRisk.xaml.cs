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
    /// Interaction logic for WindowTreeRisk.xaml
    /// </summary>
    public partial class WindowTreeRisk : Window
    {
        public string Operation { get; set; }
        public int IdWBS { get; set; }
        public DataTable TopRiskTable { get; set; }
        public DataTable CM_TopRisk { get; set; }
        public DataTable Risk_TopRisk { get; set; }
        public DataRow DRow { get; set; }
        public DataView Dv { get; set; }

        public WindowTreeRisk()
        {
            InitializeComponent();
            ChangeLanguage();
        }
        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextName, StringResources.DiagramNameLabel);
            BtnCancel.Content = StringResources.CancelButton;
            Title = StringResources.RiskDiagramTitle;
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (Operation == General.INSERT)
                //{
                //    Dv = TopRiskTable.DefaultView;
                //    dgTopRisk.ItemsSource = Dv;
                //}
                if (Operation == General.UPDATE)
                {
                    TextName.Text = DRow[DT_RiskTree.DIAGRAM_NAME].ToString();
                    //ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk();
                    //TopRiskTable = ws.GetRiskTree(new object[] { (Int32)DRow[DT_RiskTree.ID_RISK_TREE] }).Tables[DT_RiskTree_Damages.TABLENAME].Copy();

                    //BtnAdd.IsEnabled = false;
                    //BtnDel.IsEnabled = false;
                }
                Dv = TopRiskTable.DefaultView;
                Dv.RowFilter = DT_RiskTree_Damages.ID_RISKTREE + " = " + DRow[DT_RiskTree.ID_RISK_TREE];
                dgTopRisk.ItemsSource = Dv;
                TextName.Focus();
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
           
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowSelectTopRisk frm = new WindowSelectTopRisk
                {
                    Drow = TopRiskTable.NewRow(),
                    Icon = Icon
                };
                if (TopRiskTable.Select(DT_RiskTree_Damages.ID_RISKTREE + " = " + DRow[DT_RiskTree.ID_RISK_TREE]).Any())
                {
                    frm.TopRiskTable = TopRiskTable.Select(DT_RiskTree_Damages.ID_RISKTREE + " = " + DRow[DT_RiskTree.ID_RISK_TREE]).CopyToDataTable();
                }
                else
                {
                    frm.TopRiskTable = TopRiskTable.Clone();
                }
                if (frm.ShowDialog() == true)
                {
                    frm.Drow[DT_RiskTree_Damages.ID_RISKTREE] = DRow[DT_RiskTree.ID_RISK_TREE];
                    frm.Drow[DT_RiskTree_Damages.RISK_TREE] = DRow[DT_RiskTree.DIAGRAM_NAME];
                    TopRiskTable.Rows.Add(frm.Drow);
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();;
            }
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow fila = Dv[dgTopRisk.SelectedIndex].Row;
                if (dgTopRisk.SelectedIndex >= 0)
                {
                    WindowMessageYesNo msg = new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + fila[DT_RiskTree_Damages.DAMAGE] + "]?");
                    
                    if (msg.ShowDialog() == true)
                    {
                        if (Operation == General.UPDATE)
                        {
                            foreach (DataRow item in Risk_TopRisk.
                            Select(DT_Risk_Damages.ID_DAMAGE + " = " + TopRiskTable.Rows[dgTopRisk.SelectedIndex][DT_RiskTree_Damages.ID_DAMAGE] +
                            " and " + DT_Risk_Damages.ID_RISK_TREE + " = " + DRow[DT_RiskTree.ID_RISK_TREE]))
                            {
                                item.Delete();
                            }
                            foreach (DataRow itemi in CM_TopRisk.
                                Select(DT_CounterM_Damage.ID_DAMAGE + " = " + TopRiskTable.Rows[dgTopRisk.SelectedIndex][DT_RiskTree_Damages.ID_DAMAGE] +
                                " and " + DT_CounterM_Damage.ID_RISK_TREE + " = " + DRow[DT_RiskTree.ID_RISK_TREE]))
                            {
                                itemi.Delete();
                            }
                        }
                            
                        TopRiskTable.Rows[dgTopRisk.SelectedIndex].Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TextName.Text!="")
                {
                    DRow[DT_RiskTree.DIAGRAM_NAME] = TextName.Text;
                    DRow[DT_RiskTree.ID_WBS] = IdWBS;
                    this.DialogResult = true;
                }
                else
                {
                    new WindowMessageOK(StringResources.FIELD_REQUIRED).ShowDialog();
                }
                
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;Close();
        }
    }
}
