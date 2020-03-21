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
    public class DataCurrentCM : INotifyPropertyChanged
    {
        private bool hasAccess = false;
        private decimal probability;

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
            if (PropertyChanged != null)
                PropertyChanged(this, new
                PropertyChangedEventArgs(property));
        }
    }
    /// <summary>
    /// Interaction logic for WindowCM.xaml
    /// </summary>
    /// 
    public partial class WindowCM : Window
    {
        public DataCurrentCM Pi { get; set; }
        public int ID_WBS { get; set; }
        public string WBS_NAME { get; set; }
        public string USER_NAME { get; set; }
        public string LOGIN_USER { get; set; }
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
        public DataTable WBS_CM_Damage { get; set; }
        private DataSet dsWBS;

        public WindowCM()
        {
            InitializeComponent();
            Pi = new DataCurrentCM();
            TextName.DataContext = Pi;
            TextDetail.DataContext = Pi;
            TextProbability.DataContext = Pi;
            gridTabRoles.DataContext = Pi;
            gridTabWBS.DataContext = Pi;
            ChangeLanguage();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceWBS.WebServiceWBS wsWBS = new ServiceWBS.WebServiceWBS();
                dsWBS = wsWBS.GetAllWBS().Copy();
                wsWBS.Dispose();
                if (Operation == General.INSERT)
                {
                    TextProbability.Text = Probability.ToString();
                    TextFather.Text = DsCM.Tables[DT_Risk.TABLE_NAME].Rows.Find(RiskPadre.ID)[DT_Risk.NAMESHORT].ToString();
                    foreach (DataRow item in DsCM.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + RiskTreeID))
                    {
                        DataRow rowTop = TopRiskTable.NewRow();
                        rowTop[DT_CounterM_Damage.COLOR] = item[DT_Diagram_Damages.COLOR];
                        rowTop[DT_CounterM_Damage.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                        rowTop[DT_CounterM_Damage.ID_COUNTERM] = CMRow[DT_CounterM.ID];
                        rowTop[DT_CounterM_Damage.VALUE] = 0;
                        rowTop[DT_CounterM_Damage.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                        rowTop[DT_CounterM_Damage.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString() + "(" + item[DT_Diagram_Damages.UM] + ")";
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
                        newRow[DT_Role_CM.ID_CM] = CMRow[DT_CounterM.ID];
                        newRow[DT_Role_CM.Role] = item[DT_Role_CM.Role];
                        newRow[DT_Role_CM.IDROL_COLUMN] = item[DT_Role_CM.IDROL_COLUMN];
                        CM_RoleTable.Rows.Add(newRow);
                    }
                    if (RiskPadre != null)
                    {
                        bool hasWBS = false;
                        foreach (DataRow itemWBS in DsCM.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskPadre.ID))
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
                                ID_WBS = (Int32)itemWBS[DT_RISK_WBS.ID_WBS];
                                WBS_NAME = itemWBS[DT_RISK_WBS.NIVEL].ToString() + " " + itemWBS[DT_RISK_WBS.WBS].ToString();
                                USER_NAME = itemWBS[DT_RISK_WBS.USERNAME].ToString();
                            }
                            newRow[DT_CM_WBS.PRIMARY] = itemWBS[DT_RISK_WBS.PRIMARY];
                            newRow[DT_CM_WBS.USERNAME] = itemWBS[DT_RISK_WBS.USERNAME];
                            hasWBS = true;
                            CM_WBS_Table.Rows.Add(newRow);
                            
                            if (WBS_isSheet((Int32)itemWBS[DT_RISK_WBS.ID_WBS]))
                            {
                                foreach (DataRow itemDamage in TopRiskTable.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
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
                                if (!(DsCM.Tables[DT_CM_WBS.TABLENAME].Rows.Contains(new object[] { CMRow[DT_CounterM.ID], item[DT_WBS.ID_WBS] })))
                                {
                                    DataRow drRCMWBS = DsCM.Tables[DT_CM_WBS.TABLENAME].NewRow();
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
                                    DsCM.Tables[DT_CM_WBS.TABLENAME].Rows.Add(drRCMWBS);
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
                        ID_WBS = (Int32)CMRow[DT_Risk.ID_WBS];
                    }
                    foreach (DataRow item in DsCM.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + RiskTreeID))
                    {
                        if (!(TopRiskTable.Rows.Contains(new object[] { CMRow[DT_CounterM.ID], item[DT_Diagram_Damages.ID_DAMAGE] })))
                        {
                            DataRow rowTop = TopRiskTable.NewRow();
                            rowTop[DT_CounterM_Damage.COLOR] = item[DT_Diagram_Damages.COLOR];
                            rowTop[DT_CounterM_Damage.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                            rowTop[DT_CounterM_Damage.ID_COUNTERM] = CMRow[DT_CounterM.ID];
                            rowTop[DT_CounterM_Damage.VALUE] = 0;
                            rowTop[DT_CounterM_Damage.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                            rowTop[DT_CounterM_Damage.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString() + "(" + item[DT_Diagram_Damages.UM] + ")";
                            rowTop[DT_CounterM_Damage.ID_RISK_TREE] = RiskTreeID;
                            rowTop[DT_CounterM_Damage.RISK_REDUCTION] = 0;
                            rowTop[DT_CounterM_Damage.GROUPENAME] = "None";
                            rowTop[DT_CounterM_Damage.RISK] = CMRow[DT_CounterM.RISK_NAMESHORT];
                            rowTop[DT_CounterM_Damage.STATUS] = (Boolean)CMRow[DT_CounterM.ENABLED] ? "Activated" : "Non Activated";
                            rowTop[DT_CounterM_Damage.IDRISK] = CMRow[DT_CounterM.ID_RISK];
                            TopRiskTable.Rows.Add(rowTop);
                        }
                    }
                }

                TextName.Focus();
                DvRoleCM = CM_RoleTable.DefaultView;
                dgRoles.ItemsSource = DvRoleCM;
                DvRoleCM.RowFilter = DT_Role_CM.ID_CM + " = " + CMRow[DT_CounterM.ID];

               

                if (Pi.HasAccess)
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                    Pi.Probability = Probabilities.Sum() / Probabilities.Count;
                }
                else
                {
                    Pi.Probability = 0;
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
                frmSelection.P.FilterString = DT_Role.ROLE_COLUM;
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
                new WindowMessageOK(ex.Message).ShowDialog(); ;
            }
        }

        private void BtnDelRole_Click(object sender, RoutedEventArgs e)
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
            CMRow[DT_CounterM.NAMESHORT] = TextName.Text;
            CMRow[DT_CounterM.DETAIL] = TextDetail.Text;
            CMRow[DT_CounterM.ID_RISK_TREE] = RiskTreeID;
            CMRow[DT_CounterM.ID_RISK] = RiskPadre.ID;
            CMRow[DT_CounterM.POSITION] = Posicion;
            CMRow[DT_CounterM.ENABLED] = true;
            CMRow[DT_CounterM.ID_WBS] = ID_WBS;
            CMRow[DT_CounterM.WBS_NAME] = WBS_NAME;
            CMRow[DT_CounterM.USER_NAME] = USER_NAME;
            foreach (DataRow itemDamage in TopRiskTable.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
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
                if (Pi.Probability > 100)
                {
                    new WindowMessageOK(StringResources.PROBABILITY_FIELD).ShowDialog();
                }
                else
                {
                    bool flag = true;
                    foreach (DataRow item in TopRiskTable.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
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
                        CalculateProbability();
                        CMRow[DT_CounterM.PROBABILITY] = Pi.Probability;
                        this.DialogResult = true;
                    }
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
                        + DT_CounterM.NAMESHORT + " = '" + TextName.Text + "' and " + DT_CounterM.ID + " <> " + CMRow[DT_CounterM.ID]).Any())
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
                    foreach (DataRow item in TopRiskTable.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
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
                return BuscarAncestros((Int32)BuscarMiPadre(idWBS)[DT_WBS.ID_WBS], dtWBSAncestors);
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
                if (CM_WBS_Table.Select(DT_CM_WBS.ID_CM + " = " + (Int32)CMRow[DT_CounterM.ID]).Count() > 0)
                {
                    frmSelection.Dt = General.DeleteExists(roleCodif, CM_WBS_Table.Select(DT_CM_WBS.ID_CM + " = " + (Int32)CMRow[DT_CounterM.ID]).CopyToDataTable(), DT_WBS.ID_WBS);
                }
                else
                {
                    frmSelection.Dt = roleCodif;
                }

                frmSelection.DcolumToShow = new string[] { DT_WBS.NIVEL, DT_WBS.WBS_NAME };
                frmSelection.DcolumToShowAlias = new string[] { "Level", DT_WBS.WBS_NAME };
                frmSelection.Title = "WBS";

                frmSelection.P.FilterString = "WBS";
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
                        foreach (DataRow itemAncestors in BuscarAncestros((Int32)itemRISKWBSi[DT_RISK_WBS.ID_WBS], dsWBS.Tables[DT_WBS.TABLE_NAME].Clone()).Rows)
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
                        if (WBS_isSheet((Int32)itemWBS[DT_CM_WBS.ID_WBS]))
                        {
                            foreach (DataRow itemDamage in TopRiskTable.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
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
                    foreach (DataRow itemR in (WBS_CM_Damage.Select(DT_WBS_CM_Damage.ID_WBS + " = " + fila[DT_WBS.ID_WBS] + " AND " + DT_WBS_CM_Damage.ID_CM + " = " + CMRow[DT_CounterM.ID])))
                    {
                        itemR.Delete();
                    }
                    fila.Delete();
                }
            }
            catch (Exception EX)
            {
                new WindowMessageOK(EX.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
    }
}
