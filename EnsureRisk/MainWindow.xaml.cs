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
//using System.Windows.Forms;

namespace EnsureRisk
{

    public class DataCurrenDiagram : INotifyPropertyChanged
    {
        int riskCount = 0;
        int cmCount = 0;
        private string line_Selected = "None";
        private string type_selected = "None";
        private readonly string diagramName = "None";
        private bool progressVisible = false;
        private bool progressIsIndeterminate = true;
        private MyLayoutDocumentt theCurrentLayout;

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

        public ScrollViewer TheCurrentViewer
        {
            get
            {
                if (TheCurrentLayout != null)
                {
                    return TheCurrentLayout.ScrollGridPaint;
                }
                else
                {
                    return new ScrollViewer();
                }
                
            }

            set
            {
                TheCurrentLayout.ScrollGridPaint = value;
                OnPropertyChanged("TheCurrentViewer");
            }
        }

        public bool ProgressVisible
        {
            get
            {
                return progressVisible;
            }

            set
            {
                progressVisible = value;
                OnPropertyChanged("ProgressVisible");
            }
        }
        public bool ProgressIsIndeterminate
        {
            get
            {
                return progressIsIndeterminate;
            }

            set
            {
                progressIsIndeterminate = value;
                OnPropertyChanged("ProgressIsIndeterminate");
            }
        }
        public int RiskCount
        {
            get { return riskCount; }
            set
            {
                riskCount = value;
                OnPropertyChanged("RiskCount");
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
        public int CMCount
        {
            get { return cmCount; }
            set
            {
                cmCount = value;
                OnPropertyChanged("CMCount");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

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
        #endregion     

        //public MyLayoutDocumentt P.TheCurrentLayout { get;set; }       

        #region DataManagers
        public DataView DVRisk_Tree { get; set; }
        public DataTable Dt_Cross_Risk { get; set; }
        public DataTable Dt_Cross_CM { get; set; }
        public DataView DV_CrossRisk { get; set; }
        public DataView DV_Cross_CM { get; set; }
        public DataView DV_WBS { get; set; }
        public DataSet DsMain { get; set; }
        public DataSet DsWBS { get; set; }
        public DataSet DsGroupRisk { get; set; }
        public DataSet DsGroupCM { get; set; }
        #endregion

        #region DecimalUIConverterParams
        static DecimalUIConverterParams decimalConfig = new DecimalUIConverterParams(Properties.Settings.Default.DecimalsStringFormat, Properties.Settings.Default.DecimalFractionalDigits);
        #endregion
        #region Another Properties & Atributes
        //public int IdWBS { get; set; }
        public bool WBSFiltering { get; set; }
        private bool Flag_login, isPanEnabled;
        private int Indexx = 0;
        public string LoginUser { get; set; }
        public int IdProject { get; set; }

        //private bool DiagramAsTree = false;

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

        public DataCurrenDiagram P { get; set; }
        public ServiceRiskController.WebServiceRisk WSRisk { get; set; }
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
                    //bool start = (Boolean)TableColors.Select().First()["StartLogo"];
                    //if (start)
                    //{
                    //    WindowOpening op = new WindowOpening();
                    //    op.ShowDialog();
                    //}                
                    WindowOpening op = new WindowOpening();
                    op.ShowDialog();
                    TableColors.Dispose();
                }
                else
                {
                    WindowOpening op = new WindowOpening();
                    op.ShowDialog();
                }
                P = new DataCurrenDiagram();
                //P.TheCurrentLayout = new MyLayoutDocument();
                //this.DataContext = P.TheCurrentLayout;
                InitializeComponent();
                buttonCantidad.DataContext = P;
                buttonCantidadCM.DataContext = P;
                TextLineSelected.DataContext = P;
                TextLineType.DataContext = P;
                MiniMap.DataContext = P;
                WSRisk = new ServiceRiskController.WebServiceRisk();
                //WSRisk.SaveRiskCompleted += WSRisk_SaveRiskCompleted;
                //WSRisk.GetRiskTreeCompleted += WSRisk_GetRiskTreeCompleted;
                //WSRisk.GetRiskTreeDataCompleted += WSRisk_GetRiskTreeDataCompleted;
                //TheProgress.DataContext = P;
                //textProgress.DataContext = p;
                DsMain = new UserDataSet();
                AccessList = new List<int>();
                OpenedDocuments = new List<MyLayoutDocumentt>();

                //P.TheCurrentLayout = new MyLayoutDocumentt();
                //Binding myBinding = new Binding
                //{
                //    Source = P,
                //    Path = new PropertyPath("TheCurrentViewer"),
                //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                //};

                //BindingOperations.SetBinding(MiniMap, MiniMapControl.MapSourceProperty, myBinding);
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
                //MostarDialog(ex.Message);
            }            
        }

        public void MostarDialog(string text)
        {
            MessageDialog.IsOpen = true;
            TextMessage.Text = text;
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
                BtnBackward.Visibility = Visibility.Hidden;
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
                ChangeLanguage();
                CbProjects_DropDownClosed(sender, e);
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
            }

        }

        /// <summary>
        /// AutoFix the Grid Area acording with the Tree Diagram sizes.
        /// </summary>
        private void CalculateGridSize()
        {
            // tree sizes are 0-based, so add 1
            //var treeWidth = Tree.Width + 1;
            //var treeHeight = Tree.Height + 1;

            //GridPaintLines.Width = ((treeWidth * NODE_WIDTH) + ((treeWidth + 1) * NODE_MARGIN_X));
            //GridPaintLines.Height = (treeHeight * NODE_HEIGHT) + ((treeHeight + 1) * NODE_MARGIN_Y);
        }

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

        private void LoginCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                LoginWindow lw = new LoginWindow
                {
                    Icon = Icon
                };
                if (lw.ShowDialog() == true)
                {
                    if (lw.Usser != "" || lw.Usser != null)
                    {
                        Title = "Ensure Risk | " + StringResources.CONNECTED_STRING + " " + lw.Usser;
                        LoginMenuItem.Header = StringResources.LOGOFF;
                        Flag_login = true;
                        LoginUser = lw.Usser;
                        AccessList = lw.AccessList;
                        ValidateAccess();
                        RefreshWBS();
                        foreach (var item in OpenedDocuments)
                        {
                            item.LoginUser = LoginUser;
                        }
                    }
                    else
                    {
                        Flag_login = false;
                        LoginMenuItem.Header = StringResources.LoginMenu;
                        Title = "Ensure Risk";
                        LoginUser = "";
                        AccessList = lw.AccessList;
                        ValidateAccess();
                    }
                }
                else
                {
                    Flag_login = false;
                }
                if (lw.DialogResult == false)
                {
                    Flag_login = false;
                    Close();
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
            if (P.TheCurrentLayout != null)
            {
                if (P.TheCurrentLayout.LinesList.Count > 0)
                {
                    ExportToImage();
                    new WindowAlert("Diagram exported successfully!").ShowDialog();
                }
            }
        }

        private void PrintCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (P.TheCurrentLayout != null)
                {
                    PrintDialog pd = new PrintDialog();
                    if (pd.ShowDialog() == true)
                    {
                        Rect printableArea = GetPrintableArea(pd);
                        Grid prueba = new Grid();
                        UIElement[] array = new UIElement[P.TheCurrentLayout.GridPaintLines.Children.Count];
                        P.TheCurrentLayout.GridPaintLines.Children.CopyTo(array, 0);
                        P.TheCurrentLayout.GridPaintLines.Children.Clear();
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
                            P.TheCurrentLayout.GridPaintLines.Children.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                if (P.TheCurrentLayout != null)
                {
                    MiniMap.MapSource = P.TheCurrentLayout.ScrollGridPaint;
                    //this.DataContext = P.TheCurrentLayout;
                    //MiniMap.DataContext = P.TheCurrentLayout;
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
            //            System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_DAMAGE + " = " + (Int32)CurrentLayout.CbFilterTopR.SelectedValue).First()[DT_Risk_Damages.COLOR].ToString()));

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
            //                            Ds.Tables[DT_RiskStructure.RiskStructure_TABLA], (Int32)cbFilterTopR.SelectedValue, Ds.Tables[DT_Risk_Damages.TABLENAME],
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

            //            if (Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_DAMAGE + " = " + (Int32)CurrentLayout.CbFilterTopR.SelectedValue).Any())
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
            //    new WindowMessageOK(ex.Message);
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
                if (P.TheCurrentLayout != null && P.TheCurrentLayout.Ds.HasChanges())
                {
                    TheProgress.Visibility = Visibility.Visible;
                    HabilitarBotones(false);
                    Salvando();
                }
            }
            catch (Exception ex)
            {
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void Salvando()
        {
            try
            {
                Cursor = Cursors.Wait;
                ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk();
                DataSet temp = P.TheCurrentLayout.Ds.GetChanges();
                temp = ws.SaveRisk(temp);
                P.TheCurrentLayout.Ds.Merge(temp);
                P.TheCurrentLayout.Ds.AcceptChanges();
                ws.Dispose();
                if (P.TheCurrentLayout.ID_Diagram < 0)
                {
                    P.TheCurrentLayout.ID_Diagram = (Int32)P.TheCurrentLayout.DrDiagram[DT_Diagram.ID_DIAGRAM];
                    //CurrentLayout.Close();
                }
                RefreshData();
                P.TheCurrentLayout.Ds = DsMain.Copy();
                P.TheCurrentLayout.Ds.AcceptChanges();
                Cursor = Cursors.Arrow;
                if (P.TheCurrentLayout.ID_Diagram > 0)
                {
                    //CurrentLayout.Ds = DsMain;
                    P.TheCurrentLayout.DropLines();
                    P.TheCurrentLayout.DropRectangles();
                    P.TheCurrentLayout.LoadLines();
                    P.TheCurrentLayout.LoadRectangles();
                    P.TheCurrentLayout.DrawNumbers();
                    P.TheCurrentLayout.SetLinesThickness();
                    TextProbabilityChange(P.TheCurrentLayout.MainLine);
                    CruzarTablaRisk(P.TheCurrentLayout.Ds);
                    CruzarTablaCM(P.TheCurrentLayout.Ds);
                    new WindowMessageInformation("Data saved successfully!!!").ShowDialog();
                }

                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
            }
            catch (Exception ex)
            {
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                new WindowMessageOK(ex.Message).ShowDialog();
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
                if (P.TheCurrentLayout != null)
                {
                    TheProgress.Visibility = Visibility.Visible;
                    DataRow currentDiagram = P.TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(P.TheCurrentLayout.ID_Diagram);
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
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void CreandoDamagesDiagram(DataSet dsResult, DataRow drDiagram, DataSet dsSource)
        {
            foreach (DataRow drDiagramDamage in dsSource.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + P.TheCurrentLayout.ID_Diagram))
            {
                DataRow newTreeDamage = dsResult.Tables[DT_Diagram_Damages.TABLENAME].NewRow();
                newTreeDamage[DT_Diagram_Damages.COLOR] = drDiagramDamage[DT_Diagram_Damages.COLOR];
                newTreeDamage[DT_Diagram_Damages.DAMAGE] = drDiagramDamage[DT_Diagram_Damages.DAMAGE];
                newTreeDamage[DT_Diagram_Damages.ID_DAMAGE] = drDiagramDamage[DT_Diagram_Damages.ID_DAMAGE];
                newTreeDamage[DT_Diagram_Damages.ID_RISKTREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
                newTreeDamage[DT_Diagram_Damages.RISK_TREE] = drDiagram[DT_Diagram.DIAGRAM_NAME];
                newTreeDamage[DT_Diagram_Damages.UM] = drDiagramDamage[DT_Diagram_Damages.UM];
                dsResult.Tables[DT_Diagram_Damages.TABLENAME].Rows.Add(newTreeDamage);
            }
        }

        private void CreandoRisks(MyLayoutDocumentt originalLayout, MyLayoutDocumentt destinyLayout, List<RiskPolyLine> listToFill)
        {
            foreach (DataRow item in originalLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + originalLayout.ID_Diagram))
            {
                int myPosition = item[DT_Risk.POSITION] == DBNull.Value ? 0 : (int)item[DT_Risk.POSITION];
                RiskPolyLine riskLine;
                if ((bool)item[DT_Risk.IS_ROOT])
                {
                    riskLine = new RiskPolyLine(destinyLayout.GridPaintLines, MenuMainRisk, false)
                    {
                        ShortName = item[DT_Risk.NAMESHORT].ToString(),
                        ID = (int)item[DT_Risk.ID],
                        Position = myPosition,
                        IsRoot = (bool)item[DT_Risk.IS_ROOT],
                        Collapsed = (bool)item[DT_Risk.ISCOLLAPSED],
                        Probability = (decimal)item[DT_Risk.PROBABILITY],
                        IsCM = false
                    };
                }
                else
                {
                    riskLine = new RiskPolyLine(destinyLayout.GridPaintLines, MenuRisk, false)
                    {
                        ShortName = item[DT_Risk.NAMESHORT].ToString(),
                        ID = (int)item[DT_Risk.ID],
                        Position = myPosition,
                        IsRoot = (bool)item[DT_Risk.IS_ROOT],
                        Collapsed = (bool)item[DT_Risk.ISCOLLAPSED],
                        Probability = (decimal)item[DT_Risk.PROBABILITY],
                        IsCM = false
                    };
                }
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

        private void CreandoCMs(MyLayoutDocumentt originalLayout, MyLayoutDocumentt destinyLayout, List<RiskPolyLine> riskPolyLines)
        {
            foreach (DataRow item in originalLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK_TREE + " = " + originalLayout.ID_Diagram))
            {
                RiskPolyLine cmline = new RiskPolyLine(destinyLayout.GridPaintLines, MenuCM, true)
                {
                    IsCM = true,
                    Position = (Int32)item[DT_CounterM.POSITION],
                    FromTop = false,
                    ShortName = item[DT_CounterM.NAMESHORT].ToString(),
                    IdRiskFather = (Int32)item[DT_CounterM.ID_RISK],
                    ID = (Int32)item[DT_CounterM.ID],
                    Probability = (Decimal)item[DT_CounterM.PROBABILITY],
                    IsActivated = (Boolean)item[DT_CounterM.ENABLED]
                };
                riskPolyLines.Add(cmline);
            }

        }

        private void SalvandoAs(DataRow currentDiagram, string name)
        {
            try
            {
                DataSet tempDS = new UserDataSet();
                P.TheCurrentLayout.ListCopy = new List<RiskPolyLine>();
                //Creando el diagrama
                DataRow drDiagram = tempDS.Tables[DT_Diagram.TABLE_NAME].NewRow();
                drDiagram[DT_Diagram.ID_PROJECT] = currentDiagram[DT_Diagram.ID_PROJECT];
                drDiagram[DT_Diagram.DIAGRAM_NAME] = name;
                tempDS.Tables[DT_Diagram.TABLE_NAME].Rows.Add(drDiagram);

                CreandoDamagesDiagram(tempDS, drDiagram, P.TheCurrentLayout.Ds);

                CreandoRisks(P.TheCurrentLayout, P.TheCurrentLayout, P.TheCurrentLayout.ListCopy) ;

                CreandoCMs(P.TheCurrentLayout, P.TheCurrentLayout, P.TheCurrentLayout.ListCopy);

                TreeOperation.Build_Tree(P.TheCurrentLayout.ListCopy);
                P.TheCurrentLayout.CopyRisk = P.TheCurrentLayout.ListCopy.FirstOrDefault(p => p.IsRoot);
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

                CreandoDamagesWBSAndRole_to_Risk(tempDS, P.TheCurrentLayout.Ds, drRisk, drDiagram, drDiagram[DT_Diagram.DIAGRAM_NAME].ToString(), P.TheCurrentLayout.CopyRisk);
                //--------------------------------------------------------------------------------------
                P.TheCurrentLayout.CopyRisk.ID = (Int32)drRisk[DT_Risk.ID];

                Paste(P.TheCurrentLayout.CopyRisk, P.TheCurrentLayout.Ds, tempDS, (Int32)drDiagram[DT_Diagram.ID_DIAGRAM], P.TheCurrentLayout.ListCopy);//Aca pego el resto del diagrama

                P.TheCurrentLayout.CopyRisk.IdRiskFather = 0;
                P.TheCurrentLayout.Copiando = false;
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
                    new WindowMessageInformation("Data saved successfully!!!").ShowDialog();
                    OpenSavedDiagram(drDiagram, DsMain);
                }
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
            }
            catch (Exception ex)
            {
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                new WindowMessageOK(ex.Message).ShowDialog();
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
            //Style btnStyle = new Style(typeof(Button), ((Button)FindResource("Delete")).Style);
            //Style sldStyle = new Style(typeof(Slider), ((Slider)FindResource("CommonSlider")).Style);
            //Style cbStyle = new Style(typeof(ComboBox), ((ComboBox)FindResource("CommonCB")).Style);

            //myly.BtMinus.Style = myly.BtMPlus.Style = btnStyle;
            //myly.SliderZoom.Style = sldStyle;
            //myly.CbFilterTopR.Style = myly.TheZoomComboBox.Style = cbStyle;

            //myly.GridPaintLines.Width = 200;
            //myly.GridPaintLines.Height = 200;
            //myly.MIdPoint = new Point(myly.GridPaintLines.Width - 180, myly.GridPaintLines.Height / 2);
            myly.DrDiagram = dataRow;
            myly.ID_Diagram = (int)dataRow[DT_Diagram.ID_DIAGRAM];
            myly.Title = dataRow[DT_Diagram.DIAGRAM_NAME].ToString();
            LayoutDocumentPanel.Children.Add(myly);
            OpenedDocuments.Add(myly);
            P.TheCurrentLayout = myly;
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
                    //generate a new form to create a new tree risk
                    Operation = General.INSERT,
                    TopRiskTable = myly.Ds.Tables[DT_Diagram_Damages.TABLENAME].Copy(),
                    DRow = myly.Ds.Tables[DT_Diagram.TABLE_NAME].NewRow(),
                    Icon = Icon,
                    IDProject = IdProject
                };
                if (riskTree.ShowDialog() == true)
                {

                    //Style btnStyle = new Style(typeof(Button), ((Button)FindResource("Delete")).Style);
                    //Style sldStyle = new Style(typeof(Slider), ((Slider)FindResource("CommonSlider")).Style);
                    //Style cbStyle = new Style(typeof(ComboBox), ((ComboBox)FindResource("CommonCB")).Style);

                    //myly.BtMinus.Style = myly.BtMPlus.Style = btnStyle;
                    //myly.SliderZoom.Style = sldStyle;
                    //myly.CbFilterTopR.Style = myly.TheZoomComboBox.Style = cbStyle;
                    myly.DrDiagram = riskTree.DRow;
                    myly.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Add(myly.DrDiagram);
                    myly.Ds.Tables[DT_Diagram_Damages.TABLENAME].Merge(riskTree.TopRiskTable);

                    DataRow drRisk = myly.Ds.Tables[DT_Risk.TABLE_NAME].NewRow();

                    drRisk[DT_Risk.NAMESHORT] = "Total Risk";
                    drRisk[DT_Risk.COMMENTS] = "Total Risk " + riskTree.DRow[DT_Diagram.DIAGRAM_NAME];
                    drRisk[DT_Risk.IS_ROOT] = true;

                    drRisk[DT_Risk.ISCOLLAPSED] = false;
                    drRisk[DT_Risk.ENABLED] = true;
                    //drRisk[RiskDatos.DIAGONAL_COLUMNA] = false;
                    drRisk[DT_Risk.FROM_TOP] = false;
                    drRisk[DT_Risk.PROBABILITY] = 100;
                    //the tree always can contain only a unique Unit of Mesurement for each Top Risk 

                    drRisk[DT_Risk.ID_DIAGRAM] = riskTree.DRow[DT_Diagram.ID_DIAGRAM];
                    myly.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);

                    DataRow newRow = myly.Ds.Tables[DT_Role_Risk.TABLENAME].NewRow();
                    newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                    newRow[DT_Role_Risk.Role] = myly.Ds.Tables[DT_Role.ROLE_TABLE].Select(DT_Role.ROLE_COLUM + " = 'Administrator'").First()[DT_Role.ROLE_COLUM];
                    newRow[DT_Role_Risk.IDROL_COLUMN] = myly.Ds.Tables[DT_Role.ROLE_TABLE].Select(DT_Role.ROLE_COLUM + " = 'Administrator'").First()[DT_Role.IDROL_COLUMN];
                    myly.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);

                    //SaveData(false);
                    System.Drawing.Color color = System.Drawing.Color.FromArgb(int.Parse(myly.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + riskTree.DRow[DT_Diagram.ID_DIAGRAM]).First()[DT_Diagram_Damages.COLOR].ToString()));
                    myly.Title = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                    LayoutDocumentPanel.Children.Add(myly);
                    OpenedDocuments.Add(myly);

                    P.TheCurrentLayout = myly;
                    P.TheCurrentLayout.MoviendoRisk = false;
                    P.TheCurrentLayout.ID_Diagram = (Int32)riskTree.DRow[DT_Diagram.ID_DIAGRAM];
                    P.TheCurrentLayout.AddMainLine(drRisk, color);
                    P.TheCurrentLayout.LoadComboDamage();
                    P.TheCurrentLayout.LoadRectangles();
                    TextProbability.Text = "0";
                    BtnBackward.Visibility = Visibility.Hidden;
                    TextDiagram.Text = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                    P.TheCurrentLayout.Title = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                    P.TheCurrentLayout.FixDrawPanel();
                    foreach (var item in OpenedDocuments)
                    {
                        item.ExitWorking();
                    }
                    P.TheCurrentLayout.EnterWorking();
                }

            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
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
                    int DiagramID = 0;
                    DiagramID = (int)DVRisk_Tree[Indexx].Row[DT_Diagram.ID_DIAGRAM];
                    MyLayoutDocumentt openDoc = OpenedDocuments.FirstOrDefault(docu => docu.ID_Diagram == (int)DVRisk_Tree[Indexx][DT_Diagram.ID_DIAGRAM]);

                    if (openDoc != null)
                    {
                        if (new WindowMessageYesNo("This diagram is already opened. Do you want to open it as New Diagram?").ShowDialog() == true)
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
                            //LA LINEA MAS IMPORTANTE EN ESTA PARTE
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

                                //Style btnStyle = new Style(typeof(Button), ((Button)FindResource("Delete")).Style);
                                //Style sldStyle = new Style(typeof(Slider), ((Slider)FindResource("CommonSlider")).Style);
                                //Style cbStyle = new Style(typeof(ComboBox), ((ComboBox)FindResource("CommonCB")).Style);

                                //myly.BtMinus.Style = myly.BtMPlus.Style = btnStyle;
                                //myly.SliderZoom.Style = sldStyle;
                                //myly.CbFilterTopR.Style = myly.TheZoomComboBox.Style = cbStyle;

                                //myly.GridPaintLines.Width = 200;
                                //myly.GridPaintLines.Height = 200;
                                //myly.MIdPoint = new Point(myly.GridPaintLines.Width - 180, myly.GridPaintLines.Height / 2);

                                SetNewDamageToEntireTree(myly.ID_Diagram, myly.Ds);

                                TextDiagram.Text = riskTree.TextName.Text;
                                TheProgress.Visibility = Visibility.Visible;
                                HabilitarBotones(false);
                                myly.Title = riskTree.TextName.Text;
                                LayoutDocumentPanel.Children.Add(myly);
                                OpenedDocuments.Add(myly);
                                P.TheCurrentLayout = myly;
                                //myly.Ds = DsMain;
                                CambiosVisuales();
                            }
                        }
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

                            //Style btnStyle = new Style(typeof(Button), ((Button)FindResource("Delete")).Style);
                            //Style sldStyle = new Style(typeof(Slider), ((Slider)FindResource("CommonSlider")).Style);
                            //Style cbStyle = new Style(typeof(ComboBox), ((ComboBox)FindResource("CommonCB")).Style);

                            //myly.BtMinus.Style = myly.BtMPlus.Style = btnStyle;
                            //myly.SliderZoom.Style = sldStyle;
                            //myly.CbFilterTopR.Style = myly.TheZoomComboBox.Style = cbStyle;

                            //myly.GridPaintLines.Width = 200;
                            //myly.GridPaintLines.Height = 200;
                            //myly.MIdPoint = new Point(myly.GridPaintLines.Width - 180, myly.GridPaintLines.Height / 2);
                            myly.ID_Diagram = DiagramID;

                            SetNewDamageToEntireTree(myly.ID_Diagram, myly.Ds);

                            TheProgress.Visibility = Visibility.Visible;
                            HabilitarBotones(false);
                            //int idRiskTree = (Int32)riskTree.DRow[DT_RiskTree.ID_RISK_TREE];
                            //await Task.Run(() =>
                            //{      
                            //    //Aca estaba la linea de agregar segun los dannos
                            //    SaveDataAsync();
                            //});
                            ///por cada daño del diagrama
                            myly.Title = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                            LayoutDocumentPanel.Children.Add(myly);
                            OpenedDocuments.Add(myly);
                            P.TheCurrentLayout = myly;
                            CambiosVisuales();
                            //myly.Ds = DsMain;
                            //Thread thread = new Thread(delegate () {  })
                            //{
                            //    IsBackground = true
                            //};
                            //thread.Start();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                new WindowMessageOK(ex.Message).ShowDialog();
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
                CreandoDamagesDiagram(destinyLayout.Ds, drDiagram, originalLayout.Ds);

                CreandoRisks(originalLayout, destinyLayout, destinyLayout.LinesList);

                CreandoCMs(originalLayout, destinyLayout, destinyLayout.LinesList);               

                TreeOperation.Build_Tree(destinyLayout.LinesList);
                destinyLayout.CopyRisk = destinyLayout.LinesList.FirstOrDefault(p => p.IsRoot);

                DataRow drRisk = destinyLayout.Ds.Tables[DT_Risk.TABLE_NAME].NewRow();
                //drRisk[DT_Risk.CAUSESRISKID_COLUMNA] = theLayout.Line_Selected.ID;
                drRisk[DT_Risk.COMMENTS] = "Detail Total Risk";
                drRisk[DT_Risk.ENABLED] = true;
                //drRisk[DT_Risk.FROM_TOP_COLUMNA] = theLayout.CopyRisk.FromTop;
                drRisk[DT_Risk.ID_DIAGRAM] = drDiagram[DT_Diagram.ID_DIAGRAM];
                drRisk[DT_Risk.ISCOLLAPSED] = false;
                drRisk[DT_Risk.IS_ROOT] = true;
                drRisk[DT_Risk.NAMESHORT] = "Total Risk " + drDiagram[DT_Diagram.DIAGRAM_NAME];
                drRisk[DT_Risk.POSITION] = 0;
                drRisk[DT_Risk.PROBABILITY] = 0;
                drRisk[DT_Risk.IDRISK_FATHER] = 0;
                destinyLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);

                CreandoDamagesWBSAndRole_to_Risk(destinyLayout.Ds, originalLayout.Ds, drRisk, drDiagram, drDiagram[DT_Diagram.DIAGRAM_NAME].ToString(), thecopiedline);

                destinyLayout.CopyRisk.ID = (Int32)drRisk[DT_Risk.ID];

                Paste(destinyLayout.CopyRisk, destinyLayout.Ds.Copy(), destinyLayout.Ds, (Int32)drDiagram[DT_Diagram.ID_DIAGRAM], destinyLayout.LinesList);//Aca pego el resto del diagrama

                destinyLayout.CopyRisk.IdRiskFather = 0;
                return (Int32)drDiagram[DT_Diagram.ID_DIAGRAM];
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
                        drDamage[DT_Risk_Damages.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString() + "(" + item[DT_Diagram_Damages.UM].ToString() + ")";
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
                        drDamage[DT_CounterM_Damage.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString() + "(" + item[DT_Diagram_Damages.UM].ToString() + ")";
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
            P.TheCurrentLayout.TheProgressBar.Visibility = Visibility.Collapsed;
            //p.ProgressVisible = false;
            //p.ProgressIsIndeterminate = false;
        }

        private void CambiosVisuales()
        {
            try
            {
                P.TheCurrentLayout.LoadComboDamage();
                P.TheCurrentLayout.LoadLines();

                P.TheCurrentLayout.LoadRectangles();
                P.TheCurrentLayout.DrawNumbers();
                P.TheCurrentLayout.SetLinesThickness();
                TextProbabilityChange(P.TheCurrentLayout.MainLine);
                
                BtnBackward.Visibility = Visibility.Hidden;

                CruzarTablaRisk(P.TheCurrentLayout.Ds);
                CruzarTablaCM(P.TheCurrentLayout.Ds);
                OnResponse();
                foreach (var item in OpenedDocuments)
                {
                    item.ExitWorking();
                }
                P.TheCurrentLayout.EnterWorking();
                TheProgress.Visibility = Visibility.Hidden;
                //P.TheCurrentLayout.MoveVisualToFishHead();
                HabilitarBotones(true);
            }
            catch (Exception ex)
            {
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }
        #endregion

        /// <summary>
        /// Delete an existing Diagram
        /// </summary>
        private void RemoveTree_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgTreeDiagrams.SelectedIndex >= 0)
                {
                    if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " the diagram [" + DVRisk_Tree[dgTreeDiagrams.SelectedIndex].Row[DT_Diagram.DIAGRAM_NAME] + "]?").ShowDialog() == true)
                    {
                        int id = (Int32)DVRisk_Tree[dgTreeDiagrams.SelectedIndex].Row[DT_Diagram.ID_DIAGRAM];

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
                        TextDiagram.Text = "None";
                        TextProbability.Text = "0";
                        DVRisk_Tree[dgTreeDiagrams.SelectedIndex].Delete();
                        SaveData(DsMain, true);
                        //CruzarTablaRisk();
                        //CruzarTablaCM();
                    }
                }
                //DataRow fila = DVRisk_Tree[dgTreeDiagrams.SelectedIndex].Row;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        #region ImportFromExcel
        /// <summary>
        /// Descontinuado pero no lo borro para hacer el otro asincronico
        /// </summary>
        //private async Task ImportarExcel(DataSet dsImporting, Excel.Application Aplicacion)
        //{
        //    using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog() { Filter = "Excel WorkBook|*.xlsx|Excel WorkBook 97-2003|*.xls", ValidateNames = true })
        //    {
        //        if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        //        {
        //            WindowText wt = new WindowText
        //            {
        //                MyTitle = "Key Word for Disable"
        //            };
        //            wt.txtKeyword.Focus();
        //            if (wt.ShowDialog() == true)
        //            {
        //                //importante Yordan, desde aca
        //                DataRow drDiagrams = dsImporting.Tables[DT_Diagram.TABLE_NAME].NewRow();
        //                Cursor = Cursors.No;
        //                //p.ProgressVisible = true;
        //                TheProgress.Visibility = Visibility.Visible;
        //                HabilitarBotones(false);

        //                await Task.Run(() =>
        //                {
        //                    Excel.Workbook Libro = Aplicacion.Workbooks.Open(ofd.FileName, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
        //                    Excel._Worksheet Hoja = (Excel._Worksheet)Libro.Sheets[1];
        //                    Excel.Range Rango = Hoja.UsedRange;

        //                    int rowCount = Rango.Rows.Count;
        //                    int colCount = Rango.Columns.Count;
        //                    List<HeaderExcelContent> listaHeader = new List<HeaderExcelContent>();
        //                    //int filaHeader = 0;
        //                    //int rowHeader = 2;
        //                    int rowHeader = 0;
        //                    for (int col = 1; col <= colCount; col++)
        //                    {
        //                        for (int row = 1; row <= rowCount; row++)
        //                        {
        //                            if (Rango.Cells[row, col] != null && Rango.Cells[row, col].Value2 != null)
        //                            {
        //                                rowHeader = row;
        //                                row = rowCount + 1;
        //                            }
        //                        }
        //                        if (Rango.Cells[rowHeader, col] != null && Rango.Cells[rowHeader, col].Value2 != null)
        //                        {
        //                            listaHeader.Add(new HeaderExcelContent(Rango.Cells[rowHeader, col].Value2.ToString(), col));
        //                        }
        //                    }
        //                    DataSet dsClasification = new DataSet();
        //                    ServiceClasifications.WebServiceClasificator wsClasification = new ServiceClasifications.WebServiceClasificator();
        //                    dsClasification = wsClasification.GetAllClasifications();
        //                    WindowHeaderClasification whc = new WindowHeaderClasification
        //                    {
        //                        MyDataset = dsClasification.Copy(),
        //                        MyList = listaHeader
        //                    };
        //                    if (whc.ShowDialog() == true)
        //                    {

        //                    }
        //                    //int rowToStart = 3;  
        //                    int rowToStart = rowHeader + 1;
        //                    //columna 2: IdRiesgo
        //                    int colIdRisk = 2;
        //                    //columna 3: Shortname
        //                    int colShortRisk = 3;
        //                    //Columna 18: idHijo
        //                    int colIdHijo = 18;
        //                    //Columna 20-21-22 va a details
        //                    int[] colRiskDetail = new int[] { 20, 21, 22 };
        //                    //28-34 damages
        //                    int colminDamage = 28; int colmaxDamage = 34;
        //                    //35 IDRISK

        //                    //36-37 idCM y ShortnameCM
        //                    int colCmShort = 35; int colCMDetail = 36; int colStatus = 40;
        //                    //columna 41: Probabilidad
        //                    int colProb = 41;
        //                    //var idrisk = Rango.Cells[3, 2].Value2;
        //                    //var idcm = Rango.Cells[3, 36].Value2;

        //                    ServiceTopRiskController.WebServiceTopRisk wstop = new ServiceTopRiskController.WebServiceTopRisk();
        //                    dsImporting.Tables[DT_Damage.TopRisk_TABLA].Merge(wstop.GetAllTopRisk().Tables[DT_Damage.TopRisk_TABLA]);
        //                    //drDiagrams = Ds.Tables[DT_RiskTree.RiskTree_TABLA].NewRow();
        //                    drDiagrams[DT_Diagram.DIAGRAM_NAME] = "Imported Diagram at " + DateTime.Now;
        //                    drDiagrams[DT_Diagram.ID_DIAGRAM] = 0000;
        //                    drDiagrams[DT_Diagram.ID_PROJECT] = IdProject;
        //                    dsImporting.Tables[DT_Diagram.TABLE_NAME].Rows.Add(drDiagrams);
        //                    for (int j = colminDamage; j <= colmaxDamage; j++)//por cada columna de Damages
        //                    {
        //                        if (Rango.Cells[rowHeader, j] != null && Rango.Cells[rowHeader, j].Value2 != null)
        //                        {
        //                            string TopRisk = Rango.Cells[rowHeader, j].Value2.ToString();//separo el daño de la UM
        //                                                                                         //string Damage = TopRisk.Split(new char[] { '(', ')' })[0];
        //                                                                                         //string UM = TopRisk.Split(new char[] { '(', ')' })[1];
        //                            if (!(dsImporting.Tables[DT_Damage.TopRisk_TABLA].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + TopRisk + "'").Any()))//si el nombre del daño no existe 
        //                            {
        //                                DataRow drDamage = dsImporting.Tables[DT_Damage.TopRisk_TABLA].NewRow();//creo un nuevo daño
        //                                drDamage[DT_Damage.TOP_RISK_COLUMN] = TopRisk;

        //                                drDamage[DT_Damage.COLORID_COLUMNA] = System.Drawing.Color.Blue.ToArgb().ToString();
        //                                dsImporting.Tables[DT_Damage.TopRisk_TABLA].Rows.Add(drDamage);

        //                                DataRow drDamage_Diagram = dsImporting.Tables[DT_Diagram_Damages.TABLENAME].NewRow();//y creo un nuevo diagrama con los daños que acabo de crear
        //                                drDamage_Diagram[DT_Diagram_Damages.ID_DAMAGE] = drDamage[DT_Damage.ID_COLUMNA];
        //                                drDamage_Diagram[DT_Diagram_Damages.COLOR] = drDamage[DT_Damage.COLORID_COLUMNA];
        //                                drDamage_Diagram[DT_Diagram_Damages.RISK_TREE] = "Imported Diagram";
        //                                drDamage_Diagram[DT_Diagram_Damages.UM] = "UM";
        //                                drDamage_Diagram[DT_Diagram_Damages.ID_RISKTREE] = drDiagrams[DT_Diagram.ID_DIAGRAM];
        //                                drDamage_Diagram[DT_Diagram_Damages.DAMAGE] = TopRisk;
        //                                dsImporting.Tables[DT_Diagram_Damages.TABLENAME].Rows.Add(drDamage_Diagram);
        //                            }
        //                            else
        //                            {
        //                                DataRow drDamage_Diagram = dsImporting.Tables[DT_Diagram_Damages.TABLENAME].NewRow();//y creo un nuevo diagrama con los daños que acabo de crear
        //                                drDamage_Diagram[DT_Diagram_Damages.ID_DAMAGE] = dsImporting.Tables[DT_Damage.TopRisk_TABLA].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + TopRisk + "'").First()[DT_Damage.ID_COLUMNA];
        //                                drDamage_Diagram[DT_Diagram_Damages.COLOR] = dsImporting.Tables[DT_Damage.TopRisk_TABLA].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + TopRisk + "'").First()[DT_Damage.COLORID_COLUMNA];
        //                                drDamage_Diagram[DT_Diagram_Damages.RISK_TREE] = "Imported Diagram";
        //                                drDamage_Diagram[DT_Diagram_Damages.UM] = "UM";
        //                                drDamage_Diagram[DT_Diagram_Damages.ID_RISKTREE] = drDiagrams[DT_Diagram.ID_DIAGRAM];
        //                                drDamage_Diagram[DT_Diagram_Damages.DAMAGE] = TopRisk;
        //                                dsImporting.Tables[DT_Diagram_Damages.TABLENAME].Rows.Add(drDamage_Diagram);
        //                            }
        //                        }
        //                    }
        //                    //BUsco el diagrama que acabo de insertar, para agregarle el riesgo padre, para agregarle los riesgos y sus dannos
        //                    DataRow theDiagram = dsImporting.Tables[DT_Diagram.TABLE_NAME].Rows.Find(0000);
        //                    //creo un riesgo root
        //                    DataRow drRisk = dsImporting.Tables[DT_Risk.TABLE_NAME].NewRow();
        //                    drRisk[DT_Risk.ID] = 0000;
        //                    drRisk[DT_Risk.NAMESHORT] = "Root " + theDiagram[DT_Diagram.DIAGRAM_NAME];
        //                    drRisk[DT_Risk.COMMENTS] = "Total Risk " + theDiagram[DT_Diagram.DIAGRAM_NAME];
        //                    drRisk[DT_Risk.IS_ROOT] = true;

        //                    drRisk[DT_Risk.ISCOLLAPSED] = false;
        //                    drRisk[DT_Risk.ENABLED] = true;
        //                    drRisk[DT_Risk.FROM_TOP] = false;
        //                    drRisk[DT_Risk.PROBABILITY] = 100;
        //                    drRisk[DT_Risk.POSITION] = 0;
        //                    drRisk[DT_Risk.ID_DIAGRAM] = theDiagram[DT_Diagram.ID_DIAGRAM];
        //                    dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);

        //                    //Asignarle al riesgo Root el rol admin
        //                    DataRow drRiskRole = dsImporting.Tables[DT_Role_Risk.TABLENAME].NewRow();
        //                    drRiskRole[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
        //                    drRiskRole[DT_Role_Risk.IDROL_COLUMN] = 101;
        //                    dsImporting.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRiskRole);

        //                    //por cada daño del diagrama
        //                    foreach (DataRow itemDiagramDamage in dsImporting.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + theDiagram[DT_Diagram.ID_DIAGRAM]))
        //                    {
        //                        //darle valor 0, porque es la raiz
        //                        DataRow drRiskDamage = dsImporting.Tables[DT_Risk_Damages.TABLENAME].NewRow();
        //                        drRiskDamage[DT_Risk_Damages.ID_DAMAGE] = itemDiagramDamage[DT_Diagram_Damages.ID_DAMAGE];
        //                        drRiskDamage[DT_Risk_Damages.ID_RISK] = drRisk[DT_Risk.ID];
        //                        drRiskDamage[DT_Risk_Damages.ID_RISK_TREE] = theDiagram[DT_Diagram.ID_DIAGRAM];
        //                        drRiskDamage[DT_Risk_Damages.VALUE] = 0;
        //                        dsImporting.Tables[DT_Risk_Damages.TABLENAME].Rows.Add(drRiskDamage);
        //                    }
        //                    //Recorrer el Excel solo para llenar los riesgos
        //                    for (int i = rowToStart; i <= rowCount; i++)
        //                    {
        //                        DataRow drRiskN = dsImporting.Tables[DT_Risk.TABLE_NAME].NewRow();
        //                        if (Rango.Cells[i, colIdRisk] != null && Rango.Cells[i, colIdRisk].Value2 != null)
        //                        {
        //                            drRiskN[DT_Risk.ID] = Convert.ToInt32(((object)Rango.Cells[i, colIdRisk].Value2).ToString().Split(new char[] { '-' })[1]);
        //                            if (Rango.Cells[i, colIdRisk] != null && Rango.Cells[i, colIdRisk].Value2 != null)
        //                            {
        //                                drRiskN[DT_Risk.NAMESHORT] = Rango.Cells[i, colIdRisk].Value2;
        //                            }
        //                            if (Rango.Cells[i, colShortRisk] != null && Rango.Cells[i, colShortRisk].Value2 != null)
        //                            {
        //                                drRiskN[DT_Risk.NAMESHORT] = drRiskN[DT_Risk.NAMESHORT] + " " + Rango.Cells[i, colShortRisk].Value2;
        //                            }
        //                            if (Rango.Cells[i, colRiskDetail[0]] != null && Rango.Cells[i, colRiskDetail[0]].Value2 != null)
        //                            {
        //                                drRiskN[DT_Risk.COMMENTS] = Rango.Cells[rowHeader, colRiskDetail[0]].Value2 + ": " + Rango.Cells[i, colRiskDetail[0]].Value2;
        //                            }
        //                            for (int k = 1; k < colRiskDetail.Length; k++)
        //                            {
        //                                if (Rango.Cells[i, colRiskDetail[k]] != null && Rango.Cells[i, colRiskDetail[k]].Value2 != null)
        //                                {
        //                                    drRiskN[DT_Risk.COMMENTS] = drRiskN[DT_Risk.COMMENTS].ToString() + "\n"
        //                                    + Rango.Cells[rowHeader, colRiskDetail[k]].Value2 + ": " + Rango.Cells[i, colRiskDetail[k]].Value2;
        //                                }
        //                            }
        //                            drRiskN[DT_Risk.IS_ROOT] = false;
        //                            drRiskN[DT_Risk.ISCOLLAPSED] = false;
        //                            drRiskN[DT_Risk.ENABLED] = true;
        //                            drRiskN[DT_Risk.POSITION] = 0;
        //                            drRiskN[DT_Risk.FROM_TOP] = false;
        //                            drRiskN[DT_Risk.ID_DIAGRAM] = theDiagram[DT_Diagram.ID_DIAGRAM];

        //                            if (Rango.Cells[i, colIdHijo] != null && Rango.Cells[i, colProb].Value2 != null)
        //                            {
        //                                drRiskN[DT_Risk.PROBABILITY] = Convert.ToDecimal(Rango.Cells[i, colProb].Value2);
        //                            }
        //                            else
        //                            {
        //                                drRiskN[DT_Risk.PROBABILITY] = 0;
        //                            }

        //                            drRiskN[DT_Risk.IDRISK_FATHER] = drRisk[DT_Risk.ID];
        //                            DataRow drStructure = dsImporting.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
        //                            drStructure[DT_RiskStructure.IDRISK] = drRiskN[DT_Risk.ID];
        //                            drStructure[DT_RiskStructure.IDRISK_FATHER] = drRisk[DT_Risk.ID];
        //                            dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(drStructure);

        //                            for (int j = colminDamage; j <= colmaxDamage; j++)
        //                            {//por cada Damage, del Diagrama, le agrega el valor al riesgo
        //                                if (Rango.Cells[rowHeader, j] != null && Rango.Cells[rowHeader, j].Value2 != null)
        //                                {
        //                                    string TopRisk = Rango.Cells[rowHeader, j].Value2.ToString();
        //                                    DataRow drRiskDamageN = dsImporting.Tables[DT_Risk_Damages.TABLENAME].NewRow();
        //                                    drRiskDamageN[DT_Risk_Damages.ID_DAMAGE] = dsImporting.Tables[DT_Damage.TopRisk_TABLA].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + TopRisk + "'").First()[DT_Damage.ID_COLUMNA];
        //                                    drRiskDamageN[DT_Risk_Damages.ID_RISK] = drRiskN[DT_Risk.ID];
        //                                    drRiskDamageN[DT_Risk_Damages.ID_RISK_TREE] = theDiagram[DT_Diagram.ID_DIAGRAM];
        //                                    if (Rango.Cells[i, j] != null && Rango.Cells[i, j].Value2 != null)
        //                                    {
        //                                        drRiskDamageN[DT_Risk_Damages.VALUE] = Convert.ToDecimal(Rango.Cells[i, j].Value2.ToString());
        //                                    }
        //                                    else
        //                                    {
        //                                        drRiskDamageN[DT_Risk_Damages.VALUE] = 0;
        //                                    }
        //                                    dsImporting.Tables[DT_Risk_Damages.TABLENAME].Rows.Add(drRiskDamageN);
        //                                }
        //                            }
        //                            //por cada riesgo, le agrega el rol admin
        //                            DataRow drRiskRoleN = dsImporting.Tables[DT_Role_Risk.TABLENAME].NewRow();
        //                            drRiskRoleN[DT_Role_Risk.ID_RISK] = drRiskN[DT_Risk.ID];
        //                            drRiskRoleN[DT_Role_Risk.IDROL_COLUMN] = 101;
        //                            dsImporting.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRiskRoleN);
        //                            dsImporting.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRiskN);
        //                        }

        //                    }

        //                    for (int i = rowToStart; i <= rowCount; i++)
        //                    {//ajustando estructura
        //                        if (Rango.Cells[i, colIdHijo] != null && Rango.Cells[i, colIdHijo].Value2 != null && Rango.Cells[i, colIdRisk] != null && Rango.Cells[i, colIdRisk].Value2 != null)
        //                        {
        //                            int idHijo = Convert.ToInt32(((object)Rango.Cells[i, colIdHijo].Value2).ToString().Split(new char[] { '-' })[1]);
        //                            int idPadre = Convert.ToInt32(((object)Rango.Cells[i, colIdRisk].Value2).ToString().Split(new char[] { '-' })[1]);
        //                            if (dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + idHijo).Any())
        //                            {
        //                                dsImporting.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + idHijo).First()[DT_RiskStructure.IDRISK_FATHER] = idPadre;
        //                            }
        //                        }
        //                    }
        //                    for (int i = rowToStart; i <= rowCount; i++)
        //                    {//agregando CM
        //                        if (Rango.Cells[i, colCMDetail] != null && Rango.Cells[i, colCMDetail].Value2 != null && Rango.Cells[i, colCmShort] != null && Rango.Cells[i, colCmShort].Value2 != null)
        //                        {
        //                            DataRow drCM = dsImporting.Tables[DT_CounterM.TABLE_NAME].NewRow();
        //                            drCM[DT_CounterM.ID_RISK_TREE] = theDiagram[DT_Diagram.ID_DIAGRAM];
        //                            drCM[DT_CounterM.DETAIL] = Rango.Cells[i, colCMDetail].Value2;
        //                            if (Rango.Cells[i, colStatus] != null && Rango.Cells[i, colStatus].Value2 != null)
        //                            {
        //                                if (wt.KeyWord == ((object)Rango.Cells[i, colStatus].Value2).ToString())
        //                                {
        //                                    drCM[DT_CounterM.ENABLED] = false;
        //                                }
        //                                else
        //                                {
        //                                    drCM[DT_CounterM.ENABLED] = true;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                drCM[DT_CounterM.ENABLED] = true;
        //                            }
        //                            drCM[DT_CounterM.DIAGONAL] = false;
        //                            drCM[DT_CounterM.FROM_TOP] = true;
        //                            drCM[DT_CounterM.NAMESHORT] = Rango.Cells[i, colCmShort].Value2;
        //                            drCM[DT_CounterM.POSITION] = 0;
        //                            drCM[DT_CounterM.PROBABILITY] = 0;
        //                            if (Rango.Cells[i, colIdRisk] != null && Rango.Cells[i, colIdRisk].Value2 != null)
        //                            {
        //                                drCM[DT_CounterM.ID_RISK] = Convert.ToInt32(((object)Rango.Cells[i, colIdRisk].Value2).ToString().Split(new char[] { '-' })[1]);
        //                            }
        //                            else
        //                            {
        //                                drCM[DT_CounterM.ID_RISK] = dsImporting.Tables[DT_CounterM.TABLE_NAME].Rows[dsImporting.Tables[DT_CounterM.TABLE_NAME].Rows.Count - 1][DT_CounterM.ID_RISK];
        //                            }

        //                            for (int j = colminDamage; j <= colmaxDamage; j++)
        //                            {//por cada Damage, del Diagrama, le agrega el valor a la CM
        //                                if (Rango.Cells[rowHeader, j] != null && Rango.Cells[rowHeader, j].Value2 != null)
        //                                {
        //                                    string TopRisk = Rango.Cells[rowHeader, j].Value2.ToString();
        //                                    DataRow drCM_Damage = dsImporting.Tables[DT_CounterM_Damage.TABLENAME].NewRow();
        //                                    drCM_Damage[DT_CounterM_Damage.ID_DAMAGE] = dsImporting.Tables[DT_Damage.TopRisk_TABLA].Select(DT_Damage.TOP_RISK_COLUMN + " = '" + TopRisk + "'").First()[DT_Damage.ID_COLUMNA];
        //                                    drCM_Damage[DT_CounterM_Damage.ID_COUNTERM] = drCM[DT_CounterM.ID];
        //                                    drCM_Damage[DT_CounterM_Damage.ID_RISK_TREE] = theDiagram[DT_Diagram.ID_DIAGRAM];
        //                                    drCM_Damage[DT_CounterM_Damage.VALUE] = 0;
        //                                    dsImporting.Tables[DT_CounterM_Damage.TABLENAME].Rows.Add(drCM_Damage);
        //                                }
        //                            }
        //                            DataRow drRoleCM = dsImporting.Tables[DT_Role_CM.TABLENAME].NewRow();
        //                            drRoleCM[DT_Role_CM.ID_CM] = drCM[DT_CounterM.ID];
        //                            drRoleCM[DT_Role_CM.IDROL_COLUMN] = 101;
        //                            dsImporting.Tables[DT_Role_CM.TABLENAME].Rows.Add(drRoleCM);
        //                            dsImporting.Tables[DT_CounterM.TABLE_NAME].Rows.Add(drCM);
        //                        }
        //                    }
        //                    //textProgress.Text = "Finishing...";
        //                    if (Libro != null)
        //                    {
        //                        Libro.Close();
        //                        Aplicacion.Quit();

        //                        Marshal.ReleaseComObject(Hoja);
        //                        Marshal.ReleaseComObject(Libro);
        //                        Marshal.ReleaseComObject(Aplicacion);
        //                    }
        //                });

        //                //Hasta aca yordan, todo esto pasa en memoria
        //                this.Dispatcher.Invoke(() =>
        //                {
        //                    if (dsImporting.HasChanges())
        //                    {
        //                        //Cursor = Cursors.Wait;
        //                        ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk();
        //                        DataSet temp = new DataSet();
        //                        temp = dsImporting.GetChanges();
        //                        temp = ws.SaveRisk(temp);
        //                        dsImporting.Merge(temp);
        //                        dsImporting.AcceptChanges();
        //                        //textProgress.Text = "Ordering...";
        //                        ServiceRiskController.WebServiceRisk tws = new ServiceRiskController.WebServiceRisk();
        //                        UserDataSet dsT1 = new UserDataSet();
        //                        dsT1.Merge(tws.GetRiskTreeID(new object[] { (Int32)drDiagrams[DT_Diagram.ID_DIAGRAM] }));
        //                        TreeOperation.AjustarPosicionHijos(TreeOperation.LoadLines(dsT1, (Int32)drDiagrams[DT_Diagram.ID_DIAGRAM]).Find(x => x.IsRoot == true), dsImporting);
        //                        DataSet tempi = new DataSet();
        //                        tempi = dsImporting.GetChanges();
        //                        tempi = tws.SaveRisk(tempi);
        //                        dsImporting.Merge(tempi);
        //                        dsImporting.AcceptChanges();
        //                        RefreshData();
        //                        //p.ProgressVisible = false;
        //                        TheProgress.Visibility = Visibility.Hidden;
        //                        Cursor = Cursors.Arrow;
        //                        new WindowAlert("Importation file success!!").ShowDialog();
        //                        HabilitarBotones(true);
        //                    }
        //                });
        //            }
        //        }
        //    }
        //}
        
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
                                        dsT1.Merge(tws.GetRiskTreeID(new object[] { (Int32)drDiagram[DT_Diagram.ID_DIAGRAM] }));
                                        TreeOperation.AjustarPosicionHijosInExcel(TreeOperation.LoadLines(dsT1, (Int32)drDiagram[DT_Diagram.ID_DIAGRAM]).Find(x => x.IsRoot == true), dsImporting);
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
                                        new WindowAlert("Importation file success!!").ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
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
                drDamage_Diagram[DT_Diagram_Damages.UM] = "UM";
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
                drDamage_Diagram[DT_Diagram_Damages.UM] = "UM";
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
            foreach (DataRow item in DsWBS.Tables[DT_WBS.TABLE_NAME].Select())
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
            if (OpenedDocuments.Count != 0 && P.TheCurrentLayout != null && P.TheCurrentLayout.ID_Diagram >= 0 && !P.TheCurrentLayout.IsExportingToExcel)
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
                        P.TheCurrentLayout.ExportToExcel(fileName);
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
            P.TheCurrentLayout.TheProgressBar.Value = e.ProgressPercentage;
        }
        void ExportToExcelWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            P.TheCurrentLayout.TheProgressBar.Value = 100;
            IFormatProvider formatProvider = CultureInfo.CurrentUICulture;
            MessageBox.Show(String.Format(formatProvider, "RiskTree {0} was saved as excel file!", P.TheCurrentLayout.Title));
            P.TheCurrentLayout.TheProgressBar.Visibility = Visibility.Collapsed;
            P.TheCurrentLayout.TheProgressBar.Value = 0;

            P.TheCurrentLayout.IsExportingToExcel = false;
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
                    RiskRow = P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].NewRow(),
                    Ds = P.TheCurrentLayout.Ds,
                    LOGIN_USER = LoginUser,
                    ID_PROJECT = (int)P.TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(P.TheCurrentLayout.ID_Diagram)[DT_Diagram.ID_PROJECT],
                    Risk_RoleTable = P.TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME],
                    WBS_RISK_Damage = P.TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME],
                    WBS_CM_Damage = P.TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME],
                    Risk_WBS_Table = P.TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME],
                    Risk_DamageTable = P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME],
                    CM_DamageTable = P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME],
                    Operation = General.INSERT,
                    RowFather = P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(P.TheCurrentLayout.Line_Selected.ID),
                    RiskTreeID = P.TheCurrentLayout.ID_Diagram,
                    RiskSelected = P.TheCurrentLayout.Line_Selected,
                    Icon = Icon,
                    MyRisks = P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Copy()
                };
                wrisk.Pi.HasAccess = true;
                if (wrisk.ShowDialog() == true)
                {
                    P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(wrisk.RiskRow);
                    DataRow rowstructure = P.TheCurrentLayout.Ds.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                    rowstructure[DT_RiskStructure.IDRISK] = wrisk.RiskRow[DT_Risk.ID];
                    rowstructure[DT_RiskStructure.IDRISK_FATHER] = wrisk.RowFather[DT_Risk.ID];
                    P.TheCurrentLayout.Ds.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);
                    RiskPolyLine Line_Created = new RiskPolyLine
                    {
                        ID = (int)wrisk.RiskRow[DT_Risk.ID],
                        IsCM = false,
                        ShortName = "LineCreated",
                        Father = P.TheCurrentLayout.Line_Selected,
                        IdRiskFather = P.TheCurrentLayout.Line_Selected.ID
                    };
                    P.TheCurrentLayout.InsertRisk(Line_Created, P.TheCurrentLayout.Line_Selected, P.TheCurrentLayout.PointSelected);

                    P.TheCurrentLayout.DropLines();
                    P.TheCurrentLayout.DropRectangles();
                    P.TheCurrentLayout.LoadLines();
                    P.TheCurrentLayout.LoadRectangles();
                    P.TheCurrentLayout.DrawNumbers();
                    P.TheCurrentLayout.SetLinesThickness();
                    TextProbabilityChange(P.TheCurrentLayout.MainLine);
                    CruzarTablaRisk(P.TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void RiskCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (P.TheCurrentLayout != null)
                {
                    P.TheCurrentLayout.GridPaintLines.Children.Remove(P.TheCurrentLayout.Line_Created);
                    System.Drawing.Color lnColor = System.Drawing.Color.FromArgb(int.Parse(P.TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + P.TheCurrentLayout.ID_Diagram)[P.TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                    //HACER: comando add risk              
                    P.TheCurrentLayout.Line_Created = new RiskPolyLine(P.TheCurrentLayout.GridPaintLines, MenuRisk, false)
                    {
                        Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(lnColor.A, lnColor.R, lnColor.G, lnColor.B)),
                        StrokeThickness = 3
                    };
                    P.TheCurrentLayout.Line_Created.NewDrawAtPoint(new Point(P.TheCurrentLayout.X, P.TheCurrentLayout.Y), "");
                    P.TheCurrentLayout.Creando = true;
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                    RiskRow = P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(P.TheCurrentLayout.Line_Selected.ID),
                    Ds = P.TheCurrentLayout.Ds,
                    ID_PROJECT = (Int32)P.TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(P.TheCurrentLayout.ID_Diagram)[DT_Diagram.ID_PROJECT],
                    Operation = General.UPDATE,
                    LOGIN_USER = LoginUser,
                    Risk_DamageTable = P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME],
                    CM_DamageTable = P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME],
                    WBS_RISK_Damage = P.TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME],
                    WBS_CM_Damage = P.TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME],
                    Risk_WBS_Table = P.TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME],
                    CM_WBS_Table = P.TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME],
                    Risk_RoleTable = P.TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME],
                    CM_RoleTable = P.TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME],
                    Posicion = (Int32)P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(P.TheCurrentLayout.Line_Selected.ID)[DT_Risk.POSITION],
                    Icon = Icon,
                    ChildrenLines = TreeOperation.GetOnlyMyChildrenWithCM(P.TheCurrentLayout.Line_Selected),
                    MyRisks = P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Clone(),
                };
                formRisk.Pi.HasAccess = P.TheCurrentLayout.FullAccess(P.TheCurrentLayout.Line_Selected);
                if (P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains(P.TheCurrentLayout.Line_Selected.ID))
                {
                    formRisk.RowFather = P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(P.TheCurrentLayout.Line_Selected.IdRiskFather);
                }
                formRisk.RiskTreeID = P.TheCurrentLayout.ID_Diagram;

                if (formRisk.ShowDialog() == true)
                {
                    if (P.TheCurrentLayout.Ds.HasChanges())
                    {
                        TreeOperation.SetRiskLineValues(P.TheCurrentLayout.Line_Selected, formRisk.RiskRow);
                        int pos = P.TheCurrentLayout.LinesList.FindIndex(rl => rl.ID == P.TheCurrentLayout.Line_Selected.ID);
                        P.TheCurrentLayout.LinesList[pos] = P.TheCurrentLayout.Line_Selected;
                        TextProbabilityChange(P.TheCurrentLayout.MainLine);
                        P.TheCurrentLayout.DrawNumbers();
                        P.TheCurrentLayout.UpdateLinesValues();
                        P.TheCurrentLayout.SetLinesThickness();
                        CruzarTablaRisk(P.TheCurrentLayout.Ds);
                        CruzarTablaCM(P.TheCurrentLayout.Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        /// <summary>
        /// Delete a Risk and its children
        /// </summary>
        private void MenuDeleteRisk_Click(object sender, RoutedEventArgs e)
        {
            if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + P.TheCurrentLayout.Line_Selected.ShortName + "] and all its children?").ShowDialog() == true)
            {
                TreeOperation.DeleteLine(P.TheCurrentLayout.Line_Selected, P.TheCurrentLayout.Ds);

                P.TheCurrentLayout.DropLines();
                P.TheCurrentLayout.DropRectangles();
                P.TheCurrentLayout.LoadLines();
                P.TheCurrentLayout.LoadRectangles();
                P.TheCurrentLayout.DrawNumbers();
                TextProbabilityChange(P.TheCurrentLayout.MainLine);
                P.TheCurrentLayout.SetLinesThickness();
                CruzarTablaRisk(P.TheCurrentLayout.Ds);
                CruzarTablaCM(P.TheCurrentLayout.Ds);
            }
        }

        /// <summary>
        /// Move a Risk and its children
        /// </summary>
        private void MenuMoveRisk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new VerticalMenu("Moving").ShowDialog();
                P.TheCurrentLayout.MoviendoRisk = true;
                P.TheCurrentLayout.LinesMoving = new List<RiskPolyLine>();
                P.TheCurrentLayout.LinesMoving.AddRange(TreeOperation.GetMeAndMyChildrenWithCM(P.TheCurrentLayout.Line_Selected));
                if (P.TheCurrentLayout != null)
                {
                    P.TheCurrentLayout.GridPaintLines.Children.Remove(P.TheCurrentLayout.LineInMoving);
                    System.Drawing.Color lnColor = System.Drawing.Color.FromArgb(int.Parse(P.TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + P.TheCurrentLayout.ID_Diagram)[P.TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                    P.TheCurrentLayout.LineInMoving = new RiskPolyLine(P.TheCurrentLayout.GridPaintLines, MenuRisk, false)
                    {
                        Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(lnColor.A, lnColor.R, lnColor.G, lnColor.B)),
                        StrokeThickness = 3,
                        IsMoving = true
                    };
                    P.TheCurrentLayout.LineInMoving.NewDrawAtPoint(new Point(P.TheCurrentLayout.X, P.TheCurrentLayout.Y), "");
                }
                foreach (var item in P.TheCurrentLayout.LinesMoving)
                {
                    item.Oculto = true;
                }
                Cursor = Cursors.Hand;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                    ID_Project = (int)P.TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(P.TheCurrentLayout.ID_Diagram)[DT_Diagram.ID_PROJECT],
                    CMRow = P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].NewRow(),
                    DsCM = P.TheCurrentLayout.Ds,
                    LOGIN_USER = LoginUser,
                    CM_RoleTable = P.TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME],
                    CM_WBS_Table = P.TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME],
                    WBS_CM_Damage = P.TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME],
                    TopRiskTable = P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME],
                    Operation = General.INSERT,
                    RowFather = P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(P.TheCurrentLayout.Line_Selected.ID),
                    RiskTreeID = P.TheCurrentLayout.ID_Diagram,
                    RiskPadre = P.TheCurrentLayout.Line_Selected,
                    Icon = Icon,
                    MyCM = P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Copy()
                };
                windowCM.Pi.HasAccess = true;
                windowCM.Probability = 0;
                if (windowCM.ShowDialog() == true)
                {
                    P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Add(windowCM.CMRow);
                    int pos = TreeOperation.DetectClickPosition(P.TheCurrentLayout.PointSelected, P.TheCurrentLayout.Line_Selected);
                    RiskPolyLine Line_Created = new RiskPolyLine
                    {
                        ID = (int)windowCM.CMRow[DT_CounterM.ID],
                        IsCM = true,
                        Father = P.TheCurrentLayout.Line_Selected,
                        ShortName = "LineCreated",
                        IdRiskFather = P.TheCurrentLayout.Line_Selected.ID
                    };
                    P.TheCurrentLayout.InsertCM(Line_Created, P.TheCurrentLayout.Line_Selected, P.TheCurrentLayout.PointSelected);

                    P.TheCurrentLayout.DropLines();
                    P.TheCurrentLayout.DropRectangles();
                    P.TheCurrentLayout.LoadLines();
                    P.TheCurrentLayout.LoadRectangles();

                    P.TheCurrentLayout.DrawNumbers();
                    P.TheCurrentLayout.SetLinesThickness();
                    TextProbabilityChange(P.TheCurrentLayout.MainLine);
                    CruzarTablaRisk(P.TheCurrentLayout.Ds);
                    CruzarTablaCM(P.TheCurrentLayout.Ds);
                }

            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void CounterMeasureCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (P.TheCurrentLayout != null)
                {
                    if (P.TheCurrentLayout.ID_Diagram != 0)
                    {
                        P.TheCurrentLayout.GridPaintLines.Children.Remove(P.TheCurrentLayout.Line_Created);
                        //HACER: comando add cm              
                        P.TheCurrentLayout.Line_Created = new RiskPolyLine(P.TheCurrentLayout.GridPaintLines, MenuRisk, true)
                        {
                            Stroke = new SolidColorBrush(System.Windows.Media.Colors.Black),

                            StrokeThickness = 3
                        };
                        P.TheCurrentLayout.Line_Created.NewDrawAtPoint(new Point(P.TheCurrentLayout.X, P.TheCurrentLayout.Y), "");
                        P.TheCurrentLayout.Creando = true;
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }

        }

        private void Scope_Click(object sender, RoutedEventArgs e)
        {
            VerticalMenu win = new VerticalMenu("Scope");
            win.ShowDialog();
            P.TheCurrentLayout.IsScoping = true;
            P.TheCurrentLayout.DropRectangles();
            P.TheCurrentLayout.ScopeLine = P.TheCurrentLayout.Line_Selected;
            P.TheCurrentLayout.Scope();
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VerticalMenu win = new VerticalMenu("Copying...");
                win.ShowDialog();
                Copy(P.TheCurrentLayout.Line_Selected);
                P.TheCurrentLayout.Copiando = true;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        /// <summary>
        /// Copy the risk and its children in memory.
        /// </summary>
        private void Copy(RiskPolyLine LineToCpy)
        {
            try
            {
                P.TheCurrentLayout.ListCopy = new List<RiskPolyLine>();
                P.TheCurrentLayout.CopyRisk = new RiskPolyLine(P.TheCurrentLayout.GridPaintLines, MenuRisk, false)
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
                P.TheCurrentLayout.CopyRisk.TextPanel.Visibility = Visibility.Collapsed;
                P.TheCurrentLayout.ListCopy.Add(P.TheCurrentLayout.CopyRisk);
                foreach (var item in TreeOperation.GetOnlyMyChildrenWithCM(LineToCpy))
                {
                    if (!(item.IsCM))
                    {
                        RiskPolyLine line = new RiskPolyLine(P.TheCurrentLayout.GridPaintLines, MenuRisk, false)
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
                        P.TheCurrentLayout.ListCopy.Add(line);
                    }
                    else
                    {
                        RiskPolyLine cmLine = new RiskPolyLine(P.TheCurrentLayout.GridPaintLines, MenuCM, true)
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
                        P.TheCurrentLayout.ListCopy.Add(cmLine);
                    }
                }
                TreeOperation.Build_Tree(P.TheCurrentLayout.ListCopy, P.TheCurrentLayout.CopyRisk);
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void ButtonPaste_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (P.TheCurrentLayout.Copiando)
                {
                    VerticalMenu win = new VerticalMenu("Paste...");
                    win.ShowDialog();
                    TreeOperation.CreateCopyOfLine(P.TheCurrentLayout.CopyRisk, P.TheCurrentLayout.Line_Selected.ID, P.TheCurrentLayout.Ds);
                    P.TheCurrentLayout.CopyRisk.Father = P.TheCurrentLayout.Line_Selected;
                    P.TheCurrentLayout.CopyRisk.Position = P.TheCurrentLayout.Line_Selected.Children.Count - 1;
                    P.TheCurrentLayout.InsertRisk(P.TheCurrentLayout.CopyRisk, P.TheCurrentLayout.Line_Selected, P.TheCurrentLayout.PointSelected);
                    //---Añadido porque no funcionaba
                    P.TheCurrentLayout.DropLines();
                    P.TheCurrentLayout.DropRectangles();
                    P.TheCurrentLayout.LoadLines();
                    P.TheCurrentLayout.LoadRectangles();
                    TextProbabilityChange(P.TheCurrentLayout.MainLine);
                    TextProbability.Text = Math.Round(General.AcumulatedLikelihood(P.TheCurrentLayout.MainLine) * 100, 2).ToString() + " %";
                    P.TheCurrentLayout.DrawNumbers();
                    P.TheCurrentLayout.SetLinesThickness();
                    CruzarTablaRisk(P.TheCurrentLayout.Ds);
                    CruzarTablaCM(P.TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        #region ImportProject
        private void ButtonImportProject_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int InternalDiagramID = 0;

                WindowSelection frmSelection = new WindowSelection();
                P.TheCurrentLayout.Ds = P.TheCurrentLayout.Ds;
                //var temp = Ds.Tables[DT_RiskTree.RiskTree_TABLA].AsEnumerable().Where(x => x.Field<int>(DT_RiskTree.ID_RISK_TREE) != CurrentLayout.ID_Diagram).AsDataView();
                frmSelection.Dt = General.DeleteExists(P.TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Copy(),
                    P.TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Select(DT_Diagram.ID_DIAGRAM + " = " + P.TheCurrentLayout.ID_Diagram).CopyToDataTable(), DT_Diagram.ID_DIAGRAM);

                frmSelection.DcolumToShow = new string[] { DT_Diagram.DIAGRAM_NAME };
                frmSelection.DcolumToShowAlias = new string[] { DT_Diagram.DIAGRAM_NAME };
                frmSelection.Title = "Diagrams";
                frmSelection.P.FilterString = "Diagrams";
                frmSelection.ColumnToFilter = DT_Diagram.DIAGRAM_NAME;
                if (frmSelection.ShowDialog() == true)
                {
                    P.TheCurrentLayout.ListCopy = new List<RiskPolyLine>();
                    InternalDiagramID = (int)frmSelection.RowsSelected[0][DT_Diagram.ID_DIAGRAM];
                    DataSet importDS = new UserDataSet();
                    ServiceRiskController.WebServiceRisk risk = new ServiceRiskController.WebServiceRisk();
                    importDS.Merge(risk.GetRiskTreeID(new object[] { InternalDiagramID }));
                    risk.Dispose();
                    foreach (DataRow item in importDS.Tables[DT_Risk.TABLE_NAME].Rows)
                    {
                        RiskPolyLine riskLine = new RiskPolyLine(P.TheCurrentLayout.GridPaintLines, MenuRisk, false)
                        {
                            ShortName = item[DT_Risk.NAMESHORT].ToString(),
                            ID = (Int32)item[DT_Risk.ID],
                            Position = 0,
                            IsRoot = (Boolean)item[DT_Risk.IS_ROOT],
                            Collapsed = (Boolean)item[DT_Risk.ISCOLLAPSED],
                            Probability = (Decimal)item[DT_Risk.PROBABILITY],
                            IsCM = false
                        };
                        if (importDS.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + (Int32)item[DT_Risk.ID]).Any())
                        {
                            riskLine.IdRiskFather = (Int32)importDS.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + (Int32)item[DT_Risk.ID]).First()[DT_RiskStructure.IDRISK_FATHER];
                        }
                        else { riskLine.IdRiskFather = 0; }
                        P.TheCurrentLayout.ListCopy.Add(riskLine);
                    }
                    foreach (DataRow item in importDS.Tables[DT_CounterM.TABLE_NAME].Rows)
                    {
                        RiskPolyLine cmline = new RiskPolyLine(P.TheCurrentLayout.GridPaintLines, MenuCM, true)
                        {
                            IsCM = true,
                            Position = 0,
                            FromTop = false,
                            ShortName = item[DT_CounterM.NAMESHORT].ToString(),
                            IdRiskFather = (Int32)item[DT_CounterM.ID_RISK],
                            ID = (Int32)item[DT_CounterM.ID],
                            Probability = (Decimal)item[DT_CounterM.PROBABILITY],
                            IsActivated = (Boolean)item[DT_CounterM.ENABLED]
                        };
                        P.TheCurrentLayout.ListCopy.Add(cmline);
                    }

                    TreeOperation.Build_Tree(P.TheCurrentLayout.ListCopy);
                    P.TheCurrentLayout.CopyRisk = P.TheCurrentLayout.ListCopy.FirstOrDefault(p => p.IsRoot);

                    DataRow drRisk = P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].NewRow();
                    drRisk[DT_Risk.COMMENTS] = P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(P.TheCurrentLayout.CopyRisk.ID)[DT_Risk.COMMENTS];
                    drRisk[DT_Risk.ENABLED] = true;
                    drRisk[DT_Risk.FROM_TOP] = P.TheCurrentLayout.CopyRisk.FromTop;
                    drRisk[DT_Risk.ID_DIAGRAM] = P.TheCurrentLayout.ID_Diagram;
                    drRisk[DT_Risk.ISCOLLAPSED] = P.TheCurrentLayout.CopyRisk.Collapsed;
                    drRisk[DT_Risk.IS_ROOT] = false;
                    drRisk[DT_Risk.NAMESHORT] = P.TheCurrentLayout.CopyRisk.ShortName;
                    drRisk[DT_Risk.POSITION] = P.TheCurrentLayout.Line_Selected.Children.Count;
                    drRisk[DT_Risk.PROBABILITY] = P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(P.TheCurrentLayout.CopyRisk.ID)[DT_Risk.PROBABILITY];
                    drRisk[DT_Risk.IDRISK_FATHER] = P.TheCurrentLayout.Line_Selected.ID;
                    P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);
                    DataRow rowstructure = P.TheCurrentLayout.Ds.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                    rowstructure[DT_RiskStructure.IDRISK] = drRisk[DT_Risk.ID];
                    rowstructure[DT_RiskStructure.IDRISK_FATHER] = P.TheCurrentLayout.Line_Selected.ID;
                    P.TheCurrentLayout.Ds.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);

                    DataRow[] drDannosImportados = importDS.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + InternalDiagramID);
                    DataRow[] drMisDannos = P.TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + P.TheCurrentLayout.ID_Diagram);
                    if (new WindowMessageYesNo("The Diagram to import have different Damages than Current Diagram. Do you want import it anyway?").ShowDialog() == true)
                    {
                        foreach (DataRow item in drDannosImportados.Concat(drMisDannos))
                        {
                            //Asignando los daños de cada arbol al arbol de conjunto
                            if (!(P.TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Rows.Contains(new object[] { P.TheCurrentLayout.ID_Diagram, item[DT_Diagram_Damages.ID_DAMAGE] })))
                            {
                                DataRow drTop = P.TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].NewRow();
                                drTop[DT_Diagram_Damages.ID_RISKTREE] = P.TheCurrentLayout.ID_Diagram;
                                drTop[DT_Diagram_Damages.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                                drTop[DT_Diagram_Damages.UM] = item[DT_Diagram_Damages.UM];
                                drTop[DT_Diagram_Damages.DAMAGE] = item[DT_Diagram_Damages.DAMAGE];
                                drTop[DT_Diagram_Damages.COLOR] = item[DT_Diagram_Damages.COLOR];
                                P.TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Rows.Add(drTop);
                            }
                            //Aca el riesgo principal del ds importado coje los Damages existentes en el diagrama conjunto
                            if (!(P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_Diagram_Damages.ID_DAMAGE] })))
                            {
                                DataRow topR = P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].NewRow();
                                topR[DT_Risk_Damages.COLOR] = item[DT_Diagram_Damages.COLOR];
                                topR[DT_Risk_Damages.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                                topR[DT_Risk_Damages.ID_RISK] = drRisk[DT_Risk.ID];
                                topR[DT_Risk_Damages.VALUE] = 0;
                                topR[DT_Risk_Damages.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                                topR[DT_Risk_Damages.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString() + "(" + item[DT_Diagram_Damages.UM].ToString() + ")";
                                topR[DT_Risk_Damages.FATHER] = P.TheCurrentLayout.Line_Selected.ShortName;
                                topR[DT_Risk_Damages.GROUPE_NAME] = drRisk[DT_Risk.GROUPE_NAME];
                                topR[DT_Risk_Damages.ID_FATHER] = P.TheCurrentLayout.Line_Selected.ID;
                                topR[DT_Risk_Damages.ID_GROUPE] = drRisk[DT_Risk.ID_GROUPE];
                                topR[DT_Risk_Damages.ID_RISK_TREE] = P.TheCurrentLayout.ID_Diagram;
                                topR[DT_Risk_Damages.IS_ROOT] = drRisk[DT_Risk.IS_ROOT];
                                topR[DT_Risk_Damages.PROBABILITY] = drRisk[DT_Risk.PROBABILITY];
                                topR[DT_Risk_Damages.RISK_NAMESHORT] = drRisk[DT_Risk.NAMESHORT];
                                topR[DT_Risk_Damages.RISK_TREE] = P.TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(P.TheCurrentLayout.ID_Diagram)[DT_Diagram.DIAGRAM_NAME];
                                topR[DT_Risk_Damages.STATUS] = (Boolean)drRisk[DT_Risk.ISCOLLAPSED] ? "Activated" : "Non Activated";
                                P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Add(topR);
                            }
                        }
                        //el root importado obtiene todos los roles del padre actual
                        foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + P.TheCurrentLayout.Line_Selected.ID))
                        {
                            if (!(P.TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_Role_Risk.IDROL_COLUMN] })))
                            {
                                DataRow newRow = P.TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].NewRow();
                                newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                                newRow[DT_Role_Risk.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                                newRow[DT_Role_Risk.Role] = item[DT_Role_Risk.Role];
                                P.TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                            }
                        }//EL root obtiene los roles que tnia originalmente
                        foreach (DataRow item in importDS.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + P.TheCurrentLayout.CopyRisk.ID))
                        {
                            if (!(P.TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_Role_Risk.IDROL_COLUMN] })))
                            {
                                DataRow newRow = P.TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].NewRow();
                                newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                                newRow[DT_Role_Risk.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                                newRow[DT_Role_Risk.Role] = item[DT_Role_Risk.Role];
                                P.TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Add(newRow);
                            }
                        }
                        foreach (DataRow item in importDS.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + P.TheCurrentLayout.CopyRisk.ID))
                        {
                            if (!(P.TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Contains(new object[] { drRisk[DT_Risk.ID], item[DT_RISK_WBS.ID_WBS] })))
                            {
                                DataRow newRow = P.TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].NewRow();
                                newRow[DT_RISK_WBS.ID_RISK] = drRisk[DT_Risk.ID];
                                newRow[DT_RISK_WBS.ID_WBS] = item[DT_RISK_WBS.ID_WBS];
                                P.TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Rows.Add(newRow);
                            }
                        }
                        foreach (var item in P.TheCurrentLayout.CopyRisk.Children)
                        {
                            item.IdRiskFather = (Int32)drRisk[DT_Risk.ID];
                        }
                        //--------------------------------------------------------------------------------------
                        P.TheCurrentLayout.CopyRisk.ID = (Int32)drRisk[DT_Risk.ID];
                        P.TheCurrentLayout.LinesList.Add(P.TheCurrentLayout.CopyRisk);

                        Paste(P.TheCurrentLayout.CopyRisk, importDS, P.TheCurrentLayout.Ds, P.TheCurrentLayout.ID_Diagram, P.TheCurrentLayout.LinesList);//Aca pego el resto del diagrama

                        P.TheCurrentLayout.CopyRisk.IdRiskFather = P.TheCurrentLayout.Line_Selected.ID;
                        P.TheCurrentLayout.Copiando = false;
                        //RegisterLines(ListaRiesgoLinea, Ds);
                        foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + P.TheCurrentLayout.ID_Diagram))
                        {
                            //Por cada riesgo del diagrama
                            foreach (DataRow itemRisk in P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + P.TheCurrentLayout.ID_Diagram))
                            {
                                if (!(P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { itemRisk[DT_Risk.ID], item[DT_Risk_Damages.ID_DAMAGE] })))
                                {
                                    DataRow drDamage = P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].NewRow();
                                    drDamage[DT_Risk_Damages.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                                    drDamage[DT_Risk_Damages.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString() + "(" + item[DT_Diagram_Damages.UM].ToString() + ")";
                                    drDamage[DT_Risk_Damages.VALUE] = 0;
                                    drDamage[DT_Risk_Damages.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                                    drDamage[DT_Risk_Damages.ID_RISK] = itemRisk[DT_Risk.ID];
                                    drDamage[DT_Risk_Damages.ID_RISK_TREE] = itemRisk[DT_Risk.ID_DIAGRAM];
                                    drDamage[DT_Risk_Damages.COLOR] = item[DT_Diagram_Damages.COLOR];
                                    drDamage[DT_Risk_Damages.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                                    drDamage[DT_Risk_Damages.DAMAGE] = item[DT_Diagram_Damages.DAMAGE];
                                    drDamage[DT_Risk_Damages.FATHER] = P.TheCurrentLayout.Line_Selected.ShortName;
                                    drDamage[DT_Risk_Damages.GROUPE_NAME] = itemRisk[DT_Risk.GROUPE_NAME];
                                    drDamage[DT_Risk_Damages.ID_FATHER] = P.TheCurrentLayout.Line_Selected.ID;
                                    drDamage[DT_Risk_Damages.ID_GROUPE] = itemRisk[DT_Risk.ID_GROUPE];
                                    drDamage[DT_Risk_Damages.ID_RISK_TREE] = P.TheCurrentLayout.ID_Diagram;
                                    drDamage[DT_Risk_Damages.IS_ROOT] = itemRisk[DT_Risk.IS_ROOT];
                                    drDamage[DT_Risk_Damages.PROBABILITY] = itemRisk[DT_Risk.PROBABILITY];
                                    drDamage[DT_Risk_Damages.RISK_NAMESHORT] = itemRisk[DT_Risk.NAMESHORT];
                                    drDamage[DT_Risk_Damages.RISK_TREE] = P.TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(P.TheCurrentLayout.ID_Diagram)[DT_Diagram.DIAGRAM_NAME];
                                    drDamage[DT_Risk_Damages.STATUS] = (Boolean)itemRisk[DT_Risk.ENABLED] || itemRisk[DT_Risk.ENABLED] == DBNull.Value ? "Activated" : "Non Activated";
                                    P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Add(drDamage);
                                }
                            }
                            foreach (DataRow itemCM in P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK_TREE + " = " + P.TheCurrentLayout.ID_Diagram))
                            {
                                if (!(P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Contains(new object[] { itemCM[DT_CounterM.ID], item[DT_CounterM_Damage.ID_DAMAGE] })))
                                {
                                    DataRow drDamage = P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].NewRow();
                                    drDamage[DT_CounterM_Damage.ID_DAMAGE] = item[DT_Diagram_Damages.ID_DAMAGE];
                                    drDamage[DT_CounterM_Damage.DAMAGE] = item[DT_Diagram_Damages.DAMAGE].ToString() + "(" + item[DT_Diagram_Damages.UM].ToString() + ")";
                                    drDamage[DT_CounterM_Damage.VALUE] = 0;
                                    drDamage[DT_CounterM_Damage.TOP_RISK] = item[DT_Diagram_Damages.DAMAGE];
                                    drDamage[DT_CounterM_Damage.ID_COUNTERM] = itemCM[DT_CounterM.ID];
                                    drDamage[DT_CounterM_Damage.ID_RISK_TREE] = itemCM[DT_CounterM.ID_RISK_TREE];
                                    drDamage[DT_CounterM_Damage.COLOR] = item[DT_Diagram_Damages.COLOR];
                                    drDamage[DT_CounterM_Damage.COUNTERM_NAMESHORT] = itemCM[DT_CounterM.NAMESHORT];
                                    drDamage[DT_CounterM_Damage.GROUPENAME] = itemCM[DT_CounterM.GROUPE_NAME];
                                    drDamage[DT_CounterM_Damage.IDRISK] = itemCM[DT_CounterM.ID_RISK];
                                    drDamage[DT_CounterM_Damage.ID_GROUPE] = itemCM[DT_CounterM.ID_GROUPE];
                                    drDamage[DT_CounterM_Damage.RISK] = P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(itemCM[DT_CounterM.ID_RISK])[DT_Risk.NAMESHORT];
                                    drDamage[DT_CounterM_Damage.RISK_REDUCTION] = itemCM[DT_CounterM.PROBABILITY];
                                    drDamage[DT_CounterM_Damage.STATUS] = (Boolean)itemCM[DT_CounterM.ENABLED] ? "Activated" : "Non Activated";

                                    P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Add(drDamage);
                                }
                            }
                        }
                        P.TheCurrentLayout.DropLines();
                        P.TheCurrentLayout.DropRectangles();
                        P.TheCurrentLayout.LoadComboDamage();
                        P.TheCurrentLayout.LoadLines();
                        P.TheCurrentLayout.LoadRectangles();
                        P.TheCurrentLayout.DrawNumbers();
                        TextProbabilityChange(P.TheCurrentLayout.MainLine);
                        P.TheCurrentLayout.SetLinesThickness();
                        P.TheCurrentLayout.DrawNumbers();
                        P.TheCurrentLayout.SetLinesThickness();
                        CruzarTablaRisk(P.TheCurrentLayout.Ds);
                        CruzarTablaCM(P.TheCurrentLayout.Ds);
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }
        #endregion

        private void EnableRiskButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Boolean result = EnableRisk(P.TheCurrentLayout.Line_Selected, false, false);
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
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        /// <summary>
        /// Enable/Disable risk
        /// </summary>
        /// <param name="Risk"> Risk to Enable/Disable</param>
        /// <param name="isGroup"> True if risk group, else if individual risk</param>
        /// <param name="estadoActual"> Risk group state (Enable/Disable)</param>
        private Boolean EnableRisk(RiskPolyLine Risk, bool isGroup = false, bool estadoActual = false)
        {
            try
            {
                Boolean result = false;
                Boolean enabledColumn = (isGroup) ? estadoActual : (Boolean)P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Risk.ID)[DT_Risk.ENABLED];
                if (enabledColumn)
                {
                    P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Risk.ID)[DT_Risk.ENABLED] = false;
                    foreach (DataRow damageRow in P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + Risk.ID))
                    {
                        damageRow[DT_Risk_Damages.STATUS] = "Non Activated";
                    }
                    //(P.TheCurrentLayout.LinesList.Find(item => (item.ID == Risk.ID))).DrawEntireLine("(Disabled)" + P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Risk.ID)[DT_Risk.NAMESHORT].ToString());
                    //(CurrentLayout.LinesList.Find(item => (item.ID == Risk.ID))).Stroke = new SolidColorBrush(Colors.Gray);
                    (P.TheCurrentLayout.LinesList.Find(item => (item.ID == Risk.ID))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                    (P.TheCurrentLayout.LinesList.Find(item => (item.ID == Risk.ID))).IsActivated = false;
                    foreach (var itemi in Risk.Children)
                    {
                        if (itemi.IsCM)
                        {
                            itemi.IsActivated = false;
                            itemi.SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                            //(P.TheCurrentLayout.LinesList.Find(item => (item.ID == itemi.ID))).DrawEntireLine("(Disabled)" + P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(itemi.ID)[DT_CounterM.NAMESHORT]);
                            //(CurrentLayout.LinesList.Find(item => (item.ID == itemi.ID))).Stroke = new SolidColorBrush(Colors.Gray);
                            (P.TheCurrentLayout.LinesList.Find(item => (item.ID == itemi.ID))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                            (P.TheCurrentLayout.LinesList.Find(item => (item.ID == itemi.ID))).IsActivated = false;
                            P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(itemi.ID)[DT_CounterM.ENABLED] = false;
                            foreach (DataRow damageRow in P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + itemi.ID))
                            {
                                damageRow[DT_CounterM_Damage.STATUS] = "Non Activated";
                            }
                        }
                    }
                }
                else
                {
                    P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Risk.ID)[DT_Risk.ENABLED] = true;
                    foreach (DataRow damageRow in P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + Risk.ID))
                    {
                        damageRow[DT_Risk_Damages.STATUS] = "Activated";
                    }
                    System.Drawing.Color drawingCColor = System.Drawing.Color.FromArgb(int.Parse(P.TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + P.TheCurrentLayout.ID_Diagram)[P.TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                    //(P.TheCurrentLayout.LinesList.Find(item => (item.ID == Risk.ID))).DrawEntireLine(P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Risk.ID)[DT_Risk.NAMESHORT].ToString());
                    //(CurrentLayout.LinesList.Find(item => (item.ID == Risk.ID))).Stroke = new SolidColorBrush(Color.FromArgb(drawingCColor.A, drawingCColor.R, drawingCColor.G, drawingCColor.B));
                    (P.TheCurrentLayout.LinesList.Find(item => (item.ID == Risk.ID))).SetColor(new SolidColorBrush(System.Windows.Media.Color.FromArgb(drawingCColor.A, drawingCColor.R, drawingCColor.G, drawingCColor.B)));
                    (P.TheCurrentLayout.LinesList.Find(item => (item.ID == Risk.ID))).IsActivated = true;

                    foreach (var itemi in Risk.Children)
                    {
                        if (itemi.IsCM)
                        {
                            DisableCounterMeasure(itemi, true, !P.TheCurrentLayout.LinesListCMState[Convert.ToInt32(itemi.ID)]);
                        }
                    }
                    result = true;
                }
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
            P.TheCurrentLayout.DrawNumbers();
            TextProbabilityChange(P.TheCurrentLayout.MainLine);
            P.TheCurrentLayout.SetLinesThickness();

            CruzarTablaRisk(P.TheCurrentLayout.Ds);
            CruzarTablaCM(P.TheCurrentLayout.Ds);
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
                if (P.TheCurrentLayout.Line_Selected.IsActivated)
                {
                    WindowCM windowCM = new WindowCM
                    {
                        ID_Project = (int)P.TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(P.TheCurrentLayout.ID_Diagram)[DT_Diagram.ID_PROJECT],
                        CMRow = P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(P.TheCurrentLayout.Line_Selected.ID),
                        DsCM = P.TheCurrentLayout.Ds,
                        Operation = General.UPDATE,
                        LOGIN_USER = LoginUser,
                        TopRiskTable = P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME],
                        WBS_CM_Damage = P.TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME],
                        CM_WBS_Table = P.TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME],
                        CM_RoleTable = P.TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME],
                        Posicion = (int)P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(P.TheCurrentLayout.Line_Selected.ID)[DT_CounterM.POSITION],
                        Icon = Icon,
                        MyCM = P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Copy()
                    };
                    windowCM.Pi.HasAccess = P.TheCurrentLayout.FullAccess(P.TheCurrentLayout.Line_Selected);
                    windowCM.RiskPadre = P.TheCurrentLayout.LinesList.Find(l => l.ID == P.TheCurrentLayout.Line_Selected.IdRiskFather);
                    windowCM.RiskTreeID = (int)P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(P.TheCurrentLayout.Line_Selected.ID)[DT_CounterM.ID_RISK_TREE];

                    if (windowCM.ShowDialog() == true)
                    {
                        if (P.TheCurrentLayout.Ds.HasChanges())
                        {
                            TreeOperation.SetCMLineValues(P.TheCurrentLayout.Line_Selected, windowCM.CMRow);
                            TextProbabilityChange(P.TheCurrentLayout.MainLine);
                            P.TheCurrentLayout.DrawNumbers();
                            P.TheCurrentLayout.UpdateLinesValues();
                            P.TheCurrentLayout.SetLinesThickness();
                            CruzarTablaRisk(P.TheCurrentLayout.Ds);                            
                            CruzarTablaCM(P.TheCurrentLayout.Ds);
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
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        /// <summary>
        /// Delete an existing CM
        /// </summary>
        private void MenuDeleteCM_Click(object sender, RoutedEventArgs e)
        {
            //DsMain = CurrentLayout.Ds;
            if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + P.TheCurrentLayout.Line_Selected.ShortName + "] ?").ShowDialog() == true)
            {
                TreeOperation.DeleteLine(P.TheCurrentLayout.Line_Selected, P.TheCurrentLayout.Ds);
                P.TheCurrentLayout.DropLines();
                P.TheCurrentLayout.DropRectangles();
                P.TheCurrentLayout.LoadLines();
                P.TheCurrentLayout.LoadRectangles();
                P.TheCurrentLayout.DrawNumbers();
                TextProbabilityChange(P.TheCurrentLayout.MainLine);
                P.TheCurrentLayout.SetLinesThickness();
                CruzarTablaRisk(P.TheCurrentLayout.Ds);
                CruzarTablaCM(P.TheCurrentLayout.Ds);
            }
        }

        /// <summary>
        /// Select a CM to Move
        /// </summary>
        private void MenuMoveCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                P.TheCurrentLayout.MoviendoCM = true;
                P.TheCurrentLayout.LinesMoving = new List<RiskPolyLine>() { P.TheCurrentLayout.Line_Selected };
                Cursor = Cursors.Hand;
                VerticalMenu win = new VerticalMenu("Moving");
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        /// <summary>
        /// Enable or Disable a CM
        /// </summary>
        private void MenuEnableCM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Boolean result = DisableCounterMeasure(P.TheCurrentLayout.Line_Selected);
                OnProjectChange();
                if (result)
                {
                    VerticalMenu win = new VerticalMenu("Enabled");
                    win.ShowDialog();
                }
                else
                {
                    if (P.TheCurrentLayout.Line_Selected.Father.IsActivated)
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
        private Boolean DisableCounterMeasure(RiskPolyLine cm_Selected, bool isGroup = false, bool estadoActual = false)
        {
            try
            {
                Boolean result = false;
                Boolean enabledColumn = (isGroup) ? estadoActual : (Boolean)P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cm_Selected.ID)[DT_CounterM.ENABLED];
                if (enabledColumn)
                {
                    P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cm_Selected.ID)[DT_CounterM.ENABLED] = false;
                    foreach (DataRow damageRow in P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + cm_Selected.ID))
                    {
                        damageRow[DT_CounterM_Damage.STATUS] = "Non Activated";
                    }
                    P.TheCurrentLayout.LinesListCMState[Convert.ToInt32(cm_Selected.ID)] = false;
                    //(P.TheCurrentLayout.LinesList.Find(item => (item.ID == cm_Selected.ID && item.IsCM))).DrawEntireLine("(Disabled)" + P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cm_Selected.ID)[DT_CounterM.NAMESHORT]);
                    //(CurrentLayout.LinesList.Find(item => (item.ID == cm_Selected.ID && item.IsCM))).Stroke = new SolidColorBrush(Colors.Gray);
                    (P.TheCurrentLayout.LinesList.Find(item => (item.ID == cm_Selected.ID && item.IsCM))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Gray));
                    (P.TheCurrentLayout.LinesList.Find(item => (item.ID == cm_Selected.ID && item.IsCM))).IsActivated = false;
                    result = false;
                }
                else
                {
                    if (cm_Selected.Father.IsActivated) // si el padre esta disabled no habilito cm
                    {
                        P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cm_Selected.ID)[DT_CounterM.ENABLED] = true;
                        foreach (DataRow damageRow in P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + cm_Selected.ID))
                        {
                            damageRow[DT_CounterM_Damage.STATUS] = "Activated";
                        }
                        P.TheCurrentLayout.LinesListCMState[Convert.ToInt32(cm_Selected.ID)] = true;
                        //(P.TheCurrentLayout.LinesList.Find(item => (item.ID == cm_Selected.ID && item.IsCM))).DrawEntireLine(P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cm_Selected.ID)[DT_CounterM.NAMESHORT].ToString());
                        //(CurrentLayout.LinesList.Find(item => (item.ID == cm_Selected.ID && item.IsCM))).Stroke = new SolidColorBrush(Colors.Black);
                        (P.TheCurrentLayout.LinesList.Find(item => (item.ID == cm_Selected.ID && item.IsCM))).SetColor(new SolidColorBrush(System.Windows.Media.Colors.Black));
                        (P.TheCurrentLayout.LinesList.Find(item => (item.ID == cm_Selected.ID && item.IsCM))).IsActivated = true;
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
                //DsMain = CurrentLayout.Ds;
                WindowGroupe wg = new WindowGroupe();
                wg.ShowDialog();
                if (wg.DialogResult == true)
                {
                    if (wg.IdGroup == -100)
                    {
                        DataRow drGroup = P.TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].NewRow();
                        drGroup[DT_Groupe.GROUPE_NAME] = wg.GroupName;
                        P.TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Add(drGroup);
                        foreach (var item in P.TheCurrentLayout.RiskGroupSelected)
                        {
                            P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.ID_GROUPE] = drGroup[DT_Groupe.ID_GROUPE];
                            P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.GROUPE_NAME] = drGroup[DT_Groupe.GROUPE_NAME];
                        }
                        foreach (var item in P.TheCurrentLayout.RiskGroupSelected)
                        {
                            item.Group.IdGroup = (Int32)drGroup[DT_Groupe.ID_GROUPE];
                            item.Group.GroupName = drGroup[DT_Groupe.GROUPE_NAME].ToString();
                        }
                    }
                    else
                    {
                        foreach (var item in P.TheCurrentLayout.RiskGroupSelected)
                        {
                            P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.ID_GROUPE] = wg.IdGroup;
                            P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.GROUPE_NAME] = wg.GroupName;
                        }
                        foreach (var item in P.TheCurrentLayout.RiskGroupSelected)
                        {
                            item.Group.IdGroup = wg.IdGroup;
                            item.Group.GroupName = wg.GroupName;
                        }
                    }
                    CruzarTablaRisk(P.TheCurrentLayout.Ds);//Esta Linea estaba, la borre al integrar
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
                    DsCM = P.TheCurrentLayout.Ds,
                    RiskTreeID = P.TheCurrentLayout.ID_Diagram,
                    Icon = Icon,
                    MyCM = P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Copy(),
                    Probability = new decimal(0),
                    TopRiskTable = P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Clone()
                };
                //Roberto: Si capturo un id que va a ser global, lo hago fuera del ciclo para no buscar tanto lo mismo.
                int RiskTreeID = P.TheCurrentLayout.ID_Diagram;
                //Roberto: Agrego esto para que cuando la Ventana Cargue, el DataGrid, tenga los valores en 0 para cada Damage
                foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + RiskTreeID))
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
                    foreach (var itemRisk in P.TheCurrentLayout.RiskGroupSelected)
                    {
                        DataRow CMRow = P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].NewRow();
                        //Roberto: Si capturo un id que va a ser global, lo hago fuera del ciclo para no buscar tanto lo mismo.
                        DataRow RowFather = P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(itemRisk.ID);
                        DataTable TopRiskTable = P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Copy();
                        DataTable CM_RoleTable = P.TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Copy();
                        int Posicion = 0;

                        CMRow[DT_CounterM.NAMESHORT] = windowCMGroup.NameShort;
                        CMRow[DT_CounterM.DETAIL] = windowCMGroup.Detail;
                        CMRow[DT_CounterM.ID_RISK_TREE] = RiskTreeID;
                        CMRow[DT_CounterM.ID_RISK] = itemRisk.ID;
                        CMRow[DT_CounterM.FROM_TOP] = RowFather[DT_Risk.FROM_TOP];
                        CMRow[DT_CounterM.POSITION] = Posicion + 1;
                        CMRow[DT_CounterM.ENABLED] = true;
                        CMRow[DT_CounterM.PROBABILITY] = windowCMGroup.Probability;

                        foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + RiskTreeID))
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
                        foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + itemRisk.ID))
                        {
                            DataRow newRow = CM_RoleTable.NewRow();
                            newRow[DT_Role_CM.ID_CM] = CMRow[DT_CounterM.ID];
                            newRow[DT_Role_CM.Role] = item[DT_Role_CM.Role];
                            newRow[DT_Role_CM.IDROL_COLUMN] = item[DT_Role_CM.IDROL_COLUMN];
                            CM_RoleTable.Rows.Add(newRow);
                        }
                        foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + itemRisk.ID))
                        {
                            DataRow newRow = P.TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].NewRow();
                            newRow[DT_CM_WBS.ID_CM] = CMRow[DT_CounterM.ID];
                            newRow[DT_CM_WBS.ID_WBS] = item[DT_RISK_WBS.ID_WBS];
                            newRow[DT_CM_WBS.IS_PRIMARY] = item[DT_RISK_WBS.IS_PRIMARY];
                            newRow[DT_CM_WBS.NIVEL] = item[DT_RISK_WBS.NIVEL];
                            newRow[DT_CM_WBS.PRIMARY] = item[DT_RISK_WBS.PRIMARY];
                            newRow[DT_CM_WBS.USERNAME] = item[DT_RISK_WBS.USERNAME];
                            newRow[DT_CM_WBS.WBS] = item[DT_RISK_WBS.WBS];
                            P.TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Rows.Add(newRow);
                        }
                        foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + itemRisk.ID))
                        {
                            DataRow newRow = P.TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].NewRow();
                            newRow[DT_WBS_CM_Damage.ID_CM] = CMRow[DT_CounterM.ID];
                            newRow[DT_WBS_CM_Damage.DAMAGE] = item[DT_WBS_RISK_DAMAGE.DAMAGE];
                            newRow[DT_WBS_CM_Damage.ID_DAMAGE] = item[DT_WBS_RISK_DAMAGE.ID_DAMAGE];
                            newRow[DT_WBS_CM_Damage.ID_WBS] = item[DT_WBS_RISK_DAMAGE.ID_WBS];
                            newRow[DT_WBS_CM_Damage.VALUE] = 0;
                            newRow[DT_WBS_CM_Damage.WBS] = item[DT_WBS_RISK_DAMAGE.WBS];
                            P.TheCurrentLayout.Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Rows.Add(newRow);
                        }
                        P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Add(CMRow);
                        P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Merge(TopRiskTable);
                        P.TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Merge(CM_RoleTable);

                        RiskPolyLine Line_Created = new RiskPolyLine
                        {
                            ID = (Int32)CMRow[DT_CounterM.ID],
                            IsCM = true,
                            ShortName = "LineCreated",
                            Father = itemRisk,
                            IdRiskFather = itemRisk.ID
                        };

                        P.TheCurrentLayout.InsertCM(Line_Created, itemRisk, itemRisk.MyMinXPoint());
                    }

                    P.TheCurrentLayout.ResetGroupRiksSelection();
                    P.TheCurrentLayout.DropLines();
                    P.TheCurrentLayout.DropRectangles();
                    P.TheCurrentLayout.LoadLines();
                    P.TheCurrentLayout.LoadRectangles();
                    P.TheCurrentLayout.DrawNumbers();
                    P.TheCurrentLayout.SetLinesThickness();

                    TextProbabilityChange(P.TheCurrentLayout.MainLine);
                    CruzarTablaRisk(P.TheCurrentLayout.Ds);
                    CruzarTablaCM(P.TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void ActDesGroupRiskButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Boolean estadoActual = false;
                var query = from item in P.TheCurrentLayout.RiskGroupSelected
                            where (Boolean)item.IsActivated == true
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
                Boolean estadoActual = false;
                var query = from item in P.TheCurrentLayout.CMGroupSelected
                            where (Boolean)item.IsActivated == true
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
                        DataRow drGroup = P.TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].NewRow();
                        drGroup[DT_Groupe.GROUPE_NAME] = wg.GroupName;
                        P.TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Add(drGroup);
                        foreach (var item in P.TheCurrentLayout.CMGroupSelected)
                        {
                            P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.ID_GROUPE] = drGroup[DT_Groupe.ID_GROUPE];
                            P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.GROUPE_NAME] = drGroup[DT_Groupe.GROUPE_NAME];
                        }
                        foreach (var item in P.TheCurrentLayout.CMGroupSelected)
                        {
                            item.Group.IdGroup = (Int32)drGroup[DT_Groupe.ID_GROUPE];
                            item.Group.GroupName = drGroup[DT_Groupe.GROUPE_NAME].ToString();
                        }
                    }
                    else
                    {
                        foreach (var item in P.TheCurrentLayout.CMGroupSelected)
                        {
                            P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.ID_GROUPE] = wg.IdGroup;
                            P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.GROUPE_NAME] = wg.GroupName;
                        }
                        foreach (var item in P.TheCurrentLayout.CMGroupSelected)
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
            if (P.TheCurrentLayout.CMGroupSelected.Count > 0 && new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " selected countermeasure?").ShowDialog() == true)
            {
                try
                {
                    foreach (RiskPolyLine cm in P.TheCurrentLayout.CMGroupSelected)
                    {
                        if (P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Contains(cm.ID))
                        {
                            P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(cm.ID).Delete();
                        }
                    }
                    P.TheCurrentLayout.ResetGroupCMSelection();
                    P.TheCurrentLayout.DropLines();
                    P.TheCurrentLayout.DropRectangles();
                    P.TheCurrentLayout.LoadLines();
                    P.TheCurrentLayout.LoadRectangles();
                    P.TheCurrentLayout.DrawNumbers();
                    TextProbabilityChange(P.TheCurrentLayout.MainLine);
                    P.TheCurrentLayout.SetLinesThickness();
                }
                catch (Exception ex)
                {
                    new WindowMessageOK(ex.Message).ShowDialog();
                }
            }

        }

        private void ActDesGroupMixedButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Boolean estadoActual = false;
                var query = from item in P.TheCurrentLayout.RiskGroupSelected
                            where (Boolean)item.IsActivated == true
                            select item;
                List<RiskPolyLine> result = query.ToList<RiskPolyLine>();

                var queryCM = from item in P.TheCurrentLayout.CMGroupSelected
                              where (Boolean)item.IsActivated == true
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
                        GroupRiskDataTableUpdate((Int32)drGroup[DT_Groupe.ID_GROUPE], wg.GroupName);
                        GroupCounterMeasureDataTableUpdate((Int32)drGroup[DT_Groupe.ID_GROUPE], wg.GroupName);
                        GroupRiskPolyLineUpdate(P.TheCurrentLayout.RiskGroupSelected, (Int32)drGroup[DT_Groupe.ID_GROUPE], wg.GroupName);
                        GroupRiskPolyLineUpdate(P.TheCurrentLayout.CMGroupSelected, (Int32)drGroup[DT_Groupe.ID_GROUPE], wg.GroupName);
                    }
                    else
                    {
                        GroupRiskDataTableUpdate(wg.IdGroup, wg.GroupName);
                        GroupCounterMeasureDataTableUpdate(wg.IdGroup, wg.GroupName);
                        GroupRiskPolyLineUpdate(P.TheCurrentLayout.RiskGroupSelected, wg.IdGroup, wg.GroupName);
                        GroupRiskPolyLineUpdate(P.TheCurrentLayout.CMGroupSelected, wg.IdGroup, wg.GroupName);
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
            DataRow drGroup = P.TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].NewRow();
            drGroup[DT_Groupe.GROUPE_NAME] = groupName;
            P.TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Add(drGroup);
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
            foreach (var item in P.TheCurrentLayout.RiskGroupSelected)
            {
                P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.ID_GROUPE] = idGroup;
                P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.GROUPE_NAME] = groupName;
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
            foreach (var item in P.TheCurrentLayout.CMGroupSelected)
            {
                P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.ID_GROUPE] = idGroup;
                P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.GROUPE_NAME] = groupName;
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
        private void DisableCounterMeasureGroup(Boolean estadoActual)
        {
            try
            {
                foreach (var cmline in P.TheCurrentLayout.CMGroupSelected)
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
        private void DisableRiskGroup(Boolean estadoActual)
        {
            try
            {
                foreach (RiskPolyLine rpl in P.TheCurrentLayout.RiskGroupSelected)
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
        public void CruzarTablaRisk(DataSet myDs)
        {
            try
            {
                if (P.TheCurrentLayout != null)
                {
                    Dt_Cross_Risk = new DataTable();
                    Dt_Cross_Risk = myDs.Tables[DT_Risk_Damages.TABLENAME].Clone();
                    foreach (var item in myDs.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK_TREE + " = " + P.TheCurrentLayout.ID_Diagram))
                    {
                        Dt_Cross_Risk.ImportRow(item);
                    }
                    DataTable dtTemp = Dt_Cross_Risk.Copy();
                    DataColumn[] pkCC = new DataColumn[2];

                    pkCC[0] = new DataColumn(DT_Risk_Damages.ID_RISK, typeof(System.Int32));
                    pkCC[1] = new DataColumn(DT_Risk_Damages.ID_RISK_TREE, typeof(System.Int32));

                    Dt_Cross_Risk = General.CrossTable(ref dtTemp, "Damage", new string[] { DT_Risk_Damages.VALUE }, pkCC);
                    foreach (var item in P.TheCurrentLayout.LinesList)
                    {
                        if (Dt_Cross_Risk.Select("idRisk = " + item.ID).Any())
                        {
                            Dt_Cross_Risk.Select("idRisk = " + item.ID).First()["nameShort"] = Dt_Cross_Risk.Select("idRisk = " + item.ID).First()["nameShort"].ToString().Insert(0, TreeOperation.Spaces(item.MyLevel));
                        }
                    }
                    Dt_Cross_Risk.AcceptChanges();
                    dgRisksCross.Columns.Clear();
                    dgRisksCross.AutoGenerateColumns = false;
                    //Adding the columns to cross table
                    Style styleText = new Style();
                    Setter set1 = new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
                    Setter set2 = new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left);
                    styleText.Setters.Add(set1);
                    styleText.Setters.Add(set2);
                    for (int i = 0; i < Dt_Cross_Risk.Columns.Count; i++)
                    {
                        DataGridTextColumn column = new DataGridTextColumn();
                        switch (Dt_Cross_Risk.Columns[i].ToString())
                        {
                            case "probability":
                                Binding probabilityBinding = new Binding(Dt_Cross_Risk.Columns[i].ToString())
                                {
                                    Converter = new DecimalUIConverter(),
                                    ConverterCulture = CultureInfo.CurrentUICulture,
                                    ConverterParameter = decimalConfig
                                };
                                column.Header = "Probability (%)";
                                column.Binding = probabilityBinding;
                                column.IsReadOnly = false;
                                column.MinWidth = 100;
                                dgRisksCross.Columns.Add(column);
                                break;
                            case "nameShort":
                                column.Header = "Risk";
                                column.Binding = new Binding(Dt_Cross_Risk.Columns[i].ToString());
                                column.ElementStyle = styleText;
                                column.IsReadOnly = false;
                                column.MinWidth = 100;
                                dgRisksCross.Columns.Add(column);
                                break;
                            case "WBS Name":
                                column.Header = "WBS Name";
                                column.Binding = new Binding(Dt_Cross_Risk.Columns[i].ToString());
                                column.ElementStyle = styleText;
                                column.IsReadOnly = true;
                                column.MinWidth = 100;
                                dgRisksCross.Columns.Add(column);
                                break;
                            case "userName":
                                column.Header = "User";
                                column.Binding = new Binding(Dt_Cross_Risk.Columns[i].ToString());
                                column.ElementStyle = styleText;
                                column.IsReadOnly = true;
                                column.MinWidth = 100;
                                dgRisksCross.Columns.Add(column);
                                break;
                            case "Father":
                                column.Header = "Father";
                                column.Binding = new Binding(Dt_Cross_Risk.Columns[i].ToString());
                                column.ElementStyle = styleText;
                                column.IsReadOnly = true;
                                column.MinWidth = 100;
                                dgRisksCross.Columns.Add(column);
                                break;
                            case "Status":
                                column.Header = "Status";
                                column.Binding = new Binding(Dt_Cross_Risk.Columns[i].ToString());
                                column.IsReadOnly = true;
                                column.MinWidth = 100;
                                dgRisksCross.Columns.Add(column);
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
                                if (Dt_Cross_Risk.Columns[i].DataType.Equals(typeof(System.Decimal)))
                                {
                                    columnBinding.Converter = new DecimalUIConverter();
                                    columnBinding.ConverterCulture = CultureInfo.CurrentUICulture;
                                    columnBinding.ConverterParameter = decimalConfig;
                                }
                                column.Header = Dt_Cross_Risk.Columns[i].ToString();
                                column.Binding = columnBinding;
                                column.IsReadOnly = false;
                                column.MinWidth = 100;
                                dgRisksCross.Columns.Add(column);
                                break;
                        }
                    }

                    TreeOperation.OrderTableHierarquical(Dt_Cross_Risk, P.TheCurrentLayout.LinesList, DT_Risk_Damages.ID_RISK);

                    DV_CrossRisk = new DataView(Dt_Cross_Risk);
                    dgRisksCross.ItemsSource = DV_CrossRisk;
                    FillTableGroup(myDs);
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        public void CruzarTablaCM(DataSet myDs)
        {
            if (P.TheCurrentLayout != null)
            {
                Dt_Cross_CM = new DataTable();
                Dt_Cross_CM = myDs.Tables[DT_CounterM_Damage.TABLENAME].Clone();
                foreach (var item in myDs.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_RISK_TREE + " = " + P.TheCurrentLayout.ID_Diagram))
                {
                    Dt_Cross_CM.ImportRow(item);
                }
                DataTable dtTemp = Dt_Cross_CM.Copy();
                DataColumn[] pkCC = new DataColumn[2];

                pkCC[0] = new DataColumn(DT_CounterM_Damage.ID_COUNTERM, typeof(System.Int32));
                pkCC[1] = new DataColumn(DT_CounterM_Damage.ID_RISK_TREE, typeof(System.Int32));

                Dt_Cross_CM = General.CrossTable(ref dtTemp, "Damage", new string[] { DT_CounterM_Damage.VALUE }, pkCC);
                foreach (var item in P.TheCurrentLayout.LinesList)
                {
                    if (Dt_Cross_CM.Select("idCounterM = " + item.ID).Any())
                    {
                        Dt_Cross_CM.Select("idCounterM = " + item.ID).First()["nameShort"] = Dt_Cross_CM.Select("idCounterM = " + item.ID).First()["nameShort"].ToString().Insert(0, TreeOperation.Spaces(item.MyLevel));
                    }
                }
                Dt_Cross_CM.AcceptChanges();
                dgCrossCM.Columns.Clear();
                dgCrossCM.AutoGenerateColumns = false;

                Style styleText = new Style();
                Setter set1 = new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
                Setter set2 = new Setter(TextBlock.TextAlignmentProperty, TextAlignment.Left);
                styleText.Setters.Add(set1);
                styleText.Setters.Add(set2);

                //dgSelection.columns
                for (int i = 0; i < Dt_Cross_CM.Columns.Count; i++)
                {
                    DataGridTextColumn column = new DataGridTextColumn();
                    switch (Dt_Cross_CM.Columns[i].ToString())
                    {
                        case DT_CounterM_Damage.RISK_REDUCTION:
                            Binding riskReductionBinding = new Binding(Dt_Cross_CM.Columns[i].ToString())
                            {
                                Converter = new DecimalUIConverter(),
                                ConverterCulture = CultureInfo.CurrentUICulture,
                                ConverterParameter = decimalConfig
                            };
                            column.MinWidth = 100;
                            column.Header = "Risk Reduction(%)";
                            column.Binding = riskReductionBinding;
                            column.IsReadOnly = false;
                            dgCrossCM.Columns.Add(column);
                            break;
                        case DT_CounterM_Damage.COUNTERM_NAMESHORT:
                            column.Header = "Counter M.";
                            column.ElementStyle = styleText;
                            column.Binding = new Binding(Dt_Cross_CM.Columns[i].ToString());
                            column.IsReadOnly = false;
                            column.MinWidth = 100;
                            dgCrossCM.Columns.Add(column);
                            break;
                        case "WBS Name":
                            column.Header = "WBS Name";
                            column.Binding = new Binding(Dt_Cross_CM.Columns[i].ToString());
                            column.ElementStyle = styleText;
                            column.IsReadOnly = true;
                            column.MinWidth = 100;
                            dgCrossCM.Columns.Add(column);
                            break;
                        case "userName":
                            column.Header = "User";
                            column.Binding = new Binding(Dt_Cross_CM.Columns[i].ToString());
                            column.ElementStyle = styleText;
                            column.IsReadOnly = true;
                            column.MinWidth = 100;
                            dgCrossCM.Columns.Add(column);
                            break;
                        case DT_CounterM_Damage.RISK:
                            column.Header = "Risk";
                            column.ElementStyle = styleText;
                            column.Binding = new Binding(Dt_Cross_CM.Columns[i].ToString());
                            column.IsReadOnly = true;
                            column.MinWidth = 100;
                            dgCrossCM.Columns.Add(column);
                            break;
                        case DT_CounterM_Damage.STATUS:
                            column.Header = "Status";
                            column.Binding = new Binding(Dt_Cross_CM.Columns[i].ToString());
                            column.IsReadOnly = true;
                            column.MinWidth = 100;
                            dgCrossCM.Columns.Add(column);
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
                            if (Dt_Cross_CM.Columns[i].DataType.Equals(typeof(System.Decimal)))
                            {
                                columnBinding.Converter = new DecimalUIConverter();
                                columnBinding.ConverterCulture = CultureInfo.CurrentUICulture;
                                columnBinding.ConverterParameter = decimalConfig;
                            }
                            column.Header = Dt_Cross_CM.Columns[i].ToString();
                            column.Binding = columnBinding;
                            column.IsReadOnly = false;
                            column.MinWidth = 100;
                            dgCrossCM.Columns.Add(column);
                            break;
                    }
                }
                TreeOperation.OrderTableHierarquical(Dt_Cross_CM, P.TheCurrentLayout.LinesList, DT_CounterM_Damage.ID_COUNTERM);
                DV_Cross_CM = new DataView(Dt_Cross_CM);
                dgCrossCM.ItemsSource = DV_Cross_CM;
                FillTableGroup(myDs);
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
            if (P.TheCurrentLayout != null)
            {
                DsGroupRisk = new DataSet();
                //var query = CurrentLayout.LinesList.GroupBy(x => x.Group.IdGroup);
                List<LineGroup> query = ListaIdGroup(P.TheCurrentLayout.LinesList);
                TreeViewGroup.Items.Clear();
                GroupService.WebServiceGroupe ws = new GroupService.WebServiceGroupe();
                myDs.Tables[DT_Groupe.TABLE_NAME].Merge(ws.GetAllGroupes().Tables[DT_Groupe.TABLE_NAME]);
                ws.Dispose();
                //bool noneFlag = false;//esto lo añadi para que el grupo NONE aparezca primero
                if (query != null)
                {
                    foreach (LineGroup item in query)
                    {
                        if (item.IdGroup != 0)
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add(new DataColumn("ID", typeof(System.Int32)));
                            dt.Columns.Add(new DataColumn("Element", typeof(System.String)));
                            dt.Columns.Add(new DataColumn("Name", typeof(System.String)));
                            dt.Columns.Add(new DataColumn("Father", typeof(System.String)));
                            dt.Columns.Add(new DataColumn("Activated", typeof(System.Boolean)));
                            dt.Columns.Add(new DataColumn("Probability", typeof(System.Decimal)));
                            //dt.Columns.Add(new DataColumn("Element", typeof(System.String)));
                            //dt.TableName = item.Key.ToString();

                            dt.TableName = myDs.Tables[DT_Groupe.TABLE_NAME].Rows.Find(item.IdGroup)[DT_Groupe.GROUPE_NAME].ToString();

                            foreach (var ite in P.TheCurrentLayout.LinesList.Where(x => x.Group.IdGroup == item.IdGroup))
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
                            btnRemovee.AddHandler(MyGroupButton.ClickEvent, new RoutedEventHandler(SomeHandler));
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
                                if (dt.Columns[i].DataType.Equals(typeof(System.Decimal)))
                                {
                                    columnBinding.Converter = new DecimalUIConverter();
                                    columnBinding.ConverterCulture = CultureInfo.CurrentUICulture;
                                    columnBinding.ConverterParameter = decimalConfig;
                                }
                                column.Binding = columnBinding;
                                column.IsReadOnly = true;
                                column.Visibility = (i > 3) ? Visibility.Hidden : Visibility.Visible;
                                dg.Columns.Add(column);
                            }
                            dg.ItemsSource = dt.DefaultView;

                            ((DataView)dg.ItemsSource).Table.RowDeleted += Table_RowDeleting;

                            StackPanel panel = new StackPanel();

                            MyGroupButton btnDetails = new MyGroupButton
                            {
                                Style = ((Button)FindResource("Delete")).Style,
                                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.Eye },
                                ToolTip = "Details",
                                //Name = "BD" + item.Key.ToString(),
                                IdGroup = (Int32)item.IdGroup
                            };
                            btnDetails.Click += BtnDetails_OnClick;
                            MyGroupButton btnAddRisk = new MyGroupButton
                            {
                                Style = ((Button)FindResource("Delete")).Style,
                                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.Plus },
                                ToolTip = "Add More",
                                //Name = "BA" + item.Key.ToString(),
                                IdGroup = (Int32)item.IdGroup
                            };
                            btnAddRisk.Click += BtnAddRisk_Click;
                            MyGroupButton btnRemove = new MyGroupButton
                            {
                                Style = ((Button)FindResource("Delete")).Style,
                                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.DeleteForever },
                                ToolTip = ((Button)FindResource("Delete")).ToolTip,
                                //Name = "B" + item.Key.ToString(),
                                IdGroup = (Int32)item.IdGroup
                            };
                            btnRemove.Click += Remove_Group_Click;

                            MyGroupButton btnChangeName = new MyGroupButton
                            {
                                Style = ((Button)FindResource("Delete")).Style,
                                Content = new MaterialDesignThemes.Wpf.PackIcon() { Kind = MaterialDesignThemes.Wpf.PackIconKind.Pencil },
                                ToolTip = "Rename Group",
                                //Name = "BN" + item.Key.ToString(),
                                IdGroup = (Int32)item.IdGroup
                            };

                            btnChangeName.Click += ChangeGoupName_Click;
                            panel.Children.Add(new TextBlock { Text = dt.TableName, VerticalAlignment = VerticalAlignment.Center });
                            panel.Children.Add(new TextBlock { Text = " " });
                            panel.Children.Add(btnAddRisk);
                            panel.Children.Add(new TextBlock { Text = " " });
                            panel.Children.Add(btnDetails);
                            panel.Children.Add(new TextBlock { Text = " " });
                            panel.Children.Add(btnChangeName);
                            panel.Children.Add(new TextBlock { Text = " " });
                            panel.Children.Add(btnRemove);
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

        public void SomeHandler(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " this value?").ShowDialog() == true)
                {
                    DataRowView row = (DataRowView)((MyGroupButton)e.Source).DataContext;
                    if (row != null)
                    {
                        if (row[1].ToString() == "Risk")
                        {
                            foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID + " = " + row[0] + " and "
                                                                                        + DT_Risk.ID_DIAGRAM + " = " + P.TheCurrentLayout.ID_Diagram))
                            {
                                item[DT_Risk.GROUPE_NAME] = "None";
                                item[DT_Risk.ID_GROUPE] = DBNull.Value;
                                P.TheCurrentLayout.LinesList.Find(x => x.ID == (Int32)item[DT_Risk.ID]).Group.IdGroup = 0;
                                P.TheCurrentLayout.LinesList.Find(x => x.ID == (Int32)item[DT_Risk.ID]).Group.GroupName = "None";
                            }
                        }
                        else
                        {
                            foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID + " = " + row[0] + " and "
                                                                                        + DT_CounterM.ID_RISK_TREE + " = " + P.TheCurrentLayout.ID_Diagram))
                            {
                                item[DT_CounterM.GROUPE_NAME] = "None";
                                item[DT_CounterM.ID_GROUPE] = DBNull.Value;
                                P.TheCurrentLayout.LinesList.Find(x => x.ID == (Int32)item[DT_CounterM.ID]).Group.IdGroup = 0;
                                P.TheCurrentLayout.LinesList.Find(x => x.ID == (Int32)item[DT_CounterM.ID]).Group.GroupName = "None";
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

        private void Table_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            try
            {
                if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " this value?").ShowDialog() == true)
                {
                    foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.GROUPE_NAME + " = '" + ((DataTable)sender).TableName + "' and "
                                                                                + DT_CounterM.ID_RISK_TREE + " = " + P.TheCurrentLayout.ID_Diagram))
                    {
                        if (!(((DataTable)sender).Select("ID = " + item[DT_CounterM.ID] + " and Element = 'CounterMeasure'").Any()))
                        {
                            item[DT_CounterM.GROUPE_NAME] = "None";
                            item[DT_CounterM.ID_GROUPE] = DBNull.Value;
                            P.TheCurrentLayout.LinesList.Find(x => x.ID == (Int32)item[DT_CounterM.ID]).Group.IdGroup = 0;
                            P.TheCurrentLayout.LinesList.Find(x => x.ID == (Int32)item[DT_CounterM.ID]).Group.GroupName = "None";
                        }

                    }
                    foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.GROUPE_NAME + " = '" + ((DataTable)sender).TableName + "' and "
                                                                                    + DT_Risk.ID_DIAGRAM + " = " + P.TheCurrentLayout.ID_Diagram))
                    {
                        if (!(((DataTable)sender).Select("ID = " + item[DT_Risk.ID] + " and Element = 'Risk'").Any()))
                        {
                            item[DT_Risk.GROUPE_NAME] = "None";
                            item[DT_Risk.ID_GROUPE] = DBNull.Value;
                            P.TheCurrentLayout.LinesList.Find(x => x.ID == (Int32)item[DT_Risk.ID]).Group.IdGroup = 0;
                            P.TheCurrentLayout.LinesList.Find(x => x.ID == (Int32)item[DT_Risk.ID]).Group.GroupName = "None";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                top[DT_Risk_Damages.STATUS] = (Boolean)drRisk[DT_Risk.ENABLED] || drRisk[DT_Risk.ENABLED] == DBNull.Value ? "Activated" : "Non activated";
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

        private void Paste(RiskPolyLine risk, DataSet dsSource, DataSet dsResult, int idDiagram, List<RiskPolyLine> polyLines)
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
                            item.ID = (Int32)drRisk[DT_Risk.ID];
                            polyLines.Add(item);
                        }
                        else
                        {
                            foreach (var item2 in item.Children)
                            {
                                item2.IdRiskFather = (Int32)drRisk[DT_Risk.ID];
                            }
                            item.ID = (Int32)drRisk[DT_Risk.ID];
                            polyLines.Add(item);
                            Paste(item, dsSource, dsResult, idDiagram, polyLines);
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
                        item.ID = (Int32)drCM[DT_CounterM.ID];
                        polyLines.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        #endregion


        public void DrawEntireDiagram()
        {
            try
            {
                if (P.TheCurrentLayout.Ds.HasChanges())
                {
                    Cursor = Cursors.Arrow;
                    if (P.TheCurrentLayout.ID_Diagram != 0)
                    {
                        P.TheCurrentLayout.DropLines();
                        P.TheCurrentLayout.DropRectangles();
                        P.TheCurrentLayout.LoadLines();
                        P.TheCurrentLayout.LoadRectangles();
                        P.TheCurrentLayout.DrawNumbers();
                        P.TheCurrentLayout.SetLinesThickness();
                        TextProbabilityChange(P.TheCurrentLayout.MainLine);
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }

        }

        public void TextProbabilityChange(RiskPolyLine line)
        {
            TextProbability.Text = General.MyRound((General.AcumulatedLikelihood(line) * 100), 8).ToString() + " %";
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                    ExportToPng(sf.FileName, P.TheCurrentLayout.GridPaintLines);
                }
                sf.Dispose();
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        /// <summary>
        /// Values for languages
        /// </summary>
        public void ChangeLanguage()
        {
            LoginMenuItem.Header = StringResources.LoginMenu;
            MenuItemExport.ToolTip = StringResources.EXPORT_IMAGE;
            MenuItemPrint.ToolTip = StringResources.PRINT_DIAGRAM;
            MenuItemUserList.Header = StringResources.UserListMenu;
            MenuItemRoleList.Header = StringResources.RoleListMenu;
            MenuItemTopRisk.Header = StringResources.TopRiskList;
            MenuItemOption.Header = StringResources.SettingsMenu;
            //MenuItemResolution.Header = StringResources.ResolutionMenu;
            //MenuItemResolution.ToolTip = StringResources.DesignAreaToolTip;
            MenuItemExit.Header = StringResources.ExitMenu;
            LabelDiagram.Content = StringResources.DiagramNameLabel;
            LabelProbability.Content = StringResources.ExpectedLKLabel;
            cbDiagramValues.Header = StringResources.DiagramValuesGroupB;
            cbFilterTR.Header = StringResources.FilterTRGroupB;
            //cbScale.Header = StringResources.ScaleGroupB;
            ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.AddRisk]).ToolTip = StringResources.AddRiskMenu;
            //((MenuItem)MenuRisk.Items[(Int32)MenuRiskItems.EditRisk]).ToolTip = StringResources.EditRiskMenu;
            //((MenuItem)MenuRisk.Items[(Int32)MenuRiskItems.DelRisk]).ToolTip = StringResources.DeleteRiskMenu;
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
            BtnBackward.ToolTip = StringResources.GoBackTooltip;
            StartMenu.ToolTip = StringResources.StartTooltip;
            MenuItemFish_Tree.ToolTip = StringResources.FISH_MENU_TOOLTIP;
            MenuItemReload.ToolTip = StringResources.REBOOT;
            if (Flag_login)
            {
                Title = "Ensure Risk | " + StringResources.CONNECTED_STRING + " " + LoginUser;
                LoginMenuItem.Header = StringResources.LOGOFF;
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
                        new WindowMessageInformation("Data saved successfully!!!").ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
        private TreeNodeModel<Node> Build_Tree(List<Node> data)
        {
            var root = data.FirstOrDefault(p => p.ParentId == 0);
            var rootTreeNode = new TreeNodeModel<Node>(root, null);

            // add tree node children recursively
            rootTreeNode.Children = GetChildNodes(data, rootTreeNode);

            return rootTreeNode;
        }

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
            P.TheCurrentLayout.GridPaintLines.Children.Add(node.Item.LineaArriba);
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


        /// <summary>
        /// Return to the Main Diagram 
        /// </summary>
        private void BtnBackward_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                P.TheCurrentLayout.IsScoping = false;
                P.TheCurrentLayout.DropLines();
                P.TheCurrentLayout.DropRectangles();
                P.TheCurrentLayout.LoadLines();
                P.TheCurrentLayout.LoadRectangles();
                P.TheCurrentLayout.DrawNumbers();
                BtnBackward.Visibility = Visibility.Hidden;
                TextDiagram.Text = P.TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(P.TheCurrentLayout.ID_Diagram)[DT_Diagram.DIAGRAM_NAME].ToString();
                P.TheCurrentLayout.Title = P.TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(P.TheCurrentLayout.ID_Diagram)[DT_Diagram.DIAGRAM_NAME].ToString();
                TextProbabilityChange(P.TheCurrentLayout.MainLine);
                P.TheCurrentLayout.SetLinesThickness();
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
        private Boolean? CanUseProposedPolyLineName(String proposedPolyLineName)
        {
            Boolean? result = null;
            WindowMessageYesNo yesNo = null;
            if (!(P.TheCurrentLayout.Line_Selected.IsCM))
            {
                //Es un riesgo
                int RiskTreeID = (Int32)P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(P.TheCurrentLayout.Line_Selected.ID)[DT_Risk.ID_DIAGRAM];

                if (P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + RiskTreeID + " and "
                    + DT_Risk.NAMESHORT + " = '" + proposedPolyLineName + "' and " + DT_Risk.ID + " <> " + P.TheCurrentLayout.Line_Selected.ID).Any())
                {
                    yesNo = new WindowMessageYesNo("The name [" + proposedPolyLineName + "] Already exists in this diagram. Do you want to use it again?");
                    yesNo.ShowDialog();
                }
            }
            else
            {
                //Es una contramedida
                int RiskTreeID = (Int32)P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(P.TheCurrentLayout.Line_Selected.IdRiskFather)[DT_Risk.ID_DIAGRAM];

                if (P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK_TREE + " = " + RiskTreeID + " and "
                    + DT_CounterM.NAMESHORT + " = '" + proposedPolyLineName + "' and " + DT_CounterM.ID + " <> " + P.TheCurrentLayout.Line_Selected.ID).Any())
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
            if (P.TheCurrentLayout != null)
            {
                P.TheCurrentLayout.RiskLeave();
            }
        }

        private void DgRisksCross_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            SalvarDatosRiskCross();
        }

        private void DgRisksCross_MouseLeave(object sender, MouseEventArgs e)
        {
            SalvarDatosRiskCross();
            if (P.TheCurrentLayout != null)
            {
                P.TheCurrentLayout.RiskLeave();
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
                            if ((Decimal)item["RiskReduction"] <= 100 && (Decimal)item["RiskReduction"] >= 0)
                            {
                                P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item[DT_CounterM_Damage.ID_COUNTERM])[DT_CounterM.NAMESHORT] = item["nameShort"].ToString().TrimStart();
                                P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item[DT_CounterM_Damage.ID_COUNTERM])[DT_CounterM.PROBABILITY] = ((Decimal)item["RiskReduction"]);
                                P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Find(new object[] { item[DT_CounterM_Damage.ID_COUNTERM], item[DT_CounterM_Damage.ID_DAMAGE] })[DT_CounterM_Damage.COUNTERM_NAMESHORT] = item["nameShort"].ToString().TrimStart();
                                P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Find(new object[] { item[DT_CounterM_Damage.ID_COUNTERM], item[DT_CounterM_Damage.ID_DAMAGE] })[DT_CounterM_Damage.RISK_REDUCTION] = ((Decimal)item["RiskReduction"]);
                                foreach (DataColumn itemi in dtTemp.Columns)
                                {
                                    if (P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + item[DT_CounterM_Damage.ID_COUNTERM] + " and " + DT_CounterM_Damage.DAMAGE + " = '" + itemi.ToString() + "'").Any())
                                    {
                                        if ((Decimal)item[itemi] >= 0)
                                        {
                                            P.TheCurrentLayout.Ds.Tables[DT_CounterM_Damage.TABLENAME].
                                                Select(DT_CounterM_Damage.ID_COUNTERM + " = " + item[DT_CounterM_Damage.ID_COUNTERM] +
                                                " and " + DT_CounterM_Damage.DAMAGE + " = '" + itemi.ToString() + "'").First()[DT_CounterM_Damage.VALUE] = (Decimal)item[itemi];
                                        }
                                        else
                                        {
                                            //CruzarTablaCM();
                                            new WindowMessageOK("Damages Values can't be less than 0. Check again").ShowDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //CruzarTablaCM();
                                new WindowMessageOK("CM Risk Reduction Value can't be less than 0 or higher to 100. No changes apply").ShowDialog();
                            }
                        }
                        else
                        {
                            //CruzarTablaCM();
                            new WindowMessageOK("CM Name Value can´t be empty. No changes apply").ShowDialog();
                        }
                    }
                    Dt_Cross_CM.AcceptChanges();
                    DrawEntireDiagram();
                    P.TheCurrentLayout.ClearFilters();
                    CruzarTablaRisk(P.TheCurrentLayout.Ds);
                    CruzarTablaCM(P.TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                            if ((Decimal)item["probability"] <= 100 && (Decimal)item["probability"] >= 0)
                            {
                                P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item[DT_Risk_Damages.ID_RISK])[DT_Risk.NAMESHORT] = item["nameShort"].ToString().TrimStart();
                                P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item[DT_Risk_Damages.ID_RISK])[DT_Risk.PROBABILITY] = ((Decimal)item["probability"]);
                                P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Find(new object[] { item[DT_Risk_Damages.ID_RISK], item[DT_Risk_Damages.ID_DAMAGE] })[DT_Risk_Damages.RISK_NAMESHORT] = item["nameShort"].ToString().TrimStart();
                                P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Find(new object[] { item[DT_Risk_Damages.ID_RISK], item[DT_Risk_Damages.ID_DAMAGE] })[DT_Risk_Damages.PROBABILITY] = ((Decimal)item["probability"]);
                                foreach (DataColumn itemi in dtTemp.Columns)
                                {
                                    if (P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + item[DT_Risk_Damages.ID_RISK] + " and " + DT_Risk_Damages.DAMAGE + " = '" + itemi.ToString() + "'").Any())
                                    {
                                        if ((Decimal)item[itemi] >= 0)
                                        {
                                            P.TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLENAME].
                                                Select(DT_Risk_Damages.ID_RISK + " = " + item[DT_Risk_Damages.ID_RISK] +
                                                " and " + DT_Risk_Damages.DAMAGE + " = '" + itemi.ToString() + "'").First()[DT_Risk_Damages.VALUE] = (Decimal)item[itemi];
                                        }
                                        else
                                        {
                                            CruzarTablaRisk(P.TheCurrentLayout.Ds);
                                            new WindowMessageOK("Damages Values can't be less than 0. Check again").ShowDialog();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                CruzarTablaRisk(P.TheCurrentLayout.Ds);
                                new WindowMessageOK("Risk Probability Value can't be less than 0 or higher to 100. No changes apply").ShowDialog();
                            }
                        }
                        else
                        {
                            CruzarTablaRisk(P.TheCurrentLayout.Ds);
                            new WindowMessageOK("Risk Name Value can´t be empty. No changes apply").ShowDialog();
                        }
                    }
                    Dt_Cross_Risk.AcceptChanges();
                    DrawEntireDiagram();
                    P.TheCurrentLayout.ClearFilters();
                    CruzarTablaRisk(P.TheCurrentLayout.Ds);
                    CruzarTablaCM(P.TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }

        }

        private void DgCrossCM_MouseLeave(object sender, MouseEventArgs e)
        {
            SalvarDatosCMCross();
            P.TheCurrentLayout.CMLeave(P.TheCurrentLayout.Line_Selected);
        }

        private void DgCrossCM_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            SalvarDatosCMCross();
        }

        private void DgCrossCM_LostFocus(object sender, RoutedEventArgs e)
        {
            SalvarDatosCMCross();
            P.TheCurrentLayout.CMLeave(P.TheCurrentLayout.Line_Selected);
        }

        private void DgRisksCross_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (sender is DataGrid riskDataGrid && riskDataGrid.SelectedItem != null)
                {
                    if (riskDataGrid.SelectedItem is DataRowView)
                    {
                        P.TheCurrentLayout.Line_Selected = P.TheCurrentLayout.LinesList.Find(item => (item.ID == (int)((DataRowView)riskDataGrid.SelectedItem).Row[DT_Risk.ID]));
                        P.TheCurrentLayout.RiskLeave();
                        P.TheCurrentLayout.RiskEnter(P.TheCurrentLayout.Line_Selected, P.TheCurrentLayout.Line_Selected.Points[P.TheCurrentLayout.Line_Selected.Points.Count - 1]);
                        P.TheCurrentLayout.BrigIntoViewSelectedRiskPolyline(P.TheCurrentLayout.Line_Selected);
                        P.TheCurrentLayout.UpdateSelectedPolyLineVisualInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                        P.TheCurrentLayout.Line_Selected = P.TheCurrentLayout.LinesList.Find(item => (item.ID == (int)((DataRowView)cmDataGrid.SelectedItem).Row[DT_CounterM.ID]));
                        P.TheCurrentLayout.CMLeave(P.TheCurrentLayout.Line_Selected);
                        P.TheCurrentLayout.CMEnter(P.TheCurrentLayout.Line_Selected, P.TheCurrentLayout.Line_Selected.Points[P.TheCurrentLayout.Line_Selected.Points.Count - 1]);
                        P.TheCurrentLayout.BrigIntoViewSelectedRiskPolyline(P.TheCurrentLayout.Line_Selected);
                        P.TheCurrentLayout.UpdateSelectedPolyLineVisualInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                        Boolean? canUsePolyLineName = CanUseProposedPolyLineName((e.EditingElement as TextBox).Text);
                        if (canUsePolyLineName.HasValue && !canUsePolyLineName.Value)
                        {
                            (sender as DataGrid).CancelEdit(DataGridEditingUnit.Cell);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                        Boolean? canUsePolyLineName = CanUseProposedPolyLineName((e.EditingElement as TextBox).Text);
                        if (canUsePolyLineName.HasValue && !canUsePolyLineName.Value)
                        {
                            (sender as DataGrid).CancelEdit(DataGridEditingUnit.Cell);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
        #region TreeViewGroupEvents

        private void BtnAddRisk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Cursor = Cursors.Hand;
                P.TheCurrentLayout.SelectingToGroup = true;
                P.TheCurrentLayout.GroupSelected = new LineGroup()
                {
                    IdGroup = ((MyGroupButton)sender).IdGroup,
                    GroupName = P.TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(((MyGroupButton)sender).IdGroup)[DT_Groupe.GROUPE_NAME].ToString()
                };
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void Remove_Group_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " this Group?").ShowDialog() == true)
                {
                    //string number = ((Button)sender).Name.Substring(1);
                    foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_GROUPE + " = " + ((MyGroupButton)sender).IdGroup))
                    {
                        item[DT_Risk.ID_GROUPE] = DBNull.Value;
                        item[DT_Risk.GROUPE_NAME] = "None";
                    }
                    foreach (DataRow item in P.TheCurrentLayout.Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_GROUPE + " = " + ((MyGroupButton)sender).IdGroup))
                    {
                        item[DT_CounterM.ID_GROUPE] = DBNull.Value;
                        item[DT_CounterM.GROUPE_NAME] = "None";
                    }
                    foreach (var item in P.TheCurrentLayout.LinesList.FindAll(x => x.Group.IdGroup == ((MyGroupButton)sender).IdGroup))
                    {
                        item.Group.IdGroup = 0;
                        item.Group.GroupName = "None";
                    }
                    P.TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(((MyGroupButton)sender).IdGroup).Delete();
                    FillTableGroup(P.TheCurrentLayout.Ds);
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void ChangeGoupName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //string number = ((Button)sender).Name.Substring(2);
                WindowRenameGroup wgrp = new WindowRenameGroup()
                {
                    DrGroup = P.TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(((MyGroupButton)sender).IdGroup)
                };

                if (wgrp.ShowDialog() == true)
                {
                    P.TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(((MyGroupButton)sender).IdGroup)[DT_Groupe.GROUPE_NAME] = wgrp.DrGroup[DT_Groupe.GROUPE_NAME];
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                P.TheCurrentLayout.ResetGroupRiksSelection();
                P.TheCurrentLayout.ResetGroupCMSelection();
                if (selectedTreeViewItem.Items.Count > 0)
                {
                    DataView dv = (DataView)((DataGrid)(selectedTreeViewItem.Items[0])).ItemsSource;
                    if (UserHasPermissionOnThisGroup(dv.Table.Rows))
                    {
                        if (dv.Table.Select("Element = 'CounterMeasure'").Any())
                        {
                            P.TheCurrentLayout.ChoosingCM = true;
                        }
                        if (dv.Table.Select("Element = 'Risk'").Any())
                        {
                            P.TheCurrentLayout.ChoosingRisk = true;
                        }
                        foreach (DataRow elementDataRow in dv.Table.Rows)
                        {
                            RiskPolyLine CurrentRiskPolyLine = P.TheCurrentLayout.LinesList.Find(rpl => rpl.ID == (Int32)elementDataRow["ID"]);
                            //CurrentRiskPolyLine.Stroke = new SolidColorBrush(Colors.LightSkyBlue);
                            CurrentRiskPolyLine.SetColor(new SolidColorBrush(System.Windows.Media.Colors.LightSkyBlue));
                            if (CurrentRiskPolyLine.IsCM)
                            {
                                P.TheCurrentLayout.CMGroupSelected.Add(CurrentRiskPolyLine);
                                //UpdateCMCounText(1);
                            }
                            else
                            {
                                P.TheCurrentLayout.RiskGroupSelected.Add(CurrentRiskPolyLine);
                                //UpdateRiskCounText(1);
                            }
                        }
                        if (P.TheCurrentLayout.ChoosingRisk && P.TheCurrentLayout.ChoosingCM)
                        {
                            P.TheCurrentLayout.ResetLinesMenu(P.TheCurrentLayout.RiskGroupSelected, MenuGroupMixed);
                            P.TheCurrentLayout.ResetLinesMenu(P.TheCurrentLayout.CMGroupSelected, MenuGroupMixed);
                        }
                        else
                        {
                            if (P.TheCurrentLayout.ChoosingRisk)
                            {
                                P.TheCurrentLayout.ResetLinesMenu(P.TheCurrentLayout.RiskGroupSelected, MenuGroupRisk);
                            }
                            else
                            {
                                if (P.TheCurrentLayout.ChoosingCM)
                                {
                                    P.TheCurrentLayout.ResetLinesMenu(P.TheCurrentLayout.CMGroupSelected, MenuGroupCM);
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
                        haspermission = UserHasPermisionOnThisCM((Int32)elementDataRow["ID"]);
                        break;
                    case "Risk":
                        haspermission = UserHasPermisionOnThisRisk((Int32)elementDataRow["ID"]);
                        break;
                }
                if (!haspermission)
                {
                    break;
                }
            }
            return haspermission;
        }

        private bool UserHasPermisionOnThisRisk(Int32 elementID)
        {
            bool haspermission = false;
            DataRow[] dr = P.TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + elementID.ToString());
            foreach (DataRow item in dr)
            {
                if (P.TheCurrentLayout.Ds.Tables[DT_User_Role.TABLE_NAME].Select(DT_User_Role.ROLE + " = '" + item[DT_Role_Risk.Role] + "' and " + DT_User_Role.USER + " = '" + LoginUser + "'").Any())
                {
                    haspermission = true;
                    break;
                }
            }
            return haspermission;
        }

        private bool UserHasPermisionOnThisCM(Int32 elementID)
        {
            bool haspermission = false;
            DataRow[] dr = P.TheCurrentLayout.Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + elementID.ToString());
            foreach (DataRow item in dr)
            {
                if (P.TheCurrentLayout.Ds.Tables[DT_User_Role.TABLE_NAME].Select(DT_User_Role.ROLE + " = '" + item[DT_Role_CM.Role] + "' and " + DT_User_Role.USER + " = '" + LoginUser + "'").Any())
                {
                    haspermission = true;
                    break;
                }
            }
            return haspermission;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (P.TheCurrentLayout != null)
            {
                if (e.Key == Key.Escape)
                {
                    if (P.TheCurrentLayout.ChoosingRisk && !P.TheCurrentLayout.ChoosingCM)
                    {
                        P.TheCurrentLayout.ResetGroupRiksSelection();
                    }
                    else if (!P.TheCurrentLayout.ChoosingRisk && P.TheCurrentLayout.ChoosingCM)
                    {
                        P.TheCurrentLayout.ResetGroupCMSelection();
                    }
                    else if (P.TheCurrentLayout.ChoosingRisk && P.TheCurrentLayout.ChoosingCM)
                    {
                        P.TheCurrentLayout.ResetGroupRiksSelection();
                        P.TheCurrentLayout.ResetGroupCMSelection();
                    }
                    else
                    {
                        // clic en el area amarilla sin seleccion multiple
                    }
                }
                if (e.Key == Key.F2)
                {
                    P.TheCurrentLayout.BrigIntoViewSelectedRiskPolyline(P.TheCurrentLayout.Line_Selected);
                    P.TheCurrentLayout.UpdateSelectedPolyLineVisualInfo();
                    P.TheCurrentLayout.EditSelectedPolyLineShorName();
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
        #endregion

        #region Pan Mouse Events
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (isPanEnabled)
            {
                if (P.TheCurrentLayout.ScrollGridPaint.IsMouseOver && e.LeftButton == MouseButtonState.Pressed)
                {
                    // Save starting point, used later when determining how much to scroll.
                    scrollStartPoint = e.GetPosition(this);
                    scrollStartOffset.X = P.TheCurrentLayout.ScrollGridPaint.HorizontalOffset;
                    scrollStartOffset.Y = P.TheCurrentLayout.ScrollGridPaint.VerticalOffset;

                    // Update the cursor if can scroll or not.
                    this.Cursor = (P.TheCurrentLayout.ScrollGridPaint.ExtentWidth > P.TheCurrentLayout.ScrollGridPaint.ViewportWidth) ||
                        (P.TheCurrentLayout.ScrollGridPaint.ExtentHeight > P.TheCurrentLayout.ScrollGridPaint.ViewportHeight) ?
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
                    P.TheCurrentLayout.ScrollGridPaint.ScrollToHorizontalOffset(this.scrollStartOffset.X + delta.X);
                    P.TheCurrentLayout.ScrollGridPaint.ScrollToVerticalOffset(this.scrollStartOffset.Y + delta.Y);
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
            if (P.TheCurrentLayout != null)
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
                WBSFiltering = false;
                TranslateToTreeViewWBS(DsWBS);
                DV_WBS = DsWBS.Tables[DT_WBS.TABLE_NAME].DefaultView;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void TranslateToTreeViewWBS(DataSet ds)
        {
            TreeViewWBS.Items.Clear();
            foreach (DataRow item in BuscarWBSSinPadre(ds))
            {
                MyTreeItem tItem = new MyTreeItem
                {
                    IsExpanded = true,
                    MyID = (Int32)item[DT_WBS.ID_WBS]
                };
                PannelWBS panel = new PannelWBS((Int32)item[DT_WBS.ID_WBS])
                {
                    MyText = item[DT_WBS.NIVEL] + " " + item[DT_WBS.WBS_NAME] + " [" + item[DT_WBS.USERNAME] + "]",
                    MyBtnStyle = ((Button)FindResource("Delete")).Style,
                    Orientation = Orientation.Horizontal
                };

                panel.BtnEye.Click += BtnEye_Click;
                panel.BtnDelete.Click += BtnDelete_Click;
                panel.BtnEdit.Click += BtnEdit_Click;
                panel.BtnEye.Style = panel.BtnEdit.Style = panel.BtnDelete.Style = ((Button)FindResource("Delete")).Style;
                panel.SetText(item[DT_WBS.NIVEL] + " " + item[DT_WBS.WBS_NAME] + " [" + item[DT_WBS.USERNAME] + "]");
                tItem.Header = panel;
                TreeViewWBS.Items.Add(tItem);
                panel.EnableBtns(LoginUser == item[DT_WBS.USERNAME].ToString());
                BuscarHijosWBS(ds, (Int32)item[DT_WBS.ID_WBS], tItem, LoginUser == item[DT_WBS.USERNAME].ToString());
            }
        }

        public void BuscarHijosWBS(DataSet ds, int idFather, MyTreeItem treeItem, bool habilita)
        {
            foreach (DataRow item in ds.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_FATHER + " = " + idFather))
            {
                MyTreeItem tItem = new MyTreeItem
                {
                    IsExpanded = true,
                    MyID = (Int32)item[DT_WBS_STRUCTURE.ID_CHILD]
                };
                PannelWBS panel = new PannelWBS((Int32)item[DT_WBS_STRUCTURE.ID_CHILD])
                {
                    MyText = item[DT_WBS_STRUCTURE.CNIVEL] + " " + item[DT_WBS_STRUCTURE.CHILD] + " [" + item[DT_WBS_STRUCTURE.CHILD_USER] + "]",
                    MyBtnStyle = ((Button)FindResource("Delete")).Style,
                    MyID = (Int32)item[DT_WBS_STRUCTURE.ID_CHILD],
                    Orientation = Orientation.Horizontal
                };
                panel.BtnEye.Click += BtnEye_Click;
                panel.BtnDelete.Click += BtnDelete_Click;
                panel.BtnEdit.Click += BtnEdit_Click;
                panel.BtnEye.Style = panel.BtnEdit.Style = panel.BtnDelete.Style = ((Button)FindResource("Delete")).Style;
                panel.SetText(item[DT_WBS_STRUCTURE.CNIVEL] + " " + item[DT_WBS_STRUCTURE.CHILD] + " [" + item[DT_WBS_STRUCTURE.CHILD_USER] + "]");
                tItem.Header = panel;
                treeItem.Items.Add(tItem);
                if (habilita)
                {
                    BuscarHijosWBS(ds, (Int32)item[DT_WBS_STRUCTURE.ID_CHILD], tItem, habilita);
                }
                else
                {
                    bool vari = false;
                    if (LoginUser == item[DT_WBS_STRUCTURE.CHILD_USER].ToString())
                    {
                        panel.EnableBtns(true);
                        vari = true;
                        BuscarHijosWBS(ds, (Int32)item[DT_WBS_STRUCTURE.ID_CHILD], tItem, vari);
                    }
                    else
                    {
                        panel.EnableBtns(false);
                        vari = false;
                        BuscarHijosWBS(ds, (Int32)item[DT_WBS_STRUCTURE.ID_CHILD], tItem, vari);
                    }

                }

            }
        }

        private void BtnEye_Click(object sender, RoutedEventArgs e)
        {
            if (P.TheCurrentLayout != null && OpenedDocuments.Count > 0)
            {
                if (sender is ButtonWBS btnEye)
                {
                    try
                    {
                        if (P.TheCurrentLayout != null)
                        {
                            System.Drawing.Color drawingCColor = System.Drawing.Color.FromArgb(int.Parse(P.TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + P.TheCurrentLayout.ID_Diagram)[P.TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));

                            DataRow dr = DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(btnEye.IdWBS);
                            foreach (var item in P.TheCurrentLayout.LinesList)
                            {
                                if (item.IsCM)
                                {
                                    if (!(P.TheCurrentLayout.Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + item.ID + " and " + DT_CM_WBS.ID_WBS + " = " + dr[DT_WBS.ID_WBS]).Any()))
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
                                    if (!(P.TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + item.ID + " and " + DT_RISK_WBS.ID_WBS + " = " + dr[DT_WBS.ID_WBS]).Any()))
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
                        new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
            }

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is ButtonWBS btnWBS)
                {
                    DataRow fila = DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(btnWBS.IdWBS);
                    if (new WindowMessageYesNo(StringResources.DELETE_MESSAGE + " [" + fila[DT_WBS.WBS_NAME] + "]?").ShowDialog() == true)
                    {
                        General.EliminarMisHijosWBS(General.MyWBSChildren(fila, DsWBS.Tables[DT_WBS.TABLE_NAME], DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME]), DsWBS.Tables[DT_WBS.TABLE_NAME], DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME]);
                        fila.Delete();
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
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog(); ;
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
            if (P?.TheCurrentLayout != null)
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
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }


        private void BtnDeselect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (P.TheCurrentLayout != null)
                {
                    //dgWBS.SelectedIndex = -1;
                    P.TheCurrentLayout.DropLines();
                    P.TheCurrentLayout.LoadLines();
                    P.TheCurrentLayout.LoadRectangles();
                    P.TheCurrentLayout.DrawNumbers();
                    P.TheCurrentLayout.SetLinesThickness();
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        #endregion       

        #endregion
    }
}
