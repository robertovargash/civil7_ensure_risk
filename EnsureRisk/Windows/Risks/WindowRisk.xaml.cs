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
using EnsureRisk.Classess;

namespace EnsureRisk.Windows
{
    /// <summary>
    /// Interaction logic for WindowRisk.xaml
    /// </summary>
    public partial class WindowRisk : Window, INotifyPropertyChanged
    {
        private bool hasAccess = false;
        private decimal probability;
        private string _shortName, probabilityHint;
        private bool isCM;
        private IEnumerable<string> _RiskNameList;
        public IEnumerable<string> RiskNameList { get { return _RiskNameList; } set { _RiskNameList = value; OnPropertyChanged("RiskNameList"); } }
        public string ShortName { get { return _shortName; } set { _shortName = value; OnPropertyChanged("ShortName"); } }

        public string ProbabilityHint { get { return probabilityHint; } set { probabilityHint = value; OnPropertyChanged("ProbabilityHint"); } }

        public bool IsCM { get { return isCM; } set { isCM = value; OnPropertyChanged("IsCM"); } }

        public decimal Probability { get { return probability; } set { probability = value; OnPropertyChanged("Probability"); } }

        public bool HasAccess { get { return hasAccess; }  set { hasAccess = value; OnPropertyChanged("HasAccess"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public bool IS_USING_NAME { get; set; } = false;
        public bool IS_DELETING_ROW { get; set; } = false;
        public DataRow Selected_RoleRow { get; set; }
        public DataRow Selected_WBSRow { get; set; }
        public DataRow RiskRow { get; set; }
        public string Operation { get; set; }
        public decimal RiskTreeID { get; set; }
        public decimal ID_PROJECT { get; set; }
        public decimal ID_WBS { get; set; }
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
        public RiskPolyLine RiskSelected { get; set; }
        public bool Enabled { get; set; }
        public int Posicion { get; set; }
        private DataSet dsWBS;

        private bool Editando;

        public WindowRisk()
        {
            InitializeComponent();
            ChangeLanguage();
            RiskName.DataContext = this;
            TextDetail.DataContext = this;
            TextProbability.DataContext = this;
            gridTabRoles.DataContext = this;
            gridTabWBS.DataContext = this;
            headerProb.DataContext = this;
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
                //TODO: 11-EXPLICAR LA HERENCIA DE LOS WBS DE PADRE A HIJOS (EN RIESGOS Y CMs), COMO CADA UNO LE PASA SUS WBS A SU HIJO
                //ADEMAS DE LAS DUDAS QUE AUN PERSISTEN EN CUANTO A, QUE PASA SI UN RISK PADRE PIERDE UN WBS, SU RISK HIJO TAMBIEN LO PIERDE?
                IsCM = (bool)RiskRow[DT_Risk.IS_CM];
                ServiceWBS.WebServiceWBS wsWBS = new ServiceWBS.WebServiceWBS();
                dsWBS = wsWBS.GetAllWBSFiltered(new object[] { ID_PROJECT }).Copy();
                wsWBS.Dispose();
                if (Operation == General.INSERT)
                {
                    RiskRow[DT_Risk.PROBABILITY] = (bool)RiskRow[DT_Risk.IS_CM] ? 0 : 100;
                    Probability = (decimal)RiskRow[DT_Risk.PROBABILITY];
                    Enabled = true;
                    if (RowFather != null)
                    {
                        TextFather.Text = RowFather[DT_Risk.NAMESHORT].ToString();
                        RiskRow[DT_Risk.IDRISK_FATHER] = RowFather[DT_Risk.ID];
                    }
                    RiskRow[DT_Risk.IS_ACTIVE] = true;

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
                    Enabled = (bool)RiskRow[DT_Risk.IS_ACTIVE];
                    //ShortName = RiskRow[DT_Risk.NAMESHORT].ToString();
                    RiskName.Text = RiskRow[DT_Risk.NAMESHORT].ToString();
                    TextDetail.Text = RiskRow[DT_Risk.COMMENTS].ToString();
                    Probability = (decimal)RiskRow[DT_Risk.PROBABILITY];
                    if (RiskRow[DT_Risk.ID_WBS] != DBNull.Value)
                    {
                        ID_WBS = (decimal)RiskRow[DT_Risk.ID_WBS];
                    }

                    SetTableRisk_Damages(true, Enabled);
                    TextFather.Text = RowFather[DT_Risk.NAMESHORT].ToString();
                }
                RiskName.Focus();
                DvRoleRisk = Ds.Tables[DT_Role_Risk.TABLE_NAME].DefaultView;
                //DvRoleRisk = Ds.Tables[DT_Role_Risk.TABLENAME].DefaultView;

                dgRoles.ItemsSource = DvRoleRisk;
                DvRoleRisk.RowFilter = DT_Role_Risk.ID_RISK + " = " + RiskRow[DT_Risk.ID];

                DvRiskWBS = Ds.Tables[DT_RISK_WBS.TABLE_NAME].DefaultView;
                //DvRiskWBS = Ds.Tables[DT_RISK_WBS.TABLE_NAME].DefaultView;
                dgWBS.ItemsSource = DvRiskWBS;
                //DvRiskWBS.RowFilter = DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID];
                DvRiskWBS.RowFilter = DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID];

                DvTopRisk = Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].DefaultView;
                dgTopRisk.ItemsSource = DvTopRisk;

                //TODO 14- AQUI ESTA EL FILTRO, SI EL USUARIO NO ES WBS MAS BAJITO EN EL RISK, NO TENDRA NINGUNA ROW EN ESTA TABLA
                //POR LO QUE NO APARECERA NADA, TAL Y COMO HEMOS VISTO EN LOS EJEMPLOS.
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
                if (IsCM)
                {
                    RiskNameList = Ds.Tables[DT_Risk.TABLE_NAME].AsEnumerable()
                                        .Where(r => r.Field<bool>(DT_Risk.IS_CM) == true)
                                        .Select(r => r.Field<string>(DT_Risk.NAMESHORT)).Distinct();
                }
                else
                {
                    RiskNameList = Ds.Tables[DT_Risk.TABLE_NAME].AsEnumerable()
                                        .Where(r => r.Field<bool>(DT_Risk.IS_CM) == false)
                                        .Select(r => r.Field<string>(DT_Risk.NAMESHORT)).Distinct();
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// Add to Risk the Damages from the Diagram
        /// </summary>
        private void SetTableRisk_Damages(bool useProbability, bool isEnabled)
        {
            foreach (DataRow item in Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + RiskTreeID))
            {
                if (!(Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { RiskRow[DT_Risk.ID], item[DT_Diagram_Damages.ID_DAMAGE] })))
                {
                    DataRow rowTop = Ds.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                    rowTop[DT_Risk_Damages.COLOR] = item[DT_Diagram_Damages.COLOR];
                    rowTop[DT_Risk_Damages.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                    rowTop[DT_Risk_Damages.ID_RISK] = RiskRow[DT_Risk.ID];
                    rowTop[DT_Risk_Damages.ID_RISK_TREE] = RiskTreeID;
                    rowTop[DT_Risk_Damages.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString();
                    //rowTop[DT_Risk_Damages.DAMAGE] = item[DT_Diagram_Damages.TOP_RISK].ToString() + "(" + item[DT_Diagram_Damages.UM] + ")";
                    rowTop[DT_Risk_Damages.VALUE] = 0;
                    rowTop[DT_Risk_Damages.TOP_RISK] = item[DT_Diagram_Damages.TOP_RISK];
                    rowTop[DT_Risk_Damages.PROBABILITY] = useProbability ? Probability : 0;
                    rowTop[DT_Risk_Damages.GROUPE_NAME] = "None";
                    rowTop[DT_Risk_Damages.RISK_NAMESHORT] = "";
                    rowTop[DT_Risk_Damages.FATHER] = RowFather[DT_Risk.NAMESHORT].ToString();
                    rowTop[DT_Risk_Damages.STATUS] = isEnabled;
                    rowTop[DT_Risk_Damages.ID_FATHER] = RiskRow[DT_Risk.IDRISK_FATHER];
                    rowTop[DT_Risk_Damages.RISK_TREE] = Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(RiskTreeID)[DT_Diagram.DIAGRAM_NAME].ToString();
                    rowTop[DT_Risk_Damages.IS_ROOT] = false;
                    rowTop[DT_Risk_Damages.IS_CM] = RiskRow[DT_Risk.IS_CM];
                    Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(rowTop);
                }
            }
        }

        /// <summary>
        /// Set the Roles from the Risk Father
        /// </summary>
        private void SetTableRisk_Role()
        {
            if (RiskSelected != null)
            {
                foreach (DataRow item in Ds.Tables[DT_Role_Risk.TABLE_NAME].Select(DT_Role_Risk.ID_RISK + " = " + RiskSelected.ID))
                {
                    DataRow newRow = Ds.Tables[DT_Role_Risk.TABLE_NAME].NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = RiskRow[DT_Risk.ID];
                    newRow[DT_Role_Risk.Role] = item[DT_Role_Risk.Role];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                    Ds.Tables[DT_Role_Risk.TABLE_NAME].Rows.Add(newRow);
                }
            }
        }

        /// <summary>
        /// Set the WBS from the Risk Father
        /// </summary>
        private void SetTableRisk_WBS()
        {
            if (RiskSelected != null)
            {
                bool hasWBS = false;
                if (!RiskSelected.IsRoot)
                {
                    foreach (DataRow itemWBS in Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskSelected.ID))
                    {
                        DataRow newRow = Ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                        newRow[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                        newRow[DT_RISK_WBS.ID_WBS] = itemWBS[DT_RISK_WBS.ID_WBS];
                        newRow[DT_RISK_WBS.NIVEL] = itemWBS[DT_RISK_WBS.NIVEL];
                        newRow[DT_RISK_WBS.WBS] = itemWBS[DT_RISK_WBS.WBS];
                        newRow[DT_RISK_WBS.IS_PRIMARY] = itemWBS[DT_RISK_WBS.IS_PRIMARY];
                        newRow[DT_RISK_WBS.USERNAME] = itemWBS[DT_RISK_WBS.USERNAME];
                        newRow[DT_RISK_WBS.WBS_USER] = itemWBS[DT_RISK_WBS.WBS] + "[" + itemWBS[DT_RISK_WBS.USERNAME] + "]";
                        newRow[DT_RISK_WBS.PROBABILITY] = (bool)RiskRow[DT_Risk.IS_CM] ? 0 : 100;
                        if ((bool)itemWBS[DT_RISK_WBS.IS_PRIMARY])
                        {
                            ID_WBS = (decimal)itemWBS[DT_RISK_WBS.ID_WBS];
                            WBS_NAME = itemWBS[DT_RISK_WBS.NIVEL].ToString() + " " + itemWBS[DT_RISK_WBS.WBS].ToString();
                            USER_NAME = itemWBS[DT_RISK_WBS.USERNAME].ToString();
                        }
                        Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRow);
                        hasWBS = true;
                        SetTableRisk_WBS_Damage(itemWBS);
                    }
                    CompleteRiskWBSTable(hasWBS);
                }
                else
                {
                    CompleteRiskWBSTable(false);
                    foreach (DataRow itemWBS in Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + (decimal)RiskRow[DT_Risk.ID]))
                    {
                        if (!(Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { RiskRow[DT_Risk.ID], itemWBS[DT_RISK_WBS.ID_WBS] })))
                        {
                            DataRow newRow = Ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                            newRow[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                            newRow[DT_RISK_WBS.ID_WBS] = itemWBS[DT_RISK_WBS.ID_WBS];
                            newRow[DT_RISK_WBS.NIVEL] = itemWBS[DT_RISK_WBS.NIVEL];
                            newRow[DT_RISK_WBS.WBS] = itemWBS[DT_RISK_WBS.WBS];
                            newRow[DT_RISK_WBS.IS_PRIMARY] = itemWBS[DT_RISK_WBS.IS_PRIMARY];
                            newRow[DT_RISK_WBS.USERNAME] = itemWBS[DT_RISK_WBS.USERNAME];
                            newRow[DT_RISK_WBS.PROBABILITY] = (bool)RiskRow[DT_Risk.IS_CM] ? 0 : 100;
                            newRow[DT_RISK_WBS.WBS_USER] = itemWBS[DT_RISK_WBS.WBS] + "[" + itemWBS[DT_RISK_WBS.USERNAME] + "]";
                            if ((bool)itemWBS[DT_RISK_WBS.IS_PRIMARY])
                            {
                                ID_WBS = (decimal)itemWBS[DT_RISK_WBS.ID_WBS];
                                WBS_NAME = itemWBS[DT_RISK_WBS.NIVEL].ToString() + " " + itemWBS[DT_RISK_WBS.WBS].ToString();
                                USER_NAME = itemWBS[DT_RISK_WBS.USERNAME].ToString();
                            }
                            Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(newRow);
                        }
                        SetTableRisk_WBS_Damage(itemWBS);
                    }
                }
            }
        }

        /// <summary>
        /// Complete with the WBS Father of the Selected WBS
        /// </summary>
        private void CompleteRiskWBSTable(bool hasWBS)
        {
            //TODO: 12-LA IMPORTANCIA DE ESTE METODO ES EN CASO DE ERROR DEL SISTEMA, DE NO HEREDAR LOS WBS DEL PADRE
            //AL MENOS HEREDA EL WBS QUE ESTA MAS AL TOP, PUES AL MENOS GARANTIZA QUE EL SUPER MANAGER DEL PROYECTO TENGA ACCESO
            //DE LO CONTRARIO SE CREARIA UN RISK, QUE NADIE PODRIA ACCEDER A EL LUEGO, NI SIQUIERA ELIMINANDOLO, SOLO POR DENTRO DE LA BD
            //COSA QUE SE HACE EN UNA EMPRESA QUE CONOCEMOS BIEN
            foreach (var item in WBSOperations.GetTopWBS(dsWBS))
            {
                if (!(Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { RiskRow[DT_Risk.ID], item[DT_WBS.ID_WBS] })))
                {
                    DataRow drRiskWBS = Ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                    drRiskWBS[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                    drRiskWBS[DT_RISK_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                    drRiskWBS[DT_RISK_WBS.WBS] = item[DT_WBS.WBS_NAME];
                    drRiskWBS[DT_RISK_WBS.NIVEL] = item[DT_WBS.NIVEL];
                    drRiskWBS[DT_RISK_WBS.USERNAME] = item[DT_WBS.USERNAME];
                    drRiskWBS[DT_RISK_WBS.WBS_USER] = item[DT_WBS.WBS_NAME] + "[" + item[DT_WBS.USERNAME] + "]";
                    drRiskWBS[DT_RISK_WBS.PROBABILITY] = (bool)RiskRow[DT_Risk.IS_CM] ? 0 : 100; ;
                    if (!hasWBS)
                    {
                        hasWBS = true;
                        drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = true;
                        ID_WBS = (decimal)item[DT_WBS.ID_WBS];
                        USER_NAME = item[DT_WBS.USERNAME].ToString();
                        WBS_NAME = item[DT_WBS.NIVEL].ToString() + " " + item[DT_WBS.WBS_NAME].ToString();
                    }
                    else
                    {
                        drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = false;
                    }
                    Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBS);
                }
            }
        }

        /// <summary>
        /// Validate if the WBS is in the low level of the risk and Add a Damage, if not, delete the damage and not have incidence on the line.
        /// </summary>
        private void SetTableRisk_WBS_Damage(DataRow riskWBS)
        {
            //TODO: 13-Y AQUI ES DONDE SE HACE LA MAGIA DE LOS VALORES DE LOS DAMAGE,
            //SIEMPRE SE VALIDA QUE SEA EL WBS MAS BAJITO EN EL RISK, 
            //IR AHORA AL todo 14 PARA VER EL FILTRO
            if (WBSOperations.IsRiskWBSLow(riskWBS, dsWBS, Ds.Tables[DT_RISK_WBS.TABLE_NAME]))
            {
                //AddWBS_Risk_Damage((decimal)RiskRow[DT_Risk.ID], riskWBS);
                WBSOperations.TabAddWBS_LINE_Damage(riskWBS, (decimal)RiskRow[DT_Risk.ID], false, Ds);
            }
            else
            {
                //DeleteWBS_Risk_Damage((decimal)RiskRow[DT_Risk.ID], riskWBS);
                WBSOperations.TabDeleteWBS_LINE_Damage(riskWBS, (decimal)RiskRow[DT_Risk.ID], false, Ds);
            }
        }

        /// <summary>
        /// Calculate the probability using an average with the WBS of the risk. just takes the lowest levels WBS
        /// </summary>
        private void CalculateProbability()
        {
            try
            {
                Probability = WBSOperations.RiskWBSValidations(RiskRow, Ds, LOGIN_USER, dsWBS, hasAccess, IsCM);                
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void CalculateProbability(DataRow RiskRow)
        {
            try
            {
                Probability = WBSOperations.RiskWBSValidations(RiskRow, Ds, LOGIN_USER, dsWBS, hasAccess, (bool)RiskRow[DT_Risk.IS_CM]);

                RiskRow[DT_Risk.PROBABILITY] = Probability;
                foreach (DataRow item in Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                {
                    item[DT_Risk_Damages.PROBABILITY] = RiskRow[DT_Risk.PROBABILITY];
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public void ChangeLanguage()
        {
            MaterialDesignThemes.Wpf.HintAssist.SetHint(RiskName, StringResources.ShortNameText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextFather, StringResources.FatherText);
            MaterialDesignThemes.Wpf.HintAssist.SetHint(TextDetail, StringResources.RiskDetailText);
            //LabelProbability.Content = StringResources.ProbabilityLabel;
            BtnCancel.Content = StringResources.CancelButton;
            //tabRoles.Header = StringResources.TabValues;
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
                if (Ds.Tables[DT_Role_Risk.TABLE_NAME].Select(DT_Role_Risk.ID_RISK + " = " + (decimal)RiskRow[DT_Risk.ID]).Count() > 0)
                {
                    frmSelection.Dt = General.DeleteExists(roleCodif, Ds.Tables[DT_Role_Risk.TABLE_NAME].Select(DT_Role_Risk.ID_RISK + " = " + (decimal)RiskRow[DT_Risk.ID]).CopyToDataTable(), DT_Role.IDROL_COLUMN);
                }
                else
                {
                    frmSelection.Dt = roleCodif;
                }

                frmSelection.DcolumToShow = new string[] { DT_Role.ROLE_COLUM };
                frmSelection.DcolumToShowAlias = new string[] { DT_Role.ROLE_COLUM };
                frmSelection.Title = DT_Role.ROLE_COLUM;
                frmSelection.FilterString = "Role Name";
                frmSelection.ColumnToFilter = DT_Role.ROLE_COLUM;
                if (frmSelection.ShowDialog() == true)
                {
                    foreach (DataRow itemRole in frmSelection.RowsSelected)
                    {
                        DataRow drRole = Ds.Tables[DT_Role_Risk.TABLE_NAME].NewRow();
                        drRole[DT_Role_Risk.ID_RISK] = RiskRow[DT_Risk.ID];
                        drRole[DT_Role_Risk.NAME_SHORT] = RiskName.Text;
                        drRole[DT_Role_Risk.Role] = itemRole[DT_Role.ROLE_COLUM];
                        drRole[DT_Role_Risk.IDROL_COLUMN] = itemRole[DT_Role.IDROL_COLUMN];
                        Ds.Tables[DT_Role_Risk.TABLE_NAME].Rows.Add(drRole);
                        if (Operation == General.UPDATE)
                        {
                            foreach (var item in ChildrenLines)
                            {
                                if (!(Ds.Tables[DT_Role_Risk.TABLE_NAME].Rows.Contains(new object[] { item.ID, itemRole[DT_Role.IDROL_COLUMN] })))
                                {
                                    DataRow drRoleRisk = Ds.Tables[DT_Role_Risk.TABLE_NAME].NewRow();
                                    drRoleRisk[DT_Role_Risk.ID_RISK] = item.ID;
                                    drRoleRisk[DT_Role_Risk.NAME_SHORT] = item.ShortName;
                                    drRoleRisk[DT_Role_Risk.Role] = itemRole[DT_Role.ROLE_COLUM];
                                    drRoleRisk[DT_Role_Risk.IDROL_COLUMN] = itemRole[DT_Role.IDROL_COLUMN];
                                    Ds.Tables[DT_Role_Risk.TABLE_NAME].Rows.Add(drRoleRisk);
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

        private void Delete_Role(DataRow fila)
        {
            try
            {
                if (Operation == General.UPDATE)
                {
                    foreach (var item in ChildrenLines)
                    {
                        if (Ds.Tables[DT_Role_Risk.TABLE_NAME].Rows.Contains(new object[] { item.ID, fila[DT_Role.IDROL_COLUMN] }))
                        {
                            Ds.Tables[DT_Role_Risk.TABLE_NAME].Rows.Find(new object[] { item.ID, fila[DT_Role.IDROL_COLUMN] }).Delete();
                        }
                    }
                }
                fila.Delete();
                IS_DELETING_ROW = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_ROW = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnDelRole_Click(object sender, RoutedEventArgs e)
        {
            if (dgRoles.SelectedIndex >= 0)
            {
                Selected_RoleRow = DvRoleRisk[dgRoles.SelectedIndex].Row;
                if (Selected_RoleRow[DT_Role_Risk.Role].ToString() != "Administrator")
                {
                    IS_DELETING_ROW = true;
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + Selected_RoleRow[DT_Role_Risk.Role].ToString() + "]?");
                }
                else
                {
                    MostrarErrorDialog("'Administrator' role can't be deleted!");
                }
            }
        }

        private void BtnAddWBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //TODO 15- TAMBIEN EXPLICAR LA BRUJERIA QUE SUPONE AGREGAR UN NUEVO WBS,
                //TENER EN CUENTA QUE SI EL WBS AGREGADO ES EL UNICO BAJITO, CAMBIA TODO, ABSOLUTAMENTE TODO!!!!!!!!
                //HASTA AQUI LAS CLASES, SOLO MUESTRA LOS BINDING EN EL XAML
                ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                DataTable wbsCodif = ws.GetAllWBSFiltered(new object[] { ID_PROJECT }).Tables[DT_WBS.TABLE_NAME].Copy();
                ws.Dispose();
                WindowSelection frmSelection = new WindowSelection();
                if (Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + (decimal)RiskRow[DT_Risk.ID]).Count() > 0)
                {
                    if (Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + (decimal)RiskRow[DT_Risk.ID] + " and " + DT_RISK_WBS.USERNAME + " = '" + LOGIN_USER + "'").Any())
                    {
                        DataTable dtExistence = General.DeleteExists(wbsCodif, Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + (decimal)RiskRow[DT_Risk.ID]).CopyToDataTable(), DT_WBS.ID_WBS);
                        frmSelection.Dt = WBSOperations.GetMyDescendantsAndDeleteExistents(wbsCodif, Ds.Tables[DT_RISK_WBS.TABLE_NAME], dtExistence, LOGIN_USER);
                    }
                    else
                    {
                        frmSelection.Dt = wbsCodif.Clone();
                    }
                }
                else
                {
                    frmSelection.Dt = wbsCodif;
                }
                foreach (DataRow item in frmSelection.Dt.Rows)
                {
                    item[DT_WBS.WBS_NAME] = item[DT_WBS.WBS_NAME].ToString().Insert(0, TreeOperation.Spaces(item[DT_WBS.NIVEL].ToString().Split('.').Count() - 1));
                    item[DT_WBS.NIVEL] = item[DT_WBS.NIVEL].ToString().Insert(0, TreeOperation.Spaces(item[DT_WBS.NIVEL].ToString().Split('.').Count() - 1));
                }
                frmSelection.DcolumToShow = new string[] { DT_WBS.NIVEL, DT_WBS.WBS_NAME, DT_WBS.USERNAME };
                frmSelection.DcolumToShowAlias = new string[] { "Level", DT_WBS.WBS_NAME, "User Owner" };
                frmSelection.Title = "WBS";
                frmSelection.FilterString = "WBS Name";
                frmSelection.ColumnToFilter = DT_WBS.WBS_NAME;
                if (frmSelection.ShowDialog() == true)
                {
                    foreach (DataRow itemWBS in frmSelection.RowsSelected)//Por cada WBS seleccionado lo agrega al riesgo
                    {
                        DataRow drRiskWBS = Ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                        drRiskWBS[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                        drRiskWBS[DT_RISK_WBS.RISK] = RiskName.Text;
                        drRiskWBS[DT_RISK_WBS.WBS] = itemWBS[DT_WBS.WBS_NAME].ToString().TrimStart();
                        drRiskWBS[DT_RISK_WBS.WBS_USER] = itemWBS[DT_WBS.WBS_NAME].ToString().TrimStart() + "[" + itemWBS[DT_WBS.USERNAME] + "]";
                        drRiskWBS[DT_RISK_WBS.ID_WBS] = itemWBS[DT_WBS.ID_WBS];
                        drRiskWBS[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                        drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = false;
                        drRiskWBS[DT_RISK_WBS.USERNAME] = itemWBS[DT_WBS.USERNAME];
                        drRiskWBS[DT_RISK_WBS.PROBABILITY] = 100;
                        Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBS);
                        foreach (DataRow itemAncestors in WBSOperations.GetAncestors((decimal)itemWBS[DT_WBS.ID_WBS], dsWBS.Tables[DT_WBS.TABLE_NAME].Clone(), dsWBS).Rows)
                        {
                            if (!(Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { RiskRow[DT_Risk.ID], itemAncestors[DT_WBS.ID_WBS] })))
                            {
                                DataRow drRiskWBSi = Ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                                drRiskWBSi[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                                drRiskWBSi[DT_RISK_WBS.RISK] = RiskName.Text;
                                drRiskWBSi[DT_RISK_WBS.WBS] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart();
                                drRiskWBSi[DT_RISK_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                drRiskWBSi[DT_RISK_WBS.NIVEL] = itemAncestors[DT_WBS.NIVEL].ToString().TrimStart();
                                drRiskWBSi[DT_RISK_WBS.USERNAME] = itemAncestors[DT_WBS.USERNAME];
                                drRiskWBSi[DT_RISK_WBS.WBS_USER] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart() + "[" + itemAncestors[DT_WBS.USERNAME] + "]";
                                drRiskWBSi[DT_RISK_WBS.IS_PRIMARY] = false;
                                drRiskWBSi[DT_RISK_WBS.PROBABILITY] = 100;
                                Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBSi);
                            }
                        }
                        if (Operation == General.UPDATE)
                        {
                            foreach (var childLine in TreeOperation.GetOnlyMyChildrenWithCM(RiskSelected))
                            {
                                if (!(Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { childLine.ID, itemWBS[DT_WBS.ID_WBS] })))
                                {
                                    DataRow drRiskWBSS = Ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                                    drRiskWBSS[DT_RISK_WBS.ID_RISK] = childLine.ID;
                                    drRiskWBSS[DT_RISK_WBS.RISK] = childLine.ShortName;
                                    drRiskWBSS[DT_RISK_WBS.WBS] = itemWBS[DT_WBS.WBS_NAME].ToString().TrimStart();
                                    drRiskWBSS[DT_RISK_WBS.ID_WBS] = itemWBS[DT_WBS.ID_WBS];
                                    drRiskWBSS[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                    drRiskWBSS[DT_RISK_WBS.USERNAME] = itemWBS[DT_WBS.USERNAME];
                                    drRiskWBSS[DT_RISK_WBS.WBS_USER] = itemWBS[DT_WBS.WBS_NAME].ToString().TrimStart() + "[" + itemWBS[DT_WBS.USERNAME] + "]";
                                    drRiskWBSS[DT_RISK_WBS.IS_PRIMARY] = false;
                                    drRiskWBSS[DT_RISK_WBS.PROBABILITY] = 100;
                                    Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBSS);
                                }
                                foreach (DataRow itemWBSRisk in Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + childLine.ID))
                                {
                                    if (WBSOperations.IsRiskWBSLow(itemWBSRisk, dsWBS, Ds.Tables[DT_RISK_WBS.TABLE_NAME]))
                                    {
                                        //AddWBS_Risk_Damage(childLine.ID, itemWBSRisk);
                                        WBSOperations.TabAddWBS_LINE_Damage(itemWBSRisk, childLine.ID, false, Ds);
                                    }
                                    else
                                    {
                                        //DeleteWBS_Risk_Damage(childLine.ID, itemWBSRisk);
                                        WBSOperations.TabDeleteWBS_LINE_Damage(itemWBSRisk, childLine.ID, false, Ds);
                                    }
                                }
                                foreach (DataRow itemRISKWBSi in Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + childLine.ID))
                                {
                                    foreach (DataRow itemAncestors in WBSOperations.GetAncestors((decimal)itemRISKWBSi[DT_RISK_WBS.ID_WBS], dsWBS.Tables[DT_WBS.TABLE_NAME].Clone(), dsWBS).Rows)
                                    {
                                        if (!(Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { childLine.ID, itemAncestors[DT_WBS.ID_WBS] })))
                                        {
                                            DataRow drRiskWBSi = Ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                                            drRiskWBSi[DT_RISK_WBS.ID_RISK] = childLine.ID;
                                            drRiskWBSi[DT_RISK_WBS.RISK] = childLine.ShortName;
                                            drRiskWBSi[DT_RISK_WBS.WBS] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart();
                                            drRiskWBSi[DT_RISK_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                            drRiskWBSi[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                            drRiskWBSi[DT_RISK_WBS.USERNAME] = itemAncestors[DT_WBS.USERNAME];
                                            drRiskWBSi[DT_RISK_WBS.WBS_USER] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart() + "[" + itemAncestors[DT_WBS.USERNAME] + "]";
                                            drRiskWBSi[DT_RISK_WBS.IS_PRIMARY] = false;
                                            drRiskWBSi[DT_RISK_WBS.PROBABILITY] = 100;
                                            Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBSi);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (DataRow itemWBS in Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                    {
                        if (WBSOperations.IsRiskWBSLow(itemWBS, dsWBS, Ds.Tables[DT_RISK_WBS.TABLE_NAME]))
                        {
                            //AddWBS_Risk_Damage((decimal)RiskRow[DT_Risk.ID], itemWBS);
                            WBSOperations.TabAddWBS_LINE_Damage(itemWBS, (decimal)RiskRow[DT_Risk.ID], false, Ds);
                        }
                        else
                        {//TENGO QUE BORRAR EL DAMAGE_WBS_RISK, PUES YA NO ES LOWLEVEL
                            //DeleteWBS_Risk_Damage((decimal)RiskRow[DT_Risk.ID], itemWBS);
                            WBSOperations.TabDeleteWBS_LINE_Damage(itemWBS, (decimal)RiskRow[DT_Risk.ID], false, Ds);
                        }
                    }
                    CalculateProbability();
                    RefreshDamageValues((decimal)RiskRow[DT_Risk.ID], false);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnDelWBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Selected_WBSRow = DvRiskWBS[dgWBS.SelectedIndex].Row;
                MaterialDesignThemes.Wpf.DialogHost dialog = new MaterialDesignThemes.Wpf.DialogHost
                {
                    DialogContent = new DialogContent.DeleteWBSConfirm()
                };
                dialog.DialogClosing += DeleteForChildrenThisWBSConfirm; ;
                Grid.SetRowSpan(dialog, 3);
                supergrid.Children.Add(dialog);
                dialog.IsOpen = true;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DeleteForChildrenThisWBSConfirm(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            try
            {
                if (eventArgs.Parameter is int parametro)
                {
                    switch (parametro)
                    {
                        case 0:
                            decimal idWBStoDelete = (decimal)Selected_WBSRow[DT_RISK_WBS.ID_WBS];
                            DeleteOnlyRisk_WBS(Selected_WBSRow, (decimal)RiskRow[DT_Risk.ID]);
                            foreach (var linee in ChildrenLines)
                            {
                                foreach (var line in TreeOperation.GetMeAndMyChildrenWithCM(linee))
                                {
                                    DataRow drWBSRiskCHild = Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Find(new object[] { line.ID, idWBStoDelete });
                                    DeleteOnlyRisk_WBS(drWBSRiskCHild, line.ID);
                                }
                            }
                            break;
                        case 1:
                            DeleteOnlyRisk_WBS(Selected_WBSRow, (decimal)RiskRow[DT_Risk.ID]);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DeleteOnlyRisk_WBS(DataRow fila, decimal riskID)
        {
            foreach (DataRow itemC in Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + riskID + " AND " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + fila[DT_WBS.ID_WBS]))
            {
                itemC.Delete();
            }

            //WBSOperations.SetDefaultWBSPrimary(fila, TheCurrentLayout.Ds, riskID);
            WBSOperations.SetAsPrimaryWhoDelete(fila, Ds, LOGIN_USER, riskID, dsWBS);
            foreach (DataRow descendant in WBSOperations.MyWBSDescendants(dsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(fila[DT_RISK_WBS.ID_WBS]), dsWBS.Tables[DT_WBS.TABLE_NAME]))
            {
                if (Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { riskID, descendant[DT_WBS.ID_WBS] }))
                {
                    Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Find(new object[] { riskID, descendant[DT_WBS.ID_WBS] }).Delete();
                }
            }
            fila.Delete();
            foreach (DataRow itemWBS in Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + riskID))
            {
                if (WBSOperations.IsRiskWBSLow(itemWBS, dsWBS, Ds.Tables[DT_RISK_WBS.TABLE_NAME]))
                {
                    WBSOperations.TabAddWBS_LINE_Damage(itemWBS, riskID, false, Ds);
                }
                else
                {//TENGO QUE BORRAR EL DAMAGE_WBS_RISK, PUES YA NO ES LOWLEVEL
                    WBSOperations.TabDeleteWBS_LINE_Damage(itemWBS, riskID, false, Ds);
                }
            }
            CalculateProbability(Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(riskID));
            RefreshRiskDamageValues(Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(riskID));
        }

        private void RefreshRiskDamageValues(DataRow RiskRow)
        {
            foreach (DataRow itemDamage in Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
            {//primero recorro los Daños de los riesgos
                decimal valor = 0;
                foreach (DataRow itemWBS in Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + RiskRow[DT_Risk.ID] + " AND " + DT_WBS_RISK_DAMAGE.ID_DAMAGE + " = " + itemDamage[DT_Risk_Damages.ID_DAMAGE]))
                {//y despues para  sumarlos todos en un mismo daño y encontrar el AD
                    if ((decimal)itemWBS[DT_WBS_RISK_DAMAGE.VALUE] >= 0)
                    {
                        valor += (decimal)itemWBS[DT_WBS_RISK_DAMAGE.VALUE];
                    }
                    else
                    {
                        MostrarErrorDialog("Value can't be negative!!!");
                        valor += (decimal)Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Find(new object[] { itemWBS[DT_WBS_RISK_DAMAGE.ID_WBS], itemWBS[DT_WBS_RISK_DAMAGE.ID_DAMAGE], itemWBS[DT_WBS_RISK_DAMAGE.ID_RISK] })[DT_WBS_RISK_DAMAGE.VALUE];
                    }
                }
                itemDamage[DT_Risk_Damages.VALUE] = valor;
            }
        }



        private void TextProbability_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!(Versioned.IsNumeric(TextProbability.Text)))
                {
                    MostrarErrorDialog(StringResources.NUMERIC_FIELD);
                }
                else
                {
                    if (decimal.Parse(TextProbability.Text) > 100)
                    {
                        MostrarErrorDialog(StringResources.PROBABILITY_FIELD);
                    }
                    else
                    {
                        if (decimal.Parse(TextProbability.Text) < 0)
                        {
                            MostrarErrorDialog(StringResources.PROBABILITY_FIELD);
                        }
                        else
                        {
                            //Probability = decimal.Parse(TextProbability.Text);
                            General.RecalculateProbability(RiskRow, Ds, Probability, false, dsWBS);
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
                if (RiskName.Text != "")
                {
                    if (MyRisks.Select(DT_Risk.ID_DIAGRAM + " = " + RiskTreeID + " and " + DT_Risk.NAMESHORT + " = '" + RiskName.Text + "' and " 
                        + DT_Risk.ID + " <> " + RiskRow[DT_Risk.ID]).Any() && RiskRow[DT_Risk.NAMESHORT].ToString() != RiskName.Text)
                    {
                        IS_USING_NAME = true;
                        MostrarDialogYesNo("The name [" + RiskName.Text + "] Already exists in this diagram. Do you want to use it again?");
                    }
                    else
                    {
                        AceptRisk();
                    }
                    foreach (DataRow item in Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                    {
                        item[DT_Risk_Damages.PROBABILITY] = RiskRow[DT_Risk.PROBABILITY];
                        item[DT_Risk_Damages.RISK_NAMESHORT] = RiskRow[DT_Risk.NAMESHORT].ToString();
                        item[DT_Risk_Damages.ID_WBS] = ID_WBS;
                        item[DT_Risk_Damages.WBS_NAME] = WBS_NAME;
                        item[DT_Risk_Damages.USERNAME] = USER_NAME;
                    }
                    if (Operation == General.INSERT)
                    {
                        RiskRow[DT_Risk.IDRISK_FATHER] = RowFather[DT_Risk.ID];
                        Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(RiskRow);
                        DataRow rowstructure = Ds.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                        rowstructure[DT_RiskStructure.IDRISK] = RiskRow[DT_Risk.ID];
                        rowstructure[DT_RiskStructure.IDRISK_FATHER] = RowFather[DT_Risk.ID];
                        Ds.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);
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
                MessageBox.Show(EX.Message);
            }
        }

        private void RefreshDamageValues(decimal idRiskCM, bool isCM)
        {
            foreach (DataRow itemDamage in Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + idRiskCM))
            {//primero recorro los Daños de los riesgos
                decimal valor = 0;
                foreach (DataRow itemWBS in Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + RiskRow[DT_Risk.ID] + " AND " + DT_WBS_RISK_DAMAGE.ID_DAMAGE + " = " + itemDamage[DT_Risk_Damages.ID_DAMAGE]))
                {//y despues para  sumarlos todos en un mismo daño y encontrar el AD
                    valor += (decimal)itemWBS[DT_WBS_RISK_DAMAGE.VALUE];
                }
                itemDamage[DT_Risk_Damages.VALUE] = valor;
            }
        }

        private void AceptRisk()
        {
            RiskRow[DT_Risk.NAMESHORT] = RiskName.Text;
            RiskRow[DT_Risk.ISCOLLAPSED] = false;
            RiskRow[DT_Risk.IS_ACTIVE] = Enabled;
            RiskRow[DT_Risk.COMMENTS] = TextDetail.Text;
            RiskRow[DT_Risk.ID_DIAGRAM] = RiskTreeID;
            RiskRow[DT_Risk.IS_ROOT] = false;
            RiskRow[DT_Risk.POSITION] = Posicion;
            RiskRow[DT_Risk.ID_WBS] = ID_WBS;
            RiskRow[DT_Risk.WBS_NAME] = WBS_NAME;
            RiskRow[DT_Risk.USER_NAME] = USER_NAME;
            RefreshDamageValues((decimal)RiskRow[DT_Risk.ID], false);
            if (Versioned.IsNumeric(TextProbability.Text))
            {
                if (Probability > 100)
                {
                    MostrarErrorDialog(StringResources.PROBABILITY_FIELD);
                }
                else
                {
                    bool flag = true;
                    foreach (DataRow item in Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK_TREE + " = " + RiskRow[DT_Risk.ID_DIAGRAM]))
                    {
                        if ((decimal)item[DT_Risk_Damages.VALUE] < 0)
                        {
                            flag = false;
                            MostrarErrorDialog("Value of damages cannot be negative");
                            break;
                        }
                    }
                    if (flag)
                    {
                        CalculateProbability();
                        RiskRow[DT_Risk.PROBABILITY] = Probability;
                        this.DialogResult = true;
                    }
                }
            }
            else
            {
                MostrarErrorDialog(StringResources.NUMERIC_FIELD);
            }
            IS_USING_NAME = false;
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
               
        private void RiskName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (RiskName.Text == "")
            {
                RiskName.ToolTip = "Empty";
            }
            if (Editando)
            {
                Editando = false;
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
                    AceptRisk();
                }
                if (IS_DELETING_ROW)
                {
                    Delete_Role(Selected_RoleRow);
                }
            }
        }


        private void KeyToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.Source != null && e.Source is Button theButton)
                {
                    if (theButton.DataContext != null && theButton.DataContext is DataRowView theDataRowView)
                    {
                        decimal idWBS = (decimal)theDataRowView.Row[DT_RISK_WBS.ID_WBS];

                        if (dgWBS.SelectedIndex >= 0)
                        {
                            DvRiskWBS[dgWBS.SelectedIndex].Row[DT_RISK_WBS.IS_PRIMARY] = true;
                            foreach (DataRow item in DvRiskWBS.Table.Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID] + " and " + DT_RISK_WBS.ID_WBS + " <> " + idWBS))
                            {
                                item[DT_RISK_WBS.IS_PRIMARY] = false;
                            }

                            ID_WBS = (decimal)DvRiskWBS[dgWBS.SelectedIndex].Row[DT_RISK_WBS.ID_WBS];
                            WBS_NAME = DvRiskWBS[dgWBS.SelectedIndex].Row[DT_RISK_WBS.NIVEL].ToString() + " " + DvRiskWBS[dgWBS.SelectedIndex].Row[DT_RISK_WBS.WBS].ToString();
                            USER_NAME = DvRiskWBS[dgWBS.SelectedIndex].Row[DT_RISK_WBS.USERNAME].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void CancelRisk()
        {
            if (Operation == General.INSERT)
            {
                foreach (DataRow rowRisk_Damage in Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                {
                    rowRisk_Damage.Delete();
                }
                foreach (DataRow rowRisk_wbs in Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                {
                    rowRisk_wbs.Delete();
                }
                foreach (DataRow rowRisk_WBS_Damage in Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                {
                    rowRisk_WBS_Damage.Delete();
                }
                foreach (DataRow rowRisk_Role in Ds.Tables[DT_Role_Risk.TABLE_NAME].Select(DT_Role_Risk.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                {
                    rowRisk_Role.Delete();
                }
            }                
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DialogResult == false || DialogResult == null)
            {
                CancelRisk();
            }
        }
    }
}
