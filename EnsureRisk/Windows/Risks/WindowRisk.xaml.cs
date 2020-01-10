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
    /// Interaction logic for WindowRisk.xaml
    /// </summary>
    public partial class WindowRisk : Window
    {
        public DataRow RiskRow { get; set; }
        public string Operation { get; set; }
        public int RiskTreeID { get; set; }
        public DataRow RowFather { get; set; }
        public List<RiskPolyLine> Lines { get; set; }
        public DataSet Ds { get; set; }
        public DataTable MyRisks { get; set; }
        public DataView DvTopRisk { get; set; }
        public DataView DvRiskWBS { get; set; }
        public DataView DvRoleRisk { get; set; }
        public DataTable TopRiskTable { get; set; }
        public DataTable Risk_WBS_Table { get; set; }
        public DataTable Risk_RoleTable { get; set; }
        public DataTable CM_RoleTable { get; set; }
        public RiskPolyLine RiskSelected { get; set; }
        public bool Enabled { get; set; }
        public int Posicion { get; set; }

        private bool Editando;
        private bool Seleccionando;

        public WindowRisk()
        {
            InitializeComponent();
            ChangeLanguage();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Operation == General.INSERT)
                {
                    Enabled = true;
                    if (RowFather != null)
                    {
                        TextFather.Text = RowFather[DT_Risk.NAMESHORT_COLUMNA].ToString();
                        RiskRow[DT_Risk.IDRISK_FATHER] = RowFather[DT_Risk.ID_COLUMNA];
                    }
                    foreach (DataRow item in Ds.Tables[DT_RiskTree_Damages.TABLENAME].Select(DT_RiskTree_Damages.ID_RISKTREE + " = " + RiskTreeID))
                    {
                        DataRow rowTop = TopRiskTable.NewRow();
                        rowTop[DT_Risk_Damages.COLOR] = item[DT_RiskTree_Damages.COLOR];
                        rowTop[DT_Risk_Damages.ID_DAMAGE] = item[DT_RiskTree_Damages.ID_DAMAGE];
                        rowTop[DT_Risk_Damages.ID_RISK] = RiskRow[DT_Risk.ID_COLUMNA];
                        rowTop[DT_Risk_Damages.ID_RISK_TREE] = RiskTreeID;
                        rowTop[DT_Risk_Damages.DAMAGE] = item[DT_RiskTree_Damages.DAMAGE].ToString() + "(" + item[DT_RiskTree_Damages.UM] + ")";
                        rowTop[DT_Risk_Damages.VALUE] = 0;
                        rowTop[DT_Risk_Damages.TOP_RISK] = item[DT_RiskTree_Damages.DAMAGE];

                        rowTop[DT_Risk_Damages.PROBABILITY] = 0;
                        rowTop[DT_Risk_Damages.GROUPE_NAME] ="None";
                        rowTop[DT_Risk_Damages.RISK_NAMESHORT] = "";
                        rowTop[DT_Risk_Damages.FATHER] = RowFather[DT_Risk.NAMESHORT_COLUMNA].ToString();

                        rowTop[DT_Risk_Damages.STATUS] = "Activated";
                        rowTop[DT_Risk_Damages.ID_FATHER] = RiskRow[DT_Risk.IDRISK_FATHER];
                        rowTop[DT_Risk_Damages.RISK_TREE] = Ds.Tables[DT_RiskTree.TABLE_NAME].Rows.Find(RiskTreeID)[DT_RiskTree.DIAGRAM_NAME].ToString();
                        rowTop[DT_Risk_Damages.IS_ROOT] = false;
                        TopRiskTable.Rows.Add(rowTop);
                    }
                    //GIVING FATHER´S ROLE TO CHILD
                    if (RiskSelected != null)
                    {
                        foreach (DataRow item in Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + RiskSelected.ID))
                        {
                            DataRow newRow = Risk_RoleTable.NewRow();
                            newRow[DT_Role_Risk.ID_RISK] = RiskRow[DT_Risk.ID_COLUMNA];
                            newRow[DT_Role_Risk.Role] = item[DT_Role_Risk.Role];
                            newRow[DT_Role_Risk.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                            Risk_RoleTable.Rows.Add(newRow);
                        }
                    }
                    if (RiskSelected != null)
                    {
                        foreach (DataRow item in Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskSelected.ID))
                        {
                            DataRow newRow = Risk_WBS_Table.NewRow();
                            newRow[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID_COLUMNA];
                            newRow[DT_RISK_WBS.ID_WBS] = item[DT_RISK_WBS.ID_WBS];
                            newRow[DT_RISK_WBS.NIVEL] = item[DT_RISK_WBS.NIVEL];
                            newRow[DT_RISK_WBS.WBS] = item[DT_RISK_WBS.WBS];
                            Risk_WBS_Table.Rows.Add(newRow);
                        }
                    }
                }
                if (Operation == General.UPDATE)
                {
                    Enabled = (Boolean)RiskRow[DT_Risk.ENABLED_COLUMN];
                    TextName.Text = RiskRow[DT_Risk.NAMESHORT_COLUMNA].ToString();
                    TextDetail.Text = RiskRow[DT_Risk.DETAIL_COLUMNA].ToString();
                    TextProbability.Text = RiskRow[DT_Risk.PROBABILITY_COLUMN].ToString();

                    foreach (DataRow item in Ds.Tables[DT_RiskTree_Damages.TABLENAME].Select(DT_RiskTree_Damages.ID_RISKTREE + " = " + RiskTreeID))
                    {
                        if (!(TopRiskTable.Rows.Contains(new object[] { RiskRow[DT_Risk.ID_COLUMNA], item[DT_RiskTree_Damages.ID_DAMAGE] })))
                        {
                            DataRow rowTop = TopRiskTable.NewRow();
                            rowTop[DT_Risk_Damages.COLOR] = item[DT_RiskTree_Damages.COLOR];
                            rowTop[DT_Risk_Damages.ID_DAMAGE] = item[DT_RiskTree_Damages.ID_DAMAGE];
                            rowTop[DT_Risk_Damages.ID_RISK] = RiskRow[DT_Risk.ID_COLUMNA];
                            rowTop[DT_Risk_Damages.ID_RISK_TREE] = RiskTreeID;
                            rowTop[DT_Risk_Damages.DAMAGE] = item[DT_RiskTree_Damages.DAMAGE].ToString() + "(" + item[DT_RiskTree_Damages.UM] + ")";
                            rowTop[DT_Risk_Damages.VALUE] = 0;
                            rowTop[DT_Risk_Damages.TOP_RISK] = item[DT_RiskTree_Damages.DAMAGE];

                            rowTop[DT_Risk_Damages.PROBABILITY] = decimal.Parse(TextProbability.Text);
                            rowTop[DT_Risk_Damages.GROUPE_NAME] = "None";
                            rowTop[DT_Risk_Damages.RISK_NAMESHORT] = TextName.Text;
                            rowTop[DT_Risk_Damages.FATHER] = RowFather[DT_Risk.NAMESHORT_COLUMNA].ToString();
                            rowTop[DT_Risk_Damages.STATUS] = (Boolean)RiskRow[DT_Risk.ENABLED_COLUMN] ? "Activated" : "No Activated";
                            rowTop[DT_Risk_Damages.ID_FATHER] = RiskRow[DT_Risk.IDRISK_FATHER];
                            rowTop[DT_Risk_Damages.RISK_TREE] = Ds.Tables[DT_RiskTree.TABLE_NAME].Rows.Find(RiskTreeID)[DT_RiskTree.DIAGRAM_NAME].ToString();
                            rowTop[DT_Risk_Damages.IS_ROOT] = false;
                            TopRiskTable.Rows.Add(rowTop);
                        }
                    }
                    TextFather.Text = RowFather[DT_Risk.NAMESHORT_COLUMNA].ToString();
                }
                TextName.Focus();
                DvRoleRisk = Risk_RoleTable.DefaultView;
                dgRoles.ItemsSource = DvRoleRisk;
                DvRoleRisk.RowFilter = DT_Role_Risk.ID_RISK + " = " + RiskRow[DT_Risk.ID_COLUMNA];

                DvRiskWBS = Risk_WBS_Table.DefaultView;
                dgWBS.ItemsSource = DvRiskWBS;
                DvRiskWBS.RowFilter = DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID_COLUMNA];

                DvTopRisk = TopRiskTable.DefaultView;
                dgTopRisk.ItemsSource = DvTopRisk;
                DvTopRisk.RowFilter = DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID_COLUMNA];
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog(); ;
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

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole();
                DataTable roleCodif = ws.GetRolesData().Tables[DT_Role.ROLE_TABLE].Copy();
                WindowSelection frmSelection = new WindowSelection();
                if (Risk_RoleTable.Select(DT_Role_Risk.ID_RISK + " = " + (Int32)RiskRow[DT_Risk.ID_COLUMNA]).Count() > 0)
                {
                    frmSelection.dt = General.DeleteExists(roleCodif, Risk_RoleTable.Select(DT_Role_Risk.ID_RISK + " = " + (Int32)RiskRow[DT_Risk.ID_COLUMNA]).CopyToDataTable(), DT_Role.IDROL_COLUMN);
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
                    foreach (DataRow itemRole in frmSelection.RowsSelected)
                    {
                        DataRow drRole = Risk_RoleTable.NewRow();
                        drRole[DT_Role_Risk.ID_RISK] = RiskRow[DT_Risk.ID_COLUMNA];
                        drRole[DT_Role_Risk.NAME_SHORT] = TextName.Text;
                        drRole[DT_Role_Risk.Role] = itemRole[DT_Role.ROLE_COLUM];
                        drRole[DT_Role_Risk.IDROL_COLUMN] = itemRole[DT_Role.IDROL_COLUMN];
                        Risk_RoleTable.Rows.Add(drRole);
                        if (Operation == General.UPDATE)
                        {
                            foreach (var item in Lines)
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

        private void BtnDel_Click(object sender, RoutedEventArgs e)
        {
            DataRow fila = DvRoleRisk[dgRoles.SelectedIndex].Row;
            if (fila[DT_Role_Risk.Role].ToString() != "Administrator")
            {
                if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + fila[DT_Role_Risk.Role].ToString() + "]?").ShowDialog() == true)
                {
                    if (Operation == General.UPDATE)
                    {
                        foreach (var item in Lines)
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
                DataTable roleCodif = ws.GetAllWBS().Tables[DT_WBS.TABLE_NAME].Copy();
                WindowSelection frmSelection = new WindowSelection();
                if (Risk_WBS_Table.Select(DT_RISK_WBS.ID_RISK + " = " + (Int32)RiskRow[DT_Risk.ID_COLUMNA]).Count() > 0)
                {
                    frmSelection.dt = General.DeleteExists(roleCodif, Risk_WBS_Table.Select(DT_RISK_WBS.ID_RISK + " = " + (Int32)RiskRow[DT_Risk.ID_COLUMNA]).CopyToDataTable(), DT_WBS.ID_WBS);
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
                        DataRow drRiskWBS = Risk_WBS_Table.NewRow();
                        drRiskWBS[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID_COLUMNA];
                        drRiskWBS[DT_RISK_WBS.RISK] = TextName.Text;
                        drRiskWBS[DT_RISK_WBS.WBS] = item[DT_WBS.WBS_NAME];
                        drRiskWBS[DT_RISK_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        drRiskWBS[DT_RISK_WBS.NIVEL] = item[DT_WBS.NIVEL];
                        Risk_WBS_Table.Rows.Add(drRiskWBS);
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
                DataRow fila = DvRiskWBS[dgWBS.SelectedIndex].Row;
                if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + fila[DT_RISK_WBS.WBS].ToString() + "]?").ShowDialog() == true)
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
                    if (MyRisks.Select(DT_Risk.ID_RISK_TREE + " = " + RiskTreeID + " and " 
                        + DT_Risk.NAMESHORT_COLUMNA + " = '" + TextName.Text + "' and " + DT_Risk.ID_COLUMNA + " <> " + RiskRow[DT_Risk.ID_COLUMNA]).Any())
                    {
                        WindowMessageYesNo yesNo = new WindowMessageYesNo("The name [" + TextName.Text  + "] Already exists in this diagram. Do you want to use it again?");
                        yesNo.ShowDialog();
                        if (yesNo.DialogResult == true)
                        {
                            AceptRisk();
                        }                        
                    }
                    else
                    {
                        AceptRisk();
                    }
                    foreach (DataRow item in TopRiskTable.Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID_COLUMNA]))
                    {
                        item[DT_Risk_Damages.PROBABILITY] = RiskRow[DT_Risk.PROBABILITY_COLUMN];
                        item[DT_Risk_Damages.RISK_NAMESHORT] = RiskRow[DT_Risk.NAMESHORT_COLUMNA].ToString();
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

        private void AceptRisk()
        {
            RiskRow[DT_Risk.NAMESHORT_COLUMNA] = TextName.Text;
            RiskRow[DT_Risk.ISCOLLAPSEDINGUI_COLUMNA] = false;
            RiskRow[DT_Risk.ENABLED_COLUMN] = Enabled;
            RiskRow[DT_Risk.DETAIL_COLUMNA] = TextDetail.Text;
            RiskRow[DT_Risk.ID_RISK_TREE] = RiskTreeID;
            RiskRow[DT_Risk.IS_ROOT_COLUMNA] = false;
            RiskRow[DT_Risk.POSITION_COLUMN] = Posicion;
            if (Versioned.IsNumeric(TextProbability.Text))
            {
                if (decimal.Parse(TextProbability.Text) > 100)
                {
                    new WindowMessageOK(StringResources.PROBABILITY_FIELD).ShowDialog();
                }
                else
                {
                    bool flag = true;
                    foreach (DataRow item in TopRiskTable.Select(DT_Risk_Damages.ID_RISK_TREE + " = " + RiskRow[DT_Risk.ID_RISK_TREE]))
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
                        RiskRow[DT_Risk.PROBABILITY_COLUMN] = decimal.Parse(TextProbability.Text) ;
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
    }
}
