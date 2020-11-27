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
using System.ComponentModel;
using System.Windows.Controls.Primitives;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowCM.xaml
    /// </summary>
    /// 
    public partial class WindowCM : Window, INotifyPropertyChanged
    {
        private bool hasAccess = false;
        private decimal probability;
        private string _shortName;
        public string ShortName { get { return _shortName; } set { _shortName = value; OnPropertyChanged("ShortName"); } }
        public decimal Probability { get { return probability; } set { probability = value; OnPropertyChanged("Probability"); } }

        public bool HasAccess
        {
            get
            {
                return hasAccess;
            }

            set
            {
                hasAccess = value;
                OnPropertyChanged("HasAccess");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        public bool IS_USING_NAME { get; set; } = false;
        public bool IS_DELETING_WBS { get; set; } = false;
        public bool IS_DELETING_ROLE { get; set; } = false;

        public DataRow Selected_WBS_Row { get; set; }
        public DataRow Selected_Role_Row { get; set; }

        public decimal ID_Project { get; set; }
        public decimal ID_WBS { get; set; }
        public string WBS_NAME { get; set; }
        public string USER_NAME { get; set; }
        public string LOGIN_USER { get; set; }
        public DataRow CMRow { get; set; }
        public string Operation { get; set; }
        public decimal RiskTreeID { get; set; }
        public DataRow RowFather { get; set; }
        public DataSet Ds { get; set; }
        public DataView DvTopRisk { get; set; }
        public DataView DVCMWBS { get; set; }
        public DataView DvRoleCM { get; set; }
        public DataTable MyCM { get; set; }
        //public DataTable Ds.Tables[DT_CounterM_Damage.TABLE_NAME] { get; set; }
        //public DataTable Ds.Tables[DT_CM_WBS.TABLE_NAME] { get; set; }
        //public DataTable Ds.Tables[DT_Role_CM.TABLENAME] { get; set; }
        //public DataTable Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME] { get; set; }

        public bool HayQueIncrementar { get; set; }
        public RiskPolyLine RiskPadre { get; set; }
        public int Posicion { get; set; }
        private DataSet dsWBS;

        public WindowCM()
        {
            InitializeComponent();
            TextName.DataContext = this;
            TextDetail.DataContext = this;
            TextProbability.DataContext = this;
            gridTabRoles.DataContext = this;
            gridTabWBS.DataContext = this;
            ChangeLanguage();
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorMessageDialog.IsOpen = true;
            TextMessage.Text = text;
        }

        public void MostrarDialogYesNo(string textAlert)
        {
            YesNoDialog.IsOpen = true;
            TextYesNoMessage.Text = textAlert;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceWBS.WebServiceWBS wsWBS = new ServiceWBS.WebServiceWBS();
                dsWBS = wsWBS.GetAllWBSFiltered(new object[] { ID_Project }).Copy();
                wsWBS.Dispose();
                if (Operation == General.INSERT)
                {
                    CMRow[DT_CounterM.PROBABILITY] = 0;
                    TextFather.Text = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(RiskPadre.ID)[DT_Risk.NAMESHORT].ToString();
                    Probability = (decimal)CMRow[DT_CounterM.PROBABILITY];
                    SetTableCM_Damages();
                    //GIVING FATHER´S ROLE TO CHILD
                    SetTableCM_Role();
                    SetTableCM_WBS();
                }
                if (Operation == General.UPDATE)
                {
                    if (CMRow[DT_CounterM.USER_NAME] != DBNull.Value)
                    {
                        USER_NAME = CMRow[DT_CounterM.USER_NAME].ToString();
                    }

                    if (CMRow[DT_CounterM.WBS_NAME] != DBNull.Value)
                    {
                        WBS_NAME = CMRow[DT_CounterM.WBS_NAME].ToString();
                    }
                    TextName.Text = CMRow[DT_CounterM.NAMESHORT].ToString();
                    TextDetail.Text = CMRow[DT_CounterM.DETAIL].ToString();
                    Probability = (decimal)CMRow[DT_CounterM.PROBABILITY];
                    TextFather.Text = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(RiskPadre.ID)[DT_Risk.NAMESHORT].ToString();
                    if (CMRow[DT_CounterM.ID_WBS] != DBNull.Value)
                    {
                        ID_WBS = (decimal)CMRow[DT_Risk.ID_WBS];
                    }
                    foreach (DataRow item in Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + RiskTreeID))
                    {
                        if (!(Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Contains(new object[] { CMRow[DT_CounterM.ID], item[DT_Diagram_Damages.ID_DAMAGE] })))
                        {
                            DataRow rowTop = Ds.Tables[DT_CounterM_Damage.TABLE_NAME].NewRow();
                            rowTop[DT_CounterM_Damage.COLOR] = item[DT_Diagram_Damages.COLOR];
                            rowTop[DT_CounterM_Damage.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                            rowTop[DT_CounterM_Damage.ID_COUNTERM] = CMRow[DT_CounterM.ID];
                            rowTop[DT_CounterM_Damage.VALUE] = 0;
                            rowTop[DT_CounterM_Damage.TOP_RISK] = item[DT_Diagram_Damages.TOP_RISK];
                            rowTop[DT_CounterM_Damage.DAMAGE] = item[DT_Diagram_Damages.DAMAGE];
                            rowTop[DT_CounterM_Damage.ID_RISK_TREE] = RiskTreeID;
                            rowTop[DT_CounterM_Damage.RISK_REDUCTION] = 0;
                            rowTop[DT_CounterM_Damage.GROUPENAME] = "None";
                            rowTop[DT_CounterM_Damage.RISK] = CMRow[DT_CounterM.RISK_NAMESHORT];
                            rowTop[DT_CounterM_Damage.STATUS] = CMRow[DT_CounterM.ENABLED];
                            rowTop[DT_CounterM_Damage.IDRISK] = CMRow[DT_CounterM.ID_RISK];
                            Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Add(rowTop);
                        }
                    }
                }

                TextName.Focus();
                DvRoleCM = Ds.Tables[DT_Role_CM.TABLENAME].DefaultView;
                dgRoles.ItemsSource = DvRoleCM;
                DvRoleCM.RowFilter = DT_Role_CM.ID_CM + " = " + CMRow[DT_CounterM.ID];

                if (HasAccess)
                {
                    DVCMWBS = Ds.Tables[DT_CM_WBS.TABLE_NAME].DefaultView;
                    dgWBS.ItemsSource = DVCMWBS;
                    DVCMWBS.RowFilter = DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID];
                }
                else
                {
                    DVCMWBS = Ds.Tables[DT_CM_WBS.TABLE_NAME].DefaultView;
                    dgWBS.ItemsSource = DVCMWBS;
                    DVCMWBS.RowFilter = DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID] + " AND " + DT_CM_WBS.USERNAME + " = '" + LOGIN_USER + "'";
                }

                DvTopRisk = Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].DefaultView;
                dgTopRisk.ItemsSource = DvTopRisk;
                //DvTopRisk.RowFilter = DT_WBS_RISK_DAMAGE.ID_RISK + " = " + RiskRow[DT_Risk.ID] + " AND " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + ID_USER_WBS;
                DvTopRisk.RowFilter = DT_WBS_CM_Damage.ID_CM + " = " + CMRow[DT_CounterM.ID];
                if (dsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.USERNAME + " = '" + LOGIN_USER + "'").Any())
                {
                    string filter = "";
                    DataRow[] item = dsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.USERNAME + " = '" + LOGIN_USER + "'");
                    for (int i = 0; i < item.Count(); i++)
                    {
                        if (i == 0)
                        {
                            filter += " AND (" + DT_WBS_CM_Damage.ID_WBS + " = " + item[i][DT_WBS.ID_WBS];
                        }
                        else
                        {
                            filter += " OR " + DT_WBS_CM_Damage.ID_WBS + " = " + item[i][DT_WBS.ID_WBS];
                        }
                    }
                    filter += ")";
                    DvTopRisk.RowFilter += filter;
                }
                CalculateProbability();
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// Set the WBS from the Risk Father
        /// </summary>
        private void SetTableCM_WBS()
        {
            if (RiskPadre != null)
            {
                bool hasWBS = false;
                foreach (DataRow itemRiskWBS in Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskPadre.ID))
                {
                    DataRow newRow = Ds.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                    newRow[DT_CM_WBS.ID_CM] = CMRow[DT_CounterM.ID];
                    newRow[DT_CM_WBS.ID_WBS] = itemRiskWBS[DT_RISK_WBS.ID_WBS];
                    newRow[DT_CM_WBS.NIVEL] = itemRiskWBS[DT_RISK_WBS.NIVEL];
                    newRow[DT_CM_WBS.WBS] = itemRiskWBS[DT_RISK_WBS.WBS];
                    
                    newRow[DT_CM_WBS.IS_PRIMARY] = itemRiskWBS[DT_RISK_WBS.IS_PRIMARY];
                    newRow[DT_CM_WBS.PROBABILITY] = 0;
                    if ((bool)itemRiskWBS[DT_RISK_WBS.IS_PRIMARY])
                    {
                        ID_WBS = (decimal)itemRiskWBS[DT_RISK_WBS.ID_WBS];
                        WBS_NAME = itemRiskWBS[DT_RISK_WBS.NIVEL].ToString() + " " + itemRiskWBS[DT_RISK_WBS.WBS].ToString();
                        USER_NAME = itemRiskWBS[DT_RISK_WBS.USERNAME].ToString();
                    }
                    newRow[DT_CM_WBS.PRIMARY] = itemRiskWBS[DT_RISK_WBS.PRIMARY];
                    newRow[DT_CM_WBS.USERNAME] = itemRiskWBS[DT_RISK_WBS.USERNAME];
                    newRow[DT_CM_WBS.WBS_USER] = itemRiskWBS[DT_RISK_WBS.WBS] + "[" + itemRiskWBS[DT_RISK_WBS.USERNAME] + "]";
                    hasWBS = true;
                    Ds.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(newRow);
                }
                foreach (DataRow rowCm_WBS in Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID]))
                {
                    SetTableCM_WBS_Damage(rowCm_WBS);
                }
                CompleteCMWBSTable(hasWBS);
            }
        }

        /// <summary>
        /// Complete with the WBS Father of the Selected WBS
        /// </summary>
        private void CompleteCMWBSTable(bool hasWBS)
        {
            foreach (DataRow item in dsWBS.Tables[DT_WBS.TABLE_NAME].Select())
            {
                if (!(dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + item[DT_WBS.ID_WBS]).Any()))
                {
                    if (!(Ds.Tables[DT_CM_WBS.TABLE_NAME].Rows.Contains(new object[] { CMRow[DT_CounterM.ID], item[DT_WBS.ID_WBS] })))
                    {
                        DataRow drRCMWBS = Ds.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                        drRCMWBS[DT_CM_WBS.ID_CM] = CMRow[DT_CounterM.ID];
                        drRCMWBS[DT_CM_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        drRCMWBS[DT_CM_WBS.USERNAME] = item[DT_WBS.USERNAME];
                        drRCMWBS[DT_CM_WBS.PROBABILITY] = 0;
                        if (!hasWBS)
                        {
                            drRCMWBS[DT_CM_WBS.PRIMARY] = "Primary";
                            drRCMWBS[DT_CM_WBS.IS_PRIMARY] = true;
                        }
                        else
                        {
                            drRCMWBS[DT_CM_WBS.PRIMARY] = "";
                            drRCMWBS[DT_CM_WBS.IS_PRIMARY] = false;
                        }
                        Ds.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(drRCMWBS);
                    }
                }
            }
        }

        /// <summary>
        /// Validate if the WBS is in the low level of the risk and Add a Damage, if not, delete the damage and not have incidence on the line.
        /// </summary>
        private void SetTableCM_WBS_Damage(DataRow itemRiskWBS)
        {
            if (WBSOperations.IsCMWBSLow(itemRiskWBS, dsWBS, Ds.Tables[DT_CM_WBS.TABLE_NAME]))
            {
                //AddWBS_CM_Damage(itemRiskWBS);
                WBSOperations.TabAddWBS_LINE_Damage(itemRiskWBS, (decimal)CMRow[DT_CounterM.ID], true, Ds);
            }
            else
            {
                //DeleteWBS_CM_Damage(itemRiskWBS);
                WBSOperations.TabDeleteWBS_LINE_Damage(itemRiskWBS, (decimal)CMRow[DT_CounterM.ID], true, Ds);
            }
        }

        /// <summary>
        /// Set the Roles from the Risk Father
        /// </summary>
        private void SetTableCM_Role()
        {
            foreach (DataRow item in Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + RiskPadre.ID))
            {
                DataRow newRow = Ds.Tables[DT_Role_CM.TABLENAME].NewRow();
                newRow[DT_Role_CM.ID_CM] = CMRow[DT_CounterM.ID];
                newRow[DT_Role_CM.Role] = item[DT_Role_CM.Role];
                newRow[DT_Role_CM.IDROL_COLUMN] = item[DT_Role_CM.IDROL_COLUMN];
                Ds.Tables[DT_Role_CM.TABLENAME].Rows.Add(newRow);
            }
        }

        /// <summary>
        /// Add to CM the Damages from the Diagram
        /// </summary>
        private void SetTableCM_Damages()
        {
            foreach (DataRow item in Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + RiskTreeID))
            {
                DataRow rowTop = Ds.Tables[DT_CounterM_Damage.TABLE_NAME].NewRow();
                rowTop[DT_CounterM_Damage.COLOR] = item[DT_Diagram_Damages.COLOR];
                rowTop[DT_CounterM_Damage.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                rowTop[DT_CounterM_Damage.ID_COUNTERM] = CMRow[DT_CounterM.ID];
                rowTop[DT_CounterM_Damage.VALUE] = 0;
                rowTop[DT_CounterM_Damage.TOP_RISK] = item[DT_Diagram_Damages.TOP_RISK];
                rowTop[DT_CounterM_Damage.DAMAGE] = item[DT_Diagram_Damages.DAMAGE];
                rowTop[DT_CounterM_Damage.ID_RISK_TREE] = RiskTreeID;
                rowTop[DT_CounterM_Damage.RISK_REDUCTION] = 0;
                rowTop[DT_CounterM_Damage.GROUPENAME] = "None";
                rowTop[DT_CounterM_Damage.RISK] = TextFather.Text;
                rowTop[DT_CounterM_Damage.STATUS] = true;
                rowTop[DT_CounterM_Damage.IDRISK] = CMRow[DT_CounterM.ID_RISK];
                Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Add(rowTop);
            }
        }

        private void YesNoDialog_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true))
            {
                return;
            }
            if (Equals(eventArgs.Parameter, true))
            {
                if (IS_USING_NAME)
                {
                    AcceptCM();
                }
                if (IS_DELETING_ROLE)
                {
                    Delete_Role(Selected_Role_Row);
                }
                if (IS_DELETING_WBS)
                {
                    Delete_WBS(Selected_WBS_Row);
                }
            }
        }

        //TODO:  AQUI LA PROBABILIDAD
        private void CalculateProbability()
        {
            try
            {
                List<decimal> Probabilities = new List<decimal>();
                foreach (DataRow rowCmWbs in Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID]))
                {
                    if (WBSOperations.IsCMWBSLow(rowCmWbs, dsWBS, Ds.Tables[DT_CM_WBS.TABLE_NAME]))
                    {
                        if (rowCmWbs[DT_CM_WBS.PROBABILITY] == DBNull.Value)
                        {
                            Probabilities.Add(0);
                        }
                        else
                        {
                            Probabilities.Add((decimal)rowCmWbs[DT_CM_WBS.PROBABILITY]);
                        }
                    }
                    else
                    {
                        rowCmWbs[DT_CM_WBS.PROBABILITY] = 0;
                    }
                }
                if (Probabilities.Count > 0)
                {
                    Probability = Probabilities.Sum() / Probabilities.Count;
                }
                else
                {
                    Probability = 0;
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
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

        private void BtnAddRole_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole();
                DataTable roleCodif = ws.GetRolesData().Tables[DT_Role.ROLE_TABLE].Copy();
                ws.Dispose();
                WindowSelection frmSelection = new WindowSelection();
                if (Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + (decimal)CMRow[DT_CounterM.ID]).Count() > 0)
                {
                    frmSelection.Dt = General.DeleteExists(roleCodif, Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + (decimal)CMRow[DT_CounterM.ID]).CopyToDataTable(), DT_Role.IDROL_COLUMN);
                }
                else
                {
                    frmSelection.Dt = roleCodif;
                }

                frmSelection.DcolumToShow = new string[] { DT_Role.ROLE_COLUM };
                frmSelection.DcolumToShowAlias = new string[] { DT_Role.ROLE_COLUM };
                frmSelection.FilterString = DT_Role.ROLE_COLUM;
                frmSelection.ColumnToFilter = DT_Role.ROLE_COLUM;
                frmSelection.Title = DT_Role.ROLE_COLUM;

                if (frmSelection.ShowDialog() == true)
                {
                    foreach (DataRow item in frmSelection.RowsSelected)
                    {
                        DataRow drRole = Ds.Tables[DT_Role_CM.TABLENAME].NewRow();
                        drRole[DT_Role_CM.ID_CM] = CMRow[DT_CounterM.ID];
                        drRole[DT_Role_CM.NAME_SHORT] = TextName.Text;
                        drRole[DT_Role_CM.Role] = item[DT_Role.ROLE_COLUM];
                        drRole[DT_Role_CM.IDROL_COLUMN] = item[DT_Role.IDROL_COLUMN];
                        Ds.Tables[DT_Role_CM.TABLENAME].Rows.Add(drRole);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Delete_Role(DataRow fila)
        {
            try
            {
                fila.Delete();
                IS_DELETING_ROLE = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_ROLE = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnDelRole_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgRoles.SelectedIndex >= 0)
                {
                    Selected_Role_Row = DvRoleCM[dgRoles.SelectedIndex].Row;
                    if (Selected_Role_Row[DT_Role_CM.Role].ToString() != "Administrator")
                    {
                        IS_DELETING_ROLE = true;
                        MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + Selected_Role_Row[DT_Role_CM.Role].ToString() + "]?");
                    }
                    else
                    {
                        MostrarErrorDialog("'Administrator' role can't be deleted!");
                    }
                }
            }
            catch (Exception ex)
            {
                IS_DELETING_ROLE = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void AcceptCM()
        {
            CMRow[DT_CounterM.NAMESHORT] = TextName.Text;
            CMRow[DT_CounterM.DETAIL] = TextDetail.Text;
            CMRow[DT_CounterM.ID_DIAGRAM] = RiskTreeID;
            CMRow[DT_CounterM.ID_RISK] = RiskPadre.ID;
            CMRow[DT_CounterM.POSITION] = Posicion;
            CMRow[DT_CounterM.ENABLED] = true;
            CMRow[DT_CounterM.ID_WBS] = ID_WBS;
            CMRow[DT_CounterM.WBS_NAME] = WBS_NAME;
            CMRow[DT_CounterM.USER_NAME] = USER_NAME;
            RefreshDamageValues((decimal)CMRow[DT_CounterM.ID]);
            if (Versioned.IsNumeric(TextProbability.Text))
            {
                if (Probability > 100)
                {
                    MostrarErrorDialog(StringResources.PROBABILITY_FIELD);
                }
                else
                {
                    bool flag = true;
                    foreach (DataRow item in Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
                    {
                        if ((decimal)item[DT_CounterM_Damage.VALUE] < 0)
                        {
                            flag = false;
                            MostrarErrorDialog("Value of damages cannot be negative");
                            break;
                        }
                    }
                    if (flag)
                    {
                        CalculateProbability();
                        CMRow[DT_CounterM.PROBABILITY] = Probability;
                        this.DialogResult = true;
                    }
                }
                IS_USING_NAME = false;
            }
            else
            {
                IS_USING_NAME = false;
                MostrarErrorDialog(StringResources.NUMERIC_FIELD);
            }
        }

        private void RefreshDamageValues(decimal idCM)
        {
            foreach (DataRow itemDamage in Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + idCM))
            {//primero recorro los Daños de los riesgos
                decimal valor = 0;
                foreach (DataRow itemWBS in Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Select(DT_WBS_CM_Damage.ID_CM + " = " + idCM + " AND " + DT_WBS_CM_Damage.ID_DAMAGE + " = " + itemDamage[DT_CounterM_Damage.ID_DAMAGE]))
                {//y despues para  sumarlos todos en un mismo daño y encontrar el AD
                    valor += (decimal)itemWBS[DT_WBS_CM_Damage.VALUE];
                }
                itemDamage[DT_CounterM_Damage.VALUE] = valor;
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TextName.Text != "")
                {
                    if (MyCM.Select(DT_CounterM.ID_DIAGRAM + " = " + RiskTreeID + " and "
                        + DT_CounterM.NAMESHORT + " = '" + TextName.Text + "' and " + DT_CounterM.ID + " <> " + CMRow[DT_CounterM.ID]).Any())
                    {
                        IS_USING_NAME = true;
                        MostrarDialogYesNo("The name [" + TextName.Text + "] Already exists in this diagram. Do you want to use it again?");
                    }
                    else
                    {
                        AcceptCM();
                    }
                    foreach (DataRow item in Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
                    {
                        item[DT_CounterM_Damage.RISK_REDUCTION] = CMRow[DT_CounterM.PROBABILITY];
                        item[DT_CounterM_Damage.COUNTERM_NAMESHORT] = CMRow[DT_CounterM.NAMESHORT].ToString();
                        item[DT_CounterM_Damage.ID_WBS] = ID_WBS;
                        item[DT_CounterM_Damage.WBS_NAME] = WBS_NAME;
                        item[DT_CounterM_Damage.USERNAME] = USER_NAME;
                    }
                }
                else
                {
                    MostrarErrorDialog(StringResources.FIELD_REQUIRED);
                }
            }
            catch (Exception EX)
            {
                IS_USING_NAME = false;
                MostrarErrorDialog(EX.Message);
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


        private void BtnAddWBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                DataTable roleCodif = ws.GetAllWBSFiltered(new object[] { ID_Project }).Tables[DT_WBS.TABLE_NAME].Copy();
                ws.Dispose();
                WindowSelection frmSelection = new WindowSelection();
                if (Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + (decimal)CMRow[DT_CounterM.ID]).Count() > 0)
                {
                    frmSelection.Dt = General.DeleteExists(roleCodif, Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + (decimal)CMRow[DT_CounterM.ID]).CopyToDataTable(), DT_WBS.ID_WBS);
                }
                else
                {
                    frmSelection.Dt = roleCodif;
                }

                frmSelection.DcolumToShow = new string[] { DT_WBS.NIVEL, DT_WBS.WBS_NAME };
                frmSelection.DcolumToShowAlias = new string[] { "Level", DT_WBS.WBS_NAME };
                frmSelection.Title = "WBS";
                frmSelection.FilterString = "WBS";
                frmSelection.ColumnToFilter = DT_WBS.WBS_NAME;
                if (frmSelection.ShowDialog() == true)
                {
                    foreach (DataRow item in frmSelection.RowsSelected)
                    {
                        DataRow drCMWBS = Ds.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                        drCMWBS[DT_CM_WBS.ID_CM] = CMRow[DT_CounterM.ID];
                        drCMWBS[DT_CM_WBS.CM] = TextName.Text;
                        drCMWBS[DT_CM_WBS.WBS] = item[DT_WBS.WBS_NAME].ToString().TrimStart();
                        drCMWBS[DT_CM_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        drCMWBS[DT_CM_WBS.NIVEL] = item[DT_WBS.NIVEL].ToString().TrimStart();
                        drCMWBS[DT_CM_WBS.IS_PRIMARY] = false;
                        drCMWBS[DT_CM_WBS.PRIMARY] = "";
                        drCMWBS[DT_CM_WBS.USERNAME] = item[DT_WBS.USERNAME];
                        drCMWBS[DT_CM_WBS.WBS_USER] = drCMWBS[DT_CM_WBS.WBS] + "[" + drCMWBS[DT_CM_WBS.USERNAME] + "]";
                        drCMWBS[DT_CM_WBS.PROBABILITY] = 0;
                        Ds.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(drCMWBS);
                    }
                    foreach (DataRow itemRISKWBSi in Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID]))
                    {
                        foreach (DataRow itemAncestors in WBSOperations.GetAncestors((decimal)itemRISKWBSi[DT_RISK_WBS.ID_WBS], dsWBS.Tables[DT_WBS.TABLE_NAME].Clone(), dsWBS).Rows)
                        {
                            if (!(Ds.Tables[DT_CM_WBS.TABLE_NAME].Rows.Contains(new object[] { CMRow[DT_CounterM.ID], itemAncestors[DT_WBS.ID_WBS] })))
                            {
                                DataRow drRiskWBSi = Ds.Tables[DT_CM_WBS.TABLE_NAME].NewRow();
                                drRiskWBSi[DT_CM_WBS.ID_CM] = CMRow[DT_CounterM.ID];
                                drRiskWBSi[DT_CM_WBS.CM] = TextName.Text;
                                drRiskWBSi[DT_CM_WBS.WBS] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart();
                                drRiskWBSi[DT_CM_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                drRiskWBSi[DT_CM_WBS.NIVEL] = itemAncestors[DT_WBS.NIVEL].ToString().TrimStart();
                                drRiskWBSi[DT_CM_WBS.USERNAME] = itemAncestors[DT_WBS.USERNAME];
                                drRiskWBSi[DT_CM_WBS.WBS_USER] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart() + "[" + itemAncestors[DT_WBS.USERNAME] + "]";
                                drRiskWBSi[DT_CM_WBS.IS_PRIMARY] = false;
                                drRiskWBSi[DT_CM_WBS.PRIMARY] = "";
                                drRiskWBSi[DT_CM_WBS.PROBABILITY] = 0;
                                Ds.Tables[DT_CM_WBS.TABLE_NAME].Rows.Add(drRiskWBSi);
                            }
                        }
                    }
                    foreach (DataRow itemWBS in Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID]))
                    {
                        if (WBSOperations.IsCMWBSLow(itemWBS, dsWBS, Ds.Tables[DT_CM_WBS.TABLE_NAME]))
                        {
                            //AddWBS_CM_Damage(itemWBS);
                            WBSOperations.TabAddWBS_LINE_Damage(itemWBS, (decimal)CMRow[DT_CounterM.ID], true, Ds);
                        }
                        else
                        {///TENGO QUE BORRAR EL DAMAGE_WBS_CM, PUES YA NO ES LOWLEVEL
                            //DeleteWBS_CM_Damage(itemWBS);
                            WBSOperations.TabDeleteWBS_LINE_Damage(itemWBS, (decimal)CMRow[DT_CounterM.ID], true, Ds);
                        }
                    }
                    CalculateProbability();
                    RefreshDamageValues((decimal)CMRow[DT_CounterM.ID]);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Delete_WBS(DataRow fila)
        {
            try
            {
                foreach (DataRow itemR in (Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Select(DT_WBS_CM_Damage.ID_WBS + " = " + fila[DT_WBS.ID_WBS] + " AND " + DT_WBS_CM_Damage.ID_CM + " = " + CMRow[DT_CounterM.ID])))
                {
                    itemR.Delete();
                }
                //SetDefaultWBSPrimary(fila);
                WBSOperations.SetDefaultWBSPrimary(fila, true, Ds, (decimal)CMRow[DT_CounterM.ID]);
                fila.Delete();
                foreach (DataRow itemWBS in Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID]))
                {
                    if (WBSOperations.IsCMWBSLow(itemWBS, dsWBS, Ds.Tables[DT_CM_WBS.TABLE_NAME]))
                    {
                        //AddWBS_CM_Damage(itemWBS);
                        WBSOperations.TabAddWBS_LINE_Damage(itemWBS, (decimal)CMRow[DT_CounterM.ID], true, Ds);

                    }
                    else
                    {///TENGO QUE BORRAR EL DAMAGE_WBS_CM, PUES YA NO ES LOWLEVEL
                        //DeleteWBS_CM_Damage(itemWBS);
                        WBSOperations.TabDeleteWBS_LINE_Damage(itemWBS, (decimal)CMRow[DT_CounterM.ID], true, Ds);

                    }
                }
                CalculateProbability();
                RefreshDamageValues((decimal)CMRow[DT_CounterM.ID]);
                IS_DELETING_WBS = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_WBS = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnDelWBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgWBS.SelectedIndex >= 0)
                {
                    Selected_WBS_Row = DVCMWBS[dgWBS.SelectedIndex].Row;
                    IS_DELETING_WBS = true;
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + Selected_WBS_Row[DT_CM_WBS.WBS].ToString() + "]?");
                }
            }
            catch (Exception ex)
            {
                IS_DELETING_WBS = false;
                MostrarErrorDialog(ex.Message);
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
                MostrarErrorDialog("Insert a Numeric Value!");
            }
        }

        private void DgWBS_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            CalculateProbability();
        }

        private void DgWBS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateProbability();
        }

        private void DgWBS_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculateProbability();
        }

        private void DgWBS_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            CalculateProbability();
        }

        private void TextName_TextChanged(object sender, TextChangedEventArgs e)
        {
            ShortName = TextName.Text;
        }

        private void KeyToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal idWBS = (decimal)((DataRowView)((ToggleButton)e.Source).DataContext).Row[DT_CM_WBS.ID_WBS];
                if (dgWBS.SelectedIndex >= 0)
                {
                    DVCMWBS[dgWBS.SelectedIndex].Row[DT_CM_WBS.IS_PRIMARY] = true;
                    DVCMWBS[dgWBS.SelectedIndex].Row[DT_CM_WBS.PRIMARY] = "PRIMARY";
                    foreach (DataRow item in DVCMWBS.Table.Select(DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID] + " and " + DT_CM_WBS.ID_WBS + " <> " + idWBS))
                    {
                        item[DT_CM_WBS.IS_PRIMARY] = false;
                        item[DT_CM_WBS.PRIMARY] = "";
                    }
                    ID_WBS = (decimal)DVCMWBS[dgWBS.SelectedIndex].Row[DT_CM_WBS.ID_WBS];
                    WBS_NAME = DVCMWBS[dgWBS.SelectedIndex].Row[DT_CM_WBS.NIVEL].ToString() + " " + DVCMWBS[dgWBS.SelectedIndex].Row[DT_CM_WBS.WBS].ToString();
                    USER_NAME = DVCMWBS[dgWBS.SelectedIndex].Row[DT_CM_WBS.USERNAME].ToString();
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void CancelCM()
        {
            if (Operation == General.INSERT)
            {
                foreach (DataRow rowCM_Damage in Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
                {
                    rowCM_Damage.Delete();
                }
                foreach (DataRow rowCM_wbs in Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID]))
                {
                    rowCM_wbs.Delete();
                }
                foreach (DataRow rowCM_WBS_Damage in Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Select(DT_WBS_CM_Damage.ID_CM + " = " + CMRow[DT_CounterM.ID]))
                {
                    rowCM_WBS_Damage.Delete();
                }
                foreach (DataRow rowCM_Role in Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + CMRow[DT_CounterM.ID]))
                {
                    rowCM_Role.Delete();
                }
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DialogResult == false || DialogResult == null)
            {
                CancelCM();
            }
        }

        private void TextProbability_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                //Probability = decimal.Parse(TextProbability.Text);
                General.RecalculateProbability(CMRow, Ds, Probability, true);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
    }
}
