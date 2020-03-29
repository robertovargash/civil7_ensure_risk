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
    public class DataCurrentRisk : INotifyPropertyChanged
    {
        private bool hasAccess = false;
        private decimal probability;

        public decimal Probability { get {return probability; } set { probability = value;OnPropertyChanged("Probability"); } }

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
    }
    /// <summary>
    /// Interaction logic for WindowRisk.xaml
    /// </summary>
    public partial class WindowRisk : Window
    {
        public DataCurrentRisk Pi { get; set; }
        public DataRow RiskRow { get; set; }
        public string Operation { get; set; }
        public int RiskTreeID { get; set; }
        public int ID_PROJECT { get; set; }
        public int ID_WBS { get; set; }
        public string WBS_NAME { get; set; }
        public string USER_NAME { get; set; }
        public string LOGIN_USER { get; set; }
        public DataRow RowFather { get; set; }
        public List<RiskPolyLine> ChildrenLines { get; set; }
        public DataSet Ds { get; set; }
        public DataTable MyRisks { get; set; }
        public DataView DvTopRisk { get; set; }
        public DataView DvRiskWBS { get; set; }
        public DataView DvRoleRisk { get; set; }
        public DataTable Risk_DamageTable { get; set; }
        public DataTable CM_DamageTable { get; set; }
        public DataTable WBS_RISK_Damage { get; set; }
        public DataTable WBS_CM_Damage { get; set; }
        public DataTable Risk_WBS_Table { get; set; }
        public DataTable CM_WBS_Table { get; set; }
        public DataTable Risk_RoleTable { get; set; }
        public DataTable CM_RoleTable { get; set; }
        public RiskPolyLine RiskSelected { get; set; }
        public bool Enabled { get; set; }
        public int Posicion { get; set; }
        private DataSet dsWBS;

        private bool Editando;
        private bool Seleccionando;

        public WindowRisk()
        {
            InitializeComponent();
            Pi = new DataCurrentRisk();
            ChangeLanguage();
            TextName.DataContext = Pi;
            TextDetail.DataContext = Pi;
            TextProbability.DataContext = Pi;
            gridTabRoles.DataContext = Pi;
            gridTabWBS.DataContext = Pi;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceWBS.WebServiceWBS wsWBS = new ServiceWBS.WebServiceWBS();
                dsWBS = wsWBS.GetAllWBSFiltered(new object[] { ID_PROJECT }).Copy();
                wsWBS.Dispose();
                if (Operation == General.INSERT)
                {
                    Enabled = true;
                    if (RowFather != null)
                    {
                        TextFather.Text = RowFather[DT_Risk.NAMESHORT].ToString();
                        RiskRow[DT_Risk.IDRISK_FATHER] = RowFather[DT_Risk.ID];
                    }
                    RiskRow[DT_Risk.ENABLED] = true;
                    SetTableRisk_Damages(false, true);

                    SetTableRisk_Role();

                    SetTableRisk_WBS();
                }
                if (Operation == General.UPDATE)
                {
                    if (RiskRow[DT_Risk.USER_NAME] != DBNull.Value)
                    {
                        USER_NAME = RiskRow[DT_Risk.USER_NAME].ToString();
                    }

                    if (RiskRow[DT_Risk.WBS_NAME] != DBNull.Value)
                    {
                        WBS_NAME = RiskRow[DT_Risk.WBS_NAME].ToString();
                    }
                    Enabled = (bool)RiskRow[DT_Risk.ENABLED];
                    TextName.Text = RiskRow[DT_Risk.NAMESHORT].ToString();
                    TextDetail.Text = RiskRow[DT_Risk.COMMENTS].ToString();
                    TextProbability.Text = RiskRow[DT_Risk.PROBABILITY].ToString();
                    if (RiskRow[DT_Risk.ID_WBS] != DBNull.Value)
                    {
                        ID_WBS = (int)RiskRow[DT_Risk.ID_WBS];
                    }

                    SetTableRisk_Damages(true, Enabled);
                    TextFather.Text = RowFather[DT_Risk.NAMESHORT].ToString();
                }
                TextName.Focus();
                DvRoleRisk = Risk_RoleTable.DefaultView;
                dgRoles.ItemsSource = DvRoleRisk;
                DvRoleRisk.RowFilter = DT_Role_Risk.ID_RISK + " = " + RiskRow[DT_Risk.ID];

                DvRiskWBS = Risk_WBS_Table.DefaultView;
                dgWBS.ItemsSource = DvRiskWBS;
                if (Pi.HasAccess)
                {                   
                    DvRiskWBS.RowFilter = DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID];
                }
                else
                {
                    DvRiskWBS.RowFilter = DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID] + " AND " + DT_RISK_WBS.USERNAME + " = '" + LOGIN_USER + "'";
                }
                

                //DvTopRisk = TopRiskTable.DefaultView;
                //dgTopRisk.ItemsSource = DvTopRisk;
                //DvTopRisk.RowFilter = DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID];

                DvTopRisk = WBS_RISK_Damage.DefaultView;
                dgTopRisk.ItemsSource = DvTopRisk;
                //DvTopRisk.RowFilter = DT_WBS_RISK_DAMAGE.ID_RISK + " = " + RiskRow[DT_Risk.ID] + " AND " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + ID_USER_WBS;
                DvTopRisk.RowFilter = DT_WBS_RISK_DAMAGE.ID_RISK + " = " + RiskRow[DT_Risk.ID];
                if (dsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.USERNAME + " = '" + LOGIN_USER + "'").Any())
                {
                    string filter = "";
                    DataRow[] item = dsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.USERNAME + " = '" + LOGIN_USER + "'");
                    for (int i = 0; i < item.Count(); i++)
                    {
                        if (i == 0)
                        {
                            filter += " AND (" + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + item[i][DT_WBS.ID_WBS];
                        }
                        else
                        {
                            filter += " OR " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + item[i][DT_WBS.ID_WBS];
                        }
                    }
                    filter += ")";
                    DvTopRisk.RowFilter += filter;
                }
                CalculateProbability();
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog(); ;
            }
        }

        private void SetTableRisk_Damages(bool useProbability, bool isEnabled)
        {
            foreach (DataRow item in Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + RiskTreeID))
            {
                if (!(Risk_DamageTable.Rows.Contains(new object[] { RiskRow[DT_Risk.ID], item[DT_Diagram_Damages.ID_DAMAGE] })))
                {
                    DataRow rowTop = Risk_DamageTable.NewRow();
                    rowTop[DT_Risk_Damages.COLOR] = item[DT_Diagram_Damages.COLOR];
                    rowTop[DT_Risk_Damages.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                    rowTop[DT_Risk_Damages.ID_RISK] = RiskRow[DT_Risk.ID];
                    rowTop[DT_Risk_Damages.ID_RISK_TREE] = RiskTreeID;
                    rowTop[DT_Risk_Damages.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString() + "(" + item[DT_Diagram_Damages.UM] + ")";
                    rowTop[DT_Risk_Damages.VALUE] = 0;
                    rowTop[DT_Risk_Damages.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                    //rowTop[DT_Risk_Damages.PROBABILITY] = 0;
                    rowTop[DT_Risk_Damages.PROBABILITY] = useProbability ? Pi.Probability : 0;
                    rowTop[DT_Risk_Damages.GROUPE_NAME] = "None";
                    rowTop[DT_Risk_Damages.RISK_NAMESHORT] = "";
                    rowTop[DT_Risk_Damages.FATHER] = RowFather[DT_Risk.NAMESHORT].ToString();
                    rowTop[DT_Risk_Damages.STATUS] = isEnabled ? "Activated" : "No Activated";
                    rowTop[DT_Risk_Damages.ID_FATHER] = RiskRow[DT_Risk.IDRISK_FATHER];
                    rowTop[DT_Risk_Damages.RISK_TREE] = Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(RiskTreeID)[DT_Diagram.DIAGRAM_NAME].ToString();
                    rowTop[DT_Risk_Damages.IS_ROOT] = false;
                    Risk_DamageTable.Rows.Add(rowTop);
                }                    
            }
        }

        private void SetTableRisk_Role()
        {
            if (RiskSelected != null)
            {
                foreach (DataRow item in Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + RiskSelected.ID))
                {
                    DataRow newRow = Risk_RoleTable.NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = RiskRow[DT_Risk.ID];
                    newRow[DT_Role_Risk.Role] = item[DT_Role_Risk.Role];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                    Risk_RoleTable.Rows.Add(newRow);
                }
            }
        }

        private void SetTableRisk_WBS()
        {
            if (RiskSelected != null)
            {
                bool hasWBS = false;
                foreach (DataRow itemWBS in Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskSelected.ID))
                {
                    DataRow newRow = Risk_WBS_Table.NewRow();
                    newRow[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                    newRow[DT_RISK_WBS.ID_WBS] = itemWBS[DT_RISK_WBS.ID_WBS];
                    newRow[DT_RISK_WBS.NIVEL] = itemWBS[DT_RISK_WBS.NIVEL];
                    newRow[DT_RISK_WBS.WBS] = itemWBS[DT_RISK_WBS.WBS];
                    newRow[DT_RISK_WBS.IS_PRIMARY] = itemWBS[DT_RISK_WBS.IS_PRIMARY];
                    newRow[DT_RISK_WBS.PRIMARY] = itemWBS[DT_RISK_WBS.PRIMARY];
                    newRow[DT_RISK_WBS.USERNAME] = itemWBS[DT_RISK_WBS.USERNAME];
                    newRow[DT_RISK_WBS.PROBABILITY] = 100;
                    if ((bool)itemWBS[DT_RISK_WBS.IS_PRIMARY])
                    {
                        ID_WBS = (int)itemWBS[DT_RISK_WBS.ID_WBS];
                        WBS_NAME = itemWBS[DT_RISK_WBS.NIVEL].ToString() + " " + itemWBS[DT_RISK_WBS.WBS].ToString();
                        USER_NAME = itemWBS[DT_RISK_WBS.USERNAME].ToString();
                    }
                    Risk_WBS_Table.Rows.Add(newRow);
                    hasWBS = true;
                    SetTableRisk_WBS_Damage(itemWBS);
                }
                CompleteRiskWBSTable(hasWBS);
            }
        }

        private void CompleteRiskWBSTable(bool hasWBS)
        {
            foreach (DataRow item in dsWBS.Tables[DT_WBS.TABLE_NAME].Rows)
            {
                if (!(dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + item[DT_WBS.ID_WBS]).Any()))
                {
                    if (!(Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Contains(new object[] { RiskRow[DT_Risk.ID], item[DT_WBS.ID_WBS] })))
                    {
                        DataRow drRiskWBS = Ds.Tables[DT_RISK_WBS.TABLENAME].NewRow();
                        drRiskWBS[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                        drRiskWBS[DT_RISK_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        drRiskWBS[DT_RISK_WBS.WBS] = item[DT_WBS.WBS_NAME];
                        drRiskWBS[DT_RISK_WBS.NIVEL] = item[DT_WBS.NIVEL];
                        drRiskWBS[DT_RISK_WBS.USERNAME] = item[DT_WBS.USERNAME];
                        drRiskWBS[DT_RISK_WBS.PROBABILITY] = 100;
                        if (!hasWBS)
                        {
                            drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = true;
                            drRiskWBS[DT_RISK_WBS.PRIMARY] = "Primary";
                            ID_WBS = (int)item[DT_WBS.ID_WBS];
                            USER_NAME = item[DT_WBS.USERNAME].ToString();
                            WBS_NAME = item[DT_WBS.NIVEL].ToString() + " " + item[DT_WBS.WBS_NAME].ToString();
                        }
                        else
                        {
                            drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = false;
                            drRiskWBS[DT_RISK_WBS.PRIMARY] = "";
                        }
                        Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Add(drRiskWBS);
                    }
                }
            }
        }

        private void SetTableRisk_WBS_Damage(DataRow itemWBS)
        {
            if (WBS_isSheet((int)itemWBS[DT_RISK_WBS.ID_WBS]))
            {
                foreach (DataRow itemDamage in Risk_DamageTable.Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                {
                    DataRow drWBS_RISK_Damage = WBS_RISK_Damage.NewRow();
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_RISK] = RiskRow[DT_Risk.ID];
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_WBS] = itemWBS[DT_RISK_WBS.ID_WBS];
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = itemDamage[DT_Risk_Damages.ID_DAMAGE];
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.DAMAGE] = itemDamage[DT_Risk_Damages.DAMAGE];
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.WBS] = itemWBS[DT_RISK_WBS.WBS];
                    if (!(WBS_RISK_Damage.Rows.Contains(new object[] { itemWBS[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], RiskRow[DT_Risk.ID] })))
                    {
                        WBS_RISK_Damage.Rows.Add(drWBS_RISK_Damage);
                    }
                }
            }

        }

        private void CalculateProbability()
        {
            try
            {
                List<decimal> Probabilities = new List<decimal>();
                foreach (DataRow item in Risk_WBS_Table.Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                {
                    if (WBS_isSheet((int)item[DT_RISK_WBS.ID_WBS]))
                    {
                        if (item[DT_RISK_WBS.PROBABILITY] == DBNull.Value)
                        {
                            Probabilities.Add(100);
                        }
                        else
                        {
                            Probabilities.Add((decimal)item[DT_RISK_WBS.PROBABILITY]);
                        }
                    }
                }
                if (Probabilities.Count > 0)
                {
                    Pi.Probability = Probabilities.Sum() / Probabilities.Count;
                }
                else
                {
                    Pi.Probability = 100;
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextName, StringResources.ShortNameText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextFather, StringResources.FatherText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextDetail, StringResources.RiskDetailText);
            LabelProbability.Content = StringResources.ProbabilityLabel;
            BtnCancel.Content = StringResources.CancelButton;
            tabRoles.Header = StringResources.TabValues;
            tabRoles.Header = StringResources.TabRoles;
            Title = StringResources.RiskTitle;
        }

        private void BtnAddRole_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole();
                DataTable roleCodif = ws.GetRolesData().Tables[DT_Role.ROLE_TABLE].Copy();
                ws.Dispose();
                WindowSelection frmSelection = new WindowSelection();
                if (Risk_RoleTable.Select(DT_Role_Risk.ID_RISK + " = " + (Int32)RiskRow[DT_Risk.ID]).Count() > 0)
                {
                    frmSelection.Dt = General.DeleteExists(roleCodif, Risk_RoleTable.Select(DT_Role_Risk.ID_RISK + " = " + (Int32)RiskRow[DT_Risk.ID]).CopyToDataTable(), DT_Role.IDROL_COLUMN);
                }
                else
                {
                    frmSelection.Dt = roleCodif;
                }

                frmSelection.DcolumToShow = new string[] { DT_Role.ROLE_COLUM };
                frmSelection.DcolumToShowAlias = new string[] { DT_Role.ROLE_COLUM };
                frmSelection.Title = DT_Role.ROLE_COLUM;
                frmSelection.P.FilterString = "Role Name";
                frmSelection.ColumnToFilter = DT_Role.ROLE_COLUM;
                if (frmSelection.ShowDialog() == true)
                {
                    foreach (DataRow itemRole in frmSelection.RowsSelected)
                    {
                        DataRow drRole = Risk_RoleTable.NewRow();
                        drRole[DT_Role_Risk.ID_RISK] = RiskRow[DT_Risk.ID];
                        drRole[DT_Role_Risk.NAME_SHORT] = TextName.Text;
                        drRole[DT_Role_Risk.Role] = itemRole[DT_Role.ROLE_COLUM];
                        drRole[DT_Role_Risk.IDROL_COLUMN] = itemRole[DT_Role.IDROL_COLUMN];
                        Risk_RoleTable.Rows.Add(drRole);
                        if (Operation == General.UPDATE)
                        {
                            foreach (var item in ChildrenLines)
                            {
                                if (item.IsCM)
                                {
                                    if (!(CM_RoleTable.Rows.Contains(new object[] { item.ID, itemRole[DT_Role.IDROL_COLUMN] })))
                                    {
                                        DataRow drRoleCM = CM_RoleTable.NewRow();
                                        drRoleCM[DT_Role_CM.ID_CM] = item.ID;
                                        drRoleCM[DT_Role_CM.NAME_SHORT] = item.ShortName;
                                        drRoleCM[DT_Role_CM.Role] = itemRole[DT_Role.ROLE_COLUM];
                                        drRoleCM[DT_Role_CM.IDROL_COLUMN] = itemRole[DT_Role.IDROL_COLUMN];
                                        CM_RoleTable.Rows.Add(drRoleCM);
                                    }
                                }
                                else
                                {
                                    if (!(Risk_RoleTable.Rows.Contains(new object[] { item.ID, itemRole[DT_Role.IDROL_COLUMN] })))
                                    {
                                        DataRow drRoleRisk = Risk_RoleTable.NewRow();
                                        drRoleRisk[DT_Role_Risk.ID_RISK] = item.ID;
                                        drRoleRisk[DT_Role_Risk.NAME_SHORT] = item.ShortName;
                                        drRoleRisk[DT_Role_Risk.Role] = itemRole[DT_Role.ROLE_COLUM];
                                        drRoleRisk[DT_Role_Risk.IDROL_COLUMN] = itemRole[DT_Role.IDROL_COLUMN];
                                        Risk_RoleTable.Rows.Add(drRoleRisk);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnDelRole_Click(object sender, RoutedEventArgs e)
        {
            DataRow fila = DvRoleRisk[dgRoles.SelectedIndex].Row;
            if (fila[DT_Role_Risk.Role].ToString() != "Administrator")
            {
                if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + fila[DT_Role_Risk.Role].ToString() + "]?").ShowDialog() == true)
                {
                    if (Operation == General.UPDATE)
                    {
                        foreach (var item in ChildrenLines)
                        {
                            if (item.IsCM)
                            {
                                if (CM_RoleTable.Rows.Contains(new object[] { item.ID, fila[DT_Role.IDROL_COLUMN] }))
                                {
                                    CM_RoleTable.Rows.Find(new object[] { item.ID, fila[DT_Role.IDROL_COLUMN] }).Delete();
                                }
                            }
                            else
                            {
                                if (Risk_RoleTable.Rows.Contains(new object[] { item.ID, fila[DT_Role.IDROL_COLUMN] }))
                                {
                                    Risk_RoleTable.Rows.Find(new object[] { item.ID, fila[DT_Role.IDROL_COLUMN] }).Delete();
                                }
                            }
                        }
                    }
                    fila.Delete();
                }
            }
            else
            {
                new WindowMessageOK("'Administrator' role can't be deleted!").ShowDialog();
            }
        }

        private void BtnAddWBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                DataTable roleCodif = ws.GetAllWBSFiltered(new object[] { ID_PROJECT }).Tables[DT_WBS.TABLE_NAME].Copy();
                ws.Dispose();
                WindowSelection frmSelection = new WindowSelection();
                if (Risk_WBS_Table.Select(DT_RISK_WBS.ID_RISK + " = " + (Int32)RiskRow[DT_Risk.ID]).Count() > 0)
                {
                    frmSelection.Dt = General.DeleteExists(roleCodif, Risk_WBS_Table.Select(DT_RISK_WBS.ID_RISK + " = " + (Int32)RiskRow[DT_Risk.ID]).CopyToDataTable(), DT_WBS.ID_WBS);
                }
                else
                {
                    frmSelection.Dt = roleCodif;
                }
                foreach (DataRow item in frmSelection.Dt.Rows)
                {
                    item[DT_WBS.WBS_NAME] = item[DT_WBS.WBS_NAME].ToString().Insert(0, TreeOperation.Spaces(item[DT_WBS.NIVEL].ToString().Split('.').Count() - 1));
                    item[DT_WBS.NIVEL] = item[DT_WBS.NIVEL].ToString().Insert(0, TreeOperation.Spaces(item[DT_WBS.NIVEL].ToString().Split('.').Count() - 1));
                }
                frmSelection.DcolumToShow = new string[] { DT_WBS.NIVEL, DT_WBS.WBS_NAME, DT_WBS.USERNAME };
                frmSelection.DcolumToShowAlias = new string[] { "Level", DT_WBS.WBS_NAME, "User Owner" };
                frmSelection.Title = "WBS";
                frmSelection.P.FilterString = "WBS Name";
                frmSelection.ColumnToFilter = DT_WBS.WBS_NAME;
                if (frmSelection.ShowDialog() == true)
                {
                    foreach (DataRow itemWBS in frmSelection.RowsSelected)//Por cada WBS seleccionado lo agrega al riesgo
                    {
                        DataRow drRiskWBS = Risk_WBS_Table.NewRow();
                        drRiskWBS[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                        drRiskWBS[DT_RISK_WBS.RISK] = TextName.Text;
                        drRiskWBS[DT_RISK_WBS.WBS] = itemWBS[DT_WBS.WBS_NAME].ToString().TrimStart();
                        drRiskWBS[DT_RISK_WBS.ID_WBS] = itemWBS[DT_WBS.ID_WBS];
                        drRiskWBS[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                        drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = false;
                        drRiskWBS[DT_RISK_WBS.PRIMARY] = "";
                        drRiskWBS[DT_RISK_WBS.USERNAME] = itemWBS[DT_WBS.USERNAME];
                        drRiskWBS[DT_RISK_WBS.PROBABILITY] = 100;
                        Risk_WBS_Table.Rows.Add(drRiskWBS);
                        foreach (DataRow itemAncestors in BuscarAncestros((Int32)itemWBS[DT_WBS.ID_WBS], dsWBS.Tables[DT_WBS.TABLE_NAME].Clone()).Rows)
                        {
                            if (!(Risk_WBS_Table.Rows.Contains(new object[] { RiskRow[DT_Risk.ID], itemAncestors[DT_WBS.ID_WBS] })))
                            {
                                DataRow drRiskWBSi = Risk_WBS_Table.NewRow();
                                drRiskWBSi[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                                drRiskWBSi[DT_RISK_WBS.RISK] = TextName.Text;
                                drRiskWBSi[DT_RISK_WBS.WBS] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart();
                                drRiskWBSi[DT_RISK_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                drRiskWBSi[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                drRiskWBSi[DT_RISK_WBS.USERNAME] = itemAncestors[DT_WBS.USERNAME];
                                drRiskWBSi[DT_RISK_WBS.IS_PRIMARY] = false;
                                drRiskWBSi[DT_RISK_WBS.PRIMARY] = "";
                                drRiskWBSi[DT_RISK_WBS.PROBABILITY] = 100;
                                Risk_WBS_Table.Rows.Add(drRiskWBSi);
                            }
                        }
                        if (Operation == General.UPDATE)
                        {
                            foreach (var itemLines in ChildrenLines)
                            {
                                if (itemLines.IsCM)
                                {
                                    if (!(CM_WBS_Table.Rows.Contains(new object[] { itemLines.ID, itemWBS[DT_WBS.ID_WBS] })))
                                    {
                                        DataRow drCMWBS = CM_WBS_Table.NewRow();
                                        drCMWBS[DT_CM_WBS.ID_CM] = itemLines.ID;
                                        drCMWBS[DT_CM_WBS.CM] = itemLines.ShortName;
                                        drCMWBS[DT_CM_WBS.WBS] = itemWBS[DT_WBS.WBS_NAME].ToString().TrimStart();
                                        drCMWBS[DT_CM_WBS.ID_WBS] = itemWBS[DT_WBS.ID_WBS];
                                        drCMWBS[DT_CM_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                        drCMWBS[DT_CM_WBS.USERNAME] = itemWBS[DT_WBS.USERNAME];
                                        drCMWBS[DT_CM_WBS.IS_PRIMARY] = false;
                                        drCMWBS[DT_CM_WBS.PRIMARY] = "";
                                        CM_WBS_Table.Rows.Add(drCMWBS);
                                    }
                                    foreach (DataRow itemWBSCM in CM_WBS_Table.Select(DT_CM_WBS.ID_CM + " = " + itemLines.ID))
                                    {
                                        if (WBS_isSheet((Int32)itemWBSCM[DT_CM_WBS.ID_WBS]))
                                        {
                                            foreach (DataRow itemDamage in CM_DamageTable.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + itemLines.ID))
                                            {
                                                if (!(WBS_CM_Damage.Rows.Contains(new object[] { itemWBSCM[DT_CM_WBS.ID_WBS], itemDamage[DT_CounterM_Damage.ID_DAMAGE], itemLines.ID })))
                                                {
                                                    DataRow drWBS_CM_Damage = WBS_CM_Damage.NewRow();
                                                    drWBS_CM_Damage[DT_WBS_CM_Damage.ID_CM] = itemLines.ID;
                                                    drWBS_CM_Damage[DT_WBS_CM_Damage.ID_WBS] = itemWBSCM[DT_CM_WBS.ID_WBS];
                                                    drWBS_CM_Damage[DT_WBS_CM_Damage.ID_DAMAGE] = itemDamage[DT_CounterM_Damage.ID_DAMAGE];
                                                    drWBS_CM_Damage[DT_WBS_CM_Damage.DAMAGE] = itemDamage[DT_CounterM_Damage.DAMAGE];
                                                    drWBS_CM_Damage[DT_WBS_CM_Damage.VALUE] = 0;
                                                    drWBS_CM_Damage[DT_WBS_CM_Damage.WBS] = itemWBSCM[DT_CM_WBS.WBS];
                                                    WBS_CM_Damage.Rows.Add(drWBS_CM_Damage);
                                                }

                                            }
                                        }
                                    }
                                    //Buscando los ancestros para c/u de los wbs del cm
                                    foreach (DataRow itemCMWBSi in CM_WBS_Table.Select(DT_CM_WBS.ID_CM + " = " + itemLines.ID))
                                    {
                                        foreach (DataRow itemAncestors in BuscarAncestros((Int32)itemCMWBSi[DT_CM_WBS.ID_WBS], dsWBS.Tables[DT_WBS.TABLE_NAME].Clone()).Rows)
                                        {
                                            if (!(CM_WBS_Table.Rows.Contains(new object[] { itemLines.ID, itemAncestors[DT_WBS.ID_WBS] })))
                                            {
                                                DataRow drCMWBS = CM_WBS_Table.NewRow();
                                                drCMWBS[DT_CM_WBS.ID_CM] = itemLines.ID;
                                                drCMWBS[DT_CM_WBS.CM] = itemLines.ShortName;
                                                drCMWBS[DT_CM_WBS.WBS] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart();
                                                drCMWBS[DT_CM_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                                drCMWBS[DT_CM_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                                drCMWBS[DT_CM_WBS.USERNAME] = itemAncestors[DT_WBS.USERNAME];
                                                drCMWBS[DT_CM_WBS.IS_PRIMARY] = false;
                                                drCMWBS[DT_CM_WBS.PRIMARY] = "";
                                                CM_WBS_Table.Rows.Add(drCMWBS);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (!(Risk_WBS_Table.Rows.Contains(new object[] { itemLines.ID, itemWBS[DT_WBS.ID_WBS] })))
                                    {
                                        DataRow drRiskWBSS = Risk_WBS_Table.NewRow();
                                        drRiskWBSS[DT_RISK_WBS.ID_RISK] = itemLines.ID;
                                        drRiskWBSS[DT_RISK_WBS.RISK] = itemLines.ShortName;
                                        drRiskWBSS[DT_RISK_WBS.WBS] = itemWBS[DT_WBS.WBS_NAME].ToString().TrimStart();
                                        drRiskWBSS[DT_RISK_WBS.ID_WBS] = itemWBS[DT_WBS.ID_WBS];
                                        drRiskWBSS[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                        drRiskWBSS[DT_RISK_WBS.USERNAME] = itemWBS[DT_WBS.USERNAME];
                                        drRiskWBSS[DT_RISK_WBS.IS_PRIMARY] = false;
                                        drRiskWBSS[DT_RISK_WBS.PRIMARY] = "";
                                        drRiskWBSS[DT_RISK_WBS.PROBABILITY] = 100;
                                        Risk_WBS_Table.Rows.Add(drRiskWBSS);
                                    }
                                    foreach (DataRow itemWBSRisk in Risk_WBS_Table.Select(DT_RISK_WBS.ID_RISK + " = " + itemLines.ID))
                                    {
                                        if (WBS_isSheet((Int32)itemWBSRisk[DT_RISK_WBS.ID_WBS]))
                                        {
                                            foreach (DataRow itemDamage in Risk_DamageTable.Select(DT_Risk_Damages.ID_RISK + " = " + itemLines.ID))
                                            {
                                                if (!(WBS_RISK_Damage.Rows.Contains(new object[] { itemWBSRisk[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], itemLines.ID })))
                                                {
                                                    DataRow drWBS_RISK_Damage = WBS_RISK_Damage.NewRow();
                                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_RISK] = itemLines.ID;
                                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_WBS] = itemWBSRisk[DT_RISK_WBS.ID_WBS];
                                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = itemDamage[DT_Risk_Damages.ID_DAMAGE];
                                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.DAMAGE] = itemDamage[DT_Risk_Damages.DAMAGE];
                                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.WBS] = itemWBSRisk[DT_RISK_WBS.WBS];
                                                    WBS_RISK_Damage.Rows.Add(drWBS_RISK_Damage);
                                                }

                                            }
                                        }
                                    }
                                    foreach (DataRow itemRISKWBSi in Risk_WBS_Table.Select(DT_RISK_WBS.ID_RISK + " = " + itemLines.ID))
                                    {
                                        foreach (DataRow itemAncestors in BuscarAncestros((Int32)itemRISKWBSi[DT_RISK_WBS.ID_WBS], dsWBS.Tables[DT_WBS.TABLE_NAME].Clone()).Rows)
                                        {
                                            if (!(Risk_WBS_Table.Rows.Contains(new object[] { itemLines.ID, itemAncestors[DT_WBS.ID_WBS] })))
                                            {
                                                DataRow drRiskWBSi = Risk_WBS_Table.NewRow();
                                                drRiskWBSi[DT_RISK_WBS.ID_RISK] = itemLines.ID;
                                                drRiskWBSi[DT_RISK_WBS.RISK] = itemLines.ShortName;
                                                drRiskWBSi[DT_RISK_WBS.WBS] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart();
                                                drRiskWBSi[DT_RISK_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                                drRiskWBSi[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                                drRiskWBSi[DT_RISK_WBS.USERNAME] = itemAncestors[DT_WBS.USERNAME];
                                                drRiskWBSi[DT_RISK_WBS.IS_PRIMARY] = false;
                                                drRiskWBSi[DT_RISK_WBS.PRIMARY] = "";
                                                drRiskWBSi[DT_RISK_WBS.PROBABILITY] = 100;
                                                Risk_WBS_Table.Rows.Add(drRiskWBSi);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (DataRow itemWBS in Risk_WBS_Table.Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                    {
                        if (WBS_isSheet((Int32)itemWBS[DT_RISK_WBS.ID_WBS]))
                        {
                            foreach (DataRow itemDamage in Risk_DamageTable.Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                            {
                                if (!(WBS_RISK_Damage.Rows.Contains(new object[] { itemWBS[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], RiskRow[DT_Risk.ID] })))
                                {
                                    DataRow drWBS_RISK_Damage = WBS_RISK_Damage.NewRow();
                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_RISK] = RiskRow[DT_Risk.ID];
                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_WBS] = itemWBS[DT_RISK_WBS.ID_WBS];
                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = itemDamage[DT_Risk_Damages.ID_DAMAGE];
                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.DAMAGE] = itemDamage[DT_Risk_Damages.DAMAGE];
                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.WBS] = itemWBS[DT_RISK_WBS.WBS];
                                    WBS_RISK_Damage.Rows.Add(drWBS_RISK_Damage);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
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
                return BuscarAncestros((Int32)BuscarMiPadre(idWBS)[DT_WBS.ID_WBS],dtWBSAncestors);
            }
            else
            {
                return dtWBSAncestors;
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

        private void BtnDelWBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRow fila = DvRiskWBS[dgWBS.SelectedIndex].Row;
                if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + fila[DT_RISK_WBS.WBS].ToString() + "]?").ShowDialog() == true)
                {
                    foreach (var itemLines in ChildrenLines)
                    {
                        if (itemLines.IsCM)
                        {//AQUI PROBLEMAS CON EL ROWSTATE
                            foreach (DataRow itemC in WBS_CM_Damage.Select(DT_WBS_CM_Damage.ID_CM + " = " + itemLines.ID + " AND " + DT_WBS_CM_Damage.ID_WBS + " = " + fila[DT_WBS.ID_WBS]))
                            {
                                itemC.Delete();
                            }
                            if (CM_WBS_Table.Rows.Contains(new object[] { itemLines.ID, fila[DT_WBS.ID_WBS] }))
                            {
                                CM_WBS_Table.Rows.Find(new object[] { itemLines.ID, fila[DT_WBS.ID_WBS] }).Delete();
                            }
                        }
                        else
                        {
                            foreach (DataRow itemC in WBS_RISK_Damage.Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + itemLines.ID + " AND " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + fila[DT_WBS.ID_WBS]))
                            {
                                itemC.Delete();
                            }
                            if (Risk_WBS_Table.Rows.Contains(new object[] { itemLines.ID, fila[DT_WBS.ID_WBS] }))
                            {
                                Risk_WBS_Table.Rows.Find(new object[] { itemLines.ID, fila[DT_WBS.ID_WBS] }).Delete();
                            }
                        }
                    }
                    foreach (DataRow itemC in WBS_RISK_Damage.Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + RiskRow[DT_Risk.ID] + " AND " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + fila[DT_WBS.ID_WBS]))
                    {
                        itemC.Delete();
                    }
                    fila.Delete();
                }
                foreach (var item in ChildrenLines)
                {
                    RefreshDamageValues(item.ID, item.IsCM);
                }
            }
            catch (Exception EX)
            {
                new WindowMessageOK(EX.Message).ShowDialog();
            }
        }

        private void TextProbability_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!(Versioned.IsNumeric(TextProbability.Text)))
            {
                new WindowMessageOK(StringResources.NUMERIC_FIELD).ShowDialog();
            }
            else
            {
                if (decimal.Parse(TextProbability.Text) > 100)
                {
                    new WindowMessageOK(StringResources.PROBABILITY_FIELD).ShowDialog();
                }
                else
                {
                    if (decimal.Parse(TextProbability.Text) < 0)
                    {
                        new WindowMessageOK(StringResources.PROBABILITY_FIELD).ShowDialog();
                    }
                }
            }
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TextName.Text != "")
                {
                    if (MyRisks.Select(DT_Risk.ID_DIAGRAM + " = " + RiskTreeID + " and " + DT_Risk.NAMESHORT + " = '" + TextName.Text + "' and " + DT_Risk.ID + " <> " + RiskRow[DT_Risk.ID]).Any())
                    {
                        WindowMessageYesNo yesNo = new WindowMessageYesNo("The name [" + TextName.Text + "] Already exists in this diagram. Do you want to use it again?");
                        if (yesNo.ShowDialog() == true)
                        {
                            AceptRisk();
                        }
                    }
                    else
                    {
                        AceptRisk();
                    }
                    foreach (DataRow item in Risk_DamageTable.Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                    {
                        item[DT_Risk_Damages.PROBABILITY] = RiskRow[DT_Risk.PROBABILITY];
                        item[DT_Risk_Damages.RISK_NAMESHORT] = RiskRow[DT_Risk.NAMESHORT].ToString();
                        item[DT_Risk_Damages.ID_WBS] = ID_WBS;
                        item[DT_Risk_Damages.WBS_NAME] = WBS_NAME;
                        item[DT_Risk_Damages.USERNAME] = USER_NAME;
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

        private void RefreshDamageValues(int id, bool isCM)
        {
            if (isCM)
            {
                foreach (DataRow itemDamage in CM_DamageTable.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + id))
                {//primero recorro los Daños de los riesgos
                    decimal valor = 0;
                    foreach (DataRow itemWBS in WBS_CM_Damage.Select(DT_WBS_CM_Damage.ID_CM + " = " + id + " AND " + DT_WBS_CM_Damage.ID_DAMAGE + " = " + itemDamage[DT_CounterM_Damage.ID_DAMAGE]))
                    {//y despues para  sumarlos todos en un mismo daño y encontrar el AD
                        valor += (decimal)itemWBS[DT_WBS_CM_Damage.VALUE];
                    }
                    itemDamage[DT_CounterM_Damage.VALUE] = valor;
                }
            }
            else
            {
                foreach (DataRow itemDamage in Risk_DamageTable.Select(DT_Risk_Damages.ID_RISK + " = " + id))
                {//primero recorro los Daños de los riesgos
                    decimal valor = 0;
                    foreach (DataRow itemWBS in WBS_RISK_Damage.Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + RiskRow[DT_Risk.ID] + " AND " + DT_WBS_RISK_DAMAGE.ID_DAMAGE + " = " + itemDamage[DT_Risk_Damages.ID_DAMAGE]))
                    {//y despues para  sumarlos todos en un mismo daño y encontrar el AD
                        valor += (decimal)itemWBS[DT_WBS_RISK_DAMAGE.VALUE];
                    }
                    itemDamage[DT_Risk_Damages.VALUE] = valor;
                }
            }
        }

        private void AceptRisk()
        {
            RiskRow[DT_Risk.NAMESHORT] = TextName.Text;
            RiskRow[DT_Risk.ISCOLLAPSED] = false;
            RiskRow[DT_Risk.ENABLED] = Enabled;
            RiskRow[DT_Risk.COMMENTS] = TextDetail.Text;
            RiskRow[DT_Risk.ID_DIAGRAM] = RiskTreeID;
            RiskRow[DT_Risk.IS_ROOT] = false;
            RiskRow[DT_Risk.POSITION] = Posicion;
            RiskRow[DT_Risk.ID_WBS] = ID_WBS;
            RiskRow[DT_Risk.WBS_NAME] = WBS_NAME;
            RiskRow[DT_Risk.USER_NAME] = USER_NAME;
            RefreshDamageValues((int)RiskRow[DT_Risk.ID], false);

            if (Versioned.IsNumeric(TextProbability.Text))
            {
                if (Pi.Probability > 100)
                {
                    new WindowMessageOK(StringResources.PROBABILITY_FIELD).ShowDialog();
                }
                else
                {
                    bool flag = true;
                    foreach (DataRow item in Risk_DamageTable.Select(DT_Risk_Damages.ID_RISK_TREE + " = " + RiskRow[DT_Risk.ID_DIAGRAM]))
                    {
                        if ((decimal)item[DT_Risk_Damages.VALUE] < 0)
                        {
                            flag = false;
                            new WindowMessageOK("Value of damages cannot be negative").ShowDialog();
                            break;
                        }
                    }
                    if (flag)
                    {
                        CalculateProbability();
                        RiskRow[DT_Risk.PROBABILITY] = Pi.Probability;
                        this.DialogResult = true;
                    }
                }
            }
            else
            {
                new WindowMessageOK(StringResources.NUMERIC_FIELD).ShowDialog();
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextName_KeyUp(object sender, KeyEventArgs e)
        {
            Editando = true;
            resultStack.Visibility = Visibility.Visible;
            bool found = false;
            var border = (resultStack.Parent as ScrollViewer).Parent as Border;

            string query = (sender as TextBox).Text;

            if (query.Length == 0)
            {
                // Clear   
                resultStack.Children.Clear();
                border.Visibility = Visibility.Collapsed;
            }
            else
            {
                border.Visibility = Visibility.Visible;
            }

            // Clear the list   
            resultStack.Children.Clear();

            // Add the result   
            foreach (DataRow obj in Ds.Tables[DT_DefaulRisk.Risk_TABLA].Rows)
            {
                if (obj[DT_DefaulRisk.RISK_NAME_COLUMNA].ToString().ToLower().StartsWith(query.ToLower()))
                {
                    // The word starts with this... Autocomplete must work   
                    AddItem(obj[DT_DefaulRisk.RISK_NAME_COLUMNA].ToString());
                    found = true;
                }
            }

            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        private void AddItem(string text)
        {
            TextBlock block = new TextBlock
            {
                Text = text,
                Margin = new Thickness(2, 3, 2, 3),
                Cursor = Cursors.Hand
            };

            block.MouseLeftButtonUp += (sender, e) =>
            {
                TextName.Text = (sender as TextBlock).Text;
                resultStack.Visibility = Visibility.Collapsed;
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
                Seleccionando = true;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
                Seleccionando = false;
            };

            resultStack.Children.Add(block);
        }

        private void TextName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TextName.Text == "")
            {
                TextName.ToolTip = "Empty";
            }
            if (Editando)
            {
                if (!Seleccionando)
                {
                    resultStack.Visibility = Visibility.Collapsed;
                    Editando = false;
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

        private void BtnSetPrimary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgWBS.SelectedIndex >= 0)
                {
                    foreach (DataRow item in Risk_WBS_Table.Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                    {
                        item[DT_RISK_WBS.IS_PRIMARY] = false;
                        item[DT_RISK_WBS.PRIMARY] = "";
                    }
                    DvRiskWBS[dgWBS.SelectedIndex].Row[DT_RISK_WBS.IS_PRIMARY] = true;
                    DvRiskWBS[dgWBS.SelectedIndex].Row[DT_RISK_WBS.PRIMARY] = "PRIMARY";
                    ID_WBS = (Int32)DvRiskWBS[dgWBS.SelectedIndex].Row[DT_RISK_WBS.ID_WBS];
                    WBS_NAME = DvRiskWBS[dgWBS.SelectedIndex].Row[DT_RISK_WBS.NIVEL].ToString() + " " + DvRiskWBS[dgWBS.SelectedIndex].Row[DT_RISK_WBS.WBS].ToString();
                    USER_NAME = DvRiskWBS[dgWBS.SelectedIndex].Row[DT_RISK_WBS.USERNAME].ToString();
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void DgWBS_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            CalculateProbability();
        }

        private void DgWBS_LostFocus(object sender, RoutedEventArgs e)
        {
            CalculateProbability();
        }

        private void DgWBS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CalculateProbability();
        }

        private void DgWBS_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            CalculateProbability();
        }
    }
}
