using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DataMapping.Data;
using System.Data;
using EnsureRisk.Windows;
using EnsureBusinesss;
using System.IO;
using EnsureRisk.Resources;
using EnsureBusinesss.Business;
using System.Threading;
using System.Globalization;
using System.Printing;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Threading;
using System.ComponentModel;
using EnsureRisk.Classess;
using System.Collections.ObjectModel;
using EnsureRisk.DataBinding;
using System.Threading.Tasks;
using EnsureRisk.Export.Trader;
using EnsureRisk.Export;
using System.Runtime.InteropServices;
using Xceed.Wpf.AvalonDock.Layout;
using Xceed.Wpf.AvalonDock;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace EnsureRisk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region BindingStuff
        int idwbsfilter = -1;
        private string line_Selected = "None";
        private string type_selected = "None";
        private string _riskshortName = "";
        private string _cmshortName = "";
        private bool showCMData = false;
        private bool showRiskData = false;
        private DataView dvCBWBS;
        private DataView dv_CrossRisk;
        private DataView dv_Cross_CM;
        public DataView DvCBWBS { get { return dvCBWBS; } set { dvCBWBS = value; OnPropertyChanged("DvCBWBS"); } }
        private bool EditandoRisk;
        private bool SeleccionandoRisk;       
        private bool hasAccess;
        private decimal probability;
        private decimal riskreduction;
        private readonly string diagramName = "None";
        private MyLayoutDocumentt theCurrentLayout;
        public string RiskLineName { get { return _riskshortName; } set { _riskshortName = value; OnPropertyChanged("RiskLineName"); } }
        public string CMLineName { get { return _cmshortName; } set { _cmshortName = value; OnPropertyChanged("CMLineName"); } }
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
        public MyLayoutDocumentt TheCurrentLayout
        {
            get
            {
                return theCurrentLayout;
            }

            set
            {
                theCurrentLayout = value;
                OnPropertyChanged("TheCurrentLayout");
            }
        }


        public int IdWBSFilter
        {
            get { return idwbsfilter; }
            set
            {
                idwbsfilter = value;
                OnPropertyChanged("IdWBSFilter");
            }
        }

        public string DiagramName
        {
            get { return diagramName; }
            set
            {
                line_Selected = value;
                OnPropertyChanged("DiagramName");
            }
        }

        public string LSelected
        {
            get { return line_Selected; }
            set
            {
                line_Selected = value;
                OnPropertyChanged("LSelected");
            }
        }

        public string TSelected
        {
            get { return type_selected; }
            set
            {
                type_selected = value;
                OnPropertyChanged("TSelected");
            }
        }

        public bool ShowCMData
        {
            get { return showCMData; }
            set
            {
                showCMData = value;
                OnPropertyChanged("ShowCMData");
            }
        }

        public decimal RiskReduction
        {
            get { return riskreduction; }
            set
            {
                riskreduction = value;
                OnPropertyChanged("RiskReduction");
            }
        }

        public decimal Probability
        {
            get { return probability; }
            set
            {
                probability = value;
                OnPropertyChanged("Probability");
            }
        }

        public bool ShowRiskData
        {
            get { return showRiskData; }
            set
            {
                showRiskData = value;
                OnPropertyChanged("ShowRiskData");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        #endregion

        #region Attributes&Properties 
        public const string CLEAR = "pack://application:,,,/Images/DelMine.png";
        #region Pan
        private Point scrollStartPoint;
        private Point scrollStartOffset;
        public List<MyLayoutDocumentt> OpenedDocuments { get; set; }
        private readonly Cursor OpenHand = CursorHelper.FromByteArray(Properties.Resources.HandOpen);

        //private readonly Cursor DiagCross = CursorHelper.FromByteArray(Properties.Resources.DiagCross);

        private readonly Cursor GrabHand = CursorHelper.FromByteArray(Properties.Resources.HandGrabbing);
        #endregion
        
        #region Menus
        ContextMenu MenuRisk
        {
            get
            {
                return (ContextMenu)Resources["MenuRisk"];
            }
        }
        ContextMenu MenuRiskLimited
        {
            get
            {
                return (ContextMenu)Resources["MenuRiskLimited"];
            }
        }
        ContextMenu MenuMainRisk
        {
            get
            {
                return (ContextMenu)Resources["MenuMainRisk"];
            }
        }
        ContextMenu MenuCM
        {
            get
            {
                return (ContextMenu)Resources["MenuCM"];
            }
        }
        ContextMenu MenuGroupRisk
        {
            get
            {
                return (ContextMenu)Resources["MenuGroupRisk"];
            }
        }
        ContextMenu MenuGroupCM
        {
            get
            {
                return (ContextMenu)Resources["MenuGroupCM"];
            }
        }
        ContextMenu MenuGroupMixed
        {
            get
            {
                return (ContextMenu)Resources["MenuGroupMixed"];
            }
        }
        #endregion

        #region Lists       
        public List<int> AccessList { get; set; }
        public List<Node> Nodos = new List<Node>();
        //public List<RiskPolyLine> ListCopy { get; set; }
        public RiskPolyLine GlobalCopyLine { get; set; }
        #endregion     

        #region DataManagers
        public DataView DVRisk_Tree { get; set; }
        public DataTable Dt_Cross_Risk { get; set; }
        public DataTable Dt_Cross_CM { get; set; }
        public DataView DV_CrossRisk { get { return dv_CrossRisk; } set { dv_CrossRisk = value; OnPropertyChanged("DV_CrossRisk"); } }
        public DataView DV_Cross_CM { get { return dv_Cross_CM; } set { dv_Cross_CM = value; OnPropertyChanged("DV_Cross_CM"); } }
        public DataView DV_WBS { get; set; }
        public DataSet DsMain { get; set; }
        public DataSet DsWBS { get; set; }
        public DataSet DsGroupRisk { get; set; }
        public DataSet DsGroupCM { get; set; }
        public DataView DvRiskWBS { get; set; }
        public DataView DvRoleRisk { get; set; }
        public DataRow Selected_RiskRoleRow { get; set; }
        public DataRow Selected_Risk_WBS_Row { get; set; }
        public DataRow Selected_CM_WBS_Row { get; set; }
        public DataRow Selected_CM_Role_Row { get; set; }
        public DataView DvRoleCM { get; set; }
        public DataView DVCMWBS { get; set; }
        public DataView DvRiskDamages { get; set; }
        public DataView DvCM_Damages { get; set; }
        #endregion

        #region RiskDataTables
        public DataTable Risk_DamageTable { get; set; }
        public DataTable Risk_WBS_Table { get; set; }
        public DataTable WBS_RISK_Damage_Table { get; set; }
        public DataTable Risk_RoleTable { get; set; }

        public DataTable CM_Table { get; set; }
        public DataTable CM_DamageTable { get; set; }
        public DataTable CM_WBS_Table { get; set; }
        public DataTable WBS_CM_Damage_Table { get; set; }        
        public DataTable CM_RoleTable { get; set; }
        #endregion

        #region DecimalUIConverterParams
        static DecimalUIConverterParams decimalConfig = new DecimalUIConverterParams(Properties.Settings.Default.DecimalsStringFormat, Properties.Settings.Default.DecimalFractionalDigits);
        #endregion
        #region Another Properties & Atributes
        //public int IdWBS { get; set; }
        private bool Flag_login, isPanEnabled;
        private int Indexx = 0;
        public string LoginUser { get; set; }
        public int IdProject { get; set; }
        public const string CONNECTION = "ConnectionServer.xml";
        #endregion

        //private static Pen NODE_PEN = Pens.Gray;
        private const int NODE_HEIGHT = 90;
        private const int NODE_WIDTH = 150;
        private const int NODE_MARGIN_X = 90;
        private const int NODE_MARGIN_Y = 150;

        //private static Pen NODE_PEN = Pens.Gray;

        public List<Node> Data { get; set; }

        public TreeNodeModel<Node> Tree { get; set; }

        public ServiceRiskController.WebServiceRisk WSRisk { get; set; }

        #region RegionOfDialogYesNO
        #region Booleans_Controls
        public bool IS_REOPEN_DIAGRAM { get; private set; } = false;
        public bool IS_OPEN_DIAGRAM { get; private set; } = false;
        public bool IS_DELETING_DIAGRAM { get; set; } = false;
        public bool IS_DELETING_RISK { get; set; } = false;
        public bool IS_DELETING_CM { get; set; } = false;
        //public bool IS_IMPORTING_PROJECT { get; set; } = false;
        public bool IS_DELETING_GROUP_CM { get; set; } = false;
        public bool IS_DELETING_GROUP_TAB { get; set; } = false;
        public bool IS_REMOVING_GROUP_FILTER { get; set; } = false;
        public bool IS_REMOVING_GROUP { get; set; } = false;
        public bool IS_REMOVING_TABLE_ROW { get; set; } = false;
        public bool IS_SAVING_DATA { get; set; } = false;
        public bool IS_REPEATING_NAME { get; set; } = false;
        public bool IS_DELETING_WBS { get; private set; } = false;
        public bool IS_LOGIN { get; private set; } = false;
        public bool IS_DELETING_RISK_ROLE_ROW { get; private set; } = false;
        public bool IS_DELETING_Risk_WBS_ROW { get; private set; } = false;
        public bool IS_DELETING_CM_WBS { get; private set; } = false;
        public bool IS_DELETING_CM_ROLE { get; private set; } = false;
        #endregion

        public int DiagramID { get; private set; } = 0;
        public UserDataSet ImportDS { get; private set; }
        public DataRow DrImportRisk { get; private set; }
        public DataRow[] DrDannosImportados { get; private set; }
        public DataRow[] DrMisDannos { get; private set; }
        public int ID_Groupe { get; private set; }
        public DataRowView TheDataRowVieww { get; private set; }
        public DataTable TheRemovingTable { get; private set; }
        public DataRow DrWBStoDelete { get; private set; }
        public DataGrid Selected_DataGrid { get; private set; }
        #endregion
        #endregion

        public MainWindow()
        {
            try
            {
                UpdateServerAccess();
                if (File.Exists("Color.xml"))
                {
                    DataTable TableColors = new DataTable();
                    TableColors.ReadXml("Color.xml");
                    WindowOpening op = new WindowOpening();
                    op.ShowDialog();
                    TableColors.Dispose();
                }
                else
                {
                    WindowOpening op = new WindowOpening();
                    op.ShowDialog();
                }
                InitializeComponent();
                WrapCM.DataContext = this;
                WrapRisk.DataContext = this;
                TextRisk.DataContext = this;
                TextCM.DataContext = this;
                TextRiskDetail.DataContext = this;
                TextCMDetail.DataContext = this;
                TextProbability.DataContext = this;
                TextRReduction.DataContext = this;
                CbFilterWBSRisk.DataContext = this;
                CbFilterWBSCM.DataContext = this;
                dgRisksCross.DataContext = this;
                dgCrossCM.DataContext = this;
                WSRisk = new ServiceRiskController.WebServiceRisk();
                DsMain = new UserDataSet();
                AccessList = new List<int>();
                OpenedDocuments = new List<MyLayoutDocumentt>();
                Binding myBinding = new Binding
                {
                    Source = CbFilterWBSRisk,
                    Path = new PropertyPath("SelectedIndex"),
                    Mode = BindingMode.TwoWay,
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                };
                BindingOperations.SetBinding(CbFilterWBSCM, ComboBox.SelectedIndexProperty, myBinding);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }            
        }

        public void MostrarErrorDialog(string text)
        {
            ErrorDialogMessage.IsOpen = true;
            TextMessage.Text = text;
        }

        public void MostrarInfoDialog(string text)
        {
            InformationMessageDialog.IsOpen = true;
            TextInformationMessage.Text = text;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateServerAccess();
                ServiceProject.WebServiceProject webService = new ServiceProject.WebServiceProject();
                cbProjects.ItemsSource = webService.GetAllProjects().Tables[DT_Project.TABLE_NAME].DefaultView;
                webService.Dispose();
                cbProjects.SelectedValuePath = DT_Project.ID_PROJECT;
                cbProjects.DisplayMemberPath = DT_Project.PROJECT_NAME;
                cbProjects.SelectedIndex = 0;
                IdProject = (int)cbProjects.SelectedValue;
                RefreshWBS();
                DsMain = new UserDataSet();
                ServiceRiskController.WebServiceRisk risk = new ServiceRiskController.WebServiceRisk();
                DsMain.Merge(risk.GetRiskTreeString(new object[] { "%", -9999 }));
                risk.Dispose();
                //UPDATE THE TABLE OF TREES AND ITS RESPECTIVE RISK OF EACH ONE
                //DV ITS THE DATAVIEW OF TREE RISK
                DVRisk_Tree = DsMain.Tables[DT_Diagram.TABLE_NAME].DefaultView;
                dgTreeDiagrams.ItemsSource = DVRisk_Tree;
                dgTreeDiagrams.SelectedIndex = Indexx;

                ValidateAccess();
                //BtnBackward.Visibility = Visibility.Hidden;
                MenuRisk.Visibility = Visibility.Visible;
                MenuRiskLimited.Visibility = Visibility.Visible;
                MenuMainRisk.Visibility = Visibility.Visible;
                MenuCM.Visibility = Visibility.Visible;
                MenuGroupRisk.Visibility = Visibility.Visible;
                MenuGroupCM.Visibility = Visibility.Visible;
                MenuGroupMixed.Visibility = Visibility.Visible;
                if (File.Exists("Color.xml"))
                {
                    DataTable TableColors = new DataTable();
                    TableColors.ReadXml("Color.xml");
                    string Theme = TableColors.Select().First()["Theme"].ToString();
                    string Color = TableColors.Select().First()["Color"].ToString();
                    TableColors.Dispose();
                    Application.Current.Resources.MergedDictionaries[0].Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme." + Theme + ".xaml", UriKind.RelativeOrAbsolute);

                    Application.Current.Resources.MergedDictionaries[2].Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor." + Color + ".xaml", UriKind.RelativeOrAbsolute);

                }
                cbLanguage.ItemsSource = General.TableLanguages().DefaultView;
                cbLanguage.SelectedValuePath = DT_Language.CULTURE_COLUMN;
                cbLanguage.DisplayMemberPath = DT_Language.LANGUAJE_COLUMN;
                cbLanguage.SelectedIndex = 0;
                Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)cbLanguage.SelectedValue);
                Thread.CurrentThread.CurrentCulture = new CultureInfo((string)cbLanguage.SelectedValue);
                ChangeLanguage();
                CbProjects_DropDownClosed(sender, e);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DsMain = new UserDataSet();
                ServiceRiskController.WebServiceRisk risk = new ServiceRiskController.WebServiceRisk();
                DsMain.Merge(risk.GetRiskTreeString(new object[] { "%", -9999 }));
                //UPDATE THE TABLE OF TREES AND ITS RESPECTIVE RISK OF EACH ONE
                //DV ITS THE DATAVIEW OF TREE RISK
                //dvRisk its the dataview of Risk table
                risk.Dispose();
                DVRisk_Tree = DsMain.Tables[DT_Diagram.TABLE_NAME].DefaultView;
                dgTreeDiagrams.ItemsSource = DVRisk_Tree;
                dgTreeDiagrams.SelectedIndex = Indexx;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }

        }

        /// <summary>
        /// AutoFix the Grid Area acording with the Tree Diagram sizes.
        /// </summary>
        //private void CalculateGridSize()
        //{
        //    // tree sizes are 0-based, so add 1
        //    //var treeWidth = Tree.Width + 1;
        //    //var treeHeight = Tree.Height + 1;

        //    //GridPaintLines.Width = ((treeWidth * NODE_WIDTH) + ((treeWidth + 1) * NODE_MARGIN_X));
        //    //GridPaintLines.Height = (treeHeight * NODE_HEIGHT) + ((treeHeight + 1) * NODE_MARGIN_Y);
        //}

        #region MenuItem------------------------------------------

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AnchorDiagramList.IsVisible = true;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            AnchorRiskList.IsVisible = true;
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            AnchorCMList.IsVisible = true;
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            AnchorGroupRisk.IsVisible = true;
        }

        private void Properties_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LayoutProperties.IsVisible = true;
            LayoutProperties.IsSelected = true;
        }

        private void WBS_CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AnchorWBSList.IsVisible = true;
            AnchorWBSList.IsSelected = true;
            //if (true)//If has access to Damage List
            //{
            //    WindowWBSList wbs = new WindowWBSList
            //    {
            //        Icon = Icon
            //    };
            //    wbs.ShowDialog();
            //}            
        }

        private void OptionMiniMap_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AnchorMiniMap.IsVisible = true;
            AnchorMiniMap.IsSelected = true;
        }


        private void LoginCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                IS_LOGIN = true;
                LoginDialog.IsOpen = true;
                //LoginWindow lw = new LoginWindow
                //{
                //    Icon = Icon
                //};
                //if (lw.ShowDialog() == true)
                //{
                //    if (lw.Usser != "" || lw.Usser != null)
                //    {
                //        Title = "Ensure Risk | " + StringResources.CONNECTED_STRING + " " + lw.Usser;
                //        LoginMenuItem.Header = StringResources.LOGOFF;
                //        Flag_login = true;
                //        LoginUser = lw.Usser;
                //        AccessList = lw.AccessList;
                //        ValidateAccess();
                //        RefreshWBS();
                //        foreach (var item in OpenedDocuments)
                //        {
                //            item.LoginUser = LoginUser;
                //        }
                //    }
                //    else
                //    {
                //        Flag_login = false;
                //        LoginMenuItem.Header = StringResources.LoginMenu;
                //        Title = "Ensure Risk";
                //        LoginUser = "";
                //        AccessList = lw.AccessList;
                //        ValidateAccess();
                //    }
                //}
                //else
                //{
                //    Flag_login = false;
                //}
                //if (lw.DialogResult == false)
                //{
                //    Flag_login = false;
                //    Close();
                //}
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void ProjectCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ExistRole(7))//If has access to Project List
            {
                WindowProjectList list = new WindowProjectList
                {
                    Icon = Icon
                };
                list.ShowDialog();
                ServiceProject.WebServiceProject webService = new ServiceProject.WebServiceProject();

                cbProjects.ItemsSource = webService.GetAllProjects().Tables[DT_Project.TABLE_NAME].DefaultView;
                webService.Dispose();
                cbProjects.SelectedValuePath = DT_Project.ID_PROJECT;
                cbProjects.DisplayMemberPath = DT_Project.PROJECT_NAME;
                cbProjects.SelectedIndex = 0;
                IdProject = (int)cbProjects.SelectedValue;
            }
        }

        private void UserCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ExistRole(1))//If has access to User List
            {
                WindowUserList list = new WindowUserList
                {
                    Icon = Icon
                };
                list.ShowDialog();
            }
        }

        private void RoleCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ExistRole(2))//If has access to Role List
            {
                WindowRoleList wr = new WindowRoleList
                {
                    Icon = Icon
                };
                wr.ShowDialog();
            }
        }

        private void OptionCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            WindowOption option = new WindowOption() { Icon = Icon };
            option.ShowDialog();
        }

        private void DamageCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ExistRole(10))//If has access to Damage List
            {
                WindowTopRiskList windowTop = new WindowTopRiskList
                {
                    Icon = Icon
                };
                windowTop.ShowDialog();
            }
        }

        private void DefaultCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ExistRole(8))//If Has access to Default list
            {
                WindowDefaultRiskList windowDefault = new WindowDefaultRiskList
                {
                    Icon = Icon
                };
                windowDefault.ShowDialog();
            }
        }

        private void ExportCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (TheCurrentLayout != null)
            {
                if (TheCurrentLayout.LinesList.Count > 0)
                {
                    ExportToImage();
                    MostrarInfoDialog("Diagram exported successfully!");                    
                }
            }
        }

        private void PrintCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null)
                {
                    PrintDialog pd = new PrintDialog();
                    if (pd.ShowDialog() == true)
                    {
                        Rect printableArea = GetPrintableArea(pd);
                        Grid prueba = new Grid();
                        UIElement[] array = new UIElement[TheCurrentLayout.GridPaintLines.Children.Count];
                        TheCurrentLayout.GridPaintLines.Children.CopyTo(array, 0);
                        TheCurrentLayout.GridPaintLines.Children.Clear();
                        foreach (UIElement item in array)
                        {
                            prueba.Children.Add(item);
                        }
                        // I'm using here a viewbox for easily adjust the canvas_Letter to the desired size
                        Viewbox viewBox = new Viewbox { Child = prueba };
                        viewBox.Measure(printableArea.Size);
                        viewBox.Arrange(printableArea);
                        pd.PrintVisual(viewBox, StringResources.DiagramNameLabel);
                        prueba.Visibility = Visibility.Collapsed;
                        prueba.Children.Clear();
                        foreach (UIElement item in array)
                        {
                            TheCurrentLayout.GridPaintLines.Children.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }

        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void UpdateMiniMapSource()
        {
            try
            {
                if (TheCurrentLayout != null)
                {
                    MiniMap.MapSource = TheCurrentLayout.ScrollGridPaint;
                }
                else
                {
                    MiniMap.MapSource = new ScrollViewer();
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void MenuItemFish_Tree_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    if (CurrentLayout.LinesList.Count > 0)
            //    {
            //        if (DiagramAsTree)
            //        {
            //            DiagramAsTree = false;
            //            MenuItemFish_Tree.Content = FindResource("Tree");
            //            for (int i = 0; i < Nodos.Count; i++)
            //            {
            //                CurrentLayout.GridPaintLines.Children.Remove(Nodos[i].LineaArriba);
            //                CurrentLayout.GridPaintLines.Children.Remove(Nodos[i]);
            //                Nodos.RemoveAt(i);
            //                i--;
            //            }
            //            DropLines(LinesList);
            //            DropRectangles();
            //            LoadLines(Ds);
            //            LoadRectangles(CurrentLayout.ID_Diagram, MainLine);

            //            DrawNumbers(CurrentLayout.ID_Diagram, Ds, IdPrincipalLine, Rectangles);
            //            LineThickness(LinesList);
            //        }
            //        else
            //        {
            //            DiagramAsTree = true;
            //            MenuItemFish_Tree.Content = FindResource("Fish");
            //            _data = new List<Node>();
            //            System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_DAMAGE + " = " + (int)CurrentLayout.CbFilterTopR.SelectedValue).First()[DT_Risk_Damages.COLOR].ToString()));

            //            foreach (var item in LinesList)
            //            {
            //                string shortname = "";
            //                if (item.ShortName != null)
            //                {
            //                    shortname = item.ShortName;
            //                }
            //                else
            //                {
            //                    shortname = "Root";
            //                }
            //                var samp = new Node(CurrentLayout.GridPaintLines, new Point(), shortname, StringResources.ACUM_VALUE, StringResources.ACUM_DAMAGE, Color.FromArgb(drawColor.A, drawColor.R, drawColor.G, drawColor.B))
            //                {
            //                    ID = item.ID,
            //                    ParentId = item.IdRiskFather,
            //                    IsCM = item.IsCM
            //                };

            //                if (samp.IsCM)
            //                {
            //                    //samp.AcumD.Text += AcumDam;
            //                    //samp.ValAcum.Text += AcumValue;
            //                }
            //                else
            //                {
            //                    decimal el = General.AcumulatedLikelihood(item);
            //                    decimal valor = General.CalculateTopRiskTreeValue(Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID), Ds.Tables[DT_Risk.TABLE_NAME],
            //                            Ds.Tables[DT_RiskStructure.RiskStructure_TABLA], (int)cbFilterTopR.SelectedValue, Ds.Tables[DT_Risk_Damages.TABLENAME],
            //                            Ds.Tables[DT_CounterM.TABLE_NAME], Ds.Tables[DT_CounterM_Damage.TABLENAME]);
            //                    string AcumValue = General.MyRound(valor, 4).ToString();

            //                    string AcumDam = General.MyRound((valor * el), 4).ToString();
            //                    samp.AcumD.Text += AcumDam;
            //                    samp.ValAcum.Text += AcumValue;
            //                }

            //                _data.Add(samp);
            //            }
            //            _tree = Build_Tree(_data);
            //            TreeHelpers<Node>.CalculateNodePositions(_tree);

            //            if (Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_DAMAGE + " = " + (int)CurrentLayout.CbFilterTopR.SelectedValue).Any())
            //            {
            //                DrawNode(_tree, Color.FromArgb(drawColor.A, drawColor.R, drawColor.G, drawColor.B));
            //                CalculateGridSize();
            //            }
            //            DropLines(LinesList, false);
            //            DropRectangles();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MostrarErrorDialog(ex.Message);
            //}
        }

        private void MenuItemReload_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();            
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Ds.HasChanges())
                {
                    TheProgress.Visibility = Visibility.Visible;
                    HabilitarBotones(false);
                    Salvando();
                }
            }
            catch (Exception ex)
            {
                IS_SAVING_DATA = false;
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Salvando()
        {
            try
            {
                Cursor = Cursors.Wait;
                ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk();
                DataSet temp = TheCurrentLayout.Ds.GetChanges();
                temp = ws.SaveRisk(temp);
                TheCurrentLayout.Ds.Merge(temp);
                TheCurrentLayout.Ds.AcceptChanges();
                ws.Dispose();
                if (TheCurrentLayout.ID_Diagram < 0)
                {
                    TheCurrentLayout.ID_Diagram = (int)TheCurrentLayout.DrDiagram[DT_Diagram.ID_DIAGRAM];
                }
                RefreshData();
                TheCurrentLayout.Ds = DsMain.Copy();
                TheCurrentLayout.Ds.AcceptChanges();
                Cursor = Cursors.Arrow;
                if (TheCurrentLayout.ID_Diagram > 0)
                {
                    //CurrentLayout.Ds = DsMain;
                    TheCurrentLayout.DropLines();
                    TheCurrentLayout.DropRectangles();
                    TheCurrentLayout.LoadLines();
                    TheCurrentLayout.LoadRectangles();
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.SetLinesThickness();
                    TextProbabilityChange(TheCurrentLayout.MainLine);
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                    CruzarTablaCM(TheCurrentLayout.Ds);
                    MostrarInfoDialog("Data saved successfully!!!");
                }
                IS_SAVING_DATA = false;
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
            }
            catch (Exception ex)
            {
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                IS_SAVING_DATA = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        #region SavingAs
        private void SaveAsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SavingOpenedDiagram();
        }

        private void SavingOpenedDiagram()
        {
            try
            {
                if (TheCurrentLayout != null)
                {
                    TheProgress.Visibility = Visibility.Visible;
                    DataRow currentDiagram = TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(TheCurrentLayout.ID_Diagram);
                    WindowSaveAs wsas = new WindowSaveAs
                    {
                        DiagramName = currentDiagram[DT_Diagram.DIAGRAM_NAME].ToString()
                    };
                    if (wsas.ShowDialog() == true)
                    {
                        HabilitarBotones(false);
                        SalvandoAs(currentDiagram, wsas.DiagramName);
                    }
                }
            }
            catch (Exception ex)
            {
                IS_SAVING_DATA = false;
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                MostrarErrorDialog(ex.Message);
            }
        }

        private void CreandoDamagesDiagram(DataSet dsResult, DataRow drDiagram, DataSet dsSource, MyLayoutDocumentt originalLayout)
        {
            foreach (DataRow drDiagramDamage in dsSource.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + originalLayout.ID_Diagram))
            {
                DataRow newTreeDamage = dsResult.Tables[DT_Diagram_Damages.TABLENAME].NewRow();
                newTreeDamage[DT_Diagram_Damages.COLOR] = drDiagramDamage[DT_Diagram_Damages.COLOR];
                newTreeDamage[DT_Diagram_Damages.DAMAGE] = drDiagramDamage[DT_Diagram_Damages.DAMAGE];
                newTreeDamage[DT_Diagram_Damages.ID_DAMAGE] = drDiagramDamage[DT_Diagram_Damages.ID_DAMAGE];
                newTreeDamage[DT_Diagram_Damages.ID_RISKTREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
                newTreeDamage[DT_Diagram_Damages.RISK_TREE] = drDiagram[DT_Diagram.DIAGRAM_NAME];
                dsResult.Tables[DT_Diagram_Damages.TABLENAME].Rows.Add(newTreeDamage);
            }
        }

        private void CreandoRisks(MyLayoutDocumentt originalLayout, List<RiskPolyLine> listToFill)
        {
            foreach (DataRow item in originalLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + originalLayout.ID_Diagram))
            {
                int myPosition = item[DT_Risk.POSITION] == DBNull.Value ? 0 : (int)item[DT_Risk.POSITION];
                RiskPolyLine riskLine = new RiskPolyLine(new Grid(), MenuRisk, false)
                {
                    ShortName = item[DT_Risk.NAMESHORT].ToString(),
                    ID = (int)item[DT_Risk.ID],
                    Position = myPosition,
                    IsRoot = (bool)item[DT_Risk.IS_ROOT],
                    Collapsed = (bool)item[DT_Risk.ISCOLLAPSED],
                    Probability = (decimal)item[DT_Risk.PROBABILITY],
                    IsCM = false
                };
                if (originalLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains((int)item[DT_Risk.ID]))
                {
                    if (originalLayout.Ds.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + (int)item[DT_Risk.ID]).Any())
                    {
                        riskLine.IdRiskFather = (int)originalLayout.Ds.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + (int)item[DT_Risk.ID]).First()[DT_RiskStructure.IDRISK_FATHER];
                    }
                    else
                    {
                        riskLine.IdRiskFather = 0;
                    }
                }
                else { riskLine.IdRiskFather = 0; }
                listToFill.Add(riskLine);
            }
        }

        private void CreandoDamagesWBSAndRole_to_Risk(DataSet destinyDS, DataSet originalDS, DataRow drRisk, DataRow drDiagram, string diagramName, RiskPolyLine lineToCopy)
        {
            foreach (DataRow itemi in originalDS.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + lineToCopy.ID))
            {
                DataRow topR = destinyDS.Tables[DT_Risk_Damages.TABLENAME].NewRow();
                topR[DT_Risk_Damages.COLOR] = itemi[DT_Risk_Damages.COLOR];
                topR[DT_Risk_Damages.ID_DAMAGE] = itemi[DT_Risk_Damages.ID_DAMAGE];
                topR[DT_Risk_Damages.ID_RISK] = drRisk[DT_Risk.ID];
                topR[DT_Risk_Damages.VALUE] = itemi[DT_Risk_Damages.VALUE];
                topR[DT_Risk_Damages.TOP_RISK] = itemi[DT_Risk_Damages.TOP_RISK];
                topR[DT_Risk_Damages.DAMAGE] = itemi[DT_Risk_Damages.DAMAGE];
                topR[DT_Risk_Damages.FATHER] = "";
                topR[DT_Risk_Damages.GROUPE_NAME] = itemi[DT_Risk_Damages.GROUPE_NAME];
                topR[DT_Risk_Damages.ID_FATHER] = drRisk[DT_Risk.IDRISK_FATHER];
                topR[DT_Risk_Damages.ID_GROUPE] = itemi[DT_Risk_Damages.ID_GROUPE];
                topR[DT_Risk_Damages.ID_RISK_TREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
                topR[DT_Risk_Damages.IS_ROOT] = drRisk[DT_Risk.IS_ROOT];
                topR[DT_Risk_Damages.PROBABILITY] = drRisk[DT_Risk.PROBABILITY];
                topR[DT_Risk_Damages.RISK_NAMESHORT] = drRisk[DT_Risk.NAMESHORT];
                topR[DT_Risk_Damages.RISK_TREE] = diagramName;
                topR[DT_Risk_Damages.STATUS] = itemi[DT_Risk_Damages.STATUS];

                destinyDS.Tables[DT_Risk_Damages.TABLENAME].Rows.Add(topR);
            }

            foreach (DataRow item in originalDS.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + lineToCopy.ID))
            {
                if (!(destinyDS.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow newRow = destinyDS.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                    newRow[DT_Role_Risk.Role] = item[DT_Role_Risk.Role];
                    destinyDS.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                }
            }
            foreach (DataRow item in originalDS.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + lineToCopy.ID))
            {
                if (!(destinyDS.Tables[DT_RISK_WBS.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_RISK_WBS.ID_WBS] })))
                {
                    DataRow newRow = destinyDS.Tables[DT_RISK_WBS.TABLENAME].NewRow();
                    newRow[DT_RISK_WBS.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_RISK_WBS.ID_WBS] = item[DT_RISK_WBS.ID_WBS];
                    newRow[DT_RISK_WBS.WBS] = item[DT_RISK_WBS.WBS];
                    newRow[DT_RISK_WBS.NIVEL] = item[DT_RISK_WBS.NIVEL];
                    newRow[DT_RISK_WBS.USERNAME] = item[DT_RISK_WBS.USERNAME];
                    newRow[DT_RISK_WBS.PRIMARY] = item[DT_RISK_WBS.PRIMARY];
                    newRow[DT_RISK_WBS.IS_PRIMARY] = item[DT_RISK_WBS.IS_PRIMARY];
                    newRow[DT_RISK_WBS.PROBABILITY] = item[DT_RISK_WBS.PROBABILITY];
                    destinyDS.Tables[DT_RISK_WBS.TABLENAME].Rows.Add(newRow);
                }
            }
            foreach (DataRow item in originalDS.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + lineToCopy.ID))
            {
                DataRow newRow = destinyDS.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                newRow[DT_WBS_RISK_DAMAGE.ID_RISK] = drRisk[DT_Risk.ID];
                newRow[DT_WBS_RISK_DAMAGE.ID_WBS] = item[DT_WBS_RISK_DAMAGE.ID_WBS];
                newRow[DT_WBS_RISK_DAMAGE.WBS] = item[DT_WBS_RISK_DAMAGE.WBS];
                newRow[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = item[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                newRow[DT_WBS_RISK_DAMAGE.VALUE] = item[DT_WBS_RISK_DAMAGE.VALUE];
                newRow[DT_WBS_RISK_DAMAGE.DAMAGE] = item[DT_WBS_RISK_DAMAGE.DAMAGE];
                destinyDS.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(newRow);
            }
        }

        private void CreandoCMs(MyLayoutDocumentt originalLayout, List<RiskPolyLine> riskPolyLines)
        {
            foreach (DataRow item in originalLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK_TREE + " = " + originalLayout.ID_Diagram))
            {
                RiskPolyLine cmline = new RiskPolyLine(new Grid(), MenuCM, true)
                {
                    IsCM = true,
                    Position = (int)item[DT_CounterM.POSITION],
                    FromTop = false,
                    ShortName = item[DT_CounterM.NAMESHORT].ToString(),
                    IdRiskFather = (int)item[DT_CounterM.ID_RISK],
                    ID = (int)item[DT_CounterM.ID],
                    Probability = (decimal)item[DT_CounterM.PROBABILITY],
                    IsActivated = (bool)item[DT_CounterM.ENABLED]
                };
                riskPolyLines.Add(cmline);
            }
        }

        private void SalvandoAs(DataRow currentDiagram, string name)
        {
            try
            {
                DataSet tempDS = new UserDataSet();
                TheCurrentLayout.ListCopy = new List<RiskPolyLine>();
                //Creando el diagrama
                DataRow drDiagram = tempDS.Tables[DT_Diagram.TABLE_NAME].NewRow();
                drDiagram[DT_Diagram.ID_PROJECT] = currentDiagram[DT_Diagram.ID_PROJECT];
                drDiagram[DT_Diagram.DIAGRAM_NAME] = name;
                tempDS.Tables[DT_Diagram.TABLE_NAME].Rows.Add(drDiagram);

                CreandoDamagesDiagram(tempDS, drDiagram, TheCurrentLayout.Ds, TheCurrentLayout);

                CreandoRisks(TheCurrentLayout, TheCurrentLayout.ListCopy) ;

                CreandoCMs(TheCurrentLayout, TheCurrentLayout.ListCopy);

                TreeOperation.Build_Tree(TheCurrentLayout.ListCopy);
                TheCurrentLayout.CopyRisk = TheCurrentLayout.ListCopy.FirstOrDefault(p => p.IsRoot);
                DataRow drRisk = tempDS.Tables[DT_Risk.TABLE_NAME].NewRow();
                drRisk[DT_Risk.COMMENTS] = "Detail Total Risk";
                drRisk[DT_Risk.ENABLED] = true;
                drRisk[DT_Risk.ID_DIAGRAM] = drDiagram[DT_Diagram.ID_DIAGRAM];
                drRisk[DT_Risk.ISCOLLAPSED] = false;
                drRisk[DT_Risk.IS_ROOT] = true;
                drRisk[DT_Risk.NAMESHORT] = "Total Risk";
                drRisk[DT_Risk.POSITION] = 0;
                drRisk[DT_Risk.PROBABILITY] = 0;
                drRisk[DT_Risk.IDRISK_FATHER] = 0;
                tempDS.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);

                CreandoDamagesWBSAndRole_to_Risk(tempDS, TheCurrentLayout.Ds, drRisk, drDiagram, drDiagram[DT_Diagram.DIAGRAM_NAME].ToString(), TheCurrentLayout.CopyRisk);
                //--------------------------------------------------------------------------------------
                TheCurrentLayout.CopyRisk.ID = (int)drRisk[DT_Risk.ID];

                //Paste(TheCurrentLayout.CopyRisk, TheCurrentLayout.Ds, tempDS, (int)drDiagram[DT_Diagram.ID_DIAGRAM], TheCurrentLayout.ListCopy);//Aca pego el resto del diagrama
                Paste(TheCurrentLayout.CopyRisk, TheCurrentLayout.Ds, tempDS, (int)drDiagram[DT_Diagram.ID_DIAGRAM]);//Aca pego el resto del diagrama
                TheCurrentLayout.CopyRisk.IdRiskFather = 0;
                TheCurrentLayout.Copiando = false;
                if (tempDS.HasChanges())
                {
                    ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk();
                    DataSet temp = tempDS.GetChanges();
                    temp = ws.SaveRisk(temp);
                    tempDS.Merge(temp);
                    tempDS.AcceptChanges();
                    ws.Dispose();
                    RefreshData();
                    Cursor = Cursors.Arrow;
                    MostrarInfoDialog("Data saved successfully!!!");
                    OpenSavedDiagram(drDiagram, DsMain);                    
                }
                IS_SAVING_DATA = false;
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
            }
            catch (Exception ex)
            {
                IS_SAVING_DATA = false;
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                MostrarErrorDialog(ex.Message);
            }
        }

        public void OpenSavedDiagram(DataRow dataRow, DataSet dsToDiagram)
        {
            MyLayoutDocumentt myly = new MyLayoutDocumentt()
            {
                SaveAsClosing = true,
                MenuRisk = MenuRisk,
                MenuRiskLimited = MenuRiskLimited,
                MenuMainRisk = MenuMainRisk,
                Ds = dsToDiagram.Copy(),
                MenuCM = MenuCM,
                LoginUser = LoginUser,
                MyWindow = this,
                MenuGroupCM = MenuGroupCM,
                MenuGroupRisk = MenuGroupRisk,
                MenuGroupMixed = MenuGroupMixed
            };
            myly.DrDiagram = dataRow;
            myly.ID_Diagram = (int)dataRow[DT_Diagram.ID_DIAGRAM];
            myly.Title = dataRow[DT_Diagram.DIAGRAM_NAME].ToString();
            LayoutDocumentPanel.Children.Add(myly);
            OpenedDocuments.Add(myly);
            TheCurrentLayout = myly;
            CambiosVisuales();
        }

        #endregion

        #endregion

        #region DiagramControl

        /// <summary>
        /// Add a new diagram to App
        /// </summary>
        private void AddTree_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MyLayoutDocumentt myly = new MyLayoutDocumentt
                {
                    SaveAsClosing = true,
                    MenuRisk = MenuRisk,
                    MenuMainRisk = MenuMainRisk,
                    MenuRiskLimited = MenuRiskLimited,
                    Ds = DsMain.Copy(),
                    MenuCM = MenuCM,
                    LoginUser = LoginUser,
                    MyWindow = this,
                    MenuGroupCM = MenuGroupCM,
                    MenuGroupRisk = MenuGroupRisk,
                    MenuGroupMixed = MenuGroupMixed
                };
                WindowTreeRisk riskTree = new WindowTreeRisk
                {
                    Operation = General.INSERT,
                    TopRiskTable = myly.Ds.Tables[DT_Diagram_Damages.TABLENAME].Copy(),
                    DRow = myly.Ds.Tables[DT_Diagram.TABLE_NAME].NewRow(),
                    Icon = Icon,
                    IDProject = IdProject
                };
                if (riskTree.ShowDialog() == true)
                {
                    myly.DrDiagram = riskTree.DRow;
                    myly.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Add(myly.DrDiagram);
                    myly.Ds.Tables[DT_Diagram_Damages.TABLENAME].Merge(riskTree.TopRiskTable);

                    DataRow drRisk = myly.Ds.Tables[DT_Risk.TABLE_NAME].NewRow();

                    drRisk[DT_Risk.NAMESHORT] = "Total Risk";
                    drRisk[DT_Risk.COMMENTS] = "Total Risk " + riskTree.DRow[DT_Diagram.DIAGRAM_NAME];
                    drRisk[DT_Risk.IS_ROOT] = true;

                    drRisk[DT_Risk.ISCOLLAPSED] = false;
                    drRisk[DT_Risk.ENABLED] = true;
                    drRisk[DT_Risk.FROM_TOP] = false;
                    drRisk[DT_Risk.PROBABILITY] = 100;
                    drRisk[DT_Risk.ID_DIAGRAM] = riskTree.DRow[DT_Diagram.ID_DIAGRAM];
                    myly.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);

                    DataRow newRow = myly.Ds.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_Role_Risk.Role] = myly.Ds.Tables[DT_Role.ROLE_TABLE].Select(DT_Role.ROLE_COLUM + " = 'Administrator'").First()[DT_Role.ROLE_COLUM];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = myly.Ds.Tables[DT_Role.ROLE_TABLE].Select(DT_Role.ROLE_COLUM + " = 'Administrator'").First()[DT_Role.IDROL_COLUMN];
                    myly.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);

                    System.Drawing.Color color = System.Drawing.Color.FromArgb(int.Parse(myly.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + riskTree.DRow[DT_Diagram.ID_DIAGRAM]).First()[DT_Diagram_Damages.COLOR].ToString()));
                    myly.Title = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                    LayoutDocumentPanel.Children.Add(myly);
                    OpenedDocuments.Add(myly);

                    TheCurrentLayout = myly;
                    TheCurrentLayout.MoviendoRisk = false;
                    TheCurrentLayout.ID_Diagram = (int)riskTree.DRow[DT_Diagram.ID_DIAGRAM];
                    TheCurrentLayout.AddMainLine(drRisk, color);
                    TheCurrentLayout.LoadComboDamage();
                    TheCurrentLayout.LoadRectangles();
                    //TextProbability.Text = "0";
                    //TextDiagram.Text = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                    TheCurrentLayout.Title = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                    TheCurrentLayout.FixDrawPanel();
                    foreach (var item in OpenedDocuments)
                    {
                        item.ExitWorking();
                    }
                    TheCurrentLayout.EnterWorking();
                    UpdateMiniMapSource();
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public void MostrarDialogYesNo(string textAlert)
        {
            YesNoDialog.IsOpen = true;
            TextYesNoMessage.Text = textAlert;

        }

        /// <summary>
        /// Load an existing Diagram to change it
        /// </summary>        
        private void EditTree_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgTreeDiagrams.SelectedIndex >= 0)
                {
                    DiagramID = 0;
                    DiagramID = (int)DVRisk_Tree[Indexx].Row[DT_Diagram.ID_DIAGRAM];
                    MyLayoutDocumentt openDoc = OpenedDocuments.FirstOrDefault(docu => docu.ID_Diagram == (int)DVRisk_Tree[Indexx][DT_Diagram.ID_DIAGRAM]);

                    if (openDoc != null)
                    {
                        MostrarDialogYesNo("This diagram is already opened. Do you want to open it as New Diagram?");
                        IS_REOPEN_DIAGRAM = true;
                    }
                    else
                    {
                        MyLayoutDocumentt myly = new MyLayoutDocumentt
                        {
                            SaveAsClosing = true,
                            MenuRisk = MenuRisk,
                            MenuMainRisk = MenuMainRisk,
                            MenuRiskLimited = MenuRiskLimited,
                            Ds = DsMain.Copy(),
                            MenuCM = MenuCM,
                            LoginUser = LoginUser,
                            MyWindow = this,
                            MenuGroupCM = MenuGroupCM,
                            MenuGroupRisk = MenuGroupRisk,
                            MenuGroupMixed = MenuGroupMixed
                        };
                        WindowTreeRisk riskTree = new WindowTreeRisk
                        {
                            Operation = General.UPDATE,
                            TopRiskTable = myly.Ds.Tables[DT_Diagram_Damages.TABLENAME].Copy(),
                            DRow = myly.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(DVRisk_Tree[Indexx].Row[DT_Diagram.ID_DIAGRAM]),
                            Icon = Icon,
                            IDProject = IdProject,
                            CM_TopRisk = myly.Ds.Tables[DT_CounterM_Damage.TABLENAME].Copy(),
                            Risk_TopRisk = myly.Ds.Tables[DT_Risk_Damages.TABLENAME].Copy()
                        };
                        if (riskTree.ShowDialog() == true)
                        {
                            DVRisk_Tree[Indexx].Row[DT_Diagram.DIAGRAM_NAME] = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                            myly.Ds.Tables[DT_Diagram_Damages.TABLENAME].Merge(riskTree.TopRiskTable);
                            myly.Ds.Tables[DT_CounterM_Damage.TABLENAME].Merge(riskTree.CM_TopRisk);
                            myly.Ds.Tables[DT_Risk_Damages.TABLENAME].Merge(riskTree.Risk_TopRisk);
                            myly.ID_Diagram = DiagramID;
                            SetNewDamageToEntireTree(myly.ID_Diagram, myly.Ds);
                            TheProgress.Visibility = Visibility.Visible;
                            HabilitarBotones(false);
                            myly.Title = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                            LayoutDocumentPanel.Children.Add(myly);
                            OpenedDocuments.Add(myly);
                            TheCurrentLayout = myly;
                            CambiosVisuales();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                MostrarErrorDialog(ex.Message);
            }
        }

        public void ReOpenDiagram(int DiagramID)
        {
            try
            {
                MyLayoutDocumentt myly = new MyLayoutDocumentt
                {
                    SaveAsClosing = true,
                    MenuRisk = MenuRisk,
                    MenuMainRisk = MenuMainRisk,
                    MenuRiskLimited = MenuRiskLimited,
                    MenuCM = MenuCM,
                    Ds = DsMain.Copy(),
                    LoginUser = LoginUser,
                    MyWindow = this,
                    MenuGroupCM = MenuGroupCM,
                    MenuGroupRisk = MenuGroupRisk,
                    MenuGroupMixed = MenuGroupMixed
                };
                myly.ID_Diagram = CreateAndOpenNewDiagram(DiagramID, DVRisk_Tree[Indexx].Row[DT_Diagram.DIAGRAM_NAME].ToString(), myly);
                WindowTreeRisk riskTree = new WindowTreeRisk
                {
                    Operation = General.UPDATE,
                    TopRiskTable = myly.Ds.Tables[DT_Diagram_Damages.TABLENAME].Copy(),
                    DRow = myly.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(myly.ID_Diagram),
                    Icon = Icon,
                    IDProject = IdProject,
                    CM_TopRisk = myly.Ds.Tables[DT_CounterM_Damage.TABLENAME].Copy(),
                    Risk_TopRisk = myly.Ds.Tables[DT_Risk_Damages.TABLENAME].Copy()
                };
                if (riskTree.ShowDialog() == true)
                {
                    myly.Ds.Tables[DT_Diagram_Damages.TABLENAME].Merge(riskTree.TopRiskTable);
                    SetNewDamageToEntireTree(myly.ID_Diagram, myly.Ds);
                    //TextDiagram.Text = riskTree.TextName.Text;
                    TheProgress.Visibility = Visibility.Visible;
                    HabilitarBotones(false);
                    myly.Title = riskTree.TextName.Text;
                    LayoutDocumentPanel.Children.Add(myly);
                    OpenedDocuments.Add(myly);
                    TheCurrentLayout = myly;
                    CambiosVisuales();
                }
                IS_REOPEN_DIAGRAM = false;
            }
            catch (Exception ex)
            {
                IS_REOPEN_DIAGRAM = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        public int CreateAndOpenNewDiagram(int idDiagram, string diagramName, MyLayoutDocumentt destinyLayout)
        {
            MyLayoutDocumentt originalLayout = OpenedDocuments.FirstOrDefault(docu => docu.ID_Diagram == idDiagram);
            if (originalLayout != null)
            {
                //DataSet destinyLayout.Ds = new UserDataSet();
                destinyLayout.LinesList = new List<RiskPolyLine>();
                RiskPolyLine thecopiedline = originalLayout.LinesList.FirstOrDefault(p => p.IsRoot);
                //Creando el diagrama
                DataRow drDiagram = destinyLayout.Ds.Tables[DT_Diagram.TABLE_NAME].NewRow();
                drDiagram[DT_Diagram.ID_PROJECT] = destinyLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(idDiagram)[DT_Diagram.ID_PROJECT];
                drDiagram[DT_Diagram.DIAGRAM_NAME] = diagramName;
                destinyLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Add(drDiagram);
                destinyLayout.DrDiagram = drDiagram;
                CreandoDamagesDiagram(destinyLayout.Ds, drDiagram, originalLayout.Ds, originalLayout);

                CreandoRisks(originalLayout, destinyLayout.LinesList);

                CreandoCMs(originalLayout, destinyLayout.LinesList);               

                TreeOperation.Build_Tree(destinyLayout.LinesList);
                destinyLayout.CopyRisk = destinyLayout.LinesList.FirstOrDefault(p => p.IsRoot);

                DataRow drRisk = destinyLayout.Ds.Tables[DT_Risk.TABLE_NAME].NewRow();
                drRisk[DT_Risk.COMMENTS] = "Detail Total Risk";
                drRisk[DT_Risk.ENABLED] = true;
                drRisk[DT_Risk.ID_DIAGRAM] = drDiagram[DT_Diagram.ID_DIAGRAM];
                drRisk[DT_Risk.ISCOLLAPSED] = false;
                drRisk[DT_Risk.IS_ROOT] = true;
                drRisk[DT_Risk.NAMESHORT] = "Total Risk " + drDiagram[DT_Diagram.DIAGRAM_NAME];
                drRisk[DT_Risk.POSITION] = 0;
                drRisk[DT_Risk.PROBABILITY] = 0;
                drRisk[DT_Risk.IDRISK_FATHER] = 0;
                destinyLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);

                CreandoDamagesWBSAndRole_to_Risk(destinyLayout.Ds, originalLayout.Ds, drRisk, drDiagram, drDiagram[DT_Diagram.DIAGRAM_NAME].ToString(), thecopiedline);
                
                destinyLayout.CopyRisk.ID = (int)drRisk[DT_Risk.ID];

                //Paste(destinyLayout.CopyRisk, destinyLayout.Ds.Copy(), destinyLayout.Ds, (int)drDiagram[DT_Diagram.ID_DIAGRAM], destinyLayout.LinesList);//Aca pego el resto del diagrama
                Paste(destinyLayout.CopyRisk, destinyLayout.Ds.Copy(), destinyLayout.Ds, (int)drDiagram[DT_Diagram.ID_DIAGRAM]);
                destinyLayout.CopyRisk.IdRiskFather = 0;
                destinyLayout.GridPaintLines.Children.Remove(destinyLayout.CopyRisk);
                destinyLayout.GridPaintLines.Children.Remove(destinyLayout.CopyRisk.TextPanel);
                return (int)drDiagram[DT_Diagram.ID_DIAGRAM];                
            }
            else
            {
                return 0;
            }
        }

        public void SetNewDamageToEntireTree(int idDiagramaa, DataSet ds)
        {
            foreach (DataRow item in ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + idDiagramaa))
            {
                //Por cada riesgo del diagrama
                foreach (DataRow itemRisk in ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + idDiagramaa))
                {
                    if (!(ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { itemRisk[DT_Risk.ID], item[DT_Risk_Damages.ID_DAMAGE] })))
                    {
                        DataRow drDamage = ds.Tables[DT_Risk_Damages.TABLENAME].NewRow();
                        drDamage[DT_Risk_Damages.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                        drDamage[DT_Risk_Damages.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString();
                        drDamage[DT_Risk_Damages.VALUE] = 0;
                        drDamage[DT_Risk_Damages.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                        drDamage[DT_Risk_Damages.ID_RISK] = itemRisk[DT_Risk.ID];
                        drDamage[DT_Risk_Damages.ID_RISK_TREE] = itemRisk[DT_Risk.ID_DIAGRAM];
                        drDamage[DT_Risk_Damages.COLOR] = item[DT_Diagram_Damages.COLOR];
                        ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Add(drDamage);
                    }
                }
                foreach (DataRow itemCM in ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK_TREE + " = " + idDiagramaa))
                {
                    if (!(ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Contains(new object[] { itemCM[DT_CounterM.ID], item[DT_CounterM_Damage.ID_DAMAGE] })))
                    {
                        DataRow drDamage = ds.Tables[DT_CounterM_Damage.TABLENAME].NewRow();
                        drDamage[DT_CounterM_Damage.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                        drDamage[DT_CounterM_Damage.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString();
                        drDamage[DT_CounterM_Damage.VALUE] = 0;
                        drDamage[DT_CounterM_Damage.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                        drDamage[DT_CounterM_Damage.ID_COUNTERM] = itemCM[DT_CounterM.ID];
                        drDamage[DT_CounterM_Damage.ID_RISK_TREE] = itemCM[DT_CounterM.ID_RISK_TREE];
                        drDamage[DT_CounterM_Damage.COLOR] = item[DT_Diagram_Damages.COLOR];
                        ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Add(drDamage);
                    }
                }
            }

        }

        #region Progress Bar
        private void OnResponse()
        {
            TheCurrentLayout.TheProgressBar.Visibility = Visibility.Collapsed;
            //p.ProgressVisible = false;
            //p.ProgressIsIndeterminate = false;
        }

        private void CambiosVisuales()
        {
            try
            {
                TheCurrentLayout.LoadComboDamage();
                TheCurrentLayout.LoadLines();

                TheCurrentLayout.LoadRectangles();
                TheCurrentLayout.DrawNumbers();
                TheCurrentLayout.SetLinesThickness();
                TextProbabilityChange(TheCurrentLayout.MainLine);
                TheCurrentLayout.ScrollGridPaint.ScrollToRightEnd();
                TheCurrentLayout.ScrollGridPaint.ScrollToVerticalOffset(TheCurrentLayout.MainLine.Points[1].Y - 200);
                UpdateMiniMapSource();

                //BtnBackward.Visibility = Visibility.Hidden;
                TheCurrentLayout.BtnUndoneScope.Visibility = Visibility.Collapsed;

                CruzarTablaRisk(TheCurrentLayout.Ds);
                CruzarTablaCM(TheCurrentLayout.Ds);
                OnResponse();
                foreach (var item in OpenedDocuments)
                {
                    item.ExitWorking();
                }
                TheCurrentLayout.EnterWorking();
                TheProgress.Visibility = Visibility.Hidden;
                //TheCurrentLayout.MoveVisualToFishHead();
                HabilitarBotones(true);
            }
            catch (Exception ex)
            {
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                MostrarErrorDialog(ex.Message);
            }
        }
        #endregion

        private void DeleteDiagram()
        {
            try
            {
                int id = (int)DVRisk_Tree[dgTreeDiagrams.SelectedIndex].Row[DT_Diagram.ID_DIAGRAM];

                var ele = from layout in LayoutDocumentPanel.Children where ((MyLayoutDocumentt)layout).ID_Diagram == id select layout;
                if (ele.Count() > 0)
                {
                    ((MyLayoutDocumentt)ele.First()).SaveAsClosing = false;
                    ((MyLayoutDocumentt)ele.First()).Close();
                }
                //foreach (var item in ele)
                //{
                //    item.Close();
                //}
                //TextDiagram.Text = "None";
                //TextProbability.Text = "0";
                DVRisk_Tree[dgTreeDiagrams.SelectedIndex].Delete();
                SaveData(DsMain, true);
                IS_DELETING_DIAGRAM = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_DIAGRAM = false;
                MostrarErrorDialog(ex.Message);
            }
            
            //CruzarTablaRisk();
            //CruzarTablaCM();
        }
        /// <summary>
        /// Delete an existing Diagram
        /// </summary>
        private void RemoveTree_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgTreeDiagrams.SelectedIndex >= 0)
                {
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " the diagram [" + DVRisk_Tree[dgTreeDiagrams.SelectedIndex].Row[DT_Diagram.DIAGRAM_NAME] + "]?");
                    IS_DELETING_DIAGRAM = true;
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        #region ImportFromExcel
        /// <summary>
        /// Descontinuado pero no lo borro para hacer el otro asincronico
        /// </summary>
       
        private async Task FillDataTableToExcelAsync(DataSet dsImporting, bool isCustom)
        {
            using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog() { Filter = "Excel WorkBook|*.xlsx|Excel WorkBook 97-2003|*.xls", ValidateNames = true })
            {
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    WindowText wt = new WindowText
                    {
                        MyTitle = "Key Word for Disable"
                    };
                    wt.txtKeyword.Focus();
                    if (wt.ShowDialog() == true)
                    {
                        DataRow drDiagram = dsImporting.Tables[DT_Diagram.TABLE_NAME].NewRow();
                        using (DataTable dtExcel = ExcelToDataTable(ofd.FileName))
                        {                            
                            List<HeaderExcelContent> listaHeader = SetColumnas(dtExcel);
                            ServiceClasifications.WebServiceClasificator wsClasification = new ServiceClasifications.WebServiceClasificator();
                            DataSet dsClasification = wsClasification.GetAllClasifications().Copy();
                            wsClasification.Dispose();
                            WindowHeaderClasification whc = new WindowHeaderClasification
                            {
                                MyDataset = dsClasification.Copy(),
                                MyList = listaHeader
                            };
                            if (whc.ShowDialog() == true)
                            {
                                Cursor = Cursors.No;
                                TheProgress.Visibility = Visibility.Visible;
                                HabilitarBotones(false);
                                await Task.Run(() =>
                                {
                                    ServiceTopRiskController.WebServiceTopRisk wstop = new ServiceTopRiskController.WebServiceTopRisk();
                                    dsImporting.Tables[DT_Damage.TopRisk_TABLA].Merge(wstop.GetAllTopRisk().Tables[DT_Damage.TopRisk_TABLA]);
                                    wstop.Dispose();
                                    drDiagram[DT_Diagram.DIAGRAM_NAME] = "Imported Diagram at " + DateTime.Now;
                                    drDiagram[DT_Diagram.ID_DIAGRAM] = 0000;
                                    drDiagram[DT_Diagram.ID_PROJECT] = IdProject;
                                    dsImporting.Tables[DT_Diagram.TABLE_NAME].Rows.Add(drDiagram);
                                    IEnumerable<HeaderExcelContent> countDamages = whc.MyList.Where(x => x.IdClasification == 10);//Los dannos son ID 10
                                    int colorvariant = 1;
                                    foreach (var itemDamages in countDamages)
                                    {
                                        string DamageName = itemDamages.MyContent;
                                        if (!(dsImporting.Tables[DT_Damage.TopRisk_TABLA].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + DamageName + "'").Any()))//si el nombre del daño no existe 
                                        {
                                            DataRow drDamage = dsImporting.Tables[DT_Damage.TopRisk_TABLA].NewRow();//creo un nuevo daño
                                            drDamage[DT_Damage.TOP_RISK_COLUMN] = DamageName;
                                            int[] R = new int[] { 255, 220, 40, 80, 54, 144, 54, 144, 158 };
                                            int[] G = new int[] { 50, 10, 150, 200, 54, 54, 158, 158, 135 };
                                            int[] B = new int[] { 60, 150, 25, 99, 158, 158, 130, 54, 54 };
                                            if (colorvariant < 10)
                                            {
                                                System.Drawing.Color color = System.Drawing.Color.FromArgb(Convert.ToByte(R[colorvariant]), Convert.ToByte(G[colorvariant]), Convert.ToByte(B[colorvariant]));
                                                drDamage[DT_Damage.COLORID_COLUMNA] = color.ToArgb().ToString();
                                                colorvariant++;
                                            }
                                            else
                                            {
                                                System.Drawing.Color color = System.Drawing.Color.Blue;
                                                drDamage[DT_Damage.COLORID_COLUMNA] = color.ToArgb().ToString();
                                            }

                                            dsImporting.Tables[DT_Damage.TopRisk_TABLA].Rows.Add(drDamage);
                                            CreateDiagramDamagesExcel(dsImporting, drDamage, DamageName, drDiagram, true);
                                        }
                                        else
                                        {
                                            CreateDiagramDamagesExcel(dsImporting, null, DamageName, drDiagram, false);
                                        }
                                    }
                                    //BUsco el diagrama que acabo de insertar, para agregarle el riesgo padre, para agregarle los riesgos y sus dannos
                                    DataRow theDiagram = dsImporting.Tables[DT_Diagram.TABLE_NAME].Rows.Find(0000);
                                    //creo un riesgo root
                                    DataRow drRisk = dsImporting.Tables[DT_Risk.TABLE_NAME].NewRow();
                                    SetDataToMainRisk(drRisk, theDiagram);
                                    dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);

                                    //Asignarle al riesgo Root el rol admin
                                    AsignRoleAdminToRisk(dsImporting, drRisk);

                                    //por cada daño del diagrama
                                    DamagesToMainRisk(dsImporting, drRisk, theDiagram);

                                    //Recorrer el Excel solo para llenar los riesgos
                                    var xIdRisk = whc.MyList.FindLast(x => x.IdClasification == 1);//1 para el idRiesgo
                                    var xRiskShortName = whc.MyList.FindLast(x => x.IdClasification == 2);//1 para el idRiesgo
                                    var xRiskDetail = whc.MyList.FindLast(x => x.IdClasification == 3);//1 para el idRiesgo
                                    var xRiskEnabled = whc.MyList.FindLast(x => x.IdClasification == 4);//1 para el idRiesgo
                                    var xRiskProb = whc.MyList.FindLast(x => x.IdClasification == 11);//1 para el idRiesgo

                                    for (int i = 0; i < dtExcel.Rows.Count; i++)
                                    {
                                        DataRow drRiskN = dsImporting.Tables[DT_Risk.TABLE_NAME].NewRow();
                                        SetValuesToRiskInExcel(dtExcel, theDiagram, drRisk, i, drRiskN, wt.KeyWord, dsImporting, xIdRisk, xRiskShortName, xRiskDetail, xRiskEnabled, xRiskProb, countDamages, isCustom);
                                    }
                                    var xRiskFather = whc.MyList.FindLast(x => x.IdClasification == 5);//1 para el idRiesgo
                                    for (int i = 0; i < dtExcel.Rows.Count; i++)
                                    {//ajustando estructura
                                        SetRiskStructureInExcel(i, isCustom, dsImporting, dtExcel, xIdRisk, xRiskFather);
                                    }
                                    HeaderExcelContent xCmShort = whc.MyList.FindLast(x => x.IdClasification == 8);//1 para el idRiesgo
                                    var xCmDetail = whc.MyList.FindLast(x => x.IdClasification == 9);//1 para el idRiesgo
                                    var xCmReduction = whc.MyList.FindLast(x => x.IdClasification == 12);//1 para el idRiesgo
                                    var xCmActive = whc.MyList.FindLast(x => x.IdClasification == 14);//1 para el idRiesgo
                                                                                                      //var xCmActive = whc.MyList.FindLast(x => x.IdClasification == 9)
                                    for (int i = 0; i < dtExcel.Rows.Count; i++)
                                    {//agregando CM
                                        SetValuesToCMInExcel(dsImporting, wt.KeyWord, i, theDiagram, dtExcel, xCmShort, xCmDetail, xCmReduction, xIdRisk, xCmActive, countDamages, isCustom);
                                    }
                                });
                                this.Dispatcher.Invoke(() =>
                                {
                                    if (dsImporting.HasChanges())
                                    {
                                        //Cursor = Cursors.Wait;
                                        ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk();
                                        DataSet temp = dsImporting.GetChanges();
                                        temp = ws.SaveRisk(temp);
                                        dsImporting.Merge(temp);
                                        dsImporting.AcceptChanges();
                                        ws.Dispose();
                                        //textProgress.Text = "Ordering...";
                                        ServiceRiskController.WebServiceRisk tws = new ServiceRiskController.WebServiceRisk();
                                        UserDataSet dsT1 = new UserDataSet();
                                        dsT1.Merge(tws.GetRiskTreeID(new object[] { (int)drDiagram[DT_Diagram.ID_DIAGRAM] }));
                                        TreeOperation.AjustarPosicionHijosInExcel(TreeOperation.LoadLines(dsT1, (int)drDiagram[DT_Diagram.ID_DIAGRAM]).Find(x => x.IsRoot == true), dsImporting);
                                        DataSet tempi = dsImporting.GetChanges();
                                        //tempi = dsImporting.GetChanges();
                                        tempi = tws.SaveRisk(tempi);
                                        dsImporting.Merge(tempi);
                                        dsImporting.AcceptChanges();
                                        tws.Dispose();
                                        RefreshData();
                                        //p.ProgressVisible = false;
                                        TheProgress.Visibility = Visibility.Hidden;
                                        Cursor = Cursors.Arrow;
                                        MostrarInfoDialog("Importation file success!!");
                                        HabilitarBotones(true);
                                    }
                                });
                            }
                        }                        
                    }
                }
            }
        }

        /// <summary>
        /// Este es el metodo para Importar desde excel
        /// </summary>
        private async void ImportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowExcelClasification wxc = new WindowExcelClasification();
                if (wxc.ShowDialog() == true)
                {
                    await FillDataTableToExcelAsync(DsMain, wxc.Custom);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                Cursor = Cursors.Arrow;
            }
        }

        private static DataTable ExcelToDataTable(string filpath)
        {
            DataTable dt = new DataTable();
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(filpath, false))
            {
                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();

                foreach (Cell cell in rows.ElementAt(0))
                {
                    dt.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                }

                foreach (Row row in rows) //this will also include your header row...
                {
                    DataRow tempRow = dt.NewRow();

                    for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                    {
                        Cell cell = row.Descendants<Cell>().ElementAt(i);
                        int index = CellReferenceToIndex(cell);
                        tempRow[index] = GetCellValue(spreadSheetDocument, cell);
                    }

                    dt.Rows.Add(tempRow);
                }
            }
            dt.Rows.RemoveAt(0); //...so i'm taking it out here.
            return dt;
        }
        private static int CellReferenceToIndex(Cell cell)
        {
            int index = -1;
            string reference = cell.CellReference.ToString().ToUpper();
            foreach (char ch in reference)
            {
                if (Char.IsLetter(ch))
                {
                    int value = (int)ch - (int)'A';
                    index = (index + 1) * 26 + value;
                }
                else
                    return index;
            }
            return index;
        }

        public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value;
            if (cell.CellValue != null)
            {
                value = cell.CellValue.InnerXml;
            }
            else
            {
                return "";
            }

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[int.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }

        private List<HeaderExcelContent> SetColumnas(DataTable dtExcel)
        {
            List<HeaderExcelContent> listaHeader = new List<HeaderExcelContent>();
            int i = 0;
            foreach (DataColumn item in dtExcel.Columns)
            {
                listaHeader.Add(new HeaderExcelContent(item.ColumnName, i));
                i++;
            }
            return listaHeader;
        }
        
        private void SetValuesToCMInExcel(DataSet dsImporting, string EnableKeyWord, int rowPosition, DataRow theDiagram, DataTable dt, HeaderExcelContent xCmShort, HeaderExcelContent xCmDetail,
    HeaderExcelContent xCmReduction, HeaderExcelContent xIdRisk, HeaderExcelContent xCmActive, IEnumerable<HeaderExcelContent> countDamages, bool isCustom)
        {
            if (xCmShort != null && dt.Rows[rowPosition][xCmShort.MyContent.ToString()].ToString() != "")
            {
                DataRow drCM = dsImporting.Tables[DT_CounterM.TABLE_NAME].NewRow();
                drCM[DT_CounterM.NAMESHORT] = dt.Rows[rowPosition][xCmShort.MyContent.ToString()].ToString();
                drCM[DT_CounterM.ID_RISK_TREE] = theDiagram[DT_Diagram.ID_DIAGRAM];
                if (xCmDetail != null && dt.Rows[rowPosition][xCmDetail.MyContent.ToString()].ToString() != "" )
                {
                    drCM[DT_CounterM.DETAIL] = dt.Rows[rowPosition][xCmDetail.MyContent.ToString()].ToString();
                }

                drCM[DT_CounterM.ENABLED] = true;
                drCM[DT_CounterM.DIAGONAL] = false;
                drCM[DT_CounterM.FROM_TOP] = true;
                drCM[DT_CounterM.POSITION] = 0;
                if (xCmReduction != null && dt.Rows[rowPosition][xCmReduction.MyContent.ToString()].ToString() != "")
                {
                    drCM[DT_CounterM.PROBABILITY] = dt.Rows[rowPosition][xCmReduction.MyContent.ToString()].ToString();
                }
                else
                {
                    drCM[DT_CounterM.PROBABILITY] = 0;
                }
                if (xIdRisk != null && dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString() != "")
                {
                    if (isCustom)
                    {
                        drCM[DT_CounterM.ID_RISK] = Convert.ToInt32(dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString());
                    }
                    else
                    {
                        drCM[DT_CounterM.ID_RISK] = Convert.ToInt32(dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString().Split(new char[] { '-' })[1]);

                    }
                }
                else
                {
                    drCM[DT_CounterM.ID_RISK] = dsImporting.Tables[DT_CounterM.TABLE_NAME].Rows[dsImporting.Tables[DT_CounterM.TABLE_NAME].Rows.Count - 1][DT_CounterM.ID_RISK];
                }

                if (xCmActive != null && dt.Rows[rowPosition][xCmActive.MyContent.ToString()].ToString() != "")
                {
                    if (EnableKeyWord == dt.Rows[rowPosition][xCmActive.MyContent.ToString()].ToString())
                    {
                        drCM[DT_CounterM.ENABLED] = false;
                    }
                    else
                    {
                        drCM[DT_CounterM.ENABLED] = true;
                    }
                }
                else
                {
                    drCM[DT_CounterM.ENABLED] = true;
                }
                foreach (var itemDamages in countDamages.OrderBy(x => x.Column))
                {
                    string TopRisk = itemDamages.MyContent;
                    DamagesToCM(dsImporting, TopRisk, drCM, theDiagram);
                }
                AsignRoleToCM(dsImporting, drCM);
                dsImporting.Tables[DT_CounterM.TABLE_NAME].Rows.Add(drCM);
                AsignarWBSDefaultToCM(drCM, dsImporting);
            }

        }

        private void SetValuesToRiskInExcel(DataTable dt, DataRow theDiagram, DataRow drRisk, int rowPosition, DataRow drRiskN, string enableKeyWord, DataSet dsImporting, HeaderExcelContent xIdRisk,
    HeaderExcelContent xRiskShortName, HeaderExcelContent xRiskDetail, HeaderExcelContent xRiskEnabled, HeaderExcelContent xRiskProb, IEnumerable<HeaderExcelContent> countDamages, bool isCustom)
        {
            if (xIdRisk != null && dt.Rows[rowPosition][xIdRisk.MyContent].ToString() != "")
            {
                if (isCustom)
                {
                    drRiskN[DT_Risk.ID] = Convert.ToInt32(dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString());
                    if (xRiskShortName != null && dt.Rows[rowPosition][xRiskShortName.MyContent.ToString()].ToString() != "")
                    {
                        drRiskN[DT_Risk.NAMESHORT] = dt.Rows[rowPosition][xRiskShortName.MyContent.ToString()].ToString();
                    }
                    if (xRiskDetail != null && dt.Rows[rowPosition][xRiskDetail.MyContent.ToString()].ToString() != "" )
                    {
                        drRiskN[DT_Risk.COMMENTS] = dt.Rows[rowPosition][xRiskDetail.MyContent.ToString()].ToString();
                    }
                }
                else
                {
                    drRiskN[DT_Risk.ID] = Convert.ToInt32((dt.Rows[rowPosition][xIdRisk.MyContent.ToString()]).ToString().Split(new char[] { '-' })[1]);
                    if (dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString() != "")
                    {
                        drRiskN[DT_Risk.NAMESHORT] = dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString();
                    }
                    if (dt.Rows[rowPosition][xRiskShortName.MyContent.ToString()].ToString() != "")
                    {
                        drRiskN[DT_Risk.NAMESHORT] = drRiskN[DT_Risk.NAMESHORT] + " " + dt.Rows[rowPosition][xRiskShortName.MyContent.ToString()].ToString();
                    }
                    if (xRiskDetail != null && dt.Rows[rowPosition][xRiskDetail.MyContent.ToString()].ToString() != "")
                    {
                        drRiskN[DT_Risk.COMMENTS] = dt.Rows[rowPosition][xRiskDetail.MyContent.ToString()].ToString();
                    }
                    drRiskN[DT_Risk.ID_DIAGRAM] = theDiagram[DT_Diagram.ID_DIAGRAM];
                }
                if (xRiskEnabled != null && dt.Rows[rowPosition][xRiskEnabled.MyContent.ToString()].ToString() != "")
                {
                    if (enableKeyWord == dt.Rows[rowPosition][xRiskEnabled.MyContent.ToString()].ToString())
                    {
                        drRiskN[DT_Risk.ENABLED] = false;
                    }
                    else
                    {
                        drRiskN[DT_Risk.ENABLED] = true;
                    }
                }
                else
                {
                    drRiskN[DT_Risk.ENABLED] = true;
                }
                drRiskN[DT_Risk.IS_ROOT] = false;
                drRiskN[DT_Risk.ISCOLLAPSED] = false;
                drRiskN[DT_Risk.POSITION] = 0;
                drRiskN[DT_Risk.FROM_TOP] = false;
                drRiskN[DT_Risk.ID_DIAGRAM] = theDiagram[DT_Diagram.ID_DIAGRAM];

                if (xRiskProb != null && dt.Rows[rowPosition][xRiskProb.MyContent.ToString()].ToString() != "")
                {
                    drRiskN[DT_Risk.PROBABILITY] = Convert.ToDecimal(dt.Rows[rowPosition][xRiskProb.MyContent.ToString()]);
                }
                else
                {
                    drRiskN[DT_Risk.PROBABILITY] = 0;
                }

                drRiskN[DT_Risk.IDRISK_FATHER] = drRisk[DT_Risk.ID];
                DataRow drStructure = dsImporting.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                drStructure[DT_RiskStructure.IDRISK] = drRiskN[DT_Risk.ID];
                drStructure[DT_RiskStructure.IDRISK_FATHER] = drRisk[DT_Risk.ID];
                dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(drStructure);

                foreach (var itemDamages in countDamages.OrderBy(x => x.Column))
                {
                    string TopRisk = itemDamages.MyContent;
                    decimal value = 0;
                    if (itemDamages != null && dt.Rows[rowPosition][itemDamages.MyContent.ToString()].ToString() != "")
                    {
                        value = Convert.ToDecimal(dt.Rows[rowPosition][itemDamages.MyContent.ToString()]);
                    }
                    else
                    {
                        value = 0;
                    }
                    DamagesToRisk(dsImporting, TopRisk, drRiskN, value, theDiagram);
                }

                //por cada riesgo, le agrega el rol admin
                AsignRoleAdminToRisk(dsImporting, drRiskN);
                dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRiskN);
                AsignarWBSDefaultToRisk(drRiskN, dsImporting);
            }

        }

        private void SetRiskStructureInExcel(int rowPosition, bool isCustom, DataSet dsImporting, DataTable dt, HeaderExcelContent xIdRisk, HeaderExcelContent xRiskFather)
        {
            if (xRiskFather != null && dt.Rows[rowPosition][xRiskFather.MyContent.ToString()].ToString() != "" &&
                xIdRisk != null && dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString() != "")
            {
                int idHijo;
                int idPadre;
                if (isCustom)
                {
                    idHijo = Convert.ToInt32(dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString());
                    idPadre = Convert.ToInt32(dt.Rows[rowPosition][xRiskFather.MyContent.ToString()].ToString());
                }
                else
                {
                    //idPadre = Convert.ToInt32(Rango.Cells[rowPosition, xIdRisk.Column].Value2.ToString());
                    //idHijo = Convert.ToInt32(((object)Rango.Cells[rowPosition, xRiskFather.Column].Value2).ToString());
                    idPadre = Convert.ToInt32((dt.Rows[rowPosition][xIdRisk.MyContent.ToString()].ToString().Split(new char[] { '-' })[1]));
                    idHijo = Convert.ToInt32(dt.Rows[rowPosition][xRiskFather.MyContent.ToString()].ToString().Split(new char[] { '-' })[1]);
                }

                if (dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + idHijo).Any())
                {
                    dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + idHijo).First()[DT_RiskStructure.IDRISK_FATHER] = idPadre;
                }
            }
        }
        
        private void SetDataToMainRisk(DataRow drRisk, DataRow theDiagram)
        {
            drRisk[DT_Risk.ID] = 0000;
            drRisk[DT_Risk.NAMESHORT] = "Root " + theDiagram[DT_Diagram.DIAGRAM_NAME];
            drRisk[DT_Risk.COMMENTS] = "Total Risk " + theDiagram[DT_Diagram.DIAGRAM_NAME];
            drRisk[DT_Risk.IS_ROOT] = true;

            drRisk[DT_Risk.ISCOLLAPSED] = false;
            drRisk[DT_Risk.ENABLED] = true;
            drRisk[DT_Risk.FROM_TOP] = false;
            drRisk[DT_Risk.PROBABILITY] = 100;
            drRisk[DT_Risk.POSITION] = 0;
            drRisk[DT_Risk.ID_DIAGRAM] = theDiagram[DT_Diagram.ID_DIAGRAM];
        }

        private void CreateDiagramDamagesExcel(DataSet dsImporting, DataRow drDamage, string Damage, DataRow drDiagram, bool existDamage)
        {
            if (existDamage)
            {
                DataRow drDamage_Diagram = dsImporting.Tables[DT_Diagram_Damages.TABLENAME].NewRow();//y creo un nuevo diagrama con los daños que acabo de crear
                drDamage_Diagram[DT_Diagram_Damages.ID_DAMAGE] = drDamage[DT_Damage.ID_COLUMNA];
                drDamage_Diagram[DT_Diagram_Damages.COLOR] = drDamage[DT_Damage.COLORID_COLUMNA];
                drDamage_Diagram[DT_Diagram_Damages.RISK_TREE] = "Imported Diagram";
                drDamage_Diagram[DT_Diagram_Damages.ID_RISKTREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
                drDamage_Diagram[DT_Diagram_Damages.DAMAGE] = Damage;
                dsImporting.Tables[DT_Diagram_Damages.TABLENAME].Rows.Add(drDamage_Diagram);
            }
            else
            {
                DataRow drDamage_Diagram = dsImporting.Tables[DT_Diagram_Damages.TABLENAME].NewRow();//y creo un nuevo diagrama con los daños que acabo de crear
                drDamage_Diagram[DT_Diagram_Damages.ID_DAMAGE] = dsImporting.Tables[DT_Damage.TopRisk_TABLA].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + Damage + "'").First()[DT_Damage.ID_COLUMNA];
                drDamage_Diagram[DT_Diagram_Damages.COLOR] = dsImporting.Tables[DT_Damage.TopRisk_TABLA].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + Damage + "'").First()[DT_Damage.COLORID_COLUMNA];
                drDamage_Diagram[DT_Diagram_Damages.RISK_TREE] = "Imported Diagram";
                drDamage_Diagram[DT_Diagram_Damages.ID_RISKTREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
                drDamage_Diagram[DT_Diagram_Damages.DAMAGE] = Damage;
                dsImporting.Tables[DT_Diagram_Damages.TABLENAME].Rows.Add(drDamage_Diagram);
            }

        }

        private void AsignRoleAdminToRisk(DataSet dsImporting, DataRow drRisk)
        {
            if (!(dsImporting.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], 101 })))
            {
                DataRow drRiskRole = dsImporting.Tables[DT_Role_Risk.TABLENAME].NewRow();
                drRiskRole[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                drRiskRole[DT_Role_Risk.IDROL_COLUMN] = 101;
                dsImporting.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRiskRole);
            }
        }

        private void AsignRoleToCM(DataSet dsImporting, DataRow drCM)
        {
            if (!(dsImporting.Tables[DT_Role_CM.TABLENAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], 101 })))
            {
                DataRow drRoleCM = dsImporting.Tables[DT_Role_CM.TABLENAME].NewRow();
                drRoleCM[DT_Role_CM.ID_CM] = drCM[DT_CounterM.ID];
                drRoleCM[DT_Role_CM.IDROL_COLUMN] = 101;
                dsImporting.Tables[DT_Role_CM.TABLENAME].Rows.Add(drRoleCM);
            }
        }

        private void DamagesToMainRisk(DataSet dsImporting, DataRow drRisk, DataRow drDiagram)
        {
            foreach (DataRow itemDiagramDamage in dsImporting.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + drDiagram[DT_Diagram.ID_DIAGRAM]))
            {
                //darle valor 0, porque es la raiz
                DataRow drRiskDamage = dsImporting.Tables[DT_Risk_Damages.TABLENAME].NewRow();
                drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = itemDiagramDamage[DT_Diagram_Damages.ID_DAMAGE];
                drRiskDamage[DT_Risk_Damages.ID_RISK] = drRisk[DT_Risk.ID];
                drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
                drRiskDamage[DT_Risk_Damages.VALUE] = 0;
                dsImporting.Tables[DT_Risk_Damages.TABLENAME].Rows.Add(drRiskDamage);
            }
        }

        private void DamagesToCM(DataSet dsImporting, string Damage, DataRow drCM, DataRow drDiagram)
        {
            DataRow drCM_Damage = dsImporting.Tables[DT_CounterM_Damage.TABLENAME].NewRow();
            drCM_Damage[DT_CounterM_Damage.ID_DAMAGE] = dsImporting.Tables[DT_Damage.TopRisk_TABLA].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + Damage + "'").First()[DT_Damage.ID_COLUMNA];
            drCM_Damage[DT_CounterM_Damage.ID_COUNTERM] = drCM[DT_CounterM.ID];
            drCM_Damage[DT_CounterM_Damage.ID_RISK_TREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
            drCM_Damage[DT_CounterM_Damage.VALUE] = 0;
            dsImporting.Tables[DT_CounterM_Damage.TABLENAME].Rows.Add(drCM_Damage);
        }

        private void DamagesToRisk(DataSet dsImporting, string Damage, DataRow drRiskN, decimal value, DataRow drDiagram)
        {
            DataRow drRiskDamageN = dsImporting.Tables[DT_Risk_Damages.TABLENAME].NewRow();
            drRiskDamageN[DT_Risk_Damages.ID_DAMAGE] = dsImporting.Tables[DT_Damage.TopRisk_TABLA].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + Damage + "'").First()[DT_Damage.ID_COLUMNA];
            drRiskDamageN[DT_Risk_Damages.ID_RISK] = drRiskN[DT_Risk.ID];
            drRiskDamageN[DT_Risk_Damages.ID_RISK_TREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
            drRiskDamageN[DT_Risk_Damages.VALUE] = value;
            dsImporting.Tables[DT_Risk_Damages.TABLENAME].Rows.Add(drRiskDamageN);
        }

        private void AsignarWBSDefaultToRisk(DataRow drRisk, DataSet DsMain)
        {
            bool primary = true;
            foreach (DataRow item in DsWBS.Tables[DT_WBS.TABLE_NAME].Select())
            {
                if (!(DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + item[DT_WBS.ID_WBS]).Any()))
                {
                    if (!(DsMain.Tables[DT_RISK_WBS.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_WBS.ID_WBS] })))
                    {
                        DataRow drRiskWBS = DsMain.Tables[DT_RISK_WBS.TABLENAME].NewRow();
                        drRiskWBS[DT_RISK_WBS.ID_RISK] = drRisk[DT_Risk.ID];
                        drRiskWBS[DT_RISK_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = primary;
                        drRiskWBS[DT_RISK_WBS.PROBABILITY] = drRisk[DT_Risk.PROBABILITY];
                        primary = false;
                        DsMain.Tables[DT_RISK_WBS.TABLENAME].Rows.Add(drRiskWBS);
                    }
                }
            }
        }

        private void AsignarWBSDefaultToCM(DataRow drCM, DataSet DsMain)
        {
            bool primary = true;
            foreach (DataRow item in DsWBS.Tables[DT_WBS.TABLE_NAME].Rows)
            {
                if (!(DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + item[DT_WBS.ID_WBS]).Any()))
                {
                    if (!(DsMain.Tables[DT_CM_WBS.TABLENAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], item[DT_WBS.ID_WBS] })))
                    {
                        DataRow drRCMWBS = DsMain.Tables[DT_CM_WBS.TABLENAME].NewRow();
                        drRCMWBS[DT_CM_WBS.ID_CM] = drCM[DT_CounterM.ID];
                        drRCMWBS[DT_CM_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        drRCMWBS[DT_CM_WBS.IS_PRIMARY] = primary;
                        drRCMWBS[DT_CM_WBS.PROBABILITY] = drCM[DT_CounterM.PROBABILITY];
                        primary = false;
                        DsMain.Tables[DT_CM_WBS.TABLENAME].Rows.Add(drRCMWBS);
                    }
                }
            }
        }


        #endregion

        #region ExportarExcel
        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (OpenedDocuments.Count != 0 && TheCurrentLayout != null && TheCurrentLayout.ID_Diagram >= 0 && !TheCurrentLayout.IsExportingToExcel)
            {
                using (System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog() { Filter = "Excel WorkBook|*.xlsx|Excel WorkBook 97-2003|*.xls", ValidateNames = true })
                {
                    saveFileDialog.OverwritePrompt = false;
                    if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        string fileName = saveFileDialog.FileName;
                        if (File.Exists(fileName))
                        {
                            string targetFileName = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));

                            File.Copy(fileName, targetFileName + ".bak", true);
                            File.Delete(fileName);
                        }
                        TheCurrentLayout.ExportToExcel(fileName);
                    }
                }
            }
        }
        void ExportToExcelWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ExportRiskTree exportRiskTree = (ExportRiskTree)e.Argument;

            exportRiskTree.Export(sender as BackgroundWorker, e);
        }
        void ExportToExcelWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            TheCurrentLayout.TheProgressBar.Value = e.ProgressPercentage;
        }
        void ExportToExcelWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TheCurrentLayout.TheProgressBar.Value = 100;
            IFormatProvider formatProvider = CultureInfo.CurrentUICulture;
            MessageBox.Show(string.Format(formatProvider, "RiskTree {0} was saved as excel file!", TheCurrentLayout.Title));
            TheCurrentLayout.TheProgressBar.Visibility = Visibility.Collapsed;
            TheCurrentLayout.TheProgressBar.Value = 0;

            TheCurrentLayout.IsExportingToExcel = false;
        }
        #endregion


        #endregion

        #region MenuRiskClick
        /// <summary>
        /// Add a new Risk to Diagram
        /// </summary>                                     
        private void MenuAddRisk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //DsMain = CurrentLayout.Ds;
                WindowRisk wrisk = new WindowRisk()
                {
                    RiskRow = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].NewRow(),
                    Ds = TheCurrentLayout.Ds,
                    LOGIN_USER = LoginUser,
                    ID_PROJECT = (int)TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(TheCurrentLayout.ID_Diagram)[DT_Diagram.ID_PROJECT],
                    Risk_RoleTable = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Copy(),
                    WBS_RISK_Damage = TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Copy(),
                    WBS_CM_Damage = TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Copy(),
                    Risk_WBS_Table = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Copy(),
                    Risk_DamageTable = TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Copy(),
                    CM_DamageTable = TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Copy(),
                    Operation = General.INSERT,
                    RowFather = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID),
                    RiskTreeID = TheCurrentLayout.ID_Diagram,
                    RiskSelected = TheCurrentLayout.Line_Selected,
                    Icon = Icon,
                    MyRisks = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Copy()
                };
                wrisk.HasAccess = true;
                if (wrisk.ShowDialog() == true)
                {
                    TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(wrisk.RiskRow);
                    DataRow rowstructure = TheCurrentLayout.Ds.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                    rowstructure[DT_RiskStructure.IDRISK] = wrisk.RiskRow[DT_Risk.ID];
                    rowstructure[DT_RiskStructure.IDRISK_FATHER] = wrisk.RowFather[DT_Risk.ID];
                    TheCurrentLayout.Ds.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);
                    TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Merge(wrisk.Risk_RoleTable);
                    TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Merge(wrisk.WBS_RISK_Damage);
                    TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Merge(wrisk.WBS_CM_Damage);
                    TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Merge(wrisk.Risk_WBS_Table);
                    TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Merge(wrisk.Risk_DamageTable);
                    TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Merge(wrisk.CM_DamageTable);
                    RiskPolyLine Line_Created = new RiskPolyLine
                    {
                        ID = (int)wrisk.RiskRow[DT_Risk.ID],
                        IsCM = false,
                        ShortName = "LineCreated",
                        Father = TheCurrentLayout.Line_Selected,
                        IdRiskFather = TheCurrentLayout.Line_Selected.ID
                    };
                    TheCurrentLayout.InsertRisk(Line_Created, TheCurrentLayout.Line_Selected, TheCurrentLayout.PointSelected);

                    TheCurrentLayout.DropLines();
                    TheCurrentLayout.DropRectangles();
                    TheCurrentLayout.LoadLines();
                    TheCurrentLayout.LoadRectangles();
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.SetLinesThickness();
                    TextProbabilityChange(TheCurrentLayout.MainLine);
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void RiskCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null)
                {
                    TheCurrentLayout.GridPaintLines.Children.Remove(TheCurrentLayout.Line_Created);
                    System.Drawing.Color lnColor = System.Drawing.Color.FromArgb(int.Parse(TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram)[TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                    //HACER: comando add risk              
                    TheCurrentLayout.Line_Created = new RiskPolyLine(TheCurrentLayout.GridPaintLines, MenuRisk, false)
                    {
                        Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(lnColor.A, lnColor.R, lnColor.G, lnColor.B)),
                        StrokeThickness = 3
                    };
                    TheCurrentLayout.Line_Created.NewDrawAtPoint(new Point(TheCurrentLayout.X, TheCurrentLayout.Y), "");
                    TheCurrentLayout.Creando = true;
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// Edit a selected Risk in Diagram
        /// </summary>
        private void MenuEditRisk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowRisk formRisk = new WindowRisk
                {
                    RiskRow = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID),
                    Ds = TheCurrentLayout.Ds,
                    ID_PROJECT = (int)TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(TheCurrentLayout.ID_Diagram)[DT_Diagram.ID_PROJECT],
                    Operation = General.UPDATE,
                    LOGIN_USER = LoginUser,
                    Risk_DamageTable = TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Copy(),
                    CM_DamageTable = TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Copy(),
                    WBS_RISK_Damage = TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Copy(),
                    WBS_CM_Damage = TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Copy(),
                    Risk_WBS_Table = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Copy(),
                    CM_WBS_Table = TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Copy(),
                    Risk_RoleTable = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Copy(),
                    CM_RoleTable = TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Copy(),
                    Posicion = (int)TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_Risk.POSITION],
                    Icon = Icon,
                    ChildrenLines = TreeOperation.GetOnlyMyChildrenWithCM(TheCurrentLayout.Line_Selected),
                    MyRisks = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Copy(),
                };
                formRisk.HasAccess = TheCurrentLayout.FullAccess(TheCurrentLayout.Line_Selected);
                if (TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains(TheCurrentLayout.Line_Selected.ID))
                {
                    formRisk.RowFather = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.IdRiskFather);
                }
                formRisk.RiskTreeID = TheCurrentLayout.ID_Diagram;

                if (formRisk.ShowDialog() == true)
                {
                    if (TheCurrentLayout.Ds.HasChanges())
                    {
                        TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Merge(formRisk.Risk_DamageTable);
                        TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Merge(formRisk.CM_DamageTable);
                        TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Merge(formRisk.WBS_RISK_Damage);
                        TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Merge(formRisk.WBS_CM_Damage);
                        TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Merge(formRisk.Risk_WBS_Table);
                        TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Merge(formRisk.CM_WBS_Table);
                        TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Merge(formRisk.Risk_RoleTable);
                        TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Merge(formRisk.CM_RoleTable);
                        TreeOperation.SetRiskLineValues(TheCurrentLayout.Line_Selected, formRisk.RiskRow);
                        int pos = TheCurrentLayout.LinesList.FindIndex(rl => rl.ID == TheCurrentLayout.Line_Selected.ID);
                        TheCurrentLayout.LinesList[pos] = TheCurrentLayout.Line_Selected;
                        TextProbabilityChange(TheCurrentLayout.MainLine);
                        TheCurrentLayout.DrawNumbers();
                        TheCurrentLayout.UpdateLinesValues();
                        TheCurrentLayout.SetLinesThickness();
                        CruzarTablaRisk(TheCurrentLayout.Ds);
                        CruzarTablaCM(TheCurrentLayout.Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        #region TabEditingRisk
        public void UpdateRiskTabInformation()
        {
            if (TheCurrentLayout != null)
            {
                if (TheCurrentLayout.Line_Selected != null)
                {
                    Risk_DamageTable = TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Copy();
                    CM_DamageTable = TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Copy();
                    WBS_RISK_Damage_Table = TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Copy();
                    WBS_CM_Damage_Table = TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Copy();
                    Risk_WBS_Table = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Copy();
                    CM_WBS_Table = TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Copy();
                    Risk_RoleTable = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Copy();
                    CM_RoleTable = TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Copy();
                    HasAccess = TheCurrentLayout.FullAccess(TheCurrentLayout.Line_Selected);
                    DataRow RiskRow = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID);
                    RiskLineName = RiskRow[DT_Risk.NAMESHORT].ToString();
                    TextRiskDetail.Text = RiskRow[DT_Risk.COMMENTS].ToString();

                    DvRiskWBS = Risk_WBS_Table.DefaultView;
                    DvRiskWBS.RowFilter = DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID];
                    DgRiskWBS.ItemsSource = DvRiskWBS;

                    DvRoleRisk = Risk_RoleTable.DefaultView;
                    DvRoleRisk.RowFilter = DT_Role_Risk.ID_RISK + " = " + RiskRow[DT_Risk.ID];
                    DgRiskRoles.ItemsSource = DvRoleRisk;

                    DvRiskDamages = WBS_RISK_Damage_Table.DefaultView;

                    DvRiskDamages.RowFilter = DT_WBS_RISK_DAMAGE.ID_RISK + " = " + RiskRow[DT_Risk.ID];
                    if (DsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.USERNAME + " = '" + LoginUser + "'").Any())
                    {
                        string filter = "";
                        DataRow[] item = DsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.USERNAME + " = '" + LoginUser + "'");
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
                        DvRiskDamages.RowFilter += filter;
                    }
                    DgRiskDamages.ItemsSource = DvRiskDamages;
                    CalculateProbability(RiskRow);
                }
            }
        }

        private void CalculateProbability(DataRow RiskRow)
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
                            if ((decimal)item[DT_RISK_WBS.PROBABILITY] >= 0 && (decimal)item[DT_RISK_WBS.PROBABILITY] <= 100)
                            {
                                Probabilities.Add((decimal)item[DT_RISK_WBS.PROBABILITY]);
                            }
                            else
                            {
                                decimal originalValue = (decimal)TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Find(new object[] { item[DT_RISK_WBS.ID_RISK], item[DT_RISK_WBS.ID_WBS] })[DT_RISK_WBS.PROBABILITY];
                                Probabilities.Add(originalValue);
                                MostrarErrorDialog("Risk Probability Value can't be less than 0 or higher to 100. No changes apply");
                                item[DT_RISK_WBS.PROBABILITY] = originalValue;
                            }
                        }
                    }
                }
                if (Probabilities.Count > 0)
                {
                    Probability = Probabilities.Sum() / Probabilities.Count;
                }
                else
                {
                    Probability = 100;
                }
                RiskRow[DT_Risk.PROBABILITY] = Probability;
                foreach (DataRow item in Risk_DamageTable.Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                {
                    item[DT_Risk_Damages.PROBABILITY] = RiskRow[DT_Risk.PROBABILITY];
                }
                TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Merge(Risk_WBS_Table);
                TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Merge(Risk_DamageTable);
               
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void TextRisk_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    if (EditandoRisk)
                    {
                        if (!SeleccionandoRisk)
                        {
                            RiskresultStack.Visibility = Visibility.Collapsed;
                            EditandoRisk = false;
                        }
                    }
                    if (TextRisk.Text != string.Empty)
                    {
                        if (TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + DiagramID + " and " + DT_Risk.NAMESHORT + " = '" + TextRisk.Text + "' and " + DT_Risk.ID + " <> " + TheCurrentLayout.Line_Selected.ID).Any())
                        {
                            if (new WindowMessageYesNo("The name [" + TextRisk.Text + "] Already exists in this diagram. Do you want to use it again?").ShowDialog() == true)
                            {
                                TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_Risk.NAMESHORT] = TextRisk.Text;
                                foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + TheCurrentLayout.Line_Selected.ID))
                                {
                                    item[DT_Risk_Damages.RISK_NAMESHORT] = TextRisk.Text;
                                }
                                TreeOperation.SetRiskLineValues(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                                TheCurrentLayout.DrawNumbers();
                                TheCurrentLayout.UpdateLinesValues();
                                TheCurrentLayout.SetLinesThickness();
                                CruzarTablaRisk(TheCurrentLayout.Ds);
                            }
                            else
                            {
                                TextRisk.Text = TheCurrentLayout.Line_Selected.ShortName;
                            }
                        }
                        else
                        {
                            TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_Risk.NAMESHORT] = TextRisk.Text;
                            foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + TheCurrentLayout.Line_Selected.ID))
                            {
                                item[DT_Risk_Damages.RISK_NAMESHORT] = TextRisk.Text;
                            }
                            TreeOperation.SetRiskLineValues(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                            TheCurrentLayout.DrawNumbers();
                            TheCurrentLayout.UpdateLinesValues();
                            TheCurrentLayout.SetLinesThickness();
                            CruzarTablaRisk(TheCurrentLayout.Ds);
                        }
                    }
                    else
                    {
                        MostrarErrorDialog("Risk name can't be empty!");

                        UpdateRiskTabInformation();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void TextRiskDetail_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_Risk.COMMENTS] = TextRiskDetail.Text;
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        #region DgRiskWBS_Events
        private void DgRiskWBS_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    CalculateProbability(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TreeOperation.SetRiskLineValues(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.UpdateLinesValues();
                    TheCurrentLayout.SetLinesThickness();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DgRiskWBS_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    CalculateProbability(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TreeOperation.SetRiskLineValues(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.UpdateLinesValues();
                    TheCurrentLayout.SetLinesThickness();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DgRiskWBS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    CalculateProbability(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TreeOperation.SetRiskLineValues(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.UpdateLinesValues();
                    TheCurrentLayout.SetLinesThickness();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
        #endregion

        private void BtnAddWBSRisk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    DataRow RiskRow = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID);
                    int ID_PROJECT = (int)TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(TheCurrentLayout.ID_Diagram)[DT_Diagram.ID_PROJECT];
                    ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                    DataTable roleCodif = ws.GetAllWBSFiltered(new object[] { }).Tables[DT_WBS.TABLE_NAME].Copy();
                    ws.Dispose();
                    WindowSelection frmSelection = new WindowSelection();
                    if (TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + (int)RiskRow[DT_Risk.ID]).Count() > 0)
                    {
                        frmSelection.Dt = General.DeleteExists(roleCodif, TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + (int)RiskRow[DT_Risk.ID]).CopyToDataTable(), DT_WBS.ID_WBS);
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
                            DataRow drRiskWBS = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].NewRow();
                            drRiskWBS[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                            drRiskWBS[DT_RISK_WBS.RISK] = TextRisk.Text;
                            drRiskWBS[DT_RISK_WBS.WBS] = itemWBS[DT_WBS.WBS_NAME].ToString().TrimStart();
                            drRiskWBS[DT_RISK_WBS.ID_WBS] = itemWBS[DT_WBS.ID_WBS];
                            drRiskWBS[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                            drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = false;
                            drRiskWBS[DT_RISK_WBS.PRIMARY] = "";
                            drRiskWBS[DT_RISK_WBS.USERNAME] = itemWBS[DT_WBS.USERNAME];
                            drRiskWBS[DT_RISK_WBS.PROBABILITY] = 100;
                            TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Add(drRiskWBS);
                            foreach (DataRow itemAncestors in BuscarAncestros((int)itemWBS[DT_WBS.ID_WBS], DsWBS.Tables[DT_WBS.TABLE_NAME].Clone()).Rows)
                            {
                                if (!(TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Contains(new object[] { RiskRow[DT_Risk.ID], itemAncestors[DT_WBS.ID_WBS] })))
                                {
                                    DataRow drRiskWBSi = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].NewRow();
                                    drRiskWBSi[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                                    drRiskWBSi[DT_RISK_WBS.RISK] = TextRisk.Text;
                                    drRiskWBSi[DT_RISK_WBS.WBS] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart();
                                    drRiskWBSi[DT_RISK_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                    drRiskWBSi[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                    drRiskWBSi[DT_RISK_WBS.USERNAME] = itemAncestors[DT_WBS.USERNAME];
                                    drRiskWBSi[DT_RISK_WBS.IS_PRIMARY] = false;
                                    drRiskWBSi[DT_RISK_WBS.PRIMARY] = "";
                                    drRiskWBSi[DT_RISK_WBS.PROBABILITY] = 100;
                                    TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Add(drRiskWBSi);
                                }
                            }
                            foreach (var itemLines in TreeOperation.GetOnlyMyChildrenWithCM(TheCurrentLayout.Line_Selected))
                            {
                                if (itemLines.IsCM)
                                {
                                    if (!(TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Contains(new object[] { itemLines.ID, itemWBS[DT_WBS.ID_WBS] })))
                                    {
                                        DataRow drCMWBS = TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].NewRow();
                                        drCMWBS[DT_CM_WBS.ID_CM] = itemLines.ID;
                                        drCMWBS[DT_CM_WBS.CM] = itemLines.ShortName;
                                        drCMWBS[DT_CM_WBS.WBS] = itemWBS[DT_WBS.WBS_NAME].ToString().TrimStart();
                                        drCMWBS[DT_CM_WBS.ID_WBS] = itemWBS[DT_WBS.ID_WBS];
                                        drCMWBS[DT_CM_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                        drCMWBS[DT_CM_WBS.USERNAME] = itemWBS[DT_WBS.USERNAME];
                                        drCMWBS[DT_CM_WBS.IS_PRIMARY] = false;
                                        drCMWBS[DT_CM_WBS.PRIMARY] = "";
                                        TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Add(drCMWBS);
                                    }
                                    foreach (DataRow itemWBSCM in TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + itemLines.ID))
                                    {
                                        if (WBS_isSheet((int)itemWBSCM[DT_CM_WBS.ID_WBS]))
                                        {
                                            foreach (DataRow itemDamage in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + itemLines.ID))
                                            {
                                                if (!(TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Contains(new object[] { itemWBSCM[DT_CM_WBS.ID_WBS], itemDamage[DT_CounterM_Damage.ID_DAMAGE], itemLines.ID })))
                                                {
                                                    DataRow drWBS_CM_Damage = TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                                                    drWBS_CM_Damage[DT_WBS_CM_Damage.ID_CM] = itemLines.ID;
                                                    drWBS_CM_Damage[DT_WBS_CM_Damage.ID_WBS] = itemWBSCM[DT_CM_WBS.ID_WBS];
                                                    drWBS_CM_Damage[DT_WBS_CM_Damage.ID_DAMAGE] = itemDamage[DT_CounterM_Damage.ID_DAMAGE];
                                                    drWBS_CM_Damage[DT_WBS_CM_Damage.DAMAGE] = itemDamage[DT_CounterM_Damage.DAMAGE];
                                                    drWBS_CM_Damage[DT_WBS_CM_Damage.VALUE] = 0;
                                                    drWBS_CM_Damage[DT_WBS_CM_Damage.WBS] = itemWBSCM[DT_CM_WBS.WBS];
                                                    TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(drWBS_CM_Damage);
                                                }

                                            }
                                        }
                                    }
                                    //Buscando los ancestros para c/u de los wbs del cm
                                    foreach (DataRow itemCMWBSi in TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + itemLines.ID))
                                    {
                                        foreach (DataRow itemAncestors in BuscarAncestros((int)itemCMWBSi[DT_CM_WBS.ID_WBS], DsWBS.Tables[DT_WBS.TABLE_NAME].Clone()).Rows)
                                        {
                                            if (!(TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Contains(new object[] { itemLines.ID, itemAncestors[DT_WBS.ID_WBS] })))
                                            {
                                                DataRow drCMWBS = TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].NewRow();
                                                drCMWBS[DT_CM_WBS.ID_CM] = itemLines.ID;
                                                drCMWBS[DT_CM_WBS.CM] = itemLines.ShortName;
                                                drCMWBS[DT_CM_WBS.WBS] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart();
                                                drCMWBS[DT_CM_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                                drCMWBS[DT_CM_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                                drCMWBS[DT_CM_WBS.USERNAME] = itemAncestors[DT_WBS.USERNAME];
                                                drCMWBS[DT_CM_WBS.IS_PRIMARY] = false;
                                                drCMWBS[DT_CM_WBS.PRIMARY] = "";
                                                TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Add(drCMWBS);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (!(TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Contains(new object[] { itemLines.ID, itemWBS[DT_WBS.ID_WBS] })))
                                    {
                                        DataRow drRiskWBSS = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].NewRow();
                                        drRiskWBSS[DT_RISK_WBS.ID_RISK] = itemLines.ID;
                                        drRiskWBSS[DT_RISK_WBS.RISK] = itemLines.ShortName;
                                        drRiskWBSS[DT_RISK_WBS.WBS] = itemWBS[DT_WBS.WBS_NAME].ToString().TrimStart();
                                        drRiskWBSS[DT_RISK_WBS.ID_WBS] = itemWBS[DT_WBS.ID_WBS];
                                        drRiskWBSS[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                        drRiskWBSS[DT_RISK_WBS.USERNAME] = itemWBS[DT_WBS.USERNAME];
                                        drRiskWBSS[DT_RISK_WBS.IS_PRIMARY] = false;
                                        drRiskWBSS[DT_RISK_WBS.PRIMARY] = "";
                                        drRiskWBSS[DT_RISK_WBS.PROBABILITY] = 100;
                                        TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Add(drRiskWBSS);
                                    }
                                    foreach (DataRow itemWBSRisk in TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + itemLines.ID))
                                    {
                                        if (WBS_isSheet((int)itemWBSRisk[DT_RISK_WBS.ID_WBS]))
                                        {
                                            foreach (DataRow itemDamage in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + itemLines.ID))
                                            {
                                                if (!(TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { itemWBSRisk[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], itemLines.ID })))
                                                {
                                                    DataRow drWBS_RISK_Damage = TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_RISK] = itemLines.ID;
                                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_WBS] = itemWBSRisk[DT_RISK_WBS.ID_WBS];
                                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = itemDamage[DT_Risk_Damages.ID_DAMAGE];
                                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.DAMAGE] = itemDamage[DT_Risk_Damages.DAMAGE];
                                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                                                    drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.WBS] = itemWBSRisk[DT_RISK_WBS.WBS];
                                                    TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(drWBS_RISK_Damage);
                                                }

                                            }
                                        }
                                    }
                                    foreach (DataRow itemRISKWBSi in TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + itemLines.ID))
                                    {
                                        foreach (DataRow itemAncestors in BuscarAncestros((int)itemRISKWBSi[DT_RISK_WBS.ID_WBS], DsWBS.Tables[DT_WBS.TABLE_NAME].Clone()).Rows)
                                        {
                                            if (!(TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Contains(new object[] { itemLines.ID, itemAncestors[DT_WBS.ID_WBS] })))
                                            {
                                                DataRow drRiskWBSi = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].NewRow();
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

                        foreach (DataRow itemWBS in TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                        {
                            if (WBS_isSheet((int)itemWBS[DT_RISK_WBS.ID_WBS]))
                            {
                                foreach (DataRow itemDamage in Risk_DamageTable.Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                                {
                                    if (!(TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { itemWBS[DT_RISK_WBS.ID_WBS], itemDamage[DT_Risk_Damages.ID_DAMAGE], RiskRow[DT_Risk.ID] })))
                                    {
                                        DataRow drWBS_RISK_Damage = TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                                        drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_RISK] = RiskRow[DT_Risk.ID];
                                        drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_WBS] = itemWBS[DT_RISK_WBS.ID_WBS];
                                        drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = itemDamage[DT_Risk_Damages.ID_DAMAGE];
                                        drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.DAMAGE] = itemDamage[DT_Risk_Damages.DAMAGE];
                                        drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                                        drWBS_RISK_Damage[DT_WBS_RISK_DAMAGE.WBS] = itemWBS[DT_RISK_WBS.WBS];
                                        TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(drWBS_RISK_Damage);
                                    }
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

        private bool TengoPadre(int idWBS)
        {

            return DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + idWBS).Any();
        }
        private DataRow BuscarMiPadre(int idWBS)
        {
            int idPadre = (int)DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + idWBS).First()[DT_WBS_STRUCTURE.ID_FATHER];
            return DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(idPadre);
        }

        private void BtnDelWBSRisk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Selected_Risk_WBS_Row = DvRiskWBS[DgRiskWBS.SelectedIndex].Row;
                IS_DELETING_Risk_WBS_ROW = true;
                MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + Selected_Risk_WBS_Row[DT_RISK_WBS.WBS].ToString() + "]?");
            }
            catch (Exception ex)
            {
                IS_DELETING_Risk_WBS_ROW = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Delete_RISK_WBS_Row(DataRow fila)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    foreach (var itemLines in TreeOperation.GetOnlyMyChildrenWithCM(TheCurrentLayout.Line_Selected))
                    {
                        if (itemLines.IsCM)
                        {//AQUI PROBLEMAS CON EL ROWSTATE
                            foreach (DataRow itemC in TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Select(DT_WBS_CM_Damage.ID_CM + " = " + itemLines.ID + " AND " + DT_WBS_CM_Damage.ID_WBS + " = " + fila[DT_WBS.ID_WBS]))
                            {
                                itemC.Delete();
                            }
                            if (TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Contains(new object[] { itemLines.ID, fila[DT_WBS.ID_WBS] }))
                            {
                                TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Find(new object[] { itemLines.ID, fila[DT_WBS.ID_WBS] }).Delete();
                            }
                        }
                        else
                        {
                            foreach (DataRow itemC in TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + itemLines.ID + " AND " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + fila[DT_WBS.ID_WBS]))
                            {
                                itemC.Delete();
                            }
                            if (TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Contains(new object[] { itemLines.ID, fila[DT_WBS.ID_WBS] }))
                            {
                                TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Find(new object[] { itemLines.ID, fila[DT_WBS.ID_WBS] }).Delete();
                            }
                        }
                    }
                    foreach (DataRow itemC in TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + TheCurrentLayout.Line_Selected.ID + " AND " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + fila[DT_WBS.ID_WBS]))
                    {
                        itemC.Delete();
                    }
                    fila.Delete();
                    foreach (var item in TreeOperation.GetOnlyMyChildrenWithCM(TheCurrentLayout.Line_Selected))
                    {
                        RefreshDamageValues(item.ID, item.IsCM);
                    }
                    IS_DELETING_Risk_WBS_ROW = false;
                }
            }
            catch (Exception ex)
            {
                IS_DELETING_Risk_WBS_ROW = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void RefreshDamageValues(int id, bool isCM)
        {
            if (isCM)
            {
                foreach (DataRow itemDamage in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + id))
                {//primero recorro los Daños de los riesgos
                    decimal valor = 0;
                    foreach (DataRow itemWBS in TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Select(DT_WBS_CM_Damage.ID_CM + " = " + id + " AND " + DT_WBS_CM_Damage.ID_DAMAGE + " = " + itemDamage[DT_CounterM_Damage.ID_DAMAGE]))
                    {//y despues para  sumarlos todos en un mismo daño y encontrar el AD
                        valor += (decimal)itemWBS[DT_WBS_CM_Damage.VALUE];
                    }
                    itemDamage[DT_CounterM_Damage.VALUE] = valor;
                }
            }
            else
            {
                foreach (DataRow itemDamage in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + id))
                {//primero recorro los Daños de los riesgos
                    decimal valor = 0;
                    foreach (DataRow itemWBS in TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + TheCurrentLayout.Line_Selected.ID + " AND " + DT_WBS_RISK_DAMAGE.ID_DAMAGE + " = " + itemDamage[DT_Risk_Damages.ID_DAMAGE]))
                    {//y despues para  sumarlos todos en un mismo daño y encontrar el AD
                        valor += (decimal)itemWBS[DT_WBS_RISK_DAMAGE.VALUE];
                    }
                    itemDamage[DT_Risk_Damages.VALUE] = valor;
                }
            }
        }

        private void BtnSetPrimary_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DgRiskWBS.SelectedIndex >= 0)
                {
                    if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                    {
                        foreach (DataRow item in DvRiskWBS.Table.Select(DT_RISK_WBS.ID_RISK + " = " + TheCurrentLayout.Line_Selected.ID))
                        {
                            item[DT_RISK_WBS.IS_PRIMARY] = false;
                            item[DT_RISK_WBS.PRIMARY] = "";
                        }
                        DvRiskWBS[DgRiskWBS.SelectedIndex].Row[DT_RISK_WBS.IS_PRIMARY] = true;
                        DvRiskWBS[DgRiskWBS.SelectedIndex].Row[DT_RISK_WBS.PRIMARY] = "PRIMARY";
                        TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Merge(DvRiskWBS.Table);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }       

        private void RefreshRiskDamageValues(DataRow RiskRow)
        {
            foreach (DataRow itemDamage in Risk_DamageTable.Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
            {//primero recorro los Daños de los riesgos
                decimal valor = 0;
                foreach (DataRow itemWBS in WBS_RISK_Damage_Table.Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + RiskRow[DT_Risk.ID] + " AND " + DT_WBS_RISK_DAMAGE.ID_DAMAGE + " = " + itemDamage[DT_Risk_Damages.ID_DAMAGE]))
                {//y despues para  sumarlos todos en un mismo daño y encontrar el AD
                    if ((decimal)itemWBS[DT_WBS_RISK_DAMAGE.VALUE] >= 0)
                    {
                        valor += (decimal)itemWBS[DT_WBS_RISK_DAMAGE.VALUE];
                    }
                    else
                    {
                        MostrarErrorDialog("Value can't be negative!!!");
                        valor += (decimal)TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Find(new object[] { itemWBS[DT_WBS_RISK_DAMAGE.ID_WBS], itemWBS[DT_WBS_RISK_DAMAGE.ID_DAMAGE], itemWBS[DT_WBS_RISK_DAMAGE.ID_RISK] })[DT_WBS_RISK_DAMAGE.VALUE];
                    }
                }
                itemDamage[DT_Risk_Damages.VALUE] = valor;
            }
            TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Merge(WBS_RISK_Damage_Table);
            TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Merge(Risk_DamageTable);
        }

        #region DgRiskDamages_Events
        private void DgRiskDamages_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    RefreshRiskDamageValues(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TreeOperation.SetRiskLineValues(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.UpdateLinesValues();
                    TheCurrentLayout.SetLinesThickness();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DgRiskDamages_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    RefreshRiskDamageValues(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TreeOperation.SetRiskLineValues(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.UpdateLinesValues();
                    TheCurrentLayout.SetLinesThickness();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DgRiskDamages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    RefreshRiskDamageValues(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TreeOperation.SetRiskLineValues(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.UpdateLinesValues();
                    TheCurrentLayout.SetLinesThickness();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
        #endregion

        private void BtnAddRiskRole_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    DataRow RiskRow = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID);
                    ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole();
                    DataTable roleCodif = ws.GetRolesData().Tables[DT_Role.ROLE_TABLE].Copy();
                    ws.Dispose();
                    WindowSelection frmSelection = new WindowSelection();
                    if (TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + (int)RiskRow[DT_Risk.ID]).Count() > 0)
                    {
                        frmSelection.Dt = General.DeleteExists(roleCodif, TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + (int)RiskRow[DT_Risk.ID]).CopyToDataTable(), DT_Role.IDROL_COLUMN);
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
                            DataRow drRole = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].NewRow();
                            drRole[DT_Role_Risk.ID_RISK] = RiskRow[DT_Risk.ID];
                            drRole[DT_Role_Risk.NAME_SHORT] = TextRisk.Text;
                            drRole[DT_Role_Risk.Role] = itemRole[DT_Role.ROLE_COLUM];
                            drRole[DT_Role_Risk.IDROL_COLUMN] = itemRole[DT_Role.IDROL_COLUMN];
                            TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRole);
                            foreach (var item in TreeOperation.GetOnlyMyChildrenWithCM(TheCurrentLayout.Line_Selected))
                            {
                                if (item.IsCM)
                                {
                                    if (!(TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Rows.Contains(new object[] { item.ID, itemRole[DT_Role.IDROL_COLUMN] })))
                                    {
                                        DataRow drRoleCM = TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].NewRow();
                                        drRoleCM[DT_Role_CM.ID_CM] = item.ID;
                                        drRoleCM[DT_Role_CM.NAME_SHORT] = item.ShortName;
                                        drRoleCM[DT_Role_CM.Role] = itemRole[DT_Role.ROLE_COLUM];
                                        drRoleCM[DT_Role_CM.IDROL_COLUMN] = itemRole[DT_Role.IDROL_COLUMN];
                                        TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Rows.Add(drRoleCM);
                                    }
                                }
                                else
                                {
                                    if (!(TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { item.ID, itemRole[DT_Role.IDROL_COLUMN] })))
                                    {
                                        DataRow drRoleRisk = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].NewRow();
                                        drRoleRisk[DT_Role_Risk.ID_RISK] = item.ID;
                                        drRoleRisk[DT_Role_Risk.NAME_SHORT] = item.ShortName;
                                        drRoleRisk[DT_Role_Risk.Role] = itemRole[DT_Role.ROLE_COLUM];
                                        drRoleRisk[DT_Role_Risk.IDROL_COLUMN] = itemRole[DT_Role.IDROL_COLUMN];
                                        TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRoleRisk);
                                    }
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

        private void BtnDelRiskRole_Click(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
            if (DgRiskRoles.SelectedIndex >= 0)
            {
                Selected_RiskRoleRow = DvRoleRisk[DgRiskRoles.SelectedIndex].Row;
                if (Selected_RiskRoleRow[DT_Role_Risk.Role].ToString() != "Administrator")
                {
                    IS_DELETING_RISK_ROLE_ROW = true;
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + Selected_RiskRoleRow[DT_Role_Risk.Role].ToString() + "]?");
                }
                else
                {
                    MostrarErrorDialog("'Administrator' role can't be deleted!");
                }
            }
        }

        private void Delete_Role(DataRow fila)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    foreach (var item in TreeOperation.GetOnlyMyChildrenWithCM(TheCurrentLayout.Line_Selected))
                    {
                        if (item.IsCM)
                        {
                            if (TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Rows.Contains(new object[] { item.ID, fila[DT_Role.IDROL_COLUMN] }))
                            {
                                TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Rows.Find(new object[] { item.ID, fila[DT_Role.IDROL_COLUMN] }).Delete();
                            }
                        }
                        else
                        {
                            if (TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { item.ID, fila[DT_Role.IDROL_COLUMN] }))
                            {
                                TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Find(new object[] { item.ID, fila[DT_Role.IDROL_COLUMN] }).Delete();
                            }
                        }
                    }
                    fila.Delete();
                    IS_DELETING_RISK_ROLE_ROW = false;
                }
            }
            catch (Exception ex)
            {
                IS_DELETING_RISK_ROLE_ROW = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        #endregion

        private void DeleteRisk()
        {
            try
            {
                TreeOperation.DeleteLine(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds);

                TheCurrentLayout.DropLines();
                TheCurrentLayout.DropRectangles();
                TheCurrentLayout.LoadLines();
                TheCurrentLayout.LoadRectangles();
                TheCurrentLayout.DrawNumbers();
                TextProbabilityChange(TheCurrentLayout.MainLine);
                TheCurrentLayout.SetLinesThickness();
                CruzarTablaRisk(TheCurrentLayout.Ds);
                CruzarTablaCM(TheCurrentLayout.Ds);
                IS_DELETING_RISK = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_RISK = false;
                MostrarErrorDialog(ex.Message);
            }            
        }

        /// <summary>
        /// Delete a Risk and its children
        /// </summary>
        private void MenuDeleteRisk_Click(object sender, RoutedEventArgs e)
        {
            MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + TheCurrentLayout.Line_Selected.ShortName + "] and all its children?");
            IS_DELETING_RISK = true;
        }

        /// <summary>
        /// Move a Risk and its children
        /// </summary>
        private void MenuMoveRisk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new VerticalMenu("Moving").ShowDialog();
                TheCurrentLayout.MoviendoRisk = true;
                TheCurrentLayout.LinesMoving = new List<RiskPolyLine>();
                TheCurrentLayout.LinesMoving.AddRange(TreeOperation.GetMeAndMyChildrenWithCM(TheCurrentLayout.Line_Selected));
                if (TheCurrentLayout != null)
                {
                    TheCurrentLayout.GridPaintLines.Children.Remove(TheCurrentLayout.LineInMoving);
                    System.Drawing.Color lnColor = System.Drawing.Color.FromArgb(int.Parse(TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram)[TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                    TheCurrentLayout.LineInMoving = new RiskPolyLine(TheCurrentLayout.GridPaintLines, MenuRisk, false)
                    {
                        Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(lnColor.A, lnColor.R, lnColor.G, lnColor.B)),
                        StrokeThickness = 3,
                        IsMoving = true
                    };
                    TheCurrentLayout.LineInMoving.NewDrawAtPoint(new Point(TheCurrentLayout.X, TheCurrentLayout.Y), "");
                }
                foreach (var item in TheCurrentLayout.LinesMoving)
                {
                    item.Oculto = true;
                }
                Cursor = Cursors.Hand;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// Add a CounterMeasure (CM) to RIsk
        /// </summary>
        private void MenuCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //DsMain = CurrentLayout.Ds;
                WindowCM windowCM = new WindowCM()
                {
                    ID_Project = (int)TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(TheCurrentLayout.ID_Diagram)[DT_Diagram.ID_PROJECT],
                    CMRow = TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].NewRow(),
                    DsCM = TheCurrentLayout.Ds,
                    LOGIN_USER = LoginUser,
                    CM_RoleTable = TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Copy(),
                    CM_WBS_Table = TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Copy(),
                    WBS_CM_Damage = TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Copy(),
                    CM_Damage_Table = TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Copy(),
                    Operation = General.INSERT,
                    RowFather = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID),
                    RiskTreeID = TheCurrentLayout.ID_Diagram,
                    RiskPadre = TheCurrentLayout.Line_Selected,
                    Icon = Icon,
                    MyCM = TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Copy()
                };
                windowCM.HasAccess = true;
                windowCM.Probability = 0;
                if (windowCM.ShowDialog() == true)
                {
                    TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Add(windowCM.CMRow);
                    //int pos = TreeOperation.DetectClickPosition(TheCurrentLayout.PointSelected, TheCurrentLayout.Line_Selected);
                    RiskPolyLine Line_Created = new RiskPolyLine
                    {
                        ID = (int)windowCM.CMRow[DT_CounterM.ID],
                        IsCM = true,
                        Father = TheCurrentLayout.Line_Selected,
                        ShortName = "LineCreated",
                        IdRiskFather = TheCurrentLayout.Line_Selected.ID
                    };
                    TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Merge(windowCM.CM_RoleTable);
                    TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Merge(windowCM.CM_WBS_Table);
                    TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Merge(windowCM.WBS_CM_Damage);
                    TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Merge(windowCM.CM_Damage_Table);
                    TheCurrentLayout.InsertCM(Line_Created, TheCurrentLayout.Line_Selected, TheCurrentLayout.PointSelected);

                    TheCurrentLayout.DropLines();
                    TheCurrentLayout.DropRectangles();
                    TheCurrentLayout.LoadLines();
                    TheCurrentLayout.LoadRectangles();

                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.SetLinesThickness();
                    TextProbabilityChange(TheCurrentLayout.MainLine);
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                    CruzarTablaCM(TheCurrentLayout.Ds);
                }

            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void CounterMeasureCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null)
                {
                    if (TheCurrentLayout.ID_Diagram != 0)
                    {
                        TheCurrentLayout.GridPaintLines.Children.Remove(TheCurrentLayout.Line_Created);
                        //HACER: comando add cm              
                        TheCurrentLayout.Line_Created = new RiskPolyLine(TheCurrentLayout.GridPaintLines, MenuRisk, true)
                        {
                            Stroke = new SolidColorBrush(System.Windows.Media.Colors.Black),

                            StrokeThickness = 3
                        };
                        TheCurrentLayout.Line_Created.NewDrawAtPoint(new Point(TheCurrentLayout.X, TheCurrentLayout.Y), "");
                        TheCurrentLayout.Creando = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }

        }

        private void Scope_Click(object sender, RoutedEventArgs e)
        {
            VerticalMenu win = new VerticalMenu("Scope");
            win.ShowDialog();
            TheCurrentLayout.IsScoping = true;
            TheCurrentLayout.DropRectangles();
            TheCurrentLayout.ScopeLine = TheCurrentLayout.Line_Selected;
            TheCurrentLayout.Scope();
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VerticalMenu win = new VerticalMenu("Copying...");
                win.ShowDialog();
                Copy(TheCurrentLayout.Line_Selected);
                TheCurrentLayout.Copiando = true;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// Copy the risk and its children in memory.
        /// </summary>
        private void Copy(RiskPolyLine LineToCpy)
        {
            try
            {
                TheCurrentLayout.ListCopy = new List<RiskPolyLine>();
                TheCurrentLayout.CopyRisk = new RiskPolyLine(TheCurrentLayout.GridPaintLines, MenuRisk, false)
                {
                    ShortName = LineToCpy.ShortName,
                    ID = LineToCpy.ID,
                    IdRiskFather = 0,
                    Position = 0,
                    Collapsed = LineToCpy.Collapsed,
                    Probability = LineToCpy.Probability,
                    IsCM = LineToCpy.IsCM,
                    IsRoot = false
                };
                TheCurrentLayout.CopyRisk.TextPanel.Visibility = Visibility.Collapsed;
                TheCurrentLayout.ListCopy.Add(TheCurrentLayout.CopyRisk);
                foreach (var item in TreeOperation.GetOnlyMyChildrenWithCM(LineToCpy))
                {
                    if (!(item.IsCM))
                    {
                        RiskPolyLine line = new RiskPolyLine(TheCurrentLayout.GridPaintLines, MenuRisk, false)
                        {
                            ID = item.ID,
                            IdRiskFather = item.IdRiskFather,
                            Stroke = item.Stroke,
                            ShortName = item.ShortName,
                            Position = item.Position,
                            IsCM = item.IsCM,
                            Collapsed = item.Collapsed,
                            FromTop = item.FromTop,
                            IsRoot = item.IsRoot
                        };
                        line.TextPanel.Visibility = Visibility.Collapsed;
                        TheCurrentLayout.ListCopy.Add(line);
                    }
                    else
                    {
                        RiskPolyLine cmLine = new RiskPolyLine(TheCurrentLayout.GridPaintLines, MenuCM, true)
                        {
                            ID = item.ID,
                            IdRiskFather = item.IdRiskFather,
                            Stroke = item.Stroke,
                            ShortName = item.ShortName,
                            Position = item.Position,
                            IsCM = item.IsCM,
                            Collapsed = item.Collapsed,
                            FromTop = item.FromTop,
                            IsRoot = item.IsRoot
                        };
                        cmLine.TextPanel.Visibility = Visibility.Collapsed;
                        TheCurrentLayout.ListCopy.Add(cmLine);
                    }
                }
                TreeOperation.Build_Tree(TheCurrentLayout.ListCopy, TheCurrentLayout.CopyRisk);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void ButtonPaste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout.Copiando)
                {
                    VerticalMenu win = new VerticalMenu("Paste...");
                    win.ShowDialog();
                    TreeOperation.CreateCopyOfLine(TheCurrentLayout.CopyRisk, TheCurrentLayout.Line_Selected.ID, TheCurrentLayout.Ds);
                    TheCurrentLayout.CopyRisk.Father = TheCurrentLayout.Line_Selected;
                    TheCurrentLayout.CopyRisk.Position = TheCurrentLayout.Line_Selected.Children.Count - 1;
                    TheCurrentLayout.InsertRisk(TheCurrentLayout.CopyRisk, TheCurrentLayout.Line_Selected, TheCurrentLayout.PointSelected);
                    TheCurrentLayout.DropLines();
                    TheCurrentLayout.DropRectangles();
                    TheCurrentLayout.LoadLines();
                    TheCurrentLayout.LoadRectangles();
                    TextProbabilityChange(TheCurrentLayout.MainLine);
                    //TextProbability.Text = Math.Round(General.AcumulatedLikelihood(TheCurrentLayout.MainLine) * 100, 2).ToString() + " %";
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.SetLinesThickness();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                    CruzarTablaCM(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void ConcatProjects(DataRow drRisk, DataRow[] drDannosImportados, DataRow[] drMisDannos, DataSet importDS)
        {
            try
            {
                foreach (DataRow item in drDannosImportados.Concat(drMisDannos))
                {
                    //Asignando los daños de cada arbol al arbol de conjunto
                    if (!(TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Rows.Contains(new object[] { TheCurrentLayout.ID_Diagram, item[DT_Diagram_Damages.ID_DAMAGE] })))
                    {
                        DataRow drTop = TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].NewRow();
                        drTop[DT_Diagram_Damages.ID_RISKTREE] = TheCurrentLayout.ID_Diagram;
                        drTop[DT_Diagram_Damages.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                        drTop[DT_Diagram_Damages.DAMAGE] = item[DT_Diagram_Damages.DAMAGE];
                        drTop[DT_Diagram_Damages.COLOR] = item[DT_Diagram_Damages.COLOR];
                        TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Rows.Add(drTop);
                    }
                    //Aca el riesgo principal del ds importado coje los Damages existentes en el diagrama conjunto
                    if (!(TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_Diagram_Damages.ID_DAMAGE] })))
                    {
                        DataRow topR = TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].NewRow();
                        topR[DT_Risk_Damages.COLOR] = item[DT_Diagram_Damages.COLOR];
                        topR[DT_Risk_Damages.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                        topR[DT_Risk_Damages.ID_RISK] = drRisk[DT_Risk.ID];
                        topR[DT_Risk_Damages.VALUE] = 0;
                        topR[DT_Risk_Damages.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                        topR[DT_Risk_Damages.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString();
                        topR[DT_Risk_Damages.FATHER] = TheCurrentLayout.Line_Selected.ShortName;
                        topR[DT_Risk_Damages.GROUPE_NAME] = drRisk[DT_Risk.GROUPE_NAME];
                        topR[DT_Risk_Damages.ID_FATHER] = TheCurrentLayout.Line_Selected.ID;
                        topR[DT_Risk_Damages.ID_GROUPE] = drRisk[DT_Risk.ID_GROUPE];
                        topR[DT_Risk_Damages.ID_RISK_TREE] = TheCurrentLayout.ID_Diagram;
                        topR[DT_Risk_Damages.IS_ROOT] = drRisk[DT_Risk.IS_ROOT];
                        topR[DT_Risk_Damages.PROBABILITY] = drRisk[DT_Risk.PROBABILITY];
                        topR[DT_Risk_Damages.RISK_NAMESHORT] = drRisk[DT_Risk.NAMESHORT];
                        topR[DT_Risk_Damages.RISK_TREE] = TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(TheCurrentLayout.ID_Diagram)[DT_Diagram.DIAGRAM_NAME];
                        topR[DT_Risk_Damages.STATUS] = drRisk[DT_Risk.ENABLED];
                        TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Add(topR);
                    }
                }
                //el root importado obtiene todos los roles del padre actual
                foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + TheCurrentLayout.Line_Selected.ID))
                {
                    if (!(TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_Role_Risk.IDROL_COLUMN] })))
                    {
                        DataRow newRow = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].NewRow();
                        newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                        newRow[DT_Role_Risk.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                        newRow[DT_Role_Risk.Role] = item[DT_Role_Risk.Role];
                        TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                    }
                }//EL root obtiene los roles que tnia originalmente
                foreach (DataRow item in importDS.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + TheCurrentLayout.CopyRisk.ID))
                {
                    if (!(TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_Role_Risk.IDROL_COLUMN] })))
                    {
                        DataRow newRow = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].NewRow();
                        newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                        newRow[DT_Role_Risk.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                        newRow[DT_Role_Risk.Role] = item[DT_Role_Risk.Role];
                        TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                    }
                }
                foreach (DataRow item in importDS.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + TheCurrentLayout.CopyRisk.ID))
                {
                    if (!(TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_RISK_WBS.ID_WBS] })))
                    {
                        DataRow newRow = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].NewRow();
                        newRow[DT_RISK_WBS.ID_RISK] = drRisk[DT_Risk.ID];
                        newRow[DT_RISK_WBS.ID_WBS] = item[DT_RISK_WBS.ID_WBS];
                        TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Add(newRow);
                    }
                }
                foreach (var item in TheCurrentLayout.CopyRisk.Children)
                {
                    item.IdRiskFather = (int)drRisk[DT_Risk.ID];
                }
                //--------------------------------------------------------------------------------------
                TheCurrentLayout.CopyRisk.ID = (int)drRisk[DT_Risk.ID];
                TheCurrentLayout.LinesList.Add(TheCurrentLayout.CopyRisk);

                /*Paste(TheCurrentLayout.CopyRisk, importDS, TheCurrentLayout.Ds, TheCurrentLayout.ID_Diagram, TheCurrentLayout.LinesList);*///Aca pego el resto del diagrama
                Paste(TheCurrentLayout.CopyRisk, importDS, TheCurrentLayout.Ds, TheCurrentLayout.ID_Diagram);
                TheCurrentLayout.CopyRisk.IdRiskFather = TheCurrentLayout.Line_Selected.ID;
                TheCurrentLayout.Copiando = false;
                //RegisterLines(ListaRiesgoLinea, Ds);
                foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram))
                {
                    //Por cada riesgo del diagrama
                    foreach (DataRow itemRisk in TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + TheCurrentLayout.ID_Diagram))
                    {
                        if (!(TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { itemRisk[DT_Risk.ID], item[DT_Risk_Damages.ID_DAMAGE] })))
                        {
                            DataRow drDamage = TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].NewRow();
                            drDamage[DT_Risk_Damages.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                            drDamage[DT_Risk_Damages.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString();
                            drDamage[DT_Risk_Damages.VALUE] = 0;
                            drDamage[DT_Risk_Damages.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                            drDamage[DT_Risk_Damages.ID_RISK] = itemRisk[DT_Risk.ID];
                            drDamage[DT_Risk_Damages.ID_RISK_TREE] = itemRisk[DT_Risk.ID_DIAGRAM];
                            drDamage[DT_Risk_Damages.COLOR] = item[DT_Diagram_Damages.COLOR];
                            drDamage[DT_Risk_Damages.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                            drDamage[DT_Risk_Damages.DAMAGE] = item[DT_Diagram_Damages.DAMAGE];
                            drDamage[DT_Risk_Damages.FATHER] = TheCurrentLayout.Line_Selected.ShortName;
                            drDamage[DT_Risk_Damages.GROUPE_NAME] = itemRisk[DT_Risk.GROUPE_NAME];
                            drDamage[DT_Risk_Damages.ID_FATHER] = TheCurrentLayout.Line_Selected.ID;
                            drDamage[DT_Risk_Damages.ID_GROUPE] = itemRisk[DT_Risk.ID_GROUPE];
                            drDamage[DT_Risk_Damages.ID_RISK_TREE] = TheCurrentLayout.ID_Diagram;
                            drDamage[DT_Risk_Damages.IS_ROOT] = itemRisk[DT_Risk.IS_ROOT];
                            drDamage[DT_Risk_Damages.PROBABILITY] = itemRisk[DT_Risk.PROBABILITY];
                            drDamage[DT_Risk_Damages.RISK_NAMESHORT] = itemRisk[DT_Risk.NAMESHORT];
                            drDamage[DT_Risk_Damages.RISK_TREE] = TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(TheCurrentLayout.ID_Diagram)[DT_Diagram.DIAGRAM_NAME];
                            drDamage[DT_Risk_Damages.STATUS] = itemRisk[DT_Risk.ENABLED];
                            TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Add(drDamage);
                        }
                    }
                    foreach (DataRow itemCM in TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK_TREE + " = " + TheCurrentLayout.ID_Diagram))
                    {
                        if (!(TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Contains(new object[] { itemCM[DT_CounterM.ID], item[DT_CounterM_Damage.ID_DAMAGE] })))
                        {
                            DataRow drDamage = TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].NewRow();
                            drDamage[DT_CounterM_Damage.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                            drDamage[DT_CounterM_Damage.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString();
                            drDamage[DT_CounterM_Damage.VALUE] = 0;
                            drDamage[DT_CounterM_Damage.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                            drDamage[DT_CounterM_Damage.ID_COUNTERM] = itemCM[DT_CounterM.ID];
                            drDamage[DT_CounterM_Damage.ID_RISK_TREE] = itemCM[DT_CounterM.ID_RISK_TREE];
                            drDamage[DT_CounterM_Damage.COLOR] = item[DT_Diagram_Damages.COLOR];
                            drDamage[DT_CounterM_Damage.COUNTERM_NAMESHORT] = itemCM[DT_CounterM.NAMESHORT];
                            drDamage[DT_CounterM_Damage.GROUPENAME] = itemCM[DT_CounterM.GROUPE_NAME];
                            drDamage[DT_CounterM_Damage.IDRISK] = itemCM[DT_CounterM.ID_RISK];
                            drDamage[DT_CounterM_Damage.ID_GROUPE] = itemCM[DT_CounterM.ID_GROUPE];
                            drDamage[DT_CounterM_Damage.RISK] = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(itemCM[DT_CounterM.ID_RISK])[DT_Risk.NAMESHORT];
                            drDamage[DT_CounterM_Damage.RISK_REDUCTION] = itemCM[DT_CounterM.PROBABILITY];
                            drDamage[DT_CounterM_Damage.STATUS] = itemCM[DT_CounterM.ENABLED];

                            TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Add(drDamage);
                        }
                    }
                }
                TheCurrentLayout.DropLines();
                TheCurrentLayout.DropRectangles();
                TheCurrentLayout.LoadComboDamage();
                TheCurrentLayout.LoadLines();
                TheCurrentLayout.LoadRectangles();
                TheCurrentLayout.DrawNumbers();
                TextProbabilityChange(TheCurrentLayout.MainLine);
                TheCurrentLayout.SetLinesThickness();
                TheCurrentLayout.DrawNumbers();
                TheCurrentLayout.SetLinesThickness();
                CruzarTablaRisk(TheCurrentLayout.Ds);
                CruzarTablaCM(TheCurrentLayout.Ds);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }            
        }

        #region ImportProject
        private void ButtonImportProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int InternalDiagramID = 0;

                WindowSelection frmSelection = new WindowSelection();
                TheCurrentLayout.Ds = TheCurrentLayout.Ds;
                //var temp = Ds.Tables[DT_RiskTree.RiskTree_TABLA].AsEnumerable().Where(x => x.Field<int>(DT_RiskTree.ID_RISK_TREE) != CurrentLayout.ID_Diagram).AsDataView();
                frmSelection.Dt = General.DeleteExists(TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Copy(),
                    TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Select(DT_Diagram.ID_DIAGRAM + " = " + TheCurrentLayout.ID_Diagram).CopyToDataTable(), DT_Diagram.ID_DIAGRAM);

                frmSelection.DcolumToShow = new string[] { DT_Diagram.DIAGRAM_NAME };
                frmSelection.DcolumToShowAlias = new string[] { DT_Diagram.DIAGRAM_NAME };
                frmSelection.Title = "Diagrams";
                frmSelection.P.FilterString = "Diagrams";
                frmSelection.ColumnToFilter = DT_Diagram.DIAGRAM_NAME;
                if (frmSelection.ShowDialog() == true)
                {
                    TheCurrentLayout.ListCopy = new List<RiskPolyLine>();
                    InternalDiagramID = (int)frmSelection.RowsSelected[0][DT_Diagram.ID_DIAGRAM];
                    ImportDS = new UserDataSet();
                    ServiceRiskController.WebServiceRisk risk = new ServiceRiskController.WebServiceRisk();
                    ImportDS.Merge(risk.GetRiskTreeID(new object[] { InternalDiagramID }));
                    risk.Dispose();
                    foreach (DataRow item in ImportDS.Tables[DT_Risk.TABLE_NAME].Rows)
                    {
                        RiskPolyLine riskLine = new RiskPolyLine(new Grid(), MenuRisk, false)
                        {
                            ShortName = item[DT_Risk.NAMESHORT].ToString(),
                            ID = (int)item[DT_Risk.ID],
                            Position = 0,
                            IsRoot = (bool)item[DT_Risk.IS_ROOT],
                            Collapsed = (bool)item[DT_Risk.ISCOLLAPSED],
                            Probability = (decimal)item[DT_Risk.PROBABILITY],
                            IsCM = false
                        };
                        if (ImportDS.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + (int)item[DT_Risk.ID]).Any())
                        {
                            riskLine.IdRiskFather = (int)ImportDS.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + (int)item[DT_Risk.ID]).First()[DT_RiskStructure.IDRISK_FATHER];
                        }
                        else { riskLine.IdRiskFather = 0; }
                        TheCurrentLayout.ListCopy.Add(riskLine);
                    }
                    foreach (DataRow item in ImportDS.Tables[DT_CounterM.TABLE_NAME].Rows)
                    {
                        RiskPolyLine cmline = new RiskPolyLine(new Grid(), MenuCM, true)
                        {
                            IsCM = true,
                            Position = 0,
                            FromTop = false,
                            ShortName = item[DT_CounterM.NAMESHORT].ToString(),
                            IdRiskFather = (int)item[DT_CounterM.ID_RISK],
                            ID = (int)item[DT_CounterM.ID],
                            Probability = (decimal)item[DT_CounterM.PROBABILITY],
                            IsActivated = (bool)item[DT_CounterM.ENABLED]
                        };
                        TheCurrentLayout.ListCopy.Add(cmline);
                    }

                    TreeOperation.Build_Tree(TheCurrentLayout.ListCopy);
                    TheCurrentLayout.CopyRisk = TheCurrentLayout.ListCopy.FirstOrDefault(p => p.IsRoot);

                    DrImportRisk = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].NewRow();
                    DrImportRisk[DT_Risk.COMMENTS] = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.CopyRisk.ID)[DT_Risk.COMMENTS];
                    DrImportRisk[DT_Risk.ENABLED] = true;
                    DrImportRisk[DT_Risk.FROM_TOP] = TheCurrentLayout.CopyRisk.FromTop;
                    DrImportRisk[DT_Risk.ID_DIAGRAM] = TheCurrentLayout.ID_Diagram;
                    DrImportRisk[DT_Risk.ISCOLLAPSED] = TheCurrentLayout.CopyRisk.Collapsed;
                    DrImportRisk[DT_Risk.IS_ROOT] = false;
                    DrImportRisk[DT_Risk.NAMESHORT] = TheCurrentLayout.CopyRisk.ShortName;
                    DrImportRisk[DT_Risk.POSITION] = TheCurrentLayout.Line_Selected.Children.Count;
                    DrImportRisk[DT_Risk.PROBABILITY] = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.CopyRisk.ID)[DT_Risk.PROBABILITY];
                    DrImportRisk[DT_Risk.IDRISK_FATHER] = TheCurrentLayout.Line_Selected.ID;
                    TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(DrImportRisk);
                    DataRow rowstructure = TheCurrentLayout.Ds.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                    rowstructure[DT_RiskStructure.IDRISK] = DrImportRisk[DT_Risk.ID];
                    rowstructure[DT_RiskStructure.IDRISK_FATHER] = TheCurrentLayout.Line_Selected.ID;
                    TheCurrentLayout.Ds.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);

                    DrDannosImportados = ImportDS.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + InternalDiagramID);
                    DrMisDannos = TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram);
                    ConcatProjects(DrImportRisk, DrDannosImportados, DrMisDannos, ImportDS);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
        #endregion

        private void EnableRiskButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool result = EnableRisk(TheCurrentLayout.Line_Selected, false, false);
                if (result)
                {
                    VerticalMenu win = new VerticalMenu("Enabled");
                    win.ShowDialog();
                }
                else
                {
                    VerticalMenu win = new VerticalMenu("Disabled");
                    win.ShowDialog();
                }
                OnProjectChange();
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// Enable/Disable risk
        /// </summary>
        /// <param name="Risk"> Risk to Enable/Disable</param>
        /// <param name="isGroup"> True if risk group, else if individual risk</param>
        /// <param name="estadoActual"> Risk group state (Enable/Disable)</param>
        private bool EnableRisk(RiskPolyLine Risk, bool isGroup = false, bool estadoActual = false)
        {
            try
            {
                bool result = false;
                bool enabledColumn = (isGroup) ? estadoActual : (bool)TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Risk.ID)[DT_Risk.ENABLED];
                if (enabledColumn)
                {
                    TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Risk.ID)[DT_Risk.ENABLED] = false;
                    foreach (DataRow damageRow in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + Risk.ID))
                    {
                        damageRow[DT_Risk_Damages.STATUS] = false;
                    }
                    (TheCurrentLayout.LinesList.Find(item => (item.ID == Risk.ID))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                    (TheCurrentLayout.LinesList.Find(item => (item.ID == Risk.ID))).IsActivated = false;
                    foreach (var itemi in Risk.Children)
                    {
                        if (itemi.IsCM)
                        {
                            itemi.IsActivated = false;
                            itemi.SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                            (TheCurrentLayout.LinesList.Find(item => (item.ID == itemi.ID))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                            (TheCurrentLayout.LinesList.Find(item => (item.ID == itemi.ID))).IsActivated = false;
                            TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(itemi.ID)[DT_CounterM.ENABLED] = false;
                            foreach (DataRow damageRow in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + itemi.ID))
                            {
                                damageRow[DT_CounterM_Damage.STATUS] = false;
                            }
                        }
                    }
                }
                else
                {
                    TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Risk.ID)[DT_Risk.ENABLED] = true;
                    foreach (DataRow damageRow in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + Risk.ID))
                    {
                        damageRow[DT_Risk_Damages.STATUS] = true;
                    }
                    System.Drawing.Color drawingCColor = System.Drawing.Color.FromArgb(int.Parse(TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram)[TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                    (TheCurrentLayout.LinesList.Find(item => (item.ID == Risk.ID))).SetColor(new SolidColorBrush(System.Windows.Media.Color.FromArgb(drawingCColor.A, drawingCColor.R, drawingCColor.G, drawingCColor.B)));
                    (TheCurrentLayout.LinesList.Find(item => (item.ID == Risk.ID))).IsActivated = true;

                    foreach (var itemi in Risk.Children)
                    {
                        if (itemi.IsCM)
                        {
                            DisableCounterMeasure(itemi, true, !TheCurrentLayout.LinesListCMState[Convert.ToInt32(itemi.ID)]);
                        }
                    }
                    result = true;
                }
                TheCurrentLayout.UpdateLinesValues();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("EnableRisk: " + ex.Message);
            }

        }

        /// <summary>
        /// Code executed after Enable/Disable risk
        /// </summary>
        private void OnProjectChange()
        {
            TheCurrentLayout.DrawNumbers();
            TextProbabilityChange(TheCurrentLayout.MainLine);
            TheCurrentLayout.SetLinesThickness();

            CruzarTablaRisk(TheCurrentLayout.Ds);
            CruzarTablaCM(TheCurrentLayout.Ds);
        }

        #endregion

        #region CounterMeasure_Menu
        /// <summary>
        /// Edit the value of an existing CM 
        /// </summary>
        private void MenuEditCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //DsMain = CurrentLayout.Ds;
                if (TheCurrentLayout.Line_Selected.IsActivated)
                {
                    WindowCM windowCM = new WindowCM
                    {
                        ID_Project = (int)TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(TheCurrentLayout.ID_Diagram)[DT_Diagram.ID_PROJECT],
                        CMRow = TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID),
                        DsCM = TheCurrentLayout.Ds,
                        Operation = General.UPDATE,
                        LOGIN_USER = LoginUser,
                        CM_Damage_Table = TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Copy(),
                        WBS_CM_Damage = TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Copy(),
                        CM_WBS_Table = TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Copy(),
                        CM_RoleTable = TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Copy(),
                        Posicion = (int)TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_CounterM.POSITION],
                        Icon = Icon,
                        MyCM = TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Copy()
                    };
                    windowCM.HasAccess = TheCurrentLayout.FullAccess(TheCurrentLayout.Line_Selected);
                    windowCM.RiskPadre = TheCurrentLayout.LinesList.Find(l => l.ID == TheCurrentLayout.Line_Selected.IdRiskFather);
                    windowCM.RiskTreeID = (int)TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_CounterM.ID_RISK_TREE];

                    if (windowCM.ShowDialog() == true)
                    {
                        if (TheCurrentLayout.Ds.HasChanges())
                        {
                            TreeOperation.SetCMLineValues(TheCurrentLayout.Line_Selected, windowCM.CMRow);
                            TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Merge(windowCM.CM_Damage_Table);
                            TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Merge(windowCM.WBS_CM_Damage);
                            TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Merge(windowCM.CM_WBS_Table);
                            TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Merge(windowCM.CM_RoleTable);
                            TextProbabilityChange(TheCurrentLayout.MainLine);
                            TheCurrentLayout.DrawNumbers();
                            TheCurrentLayout.UpdateLinesValues();
                            TheCurrentLayout.SetLinesThickness();
                            CruzarTablaRisk(TheCurrentLayout.Ds);                            
                            CruzarTablaCM(TheCurrentLayout.Ds);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("The object selected is disabled. Please enable it to change this value.", "Ensure Risk Message", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        #region TabEditCM
        public void UpdateCMTabInformation()
        {
            if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
            {
                Risk_DamageTable = TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Copy();
                CM_DamageTable = TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Copy();
                WBS_RISK_Damage_Table = TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Copy();
                WBS_CM_Damage_Table = TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Copy();
                Risk_WBS_Table = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Copy();
                CM_WBS_Table = TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Copy();
                Risk_RoleTable = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Copy();
                CM_RoleTable = TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Copy();

                DataRow CMRow = TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID);
                CMLineName = CMRow[DT_CounterM.NAMESHORT].ToString();
                TextCMDetail.Text = CMRow[DT_CounterM.DETAIL].ToString();
                HasAccess = TheCurrentLayout.FullAccess(TheCurrentLayout.Line_Selected);
                DvRoleCM = CM_RoleTable.DefaultView;
                DvRoleCM.RowFilter = DT_Role_CM.ID_CM + " = " + CMRow[DT_CounterM.ID];
                dgRolesCM.ItemsSource = DvRoleCM;

                DVCMWBS = CM_WBS_Table.DefaultView;
                dgWBSCM.ItemsSource = DVCMWBS;
                DVCMWBS.RowFilter = DT_CM_WBS.ID_CM + " = " + CMRow[DT_CounterM.ID];

                DvCM_Damages = WBS_CM_Damage_Table.DefaultView;

                DvCM_Damages.RowFilter = DT_WBS_CM_Damage.ID_CM + " = " + CMRow[DT_CounterM.ID];
                if (DsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.USERNAME + " = '" + LoginUser + "'").Any())
                {
                    string filter = "";
                    DataRow[] item = DsWBS.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.USERNAME + " = '" + LoginUser + "'");
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
                    DvCM_Damages.RowFilter += filter;
                }
                dgCMDamages.ItemsSource = DvCM_Damages;
                CalculateRiskReduction(CMRow);
            }
        }

        private void TextCM_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    if (TextCM.Text != string.Empty)
                    {
                        if (TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK_TREE + " = " + DiagramID + " and " + DT_CounterM.NAMESHORT + " = '" + TextCM.Text + "' and " + DT_CounterM.ID + " <> " + TheCurrentLayout.Line_Selected.ID).Any())
                        {
                            if (new WindowMessageYesNo("The name [" + TextCM.Text + "] Already exists in this diagram. Do you want to use it again?").ShowDialog() == true)
                            {

                                TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_CounterM.NAMESHORT] = TextCM.Text;
                                foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + TheCurrentLayout.Line_Selected.ID))
                                {
                                    item[DT_CounterM_Damage.COUNTERM_NAMESHORT] = TextCM.Text;
                                }
                                TheCurrentLayout.DrawNumbers();
                                TheCurrentLayout.UpdateLinesValues();
                                TheCurrentLayout.SetLinesThickness();
                                CruzarTablaRisk(TheCurrentLayout.Ds);
                                CruzarTablaCM(TheCurrentLayout.Ds);
                            }
                            else
                            {
                                TextCM.Text = TheCurrentLayout.Line_Selected.ShortName;
                            }
                        }
                        else
                        {

                            TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_CounterM.NAMESHORT] = TextCM.Text;
                            foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + TheCurrentLayout.Line_Selected.ID))
                            {
                                item[DT_CounterM_Damage.COUNTERM_NAMESHORT] = TextCM.Text;
                            }
                            TheCurrentLayout.DrawNumbers();
                            TheCurrentLayout.UpdateLinesValues();
                            TheCurrentLayout.SetLinesThickness();
                            CruzarTablaRisk(TheCurrentLayout.Ds);
                            CruzarTablaCM(TheCurrentLayout.Ds);
                        }
                    }
                    else
                    {
                        MostrarErrorDialog("CM name can't be empty!");
                        UpdateCMTabInformation();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void TextCMDetail_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_CounterM.DETAIL] = TextCMDetail.Text;
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void RefreshCMDamageValues(DataRow CMRow)
        {
            foreach (DataRow itemDamage in CM_DamageTable.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
            {//primero recorro los Daños de los riesgos
                decimal valor = 0;
                foreach (DataRow itemWBS in WBS_CM_Damage_Table.Select(DT_WBS_CM_Damage.ID_CM + " = " + CMRow[DT_CounterM.ID] + " AND " + DT_WBS_CM_Damage.ID_DAMAGE + " = " + itemDamage[DT_CounterM_Damage.ID_DAMAGE]))
                {//y despues para  sumarlos todos en un mismo daño y encontrar el AD.
                    if ((decimal)itemWBS[DT_WBS_CM_Damage.VALUE] >= 0)
                    {
                        valor += (decimal)itemWBS[DT_WBS_CM_Damage.VALUE];
                    }
                    else
                    {
                        MostrarErrorDialog("Value can't be negative!!!");
                        valor += (decimal)TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Find(new object[] { itemWBS[DT_WBS_CM_Damage.ID_WBS], itemWBS[DT_WBS_CM_Damage.ID_DAMAGE], itemWBS[DT_WBS_CM_Damage.ID_CM] })[DT_WBS_CM_Damage.VALUE];

                    }
                }
                itemDamage[DT_CounterM_Damage.VALUE] = valor;
            }
            TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Merge(WBS_CM_Damage_Table);
            TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Merge(CM_DamageTable);

        }

        #region  DgCMDamages_events
        private void DgCMDamages_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    RefreshCMDamageValues(TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)); 
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.UpdateLinesValues();
                    TheCurrentLayout.SetLinesThickness();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                    CruzarTablaCM(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DgCMDamages_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                RefreshCMDamageValues(TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                TheCurrentLayout.DrawNumbers();
                TheCurrentLayout.UpdateLinesValues();
                TheCurrentLayout.SetLinesThickness();
                CruzarTablaRisk(TheCurrentLayout.Ds);
                CruzarTablaCM(TheCurrentLayout.Ds);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DgCMDamages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                RefreshCMDamageValues(TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                TheCurrentLayout.DrawNumbers();
                TheCurrentLayout.UpdateLinesValues();
                TheCurrentLayout.SetLinesThickness();
                CruzarTablaRisk(TheCurrentLayout.Ds);
                CruzarTablaCM(TheCurrentLayout.Ds);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
        #endregion

        #region DgWBSCM_Events
        private void CalculateRiskReduction(DataRow CMRow)
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
                            if ((decimal)item[DT_CM_WBS.PROBABILITY] >= 0 && (decimal)item[DT_CM_WBS.PROBABILITY] <= 100)
                            {
                                Probabilities.Add((decimal)item[DT_CM_WBS.PROBABILITY]);
                            }
                            else
                            {
                                decimal originalValue = (decimal)TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Find(new object[] { item[DT_CM_WBS.ID_CM], item[DT_CM_WBS.ID_WBS] })[DT_CM_WBS.PROBABILITY];
                                Probabilities.Add(originalValue);
                                MostrarErrorDialog("Risk Reduction must be between 0 and 100");
                                item[DT_CM_WBS.PROBABILITY] = originalValue;
                            }
                        }
                    }
                }
                if (Probabilities.Count > 0)
                {
                    RiskReduction = Probabilities.Sum() / Probabilities.Count;
                }
                else
                {
                    RiskReduction = 0;
                }
                CMRow[DT_CounterM.PROBABILITY] = RiskReduction;
                foreach (DataRow item in CM_DamageTable.Select(DT_CounterM_Damage.ID_COUNTERM + " = " + CMRow[DT_CounterM.ID]))
                {
                    item[DT_CounterM_Damage.RISK_REDUCTION] = CMRow[DT_CounterM.PROBABILITY];
                }
                TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Merge(CM_WBS_Table);
                TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Merge(CM_DamageTable);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DgWBSCM_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    CalculateRiskReduction(TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.UpdateLinesValues();
                    TheCurrentLayout.SetLinesThickness();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                    CruzarTablaCM(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DgWBSCM_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    CalculateRiskReduction(TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.UpdateLinesValues();
                    TheCurrentLayout.SetLinesThickness();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                    CruzarTablaCM(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DgWBSCM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    CalculateRiskReduction(TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.UpdateLinesValues();
                    TheCurrentLayout.SetLinesThickness();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                    CruzarTablaCM(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
        #endregion

        private void BtnAddWBSCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {

                }
                ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                DataTable roleCodif = ws.GetAllWBS().Tables[DT_WBS.TABLE_NAME].Copy();
                ws.Dispose();
                WindowSelection frmSelection = new WindowSelection();
                if (TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " +  TheCurrentLayout.Line_Selected.ID).Count() > 0)
                {
                    frmSelection.Dt = General.DeleteExists(roleCodif, TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + TheCurrentLayout.Line_Selected.ID).CopyToDataTable(), DT_WBS.ID_WBS);
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
                        DataRow drCMWBS = TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].NewRow();
                        drCMWBS[DT_CM_WBS.ID_CM] = TheCurrentLayout.Line_Selected.ID;
                        drCMWBS[DT_CM_WBS.CM] = TextCM.Text;
                        drCMWBS[DT_CM_WBS.WBS] = item[DT_WBS.WBS_NAME].ToString().TrimStart();
                        drCMWBS[DT_CM_WBS.ID_WBS] = item[DT_WBS.ID_WBS];
                        drCMWBS[DT_CM_WBS.NIVEL] = item[DT_WBS.NIVEL].ToString().TrimStart();
                        drCMWBS[DT_CM_WBS.IS_PRIMARY] = false;
                        drCMWBS[DT_CM_WBS.PRIMARY] = "";
                        drCMWBS[DT_CM_WBS.USERNAME] = item[DT_WBS.USERNAME];
                        drCMWBS[DT_CM_WBS.PROBABILITY] = 0;
                        TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Add(drCMWBS);
                    }
                    foreach (DataRow itemRISKWBSi in TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + TheCurrentLayout.Line_Selected.ID))
                    {
                        foreach (DataRow itemAncestors in BuscarAncestros((int)itemRISKWBSi[DT_RISK_WBS.ID_WBS], DsWBS.Tables[DT_WBS.TABLE_NAME].Clone()).Rows)
                        {
                            if (!(TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Contains(new object[] { TheCurrentLayout.Line_Selected.ID, itemAncestors[DT_WBS.ID_WBS] })))
                            {
                                DataRow drRiskWBSi = TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].NewRow();
                                drRiskWBSi[DT_CM_WBS.ID_CM] = TheCurrentLayout.Line_Selected.ID;
                                drRiskWBSi[DT_CM_WBS.CM] = TextCM.Text;
                                drRiskWBSi[DT_CM_WBS.WBS] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart();
                                drRiskWBSi[DT_CM_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                drRiskWBSi[DT_CM_WBS.NIVEL] = itemAncestors[DT_WBS.NIVEL].ToString().TrimStart();
                                drRiskWBSi[DT_CM_WBS.USERNAME] = itemAncestors[DT_WBS.USERNAME];
                                drRiskWBSi[DT_CM_WBS.IS_PRIMARY] = false;
                                drRiskWBSi[DT_CM_WBS.PRIMARY] = "";
                                drRiskWBSi[DT_CM_WBS.PROBABILITY] = 0;
                                TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Add(drRiskWBSi);
                            }
                        }
                    }
                    foreach (DataRow itemWBS in TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + TheCurrentLayout.Line_Selected.ID))
                    {
                        if (WBS_isSheet((int)itemWBS[DT_CM_WBS.ID_WBS]))
                        {
                            foreach (DataRow itemDamage in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + TheCurrentLayout.Line_Selected.ID))
                            {
                                DataRow drWBS_CM_Damage = TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                                drWBS_CM_Damage[DT_WBS_CM_Damage.ID_CM] = TheCurrentLayout.Line_Selected.ID;
                                drWBS_CM_Damage[DT_WBS_CM_Damage.ID_WBS] = itemWBS[DT_RISK_WBS.ID_WBS];
                                drWBS_CM_Damage[DT_WBS_CM_Damage.ID_DAMAGE] = itemDamage[DT_Risk_Damages.ID_DAMAGE];
                                drWBS_CM_Damage[DT_WBS_CM_Damage.DAMAGE] = itemDamage[DT_Risk_Damages.DAMAGE];
                                drWBS_CM_Damage[DT_WBS_CM_Damage.VALUE] = 0;
                                drWBS_CM_Damage[DT_WBS_CM_Damage.WBS] = itemWBS[DT_RISK_WBS.WBS];
                                if (!(TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Contains(new object[] { itemWBS[DT_CM_WBS.ID_WBS], itemDamage[DT_CounterM_Damage.ID_DAMAGE], TheCurrentLayout.Line_Selected.ID })))
                                {
                                    TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(drWBS_CM_Damage);
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

        private void BtnDelWBSCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgWBSCM.SelectedIndex >= 0)
                {
                    Selected_CM_WBS_Row = DVCMWBS[dgWBSCM.SelectedIndex].Row;
                    IS_DELETING_CM_WBS = true;
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + Selected_CM_WBS_Row[DT_CM_WBS.WBS].ToString() + "]?");
                }
            }
            catch (Exception ex)
            {
                IS_DELETING_CM_WBS = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Delete_WBS(DataRow fila)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    foreach (DataRow itemR in (TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Select(DT_WBS_CM_Damage.ID_WBS + " = " + fila[DT_WBS.ID_WBS] + " AND " + DT_WBS_CM_Damage.ID_CM + " = " + TheCurrentLayout.Line_Selected.ID)))
                    {
                        itemR.Delete();
                    }
                    fila.Delete();
                    IS_DELETING_WBS = false;
                }                
            }
            catch (Exception ex)
            {
                IS_DELETING_WBS = false;
                MostrarErrorDialog(ex.Message);
            }
        }


        private void BtnSetPrimaryCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    if (dgWBSCM.SelectedIndex >= 0)
                    {
                        foreach (DataRow item in DVCMWBS.Table.Select(DT_CM_WBS.ID_CM + " = " + TheCurrentLayout.Line_Selected.ID))
                        {
                            item[DT_CM_WBS.IS_PRIMARY] = false;
                            item[DT_CM_WBS.PRIMARY] = "";
                        }
                        DVCMWBS[dgWBSCM.SelectedIndex].Row[DT_CM_WBS.IS_PRIMARY] = true;
                        DVCMWBS[dgWBSCM.SelectedIndex].Row[DT_CM_WBS.PRIMARY] = "PRIMARY";
                        TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Merge(DVCMWBS.Table);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnAddRoleCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole();
                    DataTable roleCodif = ws.GetRolesData().Tables[DT_Role.ROLE_TABLE].Copy();
                    ws.Dispose();
                    WindowSelection frmSelection = new WindowSelection();
                    if (TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + TheCurrentLayout.Line_Selected.ID).Count() > 0)
                    {
                        frmSelection.Dt = General.DeleteExists(roleCodif, TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + TheCurrentLayout.Line_Selected.ID).CopyToDataTable(), DT_Role.IDROL_COLUMN);
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
                            DataRow drRole = TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].NewRow();
                            drRole[DT_Role_CM.ID_CM] = TheCurrentLayout.Line_Selected.ID;
                            drRole[DT_Role_CM.NAME_SHORT] = TextCM.Text;
                            drRole[DT_Role_CM.Role] = item[DT_Role.ROLE_COLUM];
                            drRole[DT_Role_CM.IDROL_COLUMN] = item[DT_Role.IDROL_COLUMN];
                            TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Rows.Add(drRole);
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }

        }

        private void Delete_CM_Role(DataRow fila)
        {
            try
            {
                fila.Delete();
                IS_DELETING_CM_ROLE = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_CM_ROLE = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnDelRoleCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgRolesCM.SelectedIndex >= 0)
                {
                    Selected_CM_Role_Row = DvRoleCM[dgRolesCM.SelectedIndex].Row;
                    if (Selected_CM_Role_Row[DT_Role_CM.Role].ToString() != "Administrator")
                    {
                        IS_DELETING_CM_ROLE = true;
                        MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + Selected_CM_Role_Row[DT_Role_CM.Role].ToString() + "]?");
                    }
                    else
                    {
                        MostrarErrorDialog("'Administrator' role can't be deleted!");
                    }
                }
            }
            catch (Exception ex)
            {
                IS_DELETING_CM_ROLE = false;
                MostrarErrorDialog(ex.Message);
            }
        }


        #endregion

        private void DeleteCM()
        {
            try
            {
                TreeOperation.DeleteLine(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds);
                TheCurrentLayout.DropLines();
                TheCurrentLayout.DropRectangles();
                TheCurrentLayout.LoadLines();
                TheCurrentLayout.LoadRectangles();
                TheCurrentLayout.DrawNumbers();
                TextProbabilityChange(TheCurrentLayout.MainLine);
                TheCurrentLayout.SetLinesThickness();
                CruzarTablaRisk(TheCurrentLayout.Ds);
                CruzarTablaCM(TheCurrentLayout.Ds);
                IS_DELETING_CM = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_CM = false;
                MostrarErrorDialog(ex.Message);
            }
            
        }

        /// <summary>
        /// Delete an existing CM
        /// </summary>
        private void MenuDeleteCM_Click(object sender, RoutedEventArgs e)
        {
            MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + TheCurrentLayout.Line_Selected.ShortName + "] ?");
            IS_DELETING_CM = true;
            //DsMain = CurrentLayout.Ds;
            
        }

        /// <summary>
        /// Select a CM to Move
        /// </summary>
        private void MenuMoveCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TheCurrentLayout.MoviendoCM = true;
                TheCurrentLayout.LinesMoving = new List<RiskPolyLine>() { TheCurrentLayout.Line_Selected };
                Cursor = Cursors.Hand;
                VerticalMenu win = new VerticalMenu("Moving");
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// Enable or Disable a CM
        /// </summary>
        private void MenuEnableCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool result = DisableCounterMeasure(TheCurrentLayout.Line_Selected);
                OnProjectChange();
                if (result)
                {
                    VerticalMenu win = new VerticalMenu("Enabled");
                    win.ShowDialog();
                }
                else
                {
                    if (TheCurrentLayout.Line_Selected.Father.IsActivated)
                    {
                        VerticalMenu win = new VerticalMenu("Disabled");
                        win.ShowDialog();
                    }
                    else
                    {
                        VerticalMenu win = new VerticalMenu("No Changes");
                        win.ShowDialog();
                    }
                }
            }
            catch
            {
                throw new Exception("MenuEnableCM_Click error");
            }
        }

        /// <summary>
        /// Disable or enable one counter measure.
        /// </summary>
        /// <param name="cm_Selected"></param>
        private bool DisableCounterMeasure(RiskPolyLine cm_Selected, bool isGroup = false, bool estadoActual = false)
        {
            try
            {
                bool result = false;
                bool enabledColumn = (isGroup) ? estadoActual : (bool)TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cm_Selected.ID)[DT_CounterM.ENABLED];
                if (enabledColumn)
                {
                    TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cm_Selected.ID)[DT_CounterM.ENABLED] = false;
                    foreach (DataRow damageRow in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + cm_Selected.ID))
                    {
                        damageRow[DT_CounterM_Damage.STATUS] = false;
                    }
                    TheCurrentLayout.LinesListCMState[Convert.ToInt32(cm_Selected.ID)] = false;
                    (TheCurrentLayout.LinesList.Find(item => (item.ID == cm_Selected.ID && item.IsCM))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                    (TheCurrentLayout.LinesList.Find(item => (item.ID == cm_Selected.ID && item.IsCM))).IsActivated = false;
                    result = false;
                }
                else
                {
                    if (cm_Selected.Father.IsActivated) // si el padre esta disabled no habilito cm
                    {
                        TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cm_Selected.ID)[DT_CounterM.ENABLED] = true;
                        foreach (DataRow damageRow in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + cm_Selected.ID))
                        {
                            damageRow[DT_CounterM_Damage.STATUS] = true;
                        }
                        TheCurrentLayout.LinesListCMState[Convert.ToInt32(cm_Selected.ID)] = true;
                        (TheCurrentLayout.LinesList.Find(item => (item.ID == cm_Selected.ID && item.IsCM))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Black));
                        (TheCurrentLayout.LinesList.Find(item => (item.ID == cm_Selected.ID && item.IsCM))).IsActivated = true;
                        result = true;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("DisableCounterMeasure: " + ex.Message);
            }
        }
        #endregion

        #region GroupRisk_Menu
        private void GroupingGroupRiskButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowGroupe wg = new WindowGroupe
                {
                    DT_Groups = TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Copy()
                };
                wg.ShowDialog();
                if (wg.DialogResult == true)
                {
                    if (wg.IdGroup == -100)
                    {
                        DataRow drGroup = TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].NewRow();
                        drGroup[DT_Groupe.GROUPE_NAME] = wg.GroupName;
                        TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Add(drGroup);
                        foreach (var item in TheCurrentLayout.RiskGroupSelected)
                        {
                            TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.ID_GROUPE] = drGroup[DT_Groupe.ID_GROUPE];
                            TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.GROUPE_NAME] = drGroup[DT_Groupe.GROUPE_NAME];
                        }
                        foreach (var item in TheCurrentLayout.RiskGroupSelected)
                        {
                            item.Group.IdGroup = (int)drGroup[DT_Groupe.ID_GROUPE];
                            item.Group.GroupName = drGroup[DT_Groupe.GROUPE_NAME].ToString();
                        }
                    }
                    else
                    {
                        foreach (var item in TheCurrentLayout.RiskGroupSelected)
                        {
                            TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.ID_GROUPE] = wg.IdGroup;
                            TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.GROUPE_NAME] = wg.GroupName;
                        }
                        foreach (var item in TheCurrentLayout.RiskGroupSelected)
                        {
                            item.Group.IdGroup = wg.IdGroup;
                            item.Group.GroupName = wg.GroupName;
                        }
                    }
                    CruzarTablaRisk(TheCurrentLayout.Ds);//Esta Linea estaba, la borre al integrar
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Standard Grouping: " + ex.Message);
            }
        }

        private void AddCMGroupRiskButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowCMGroup windowCMGroup = new WindowCMGroup()
                {
                    DsCM = TheCurrentLayout.Ds,
                    RiskTreeID = TheCurrentLayout.ID_Diagram,
                    Icon = Icon,
                    MyCM = TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Copy(),
                    Probability = new decimal(0),
                    TopRiskTable = TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Clone()
                };
                //Roberto: Si capturo un id que va a ser global, lo hago fuera del ciclo para no buscar tanto lo mismo.
                int RiskTreeID = TheCurrentLayout.ID_Diagram;
                //Roberto: Agrego esto para que cuando la Ventana Cargue, el DataGrid, tenga los valores en 0 para cada Damage
                foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + RiskTreeID))
                {
                    DataRow rowTop = windowCMGroup.TopRiskTable.NewRow();
                    rowTop[DT_CounterM_Damage.COLOR] = item[DT_Diagram_Damages.COLOR];
                    rowTop[DT_CounterM_Damage.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                    rowTop[DT_CounterM_Damage.ID_COUNTERM] = -100;
                    rowTop[DT_CounterM_Damage.VALUE] = 0;
                    rowTop[DT_CounterM_Damage.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                    windowCMGroup.TopRiskTable.Rows.Add(rowTop);
                }
                if (windowCMGroup.ShowDialog() == true)
                {
                    foreach (var itemRisk in TheCurrentLayout.RiskGroupSelected)
                    {
                        DataRow CMRow = TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].NewRow();
                        //Roberto: Si capturo un id que va a ser global, lo hago fuera del ciclo para no buscar tanto lo mismo.
                        DataRow RowFather = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(itemRisk.ID);
                        DataTable TopRiskTable = TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Copy();
                        DataTable CM_RoleTable = TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Copy();
                        int Posicion = 0;

                        CMRow[DT_CounterM.NAMESHORT] = windowCMGroup.NameShort;
                        CMRow[DT_CounterM.DETAIL] = windowCMGroup.Detail;
                        CMRow[DT_CounterM.ID_RISK_TREE] = RiskTreeID;
                        CMRow[DT_CounterM.ID_RISK] = itemRisk.ID;
                        CMRow[DT_CounterM.FROM_TOP] = RowFather[DT_Risk.FROM_TOP];
                        CMRow[DT_CounterM.POSITION] = Posicion + 1;
                        CMRow[DT_CounterM.ENABLED] = true;
                        CMRow[DT_CounterM.PROBABILITY] = windowCMGroup.Probability;

                        foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + RiskTreeID))
                        {
                            DataRow rowTop = TopRiskTable.NewRow();
                            rowTop[DT_CounterM_Damage.COLOR] = item[DT_Diagram_Damages.COLOR];
                            rowTop[DT_CounterM_Damage.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                            rowTop[DT_CounterM_Damage.ID_COUNTERM] = CMRow[DT_CounterM.ID];
                            //Modificado por Roberto: El valor sera 0 porque el dueño ahora decide su valor
                            rowTop[DT_CounterM_Damage.VALUE] = 0;
                            rowTop[DT_CounterM_Damage.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                            TopRiskTable.Rows.Add(rowTop);
                        }
                        //GIVING FATHER´S ROLE TO CHILD
                        foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + itemRisk.ID))
                        {
                            DataRow newRow = CM_RoleTable.NewRow();
                            newRow[DT_Role_CM.ID_CM] = CMRow[DT_CounterM.ID];
                            newRow[DT_Role_CM.Role] = item[DT_Role_CM.Role];
                            newRow[DT_Role_CM.IDROL_COLUMN] = item[DT_Role_CM.IDROL_COLUMN];
                            CM_RoleTable.Rows.Add(newRow);
                        }
                        foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + itemRisk.ID))
                        {
                            DataRow newRow = TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].NewRow();
                            newRow[DT_CM_WBS.ID_CM] = CMRow[DT_CounterM.ID];
                            newRow[DT_CM_WBS.ID_WBS] = item[DT_RISK_WBS.ID_WBS];
                            newRow[DT_CM_WBS.IS_PRIMARY] = item[DT_RISK_WBS.IS_PRIMARY];
                            newRow[DT_CM_WBS.NIVEL] = item[DT_RISK_WBS.NIVEL];
                            newRow[DT_CM_WBS.PRIMARY] = item[DT_RISK_WBS.PRIMARY];
                            newRow[DT_CM_WBS.USERNAME] = item[DT_RISK_WBS.USERNAME];
                            newRow[DT_CM_WBS.WBS] = item[DT_RISK_WBS.WBS];
                            TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Add(newRow);
                        }
                        foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + itemRisk.ID))
                        {
                            DataRow newRow = TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                            newRow[DT_WBS_CM_Damage.ID_CM] = CMRow[DT_CounterM.ID];
                            newRow[DT_WBS_CM_Damage.DAMAGE] = item[DT_WBS_RISK_DAMAGE.DAMAGE];
                            newRow[DT_WBS_CM_Damage.ID_DAMAGE] = item[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                            newRow[DT_WBS_CM_Damage.ID_WBS] = item[DT_WBS_RISK_DAMAGE.ID_WBS];
                            newRow[DT_WBS_CM_Damage.VALUE] = 0;
                            newRow[DT_WBS_CM_Damage.WBS] = item[DT_WBS_RISK_DAMAGE.WBS];
                            TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(newRow);
                        }
                        TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Add(CMRow);
                        TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Merge(TopRiskTable);
                        TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Merge(CM_RoleTable);

                        RiskPolyLine Line_Created = new RiskPolyLine
                        {
                            ID = (int)CMRow[DT_CounterM.ID],
                            IsCM = true,
                            ShortName = "LineCreated",
                            Father = itemRisk,
                            IdRiskFather = itemRisk.ID
                        };

                        TheCurrentLayout.InsertCM(Line_Created, itemRisk, itemRisk.MyMinXPoint());
                    }

                    TheCurrentLayout.ResetGroupRiksSelection();
                    TheCurrentLayout.DropLines();
                    TheCurrentLayout.DropRectangles();
                    TheCurrentLayout.LoadLines();
                    TheCurrentLayout.LoadRectangles();
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.SetLinesThickness();

                    TextProbabilityChange(TheCurrentLayout.MainLine);
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                    CruzarTablaCM(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void ActDesGroupRiskButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool estadoActual = false;
                var query = from item in TheCurrentLayout.RiskGroupSelected
                            where (bool)item.IsActivated == true
                            select item;
                List<RiskPolyLine> result = query.ToList<RiskPolyLine>();

                if (result.Count > 0)
                {
                    // si hay al menos una "Enabled" envio true, para desactivarlas todas
                    estadoActual = true;
                }
                else
                {
                    estadoActual = false;
                }

                DisableRiskGroup(estadoActual);

                if (!estadoActual)
                {
                    VerticalMenu win = new VerticalMenu("Enabled");
                    win.ShowDialog();
                }
                else
                {
                    VerticalMenu win = new VerticalMenu("Disabled");
                    win.ShowDialog();
                }

                OnProjectChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MenuGroupRisk.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region GroupCounterMeasure_Menu
        private void ActDesGroupCMButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool estadoActual = false;
                var query = from item in TheCurrentLayout.CMGroupSelected
                            where (bool)item.IsActivated == true
                            select item;
                List<RiskPolyLine> result = query.ToList<RiskPolyLine>();

                if (result.Count > 0)
                {
                    // si hay al menos una "Enabled" envio true, para desactivarlas todas
                    estadoActual = true;
                }
                else
                {
                    estadoActual = false;
                }

                DisableCounterMeasureGroup(estadoActual);

                if (!estadoActual)
                {
                    VerticalMenu win = new VerticalMenu("Enabled");
                    win.ShowDialog();
                }
                else
                {
                    VerticalMenu win = new VerticalMenu("Disabled");
                    win.ShowDialog();
                }

                OnProjectChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MenuGroupCM.Visibility = Visibility.Hidden;
            }
        }

        private void GroupingGroupCMButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //DsMain = CurrentLayout.Ds;
                WindowGroupe wg = new WindowGroupe();
                wg.ShowDialog();
                if (wg.DialogResult == true)
                {
                    if (wg.IdGroup == -100)
                    {
                        DataRow drGroup = TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].NewRow();
                        drGroup[DT_Groupe.GROUPE_NAME] = wg.GroupName;
                        TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Add(drGroup);
                        foreach (var item in TheCurrentLayout.CMGroupSelected)
                        {
                            TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.ID_GROUPE] = drGroup[DT_Groupe.ID_GROUPE];
                            TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.GROUPE_NAME] = drGroup[DT_Groupe.GROUPE_NAME];
                        }
                        foreach (var item in TheCurrentLayout.CMGroupSelected)
                        {
                            item.Group.IdGroup = (int)drGroup[DT_Groupe.ID_GROUPE];
                            item.Group.GroupName = drGroup[DT_Groupe.GROUPE_NAME].ToString();
                        }
                    }
                    else
                    {
                        foreach (var item in TheCurrentLayout.CMGroupSelected)
                        {
                            TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.ID_GROUPE] = wg.IdGroup;
                            TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.GROUPE_NAME] = wg.GroupName;
                        }
                        foreach (var item in TheCurrentLayout.CMGroupSelected)
                        {
                            item.Group.IdGroup = wg.IdGroup;
                            item.Group.GroupName = wg.GroupName;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Standard Grouping: " + ex.Message);
            }
        }

        private void DelCMGroupCMButton_Click(object sender, RoutedEventArgs e)
        {
            //DsMain = CurrentLayout.Ds;
            //if (TheCurrentLayout.CMGroupSelected.Count > 0 && new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " selected countermeasure?").ShowDialog() == true)
            if (TheCurrentLayout.CMGroupSelected.Count > 0)
            {
                MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " selected countermeasure?");
                IS_DELETING_GROUP_CM = true;                
            }
        }

        private void DeletingCM_Group()
        {
            try
            {
                foreach (RiskPolyLine cm in TheCurrentLayout.CMGroupSelected)
                {
                    if (TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Contains(cm.ID))
                    {
                        TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cm.ID).Delete();
                    }
                }
                TheCurrentLayout.ResetGroupCMSelection();
                TheCurrentLayout.DropLines();
                TheCurrentLayout.DropRectangles();
                TheCurrentLayout.LoadLines();
                TheCurrentLayout.LoadRectangles();
                TheCurrentLayout.DrawNumbers();
                TextProbabilityChange(TheCurrentLayout.MainLine);
                TheCurrentLayout.SetLinesThickness();
                IS_DELETING_GROUP_CM = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_GROUP_CM = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void ActDesGroupMixedButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool estadoActual = false;
                var query = from item in TheCurrentLayout.RiskGroupSelected
                            where (bool)item.IsActivated == true
                            select item;
                List<RiskPolyLine> result = query.ToList<RiskPolyLine>();

                var queryCM = from item in TheCurrentLayout.CMGroupSelected
                              where (bool)item.IsActivated == true
                              select item;
                List<RiskPolyLine> resultCM = queryCM.ToList<RiskPolyLine>();

                if (result.Count > 0 || resultCM.Count > 0)
                {
                    // si hay al menos una "Enabled" envio true, para desactivarlas todas
                    estadoActual = true;
                }
                else
                {
                    estadoActual = false;
                }

                DisableRiskGroup(estadoActual);
                DisableCounterMeasureGroup(estadoActual);

                if (!estadoActual)
                {
                    VerticalMenu win = new VerticalMenu("Enabled");
                    win.ShowDialog();
                }
                else
                {
                    VerticalMenu win = new VerticalMenu("Disabled");
                    win.ShowDialog();
                }

                OnProjectChange();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MenuGroupMixed.Visibility = Visibility.Hidden;
            }
        }

        //GHT:Adicionando agrupar Mixto. Tener en cuenta que el XAML se modifico.
        /// <summary>
        /// Gruop risk, counter measure or both
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupingGroupMixedButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowGroupe wg = new WindowGroupe();
                wg.ShowDialog();
                if (wg.DialogResult == true)
                {
                    if (wg.IdGroup == -100)
                    {
                        DataRow drGroup = NewGroup(wg.GroupName);
                        GroupRiskDataTableUpdate((int)drGroup[DT_Groupe.ID_GROUPE], wg.GroupName);
                        GroupCounterMeasureDataTableUpdate((int)drGroup[DT_Groupe.ID_GROUPE], wg.GroupName);
                        GroupRiskPolyLineUpdate(TheCurrentLayout.RiskGroupSelected, (int)drGroup[DT_Groupe.ID_GROUPE], wg.GroupName);
                        GroupRiskPolyLineUpdate(TheCurrentLayout.CMGroupSelected, (int)drGroup[DT_Groupe.ID_GROUPE], wg.GroupName);
                    }
                    else
                    {
                        GroupRiskDataTableUpdate(wg.IdGroup, wg.GroupName);
                        GroupCounterMeasureDataTableUpdate(wg.IdGroup, wg.GroupName);
                        GroupRiskPolyLineUpdate(TheCurrentLayout.RiskGroupSelected, wg.IdGroup, wg.GroupName);
                        GroupRiskPolyLineUpdate(TheCurrentLayout.CMGroupSelected, wg.IdGroup, wg.GroupName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Standard Grouping: " + ex.Message);
            }
        }

        //GHT:Adicionando para agrupar Mixto
        /// <summary>
        /// Create a new Group. A group can contain risks, counter measures or both
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private DataRow NewGroup(string groupName)
        {
            DataRow drGroup = TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].NewRow();
            drGroup[DT_Groupe.GROUPE_NAME] = groupName;
            TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Add(drGroup);
            return drGroup;
        }

        //GHT:Adicionando para agrupar Mixto
        /// <summary>
        /// Update group owner for every risk selected
        /// </summary>
        /// <param name="idGroup">Group owner id</param>
        /// <param name="groupName">Group owner name</param>
        private void GroupRiskDataTableUpdate(int idGroup, string groupName)
        {
            foreach (var item in TheCurrentLayout.RiskGroupSelected)
            {
                TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.ID_GROUPE] = idGroup;
                TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.GROUPE_NAME] = groupName;
            }
        }

        //GHT:Adicionando para agrupar Mixto
        /// <summary>
        /// Update group owner for every counter measure selected
        /// </summary>
        /// <param name="idGroup">Group owner id</param>
        /// <param name="groupName">Group owner name</param>
        private void GroupCounterMeasureDataTableUpdate(int idGroup, string groupName)
        {
            foreach (var item in TheCurrentLayout.CMGroupSelected)
            {
                TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.ID_GROUPE] = idGroup;
                TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.GROUPE_NAME] = groupName;
            }
        }

        //GHT:Adicionando para agrupar Mixto
        /// <summary>
        /// Update group owner for every riskpolyline instance selected
        /// </summary>
        /// <param name="riskPolyLineToUptate">RiskPolyLine list to update</param>
        /// <param name="idGroup"></param>
        /// <param name="groupName"></param>
        private void GroupRiskPolyLineUpdate(List<RiskPolyLine> riskPolyLineToUptate, int idGroup, string groupName)
        {
            foreach (var rplToUpdate in riskPolyLineToUptate)
            {
                rplToUpdate.Group.IdGroup = idGroup;
                rplToUpdate.Group.GroupName = groupName;
            }
        }

        /// <summary>
        /// Enable or disable various counter measure
        /// </summary>
        /// <param name="linesCMSelected"></param>
        private void DisableCounterMeasureGroup(bool estadoActual)
        {
            try
            {
                foreach (var cmline in TheCurrentLayout.CMGroupSelected)
                {
                    DisableCounterMeasure(cmline, true, estadoActual);
                    //cmline.Stroke = new SolidColorBrush(Colors.LightSkyBlue);
                    cmline.SetColor(new SolidColorBrush(System.Windows.Media.Colors.LightSkyBlue));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DisableCounterMeasureGroup: " + ex.Message);
            }
        }
        /// <summary>
        /// Enable or disable various risks
        /// </summary>
        /// <param name="linesSelected"></param>
        private void DisableRiskGroup(bool estadoActual)
        {
            try
            {
                foreach (RiskPolyLine rpl in TheCurrentLayout.RiskGroupSelected)
                {
                    EnableRisk(rpl, true, estadoActual);
                    //rpl.Stroke = new SolidColorBrush(Colors.LightSkyBlue);
                    rpl.SetColor(new SolidColorBrush(System.Windows.Media.Colors.LightSkyBlue));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("DisableRiskGroup: " + ex.Message);
            }
        }
        #endregion

        #region Functions

        #region CursorsFunctions

        public void HandCursor() => Cursor = Cursors.Hand;

        public void NormalArrowCursor() => Cursor = Cursors.Arrow;

        #endregion

        #region CrossTab

        private void AjustarProbabilidadRisk(DataRow drRiskDamage, DataTable dtRisk_WBS)
        {
            try
            {
                if (dtRisk_WBS.Select(DT_RISK_WBS.ID_WBS + " = " + IdWBSFilter + " and " + DT_RISK_WBS.ID_RISK + " = " + drRiskDamage[DT_Risk_Damages.ID_RISK]).Any())
                {
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = dtRisk_WBS.Select(DT_RISK_WBS.ID_WBS + " = " + IdWBSFilter + " and " + DT_RISK_WBS.ID_RISK + " = " + drRiskDamage[DT_Risk_Damages.ID_RISK]).First()[DT_RISK_WBS.PROBABILITY];
                }
                if ((decimal)drRiskDamage[DT_Risk_Damages.PROBABILITY] == 0)
                {
                    drRiskDamage[DT_Risk_Damages.PROBABILITY] = DBNull.Value;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void AjustarProbabilidadCM(DataRow drRiskDamage, DataTable dtCM_WBS)
        {
            try
            {
                if (dtCM_WBS.Select(DT_CM_WBS.ID_WBS + " = " + IdWBSFilter + " and " + DT_CM_WBS.ID_CM + " = " + drRiskDamage[DT_CounterM_Damage.ID_COUNTERM]).Any())
                {
                    drRiskDamage[DT_CounterM_Damage.RISK_REDUCTION] = dtCM_WBS.Select(DT_CM_WBS.ID_WBS + " = " + IdWBSFilter + " and " + DT_CM_WBS.ID_CM + " = " + drRiskDamage[DT_CounterM_Damage.ID_COUNTERM]).First()[DT_CM_WBS.PROBABILITY];
                }
            }
            catch
            {
                throw new Exception("Error ajustando ProbCM");
            }
        }

        private void AjustarDamagesRisk(DataRow drRiskDamage, DataTable dtRisk_WBS_Damage)
        {
            try
            {
                if (dtRisk_WBS_Damage.Select(DT_WBS_RISK_DAMAGE.ID_WBS + " = " + IdWBSFilter + " and " + DT_WBS_RISK_DAMAGE.ID_RISK + " = " + drRiskDamage[DT_Risk_Damages.ID_RISK] + " and " + DT_WBS_RISK_DAMAGE.ID_DAMAGE + " = " + drRiskDamage[DT_Risk_Damages.ID_DAMAGE]).Any())
                {
                    drRiskDamage[DT_Risk_Damages.VALUE] = dtRisk_WBS_Damage.Select(DT_WBS_RISK_DAMAGE.ID_WBS + " = " + IdWBSFilter + " and " + DT_WBS_RISK_DAMAGE.ID_RISK + " = " + drRiskDamage[DT_Risk_Damages.ID_RISK] + " and " + DT_WBS_RISK_DAMAGE.ID_DAMAGE + " = " + drRiskDamage[DT_Risk_Damages.ID_DAMAGE]).First()[DT_WBS_RISK_DAMAGE.VALUE];
                }
                if ((decimal)drRiskDamage[DT_Risk_Damages.VALUE] == 0)
                {
                    drRiskDamage[DT_Risk_Damages.VALUE] = DBNull.Value;
                }
            }
            catch
            {
                throw new Exception("Error ajustandoDamagesRisk");
            }
        }

        private void AjustarDamagesCM(DataRow drCMDamage, DataTable dtCM_WBS_Damage)
        {
            try
            {
                if (dtCM_WBS_Damage.Select(DT_WBS_CM_Damage.ID_WBS + " = " + IdWBSFilter + " and " + DT_WBS_CM_Damage.ID_CM + " = " + drCMDamage[DT_CounterM_Damage.ID_COUNTERM] + " and " + DT_WBS_CM_Damage.ID_DAMAGE + " = " + drCMDamage[DT_CounterM_Damage.ID_DAMAGE]).Any())
                {
                    drCMDamage[DT_CounterM_Damage.VALUE] = dtCM_WBS_Damage.Select(DT_WBS_CM_Damage.ID_WBS + " = " + IdWBSFilter + " and " + DT_WBS_CM_Damage.ID_CM + " = " + drCMDamage[DT_CounterM_Damage.ID_COUNTERM] + " and " + DT_WBS_CM_Damage.ID_DAMAGE + " = " + drCMDamage[DT_CounterM_Damage.ID_DAMAGE]).First()[DT_WBS_CM_Damage.VALUE];
                }
            }
            catch 
            {
                throw new Exception("Error ajustando DmaagesCM");
            }
        }

        private void ColumnasTextoRiskReadOnly(bool flag, DataGrid dg)
        {
            try
            {
                foreach (var item in dg.Columns)
                {
                    if (item.Header.ToString() == "Risk")
                    {
                        item.IsReadOnly = !flag;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ColumnasNumerosRiskReadOnly(bool flag, DataGrid dg)
        {
            try
            {
                foreach (var item in dg.Columns)
                {
                    if (item.Header.ToString() != "Risk" && item.Header.ToString() != "WBS Name" && item.Header.ToString() != "User" && item.Header.ToString() != "Father" && item.Header.ToString() != "Active?")
                    {
                        item.IsReadOnly = !flag;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ColumnasTextoCMReadOnly(bool flag, DataGrid dg)
        {
            try
            {
                foreach (var item in dg.Columns)
                {
                    if (item.Header.ToString() == "Counter M.")
                    {
                        item.IsReadOnly = !flag;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ColumnasNumerosCMReadOnly(bool flag, DataGrid dg)
        {
            try
            {
                foreach (var item in dg.Columns)
                {
                    if (item.Header.ToString() != "Counter M." && item.Header.ToString() != "WBS Name" && item.Header.ToString() != "User" &&  item.Header.ToString() != "Risk" && item.Header.ToString() != "Status")
                    {
                        item.IsReadOnly = !flag;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CruzarTablaRisk(DataSet myDs)
        {
            try
            {
                if (TheCurrentLayout != null)
                {
                    Dt_Cross_Risk = new DataTable();
                    Dt_Cross_Risk = myDs.Tables[DT_Risk_Damages.TABLENAME].Clone();
                    
                    if (IdWBSFilter != -1)
                    {
                        if (myDs.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_WBS + " = " + IdWBSFilter).Any())
                        {
                            foreach (DataRow itemRiskWBS in myDs.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_WBS + " = " + IdWBSFilter))
                            {
                                foreach (DataRow item in myDs.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK_TREE + " = " + TheCurrentLayout.ID_Diagram + " and " + DT_Risk_Damages.ID_RISK + " = " + itemRiskWBS[DT_RISK_WBS.ID_RISK]))
                                {
                                    Dt_Cross_Risk.ImportRow(item);
                                }                                
                            }
                            foreach (DataRow rowRisk in Dt_Cross_Risk.Rows)
                            {
                                AjustarProbabilidadRisk(rowRisk, myDs.Tables[DT_RISK_WBS.TABLENAME].Copy());
                                AjustarDamagesRisk(rowRisk, myDs.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Copy());
                            }
                        }                        
                    }
                    else
                    {
                        foreach (DataRow item in myDs.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK_TREE + " = " + TheCurrentLayout.ID_Diagram))
                        {
                            Dt_Cross_Risk.ImportRow(item);
                        }
                    }
                   
                    DataTable dtTemp = Dt_Cross_Risk.Copy();
                    DataColumn[] pkCC = new DataColumn[2];

                    pkCC[0] = new DataColumn(DT_Risk_Damages.ID_RISK, typeof(int));
                    pkCC[1] = new DataColumn(DT_Risk_Damages.ID_RISK_TREE, typeof(int));

                    Dt_Cross_Risk = General.CrossTable(ref dtTemp, "Damage", new string[] { DT_Risk_Damages.VALUE }, pkCC);
                    foreach (var item in TheCurrentLayout.LinesList)
                    {
                        if (Dt_Cross_Risk.Select("idRisk = " + item.ID).Any())
                        {
                            Dt_Cross_Risk.Select("idRisk = " + item.ID).First()["nameShort"] = Dt_Cross_Risk.Select("idRisk = " + item.ID).First()["nameShort"].ToString().Insert(0, TreeOperation.Spaces(item.MyLevel));
                        }
                    }
                    Dt_Cross_Risk.AcceptChanges();
                    CleanDynamicRiskColumns(dgRisksCross);
                    dgRisksCross.AutoGenerateColumns = false;
                    AddDynamicRiskColumns();
                    TreeOperation.OrderTableHierarquical(Dt_Cross_Risk, TheCurrentLayout.LinesList, DT_Risk_Damages.ID_RISK);
                    DV_CrossRisk = new DataView(Dt_Cross_Risk);
                    //dgRisksCross.ItemsSource = DV_CrossRisk;
                    //dgRisksCross.DataContext = DV_CrossRisk;
                    FillTableGroup(myDs);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message + "- CruzandoTablaRisk");
            }
        }

        private void AddDynamicRiskColumns()
        {
            for (int i = 0; i < Dt_Cross_Risk.Columns.Count; i++)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                switch (Dt_Cross_Risk.Columns[i].ToString())
                {
                    case "probability":                       
                        break;
                    case "nameShort":
                        break;
                    case "WBSName":
                        break;
                    case "userName":
                        break;
                    case "Father":
                        break;
                    case DT_Risk_Damages.STATUS:
                        break;
                    case "GroupeName":
                        break;
                    case "idWBS":
                        break;
                    case "idRiskTree":
                        break;
                    case "IDFather":
                        break;
                    case "details":
                        break;
                    case "TopRisk":
                        break;
                    case "Color":
                        break;
                    case "RiskTree":
                        break;
                    case "isRoot":
                        break;
                    case "idTopRisk":
                        break;
                    case "idRisk":
                        break;
                    case "idGroupe":
                        break;
                    default:
                        Binding columnBinding = new Binding(Dt_Cross_Risk.Columns[i].ToString());
                        if (Dt_Cross_Risk.Columns[i].DataType.Equals(typeof(decimal)))
                        {
                            columnBinding.Converter = new DecimalUIConverter();
                            columnBinding.ConverterCulture = CultureInfo.CurrentUICulture;
                            columnBinding.ConverterParameter = decimalConfig;
                        }
                        column.Header = Dt_Cross_Risk.Columns[i].ToString();
                        column.Binding = columnBinding;
                        column.IsReadOnly = true;
                        column.MinWidth = 100;
                        dgRisksCross.Columns.Add(column);
                        break;
                }
            }

        }

        private void CleanDynamicRiskColumns(DataGrid dataGrid)
        {
            int i = 0;
            while (i < dataGrid.Columns.Count)
            {
                if (dataGrid.Columns[i].Header.ToString() != "Probability (%)" && dataGrid.Columns[i].Header.ToString() != "Risk" && dataGrid.Columns[i].Header.ToString() != "WBS Name"
                    && dataGrid.Columns[i].Header.ToString() != "User" && dataGrid.Columns[i].Header.ToString() != "Father" && dataGrid.Columns[i].Header.ToString() != "Active?")
                {
                    dataGrid.Columns.RemoveAt(i);
                    i--;
                }
                i++;
            }
        }

        public void RiskToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                //TheDataRowVieww = (DataRowView)((ToggleButton)e.Source).DataContext;
                int riskID = (int)((DataRowView)((ToggleButton)e.Source).DataContext).Row[DT_Risk.ID]; 
                TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(riskID)[DT_Risk.ENABLED] = true;
                foreach (DataRow damageRow in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + riskID))
                {
                    damageRow[DT_Risk_Damages.STATUS] = true;
                }
                System.Drawing.Color drawingCColor = System.Drawing.Color.FromArgb(int.Parse(TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram)[TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));

                (TheCurrentLayout.LinesList.Find(item => (item.ID == riskID))).SetColor(new SolidColorBrush(System.Windows.Media.Color.FromArgb(drawingCColor.A, drawingCColor.R, drawingCColor.G, drawingCColor.B)));
                (TheCurrentLayout.LinesList.Find(item => (item.ID == riskID))).IsActivated = true;

                foreach (var itemi in TheCurrentLayout.LinesList.Find(item => (item.ID == riskID)).Children)
                {
                    if (itemi.IsCM)
                    {
                        DisableCounterMeasure(itemi, true, !TheCurrentLayout.LinesListCMState[Convert.ToInt32(itemi.ID)]);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public void RiskToggleButtonUnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                int riskID = (int)((DataRowView)((ToggleButton)e.Source).DataContext).Row[DT_Risk.ID];
                TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(riskID)[DT_Risk.ENABLED] = false;
                foreach (DataRow damageRow in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + riskID))
                {
                    damageRow[DT_Risk_Damages.STATUS] = false;
                }
                (TheCurrentLayout.LinesList.Find(item => (item.ID == riskID))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                (TheCurrentLayout.LinesList.Find(item => (item.ID == riskID))).IsActivated = false;
                foreach (var itemi in TheCurrentLayout.LinesList.Find(item => (item.ID == riskID)).Children)
                {
                    if (itemi.IsCM)
                    {
                        itemi.IsActivated = false;
                        itemi.SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                        (TheCurrentLayout.LinesList.Find(item => (item.ID == itemi.ID))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                        (TheCurrentLayout.LinesList.Find(item => (item.ID == itemi.ID))).IsActivated = false;
                        TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(itemi.ID)[DT_CounterM.ENABLED] = false;
                        foreach (DataRow damageRow in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + itemi.ID))
                        {
                            damageRow[DT_CounterM_Damage.STATUS] = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public void CruzarTablaCM(DataSet myDs)
        {
            if (TheCurrentLayout != null)
            {
                Dt_Cross_CM = new DataTable();
                Dt_Cross_CM = myDs.Tables[DT_CounterM_Damage.TABLENAME].Clone();
                
                if (IdWBSFilter != -1)
                {
                    if (myDs.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_WBS + " = " + IdWBSFilter).Any())
                    {
                        foreach (DataRow itemCMWBS in myDs.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_WBS + " = " + IdWBSFilter))
                        {
                            foreach (var item in myDs.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_RISK_TREE + " = " + TheCurrentLayout.ID_Diagram + " and " + DT_CounterM_Damage.ID_COUNTERM + " = " + itemCMWBS[DT_CM_WBS.ID_CM]))
                            {
                                Dt_Cross_CM.ImportRow(item);
                            }
                        }
                    }
                    foreach (DataRow rowCM in Dt_Cross_CM.Rows)
                    {
                        AjustarProbabilidadCM(rowCM, myDs.Tables[DT_CM_WBS.TABLENAME].Copy());
                        AjustarDamagesCM(rowCM, myDs.Tables[DT_WBS_CM_Damage.TABLE_NAME].Copy());
                    }
                }
                else
                {
                    foreach (DataRow item in myDs.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_RISK_TREE + " = " + TheCurrentLayout.ID_Diagram))
                    {
                        Dt_Cross_CM.ImportRow(item);
                    }
                }
                DataTable dtTemp = Dt_Cross_CM.Copy();
                DataColumn[] pkCC = new DataColumn[2];

                pkCC[0] = new DataColumn(DT_CounterM_Damage.ID_COUNTERM, typeof(int));
                pkCC[1] = new DataColumn(DT_CounterM_Damage.ID_RISK_TREE, typeof(int));

                Dt_Cross_CM = General.CrossTable(ref dtTemp, "Damage", new string[] { DT_CounterM_Damage.VALUE }, pkCC);
                foreach (var item in TheCurrentLayout.LinesList)
                {
                    if (Dt_Cross_CM.Select("idCounterM = " + item.ID).Any())
                    {
                        Dt_Cross_CM.Select("idCounterM = " + item.ID).First()["nameShort"] = Dt_Cross_CM.Select("idCounterM = " + item.ID).First()["nameShort"].ToString().Insert(0, TreeOperation.Spaces(item.MyLevel));
                    }
                }
                Dt_Cross_CM.AcceptChanges();
                //dgCrossCM.Columns.Clear();
                CleanDynamicCMColumns(dgCrossCM);
                dgCrossCM.AutoGenerateColumns = false;
                AddDynamicCMColumns();
                TreeOperation.OrderTableHierarquical(Dt_Cross_CM, TheCurrentLayout.LinesList, DT_CounterM_Damage.ID_COUNTERM);
                DV_Cross_CM = new DataView(Dt_Cross_CM);
                //dgCrossCM.ItemsSource = DV_Cross_CM;
                
                FillTableGroup(myDs);
            }
        }

        private void AddDynamicCMColumns()
        {
            for (int i = 0; i < Dt_Cross_CM.Columns.Count; i++)
            {
                DataGridTextColumn column = new DataGridTextColumn();
                switch (Dt_Cross_CM.Columns[i].ToString())
                {
                    case DT_CounterM_Damage.RISK_REDUCTION:
                        break;
                    case DT_CounterM_Damage.COUNTERM_NAMESHORT:
                        break;
                    case DT_CounterM_Damage.WBS_NAME:
                        break;
                    case DT_CounterM_Damage.USERNAME:
                        break;
                    case DT_CounterM_Damage.RISK:
                        break;
                    case DT_CounterM_Damage.STATUS:
                        break;
                    case DT_CounterM_Damage.GROUPENAME:
                        break;
                    case "idWBS":
                        break;
                    case "idCounterM":
                        break;
                    case "idTopRisk":
                        break;
                    case "TopRisk":
                        break;
                    case "Color":
                        break;
                    case "idRiskTree":
                        break;
                    case "idRisk":
                        break;
                    case "idGroupe":
                        break;
                    default:
                        Binding columnBinding = new Binding(Dt_Cross_CM.Columns[i].ToString());
                        if (Dt_Cross_CM.Columns[i].DataType.Equals(typeof(decimal)))
                        {
                            columnBinding.Converter = new DecimalUIConverter();
                            columnBinding.ConverterCulture = CultureInfo.CurrentUICulture;
                            columnBinding.ConverterParameter = decimalConfig;
                        }
                        column.Header = Dt_Cross_CM.Columns[i].ToString();
                        column.Binding = columnBinding;
                        column.IsReadOnly = true;
                        column.MinWidth = 100;
                        dgCrossCM.Columns.Add(column);
                        break;
                }
            }

        }

        private void CleanDynamicCMColumns(DataGrid dataGrid)
        {
            int i = 0;
            while (i < dataGrid.Columns.Count)
            {
                if (dataGrid.Columns[i].Header.ToString() != "Risk Reduction(%)" && dataGrid.Columns[i].Header.ToString() != "Counter M." && dataGrid.Columns[i].Header.ToString() != "WBS Name"
                    && dataGrid.Columns[i].Header.ToString() != "User" && dataGrid.Columns[i].Header.ToString() != "Risk" && dataGrid.Columns[i].Header.ToString() != "Active?")
                {
                    dataGrid.Columns.RemoveAt(i);
                    i--;
                }
                i++;
            }
        }

        public void CMToggleButtonChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                int cmID = (int)((DataRowView)((ToggleButton)e.Source).DataContext).Row[DT_CounterM.ID];
                TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cmID)[DT_CounterM.ENABLED] = true;
                foreach (DataRow damageRow in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + cmID))
                {
                    damageRow[DT_CounterM_Damage.STATUS] = true;
                }
                TheCurrentLayout.LinesListCMState[Convert.ToInt32(cmID)] = true;
                (TheCurrentLayout.LinesList.Find(item => (item.ID == cmID && item.IsCM))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Black));

                (TheCurrentLayout.LinesList.Find(item => (item.ID == cmID && item.IsCM))).IsActivated = true;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public void CMToggleButtonUnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                int cmID = (int)((DataRowView)((ToggleButton)e.Source).DataContext).Row[DT_CounterM.ID];
                if (TheCurrentLayout.LinesList.Find(item => (item.ID == cmID && item.IsCM)).Father.IsActivated) // si el padre esta disabled no habilito cm
                {
                    TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cmID)[DT_CounterM.ENABLED] = false;
                    foreach (DataRow damageRow in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + cmID))
                    {
                        damageRow[DT_CounterM_Damage.STATUS] = false;
                    }
                    TheCurrentLayout.LinesListCMState[Convert.ToInt32(cmID)] = false;
                    (TheCurrentLayout.LinesList.Find(item => (item.ID == cmID && item.IsCM))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                    (TheCurrentLayout.LinesList.Find(item => (item.ID == cmID && item.IsCM))).IsActivated = false;
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }


        private List<LineGroup> ListaIdGroup(List<RiskPolyLine> listaRisk)
        {
            List<LineGroup> listaGroup = new List<LineGroup>();
            foreach (var item in listaRisk)
            {
                if (!(listaGroup.Exists(lg=>lg.IdGroup == item.Group.IdGroup)))
                {
                    listaGroup.Add(item.Group);
                }
            }
            return listaGroup;
        }

        public void FillTableGroup(DataSet myDs)
        {
            if (TheCurrentLayout != null)
            {
                DsGroupRisk = new DataSet();
                //var query = CurrentLayout.LinesList.GroupBy(x => x.Group.IdGroup);
                List<LineGroup> query = ListaIdGroup(TheCurrentLayout.LinesList);
                TreeViewGroup.Items.Clear();
                //GroupService.WebServiceGroupe ws = new GroupService.WebServiceGroupe();
                //myDs.Tables[DT_Groupe.TABLE_NAME].Merge(ws.GetAllGroupes().Tables[DT_Groupe.TABLE_NAME]);
                //ws.Dispose();
                if (query != null)
                {
                    foreach (LineGroup item in query)
                    {
                        if (item.IdGroup != 0)
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add(new DataColumn("ID", typeof(int)));
                            dt.Columns.Add(new DataColumn("Element", typeof(string)));
                            dt.Columns.Add(new DataColumn("Name", typeof(string)));
                            dt.Columns.Add(new DataColumn("Father", typeof(string)));
                            dt.Columns.Add(new DataColumn("Activated", typeof(bool)));
                            dt.Columns.Add(new DataColumn("Probability", typeof(decimal)));
                            dt.TableName = myDs.Tables[DT_Groupe.TABLE_NAME].Rows.Find(item.IdGroup)[DT_Groupe.GROUPE_NAME].ToString();

                            foreach (var ite in TheCurrentLayout.LinesList.Where(x => x.Group.IdGroup == item.IdGroup))
                            {
                                DataRow dr = dt.NewRow();
                                dr["Element"] = ite.IsCM ? "CounterMeasure" : "Risk";
                                dr["Name"] = ite.ShortName;
                                dr["ID"] = ite.ID;
                                dr["Probability"] = ite.Probability;
                                dr["Activated"] = ite.IsEnabled;
                                if (ite.Father != null)
                                {
                                    dr["Father"] = ite.Father.ShortName;
                                }
                                else { dr["Father"] = "Is Root!"; }

                                dt.Rows.Add(dr);
                            }

                            DsGroupRisk.Tables.Add(dt);
                            TreeViewItem tItem = new TreeViewItem
                            {
                                IsExpanded = true
                            };

                            DataGrid dg = new DataGrid
                            {
                                AutoGenerateColumns = false,
                                CanUserAddRows = false,
                                CanUserReorderColumns = false,
                                CanUserSortColumns = false,
                                CanUserResizeRows = false
                            };
                            Style style = new Style(typeof(Button), ((Button)FindResource("DeleteRow")).Style);
                            FrameworkElementFactory img = new FrameworkElementFactory(typeof(Image));
                            img.SetValue(HeightProperty, 25d);
                            img.SetValue(Image.SourceProperty, new BitmapImage(new Uri(CLEAR)));

                            var btnRemovee = new FrameworkElementFactory(typeof(MyGroupButton));
                            btnRemovee.SetValue(MyGroupButton.StyleProperty, style);
                            btnRemovee.SetValue(HeightProperty, 25d);
                            btnRemovee.SetValue(WidthProperty, 25d);
                            btnRemovee.SetValue(ToolTipProperty, "Delete Row");
                            btnRemovee.AddHandler(MyGroupButton.ClickEvent, new RoutedEventHandler(RemoveSomeHandler));
                            btnRemovee.AppendChild(img);

                            DataGridTemplateColumn tmp = new DataGridTemplateColumn
                            {
                                Header = ""
                            };

                            DataTemplate template = new DataTemplate() { VisualTree = btnRemovee };
                            tmp.CellTemplate = template;
                            dg.Columns.Insert(0, tmp);
                            Style styleText = new Style();
                            Setter set1 = new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
                            Setter set2 = new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left);
                            styleText.Setters.Add(set1);
                            styleText.Setters.Add(set2);
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                DataGridTextColumn column = new DataGridTextColumn
                                {
                                    Header = dt.Columns[i].ToString(),
                                    ElementStyle = styleText
                                };
                                Binding columnBinding = new Binding(dt.Columns[i].ToString());
                                if (dt.Columns[i].DataType.Equals(typeof(decimal)))
                                {
                                    columnBinding.Converter = new DecimalUIConverter();
                                    columnBinding.ConverterCulture = CultureInfo.CurrentUICulture;
                                    columnBinding.ConverterParameter = decimalConfig;
                                }
                                column.Binding = columnBinding;
                                column.IsReadOnly = true;
                                column.Visibility = (i > 3) ? Visibility.Hidden : Visibility.Visible;
                                if (i == 0)
                                {
                                    column.Visibility = Visibility.Hidden;
                                }
                                dg.Columns.Add(column);
                            }
                            dg.ItemsSource = dt.DefaultView;
                            dg.CanUserDeleteRows = false;
                            //((DataView)dg.ItemsSource).Table.RowDeleted += Table_RowDeleting;

                            StackPanel panel = new StackPanel();

                            MyGroupButton btnDetails = new MyGroupButton
                            {
                                Style = ((Button)FindResource("Delete")).Style,
                                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.Eye },
                                ToolTip = "Details",
                                //Name = "BD" + item.Key.ToString(),
                                IdGroup = (int)item.IdGroup
                            };
                            btnDetails.Click += BtnDetails_OnClick;
                            MyGroupButton btnAddRisk = new MyGroupButton
                            {
                                Style = ((Button)FindResource("Delete")).Style,
                                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.Plus },
                                ToolTip = "Add More",
                                //Name = "BA" + item.Key.ToString(),
                                IdGroup = (int)item.IdGroup
                            };
                            btnAddRisk.Click += BtnAddRisk_Click;
                            MyGroupButton btnRemove = new MyGroupButton
                            {
                                Style = ((Button)FindResource("Delete")).Style,
                                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.DeleteForeverOutline },
                                ToolTip = "Remove the filter",
                                //Name = "B" + item.Key.ToString(),
                                IdGroup = (int)item.IdGroup
                            };
                            btnRemove.Click += Remove_Group_Click;

                            MyGroupButton btnRemoveGroup = new MyGroupButton
                            {
                                Style = ((Button)FindResource("Delete")).Style,
                                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.DeleteForever },
                                ToolTip = "Remove the group (delete from database)",
                                //Name = "B" + item.Key.ToString(),
                                IdGroup = (int)item.IdGroup
                            };
                            btnRemoveGroup.Click += BtnRemoveGroup_Click;

                            MyGroupButton btnChangeName = new MyGroupButton
                            {
                                Style = ((Button)FindResource("Delete")).Style,
                                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.Pencil },
                                ToolTip = "Rename Group",
                                //Name = "BN" + item.Key.ToString(),
                                IdGroup = (int)item.IdGroup
                            };

                            btnChangeName.Click += ChangeGoupName_Click;
                            MyGroupButton btnEnableDisable = new MyGroupButton
                            {
                                Style = ((Button)FindResource("Delete")).Style,
                                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.ToggleSwitch },
                                ToolTip = "Enable/Disable",
                                IdGroup = (int)item.IdGroup
                            };
                            btnEnableDisable.Click += BtnEnableDisable_Click;

                            panel.Children.Add(new TextBlock { Text = dt.TableName, VerticalAlignment = VerticalAlignment.Center });
                            panel.Children.Add(new TextBlock { Text = " " });
                            panel.Children.Add(btnAddRisk);
                            panel.Children.Add(new TextBlock { Text = " " });
                            panel.Children.Add(btnDetails);
                            panel.Children.Add(new TextBlock { Text = " " });
                            panel.Children.Add(btnChangeName);
                            panel.Children.Add(new TextBlock { Text = " " });
                            panel.Children.Add(btnRemove);
                            panel.Children.Add(new TextBlock { Text = " " });
                            panel.Children.Add(btnRemoveGroup);
                            panel.Children.Add(new TextBlock { Text = " " });
                            panel.Children.Add(btnEnableDisable);
                            panel.Orientation = Orientation.Horizontal;
                            //tItem.ContextMenu = MenuRemoveFromGroup;
                            tItem.Header = panel;
                            tItem.Items.Add(dg);
                            //GHT:Mostrar miembros del grupo seleccionado en color de seleccion
                            tItem.GotFocus += TreeViewGroupItem_GotFocus;
                            TreeViewGroup.Items.Add(tItem);
                        }
                    }
                }
            }
        }

        private void BtnRemoveGroup_Click(object sender, RoutedEventArgs e)
        {
            ID_Groupe = ((MyGroupButton)sender).IdGroup;
            IS_REMOVING_GROUP = true;
            MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " this Group?");
        }

        #region TreeViewGroupEvents
        private void BtnAddRisk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Hand;
                TheCurrentLayout.SelectingToGroup = true;
                TheCurrentLayout.GroupSelected = new LineGroup()
                {
                    IdGroup = ((MyGroupButton)sender).IdGroup,
                    GroupName = TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(((MyGroupButton)sender).IdGroup)[DT_Groupe.GROUPE_NAME].ToString()
                };
                FillTableGroup(TheCurrentLayout.Ds);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void RemoveGroupFilter(int IDGroupe)
        {
            try
            {
                foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_GROUPE + " = " + IDGroupe))
                {
                    item[DT_Risk.ID_GROUPE] = DBNull.Value;
                    item[DT_Risk.GROUPE_NAME] = "None";
                }
                foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_GROUPE + " = " + IDGroupe))
                {
                    item[DT_CounterM.ID_GROUPE] = DBNull.Value;
                    item[DT_CounterM.GROUPE_NAME] = "None";
                }
                foreach (var item in TheCurrentLayout.LinesList.FindAll(x => x.Group.IdGroup == IDGroupe))
                {
                    item.Group.IdGroup = 0;
                    item.Group.GroupName = "None";
                }
                TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(IDGroupe).Delete();
                FillTableGroup(TheCurrentLayout.Ds);
                IS_REMOVING_GROUP_FILTER = false;
            }
            catch (Exception ex)
            {
                IS_REMOVING_GROUP_FILTER = false;
                MostrarErrorDialog(ex.Message);
            }            
        }

        private void RemoveGroup(int IdGroup)
        {
            try
            {
                RemoveGroupFilter(IdGroup);
                if (TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Contains(IdGroup))
                {
                    TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(IdGroup).Delete();
                }
                IS_REMOVING_GROUP = false;
            }
            catch (Exception ex)
            {
                IS_REMOVING_GROUP = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Remove_Group_Click(object sender, RoutedEventArgs e)
        {
            ID_Groupe = ((MyGroupButton)sender).IdGroup;
            IS_REMOVING_GROUP_FILTER = true;
            MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " this Group?");            
        }

        private void ChangeGoupName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowRenameGroup wgrp = new WindowRenameGroup()
                {
                    DrGroup = TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(((MyGroupButton)sender).IdGroup)
                };
                if (wgrp.ShowDialog() == true)
                {
                    TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(((MyGroupButton)sender).IdGroup)[DT_Groupe.GROUPE_NAME] = wgrp.DrGroup[DT_Groupe.GROUPE_NAME];
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public void TreeViewGroupItem_GotFocus(Object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem selectedTreeViewItem)
            {
                SelectGroupRiskAndCM(selectedTreeViewItem);
            }
        }

        private void SelectGroupRiskAndCM(TreeViewItem selectedTreeViewItem)
        {
            try
            {
                TheCurrentLayout.ResetGroupRiksSelection();
                TheCurrentLayout.ResetGroupCMSelection();
                if (selectedTreeViewItem.Items.Count > 0)
                {
                    DataView dv = (DataView)((DataGrid)(selectedTreeViewItem.Items[0])).ItemsSource;
                    if (UserHasPermissionOnThisGroup(dv.Table.Rows))
                    {
                        if (dv.Table.Select("Element = 'CounterMeasure'").Any())
                        {
                            TheCurrentLayout.ChoosingCM = true;
                        }
                        if (dv.Table.Select("Element = 'Risk'").Any())
                        {
                            TheCurrentLayout.ChoosingRisk = true;
                        }
                        foreach (DataRow elementDataRow in dv.Table.Rows)
                        {
                            RiskPolyLine CurrentRiskPolyLine = TheCurrentLayout.LinesList.Find(rpl => rpl.ID == (int)elementDataRow["ID"]);
                            CurrentRiskPolyLine.SetColor(new SolidColorBrush(System.Windows.Media.Colors.LightSkyBlue));
                            if (CurrentRiskPolyLine.IsCM)
                            {
                                TheCurrentLayout.CMGroupSelected.Add(CurrentRiskPolyLine);
                            }
                            else
                            {
                                TheCurrentLayout.RiskGroupSelected.Add(CurrentRiskPolyLine);
                            }
                        }
                        if (TheCurrentLayout.ChoosingRisk && TheCurrentLayout.ChoosingCM)
                        {
                            TheCurrentLayout.ResetLinesMenu(TheCurrentLayout.RiskGroupSelected, MenuGroupMixed);
                            TheCurrentLayout.ResetLinesMenu(TheCurrentLayout.CMGroupSelected, MenuGroupMixed);
                        }
                        else
                        {
                            if (TheCurrentLayout.ChoosingRisk)
                            {
                                TheCurrentLayout.ResetLinesMenu(TheCurrentLayout.RiskGroupSelected, MenuGroupRisk);
                            }
                            else
                            {
                                if (TheCurrentLayout.ChoosingCM)
                                {
                                    TheCurrentLayout.ResetLinesMenu(TheCurrentLayout.CMGroupSelected, MenuGroupCM);
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

        /// <summary>
        /// True if the user has permission on every risk else false
        /// </summary>
        private bool UserHasPermissionOnThisGroup(DataRowCollection rows)
        {
            bool haspermission = false;
            foreach (DataRow elementDataRow in rows)
            {
                switch (elementDataRow["Element"].ToString())
                {
                    case "CounterMeasure":
                        haspermission = UserHasPermisionOnThisCM((int)elementDataRow["ID"]);
                        break;
                    case "Risk":
                        haspermission = UserHasPermisionOnThisRisk((int)elementDataRow["ID"]);
                        break;
                }
                if (!haspermission)
                {
                    break;
                }
            }
            return haspermission;
        }

        private bool UserHasPermisionOnThisRisk(int elementID)
        {
            bool haspermission = false;
            DataRow[] dr = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + elementID.ToString());
            foreach (DataRow item in dr)
            {
                if (TheCurrentLayout.Ds.Tables[DT_User_Role.TABLE_NAME].Select(DT_User_Role.ROLE + " = '" + item[DT_Role_Risk.Role] + "' and " + DT_User_Role.USER + " = '" + LoginUser + "'").Any())
                {
                    haspermission = true;
                    break;
                }
            }
            return haspermission;
        }

        private bool UserHasPermisionOnThisCM(int elementID)
        {
            bool haspermission = false;
            DataRow[] dr = TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + elementID.ToString());
            foreach (DataRow item in dr)
            {
                if (TheCurrentLayout.Ds.Tables[DT_User_Role.TABLE_NAME].Select(DT_User_Role.ROLE + " = '" + item[DT_Role_CM.Role] + "' and " + DT_User_Role.USER + " = '" + LoginUser + "'").Any())
                {
                    haspermission = true;
                    break;
                }
            }
            return haspermission;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (TheCurrentLayout != null)
            {
                if (e.Key == Key.Escape)
                {
                    if (TheCurrentLayout.ChoosingRisk && !TheCurrentLayout.ChoosingCM)
                    {
                        TheCurrentLayout.ResetGroupRiksSelection();
                    }
                    else if (!TheCurrentLayout.ChoosingRisk && TheCurrentLayout.ChoosingCM)
                    {
                        TheCurrentLayout.ResetGroupCMSelection();
                    }
                    else if (TheCurrentLayout.ChoosingRisk && TheCurrentLayout.ChoosingCM)
                    {
                        TheCurrentLayout.ResetGroupRiksSelection();
                        TheCurrentLayout.ResetGroupCMSelection();
                    }
                    else
                    {
                        // clic en el area amarilla sin seleccion multiple
                    }
                }
                if (e.Key == Key.F2)
                {
                    TheCurrentLayout.BrigIntoViewSelectedRiskPolyline(TheCurrentLayout.Line_Selected);
                    TheCurrentLayout.UpdateSelectedPolyLineVisualInfo();
                    TheCurrentLayout.EditSelectedPolyLineShorName();
                }
            }
        }

        //GHT: Mostrar/ocultar detalle
        private void ViewGroupDetail(ObservableCollection<DataGridColumn> dataGrigColumns, Visibility isVisible)
        {
            for (int i = 4; i < dataGrigColumns.Count; i++)
            {
                DataGridColumn column = dataGrigColumns[i];
                column.Visibility = isVisible;
            }
        }

        private void BtnDetails_OnClick(Object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn)
                {
                    if (btn.Content is MaterialDesignThemes.Wpf.PackIcon btnContent)
                    {
                        if (btnContent.Kind == MaterialDesignThemes.Wpf.PackIconKind.Eye)
                        {
                            if (btn.Parent is StackPanel selectedStackPanel)
                            {
                                if (selectedStackPanel.Parent is TreeViewItem selectedTreeViewItem)
                                {
                                    ViewGroupDetail(((DataGrid)(selectedTreeViewItem.Items[0])).Columns, Visibility.Visible);
                                }
                            }
                            btnContent.Kind = MaterialDesignThemes.Wpf.PackIconKind.EyeOff;
                        }
                        else
                        {
                            if (btn.Parent is StackPanel selectedStackPanel)
                            {
                                if (selectedStackPanel.Parent is TreeViewItem selectedTreeViewItem)
                                {
                                    ViewGroupDetail(((DataGrid)(selectedTreeViewItem.Items[0])).Columns, Visibility.Hidden);
                                }
                            }
                            btnContent.Kind = MaterialDesignThemes.Wpf.PackIconKind.Eye;
                        }
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void BtnEnableDisable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ActDesGroupMixedButton_Click(sender, e);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
        #endregion
        private void RemoveSomeHandler(DataRowView row)
        {
            try
            {
                if (row != null)
                {
                    if (row[1].ToString() == "Risk")
                    {
                        foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID + " = " + row[0] + " and "
                                                                                    + DT_Risk.ID_DIAGRAM + " = " + TheCurrentLayout.ID_Diagram))
                        {
                            item[DT_Risk.GROUPE_NAME] = "None";
                            item[DT_Risk.ID_GROUPE] = DBNull.Value;
                            TheCurrentLayout.LinesList.Find(x => x.ID == (int)item[DT_Risk.ID]).Group.IdGroup = 0;
                            TheCurrentLayout.LinesList.Find(x => x.ID == (int)item[DT_Risk.ID]).Group.GroupName = "None";
                        }
                    }
                    else
                    {
                        foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID + " = " + row[0] + " and "
                                                                                    + DT_CounterM.ID_RISK_TREE + " = " + TheCurrentLayout.ID_Diagram))
                        {
                            item[DT_CounterM.GROUPE_NAME] = "None";
                            item[DT_CounterM.ID_GROUPE] = DBNull.Value;
                            TheCurrentLayout.LinesList.Find(x => x.ID == (int)item[DT_CounterM.ID]).Group.IdGroup = 0;
                            TheCurrentLayout.LinesList.Find(x => x.ID == (int)item[DT_CounterM.ID]).Group.GroupName = "None";
                        }
                    }
                }
                FillTableGroup(TheCurrentLayout.Ds);
                IS_DELETING_GROUP_TAB = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_GROUP_TAB = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        public void RemoveSomeHandler(object sender, RoutedEventArgs e)
        {
            try
            {
                TheDataRowVieww = (DataRowView)((MyGroupButton)e.Source).DataContext;
                IS_DELETING_GROUP_TAB = true;
                MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " this value?");
            }
            catch (Exception ex)
            {
                IS_DELETING_GROUP_TAB = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void TableRowDeleting(DataTable senderTable)
        {
            try
            {
                foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.GROUPE_NAME + " = '" + senderTable.TableName + "' and "
                                                                                 + DT_CounterM.ID_RISK_TREE + " = " + TheCurrentLayout.ID_Diagram))
                {
                    if (!(senderTable.Select("ID = " + item[DT_CounterM.ID] + " and Element = 'CounterMeasure'").Any()))
                    {
                        item[DT_CounterM.GROUPE_NAME] = "None";
                        item[DT_CounterM.ID_GROUPE] = DBNull.Value;
                        TheCurrentLayout.LinesList.Find(x => x.ID == (int)item[DT_CounterM.ID]).Group.IdGroup = 0;
                        TheCurrentLayout.LinesList.Find(x => x.ID == (int)item[DT_CounterM.ID]).Group.GroupName = "None";
                    }

                }
                foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.GROUPE_NAME + " = '" + senderTable.TableName + "' and "
                                                                                + DT_Risk.ID_DIAGRAM + " = " + TheCurrentLayout.ID_Diagram))
                {
                    if (!(senderTable.Select("ID = " + item[DT_Risk.ID] + " and Element = 'Risk'").Any()))
                    {
                        item[DT_Risk.GROUPE_NAME] = "None";
                        item[DT_Risk.ID_GROUPE] = DBNull.Value;
                        TheCurrentLayout.LinesList.Find(x => x.ID == (int)item[DT_Risk.ID]).Group.IdGroup = 0;
                        TheCurrentLayout.LinesList.Find(x => x.ID == (int)item[DT_Risk.ID]).Group.GroupName = "None";
                    }
                }
                IS_REMOVING_TABLE_ROW = false;
            }
            catch (Exception ex)
            {
                IS_REMOVING_TABLE_ROW = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Table_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (sender is DataTable)
            {
                TheRemovingTable = (DataTable)sender;
                IS_REMOVING_TABLE_ROW = true;
                MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " this value?");
            }
        }

        #endregion

        #region Paste&PasteNewFunction
        private void FillRiskDamages(DataSet dsSource, DataSet dsResult, RiskPolyLine itemRisk, RiskPolyLine riskFather, DataRow drRisk, int idDiagram)
        {
            foreach (DataRow itemi in dsSource.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + itemRisk.ID))
            {
                DataRow top = dsResult.Tables[DT_Risk_Damages.TABLENAME].NewRow();
                top[DT_Risk_Damages.COLOR] = itemi[DT_Risk_Damages.COLOR];
                top[DT_Risk_Damages.DAMAGE] = itemi[DT_Risk_Damages.DAMAGE];
                top[DT_Risk_Damages.FATHER] = riskFather.ShortName;
                top[DT_Risk_Damages.GROUPE_NAME] = drRisk[DT_Risk.GROUPE_NAME];
                top[DT_Risk_Damages.ID_FATHER] = riskFather.ID;
                top[DT_Risk_Damages.ID_GROUPE] = drRisk[DT_Risk.ID_GROUPE];
                top[DT_Risk_Damages.ID_RISK] = drRisk[DT_Risk.ID];
                top[DT_Risk_Damages.ID_RISK_TREE] = idDiagram;
                top[DT_Risk_Damages.IS_ROOT] = drRisk[DT_Risk.IS_ROOT];
                top[DT_Risk_Damages.PROBABILITY] = drRisk[DT_Risk.PROBABILITY];
                top[DT_Risk_Damages.RISK_NAMESHORT] = drRisk[DT_Risk.NAMESHORT];
                top[DT_Risk_Damages.RISK_TREE] = dsResult.Tables[DT_Diagram.TABLE_NAME].Rows.Find(idDiagram)[DT_Diagram.DIAGRAM_NAME];
                top[DT_Risk_Damages.STATUS] = drRisk[DT_Risk.ENABLED];
                top[DT_Risk_Damages.ID_DAMAGE] = itemi[DT_Risk_Damages.ID_DAMAGE];
                top[DT_Risk_Damages.VALUE] = itemi[DT_Risk_Damages.VALUE];
                top[DT_Risk_Damages.TOP_RISK] = itemi[DT_Risk_Damages.TOP_RISK];
                top[DT_Risk_Damages.ID_WBS] = itemi[DT_Risk_Damages.ID_WBS];
                top[DT_Risk_Damages.WBS_NAME] = itemi[DT_Risk_Damages.WBS_NAME];
                top[DT_Risk_Damages.USERNAME] = itemi[DT_Risk_Damages.USERNAME];
                dsResult.Tables[DT_Risk_Damages.TABLENAME].Rows.Add(top);
            }
        }

        private void FillRiskWBS(DataSet dsSource, DataSet dsResult, RiskPolyLine item, DataRow drRisk)
        {
            foreach (DataRow itemi in dsSource.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + item.ID))
            {
                DataRow rwbs = dsResult.Tables[DT_RISK_WBS.TABLENAME].NewRow();
                rwbs[DT_RISK_WBS.ID_RISK] = drRisk[DT_Risk.ID];
                rwbs[DT_RISK_WBS.ID_WBS] = itemi[DT_RISK_WBS.ID_WBS];
                rwbs[DT_RISK_WBS.NIVEL] = itemi[DT_RISK_WBS.NIVEL];
                rwbs[DT_RISK_WBS.RISK] = drRisk[DT_Risk.NAMESHORT];
                rwbs[DT_RISK_WBS.WBS] = itemi[DT_RISK_WBS.WBS];
                rwbs[DT_RISK_WBS.IS_PRIMARY] = itemi[DT_RISK_WBS.IS_PRIMARY];
                rwbs[DT_RISK_WBS.PRIMARY] = itemi[DT_RISK_WBS.PRIMARY];
                rwbs[DT_RISK_WBS.USERNAME] = itemi[DT_RISK_WBS.USERNAME];
                rwbs[DT_RISK_WBS.PROBABILITY] = itemi[DT_RISK_WBS.PROBABILITY];
                dsResult.Tables[DT_RISK_WBS.TABLENAME].Rows.Add(rwbs);
            }
            foreach (DataRow itemi in dsSource.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + item.ID))
            {
                DataRow rwbs = dsResult.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                rwbs[DT_WBS_RISK_DAMAGE.ID_RISK] = drRisk[DT_Risk.ID];
                rwbs[DT_WBS_RISK_DAMAGE.ID_WBS] = itemi[DT_WBS_RISK_DAMAGE.ID_WBS];
                rwbs[DT_WBS_RISK_DAMAGE.WBS] = itemi[DT_WBS_RISK_DAMAGE.WBS];
                rwbs[DT_WBS_RISK_DAMAGE.VALUE] = itemi[DT_WBS_RISK_DAMAGE.VALUE];
                rwbs[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = itemi[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                rwbs[DT_WBS_RISK_DAMAGE.DAMAGE] = itemi[DT_WBS_RISK_DAMAGE.DAMAGE];
                dsResult.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(rwbs);
            }
        }

        private void FillRiskRole(DataSet dsSource, DataSet dsResult, RiskPolyLine risk, DataRow drRisk, RiskPolyLine item)
        {
            foreach (DataRow itemo in dsSource.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + risk.ID))
            {
                if (!(dsResult.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], itemo[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow newRow = dsResult.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = itemo[DT_Role_Risk.IDROL_COLUMN];
                    newRow[DT_Role_Risk.Role] = itemo[DT_Role_Risk.Role];
                    dsResult.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                }
            }
            //for each role that original have 
            foreach (DataRow itemo in dsSource.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + item.ID))
            {
                if (!(dsResult.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], itemo[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow newRow = dsResult.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_Role_Risk.Role] = itemo[DT_Role_Risk.Role];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = itemo[DT_Role_Risk.IDROL_COLUMN];
                    dsResult.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                }
            }
        }

        private void FillCMDamages(DataSet dsSource, DataSet dsResult, RiskPolyLine risk, DataRow drCM, RiskPolyLine item, int idDiagram)
        {
            foreach (DataRow itemi in dsSource.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + item.ID))
            {
                DataRow top = dsResult.Tables[DT_CounterM_Damage.TABLENAME].NewRow();
                top[DT_CounterM_Damage.COLOR] = itemi[DT_CounterM_Damage.COLOR];
                top[DT_CounterM_Damage.ID_DAMAGE] = itemi[DT_CounterM_Damage.ID_DAMAGE];
                top[DT_CounterM_Damage.ID_COUNTERM] = drCM[DT_CounterM.ID];
                top[DT_CounterM_Damage.VALUE] = itemi[DT_CounterM_Damage.VALUE];
                top[DT_CounterM_Damage.TOP_RISK] = itemi[DT_CounterM_Damage.TOP_RISK];
                top[DT_CounterM_Damage.COUNTERM_NAMESHORT] = drCM[DT_CounterM.NAMESHORT];
                top[DT_CounterM_Damage.DAMAGE] = itemi[DT_CounterM_Damage.DAMAGE];
                top[DT_CounterM_Damage.GROUPENAME] = itemi[DT_CounterM_Damage.GROUPENAME];
                top[DT_CounterM_Damage.IDRISK] = drCM[DT_CounterM.ID_RISK];
                top[DT_CounterM_Damage.ID_GROUPE] = itemi[DT_CounterM_Damage.ID_GROUPE];
                top[DT_CounterM_Damage.ID_RISK_TREE] = idDiagram;
                top[DT_CounterM_Damage.RISK] = risk.ShortName;
                top[DT_CounterM_Damage.RISK_REDUCTION] = drCM[DT_CounterM.PROBABILITY];
                top[DT_CounterM_Damage.STATUS] = itemi[DT_CounterM_Damage.STATUS];
                top[DT_CounterM_Damage.ID_WBS] = itemi[DT_CounterM_Damage.ID_WBS];
                top[DT_CounterM_Damage.WBS_NAME] = itemi[DT_CounterM_Damage.WBS_NAME];
                top[DT_CounterM_Damage.USERNAME] = itemi[DT_CounterM_Damage.USERNAME];
                dsResult.Tables[DT_CounterM_Damage.TABLENAME].Rows.Add(top);
            }
        }

        private void FillCMWBS(DataSet dsSource, DataSet dsResult, RiskPolyLine item, DataRow drCM)
        {
            foreach (DataRow itemi in dsSource.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + item.ID))
            {
                DataRow rwbs = dsResult.Tables[DT_CM_WBS.TABLENAME].NewRow();
                rwbs[DT_CM_WBS.ID_CM] = drCM[DT_CounterM.ID];
                rwbs[DT_CM_WBS.ID_WBS] = itemi[DT_CM_WBS.ID_WBS];
                rwbs[DT_CM_WBS.NIVEL] = itemi[DT_CM_WBS.NIVEL];
                rwbs[DT_CM_WBS.CM] = drCM[DT_CounterM.NAMESHORT];
                rwbs[DT_CM_WBS.WBS] = itemi[DT_CM_WBS.WBS];
                rwbs[DT_CM_WBS.IS_PRIMARY] = itemi[DT_CM_WBS.IS_PRIMARY];
                rwbs[DT_CM_WBS.PRIMARY] = itemi[DT_CM_WBS.PRIMARY];
                rwbs[DT_CM_WBS.USERNAME] = itemi[DT_CM_WBS.USERNAME];
                rwbs[DT_CM_WBS.CM] = itemi[DT_CM_WBS.CM];
                rwbs[DT_CM_WBS.PROBABILITY] = itemi[DT_CM_WBS.PROBABILITY];
                dsResult.Tables[DT_CM_WBS.TABLENAME].Rows.Add(rwbs);
            }
            foreach (DataRow itemi in dsSource.Tables[DT_WBS_CM_Damage.TABLE_NAME].Select(DT_WBS_CM_Damage.ID_CM + " = " + item.ID))
            {
                DataRow rwbs = dsResult.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                rwbs[DT_WBS_CM_Damage.ID_CM] = drCM[DT_CounterM.ID];
                rwbs[DT_WBS_CM_Damage.ID_WBS] = itemi[DT_WBS_CM_Damage.ID_WBS];
                rwbs[DT_WBS_CM_Damage.ID_DAMAGE] = itemi[DT_WBS_CM_Damage.ID_DAMAGE];
                rwbs[DT_WBS_CM_Damage.DAMAGE] = itemi[DT_WBS_CM_Damage.DAMAGE];
                rwbs[DT_WBS_CM_Damage.VALUE] = itemi[DT_WBS_CM_Damage.VALUE];
                rwbs[DT_WBS_CM_Damage.WBS] = itemi[DT_WBS_CM_Damage.WBS];
                dsResult.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(rwbs);
            }
        }

        private void FillCMRoles(DataSet dsSource, DataSet dsResult, RiskPolyLine risk, DataRow drCM, RiskPolyLine item)
        {
            //foreach role that its father have
            foreach (DataRow itemo in dsSource.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + risk.ID))
            {
                if (!(dsResult.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], itemo[DT_Role_Risk.IDROL_COLUMN] })))
                {
                    DataRow newRow = dsResult.Tables[DT_Role_CM.TABLENAME].NewRow();
                    newRow[DT_Role_CM.ID_CM] = drCM[DT_CounterM.ID];
                    newRow[DT_Role_CM.Role] = itemo[DT_Role_CM.Role];
                    newRow[DT_Role_CM.IDROL_COLUMN] = itemo[DT_Role_CM.IDROL_COLUMN];
                    dsResult.Tables[DT_Role_CM.TABLENAME].Rows.Add(newRow);
                }
            }
            //for each role that original have 
            foreach (DataRow itemo in dsSource.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + item.ID))
            {
                if (!(dsResult.Tables[DT_Role_CM.TABLENAME].Rows.Contains(new object[] { drCM[DT_CounterM.ID], itemo[DT_Role_CM.IDROL_COLUMN] })))
                {
                    DataRow newRow = dsResult.Tables[DT_Role_CM.TABLENAME].NewRow();
                    newRow[DT_Role_CM.ID_CM] = drCM[DT_CounterM.ID];
                    newRow[DT_Role_CM.Role] = itemo[DT_Role_CM.Role];
                    newRow[DT_Role_CM.IDROL_COLUMN] = itemo[DT_Role_CM.IDROL_COLUMN];
                    dsResult.Tables[DT_Role_CM.TABLENAME].Rows.Add(newRow);
                }
            }
        }

        private void Paste(RiskPolyLine risk, DataSet dsSource, DataSet dsResult, int idDiagram)
        {
            try
            {
                foreach (var item in risk.Children)
                {
                    if (!(item.IsCM))
                    {
                        DataRow drRisk = dsResult.Tables[DT_Risk.TABLE_NAME].NewRow();
                        drRisk[DT_Risk.COMMENTS] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.COMMENTS];
                        drRisk[DT_Risk.ENABLED] = true;
                        drRisk[DT_Risk.FROM_TOP] = item.FromTop;
                        drRisk[DT_Risk.ID_DIAGRAM] = idDiagram;
                        drRisk[DT_Risk.ISCOLLAPSED] = item.Collapsed;
                        drRisk[DT_Risk.IS_ROOT] = false;
                        drRisk[DT_Risk.NAMESHORT] = item.ShortName;
                        drRisk[DT_Risk.POSITION] = item.Position;
                        drRisk[DT_Risk.PROBABILITY] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.PROBABILITY];
                        drRisk[DT_Risk.IDRISK_FATHER] = risk.ID;
                        drRisk[DT_Risk.ID_WBS] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.ID_WBS];
                        drRisk[DT_Risk.WBS_NAME] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.WBS_NAME];
                        drRisk[DT_Risk.USER_NAME] = dsSource.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.USER_NAME];
                        dsResult.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);
                        DataRow rowstructure = dsResult.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                        rowstructure[DT_RiskStructure.IDRISK] = drRisk[DT_Risk.ID];
                        rowstructure[DT_RiskStructure.IDRISK_FATHER] = risk.ID;
                        dsResult.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);
                        FillRiskDamages(dsSource, dsResult, item, risk, drRisk, idDiagram);
                        FillRiskWBS(dsSource, dsResult, item, drRisk);
                        FillRiskRole(dsSource, dsResult, risk, drRisk, item);
                        if (item.IsLeaf())
                        {
                            item.ID = (int)drRisk[DT_Risk.ID];
                            //polyLines.Add(item);
                        }
                        else
                        {
                            foreach (var item2 in item.Children)
                            {
                                item2.IdRiskFather = (int)drRisk[DT_Risk.ID];
                            }
                            item.ID = (int)drRisk[DT_Risk.ID];
                            //polyLines.Add(item);
                            Paste(item, dsSource, dsResult, idDiagram);
                        }
                    }
                    else
                    {
                        DataRow drCM = dsResult.Tables[DT_CounterM.TABLE_NAME].NewRow();
                        drCM[DT_CounterM.DETAIL] = dsSource.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.DETAIL];
                        drCM[DT_CounterM.ENABLED] = true;
                        drCM[DT_CounterM.FROM_TOP] = item.FromTop;
                        drCM[DT_CounterM.ID_RISK_TREE] = idDiagram;
                        drCM[DT_CounterM.ID_RISK] = risk.ID;
                        drCM[DT_CounterM.NAMESHORT] = item.ShortName;
                        drCM[DT_CounterM.POSITION] = item.Position;
                        drCM[DT_CounterM.PROBABILITY] = dsSource.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.PROBABILITY];
                        drCM[DT_CounterM.RISK_NAMESHORT] = item.ShortName;
                        drCM[DT_CounterM.ID_WBS] = dsSource.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.ID_WBS];
                        drCM[DT_CounterM.WBS_NAME] = dsSource.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.WBS_NAME];
                        drCM[DT_CounterM.USER_NAME] = dsSource.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.USER_NAME];
                        dsResult.Tables[DT_CounterM.TABLE_NAME].Rows.Add(drCM);
                        FillCMDamages(dsSource, dsResult, risk, drCM, item, idDiagram);
                        FillCMWBS(dsSource, dsResult, item, drCM);
                        FillCMRoles(dsSource, dsResult, risk, drCM, item);
                        item.ID = (int)drCM[DT_CounterM.ID];
                        //polyLines.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        #endregion


        public void DrawEntireDiagram()
        {
            try
            {
                if (TheCurrentLayout.Ds.HasChanges())
                {
                    Cursor = Cursors.Arrow;
                    if (TheCurrentLayout.ID_Diagram != 0)
                    {
                        TheCurrentLayout.DropLines();
                        TheCurrentLayout.DropRectangles();
                        TheCurrentLayout.LoadLines();
                        TheCurrentLayout.LoadRectangles();
                        TheCurrentLayout.DrawNumbers();
                        TheCurrentLayout.SetLinesThickness();
                        TextProbabilityChange(TheCurrentLayout.MainLine);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public void TextProbabilityChange(RiskPolyLine line)
        {
            //TextProbability.Text = General.MyRound(General.AcumulatedLikelihood(line) * 100, 2).ToString() + " %";
        }

        public void UpdateServerAccess()
        {            
            DataTable tableConnecion = new DataTable();
            tableConnecion.ReadXml(CONNECTION);
            General.UpdateConfigurationFile(tableConnecion.Select().First()["Server"].ToString());
            tableConnecion.Dispose();
        }

        /// <summary>
        ///    Refresh the Datargids
        /// </summary>
        private void RefreshData()
        {
            try
            {
                UpdateServerAccess();
                DsMain = new UserDataSet();
                ServiceRiskController.WebServiceRisk risk = new ServiceRiskController.WebServiceRisk();
                DsMain.Merge(risk.GetRiskTreeString(new object[] { "%", IdProject }));
                //UPDATE THE TABLE OF TREES AND ITS RESPECTIVE RISK OF EACH ONE
                //DV ITS THE DATAVIEW OF TREE RISK
                //dvRisk its the dataview of Risk table
                risk.Dispose();
                DVRisk_Tree = DsMain.Tables[DT_Diagram.TABLE_NAME].DefaultView;
                dgTreeDiagrams.ItemsSource = DVRisk_Tree;
                dgTreeDiagrams.SelectedIndex = Indexx;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// Export to  bitmap, jpeg or png the elements contained in paint diagram
        /// </summary>
        private void ExportToImage()
        {
            try
            {
                System.Windows.Forms.SaveFileDialog sf = new System.Windows.Forms.SaveFileDialog
                {
                    Filter = "Png Image (.png)|*.png"
                };

                if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ExportToPng(sf.FileName, TheCurrentLayout.GridPaintLines);
                }
                sf.Dispose();
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public void ExportToPng(string path, FrameworkElement element)
        {
            if (path == null) return;

            // Save current canvas transform
            Transform transform = element.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            element.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(element.Width, element.Height);
            // Measure and arrange the surface
            // VERY IMPORTANT
            element.Measure(size);
            element.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 91, 96, PixelFormats.Default);
            renderBitmap.Render(element);

            // Create a file stream for saving image

            using (FileStream outStream = new FileStream(path, FileMode.Create))
            {
                // Use png encoder for our data
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                // push the rendered bitmap to it
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                encoder.Save(outStream);
            }
            // Restore previously saved layout
            element.LayoutTransform = transform;
        }

        public bool ExistRole(int number)
        {
            int num = 0;
            num = AccessList.Find(bk => bk == number);
            return num != 0;
        }

        /// <summary>
        /// Validate the access of an existing user in tha application, just for the functions, ex: Add or edit risk, etc.
        /// </summary>
        public void ValidateAccess()
        {
            try
            {
                MenuItemUserList.IsEnabled = ExistRole(1);//
                MenuItemRoleList.IsEnabled = ExistRole(2);//
                AddTree.IsEnabled = ExistRole(3);
                ImportExcel.IsEnabled = ExistRole(3);
                EditTree.IsEnabled = ExistRole(4);
                RemoveTree.IsEnabled = ExistRole(5);
                MenuItemNewProject.IsEnabled = ExistRole(7);
                MenuItemDefaultRisk.IsEnabled = ExistRole(8);
                MenuItemTopRisk.IsEnabled = ExistRole(10);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// Values for languages
        /// </summary>
        public void ChangeLanguage()
        {
            try
            {
                LoginMenuItem.Header = StringResources.LoginMenu;
                MenuItemExport.ToolTip = StringResources.EXPORT_IMAGE;
                MenuItemPrint.ToolTip = StringResources.PRINT_DIAGRAM;
                MenuItemUserList.Header = StringResources.UserListMenu;
                MenuItemRoleList.Header = StringResources.RoleListMenu;
                MenuItemTopRisk.Header = StringResources.TopRiskList;
                MenuItemOption.Header = StringResources.SettingsMenu;
                MenuItemExit.Header = StringResources.ExitMenu;
                //LabelDiagram.Content = StringResources.DiagramNameLabel;
                //LabelProbability.Content = StringResources.ExpectedLKLabel;
                //cbDiagramValues.Header = StringResources.DiagramValuesGroupB;
                ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.AddRisk]).ToolTip = StringResources.AddRiskMenu;
                ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.AddCM]).ToolTip = StringResources.AddCMMenu;
                ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.Scope]).ToolTip = StringResources.MENU_SCOPE;
                ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.Copy]).ToolTip = StringResources.COPY_MENU;
                ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.Paste]).ToolTip = StringResources.PASTE_MENU;
                ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.Import]).ToolTip = StringResources.MENU_IMPORT;
                ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.Enable]).ToolTip = StringResources.DisableValue;

                ((MenuItem)MenuRiskLimited.Items[(int)MRLimited.EditRisk]).ToolTip = StringResources.EditRiskMenu;
                ((MenuItem)MenuRiskLimited.Items[(int)MRLimited.Scope]).ToolTip = StringResources.MENU_SCOPE;
                ((MenuItem)MenuRiskLimited.Items[(int)MRLimited.Copy]).ToolTip = StringResources.COPY_MENU;
                ((MenuItem)MenuRiskLimited.Items[(int)MRLimited.Enable]).ToolTip = StringResources.DisableValue;

                ((MenuItem)MenuCM.Items[(int)MenuCMm.Edit]).ToolTip = StringResources.EditCMMenu;
                ((MenuItem)MenuCM.Items[(int)MenuCMm.Delete]).ToolTip = StringResources.DeleteCMMenu;
                ((MenuItem)MenuCM.Items[(int)MenuCMm.Move]).ToolTip = StringResources.MoveCMMenu;
                ((MenuItem)MenuCM.Items[(int)MenuCMm.Enable]).ToolTip = StringResources.DisableValue;

                ((MenuItem)MenuMainRisk.Items[(int)MenuMain.AddRisk]).ToolTip = StringResources.AddRiskMenu;
                ((MenuItem)MenuMainRisk.Items[(int)MenuMain.Paste]).ToolTip = StringResources.PASTE_MENU;
                ((MenuItem)MenuMainRisk.Items[(int)MenuMain.Import]).ToolTip = StringResources.MENU_IMPORT;
                //BtnBackward.ToolTip = StringResources.GoBackTooltip;
                StartMenu.ToolTip = StringResources.StartTooltip;
                MenuItemFish_Tree.ToolTip = StringResources.FISH_MENU_TOOLTIP;
                MenuItemReload.ToolTip = StringResources.REBOOT;
                if (Flag_login)
                {
                    Title = "Ensure Risk | " + StringResources.CONNECTED_STRING + " " + LoginUser;
                    LoginMenuItem.Header = StringResources.LOGOFF;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        private void HabilitarBotones(bool sino)
        {
            if (sino)
            {
                Cursor = Cursors.Arrow;
            }
            else
            {
                Cursor = Cursors.No;
            }
            Supergrid.IsEnabled = sino;
        }

        /// <summary>
        /// Save the data in Database
        /// </summary>
        public void SaveData(DataSet ds, bool cartel)
        {
            try
            {
                if (ds.HasChanges())
                {
                    ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk();
                    DataSet temp = ds.GetChanges();
                    temp = ws.SaveRisk(temp);
                    ds.Merge(temp);
                    ds.AcceptChanges();
                    ws.Dispose();
                    RefreshData();
                    Cursor = Cursors.Arrow;
                    if (cartel)
                    {
                        MostrarInfoDialog("Data saved successfully!!!");
                    }
                    IS_SAVING_DATA = false;
                }
            }
            catch (Exception ex)
            {
                IS_SAVING_DATA = false;
                MostrarErrorDialog(ex.Message);
            }

        }

        private static Rect GetPrintableArea(PrintDialog printDialog)
        {
            PrintCapabilities cap;
            try
            {
                cap = printDialog.PrintQueue.GetPrintCapabilities(printDialog.PrintTicket);
            }
            catch (PrintQueueException)
            {
                return Rect.Empty;
            }

            if (cap.PageImageableArea == null)
            {
                return Rect.Empty;
            }

            var leftMargin = cap.OrientedPageMediaWidth.HasValue ? (cap.OrientedPageMediaWidth.Value - cap.PageImageableArea.ExtentWidth) / 2 : 0;
            var topMargin = cap.OrientedPageMediaHeight.HasValue ? (cap.OrientedPageMediaHeight.Value - cap.PageImageableArea.ExtentHeight) / 2 : 0;
            var width = cap.PageImageableArea.ExtentWidth;
            var height = cap.PageImageableArea.ExtentHeight;
            return new Rect(leftMargin, topMargin, width, height);
        }

        #region VerticalTree
        /// <summary>
        /// Build a diagram as a tree
        /// </summary>
        //private TreeNodeModel<Node> Build_Tree(List<Node> data)
        //{
        //    var root = data.FirstOrDefault(p => p.ParentId == 0);
        //    var rootTreeNode = new TreeNodeModel<Node>(root, null);

        //    // add tree node children recursively
        //    rootTreeNode.Children = GetChildNodes(data, rootTreeNode);

        //    return rootTreeNode;
        //}

        private void DrawNode(TreeNodeModel<Node> node, System.Windows.Media.Color color)
        {
            //var nodeRect = new Node(GridPaintLines, new Point((NODE_MARGIN_X + (node.X * (NODE_WIDTH + NODE_MARGIN_X))),
            //    NODE_MARGIN_Y + (node.Y * (NODE_HEIGHT + NODE_MARGIN_Y))), node.Item.Name, node.Item.Value.ToString(), color);
            Point location = new Point((NODE_MARGIN_X + (node.X * (NODE_WIDTH + NODE_MARGIN_X))), NODE_MARGIN_Y + (node.Y * (NODE_HEIGHT + NODE_MARGIN_Y)));

            node.Item.Margin = new Thickness(location.X, location.Y, 0, 0);
            node.Item.MidBottomPoint = new Point(node.Item.Margin.Left + (node.Item.Width / 2), node.Item.Margin.Top + node.Item.Height);

            node.Item.MidTopPoint = new Point(node.Item.Margin.Left + (node.Item.Width / 2), node.Item.Margin.Top);

            if (node.Father != null)
            {
                //var nodeTopMiddle = new Point(nodeRect.Margin.Left + (nodeRect.Width / 2), nodeRect.Margin.Top);
                if (node.Item.IsCM)
                {
                    node.Item.LineaArriba.StrokeDashArray = new DoubleCollection { 5, 2 };
                    node.Item.LineaArriba.Stroke = new SolidColorBrush(System.Windows.Media.Colors.Black);
                }
                else
                {
                    node.Item.LineaArriba.Stroke = new SolidColorBrush(color);
                }

                node.Item.LineaArriba.StrokeThickness = 2;
                node.Item.LineaArriba.ArrowEnds = ArrowEnds.None;

                node.Item.LineaArriba.X2 = node.Item.Padre.MidBottomPoint.X;
                node.Item.LineaArriba.Y2 = node.Item.Padre.MidBottomPoint.Y;
                node.Item.LineaArriba.X1 = node.Item.MidTopPoint.X;
                node.Item.LineaArriba.Y1 = node.Item.MidTopPoint.Y;
            }
            TheCurrentLayout.GridPaintLines.Children.Add(node.Item.LineaArriba);
            Nodos.Add(node.Item);

            foreach (var item in node.Children)
            {
                DrawNode(item, color);
            }
        }

        private static List<TreeNodeModel<Node>> GetChildNodes(List<Node> data, TreeNodeModel<Node> parent)
        {
            var nodes = new List<TreeNodeModel<Node>>();

            foreach (var item in data.Where(p => p.ParentId == parent.Item.ID))
            {
                var treeNode = new TreeNodeModel<Node>(item, parent);
                item.Padre = parent.Item;
                treeNode.Children = GetChildNodes(data, treeNode);
                nodes.Add(treeNode);
            }

            return nodes;
        }

        #endregion

        #endregion

        #region AppEvents

        private void CbProjects_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                IdProject = (int)cbProjects.SelectedValue;
                RefreshData();
                RefreshWBS();
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// Change the language of the App
        /// </summary>
        private void CbLanguage_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                Thread.CurrentThread.CurrentUICulture = new CultureInfo((string)cbLanguage.SelectedValue);
                Thread.CurrentThread.CurrentCulture = new CultureInfo((string)cbLanguage.SelectedValue);
                ChangeLanguage();
                switch (cbLanguage.Text)
                {
                    case "Español":
                        ImageLanguage.Source = new BitmapImage(new Uri(General.ESPAÑOL));
                        break;
                    case "Nederlands":
                        ImageLanguage.Source = new BitmapImage(new Uri(General.NETHERLANDS));
                        break;
                    default:
                        ImageLanguage.Source = new BitmapImage(new Uri(General.ENGLISH));
                        break;
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }



        #region DataGridEvents

        /// <summary>
        /// Occurs when user chage selection in Diagram´s Table
        /// </summary>
        private void DgTreeDiagrams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dgTreeDiagrams.SelectedIndex >= 0)
                {
                    Indexx = dgTreeDiagrams.SelectedIndex;
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// User can edit a diagram double clicking the diagram (Open in drawing)
        /// </summary>
        private void DgTreeDiagrams_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (EditTree.IsEnabled)
            {
                EditTree_Click(sender, e);
            }
        }

        #region DgRisk_&_DgCM_Events
        private bool? CanUseProposedPolyLineName(string proposedPolyLineName)
        {
            bool? result = null;
            WindowMessageYesNo yesNo = null;
            if (!(TheCurrentLayout.Line_Selected.IsCM))
            {
                //Es un riesgo
                int RiskTreeID = (int)TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_Risk.ID_DIAGRAM];

                if (TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + RiskTreeID + " and "
                    + DT_Risk.NAMESHORT + " = '" + proposedPolyLineName + "' and " + DT_Risk.ID + " <> " + TheCurrentLayout.Line_Selected.ID).Any())
                {
                    yesNo = new WindowMessageYesNo("The name [" + proposedPolyLineName + "] Already exists in this diagram. Do you want to use it again?");
                    yesNo.ShowDialog();
                }
            }
            else
            {
                //Es una contramedida
                int RiskTreeID = (int)TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.IdRiskFather)[DT_Risk.ID_DIAGRAM];

                if (TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK_TREE + " = " + RiskTreeID + " and "
                    + DT_CounterM.NAMESHORT + " = '" + proposedPolyLineName + "' and " + DT_CounterM.ID + " <> " + TheCurrentLayout.Line_Selected.ID).Any())
                {
                    yesNo = new WindowMessageYesNo("The name [" + proposedPolyLineName + "] Already exists in this diagram. Do you want to use it again?");
                    yesNo.ShowDialog();
                }
            }
            if (yesNo != null)
            {
                result = yesNo.DialogResult;
            }
            return result;
        }

        private void DgRisksCross_LostFocus(object sender, RoutedEventArgs e)
        {
            SalvarDatosRiskCross();
            if (TheCurrentLayout != null)
            {
                TheCurrentLayout.RiskLeave();
            }
        }

        private void DgRisksCross_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                SalvarDatosRiskCross();                
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message + "Cambiando Celda");
            }
        }

        private void DgRisksCross_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null)
                {
                    SalvarDatosRiskCross();
                    TheCurrentLayout.RiskLeave();
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void SalvarDatosCMCross()
        {
            try
            {
                if (Dt_Cross_CM != null && Dt_Cross_CM.GetChanges() != null)
                {
                    DataTable dtTemp = Dt_Cross_CM.GetChanges();
                    foreach (DataRow item in dtTemp.Rows)
                    {
                        if (item["nameShort"].ToString() != string.Empty)
                        {
                            if (item["RiskReduction"] == DBNull.Value || item["RiskReduction"].ToString() == "" || item["RiskReduction"].ToString() == "-")
                            {
                                item["RiskReduction"] = 0;
                            }
                            if ((decimal)item["RiskReduction"] <= 100 && (decimal)item["RiskReduction"] >= 0)
                            {
                                decimal tempRiskRed = (decimal)TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item[DT_CounterM_Damage.ID_COUNTERM])[DT_CounterM.PROBABILITY];

                                TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item[DT_CounterM_Damage.ID_COUNTERM])[DT_CounterM.NAMESHORT] = item["nameShort"].ToString().TrimStart();
                                if (IdWBSFilter != -1)
                                {
                                    if (TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Contains(new object[] { item[DT_CounterM_Damage.ID_COUNTERM], IdWBSFilter }))
                                    {
                                        TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows
                                            .Find(new object[] { item[DT_CounterM_Damage.ID_COUNTERM], IdWBSFilter })[DT_CM_WBS.PROBABILITY] = ((decimal)item["RiskReduction"]);
                                    }
                                    DataRow[] drsWBS_CM = TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + item[DT_CounterM_Damage.ID_COUNTERM]);
                                    List<decimal> Probabilities = new List<decimal>();
                                    foreach (DataRow rowWBS_CM in drsWBS_CM)
                                    {
                                        if (WBS_isSheet((int)rowWBS_CM[DT_CM_WBS.ID_WBS]))
                                        {
                                            if (rowWBS_CM[DT_CM_WBS.PROBABILITY] == DBNull.Value)
                                            {
                                                Probabilities.Add(100);
                                            }
                                            else
                                            {
                                                Probabilities.Add((decimal)rowWBS_CM[DT_CM_WBS.PROBABILITY]);
                                            }
                                        }
                                    }
                                    if (Probabilities.Count > 0)
                                    {
                                        tempRiskRed = Probabilities.Sum() / Probabilities.Count;
                                    }
                                }
                                TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item[DT_CounterM_Damage.ID_COUNTERM])[DT_CounterM.PROBABILITY] = tempRiskRed;
                                TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Find(new object[] { item[DT_CounterM_Damage.ID_COUNTERM], item[DT_CounterM_Damage.ID_DAMAGE] })[DT_CounterM_Damage.COUNTERM_NAMESHORT] = item["nameShort"].ToString().TrimStart();
                                TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Find(new object[] { item[DT_CounterM_Damage.ID_COUNTERM], item[DT_CounterM_Damage.ID_DAMAGE] })[DT_CounterM_Damage.RISK_REDUCTION] = tempRiskRed;
                                foreach (DataColumn itemi in dtTemp.Columns)
                                {
                                    if (TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + item[DT_CounterM_Damage.ID_COUNTERM] + " and " + DT_CounterM_Damage.DAMAGE + " = '" + itemi.ToString() + "'").Any())
                                    {
                                        if (item[itemi] == DBNull.Value || item[itemi].ToString() == "" || item[itemi].ToString() == "-")
                                        {
                                            item[itemi] = 0;
                                        }
                                        if ((decimal)item[itemi] >= 0)
                                        {
                                            decimal tempValue = (decimal)item[itemi];
                                            if (TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Select(DT_WBS_CM_Damage.ID_CM + " = " + item[DT_CounterM_Damage.ID_COUNTERM] +
                                                    " and " + DT_WBS_CM_Damage.DAMAGE + " = '" + itemi.ToString() + "' and " + DT_WBS_CM_Damage.ID_WBS + " = " + IdWBSFilter).Any())
                                            {
                                                TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Select(DT_WBS_CM_Damage.ID_CM + " = " + item[DT_CounterM_Damage.ID_COUNTERM] +
                                                    " and " + DT_WBS_CM_Damage.DAMAGE + " = '" + itemi.ToString() + "' and " + DT_WBS_CM_Damage.ID_WBS + " = " + IdWBSFilter).First()[DT_WBS_CM_Damage.VALUE] = (decimal)item[itemi];
                                            }
                                            tempValue = 0;
                                            foreach (DataRow rowCM_WBS_DAMAGE in TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Select(DT_WBS_CM_Damage.ID_CM + " = " + item[DT_CounterM_Damage.ID_COUNTERM] +
                                                    " and " + DT_WBS_CM_Damage.DAMAGE + " = '" + itemi.ToString() + "'"))
                                            {
                                                if (WBS_isSheet((int)rowCM_WBS_DAMAGE[DT_WBS_CM_Damage.ID_WBS]))
                                                {
                                                    tempValue += (decimal)rowCM_WBS_DAMAGE[DT_WBS_CM_Damage.VALUE];
                                                }
                                            }
                                            TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].
                                                Select(DT_CounterM_Damage.ID_COUNTERM + " = " + item[DT_CounterM_Damage.ID_COUNTERM] +
                                                " and " + DT_CounterM_Damage.DAMAGE + " = '" + itemi.ToString() + "'").First()[DT_CounterM_Damage.VALUE] = tempValue;
                                        }
                                        else
                                        {
                                            MostrarErrorDialog("Damages Values can't be less than 0. Check again");
                                        }
                                    }
                                }
                            }
                            else
                            {
                                MostrarErrorDialog("CM Risk Reduction Value can't be less than 0 or higher to 100. No changes apply");
                            }
                        }
                        else
                        {
                            MostrarErrorDialog("CM Name Value can´t be empty. No changes apply");
                        }
                    }
                    Dt_Cross_CM.AcceptChanges();
                    DrawEntireDiagram();
                    TheCurrentLayout.ClearFilters();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                    CruzarTablaCM(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }

        }

        private void SalvarDatosRiskCross()
        {
            try
            {
                if (Dt_Cross_Risk != null && Dt_Cross_Risk.GetChanges() != null)
                {
                    DataTable dtTemp = Dt_Cross_Risk.GetChanges();
                    foreach (DataRow item in dtTemp.Rows)
                    {
                        if (item["nameShort"].ToString() != string.Empty)
                        {
                            if (item["probability"] == DBNull.Value || item["probability"].ToString() == "" || item["probability"].ToString() == "-")
                            {
                                item["probability"] = 0;
                            }
                            if ((decimal)item["probability"] <= 100 && (decimal)item["probability"] >= 0)
                            {
                                if (TheCurrentLayout != null)
                                {
                                    decimal tempProbability = (decimal)TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item[DT_Risk_Damages.ID_RISK])[DT_Risk.PROBABILITY];

                                    TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item[DT_Risk_Damages.ID_RISK])[DT_Risk.NAMESHORT] = item["nameShort"].ToString().TrimStart();
                                    //Filtrando los Risk-WBS para establecer la probabilidad
                                    if (IdWBSFilter != -1)
                                    {
                                        if (TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Contains(new object[] { item[DT_Risk_Damages.ID_RISK], IdWBSFilter }))
                                        {
                                            TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows
                                                .Find(new object[] { item[DT_Risk_Damages.ID_RISK], IdWBSFilter })[DT_RISK_WBS.PROBABILITY] = ((decimal)item["probability"]);
                                        }
                                        DataRow[] drsWBS_RISK = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + item[DT_Risk_Damages.ID_RISK]);
                                        List<decimal> Probabilities = new List<decimal>();
                                        foreach (DataRow rowWBS_RISK in drsWBS_RISK)
                                        {
                                            if (WBS_isSheet((int)rowWBS_RISK[DT_RISK_WBS.ID_WBS]))
                                            {
                                                if (item[DT_RISK_WBS.PROBABILITY] == DBNull.Value)
                                                {
                                                    Probabilities.Add(100);
                                                }
                                                else
                                                {
                                                    Probabilities.Add((decimal)rowWBS_RISK[DT_RISK_WBS.PROBABILITY]);
                                                }
                                            }
                                        }
                                        if (Probabilities.Count > 0)
                                        {
                                            tempProbability = Probabilities.Sum() / Probabilities.Count;
                                        }
                                    }
                                    TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item[DT_Risk_Damages.ID_RISK])[DT_Risk.PROBABILITY] = tempProbability;
                                    TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Find(new object[] { item[DT_Risk_Damages.ID_RISK], item[DT_Risk_Damages.ID_DAMAGE] })[DT_Risk_Damages.RISK_NAMESHORT] = item["nameShort"].ToString().TrimStart();
                                    TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Find(new object[] { item[DT_Risk_Damages.ID_RISK], item[DT_Risk_Damages.ID_DAMAGE] })[DT_Risk_Damages.PROBABILITY] = tempProbability;
                                    
                                    foreach (DataColumn itemi in dtTemp.Columns)
                                    {
                                        if (TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + item[DT_Risk_Damages.ID_RISK] + " and " + DT_Risk_Damages.DAMAGE + " = '" + itemi.ToString() + "'").Any())
                                        {
                                            if (item[itemi] == DBNull.Value || item[itemi].ToString() == "" || item[itemi].ToString() == "-")
                                            {
                                                item[itemi] = 0;
                                            }
                                            if ((decimal)item[itemi] >= 0)
                                            {
                                                decimal tempValue = (decimal)item[itemi];
                                                if (TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + item[DT_Risk_Damages.ID_RISK] +
                                                    " and " + DT_WBS_RISK_DAMAGE.DAMAGE + " = '" + itemi.ToString() + "' and " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + IdWBSFilter).Any())
                                                {
                                                    TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + item[DT_Risk_Damages.ID_RISK] +
                                                    " and " + DT_WBS_RISK_DAMAGE.DAMAGE + " = '" + itemi.ToString() + "' and " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + IdWBSFilter).First()[DT_WBS_RISK_DAMAGE.VALUE] = (decimal)item[itemi];
                                                }
                                                tempValue = 0;
                                                foreach (DataRow rowRISK_WBS_DAMAGE in TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + item[DT_Risk_Damages.ID_RISK] +
                                                    " and " + DT_WBS_RISK_DAMAGE.DAMAGE + " = '" + itemi.ToString() + "'"))
                                                {
                                                    if (WBS_isSheet((int)rowRISK_WBS_DAMAGE[DT_WBS_RISK_DAMAGE.ID_WBS]))
                                                    {
                                                        tempValue += (decimal)rowRISK_WBS_DAMAGE[DT_WBS_RISK_DAMAGE.VALUE];
                                                    }
                                                }
                                                TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].
                                                    Select(DT_Risk_Damages.ID_RISK + " = " + item[DT_Risk_Damages.ID_RISK] +
                                                    " and " + DT_Risk_Damages.DAMAGE + " = '" + itemi.ToString() + "'").First()[DT_Risk_Damages.VALUE] = tempValue;
                                            }
                                            else
                                            {
                                                CruzarTablaRisk(TheCurrentLayout.Ds);
                                                MostrarErrorDialog("Damages Values can't be less than 0. Check again");
                                            }
                                        }
                                    }
                                }                               
                            }
                            else
                            {
                                CruzarTablaRisk(TheCurrentLayout.Ds);
                                MostrarErrorDialog("Risk Probability Value can't be less than 0 or higher to 100. No changes apply");
                            }
                        }
                        else
                        {
                            CruzarTablaRisk(TheCurrentLayout.Ds);
                            MostrarErrorDialog("Risk Name Value can´t be empty. No changes apply");
                        }
                    }
                    Dt_Cross_Risk.AcceptChanges();
                    DrawEntireDiagram();
                    TheCurrentLayout.ClearFilters();
                    CruzarTablaRisk(TheCurrentLayout.Ds);
                    CruzarTablaCM(TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message + " Ajustando Datos");
            }
        }

        private void DgCrossCM_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
               
                if (TheCurrentLayout != null)
                {
                    SalvarDatosCMCross();
                    TheCurrentLayout.CMLeave(TheCurrentLayout.Line_Selected);
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }           
        }

        private void DgCrossCM_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            SalvarDatosCMCross();
        }

        private void DgCrossCM_LostFocus(object sender, RoutedEventArgs e)
        {
            SalvarDatosCMCross();
            TheCurrentLayout.CMLeave(TheCurrentLayout.Line_Selected);
        }

        private bool UsuarioUsaWBSRisk(int idRisk)
        {
            return TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.USERNAME + " = '" + LoginUser + "'" + " and " + DT_RISK_WBS.ID_WBS + " = " + IdWBSFilter + " and " + DT_RISK_WBS.ID_RISK + " = " + idRisk).Any();
            
        }

        private bool UsuarioUsaWBSCM(int idCM)
        {
            return TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.USERNAME + " = '" + LoginUser + "'" + " and " + DT_CM_WBS.ID_WBS + " = " + IdWBSFilter + " and " + DT_CM_WBS.ID_CM + " = " + idCM).Any();

        }

        private void DgRisksCross_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is DataGrid riskDataGrid && riskDataGrid.SelectedItem != null)
                {
                    if (riskDataGrid.SelectedItem is DataRowView)
                    {
                        if (((DataRowView)riskDataGrid.SelectedItem).Row[DT_Risk.ID] is int rowRiskID)
                        {
                            TheCurrentLayout.Line_Selected = TheCurrentLayout.LinesList.Find(item => (item.ID == rowRiskID));
                            TheCurrentLayout.RiskLeave();                            
                            TheCurrentLayout.BrigIntoViewSelectedRiskPolyline(TheCurrentLayout.Line_Selected);
                            TheCurrentLayout.RiskEnter(TheCurrentLayout.Line_Selected, TheCurrentLayout.Line_Selected.Points[TheCurrentLayout.Line_Selected.Points.Count - 1], false);
                            TheCurrentLayout.UpdateSelectedPolyLineVisualInfo();
                            bool access = TheCurrentLayout.FullAccess(TheCurrentLayout.Line_Selected);
                            ColumnasTextoRiskReadOnly(access, riskDataGrid);
                            if (IdWBSFilter != -1 && UsuarioUsaWBSRisk(rowRiskID))
                            {
                                ColumnasNumerosRiskReadOnly(true, riskDataGrid);
                            }
                            else
                            {
                                ColumnasNumerosRiskReadOnly(false, riskDataGrid);
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

        private void DgCrossCM_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is DataGrid cmDataGrid && cmDataGrid.SelectedItem != null)
                {
                    if (cmDataGrid.SelectedItem is DataRowView)
                    {
                        TheCurrentLayout.Line_Selected = TheCurrentLayout.LinesList.Find(item => (item.ID == (int)((DataRowView)cmDataGrid.SelectedItem).Row[DT_CounterM.ID]));
                        TheCurrentLayout.CMLeave(TheCurrentLayout.Line_Selected);
                        TheCurrentLayout.BrigIntoViewSelectedRiskPolyline(TheCurrentLayout.Line_Selected);
                        TheCurrentLayout.CMEnter(TheCurrentLayout.Line_Selected, TheCurrentLayout.Line_Selected.Points[TheCurrentLayout.Line_Selected.Points.Count - 1], false);

                        TheCurrentLayout.UpdateSelectedPolyLineVisualInfo();
                        bool access = TheCurrentLayout.FullAccess(TheCurrentLayout.Line_Selected);
                        ColumnasTextoCMReadOnly(access, cmDataGrid);
                        if (IdWBSFilter != -1 && UsuarioUsaWBSCM((int)((DataRowView)cmDataGrid.SelectedItem).Row[DT_CounterM.ID]))
                        {
                            ColumnasNumerosCMReadOnly(true, cmDataGrid);
                        }
                        else
                        {
                            ColumnasNumerosCMReadOnly(false, cmDataGrid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DgRisksCross_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction == DataGridEditAction.Commit && e.Column is DataGridTextColumn columnEdited)
                {
                    Binding b = columnEdited.Binding as Binding;
                    if (b.Path.Path == "nameShort")
                    {
                        bool? canUsePolyLineName = CanUseProposedPolyLineName((e.EditingElement as TextBox).Text);
                        if (canUsePolyLineName.HasValue && !canUsePolyLineName.Value)
                        {
                            (sender as DataGrid).CancelEdit(DataGridEditingUnit.Cell);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void DgCrossCM_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                if (e.EditAction == DataGridEditAction.Commit && e.Column is DataGridTextColumn columnEdited)
                {
                    Binding b = columnEdited.Binding as Binding;
                    if (b.Path.Path == "nameShort")
                    {
                        bool? canUsePolyLineName = CanUseProposedPolyLineName((e.EditingElement as TextBox).Text);
                        if (canUsePolyLineName.HasValue && !canUsePolyLineName.Value)
                        {
                            (sender as DataGrid).CancelEdit(DataGridEditingUnit.Cell);
                        }
                    }
                }
                if (e.EditAction == DataGridEditAction.Commit && e.Column is DataGridCheckBoxColumn chkcolumn)
                {
                    Binding b = chkcolumn.Binding as Binding;
                    bool? checkeddd = (e.EditingElement as CheckBox).IsChecked;
                    if (b.Path.Path == "Enabled")
                    {
                        if (sender is DataGrid cmDataGrid && cmDataGrid.SelectedItem != null)
                        {
                            int cmID = (int)((DataRowView)cmDataGrid.SelectedItem).Row[DT_CounterM.ID];
                            if (checkeddd.HasValue && checkeddd.Value)
                            {
                                TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cmID)[DT_CounterM.ENABLED] = false;
                                foreach (DataRow damageRow in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + cmID))
                                {
                                    damageRow[DT_CounterM_Damage.STATUS] = false;
                                }
                                TheCurrentLayout.LinesListCMState[Convert.ToInt32(cmID)] = false;
                                (TheCurrentLayout.LinesList.Find(item => (item.ID == cmID && item.IsCM))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                                (TheCurrentLayout.LinesList.Find(item => (item.ID == cmID && item.IsCM))).IsActivated = false;
                            }
                            else
                            {
                                if (TheCurrentLayout.LinesList.Find(item => (item.ID == cmID && item.IsCM)).Father.IsActivated) // si el padre esta disabled no habilito cm
                                {
                                    TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cmID)[DT_CounterM.ENABLED] = true;
                                    foreach (DataRow damageRow in TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + cmID))
                                    {
                                        damageRow[DT_CounterM_Damage.STATUS] = true;
                                    }
                                    TheCurrentLayout.LinesListCMState[Convert.ToInt32(cmID)] = true;
                                    (TheCurrentLayout.LinesList.Find(item => (item.ID == cmID && item.IsCM))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Black));
                                    (TheCurrentLayout.LinesList.Find(item => (item.ID == cmID && item.IsCM))).IsActivated = true;
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

        private void BtnFilterRisk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DV_CrossRisk.RowFilter = DT_Risk_Damages.RISK_NAMESHORT + " like '%" + txtFilterRisk.Text + "%'";
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnFilterCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DV_Cross_CM.RowFilter = DT_CounterM_Damage.COUNTERM_NAMESHORT + " like '%" + txtFilterCM.Text + "%'";
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void TxtFilterRisk_TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnFilterRisk_Click(sender, e);
        }

        private void TxtFilterCM_TextChanged(object sender, TextChangedEventArgs e)
        {
            BtnFilterCM_Click(sender, e);
        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            txtFilterRisk.Clear();
        }

        private void BtnCloseFilterCM_Click(object sender, RoutedEventArgs e)
        {
            txtFilterCM.Clear();
        }

        #endregion

        #endregion

        //GHT: Seleccionando y deseleccionando miembros del grupo (RISK o CM)
        #region Pan Mouse Events
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (isPanEnabled)
            {
                if (TheCurrentLayout.ScrollGridPaint.IsMouseOver && e.LeftButton == MouseButtonState.Pressed)
                {
                    // Save starting point, used later when determining how much to scroll.
                    scrollStartPoint = e.GetPosition(this);
                    scrollStartOffset.X = TheCurrentLayout.ScrollGridPaint.HorizontalOffset;
                    scrollStartOffset.Y = TheCurrentLayout.ScrollGridPaint.VerticalOffset;

                    // Update the cursor if can scroll or not.
                    this.Cursor = (TheCurrentLayout.ScrollGridPaint.ExtentWidth > TheCurrentLayout.ScrollGridPaint.ViewportWidth) ||
                        (TheCurrentLayout.ScrollGridPaint.ExtentHeight > TheCurrentLayout.ScrollGridPaint.ViewportHeight) ?
                        GrabHand : Cursors.Arrow;

                    this.CaptureMouse();
                }
            }
            base.OnPreviewMouseDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (isPanEnabled)
            {
                if (this.IsMouseCaptured)
                {
                    // Get the new scroll position.
                    Point point = e.GetPosition(this);

                    // Determine the new amount to scroll.
                    Point delta = new Point(
                        (point.X > this.scrollStartPoint.X) ?
                            -(point.X - this.scrollStartPoint.X) :
                            (this.scrollStartPoint.X - point.X),

                        (point.Y > this.scrollStartPoint.Y) ?
                            -(point.Y - this.scrollStartPoint.Y) :
                            (this.scrollStartPoint.Y - point.Y));

                    // Scroll to the new position.
                    TheCurrentLayout.ScrollGridPaint.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
                    TheCurrentLayout.ScrollGridPaint.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
                }
            }
            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            if (isPanEnabled)
            {
                if (this.IsMouseCaptured)
                {
                    this.Cursor = OpenHand;
                    this.ReleaseMouseCapture();
                }
            }
            else
            {
                this.Cursor = Cursors.Arrow;
            }
            base.OnPreviewMouseUp(e);
        }

        private void ButtonHand_Click(object sender, RoutedEventArgs e)
        {
            if (TheCurrentLayout != null)
            {
                if (sender is Button buttonHandLocal)
                {
                    if (!isPanEnabled)
                    {
                        ContentControl contentHand = new ContentControl
                        {
                            Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.CursorDefault }
                        };
                        buttonHandLocal.Content = contentHand;
                        this.Cursor = OpenHand;
                        isPanEnabled = true;
                    }
                    else
                    {
                        ContentControl contentArrow = new ContentControl
                        {
                            Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.Hand }
                        };
                        buttonHandLocal.Content = contentArrow;
                        this.Cursor = Cursors.Arrow;
                        isPanEnabled = false;
                    }
                }
            }
        }
        #endregion

        #region WBSRegion
        public void RefreshWBS()
        {
            try
            {
                DsWBS = new UserDataSet();
                ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                DsWBS.Merge(ws.GetAllWBSFiltered(new object[] { IdProject }));
                ws.Dispose();
                TranslateToTreeViewWBS(DsWBS);
                DV_WBS = DsWBS.Tables[DT_WBS.TABLE_NAME].DefaultView;
                FillCBWBSFilter();
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private bool WBS_isSheet(int ID_WBS)
        {
            try
            {
                if (DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + ID_WBS).Any())
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FillCBWBSFilter()
        {
            try
            {
                using (DataTable WBSHijos = new DataTable("WBSHojas"))
                {                    
                    DataColumn idwbsColumn = new DataColumn(DT_WBS.ID_WBS, typeof(int));
                    DataColumn WbsColumn = new DataColumn(DT_WBS.USER_WBS, typeof(string));
                    WBSHijos.Columns.Add(idwbsColumn);
                    WBSHijos.Columns.Add(WbsColumn);
                    DataRow drWBSHijos = WBSHijos.NewRow();
                    drWBSHijos[DT_WBS.ID_WBS] = -1;
                    drWBSHijos[DT_WBS.USER_WBS] = "General-All";
                    WBSHijos.Rows.Add(drWBSHijos);
                    foreach (DataRow wbsRow in DsWBS.Tables[DT_WBS.TABLE_NAME].Rows)
                    {
                        if (WBS_isSheet((int)wbsRow[DT_WBS.ID_WBS]))
                        {
                            drWBSHijos = WBSHijos.NewRow();
                            drWBSHijos[DT_WBS.ID_WBS] = wbsRow[DT_WBS.ID_WBS];
                            drWBSHijos[DT_WBS.USER_WBS] = wbsRow[DT_WBS.USERNAME] + "-" + wbsRow[DT_WBS.NIVEL] + wbsRow[DT_WBS.WBS_NAME];
                            WBSHijos.Rows.Add(drWBSHijos);
                        }
                    }
                    DvCBWBS = WBSHijos.DefaultView;
                    if (CbFilterWBSRisk != null)
                    {
                        CbFilterWBSRisk.SelectedIndex = 0;
                    }
                    if (CbFilterWBSCM != null)
                    {
                        CbFilterWBSCM.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void TranslateToTreeViewWBS(DataSet ds)
        {
            TreeViewWBS.Items.Clear();
            foreach (DataRow item in BuscarWBSSinPadre(ds))
            {

                MyTreeViewItem tItem = new MyTreeViewItem
                {
                    IsExpanded = true,
                    MyID = (int)item[DT_WBS.ID_WBS]
                };
                tItem.TextWBS.Text = item[DT_WBS.NIVEL] + " " + item[DT_WBS.WBS_NAME] + " [" + item[DT_WBS.USERNAME] + "]";
                tItem.BtnEye.Click += BtnEye_Click;
                tItem.BtnDelete.Click += BtnDelete_Click;
                tItem.BtnEdit.Click += BtnEdit_Click;
                tItem.BtnEye.IdWBS = tItem.BtnDelete.IdWBS = tItem.BtnEdit.IdWBS = tItem.MyID;

                TreeViewWBS.Items.Add(tItem);
                tItem.EnableBtns(LoginUser == item[DT_WBS.USERNAME].ToString());
                BuscarHijosWBS(ds, (int)item[DT_WBS.ID_WBS], tItem, LoginUser == item[DT_WBS.USERNAME].ToString());
            }
        }

        public void BuscarHijosWBS(DataSet ds, int idFather, MyTreeViewItem treeItem, bool habilita)
        {
            foreach (DataRow item in ds.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + idFather))
            {
                MyTreeViewItem tItem = new MyTreeViewItem
                {
                    IsExpanded = true,
                    MyID = (int)item[DT_WBS_STRUCTURE.ID_CHILD]
                };
                tItem.TextWBS.Text = item[DT_WBS_STRUCTURE.CNIVEL] + " " + item[DT_WBS_STRUCTURE.CHILD] + " [" + item[DT_WBS_STRUCTURE.CHILD_USER] + "]";
                tItem.BtnEye.Click += BtnEye_Click;
                tItem.BtnDelete.Click += BtnDelete_Click;
                tItem.BtnEdit.Click += BtnEdit_Click;
                tItem.BtnEye.IdWBS = tItem.BtnDelete.IdWBS = tItem.BtnEdit.IdWBS = tItem.MyID;

                treeItem.Items.Add(tItem);
                if (habilita)
                {
                    BuscarHijosWBS(ds, (int)item[DT_WBS_STRUCTURE.ID_CHILD], tItem, habilita);
                }
                else
                {
                    bool vari = false;
                    if (LoginUser == item[DT_WBS_STRUCTURE.CHILD_USER].ToString())
                    {
                        tItem.EnableBtns(true);
                        vari = true;
                        BuscarHijosWBS(ds, (int)item[DT_WBS_STRUCTURE.ID_CHILD], tItem, vari);
                    }
                    else
                    {
                        tItem.EnableBtns(false);
                        vari = false;
                        BuscarHijosWBS(ds, (int)item[DT_WBS_STRUCTURE.ID_CHILD], tItem, vari);
                    }

                }

            }
        }

        private void BtnEye_Click(object sender, RoutedEventArgs e)
        {
            if (TheCurrentLayout != null && OpenedDocuments.Count > 0)
            {
                if (sender is ButtonWBS btnEye)
                {
                    try
                    {
                        if (TheCurrentLayout != null)
                        {
                            System.Drawing.Color drawingCColor = System.Drawing.Color.FromArgb(int.Parse(TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram)[TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));

                            DataRow dr = DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(btnEye.IdWBS);
                            foreach (var item in TheCurrentLayout.LinesList)
                            {
                                if (item.IsCM)
                                {
                                    if (!(TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + item.ID + " and " + DT_CM_WBS.ID_WBS + " = " + dr[DT_WBS.ID_WBS]).Any()))
                                    {
                                        //item.Stroke = ;
                                        item.SetColor(new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, drawingCColor.R, drawingCColor.G, drawingCColor.B)));
                                    }
                                    else
                                    {
                                        //item.Stroke = new SolidColorBrush(Colors.Black);
                                        item.SetColor(new SolidColorBrush(System.Windows.Media.Colors.Black));
                                    }
                                }
                                else
                                {
                                    if (!(TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + item.ID + " and " + DT_RISK_WBS.ID_WBS + " = " + dr[DT_WBS.ID_WBS]).Any()))
                                    {
                                        //item.Stroke = new SolidColorBrush(Color.FromArgb(50, drawingCColor.R, drawingCColor.G, drawingCColor.B));
                                        //foreach (var segmentLine in item.Segments)
                                        //{
                                        //    segmentLine.Stroke = item.Stroke;
                                        //}
                                        item.SetColor(new SolidColorBrush(System.Windows.Media.Color.FromArgb(50, drawingCColor.R, drawingCColor.G, drawingCColor.B)));
                                    }
                                    else
                                    {
                                        //item.Stroke = new SolidColorBrush(Color.FromArgb(drawingCColor.A, drawingCColor.R, drawingCColor.G, drawingCColor.B));
                                        //foreach (var segmentLine in item.Segments)
                                        //{
                                        //    segmentLine.Stroke = item.Stroke;
                                        //}
                                        item.SetColor(new SolidColorBrush(System.Windows.Media.Color.FromArgb(drawingCColor.A, drawingCColor.R, drawingCColor.G, drawingCColor.B)));
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
            }
        }

        public List<DataRow> BuscarWBSSinPadre(DataSet ds)
        {
            List<DataRow> ListaRow = new List<DataRow>();
            foreach (DataRow item in ds.Tables[DT_WBS.TABLE_NAME].Rows)
            {
                if (!(ds.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + item[DT_WBS.ID_WBS]).Any()))
                {
                    ListaRow.Add(item);
                }
            }
            return ListaRow;
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ButtonWBS btnWBS)
                {
                    DataRow dr = DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(btnWBS.IdWBS);
                    WindowWBS wbs = new WindowWBS
                    {
                        DrWBS = dr,
                        IdProject = IdProject,
                        WBS_Structure = DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Copy(),
                        WBS_Encoder = DsWBS.Tables[DT_WBS.TABLE_NAME].Copy(),
                        Operation = General.UPDATE,
                        Icon = Icon
                    };
                    if (wbs.ShowDialog() == true)
                    {
                        DsWBS.Tables[DT_WBS.TABLE_NAME].Merge(wbs.WBS_Encoder);
                        DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Merge(wbs.WBS_Structure);
                        if (DsWBS.HasChanges())
                        {
                            //DataSet temp = new DataSet();
                            ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                            DataSet temp = DsWBS.GetChanges();
                            temp = ws.SaveWBS(temp);
                            DsWBS.Merge(temp);
                            DsWBS.AcceptChanges();
                            ws.Dispose();
                            RefreshWBS();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }

        }

        private void DeleteWBS(DataRow drWBS)
        {
            try
            {
                General.EliminarMisHijosWBS(General.MyWBSChildren(drWBS, DsWBS.Tables[DT_WBS.TABLE_NAME], DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME]), DsWBS.Tables[DT_WBS.TABLE_NAME], DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME]);
                drWBS.Delete();
                if (DsWBS.HasChanges())
                {
                    ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                    DataSet temp = DsWBS.GetChanges();
                    temp = ws.SaveWBS(temp);
                    DsWBS.Merge(temp);
                    DsWBS.AcceptChanges();
                    ws.Dispose();
                    RefreshWBS();                    
                }
                IS_DELETING_WBS = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_WBS = false;
                MostrarErrorDialog(ex.Message);
            }            
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ButtonWBS btnWBS)
                {
                    DrWBStoDelete = DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(btnWBS.IdWBS);
                    IS_DELETING_WBS = true;
                    MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + DrWBStoDelete[DT_WBS.WBS_NAME] + "]?");
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
                IS_DELETING_WBS = false;
            }
        }

        private void AnchorWBSList_IsVisibleChanged(object sender, EventArgs e)
        {
            RefreshWBS();
        }

        private void BtnAddWBS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WindowWBS wbs = new WindowWBS
                {
                    DrWBS = DsWBS.Tables[DT_WBS.TABLE_NAME].NewRow(),
                    IdProject = IdProject,
                    WBS_Structure = DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Copy(),
                    WBS_Encoder = DsWBS.Tables[DT_WBS.TABLE_NAME].Copy(),
                    Operation = General.INSERT,
                    Icon = Icon
                };
                if (wbs.ShowDialog() == true)
                {
                    DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Add(wbs.DrWBS);
                    DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Merge(wbs.WBS_Structure);
                    DsWBS.Tables[DT_WBS.TABLE_NAME].Merge(wbs.WBS_Encoder);
                    if (DsWBS.HasChanges())
                    {
                        //DataSet temp = new DataSet();
                        ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS();
                        DataSet temp = DsWBS.GetChanges();
                        temp = ws.SaveWBS(temp);
                        DsWBS.Merge(temp);
                        DsWBS.AcceptChanges();
                        ws.Dispose();
                        RefreshWBS();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }

        }

        private void BtnCollapse_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in TreeViewWBS.Items)
            {
                //((TreeViewItem)item).IsExpanded = false;
                CollapseAll((TreeViewItem)item);
            }
        }

        private void CollapseAll(TreeViewItem ItemFather)
        {
            ItemFather.IsExpanded = false;
            foreach (var item in ItemFather.Items)
            {
                CollapseAll((TreeViewItem)item);
            }
        }

        private void DgWBS_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                //if (dgWBS.SelectedIndex >= 0)
                //{
                //    if (CurrentLayout != null)
                //    {
                //        System.Drawing.Color drawingCColor = System.Drawing.Color.FromArgb(int.Parse(CurrentLayout.Ds.Tables[DT_RiskTree_Damages.TABLENAME].Select(DT_RiskTree_Damages.ID_RISKTREE + " = " + CurrentLayout.ID_Diagram)[CurrentLayout.CbFilterTopR.SelectedIndex][DT_RiskTree_Damages.COLOR].ToString()));

                //        DataRow dr = DV_WBS[dgWBS.SelectedIndex].Row;
                //        foreach (var item in CurrentLayout.LinesList)
                //        {
                //            if (item.IsCM)
                //            {
                //                if (!(CurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + item.ID + " and " + DT_CM_WBS.ID_WBS + " = " + dr[DT_WBS.ID_WBS]).Any()))
                //                {
                //                    item.Stroke = new SolidColorBrush(Color.FromArgb(50, drawingCColor.R, drawingCColor.G, drawingCColor.B));
                //                }
                //                else
                //                {
                //                    item.Stroke = new SolidColorBrush(Colors.Black);
                //                }
                //            }
                //            else
                //            {
                //                if (!(CurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + item.ID + " and " + DT_RISK_WBS.ID_WBS + " = " + dr[DT_WBS.ID_WBS]).Any()))
                //                {
                //                    item.Stroke = new SolidColorBrush(Color.FromArgb(50, drawingCColor.R, drawingCColor.G, drawingCColor.B));
                //                }
                //                else
                //                {
                //                    item.Stroke = new SolidColorBrush(Color.FromArgb(drawingCColor.A, drawingCColor.R, drawingCColor.G, drawingCColor.B));
                //                }
                //            }
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void BtnDelWBS_Click(object sender, RoutedEventArgs e)
        {


        }

        private void DgWBS_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            BtnCollapse_Click(sender, e);
        }

        private void BtnExcelWBS_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (TheCurrentLayout != null)
            {

            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                foreach (var contentToClose in _layoutRoot.Descendents().OfType<LayoutContent>().Where(d => (d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow)).ToArray())
                {
                    contentToClose.Close();
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }       

        private void YesNoDialog_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            try
            {
                if (!Equals(eventArgs.Parameter, true))
                {
                    if (IS_SAVING_DATA)
                    {
                        if (TheCurrentLayout != null)
                        {
                            TheCurrentLayout.Ds.RejectChanges();
                        }
                    }
                    if (IS_REPEATING_NAME)
                    {
                        IS_REPEATING_NAME = false;
                        Selected_DataGrid.CancelEdit(DataGridEditingUnit.Cell);
                    }
                    if (IS_REOPEN_DIAGRAM)
                    {
                        IS_REOPEN_DIAGRAM = false;
                    }
                    if (IS_DELETING_DIAGRAM)
                    {
                        IS_DELETING_DIAGRAM = false;
                    }
                    if (IS_DELETING_RISK)
                    {
                        IS_DELETING_RISK = false;
                    }
                    if (IS_DELETING_CM)
                    {
                        IS_DELETING_CM = false;
                    }
                    if (IS_DELETING_GROUP_CM)
                    {
                        IS_DELETING_GROUP_CM=false;
                    }
                    if (IS_REMOVING_GROUP_FILTER)
                    {
                        IS_REMOVING_GROUP_FILTER = false;
                    }
                    if (IS_REMOVING_GROUP)
                    {
                        IS_REMOVING_GROUP = false;
                    }
                    if (IS_DELETING_GROUP_TAB)
                    {
                        IS_DELETING_GROUP_TAB = false;
                    }
                    if (IS_REMOVING_TABLE_ROW)
                    {
                        IS_DELETING_GROUP_TAB = false;
                    }
                    if (IS_SAVING_DATA)
                    {
                        IS_SAVING_DATA = false;
                    }
                    if (IS_DELETING_WBS)
                    {
                        IS_DELETING_WBS = false;
                    }
                    if (IS_REPEATING_NAME)
                    {
                        IS_REPEATING_NAME = false;
                    }
                    if (IS_DELETING_RISK_ROLE_ROW)
                    {
                        IS_DELETING_RISK_ROLE_ROW = false;
                    }
                    if (IS_DELETING_Risk_WBS_ROW)
                    {
                        IS_DELETING_Risk_WBS_ROW = false;
                    }
                    if (IS_DELETING_CM_WBS)
                    {
                        IS_DELETING_CM_WBS = false;
                    }
                    if (IS_DELETING_CM_ROLE)
                    {
                        IS_DELETING_CM_ROLE = false;
                    }
                    return;
                }
                if (Equals(eventArgs.Parameter, true))
                {
                    if (IS_REOPEN_DIAGRAM)
                    {
                        ReOpenDiagram(DiagramID);
                    }
                    if (IS_DELETING_DIAGRAM)
                    {
                        DeleteDiagram();
                    }
                    if (IS_DELETING_RISK)
                    {
                        DeleteRisk();
                    }
                    if (IS_DELETING_CM)
                    {
                        DeleteCM();
                    }
                    if (IS_DELETING_GROUP_CM)
                    {
                        DeletingCM_Group();
                    }
                    if (IS_REMOVING_GROUP_FILTER)
                    {
                        RemoveGroupFilter(ID_Groupe);
                    }
                    if (IS_REMOVING_GROUP)
                    {
                        RemoveGroup(ID_Groupe);
                    }
                    if (IS_DELETING_GROUP_TAB)
                    {
                        RemoveSomeHandler(TheDataRowVieww);
                    }
                    if (IS_REMOVING_TABLE_ROW)
                    {
                        TableRowDeleting(TheRemovingTable);
                    }
                    if (IS_SAVING_DATA)
                    {
                        SaveData(TheCurrentLayout.Ds, true);
                    }
                    if (IS_DELETING_WBS)
                    {
                        DeleteWBS(DrWBStoDelete);
                    }
                    if (IS_REPEATING_NAME)
                    {
                        IS_REPEATING_NAME = false;
                    }
                    if (IS_DELETING_RISK_ROLE_ROW)
                    {
                        Delete_Role(Selected_RiskRoleRow);
                    }
                    if (IS_DELETING_Risk_WBS_ROW)
                    {
                        Delete_RISK_WBS_Row(Selected_Risk_WBS_Row);
                    }
                    if (IS_DELETING_CM_WBS)
                    {
                        Delete_WBS(Selected_CM_WBS_Row);
                    }
                    if (IS_DELETING_CM_ROLE)
                    {
                        Delete_CM_Role(Selected_CM_Role_Row);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void TextPasword_GotFocus(object sender, RoutedEventArgs e)
        {
            TextPasword.SelectAll();
        }

        private void TextUser_GotFocus(object sender, RoutedEventArgs e)
        {
            TextUser.SelectAll();
        }

        private void LoginDialog_DialogOpened(object sender, MaterialDesignThemes.Wpf.DialogOpenedEventArgs eventArgs)
        {
            TextUser.Focus();
        }

        private void LoginDialog_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            try
            {
                if (!Equals(eventArgs.Parameter, true))
                {
                    if (IS_LOGIN)
                    {
                        Flag_login = false;
                        Close();
                    }
                    //if (IS_REPEATING_NAME)
                    //{
                    //    IS_REPEATING_NAME = false;
                    //}
                    return;
                }
                if (Equals(eventArgs.Parameter, true))
                {
                    if (IS_LOGIN)
                    {
                        
                        if (TextUser.Text != "")
                        {
                            Autenticar();
                            
                        }
                        else
                        {
                            Flag_login = false;
                            LoginMenuItem.Header = StringResources.LoginMenu;
                            Title = "Ensure Risk";
                            LoginUser = "";
                            MostrarErrorDialog("Login can't be empty");
                            ValidateAccess();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private DataSet GetAccess()
        {
            using (DataSet tempObtenerAcceso = new DataSet())
            {
                try
                {
                    using (ServiceAuthentication.WebServiceAuthentication ws = new ServiceAuthentication.WebServiceAuthentication())
                    {
                        string Mensaje = ".";
                        string Sesion = ".";
                        tempObtenerAcceso.Merge(ws.AuthenticateUsers(TextUser.Text, General.Encrypt(TextPasword.Password), ref Mensaje, Environment.UserName,
                            Environment.UserDomainName + "\\" + Environment.MachineName, ref Sesion));
                    }
                }
                catch (Exception ex)
                {
                    MostrarErrorDialog(ex.Message);
                    IS_LOGIN = false;
                    Flag_login = false;
                    LoginMenuItem.Header = StringResources.LoginMenu;
                    Title = "Ensure Risk";
                    LoginUser = "";
                }
                return tempObtenerAcceso;
            }
        }

        private void Autenticar()
        {
            if (TextUser.Text != "")
            {
                General gen = new General();
                AccessList = new List<int>();
                gen.Usser = TextUser.Text;
                using (UserDataSet ds = new UserDataSet())
                {
                    ds.Merge(GetAccess());
                    if (ds.Tables[DT_User.User_TABLA].Rows.Count != 0)
                    {
                        LoginUser = ds.Tables[DT_User.User_TABLA].Rows[0][DT_User.USERNAME].ToString();
                        foreach (DataRow item in ds.Tables[DT_User_Operation.TABLE_NAME].Rows)
                        {
                            AccessList.Add((int)item[DT_User_Operation.ID_OPERATION_COLUMN]);
                        }
                        Title = "Ensure Risk | " + StringResources.CONNECTED_STRING + " " + TextUser.Text;
                        LoginMenuItem.Header = StringResources.LOGOFF;
                        Flag_login = true;
                        TextPasword.Clear();
                        ValidateAccess();
                        RefreshWBS();
                        foreach (var item in OpenedDocuments)
                        {
                            item.LoginUser = LoginUser;
                        }
                    }
                    else
                    {
                        IS_LOGIN = false;
                        Flag_login = false;
                        LoginMenuItem.Header = StringResources.LoginMenu;
                        Title = "Ensure Risk";
                        LoginUser = "";
                        TextPasword.Clear();
                        MostrarErrorDialog(StringResources.WRONG_PASSWORD);
                        ValidateAccess();
                    }
                }                
            }
            IS_LOGIN = false;
        }

        private void UpdateCbFilterWBSRisk(object sender)
        {
            if (sender is ComboBox cbsender)
            {
                if (cbsender.SelectedValue != null)
                {
                    if (TheCurrentLayout != null)
                    {
                        CruzarTablaRisk(TheCurrentLayout.Ds);
                        CruzarTablaCM(TheCurrentLayout.Ds);
                    }
                }                
            }           
        }

        private void CbFilterWBSRisk_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                UpdateCbFilterWBSRisk(sender);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void CbFilterWBSRisk_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                UpdateCbFilterWBSRisk(sender);
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void RiskName_TextChanged(object sender, TextChangedEventArgs e)
        {
            RiskLineName = TextRisk.Text;
        }

        private void RiskName_KeyUp(object sender, KeyEventArgs e)
        {
            EditandoRisk = true;
            RiskresultStack.Visibility = Visibility.Visible;
            bool found = false;
            var border = (RiskresultStack.Parent as ScrollViewer).Parent as System.Windows.Controls.Border;

            string query = (sender as TextBox).Text;

            if (query.Length == 0)
            {
                // Clear   
                RiskresultStack.Children.Clear();
                border.Visibility = Visibility.Collapsed;
            }
            else
            {
                border.Visibility = Visibility.Visible;
            }

            // Clear the list   
            RiskresultStack.Children.Clear();

            // Add the result   
            foreach (DataRow obj in TheCurrentLayout.Ds.Tables[DT_DefaulRisk.Risk_TABLA].Rows)
            {
                if (obj[DT_DefaulRisk.RISK_NAME_COLUMNA].ToString().ToLower().StartsWith(query.ToLower()))
                {
                    // The word starts with this... Autocomplete must work   
                    AddRiskItem(obj[DT_DefaulRisk.RISK_NAME_COLUMNA].ToString());
                    found = true;
                }
            }

            if (!found)
            {
                RiskresultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }

        private void AddRiskItem(string text)
        {
            TextBlock block = new TextBlock
            {
                Text = text,
                Margin = new Thickness(2, 3, 2, 3),
                Cursor = Cursors.Hand
            };

            block.MouseLeftButtonUp += (sender, e) =>
            {
                TextRisk.Text = (sender as TextBlock).Text;
                RiskresultStack.Visibility = Visibility.Collapsed;
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
                SeleccionandoRisk = true;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
                SeleccionandoRisk = false;
            };

            RiskresultStack.Children.Add(block);
        }

        private void TextCM_TextChanged(object sender, TextChangedEventArgs e)
        {
            CMLineName = TextCM.Text;
        }

       
        private void BtnDeselect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null)
                {
                    //dgWBS.SelectedIndex = -1;
                    TheCurrentLayout.DropLines();
                    TheCurrentLayout.LoadLines();
                    TheCurrentLayout.LoadRectangles();
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.SetLinesThickness();
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        #endregion       

        #endregion
    }
    class Lang: DependencyObject
    {

    }
}
