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
        public DataSet DsCM { get; set; }
        public DataView DvTopRisk { get; set; }
        public DataView DVCMWBS { get; set; }
        public DataView DvRoleCM { get; set; }
        public DataTable MyCM { get; set; }
        public DataTable CM_Damage_Table { get; set; }
        public DataTable CM_WBS_Table { get; set; }
        public DataTable CM_RoleTable { get; set; }
        public bool HayQueIncrementar { get; set; }
        public RiskPolyLine RiskPadre { get; set; }
        public int Posicion { get; set; }
        public DataTable WBS_CM_Damage { get; set; }
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
                    TextProbability.Text = Probability.ToString();
                    TextFather.Text = DsCM.Tables[DT_Risk.TABLE_NAME].Rows.Find(RiskPadre.ID)[DT_Risk.NAMESHORT].ToString();
                    foreach (DataRow item in DsCM.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + RiskTreeID))
                    {
                        DataRow rowTop = CM_Damage_Table.NewRow();
                        rowTop[DT_CounterM_Damage.COLOR] = item[DT_Diagram_Damages.COLOR];
                        rowTop[DT_CounterM_Damage.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                        rowTop[DT_CounterM_Damage.ID_COUNTERM] = CMRow[DT_CounterM.ID];
                        rowTop[DT_CounterM_Damage.VALUE] = 0;
                        rowTop[DT_CounterM_Damage.TOP_RISK] = item[DT_Diagram_Damages.TOP_RISK];
                        rowTop[DT_CounterM_Damage.DAMAGE] = item[DT_Diagram_Damages.TOP_RISK] + "(" + item[DT_Diagram_Damages.TOP_RISK] + ")";
                        rowTop[DT_CounterM_Damage.ID_RISK_TREE] = RiskTreeID;
                        rowTop[DT_CounterM_Damage.RISK_REDUCTION] = 0;
                        rowTop[DT_CounterM_Damage.GROUPENAME] = "None";
                        rowTop[DT_CounterM_Damage.RISK] = TextFather.Text;
                        rowTop[DT_CounterM_Damage.STATUS] = true;
                        rowTop[DT_CounterM_Damage.IDRISK] = CMRow[DT_CounterM.ID_RISK];
                        CM_Damage_Table.Rows.Add(rowTop);
                    }
                    //GIVING FATHER´S ROLE TO CHILD
                    foreach (DataRow item in DsCM.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + RiskPadre.ID))
                    {
                        DataRow newRow = CM_RoleTable.NewRow();
                        newRow[DT_Role_CM.ID_CM] = CMRow[DT_CounterM.ID];
                        newRow[DT_Role_CM.Role] = item[DT_Role_CM.Role];
                        newRow[DT_Role_CM.IDROL_COLUMN] = item[DT_Role_CM.IDROL_COLUMN];
                        CM_RoleTable.Rows.Add(newRow);
                    }
                    if (RiskPadre != null)
                    {
                        bool hasWBS = false;
                        foreach (DataRow itemWBS in DsCM.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskPadre.ID))
                        {
                            DataRow newRow = CM_WBS_Table.NewRow();
                            newRow[DT_CM_WBS.ID_CM] = CMRow[DT_CounterM.ID];
                            newRow[DT_CM_WBS.ID_WBS] = itemWBS[DT_RISK_WBS.ID_WBS];
                            newRow[DT_CM_WBS.NIVEL] = itemWBS[DT_RISK_WBS.NIVEL];
                            newRow[DT_CM_WBS.WBS] = itemWBS[DT_RISK_WBS.WBS];
                            newRow[DT_CM_WBS.IS_PRIMARY] = itemWBS[DT_RISK_WBS.IS_PRIMARY];
                            newRow[DT_CM_WBS.PROBABILITY] = 0;
                            if ((bool)itemWBS[DT_RISK_WBS.IS_PRIMARY])
                            {
                                ID_WBS = (int)itemWBS[DT_RISK_WBS.ID_WBS];
                                WBS_NAME = itemWBS[DT_RISK_WBS.NIVEL].ToString() + " " + itemWBS[DT_RISK_WBS.WBS].ToString();
                                USER_NAME = itemWBS[DT_RISK_WBS.USERNAME].ToString();
                            }
                            newRow[DT_CM_WBS.PRIMARY] = itemWBS[DT_RISK_WBS.PRIMARY];
                            newRow[DT_CM_WBS.USERNAME] = itemWBS[DT_RISK_WBS.USERNAME];
                            hasWBS = true;
                            CM_WBS_Table.Rows.Add(newRow);
                            
                            if (WBS_isSheet((int)itemWBS[DT_RISK_WBS.ID_WBS]))
                            {
                                foreach (DataRow itemDamage in CM_Damage_Table.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
                                {
                                    DataRow drWBS_CM_Damage = WBS_CM_Damage.NewRow();
                                    drWBS_CM_Damage[DT_WBS_CM_Damage.ID_CM] = CMRow[DT_CounterM.ID];
                                    drWBS_CM_Damage[DT_WBS_CM_Damage.ID_WBS] = itemWBS[DT_CM_WBS.ID_WBS];
                                    drWBS_CM_Damage[DT_WBS_CM_Damage.ID_DAMAGE] = itemDamage[DT_CounterM_Damage.ID_DAMAGE];
                                    drWBS_CM_Damage[DT_WBS_CM_Damage.DAMAGE] = itemDamage[DT_CounterM_Damage.DAMAGE];
                                    drWBS_CM_Damage[DT_WBS_CM_Damage.VALUE] = 0;
                                    drWBS_CM_Damage[DT_WBS_CM_Damage.WBS] = itemWBS[DT_RISK_WBS.WBS];
                                    if (!(WBS_CM_Damage.Rows.Contains(new object[] { itemWBS[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], CMRow[DT_CounterM.ID] })))
                                    {
                                        WBS_CM_Damage.Rows.Add(drWBS_CM_Damage);
                                    }
                                }
                            }
                        }
                        foreach (DataRow item in dsWBS.Tables[DT_WBS.TABLE_NAME].Select())
                        {
                            if (!(dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + item[DT_WBS.ID_WBS]).Any()))
                            {
                                if (!(CM_WBS_Table.Rows.Contains(new object[] { CMRow[DT_CounterM.ID], item[DT_WBS.ID_WBS] })))
                                {
                                    DataRow drRCMWBS = CM_WBS_Table.NewRow();
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
                                    CM_WBS_Table.Rows.Add(drRCMWBS);
                                }
                            }
                        }
                    }
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
                    TextProbability.Text = CMRow[DT_CounterM.PROBABILITY].ToString();
                    TextFather.Text = DsCM.Tables[DT_Risk.TABLE_NAME].Rows.Find(RiskPadre.ID)[DT_Risk.NAMESHORT].ToString();
                    if (CMRow[DT_CounterM.ID_WBS] != DBNull.Value)
                    {
                        ID_WBS = (int)CMRow[DT_Risk.ID_WBS];
                    }
                    foreach (DataRow item in DsCM.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + RiskTreeID))
                    {
                        if (!(CM_Damage_Table.Rows.Contains(new object[] { CMRow[DT_CounterM.ID], item[DT_Diagram_Damages.ID_DAMAGE] })))
                        {
                            DataRow rowTop = CM_Damage_Table.NewRow();
                            rowTop[DT_CounterM_Damage.COLOR] = item[DT_Diagram_Damages.COLOR];
                            rowTop[DT_CounterM_Damage.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                            rowTop[DT_CounterM_Damage.ID_COUNTERM] = CMRow[DT_CounterM.ID];
                            rowTop[DT_CounterM_Damage.VALUE] = 0;
                            rowTop[DT_CounterM_Damage.TOP_RISK] = item[DT_Diagram_Damages.TOP_RISK];
                            rowTop[DT_CounterM_Damage.DAMAGE] = item[DT_Diagram_Damages.TOP_RISK] + "(" + item[DT_Diagram_Damages.UM] + ")";
                            rowTop[DT_CounterM_Damage.ID_RISK_TREE] = RiskTreeID;
                            rowTop[DT_CounterM_Damage.RISK_REDUCTION] = 0;
                            rowTop[DT_CounterM_Damage.GROUPENAME] = "None";
                            rowTop[DT_CounterM_Damage.RISK] = CMRow[DT_CounterM.RISK_NAMESHORT];
                            rowTop[DT_CounterM_Damage.STATUS] = CMRow[DT_CounterM.ENABLED];
                            rowTop[DT_CounterM_Damage.IDRISK] = CMRow[DT_CounterM.ID_RISK];
                            CM_Damage_Table.Rows.Add(rowTop);
                        }
                    }
                }

                TextName.Focus();
                DvRoleCM = CM_RoleTable.DefaultView;
                dgRoles.ItemsSource = DvRoleCM;
                DvRoleCM.RowFilter = DT_Role_CM.ID_CM + " = " + CMRow[DT_CounterM.ID];

               

                if (HasAccess)
                {
                    DVCMWBS = CM_WBS_Table.DefaultView;
                    dgWBS.ItemsSource = DVCMWBS;
                    DVCMWBS.RowFilter = DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID];
                }
                else
                {
                    DVCMWBS = CM_WBS_Table.DefaultView;
                    dgWBS.ItemsSource = DVCMWBS;
                    DVCMWBS.RowFilter = DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID] + " AND " + DT_CM_WBS.USERNAME + " = '" + LOGIN_USER + "'";
                }


                DvTopRisk = WBS_CM_Damage.DefaultView;
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

        private void CalculateProbability()
        {
            try
            {
                List<decimal> Probabilities = new List<decimal>();
                foreach (DataRow item in CM_WBS_Table.Select(DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID]))
                {
                    if (WBS_isSheet((int)item[DT_CM_WBS.ID_WBS]))
                    {
                        if (item[DT_CM_WBS.PROBABILITY] == DBNull.Value)
                        {
                            Probabilities.Add(0);
                        }
                        else
                        {
                            Probabilities.Add((decimal)item[DT_CM_WBS.PROBABILITY]);
                        }
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
                if (CM_RoleTable.Select(DT_Role_CM.ID_CM + " = " + (Int32)CMRow[DT_CounterM.ID]).Count() > 0)
                {
                    frmSelection.Dt = General.DeleteExists(roleCodif, CM_RoleTable.Select(DT_Role_CM.ID_CM + " = " + (Int32)CMRow[DT_CounterM.ID]).CopyToDataTable(), DT_Role.IDROL_COLUMN);
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
                        DataRow drRole = CM_RoleTable.NewRow();
                        drRole[DT_Role_CM.ID_CM] = CMRow[DT_CounterM.ID];
                        drRole[DT_Role_CM.NAME_SHORT] = TextName.Text;
                        drRole[DT_Role_CM.Role] = item[DT_Role.ROLE_COLUM];
                        drRole[DT_Role_CM.IDROL_COLUMN] = item[DT_Role.IDROL_COLUMN];
                        CM_RoleTable.Rows.Add(drRole);
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
            CMRow[DT_CounterM.ID_RISK_TREE] = RiskTreeID;
            CMRow[DT_CounterM.ID_RISK] = RiskPadre.ID;
            CMRow[DT_CounterM.POSITION] = Posicion;
            CMRow[DT_CounterM.ENABLED] = true;
            CMRow[DT_CounterM.ID_WBS] = ID_WBS;
            CMRow[DT_CounterM.WBS_NAME] = WBS_NAME;
            CMRow[DT_CounterM.USER_NAME] = USER_NAME;

            foreach (DataRow itemDamage in CM_Damage_Table.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
            {//primero recorro los Daños de los riesgos
                decimal valor = 0;
                foreach (DataRow itemWBS in WBS_CM_Damage.Select(DT_WBS_CM_Damage.ID_CM + " = " + CMRow[DT_CounterM.ID] + " AND " + DT_WBS_CM_Damage.ID_DAMAGE + " = " + itemDamage[DT_CounterM_Damage.ID_DAMAGE]))
                {//y despues para  sumarlos todos en un mismo daño y encontrar el AD
                    valor += (decimal)itemWBS[DT_WBS_CM_Damage.VALUE];
                }
                itemDamage[DT_Risk_Damages.VALUE] = valor;
            }
            if (Versioned.IsNumeric(TextProbability.Text))
            {
                if (Probability > 100)
                {
                    MostrarErrorDialog(StringResources.PROBABILITY_FIELD);
                }
                else
                {
                    bool flag = true;
                    foreach (DataRow item in CM_Damage_Table.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
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

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TextName.Text != "")
                {
                    if (MyCM.Select(DT_CounterM.ID_RISK_TREE + " = " + RiskTreeID + " and "
                        + DT_CounterM.NAMESHORT + " = '" + TextName.Text + "' and " + DT_CounterM.ID + " <> " + CMRow[DT_CounterM.ID]).Any())
                    {
                        IS_USING_NAME = true;
                        MostrarDialogYesNo("The name [" + TextName.Text + "] Already exists in this diagram. Do you want to use it again?");
                    }
                    else
                    {
                        AcceptCM();
                    }
                    foreach (DataRow item in CM_Damage_Table.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
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

        private bool TengoPadre(int idWBS)
        {
            return dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + idWBS).Any();
        }
        private DataRow BuscarMiPadre(int idWBS)
        {
            int idPadre = (Int32)dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + idWBS).First()[DT_WBS_STRUCTURE.ID_FATHER];
            return dsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(idPadre);
        }

        public DataTable BuscarAncestros(int idWBS, DataTable dtWBSAncestors)
        {
            if (TengoPadre(idWBS))
            {
                dtWBSAncestors.ImportRow(BuscarMiPadre(idWBS));
                return BuscarAncestros((int)BuscarMiPadre(idWBS)[DT_WBS.ID_WBS], dtWBSAncestors);
            }
            else
            {
                return dtWBSAncestors;
            }
        }


        private void BtnAddWBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                DataTable roleCodif = ws.GetAllWBS().Tables[DT_WBS.TABLE_NAME].Copy();
                ws.Dispose();
                WindowSelection frmSelection = new WindowSelection();
                if (CM_WBS_Table.Select(DT_CM_WBS.ID_CM + " = " + (int)CMRow[DT_CounterM.ID]).Count() > 0)
                {
                    frmSelection.Dt = General.DeleteExists(roleCodif, CM_WBS_Table.Select(DT_CM_WBS.ID_CM + " = " + (int)CMRow[DT_CounterM.ID]).CopyToDataTable(), DT_WBS.ID_WBS);
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
                        DataRow drCMWBS = CM_WBS_Table.NewRow();
                        drCMWBS[DT_CM_WBS.ID_CM] = CMRow[DT_CounterM.ID];
                        drCMWBS[DT_CM_WBS.CM] = TextName.Text;
                        drCMWBS[DT_CM_WBS.WBS] = item[DT_WBS.WBS_NAME].ToString().TrimStart();
                        drCMWBS[DT_CM_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        drCMWBS[DT_CM_WBS.NIVEL] = item[DT_WBS.NIVEL].ToString().TrimStart();
                        drCMWBS[DT_CM_WBS.IS_PRIMARY] = false;
                        drCMWBS[DT_CM_WBS.PRIMARY] = "";
                        drCMWBS[DT_CM_WBS.USERNAME] = item[DT_WBS.USERNAME];
                        drCMWBS[DT_CM_WBS.PROBABILITY] = 0;
                        CM_WBS_Table.Rows.Add(drCMWBS);
                    }
                    foreach (DataRow itemRISKWBSi in CM_WBS_Table.Select(DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID]))
                    {
                        foreach (DataRow itemAncestors in BuscarAncestros((int)itemRISKWBSi[DT_RISK_WBS.ID_WBS], dsWBS.Tables[DT_WBS.TABLE_NAME].Clone()).Rows)
                        {
                            if (!(CM_WBS_Table.Rows.Contains(new object[] { CMRow[DT_CounterM.ID], itemAncestors[DT_WBS.ID_WBS] })))
                            {
                                DataRow drRiskWBSi = CM_WBS_Table.NewRow();
                                drRiskWBSi[DT_CM_WBS.ID_CM] = CMRow[DT_CounterM.ID];
                                drRiskWBSi[DT_CM_WBS.CM] = TextName.Text;
                                drRiskWBSi[DT_CM_WBS.WBS] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart();
                                drRiskWBSi[DT_CM_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                drRiskWBSi[DT_CM_WBS.NIVEL] = itemAncestors[DT_WBS.NIVEL].ToString().TrimStart();
                                drRiskWBSi[DT_CM_WBS.USERNAME] = itemAncestors[DT_WBS.USERNAME];
                                drRiskWBSi[DT_CM_WBS.IS_PRIMARY] = false;
                                drRiskWBSi[DT_CM_WBS.PRIMARY] = "";
                                drRiskWBSi[DT_CM_WBS.PROBABILITY] = 0;
                                CM_WBS_Table.Rows.Add(drRiskWBSi);
                            }
                        }
                    }
                    foreach (DataRow itemWBS in CM_WBS_Table.Select(DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID]))
                    {
                        if (WBS_isSheet((int)itemWBS[DT_CM_WBS.ID_WBS]))
                        {
                            foreach (DataRow itemDamage in CM_Damage_Table.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
                            {
                                DataRow drWBS_RISK_Damage = WBS_CM_Damage.NewRow();
                                drWBS_RISK_Damage[DT_WBS_CM_Damage.ID_CM] = CMRow[DT_CounterM.ID];
                                drWBS_RISK_Damage[DT_WBS_CM_Damage.ID_WBS] = itemWBS[DT_RISK_WBS.ID_WBS];
                                drWBS_RISK_Damage[DT_WBS_CM_Damage.ID_DAMAGE] = itemDamage[DT_Risk_Damages.ID_DAMAGE];
                                drWBS_RISK_Damage[DT_WBS_CM_Damage.DAMAGE] = itemDamage[DT_Risk_Damages.DAMAGE];
                                drWBS_RISK_Damage[DT_WBS_CM_Damage.VALUE] = 0;
                                drWBS_RISK_Damage[DT_WBS_CM_Damage.WBS] = itemWBS[DT_RISK_WBS.WBS];
                                if (!(WBS_CM_Damage.Rows.Contains(new object[] { itemWBS[DT_CM_WBS.ID_WBS], itemDamage[DT_CounterM_Damage.ID_DAMAGE], CMRow[DT_CounterM.ID] })))
                                {
                                    WBS_CM_Damage.Rows.Add(drWBS_RISK_Damage);
                                }
                            }
                        }
                    }
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
                foreach (DataRow itemR in (WBS_CM_Damage.Select(DT_WBS_CM_Damage.ID_WBS + " = " + fila[DT_WBS.ID_WBS] + " AND " + DT_WBS_CM_Damage.ID_CM + " = " + CMRow[DT_CounterM.ID])))
                {
                    itemR.Delete();
                }
                fila.Delete();
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

        private void BtnSetPrimary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgWBS.SelectedIndex >= 0)
                {
                    foreach (DataRow item in CM_WBS_Table.Select(DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID]))
                    {
                        item[DT_CM_WBS.IS_PRIMARY] = false;
                        item[DT_CM_WBS.PRIMARY] = "";
                    }
                    DVCMWBS[dgWBS.SelectedIndex].Row[DT_CM_WBS.IS_PRIMARY] = true;
                    DVCMWBS[dgWBS.SelectedIndex].Row[DT_CM_WBS.PRIMARY] = "PRIMARY";
                    ID_WBS = (Int32)DVCMWBS[dgWBS.SelectedIndex].Row[DT_CM_WBS.ID_WBS];
                    WBS_NAME = DVCMWBS[dgWBS.SelectedIndex].Row[DT_CM_WBS.NIVEL].ToString() + " " + DVCMWBS[dgWBS.SelectedIndex].Row[DT_CM_WBS.WBS].ToString();
                    USER_NAME = DVCMWBS[dgWBS.SelectedIndex].Row[DT_CM_WBS.USERNAME].ToString();
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private bool WBS_isSheet(int ID_WBS)
        {

            if (dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + ID_WBS).Any())
            {
                return false;
            }
            else
            { return true; }
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
    }
}
