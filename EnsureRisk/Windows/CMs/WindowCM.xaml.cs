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
using Microsoft.VisualBasic.CompilerServices;
using EnsureBusinesss;
using EnsureBusinesss.Business;
using EnsureRisk.Resources;
using System.Text.RegularExpressions;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowCM.xaml
    /// </summary>
    public partial class WindowCM : Window
    {
        public DataRow CMRow { get; set; }
        public string Operation { get; set; }
        public int RiskTreeID { get; set; }
        public DataRow RowFather { get; set; }
        public DataSet DsCM { get; set; }
        public DataView DvTopRisk { get; set; }
        public DataView DVCMWBS { get; set; }
        public DataView DvRoleCM { get; set; }
        public DataTable MyCM { get; set; }
        public DataTable TopRiskTable { get; set; }
        public DataTable CM_WBS_Table { get; set; }
        public DataTable CM_RoleTable { get; set; }
        public bool HayQueIncrementar { get; set; }
        public RiskPolyLine RiskPadre { get; set; }
        public int Posicion { get; set; }
        public decimal Probability { get; set; }
        public WindowCM()
        {
            InitializeComponent();
            ChangeLanguage();
        }

        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextName, StringResources.ShortNameText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextFather, StringResources.FatherText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextDetail, StringResources.RiskDetailText);
            //LabelProbability.Content = StringResources.ProbabilityLabel;
            BtnCancel.Content = StringResources.CancelButton;
            tabRoles.Header = StringResources.TabValues;
            tabRoles.Header = StringResources.TabRoles;
            //Title = StringResources.RiskTitle;
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole();
                DataTable roleCodif = ws.GetRolesData().Tables[DT_Role.ROLE_TABLE].Copy();
                WindowSelection frmSelection = new WindowSelection();
                if (CM_RoleTable.Select(DT_Role_CM.ID_CM + " = " + (Int32)CMRow[DT_CounterM.ID_COLUMNA]).Count() > 0)
                {
                    frmSelection.dt = General.DeleteExists(roleCodif, CM_RoleTable.Select(DT_Role_CM.ID_CM + " = " + (Int32)CMRow[DT_CounterM.ID_COLUMNA]).CopyToDataTable(), DT_Role.IDROL_COLUMN);
                }
                else
                {
                    frmSelection.dt = roleCodif;
                }

                frmSelection.dcolumToShow = new string[] { DT_Role.ROLE_COLUM };
                frmSelection.dcolumToShowAlias = new string[] { DT_Role.ROLE_COLUM };
                frmSelection.Title = DT_Role.ROLE_COLUM;

                if (frmSelection.ShowDialog() == true)
                {
                    foreach (DataRow item in frmSelection.RowsSelected)
                    {
                        DataRow drRole = CM_RoleTable.NewRow();
                        drRole[DT_Role_CM.ID_CM] = CMRow[DT_CounterM.ID_COLUMNA];
                        drRole[DT_Role_CM.NAME_SHORT] = TextName.Text;
                        drRole[DT_Role_CM.Role] = item[DT_Role.ROLE_COLUM];
                        drRole[DT_Role_CM.IDROL_COLUMN] = item[DT_Role.IDROL_COLUMN];
                        CM_RoleTable.Rows.Add(drRole);
                    }                    
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog(); ;
            }
        }

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            DataRow fila = DvRoleCM[dgRoles.SelectedIndex].Row;
            if (fila[DT_Role_CM.Role].ToString() != "Administrator")
            {
                if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + fila[DT_Role_CM.Role].ToString() + "]?").ShowDialog() == true)
                {
                    fila.Delete();
                }
            }
            else
            {
                new WindowMessageOK("'Administrator' role can't be deleted!").ShowDialog();
            }            
        }
        private void AcceptCM()
        {
            CMRow[DT_CounterM.NAMESHORT_COLUMNA] = TextName.Text;
            CMRow[DT_CounterM.DETAIL_COLUMNA] = TextDetail.Text;
            CMRow[DT_CounterM.ID_RISK_TREE] = RiskTreeID;
            CMRow[DT_CounterM.ID_RISK] = RiskPadre.ID;
            CMRow[DT_CounterM.POSITION_COLUMN] = Posicion;
            CMRow[DT_CounterM.ENABLED_COLUMN] = true;
           
            if (Versioned.IsNumeric(TextProbability.Text))
            {
                if (decimal.Parse(TextProbability.Text) > 100)
                {
                    new WindowMessageOK(StringResources.PROBABILITY_FIELD).ShowDialog();
                }
                else
                {
                    bool flag = true;
                    foreach (DataRow item in TopRiskTable.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID_COLUMNA]))
                    {
                        if ((decimal)item[DT_CounterM_Damage.VALUE] < 0)
                        {
                            flag = false;
                            new WindowMessageOK("Value of damages cannot be negative").ShowDialog();
                            break;
                        }
                    }
                    if (flag)
                    {
                        CMRow[DT_CounterM.PROBABILITY_COLUMN] = decimal.Parse(TextProbability.Text);
                        this.DialogResult = true;
                    }
                }
                foreach (DataRow item in TopRiskTable.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID_COLUMNA]))
                {
                    item[DT_CounterM_Damage.RISK_REDUCTION] = CMRow[DT_CounterM.PROBABILITY_COLUMN];
                    item[DT_CounterM_Damage.COUNTERM_NAMESHORT] = CMRow[DT_CounterM.NAMESHORT_COLUMNA].ToString();
                }
            }
            else
            {
                new WindowMessageOK(StringResources.NUMERIC_FIELD).ShowDialog();
            }
        }
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TextName.Text != "")
                {
                    if (MyCM.Select(DT_CounterM.ID_RISK_TREE + " = " + RiskTreeID + " and "
                        + DT_CounterM.NAMESHORT_COLUMNA + " = '" + TextName.Text + "' and " + DT_CounterM.ID_COLUMNA + " <> " + CMRow[DT_CounterM.ID_COLUMNA]).Any())
                    {
                        WindowMessageYesNo yesNo = new WindowMessageYesNo("The name [" + TextName.Text + "] Already exists in this diagram. Do you want to use it again?");
                        yesNo.ShowDialog();
                        if (yesNo.DialogResult == true)
                        {
                            AcceptCM();
                        }
                    }
                    else
                    {
                        AcceptCM();
                    }
                }
                else
                {
                    new WindowMessageOK(StringResources.FIELD_REQUIRED).ShowDialog();
                }
            }
            catch (Exception EX)
            {
                MessageBox.Show(EX.Message);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Operation == General.INSERT)
                {
                    TextProbability.Text = Probability.ToString();
                    TextFather.Text = DsCM.Tables[DT_Risk.TABLE_NAME].Rows.Find(RiskPadre.ID)[DT_Risk.NAMESHORT_COLUMNA].ToString();

                    foreach (DataRow item in DsCM.Tables[DT_RiskTree_Damages.TABLENAME].Select(DT_RiskTree_Damages.ID_RISKTREE + " = " + RiskTreeID))
                    {
                        DataRow rowTop = TopRiskTable.NewRow();
                        rowTop[DT_CounterM_Damage.COLOR] = item[DT_RiskTree_Damages.COLOR];
                        rowTop[DT_CounterM_Damage.ID_DAMAGE] = item[DT_RiskTree_Damages.ID_DAMAGE];
                        rowTop[DT_CounterM_Damage.ID_COUNTERM] = CMRow[DT_CounterM.ID_COLUMNA];
                        rowTop[DT_CounterM_Damage.VALUE] = 0;
                        rowTop[DT_CounterM_Damage.TOP_RISK] = item[DT_RiskTree_Damages.DAMAGE];
                        rowTop[DT_CounterM_Damage.DAMAGE] = item[DT_RiskTree_Damages.DAMAGE].ToString() + "(" + item[DT_RiskTree_Damages.UM] + ")";
                        rowTop[DT_CounterM_Damage.ID_RISK_TREE] = RiskTreeID;
                        rowTop[DT_CounterM_Damage.RISK_REDUCTION] = 0;
                        rowTop[DT_CounterM_Damage.GROUPENAME] = "None";
                        rowTop[DT_CounterM_Damage.RISK] = TextFather.Text;
                        rowTop[DT_CounterM_Damage.STATUS] = "Activated";
                        rowTop[DT_CounterM_Damage.IDRISK] = CMRow[DT_CounterM.ID_RISK];
                        TopRiskTable.Rows.Add(rowTop);
                    }
                    //GIVING FATHER´S ROLE TO CHILD
                    foreach (DataRow item in DsCM.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + RiskPadre.ID))
                    {
                        DataRow newRow = CM_RoleTable.NewRow();
                        newRow[DT_Role_CM.ID_CM] = CMRow[DT_CounterM.ID_COLUMNA];
                        newRow[DT_Role_CM.Role] = item[DT_Role_CM.Role];
                        newRow[DT_Role_CM.IDROL_COLUMN] = item[DT_Role_CM.IDROL_COLUMN];
                        CM_RoleTable.Rows.Add(newRow);
                    }
                }
                if (Operation == General.UPDATE)
                {
                    TextName.Text = CMRow[DT_CounterM.NAMESHORT_COLUMNA].ToString();
                    TextDetail.Text = CMRow[DT_CounterM.DETAIL_COLUMNA].ToString();
                    TextProbability.Text = CMRow[DT_CounterM.PROBABILITY_COLUMN].ToString();
                    TextFather.Text = DsCM.Tables[DT_Risk.TABLE_NAME].Rows.Find(RiskPadre.ID)[DT_Risk.NAMESHORT_COLUMNA].ToString();
                    foreach (DataRow item in DsCM.Tables[DT_RiskTree_Damages.TABLENAME].Select(DT_RiskTree_Damages.ID_RISKTREE + " = " + RiskTreeID))
                    {
                        if (!(TopRiskTable.Rows.Contains(new object[] { CMRow[DT_CounterM.ID_COLUMNA], item[DT_RiskTree_Damages.ID_DAMAGE] })))
                        {
                            DataRow rowTop = TopRiskTable.NewRow();
                            rowTop[DT_CounterM_Damage.COLOR] = item[DT_RiskTree_Damages.COLOR];
                            rowTop[DT_CounterM_Damage.ID_DAMAGE] = item[DT_RiskTree_Damages.ID_DAMAGE];
                            rowTop[DT_CounterM_Damage.ID_COUNTERM] = CMRow[DT_CounterM.ID_COLUMNA];
                            rowTop[DT_CounterM_Damage.VALUE] = 0;
                            rowTop[DT_CounterM_Damage.TOP_RISK] = item[DT_RiskTree_Damages.DAMAGE];
                            rowTop[DT_CounterM_Damage.DAMAGE] = item[DT_RiskTree_Damages.DAMAGE].ToString() + "(" + item[DT_RiskTree_Damages.UM] + ")";
                            rowTop[DT_CounterM_Damage.ID_RISK_TREE] = RiskTreeID;
                            rowTop[DT_CounterM_Damage.RISK_REDUCTION] = 0;
                            rowTop[DT_CounterM_Damage.GROUPENAME] = "None";
                            rowTop[DT_CounterM_Damage.RISK] = CMRow[DT_CounterM.RISK_NAMESHORT_COLUMNA];
                            rowTop[DT_CounterM_Damage.STATUS] = (Boolean)CMRow[DT_CounterM.ENABLED_COLUMN] ? "Activated" : "Non Activated";
                            rowTop[DT_CounterM_Damage.IDRISK] = CMRow[DT_CounterM.ID_RISK];
                            TopRiskTable.Rows.Add(rowTop);
                        }
                    }
                }
                TextName.Focus();
                DvRoleCM = CM_RoleTable.DefaultView;
                dgRoles.ItemsSource = DvRoleCM;
                DvRoleCM.RowFilter = DT_Role_CM.ID_CM + " = " + CMRow[DT_CounterM.ID_COLUMNA];

                DVCMWBS = CM_WBS_Table.DefaultView;
                dgWBS.ItemsSource = DVCMWBS;
                DVCMWBS.RowFilter = DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID_COLUMNA];

                DvTopRisk = TopRiskTable.DefaultView;
                dgTopRisk.ItemsSource = DvTopRisk;
                DvTopRisk.RowFilter = DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID_COLUMNA];
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnAddWBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                DataTable roleCodif = ws.GetAllWBS().Tables[DT_WBS.TABLE_NAME].Copy();
                WindowSelection frmSelection = new WindowSelection();
                if (CM_WBS_Table.Select(DT_CM_WBS.ID_CM + " = " + (Int32)CMRow[DT_CounterM.ID_COLUMNA]).Count() > 0)
                {
                    frmSelection.dt = General.DeleteExists(roleCodif, CM_WBS_Table.Select(DT_CM_WBS.ID_CM + " = " + (Int32)CMRow[DT_CounterM.ID_COLUMNA]).CopyToDataTable(), DT_WBS.ID_WBS);
                }
                else
                {
                    frmSelection.dt = roleCodif;
                }

                frmSelection.dcolumToShow = new string[] { DT_WBS.NIVEL, DT_WBS.WBS_NAME };
                frmSelection.dcolumToShowAlias = new string[] { "Level", DT_WBS.WBS_NAME };
                frmSelection.Title = "WBS";
                if (frmSelection.ShowDialog() == true)
                {
                    foreach (DataRow item in frmSelection.RowsSelected)
                    {
                        DataRow drCMWBS = CM_WBS_Table.NewRow();
                        drCMWBS[DT_CM_WBS.ID_CM] = CMRow[DT_CounterM.ID_COLUMNA];
                        drCMWBS[DT_CM_WBS.CM] = TextName.Text;
                        drCMWBS[DT_CM_WBS.WBS] = item[DT_WBS.WBS_NAME];
                        drCMWBS[DT_CM_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        drCMWBS[DT_CM_WBS.NIVEL] = item[DT_WBS.NIVEL];
                        CM_WBS_Table.Rows.Add(drCMWBS);
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }

        }

        private void BtnDelWBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow fila = DVCMWBS[dgWBS.SelectedIndex].Row;                
                if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + fila[DT_CM_WBS.WBS].ToString() + "]?").ShowDialog() == true)
                {
                    fila.Delete();
                }
            }
            catch (Exception EX)
            {
                new WindowMessageOK(EX.Message).ShowDialog();
            }
        }

        private void TextProbability_LostFocus(object sender, RoutedEventArgs e)
        {
           
        }

        private void TextProbability_LostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (!(Versioned.IsNumeric(TextProbability.Text)))
            {
                new WindowMessageOK(StringResources.NUMERIC_FIELD).ShowDialog();
                TextProbability.Focus();
                TextProbability.SelectAll();
            }
            else
            {
                if (decimal.Parse(TextProbability.Text) > 100)
                {
                    new WindowMessageOK(StringResources.PROBABILITY_FIELD).ShowDialog();
                    TextProbability.Focus();
                    TextProbability.SelectAll();
                }
                else
                {
                    if (decimal.Parse(TextProbability.Text) < 0)
                    {
                        new WindowMessageOK(StringResources.PROBABILITY_FIELD).ShowDialog();
                        TextProbability.Focus();
                        TextProbability.SelectAll();
                    }
                }
            }
        }

        private void DgTopRisk_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.NumPad0 && e.Key != Key.NumPad1 && e.Key != Key.NumPad2 && e.Key != Key.NumPad3 && e.Key != Key.NumPad4 && e.Key != Key.NumPad5 && e.Key != Key.NumPad6 &&
                e.Key != Key.NumPad7 && e.Key != Key.NumPad8 && e.Key != Key.NumPad9 && e.Key != Key.Decimal && e.Key != Key.Back &&
                e.Key != Key.Delete && e.Key != Key.OemComma && e.Key != Key.OemPeriod &&
                e.Key != Key.D0 && e.Key != Key.D1 && e.Key != Key.D2 && e.Key != Key.D3 && e.Key != Key.D4 && e.Key != Key.D5 && e.Key != Key.D6 &&
                e.Key != Key.D7 && e.Key != Key.D8 && e.Key != Key.D9)
            {
                new WindowMessageOK("Insert a Numeric Value!").ShowDialog();
            }
        }
    }
}
