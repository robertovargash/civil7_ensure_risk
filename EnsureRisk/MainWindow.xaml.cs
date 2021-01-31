using DataMapping.Data;
using EnsureBusinesss;
using EnsureBusinesss.Business;
using EnsureRisk.Classess;
using EnsureRisk.Controllers.Import;
using EnsureRisk.DataBinding;
using EnsureRisk.Resources;
using EnsureRisk.Windows;
using EnsureRisk.Windows.Logs;
using ERDock.Layout;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace EnsureRisk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region BindingStuff
        private string line_Selected = "None";
        private string textFilterRisk = "";
        private string textFilterCM = "";
        private string type_selected = "None";
        private string _riskshortName = "";
        private decimal idProject;
        private decimal idWBSFilterSelected;
        private bool showRiskData = false;
        private bool isCalculatingRisk = false;
        private bool isCalculatingCM = false;
        private bool isPanEnabled = false;
        public bool IsPanEnabled { get { return isPanEnabled; } set { isPanEnabled = value; OnPropertyChanged("IsPanEnabled"); } }
        private bool isImporting = false;
        private bool canEditDiagram = false;
        private bool canDeleteDiagram = false;        
        private DataView dvCBWBS;
        private DataView dvCBProjects;
        private DataView dv_CrossRisk;
        private DataView dv_Cross_CM;
        private DataView dVRisk_Tree;
        private readonly BackgroundWorker importToExcelWorker = new BackgroundWorker();
        public bool IsCalculatingRisk { get { return isCalculatingRisk; } set { isCalculatingRisk = value; OnPropertyChanged("IsCalculatingRisk"); } }
        public bool IsCalculatingCM { get { return isCalculatingCM; } set { isCalculatingCM = value; OnPropertyChanged("IsCalculatingCM"); } }
        public bool CanEditDiagram { get { return canEditDiagram; } set { canEditDiagram = value; OnPropertyChanged("CanEditDiagram"); } }
        public bool CanDeleteDiagram { get { return canDeleteDiagram; } set { canDeleteDiagram = value; OnPropertyChanged("CanDeleteDiagram"); } }

        private bool isWBSEyed = false;

        public bool IsWBSEyedOff { get { return isWBSEyed; } set { isWBSEyed = value; OnPropertyChanged("IsWBSEyedOff"); } }

        public bool IsImporting { get { return isImporting; } set { isImporting = value; OnPropertyChanged("IsImporting"); } }


        private bool EditandoRisk;
        private bool SeleccionandoRisk;
        private bool hasAccess;
        private bool copiando;
        private decimal probability;
        private MyLayoutDocumentt theCurrentLayout;
        public string RiskLineName { get { return _riskshortName; } set { _riskshortName = value; OnPropertyChanged("RiskLineName"); } }
        public bool COPIANDO { get { return copiando; } set { copiando = value; OnPropertyChanged("COPIANDO"); } }
        public bool HasAccess { get { return hasAccess; } set { hasAccess = value; OnPropertyChanged("HasAccess"); } }
        public MyLayoutDocumentt TheCurrentLayout { get { return theCurrentLayout; } set { theCurrentLayout = value; OnPropertyChanged("TheCurrentLayout"); } }
        //public decimal IdWBSFilter { get { return idwbsfilter; } set { idwbsfilter = value; OnPropertyChanged("IdWBSFilter"); } }

        public string LSelected { get { return line_Selected; } set { line_Selected = value; OnPropertyChanged("LSelected"); } }

        public string TSelected { get { return type_selected; } set { type_selected = value; OnPropertyChanged("TSelected"); } }

        public decimal Probability { get { return probability; } set { probability = value; OnPropertyChanged("Probability"); } }

        public bool ShowRiskData { get { return showRiskData; } set { showRiskData = value; OnPropertyChanged("ShowRiskData"); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

       
        #endregion

        #region Attributes&Properties 
        #region Pan
        public List<MyLayoutDocumentt> OpenedDocuments { get; set; }
        #endregion

        #region Menus
        ContextMenu MenuRisk { get { return (ContextMenu)Resources["MenuRisk"]; } }
        //MyTreeItemGroup TheTreeItem { get { return (MyTreeItemGroup)Resources["Mytree"]; } }

        ContextMenu MenuMainRisk { get { return (ContextMenu)Resources["MenuMainRisk"]; } }
        ContextMenu MenuCM { get { return (ContextMenu)Resources["MenuCM"]; } }
        ContextMenu MenuGroupRisk { get { return (ContextMenu)Resources["MenuGroupRisk"]; } }
        ContextMenu MenuGroupCM { get { return (ContextMenu)Resources["MenuGroupCM"]; } }
        ContextMenu MenuGroupMixed { get { return (ContextMenu)Resources["MenuGroupMixed"]; } }
        #endregion

        #region Lists       
        private ObservableCollection<WBSNodes> mRootNodes;
        public ObservableCollection<WBSNodes> WBSNodeList { get { return mRootNodes; } set { mRootNodes = value; OnPropertyChanged("WBSNodeList"); } }
        public List<decimal> AccessList { get; set; }
        public List<Node> Nodos = new List<Node>();
        public List<RiskPolyLine> GlobalListCopy { get; set; }
        public RiskPolyLine GlobalCopyLine { get; set; }
        #endregion     

        #region DataManagers
        public DataView DVRisk_Tree { get { return dVRisk_Tree; } set { dVRisk_Tree = value; OnPropertyChanged("DVRisk_Tree"); } }
        //public DataTable Dt_Cross_Risk { get; set; }
        //public DataTable Dt_Cross_CM { get; set; }
        public DataView DV_CrossRisk { get { return dv_CrossRisk; } set { dv_CrossRisk = value; OnPropertyChanged("DV_CrossRisk"); } }
        public DataView DV_Cross_CM { get { return dv_Cross_CM; } set { dv_Cross_CM = value; OnPropertyChanged("DV_Cross_CM"); } }
        public DataView DV_WBS { get; set; }

        public DataView DvCBWBS { get { return dvCBWBS; } set { dvCBWBS = value; OnPropertyChanged("DvCBWBS"); } }
        public DataView DvCBProjects { get { return dvCBProjects; } set { dvCBProjects = value; OnPropertyChanged("DvCBProjects"); } }

        public DataSet DsMain { get; set; }
        public DataSet DsWBS { get; set; }
        private DataView dvRiskWBS;
        public DataView DvRiskWBS { get { return dvRiskWBS; } set { dvRiskWBS = value; OnPropertyChanged("DvRiskWBS"); } }
        private DataView dvRoleRisk;
        public DataView DvRoleRisk { get { return dvRoleRisk; } set { dvRoleRisk = value; OnPropertyChanged("DvRoleRisk"); } }
        private DataView dvRiskDamages;
        public DataView DvRiskDamages { get { return dvRiskDamages; } set { dvRiskDamages = value; OnPropertyChanged("DvRiskDamages"); } }
        public DataRow Selected_RiskRoleRow { get; set; }
        public DataRow Selected_Risk_WBS_Row { get; set; }        
        #endregion

        #region DecimalUIConverterParams
        static DecimalUIConverterParams decimalConfig = new DecimalUIConverterParams(Properties.Settings.Default.DecimalsStringFormat, Properties.Settings.Default.DecimalFractionalDigits);
        #endregion
        #region Another Properties & Atributes
        //public int IdWBS { get; set; }
        private bool Flag_login;
        public string LoginUser { get; set; }

        private readonly bool yaCargoTodo = false;
       
        public decimal IdProject { get { return idProject; } set { idProject = value; OnID_ProjectChanged("IdProject"); } }
        public decimal IdWBSFilterSelected { get { return idWBSFilterSelected; } set { idWBSFilterSelected = value; OnIdWBSFilterSelectedChanged("IdWBSFilterSelected"); } }
        public string TextFilterRisk { get { return textFilterRisk; } set { textFilterRisk = value; OnPropertyChanged("TextFilterRisk"); } }
        public string TextFilterCM { get { return textFilterCM; } set { textFilterCM = value; OnPropertyChanged("TextFilterCM"); } }
        private void OnID_ProjectChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            if (yaCargoTodo && IdProject != 0)
            {
                RefreshData();
                RefreshWBS();
            }
        }
        private void OnIdWBSFilterSelectedChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            if (yaCargoTodo && IdWBSFilterSelected != 0)
            {
                if (TheCurrentLayout != null)
                {
                    TheCurrentLayout.IdWBSFilter = IdWBSFilterSelected;
                    CrossRiskRightTab(TheCurrentLayout.Ds);
                    CrossCMRightTab(TheCurrentLayout.Ds);
                }
            }
        }

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
        public bool IS_DELETING_GROUP_CM { get; set; } = false;
        public bool IS_DELETING_RISK_FROM_GROUP_TAB { get; set; } = false;
        public bool IS_REMOVING_GROUP_FROM_DATABASE { get; set; } = false;
        public bool IS_CLEANING_GROUP { get; set; } = false;
        public bool IS_SAVING_DATA { get; set; } = false;
        public bool IS_REPEATING_NAME { get; set; } = false;
        public bool IS_DELETING_WBS { get; private set; } = false;
        public bool IS_LOGIN { get; private set; } = false;
        public bool IS_DELETING_RISK_ROLE_ROW { get; private set; } = false;
        public bool IS_DELETING_Risk_WBS_ROW { get; private set; } = false;
        #endregion

        public decimal DiagramID { get; private set; } = 0;
        public decimal ID_Groupe { get; private set; }
        public DataRowView TheDataRowVieww { get; private set; }
        public decimal IdRiskToDeleteFromGroup { get; private set; }
        public DataTable TheRemovingTable { get; private set; }
        public DataRow DrWBStoDelete { get; private set; }
        public DataGrid Selected_DataGrid { get; private set; }
        #endregion
        #endregion

        #region COMMANDS
        #region WBS_COMMANDS
        public RelayyCommand AddWBSCommand { get; set; }
        private void ImplementAddWBSCommand()
        {
            try
            {
                AddWBSCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (parametro is bool epa)
                        {
                            if (epa)
                            {
                                WindowWBS wbs = new WindowWBS
                                {
                                    DrWBS = DsWBS.Tables[DT_WBS.TABLE_NAME].NewRow(),
                                    IdProject = IdProject,
                                    //WBS_Structure = DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Copy(),
                                    WBS_Encoder = DsWBS.Tables[DT_WBS.TABLE_NAME].Copy(),
                                    Operation = General.INSERT,
                                    Icon = Icon
                                };
                                if (wbs.ShowDialog() == true)
                                {
                                    DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Add(wbs.DrWBS);
                                    //DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Merge(wbs.WBS_Structure);
                                    DsWBS.Tables[DT_WBS.TABLE_NAME].Merge(wbs.WBS_Encoder);
                                    if (DsWBS.HasChanges())
                                    {
                                        using (ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS())
                                        {
                                            DataSet temp = DsWBS.GetChanges();
                                            temp = ws.SaveWBS(temp);
                                            DsWBS.Merge(temp);
                                            DsWBS.AcceptChanges();
                                            RefreshWBS();
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
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }            
        }

        public RelayyCommand CollapseWBSCommand { get; set; }
        private void ImplementCollapseWBSCommand()
        {
            try
            {
                CollapseWBSCommand = new RelayyCommand(
                   _ =>
                   {
                       try
                       {
                           foreach (var wbsNode in WBSNodeList)
                           {
                               wbsNode.IsExpanded = false;
                           }
                       }
                       catch (Exception ex)
                       {
                           MostrarErrorDialog(ex.Message);
                       }
                   });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }   
        }
        
        public RelayyCommand UnSelectWBSCommand { get; set; }
        private void ImplementUnSelectCommand()
        {
            try
            {
                UnSelectWBSCommand = new RelayyCommand(
              _ =>
              {
                  try
                  {
                      IsWBSEyedOff = false;

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
              });                
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public RelayyCommand EyeWBSCommand { get; set; }
        private void ImplementEyeCommand()
        {
            try
            {
                EyeWBSCommand = new RelayyCommand(
              parameter =>
              {
                  try
                  {
                      if (parameter is decimal idBWS)
                      {
                          IsWBSEyedOff = true;
                          if (TheCurrentLayout != null)
                          {
                              Color drawingCColor = ((SolidColorBrush)new BrushConverter().ConvertFrom(TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram)[TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString())).Color;

                              foreach (var line in TheCurrentLayout.LinesList)
                              {
                                  if (!(TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + line.ID + " and " + DT_RISK_WBS.ID_WBS + " = " + idBWS).Any()))
                                  {
                                      if (line.IsCM)
                                      {
                                          line.SetColor(new SolidColorBrush(Color.FromArgb(50, Colors.Black.R, Colors.Black.G, Colors.Black.B)));
                                      }
                                      else
                                      {
                                          line.SetColor(new SolidColorBrush(Color.FromArgb(50, drawingCColor.R, drawingCColor.G, drawingCColor.B)));
                                      }
                                  }
                                  else
                                  {
                                      if (line.IsCM)
                                      {
                                          line.SetColor(new SolidColorBrush(Colors.Black));
                                      }
                                      else
                                      {
                                          line.SetColor(new SolidColorBrush(Color.FromArgb(drawingCColor.A, drawingCColor.R, drawingCColor.G, drawingCColor.B)));
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
              });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public RelayyCommand EditTreeWBSCommand { get; set; }
        private void ImplementEditTreeWBSCommand()
        {
            try
            {
                EditTreeWBSCommand = new RelayyCommand(
              parameter =>
              {
                  try
                  {
                      if (parameter is decimal idBWS)
                      {
                          DataRow dr = DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(idBWS);
                          WindowWBS wbs = new WindowWBS
                          {
                              DrWBS = dr,
                              IdProject = IdProject,
                              //WBS_Structure = DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Copy(),
                              WBS_Encoder = DsWBS.Tables[DT_WBS.TABLE_NAME].Copy(),
                              Operation = General.UPDATE,
                              Icon = Icon
                          };
                          if (wbs.ShowDialog() == true)
                          {
                              DsWBS.Tables[DT_WBS.TABLE_NAME].Merge(wbs.WBS_Encoder);
                              //DsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Merge(wbs.WBS_Structure);
                              if (DsWBS.HasChanges())
                              {
                                  //DataSet temp = new DataSet();
                                  using (ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS())
                                  {
                                      DataSet temp = DsWBS.GetChanges();
                                      temp = ws.SaveWBS(temp);
                                      DsWBS.Merge(temp);
                                      DsWBS.AcceptChanges();
                                      RefreshWBS();
                                  }
                              }
                          }
                      }
                  }
                  catch (Exception ex)
                  {
                      MostrarErrorDialog(ex.Message);
                  }
              });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public RelayyCommand DeleteTreeWBSCommand { get; set; }
        private void ImplementDeleteTreeWBSCommand()
        {
            try
            {
                DeleteTreeWBSCommand = new RelayyCommand(
              parameter =>
              {
                  try
                  {
                      if (parameter is decimal idBWS)
                      {
                          DrWBStoDelete = DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(idBWS);
                          IS_DELETING_WBS = true;
                          MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + DrWBStoDelete[DT_WBS.WBS_NAME] + "]?");
                      }
                  }
                  catch (Exception ex)
                  {
                      MostrarErrorDialog(ex.Message);
                  }
              });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        #endregion

        #region PROPERTIES_TAB_COMMANDS
        #region RISK_GENERAL_VALUES

        private IEnumerable<string> _RiskNameList;
        public IEnumerable<string> RiskNameList { get { return _RiskNameList; } set { _RiskNameList = value; OnPropertyChanged("RiskNameList"); } }

        private RelayyCommand _RiskName_KeyUpCommand;
        public RelayyCommand RiskName_KeyUpCommand { get { return _RiskName_KeyUpCommand; } set { _RiskName_KeyUpCommand = value; OnPropertyChanged("RiskName_KeyUpCommand"); } }
        private void ImplementRiskName_KeyUpCommand()
        {
            try
            {
                RiskName_KeyUpCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (parametro is string textico)
                        {
                            EditandoRisk = true;
                            
                            RiskNameList = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].AsEnumerable()
                                            .Where(r => r.Field<string>(DT_Risk.NAMESHORT).ToLower().Contains(textico.ToLower()))
                                            .Select(r=>r.Field<string>(DT_Risk.NAMESHORT)).Distinct();
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        //private void AddRiskItem(string text)
        //{
        //    TextBlock block = new TextBlock
        //    {
        //        Text = text,
        //        Margin = new Thickness(2, 3, 2, 3),
        //        Cursor = Cursors.Hand
        //    };

        //    block.MouseLeftButtonUp += (sender, e) =>
        //    {
        //        wrapRisk.TextRisk.InputText = (sender as TextBlock).Text;
        //        wrapRisk.scrollRiskAutocomplete.Visibility = Visibility.Collapsed;
        //        wrapRisk.TextRisk.Focus();
        //    };

        //    block.MouseEnter += (sender, e) =>
        //    {
        //        TextBlock b = sender as TextBlock;
        //        b.Background = Brushes.PeachPuff;
        //        SeleccionandoRisk = true;
        //    };

        //    block.MouseLeave += (sender, e) =>
        //    {
        //        TextBlock b = sender as TextBlock;
        //        b.Background = Brushes.Transparent;
        //        SeleccionandoRisk = false;
        //    };

        //    wrapRisk.RiskresultStack.Children.Add(block);
        //}


        private RelayyCommand _TextRisk_LostFocusCommand;
        public RelayyCommand TextRisk_LostFocusCommand { get { return _TextRisk_LostFocusCommand; } set { _TextRisk_LostFocusCommand = value; OnPropertyChanged("TextRisk_LostFocusCommand"); } }
        private void ImplementTextRisk_LostFocusCommand()
        {
            try
            {
                TextRisk_LostFocusCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                        {
                            if (EditandoRisk)
                            {
                                if (!SeleccionandoRisk)
                                {
                                    //wrapRisk.scrollRiskAutocomplete.Visibility = Visibility.Collapsed;
                                    EditandoRisk = false;
                                }
                            }
                            if (wrapRisk.TextRisk.InputText != string.Empty)
                            {
                                if (TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + DiagramID + " and " + DT_Risk.NAMESHORT + " = '" + wrapRisk.TextRisk.InputText + "' and " + DT_Risk.ID + " <> " + TheCurrentLayout.Line_Selected.ID).Any())
                                {
                                    if (new WindowMessageYesNo("The name [" + wrapRisk.TextRisk.InputText + "] Already exists in this diagram. Do you want to use it again?").ShowDialog() == true)
                                    {
                                        TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_Risk.NAMESHORT] = wrapRisk.TextRisk.InputText;
                                        foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + TheCurrentLayout.Line_Selected.ID))
                                        {
                                            item[DT_Risk_Damages.RISK_NAMESHORT] = wrapRisk.TextRisk.InputText;
                                        }
                                        TheCurrentLayout.Line_Selected.ShortName = wrapRisk.TextRisk.InputText;

                                        TreeOperation.SetRiskLineValues(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                                        TheCurrentLayout.DrawNumbers();
                                        TheCurrentLayout.UpdateLinesValues();
                                        TheCurrentLayout.SetLinesThickness();
                                        CrossRiskRightTab(TheCurrentLayout.Ds);
                                    }
                                    else
                                    {
                                        wrapRisk.TextRisk.InputText = TheCurrentLayout.Line_Selected.ShortName;
                                    }
                                }
                                else
                                {
                                    TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_Risk.NAMESHORT] = wrapRisk.TextRisk.InputText;
                                    foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + TheCurrentLayout.Line_Selected.ID))
                                    {
                                        item[DT_Risk_Damages.RISK_NAMESHORT] = wrapRisk.TextRisk.InputText;
                                    }
                                    TheCurrentLayout.Line_Selected.ShortName = wrapRisk.TextRisk.InputText;
                                    TreeOperation.SetRiskLineValues(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                                    TheCurrentLayout.DrawNumbers();
                                    TheCurrentLayout.UpdateLinesValues();
                                    TheCurrentLayout.SetLinesThickness();
                                    CrossRiskRightTab(TheCurrentLayout.Ds);
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
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _RiskName_TextChangedCommand;
        public RelayyCommand RiskName_TextChangedCommand { get { return _RiskName_TextChangedCommand; } set { _RiskName_TextChangedCommand = value; OnPropertyChanged("RiskName_TextChangedCommand"); } }
        private void ImplementRiskName_TextChangedCommand()
        {
            try
            {
                RiskName_TextChangedCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        RiskLineName = wrapRisk.TextRisk.InputText;
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _TextRiskDetail_LostFocusCommand;
        public RelayyCommand TextRiskDetail_LostFocusCommand { get { return _TextRiskDetail_LostFocusCommand; } set { _TextRiskDetail_LostFocusCommand = value; OnPropertyChanged("TextRiskDetail_LostFocusCommand"); } }
        private void ImplementTextRiskDetail_LostFocusCommand()
        {
            try
            {
                TextRiskDetail_LostFocusCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (parametro is string textogrande)
                        {
                            if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                            {
                                TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_Risk.COMMENTS] = textogrande;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        #endregion

        #region RISK_DAMAGE_COMMANDS
        private RelayyCommand _DamageValueGotFocusCommand;
        public RelayyCommand DamageValueGotFocusCommand { get { return _DamageValueGotFocusCommand; } set { _DamageValueGotFocusCommand = value; OnPropertyChanged("DamageValueGotFocusCommand"); } }
        private void ImplementDamageValueGotFocusCommand()
        {
            try
            {
                DamageValueGotFocusCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (parametro is TextBox theText)
                        {
                            theText.SelectAll();
                            //string texto = theText.Text;
                            //Binding columnBinding = new Binding(texto);
                            //columnBinding.Converter = new DecimalUIConverterV2();
                            //columnBinding.ConverterCulture = CultureInfo.CurrentUICulture;
                            //columnBinding.ConverterParameter = decimalConfig;
                            //theText.SetBinding(TextBox.TextProperty, columnBinding);

                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _DamageValueLostFocusCommand;
        public RelayyCommand DamageValueLostFocusCommand { get { return _DamageValueLostFocusCommand; } set { _DamageValueLostFocusCommand = value; OnPropertyChanged("DamageValueLostFocusCommand"); } }
        private void ImplementDamageValueLostFocusCommand()
        {
            try
            {
                DamageValueLostFocusCommand = new RelayyCommand(
                _ =>
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
                            CrossRiskRightTab(TheCurrentLayout.Ds);
                            CrossCMRightTab(TheCurrentLayout.Ds);
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
        #endregion

        #region WBS_RISK_TABLE_COMMANDS
        private RelayyCommand _ProbabilityLostFocusCommand;
        public RelayyCommand ProbabilityLostFocusCommand { get { return _ProbabilityLostFocusCommand; } set { _ProbabilityLostFocusCommand = value; OnPropertyChanged("ProbabilityLostFocusCommand"); } }
        private void ImplementProbabilityLostFocusCommand()
        {
            try
            {
                ProbabilityLostFocusCommand = new RelayyCommand(
                _ =>
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
                            CrossRiskRightTab(TheCurrentLayout.Ds);
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _AddWBSRiskCommand;
        public RelayyCommand AddWBSRiskCommand { get { return _AddWBSRiskCommand; } set { _AddWBSRiskCommand = value; OnPropertyChanged("AddWBSRiskCommand"); } }
        private void ImplementAddWBSRiskCommand()
        {
            try
            {
                AddWBSRiskCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                       if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                        {
                            DataRow RiskRow = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID);
                            decimal ID_PROJECT = (decimal)TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(TheCurrentLayout.ID_Diagram)[DT_Diagram.ID_PROJECT];
                            using (ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS())
                            {

                                DataTable wbsEncoder = ws.GetAllWBSFiltered(new object[] { IdProject }).Tables[DT_WBS.TABLE_NAME].Copy();

                                WindowSelection frmSelection = new WindowSelection();
                                if (TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + (decimal)RiskRow[DT_Risk.ID]).Count() > 0)
                                {
                                    if (TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + (decimal)RiskRow[DT_Risk.ID] + " and " + DT_RISK_WBS.USERNAME + " = '" + LoginUser + "'").Any())
                                    {
                                        //frmSelection.Dt = General.DeleteExists(wbsEncoder, TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + (decimal)RiskRow[DT_Risk.ID]).CopyToDataTable(), DT_WBS.ID_WBS);
                                        DataTable dtExistence = General.DeleteExists(wbsEncoder, TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + (decimal)RiskRow[DT_Risk.ID]).CopyToDataTable(), DT_WBS.ID_WBS);
                                        frmSelection.Dt = WBSOperations.GetMyDescendantsAndDeleteExistents(wbsEncoder, TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME], dtExistence, LoginUser);
                                    }
                                    else
                                    {
                                        frmSelection.Dt = wbsEncoder.Clone();
                                    }
                                }
                                else
                                {
                                    frmSelection.Dt = wbsEncoder;
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
                                        DataRow drRiskWBS = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                                        drRiskWBS[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                                        drRiskWBS[DT_RISK_WBS.RISK] = wrapRisk.TextRisk.InputText;
                                        drRiskWBS[DT_RISK_WBS.WBS] = itemWBS[DT_WBS.WBS_NAME].ToString().TrimStart();
                                        drRiskWBS[DT_RISK_WBS.ID_WBS] = itemWBS[DT_WBS.ID_WBS];
                                        drRiskWBS[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                        drRiskWBS[DT_RISK_WBS.IS_PRIMARY] = false;
                                        drRiskWBS[DT_RISK_WBS.USERNAME] = itemWBS[DT_WBS.USERNAME];
                                        drRiskWBS[DT_RISK_WBS.WBS_USER] = itemWBS[DT_WBS.WBS_NAME] + "[" + itemWBS[DT_WBS.USERNAME] + "]";
                                        drRiskWBS[DT_RISK_WBS.PROBABILITY] = 100;
                                        TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBS);
                                        foreach (DataRow itemAncestors in WBSOperations.GetAncestors((decimal)itemWBS[DT_WBS.ID_WBS], DsWBS.Tables[DT_WBS.TABLE_NAME].Clone(), DsWBS).Rows)
                                        {
                                            if (!(TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { RiskRow[DT_Risk.ID], itemAncestors[DT_WBS.ID_WBS] })))
                                            {
                                                DataRow drRiskWBSi = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                                                drRiskWBSi[DT_RISK_WBS.ID_RISK] = RiskRow[DT_Risk.ID];
                                                drRiskWBSi[DT_RISK_WBS.RISK] = wrapRisk.TextRisk.InputText;
                                                drRiskWBSi[DT_RISK_WBS.WBS] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart();
                                                drRiskWBSi[DT_RISK_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                                drRiskWBSi[DT_RISK_WBS.NIVEL] = itemAncestors[DT_WBS.NIVEL].ToString().TrimStart();
                                                drRiskWBSi[DT_RISK_WBS.USERNAME] = itemAncestors[DT_WBS.USERNAME];
                                                drRiskWBSi[DT_RISK_WBS.WBS_USER] = itemAncestors[DT_WBS.WBS_NAME] + "[" + itemAncestors[DT_WBS.USERNAME] + "]";
                                                drRiskWBSi[DT_RISK_WBS.IS_PRIMARY] = false;
                                                drRiskWBSi[DT_RISK_WBS.PROBABILITY] = 100;
                                                TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBSi);
                                            }
                                        }
                                        foreach (var itemLine in TreeOperation.GetOnlyMyChildrenWithCM(TheCurrentLayout.Line_Selected))
                                        {
                                            if (!(TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { itemLine.ID, itemWBS[DT_WBS.ID_WBS] })))
                                            {
                                                DataRow drRiskWBSS = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                                                drRiskWBSS[DT_RISK_WBS.ID_RISK] = itemLine.ID;
                                                drRiskWBSS[DT_RISK_WBS.RISK] = itemLine.ShortName;
                                                drRiskWBSS[DT_RISK_WBS.WBS] = itemWBS[DT_WBS.WBS_NAME].ToString().TrimStart();
                                                drRiskWBSS[DT_RISK_WBS.ID_WBS] = itemWBS[DT_WBS.ID_WBS];
                                                drRiskWBSS[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                                drRiskWBSS[DT_RISK_WBS.USERNAME] = itemWBS[DT_WBS.USERNAME];
                                                drRiskWBSS[DT_RISK_WBS.WBS_USER] = itemWBS[DT_WBS.WBS_NAME] + "[" + itemWBS[DT_WBS.USERNAME] + "]";
                                                drRiskWBSS[DT_RISK_WBS.IS_PRIMARY] = false;
                                                drRiskWBSS[DT_RISK_WBS.PROBABILITY] = 100;
                                                TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBSS);
                                            }
                                            foreach (DataRow itemWBSRisk in TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + itemLine.ID))
                                            {
                                                if (WBSOperations.IsRiskWBSLow(itemWBSRisk, DsWBS, TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME]))
                                                {
                                                    WBSOperations.TabAddWBS_LINE_Damage(itemWBSRisk, itemLine.ID, false, TheCurrentLayout.Ds);
                                                }
                                                else
                                                {//TENGO QUE BORRAR EL DAMAGE_WBS_RISK, PUES YA NO ES LOWLEVEL
                                                    WBSOperations.TabDeleteWBS_LINE_Damage(itemWBSRisk, itemLine.ID, false, TheCurrentLayout.Ds);
                                                }
                                            }
                                            foreach (DataRow itemRISKWBSi in TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + itemLine.ID))
                                            {
                                                foreach (DataRow itemAncestors in WBSOperations.GetAncestors((decimal)itemRISKWBSi[DT_RISK_WBS.ID_WBS], DsWBS.Tables[DT_WBS.TABLE_NAME].Clone(), DsWBS).Rows)
                                                {
                                                    if (!(TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { itemLine.ID, itemAncestors[DT_WBS.ID_WBS] })))
                                                    {
                                                        DataRow drRiskWBSi = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].NewRow();
                                                        drRiskWBSi[DT_RISK_WBS.ID_RISK] = itemLine.ID;
                                                        drRiskWBSi[DT_RISK_WBS.RISK] = itemLine.ShortName;
                                                        drRiskWBSi[DT_RISK_WBS.WBS] = itemAncestors[DT_WBS.WBS_NAME].ToString().TrimStart();
                                                        drRiskWBSi[DT_RISK_WBS.ID_WBS] = itemAncestors[DT_WBS.ID_WBS];
                                                        drRiskWBSi[DT_RISK_WBS.NIVEL] = itemWBS[DT_WBS.NIVEL].ToString().TrimStart();
                                                        drRiskWBSi[DT_RISK_WBS.USERNAME] = itemAncestors[DT_WBS.USERNAME];
                                                        drRiskWBSi[DT_RISK_WBS.WBS_USER] = itemAncestors[DT_WBS.WBS_NAME] + "[" + itemAncestors[DT_WBS.USERNAME] + "]";
                                                        drRiskWBSi[DT_RISK_WBS.IS_PRIMARY] = false;
                                                        drRiskWBSi[DT_RISK_WBS.PROBABILITY] = 100;
                                                        TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Add(drRiskWBSi);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    foreach (DataRow itemWBS in TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                                    {
                                        if (WBSOperations.IsRiskWBSLow(itemWBS, DsWBS, TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME]))
                                        {
                                            WBSOperations.TabAddWBS_LINE_Damage(itemWBS, (decimal)RiskRow[DT_Risk.ID], false, TheCurrentLayout.Ds);
                                        }
                                        else
                                        {//TENGO QUE BORRAR EL DAMAGE_WBS_RISK, PUES YA NO ES LOWLEVEL
                                            WBSOperations.TabDeleteWBS_LINE_Damage(itemWBS, (decimal)RiskRow[DT_Risk.ID], false, TheCurrentLayout.Ds);
                                        }
                                    }
                                    CalculateProbability(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                                    RefreshRiskDamageValues(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _DeleteWBSRiskCommand;
        public RelayyCommand DeleteWBSRiskCommand { get { return _DeleteWBSRiskCommand; } set { _DeleteWBSRiskCommand = value; OnPropertyChanged("DeleteWBSRiskCommand"); } }
        private void ImplementDeleteWBSRiskCommand()
        {
            try
            {
                DeleteWBSRiskCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (parametro is int rowSelectedIndex)
                        {
                            Selected_Risk_WBS_Row = DvRiskWBS[rowSelectedIndex].Row;
                            IS_DELETING_Risk_WBS_ROW = true;
                            MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " [" + Selected_Risk_WBS_Row[DT_RISK_WBS.WBS].ToString() + "]?");
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _ChangePrimaryWBSRiskCommand;
        public RelayyCommand ChangePrimaryWBSRiskCommand { get { return _ChangePrimaryWBSRiskCommand; } set { _ChangePrimaryWBSRiskCommand = value; OnPropertyChanged("ChangePrimaryWBSRiskCommand"); } }
        private void ImplementChangePrimaryWBSRiskCommand()
        {
            try
            {
                ChangePrimaryWBSRiskCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (parametro is int rowSelectedIndex)
                        {
                            decimal idWBS = (decimal)DvRiskWBS[rowSelectedIndex].Row[DT_RISK_WBS.ID_WBS];
                            DvRiskWBS[wrapRisk.DgRiskWBS.SelectedIndex].Row[DT_RISK_WBS.IS_PRIMARY] = true;
                            if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                            {
                                foreach (DataRow item in DvRiskWBS.Table.Select(DT_RISK_WBS.ID_RISK + " = " + TheCurrentLayout.Line_Selected.ID + " and " + DT_RISK_WBS.ID_WBS + " <> " + idWBS))
                                {
                                    item[DT_RISK_WBS.IS_PRIMARY] = false;
                                }
                                foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + TheCurrentLayout.Line_Selected.ID))
                                {
                                    item[DT_Risk_Damages.ID_WBS] = DvRiskWBS[wrapRisk.DgRiskWBS.SelectedIndex].Row[DT_RISK_WBS.ID_WBS];
                                    item[DT_Risk_Damages.WBS_NAME] = DvRiskWBS[wrapRisk.DgRiskWBS.SelectedIndex].Row[DT_RISK_WBS.WBS];
                                    item[DT_Risk_Damages.USERNAME] = DvRiskWBS[wrapRisk.DgRiskWBS.SelectedIndex].Row[DT_RISK_WBS.USERNAME];
                                }
                                TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Merge(DvRiskWBS.Table);
                                TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_Risk.ID_WBS] = DvRiskWBS[wrapRisk.DgRiskWBS.SelectedIndex].Row[DT_RISK_WBS.ID_WBS];
                                CrossRiskRightTab(TheCurrentLayout.Ds);
                                CrossCMRightTab(TheCurrentLayout.Ds);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
        #endregion

        #region RISK_ROLE_TABLE_COMMANDS
        private RelayyCommand _AddTabRiskRoleCommand;
        public RelayyCommand AddTabRiskRoleCommand { get { return _AddTabRiskRoleCommand; } set { _AddTabRiskRoleCommand = value; OnPropertyChanged("AddTabRiskRoleCommand"); } }
        private void ImplementAddTabRiskRoleCommand()
        {
            try
            {
                AddTabRiskRoleCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                        {
                            DataRow RiskRow = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID);
                            using (ServiceRoleController.WebServiceRole ws = new ServiceRoleController.WebServiceRole())
                            {
                                DataTable roleCodif = ws.GetRolesData().Tables[DT_Role.ROLE_TABLE].Copy();
                                WindowSelection frmSelection = new WindowSelection();
                                if (TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLE_NAME].Select(DT_Role_Risk.ID_RISK + " = " + (decimal)RiskRow[DT_Risk.ID]).Count() > 0)
                                {
                                    frmSelection.Dt = General.DeleteExists(roleCodif, TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLE_NAME].Select(DT_Role_Risk.ID_RISK + " = " + (decimal)RiskRow[DT_Risk.ID]).CopyToDataTable(), DT_Role.IDROL_COLUMN);
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
                                        DataRow drRole = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLE_NAME].NewRow();
                                        drRole[DT_Role_Risk.ID_RISK] = RiskRow[DT_Risk.ID];
                                        drRole[DT_Role_Risk.NAME_SHORT] = wrapRisk.TextRisk.InputText;
                                        drRole[DT_Role_Risk.Role] = itemRole[DT_Role.ROLE_COLUM];
                                        drRole[DT_Role_Risk.IDROL_COLUMN] = itemRole[DT_Role.IDROL_COLUMN];
                                        TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLE_NAME].Rows.Add(drRole);
                                        foreach (var item in TreeOperation.GetOnlyMyChildrenWithCM(TheCurrentLayout.Line_Selected))
                                        {
                                            if (!(TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLE_NAME].Rows.Contains(new object[] { item.ID, itemRole[DT_Role.IDROL_COLUMN] })))
                                            {
                                                DataRow drRoleRisk = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLE_NAME].NewRow();
                                                drRoleRisk[DT_Role_Risk.ID_RISK] = item.ID;
                                                drRoleRisk[DT_Role_Risk.NAME_SHORT] = item.ShortName;
                                                drRoleRisk[DT_Role_Risk.Role] = itemRole[DT_Role.ROLE_COLUM];
                                                drRoleRisk[DT_Role_Risk.IDROL_COLUMN] = itemRole[DT_Role.IDROL_COLUMN];
                                                TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLE_NAME].Rows.Add(drRoleRisk);
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
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _DeleteTabRoleRiskCommand;
        public RelayyCommand DeleteTabRoleRiskCommand { get { return _DeleteTabRoleRiskCommand; } set { _DeleteTabRoleRiskCommand = value; OnPropertyChanged("DeleteTabRoleRiskCommand"); } }
        private void ImplementDeleteTabRoleRiskCommand()
        {
            try
            {
                DeleteTabRoleRiskCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (parametro is int rowSelectedIndex)
                        {
                            if (rowSelectedIndex >= 0)
                            {
                                Selected_RiskRoleRow = DvRoleRisk[rowSelectedIndex].Row;
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
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
        #endregion
        #endregion

        #region GROUP_TAB_COMMANDS

        public RelayyCommand AddRiskToGroupCommand { get; set; }
        public virtual void AddRiskToGroupCommandFunction()
        {
            try
            {
                AddRiskToGroupCommand = new RelayyCommand(
                    parametro =>
                    {
                        try
                        {
                            if (parametro is decimal idGroup)
                            {
                                TheCurrentLayout.SelectingToGroup = true;
                                TheCurrentLayout.GroupSelected = new LineGroup()
                                {
                                    IdGroup = idGroup,
                                    GroupName = TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(idGroup)[DT_Groupe.GROUPE_NAME].ToString()
                                };
                            }
                        }
                        catch (Exception ex)
                        {
                            MostrarErrorDialog(ex.Message);
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public RelayyCommand RemoveGroupCommand { get; set; }
        public virtual void RemoveGroupCommandFunction()
        {
            try
            {
                RemoveGroupCommand = new RelayyCommand(
                    parametro =>
                    {
                        try
                        {
                            if (parametro is decimal idGroup)
                            {
                                ID_Groupe = idGroup;
                                IS_CLEANING_GROUP = true;
                                MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " this Group?");
                            }
                        }
                        catch (Exception ex)
                        {
                            MostrarErrorDialog(ex.Message);
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        /// <summary>
        /// Remove the group from Database, not just the filter
        /// </summary>
        public RelayyCommand RemoveFilterGroupCommand { get; set; }
        public virtual void RemoveFilterGroupCommandFunction()
        {
            try
            {
                RemoveFilterGroupCommand = new RelayyCommand(
                    parametro =>
                    {
                        try
                        {
                            if (parametro is decimal idGroup)
                            {
                                ID_Groupe = idGroup;
                                IS_REMOVING_GROUP_FROM_DATABASE = true;
                                MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " this Group?");
                            }
                        }
                        catch (Exception ex)
                        {
                            MostrarErrorDialog(ex.Message);
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public RelayyCommand RenameGroupCommand { get; set; }
        public virtual void RenameGroupCommandFunction()
        {
            try
            {
                RenameGroupCommand = new RelayyCommand(
                    parametro =>
                    {
                        try
                        {
                            if (parametro is decimal idGroup)
                            {
                                WindowRenameGroup wgrp = new WindowRenameGroup()
                                {
                                    DrGroup = TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(idGroup)
                                };
                                if (wgrp.ShowDialog() == true)
                                {
                                    TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(idGroup)[DT_Groupe.GROUPE_NAME] = wgrp.DrGroup[DT_Groupe.GROUPE_NAME];
                                    foreach (var line in TheCurrentLayout.LinesList)
                                    {
                                        if (line.Group.IdGroup == idGroup)
                                        {
                                            line.Group.GroupName = wgrp.DrGroup[DT_Groupe.GROUPE_NAME].ToString();
                                        }
                                    }
                                    UpdateGroupTab(TheCurrentLayout.Ds);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MostrarErrorDialog(ex.Message);
                        }
                    }
                    );
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public RelayyCommand EnableDisableGroupCommand { get; set; }
        public virtual void EnableDisableGroupCommanddFunction()
        {
            try
            {
                EnableDisableGroupCommand = new RelayyCommand(
                    _ =>
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

                            foreach (RiskPolyLine rpl in TheCurrentLayout.RiskGroupSelected)
                            {
                                TheCurrentLayout.EnableRisk(rpl, true, estadoActual);
                                //rpl.Stroke = new SolidColorBrush(Colors.LightSkyBlue);
                                rpl.SetColor(new SolidColorBrush(System.Windows.Media.Colors.LightSkyBlue));
                            }
                            foreach (var cmline in TheCurrentLayout.CMGroupSelected)
                            {
                                TheCurrentLayout.DisableCounterMeasure(cmline, true, estadoActual);
                                //cmline.Stroke = new SolidColorBrush(Colors.LightSkyBlue);
                                cmline.SetColor(new SolidColorBrush(System.Windows.Media.Colors.LightSkyBlue));
                            }

                            if (!estadoActual)
                            {
                                GhostWindow win = new GhostWindow("Enabled");
                                win.ShowDialog();
                            }
                            else
                            {
                                GhostWindow win = new GhostWindow("Disabled");
                                win.ShowDialog();
                            }

                            OnProjectChange();
                            UpdateGroupTab(TheCurrentLayout.Ds);

                        }
                        catch (Exception ex)
                        {
                            MostrarErrorDialog(ex.Message);
                        }
                    }
                 );
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _treviewItemGotFocusCommand;
        public RelayyCommand TreviewItemGotFocusCommand { get { return _treviewItemGotFocusCommand; } set { _treviewItemGotFocusCommand = value; OnPropertyChanged("TreviewItemGotFocusCommand"); } }
        private void ImplementTreviewItemGotFocusCommand()
        {
            try
            {
                TreviewItemGotFocusCommand = new RelayyCommand(
                o =>
                {
                    try
                    {
                        if (o is MyExpanderGroupItemm selectedTreeViewItem)
                        {

                            TheCurrentLayout.ResetGroupRiksSelection();
                            TheCurrentLayout.ResetGroupCMSelection();
                            DataView dv = (DataView)(selectedTreeViewItem.MyDataGrid).ItemsSource;
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
                                    RiskPolyLine CurrentRiskPolyLine = TheCurrentLayout.LinesList.Find(rpl => rpl.ID == (decimal)elementDataRow["ID"]);
                                    CurrentRiskPolyLine.SetColor(new SolidColorBrush(Colors.LightSkyBlue));
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
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public RelayyCommand DeleteRiskFromGroupCommand { get; set; }
        public virtual void DeleteRiskFromGroupCommandFunction()
        {
            try
            {
                DeleteRiskFromGroupCommand = new RelayyCommand(
                    parametro =>
                    {
                        try
                        {
                            if (parametro is decimal idRisk)
                            {
                                IdRiskToDeleteFromGroup = idRisk;
                                IS_DELETING_RISK_FROM_GROUP_TAB = true;
                                MostrarDialogYesNo(StringResources.DELETE_MESSAGE + " this value?");
                            }
                        }
                        catch (Exception ex)
                        {
                            MostrarErrorDialog(ex.Message);
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        #endregion

        #region DIAGRAM_LIST_TAB
        private RelayyCommand _RefreshButtonCommand;
        public RelayyCommand RefreshButtonCommand { get { return _RefreshButtonCommand; } set { _RefreshButtonCommand = value; OnPropertyChanged("RefreshButtonCommand"); } }
        private void ImplementRefreshButtonCommand()
        {
            try
            {
                RefreshButtonCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        DsMain = new UserDataSet();
                        using (ServiceRiskController.WebServiceRisk risk = new ServiceRiskController.WebServiceRisk())
                        {
                            DsMain.Merge(risk.GetRiskTreeString(new object[] { "%", IdProject }));
                        }

                        DVRisk_Tree = DsMain.Tables[DT_Diagram.TABLE_NAME].DefaultView;
                        //gridDiagramList.dgTreeDiagrams.SelectedIndex = 0;
                        gridDiagramList.dgTreeDiagrams.SelectedIndex = 0;
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _AddDiagramCommand;
        public RelayyCommand AddDiagramCommand { get { return _AddDiagramCommand; } set { _AddDiagramCommand = value; OnPropertyChanged("AddDiagramCommand"); } }
        private void ImplementAddDiagramCommand()
        {
            try
            {
                AddDiagramCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        MyLayoutDocumentt myly = new MyLayoutDocumentt
                        {
                            SaveAsClosing = true,
                            MenuRisk = MenuRisk,
                            MenuMainRisk = MenuMainRisk,
                            //MenuRiskLimited = MenuRisk,
                            Ds = DsMain.Copy(),
                            MenuCM = MenuCM,
                            LoginUser = LoginUser,
                            MyWindow = this,
                            MenuGroupCM = MenuGroupCM,
                            MenuGroupRisk = MenuGroupRisk,
                            MenuGroupMixed = MenuGroupMixed,
                            IsPanEnable = isPanEnabled,
                            IdWBSFilter = IdWBSFilterSelected
                        };
                        WindowTreeRisk riskTree = new WindowTreeRisk
                        {
                            Operation = General.INSERT,
                            TopRiskTable = myly.Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Copy(),
                            DRow = myly.Ds.Tables[DT_Diagram.TABLE_NAME].NewRow(),
                            Icon = Icon,
                            IDProject = IdProject
                        };
                        if (riskTree.ShowDialog() == true)
                        {
                            myly.DrDiagram = riskTree.DRow;
                            myly.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Add(myly.DrDiagram);
                            myly.Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Merge(riskTree.TopRiskTable);

                            DataRow drRisk = myly.Ds.Tables[DT_Risk.TABLE_NAME].NewRow();

                            drRisk[DT_Risk.NAMESHORT] = "Main Risk";
                            drRisk[DT_Risk.COMMENTS] = "Main Risk " + riskTree.DRow[DT_Diagram.DIAGRAM_NAME];
                            drRisk[DT_Risk.IS_ROOT] = true;
                            drRisk[DT_Risk.IS_CM] = false;
                            drRisk[DT_Risk.ISCOLLAPSED] = false;
                            drRisk[DT_Risk.IS_ACTIVE] = true;
                            drRisk[DT_Risk.PROBABILITY] = 100;
                            drRisk[DT_Risk.ID_DIAGRAM] = riskTree.DRow[DT_Diagram.ID_DIAGRAM];
                            myly.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);

                            DataRow newRow = myly.Ds.Tables[DT_Role_Risk.TABLE_NAME].NewRow();
                            newRow[DT_Role_Risk.ID_RISK] = drRisk[DT_Risk.ID];
                            newRow[DT_Role_Risk.Role] = myly.Ds.Tables[DT_Role.ROLE_TABLE].Select(DT_Role.ROLE_COLUM + " = 'Administrator'").First()[DT_Role.ROLE_COLUM];
                            newRow[DT_Role_Risk.IDROL_COLUMN] = myly.Ds.Tables[DT_Role.ROLE_TABLE].Select(DT_Role.ROLE_COLUM + " = 'Administrator'").First()[DT_Role.IDROL_COLUMN];
                            myly.Ds.Tables[DT_Role_Risk.TABLE_NAME].Rows.Add(newRow);
                            System.Windows.Media.Color color = ((SolidColorBrush)new BrushConverter().ConvertFrom(myly.Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + riskTree.DRow[DT_Diagram.ID_DIAGRAM]).First()[DT_Diagram_Damages.COLOR].ToString())).Color;
                            myly.Title = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                            if (LayoutDocPaneGroup.Children.Count > 0)
                            {
                                ((LayoutDocumentPane)LayoutDocPaneGroup.Children[0]).Children.Add(myly);
                            }
                            OpenedDocuments.Add(myly);
                            TheCurrentLayout = myly;
                            TheCurrentLayout.MoviendoRisk = false;
                            TheCurrentLayout.ID_Diagram = (decimal)riskTree.DRow[DT_Diagram.ID_DIAGRAM];
                            TheCurrentLayout.AddMainLine(drRisk, color);
                            TheCurrentLayout.LoadComboDamage();
                            TheCurrentLayout.LoadRectangles();
                            TheCurrentLayout.DrawNumbers();
                            TheCurrentLayout.Title = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                            TheCurrentLayout.FixDrawPanel();
                            foreach (var item in OpenedDocuments)
                            {
                                item.ExitWorking();
                            }
                            TheCurrentLayout.EnterWorking();
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _EditDiagramCommand;
        public RelayyCommand DiagramListDoubleClickCommand { get { return _EditDiagramCommand; } set { _EditDiagramCommand = value; OnPropertyChanged("DiagramListDoubleClickCommand"); } }
        private void ImplementDiagramListDoubleClickCommand()
        {
            try
            {
                DiagramListDoubleClickCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (ExistRole(4))
                        {
                            if (gridDiagramList.dgTreeDiagrams.SelectedIndex >= 0)
                            {
                                OpenDiagramFromDiagramList((decimal)DVRisk_Tree[gridDiagramList.dgTreeDiagrams.SelectedIndex].Row[DT_Diagram.ID_DIAGRAM]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        IsImporting = false;
                        TheProgress.Visibility = Visibility.Hidden;
                        HabilitarBotones(true);
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                IsImporting = false;
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _DeleteDiagramCommand;
        public RelayyCommand DeleteDiagramCommand { get { return _DeleteDiagramCommand; } set { _DeleteDiagramCommand = value; OnPropertyChanged("DeleteDiagramCommand"); } }
        private void ImplementDeleteDiagramCommand()
        {
            try
            {
                DeleteDiagramCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        if (gridDiagramList.dgTreeDiagrams.SelectedIndex >= 0)
                        {
                            MostrarDialogYesNo($"{StringResources.DELETE_MESSAGE} the diagram [{ DVRisk_Tree[gridDiagramList.dgTreeDiagrams.SelectedIndex].Row[DT_Diagram.DIAGRAM_NAME] }]?");
                            IS_DELETING_DIAGRAM = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
        #region ImportFromExcel
        private RelayyCommand _ImportExcelDiagramCommand;
        public RelayyCommand ImportFromExcelDiagramCommand { get { return _ImportExcelDiagramCommand; } set { _ImportExcelDiagramCommand = value; OnPropertyChanged("ImportFromExcelDiagramCommand"); } }
        private void ImplementImportFromExcelDiagramCommand()
        {
            try
            {
                ImportFromExcelDiagramCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        using (System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog() { Filter = "Excel WorkBook|*.xlsx|Excel WorkBook 97-2003|*.xls", ValidateNames = true })
                        {
                            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                            {
                                ImportFromExcel(ofd.FileName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        IsImporting = false;
                        MostrarErrorDialog(ex.Message);
                        TheProgress.Visibility = Visibility.Hidden;
                        HabilitarBotones(true);
                    }
                });
            }
            catch (Exception ex)
            {
                IsImporting = false;
                MostrarErrorDialog(ex.Message);
                TheProgress.Visibility = Visibility.Hidden;
                HabilitarBotones(true);
            }
        }

        private void ImportToExcelWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                RefreshData();
                IsImporting = false;
                MostrarInfoDialog("Importing finished!!");
                HabilitarBotones(true);
            }
            catch (Exception ex)
            {
                IsImporting = false;
                HabilitarBotones(true);
                MostrarErrorDialog(ex.Message);
            }
        }

        private void ImportToExcelWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ImportFromExcel importRiskTree = (ImportFromExcel)e.Argument;
                importRiskTree.ImportExcel(sender as BackgroundWorker, e);
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(() =>
                {
                    IsImporting = false;
                    HabilitarBotones(true);
                    MostrarErrorDialog(ex.Message);
                });
            }
        }

        private void ImportFromExcel(string filename)
        {
            WindowText wt = new WindowText();
            wt.txtKeyword.Focus();
            if (wt.ShowDialog() == true)
            {
                UserDataSet dsImporting = new UserDataSet();
                DataRow drDiagram = dsImporting.Tables[DT_Diagram.TABLE_NAME].NewRow();
                using (DataTable dtExcel = ExcelController.ExcelToDataTable(filename))
                {
                    WindowHeaderClasification whc = ExcelController.SelectExcelColumns(dtExcel);
                    if (whc.ShowDialog() == true)
                    {

                        bool isMarkedAll = wt.IsMarkedAll;
                        Cursor = Cursors.No;
                        HabilitarBotones(false);
                        IsImporting = true;
                        using (ImportFromExcel import = new ImportFromExcel(dsImporting, DsMain, IdProject, drDiagram, DsWBS, isMarkedAll, whc.IsCustom, wt.KeyWord, dtExcel, whc.MyList))
                        {
                            importToExcelWorker.RunWorkerAsync(import);
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Cross_Risk_List
        private RelayyCommand _FilterRiskCommand;
        public RelayyCommand FilterRiskCommand { get { return _FilterRiskCommand; } set { _FilterRiskCommand = value; OnPropertyChanged("FilterRiskCommand"); } }
        private void ImplementFilterRiskCommand()
        {
            FilterRiskCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        DV_CrossRisk.RowFilter = DT_Risk_Damages.RISK_NAMESHORT + " like '%" + TextFilterRisk + "%'";
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _ClearFilterRiskCommand;
        public RelayyCommand ClearFilterRiskCommand { get { return _ClearFilterRiskCommand; } set { _ClearFilterRiskCommand = value; OnPropertyChanged("ClearFilterRiskCommand"); } }
        private void ImplementClearFilterRiskCommand()
        {
            ClearFilterRiskCommand = new RelayyCommand(
               _ =>
               {
                   try
                   {
                       TextFilterRisk = string.Empty;
                   }
                   catch (Exception ex)
                   {
                       MostrarErrorDialog(ex.Message);
                   }
               });
        }

        private RelayyCommand _DgRiskMouseLeaveCommand;
        public RelayyCommand DgRiskMouseLeaveCommand { get { return _DgRiskMouseLeaveCommand; } set { _DgRiskMouseLeaveCommand = value; OnPropertyChanged("DgRiskMouseLeaveCommand"); } }
        private void ImplementDgRiskMouseLeaveCommand()
        {
            DgRiskMouseLeaveCommand = new RelayyCommand(
                 _ =>
                 {
                     try
                     {
                         if (TheCurrentLayout != null)
                         {
                             TheCurrentLayout.LineLeave(TheCurrentLayout.Line_Selected);
                         }
                     }
                     catch (Exception ex)
                     {
                         MostrarErrorDialog(ex.Message);
                     }
                 });
        }

        private RelayyCommand _DgRiskSelectionChangedCommand;
        public RelayyCommand DgRiskSelectionChangedCommand { get { return _DgRiskSelectionChangedCommand; } set { _DgRiskSelectionChangedCommand = value; OnPropertyChanged("DgRiskSelectionChangedCommand"); } }
        private void ImplementDgRiskSelectionChangedCommand()
        {
            DgRiskSelectionChangedCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (parametro is DataGrid riskDataGrid && riskDataGrid.SelectedItem != null)
                        {
                            if (riskDataGrid.SelectedItem is DataRowView thedataRowView)
                            {
                                if (thedataRowView.Row[DT_Risk.ID] is decimal rowRiskID)
                                {
                                    TheCurrentLayout.Line_Selected = TheCurrentLayout.LinesList.Find(item => (item.ID == rowRiskID && !item.IsCM));
                                    TheCurrentLayout.LineLeave(TheCurrentLayout.Line_Selected);
                                    TheCurrentLayout.BrigIntoViewSelectedRiskPolyline(TheCurrentLayout.Line_Selected);
                                    TheCurrentLayout.RiskEnter(TheCurrentLayout.Line_Selected, TheCurrentLayout.Line_Selected.Points[TheCurrentLayout.Line_Selected.Points.Count - 1], false);
                                    TheCurrentLayout.UpdateSelectedPolyLineVisualInfo();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _ActivateRiskCommand;
        public RelayyCommand ActivateRiskCommand { get { return _ActivateRiskCommand; } set { _ActivateRiskCommand = value; OnPropertyChanged("ActivateRiskCommand"); } }
        private void ImplementActivateRiskCommand()
        {
            try
            {
                ActivateRiskCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (parametro is decimal rowRiskID)
                        {
                            bool result = TheCurrentLayout.EnableRisk(TheCurrentLayout.LinesList.Find(x => x.ID == rowRiskID), false, false);
                            OnProjectChange();
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }                    
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _RiskNameTextLostFocusCommand;
        public RelayyCommand RiskNameTextLostFocusCommand { get { return _RiskNameTextLostFocusCommand; } set { _RiskNameTextLostFocusCommand = value; OnPropertyChanged("RiskNameTextLostFocusCommand"); } }
        private void ImplementRiskNameTextLostFocusCommand()
        {
            RiskNameTextLostFocusCommand = new RelayyCommand(
               parametro =>
               {
                   try
                   {
                       if (parametro is TextBox theTextBox)
                       {
                           if (!string.IsNullOrWhiteSpace(theTextBox.Text))
                           {
                               if (theTextBox.DataContext != null && theTextBox.DataContext is DataRowView theDataView)
                               {
                                   bool? canUsePolyLineName = CanUseProposedPolyLineName(theTextBox.Text.TrimStart());
                                   decimal idRisk = (decimal)theDataView.Row[DT_Risk_Damages.ID_RISK];
                                   if (canUsePolyLineName.HasValue)
                                   {
                                       if (canUsePolyLineName.HasValue && !canUsePolyLineName.Value)
                                       {
                                           theTextBox.Text = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(idRisk)[DT_Risk.NAMESHORT].ToString();
                                       }
                                       else
                                       {
                                           ChangeRiskNameInCrossTable(theTextBox, idRisk);
                                       }
                                   }
                                   else
                                   {
                                       ChangeRiskNameInCrossTable(theTextBox, idRisk);
                                   }
                               }
                           }
                           else
                           {
                               CrossRiskRightTab(TheCurrentLayout.Ds);
                               CrossCMRightTab(TheCurrentLayout.Ds);
                               MostrarErrorDialog("Risk Name Value can´t be empty or white space. No changes apply");
                           }
                       }
                   }
                   catch (Exception ex)
                   {
                       MostrarErrorDialog(ex.Message);
                   }
               });
        }

        private RelayyCommand _RiskProbabilityLostFocusCommand;
        public RelayyCommand RiskProbabilityLostFocusCommand { get { return _RiskProbabilityLostFocusCommand; } set { _RiskProbabilityLostFocusCommand = value; OnPropertyChanged("RiskProbabilityLostFocusCommand"); } }
        private void ImplementRiskProbabilityLostFocusCommand()
        {
            RiskProbabilityLostFocusCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (parametro is decimal idRisk)
                        {
                            string newProbability;
                            if (DV_CrossRisk.Table.Select(DT_Risk_Damages.ID_RISK + " = " + idRisk).Any())
                            {
                                DataRow drRiskDamageSelected = DV_CrossRisk.Table.Select(DT_Risk_Damages.ID_RISK + " = " + idRisk).First();
                                newProbability = DV_CrossRisk.Table.Select(DT_Risk_Damages.ID_RISK + " = " + idRisk).First()[DT_Risk_Damages.PROBABILITY].ToString();
                                if (string.IsNullOrWhiteSpace(newProbability) || !General.IsNumeric(newProbability))
                                {
                                    probability = 0;
                                }
                                else
                                {
                                    probability = Convert.ToDecimal(newProbability);
                                }
                                if (TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains(idRisk))
                                {
                                    if (IdWBSFilterSelected != -1)
                                    {
                                        if (TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { idRisk, IdWBSFilterSelected }))
                                        {
                                            TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows
                                                .Find(new object[] { idRisk, IdWBSFilterSelected })[DT_RISK_WBS.PROBABILITY] = probability;
                                        }
                                        CalculateProbability(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(idRisk));
                                        TreeOperation.SetRiskLineValues(TheCurrentLayout.Line_Selected, TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(idRisk));
                                        TheCurrentLayout.ClearFilters();

                                        TheCurrentLayout.DrawNumbers();
                                        TheCurrentLayout.UpdateLinesValues();
                                        TheCurrentLayout.SetLinesThickness();

                                        CrossRiskRightTab(TheCurrentLayout.Ds);
                                        CrossCMRightTab(TheCurrentLayout.Ds);
                                        TheCurrentLayout.UpdateSelectedPolyLineVisualInfo();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }
        #endregion

        #region Cross_CM_List
        private RelayyCommand _FilterCMCommand;
        public RelayyCommand FilterCMCommand { get { return _FilterCMCommand; } set { _FilterCMCommand = value; OnPropertyChanged("FilterCMCommand"); } }
        private void ImplementFilterCMCommand()
        {
            FilterCMCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        DV_Cross_CM.RowFilter = DT_Risk_Damages.RISK_NAMESHORT + " like '%" + TextFilterCM + "%'";
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _ClearFilterCMCommand;
        public RelayyCommand ClearFilterCMCommand { get { return _ClearFilterCMCommand; } set { _ClearFilterCMCommand = value; OnPropertyChanged("ClearFilterCMCommand"); } }
        private void ImplementClearFilterCMCommand()
        {
            ClearFilterCMCommand = new RelayyCommand(
               _ =>
               {
                   try
                   {
                       TextFilterCM = string.Empty;
                   }
                   catch (Exception ex)
                   {
                       MostrarErrorDialog(ex.Message);
                   }
               });
        }

        private RelayyCommand _ActivateCMCommand;
        public RelayyCommand ActivateCMCommand { get { return _ActivateCMCommand; } set { _ActivateCMCommand = value; OnPropertyChanged("ActivateCMCommand"); } }
        private void ImplementActivateCMCommand()
        {
            try
            {
                ActivateCMCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (parametro is decimal rowRiskID)
                        {
                            bool result = TheCurrentLayout.DisableCounterMeasure(TheCurrentLayout.LinesList.Find(x => x.ID == rowRiskID));
                            OnProjectChange();
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private RelayyCommand _DgCMSelectionChangedCommand;
        public RelayyCommand DgCMSelectionChangedCommand { get { return _DgCMSelectionChangedCommand; } set { _DgCMSelectionChangedCommand = value; OnPropertyChanged("DgCMSelectionChangedCommand"); } }
        private void ImplementDgCMSelectionChangedCommand()
        {
            DgCMSelectionChangedCommand = new RelayyCommand(
                parametro =>
                {
                    try
                    {
                        if (parametro is DataGrid riskDataGrid && riskDataGrid.SelectedItem != null)
                        {
                            if (riskDataGrid.SelectedItem is DataRowView thedataRowView)
                            {
                                if (thedataRowView.Row[DT_Risk_Damages.ID_RISK] is decimal rowRiskID)
                                {
                                    TheCurrentLayout.Line_Selected = TheCurrentLayout.LinesList.Find(item => (item.ID == rowRiskID && item.IsCM));
                                    TheCurrentLayout.LineLeave(TheCurrentLayout.Line_Selected);
                                    TheCurrentLayout.BrigIntoViewSelectedRiskPolyline(TheCurrentLayout.Line_Selected);
                                    TheCurrentLayout.CMEnter(TheCurrentLayout.Line_Selected, TheCurrentLayout.Line_Selected.Points[TheCurrentLayout.Line_Selected.Points.Count - 1], false);
                                    TheCurrentLayout.UpdateSelectedPolyLineVisualInfo();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

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
                    if (TableColors.Rows.Count > 1)
                    {
                        string theme = TableColors.Rows[0]["Theme"].ToString();
                        string color = TableColors.Rows[0]["Color"].ToString();
                        Application.Current.Resources.MergedDictionaries[2].Source = new Uri("pack://application:,,,/MaterialDesignColors;component/Themes/Recommended/Primary/MaterialDesignColor." + color + ".xaml", UriKind.RelativeOrAbsolute);
                        Application.Current.Resources.MergedDictionaries[0].Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme." + theme + ".xaml", UriKind.RelativeOrAbsolute);
                    }
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
                
                MenuRisk.DataContext = this;
                MenuMainRisk.DataContext = this;
                MenuCM.DataContext = this;
                MenuGroupCM.DataContext = this;
                MenuGroupMixed.DataContext = this;
                MenuGroupRisk.DataContext = this;
                //gridDiagramList.DataContext = this;
                wrapRisk.headerProb.DataContext = this;
                wrapRisk.BtnAddWBSRisk.DataContext = this;
                wrapRisk.BtnAddRiskRole.DataContext = this;
                //GridRisksCross.DataContext = this;
                IsImporting = false;
                IsCalculatingRisk = false;
                IsCalculatingCM = false;
                WSRisk = new ServiceRiskController.WebServiceRisk();
                DsMain = new UserDataSet();
                AccessList = new List<decimal>();
                OpenedDocuments = new List<MyLayoutDocumentt>();
                //Binding myBinding = new Binding
                //{
                //    Source = CbFilterWBSRisk,
                //    Path = new PropertyPath("SelectedIndex"),
                //    Mode = BindingMode.TwoWay,
                //    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
                //};
                //BindingOperations.SetBinding(CbFilterWBSCM, ComboBox.SelectedIndexProperty, myBinding);
                importToExcelWorker.WorkerReportsProgress = true;
                importToExcelWorker.WorkerSupportsCancellation = true;
                importToExcelWorker.DoWork += ImportToExcelWorker_DoWork; ;
                importToExcelWorker.RunWorkerCompleted += ImportToExcelWorker_RunWorkerCompleted;
               
                yaCargoTodo = true;
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //HabilitarBotones(false);

                UpdateServerAccess();
                using (ServiceProject.WebServiceProject webService = new ServiceProject.WebServiceProject())
                {
                    DvCBProjects = new DataView(webService.GetAllProjects().Tables[DT_Project.TABLE_NAME]);
                }
                
                DsMain = new UserDataSet();
                using (ServiceRiskController.WebServiceRisk risk = new ServiceRiskController.WebServiceRisk())
                {
                    DsMain.Merge(risk.GetRiskTreeString(new object[] { "%", -9999 }));
                }
                
                HabilitarBotones(true);
                //UPDATE THE TABLE OF TREES AND ITS RESPECTIVE RISK OF EACH ONE
                //DV ITS THE DATAVIEW OF TREE RISK
                DVRisk_Tree = DsMain.Tables[DT_Diagram.TABLE_NAME].DefaultView;
                gridDiagramList.dgTreeDiagrams.SelectedIndex = 0;
                ValidateAccess();
                MenuRisk.Visibility = Visibility.Visible;
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
                //CbProjects_DropDownClosed(sender, e);
                //TAB WBS
                ImplementAddWBSCommand(); ImplementCollapseWBSCommand(); ImplementUnSelectCommand();
                ImplementEyeCommand(); ImplementEditTreeWBSCommand(); ImplementDeleteTreeWBSCommand();
                //Group Tab
                AddRiskToGroupCommandFunction(); RemoveGroupCommandFunction(); RemoveFilterGroupCommandFunction();
                RenameGroupCommandFunction(); EnableDisableGroupCommanddFunction(); ImplementTreviewItemGotFocusCommand();
                DeleteRiskFromGroupCommandFunction();
                //properties tab
                ImplementRiskName_KeyUpCommand(); ImplementTextRisk_LostFocusCommand(); ImplementRiskName_TextChangedCommand();
                ImplementTextRiskDetail_LostFocusCommand(); ImplementDamageValueGotFocusCommand(); ImplementDamageValueLostFocusCommand(); ImplementChangePrimaryWBSRiskCommand();
                ImplementAddWBSRiskCommand(); ImplementDeleteWBSRiskCommand(); ImplementProbabilityLostFocusCommand();
                ImplementAddTabRiskRoleCommand(); ImplementDeleteTabRoleRiskCommand();

                //Diagram List Tab
                ImplementRefreshButtonCommand(); ImplementAddDiagramCommand(); ImplementDiagramListDoubleClickCommand();
                ImplementDeleteDiagramCommand(); ImplementImportFromExcelDiagramCommand();

                //Cross Risk List
                ImplementFilterRiskCommand();ImplementClearFilterRiskCommand();ImplementDgRiskMouseLeaveCommand();ImplementDgRiskSelectionChangedCommand(); ImplementActivateRiskCommand();
                ImplementRiskNameTextLostFocusCommand(); ImplementRiskProbabilityLostFocusCommand();

                //Cross CM List
                ImplementFilterCMCommand(); ImplementClearFilterCMCommand(); ImplementActivateCMCommand(); ImplementDgCMSelectionChangedCommand();

                //menu commands
                ImplementLogin_Command(); ImplementProject_Command(); ImplementUser_Command(); ImplementRole_Command(); ImplementDamageList_Command();
                ImplementDefaultRiskList_Command(); ImplementSaveDiagram_Command(); ImplementSaveAs_Command(); ImplementRiskCommand(); ImplementCMCommand();
                ImplementSettingsCommand(); ImplementLogsListCommand(); ImplementExporToImageCommand(); ImplementExporToExcelCommand();
                ImplementPanDragCommand(); ImplementShowWBSPannelCommand(); ImplementShowPropertiesPannelCommand(); ImplementPrintDiagramCommand();
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        public void MostrarErrorDialog(string text)
        {
            try
            {
                this.Dispatcher.Invoke(() =>
                {
                    ErrorDialogMessage.IsOpen = true;
                    ErrorContent.TextMessage.Text = text;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void MostrarInfoDialog(string text)
        {
            InformationMessageDialog.IsOpen = true;
            infoContent.TextInformationMessage.Text = text;
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
            //LayoutDocumentPanel.Children.Add(new LayoutDocument() { Title = "Document", IsClicked = true });
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
        
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
            //            System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_DAMAGE + " = " + (decimal)CurrentLayout.CbFilterTopR.SelectedValue).First()[DT_Risk_Damages.COLOR].ToString()));

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
            //                            Ds.Tables[DT_RiskStructure.RiskStructure_TABLA], (decimal)cbFilterTopR.SelectedValue, Ds.Tables[DT_Risk_Damages.TABLENAME],
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

            //            if (Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_DAMAGE + " = " + (decimal)CurrentLayout.CbFilterTopR.SelectedValue).Any())
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

        private void Salvando()
        {
            try
            {
                using (ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk())
                {
                    DataSet temp = TheCurrentLayout.Ds.GetChanges();
                    temp = ws.SaveRisk(temp);
                    TheCurrentLayout.Ds.Merge(temp);
                    TheCurrentLayout.Ds.AcceptChanges();
                }

                if (TheCurrentLayout.ID_Diagram < 0)
                {
                    TheCurrentLayout.ID_Diagram = (decimal)TheCurrentLayout.DrDiagram[DT_Diagram.ID_DIAGRAM];
                }
                RefreshData();
                TheCurrentLayout.Ds = DsMain.Copy();
                TheCurrentLayout.Ds.AcceptChanges();

                if (TheCurrentLayout.ID_Diagram > 0)
                {
                    //CurrentLayout.Ds = DsMain;
                    TheCurrentLayout.DropLines();
                    TheCurrentLayout.DropRectangles();
                    TheCurrentLayout.LoadLines();
                    TheCurrentLayout.LoadRectangles();
                    TheCurrentLayout.DrawNumbers();
                    TheCurrentLayout.SetLinesThickness();
                    ////TextProbabilityChange(TheCurrentLayout.MainLine);
                    CrossRiskRightTab(TheCurrentLayout.Ds);
                    CrossCMRightTab(TheCurrentLayout.Ds);
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
        /// <summary>
        /// Open and Save an opened diagram as new diagram. Not save the source value.
        /// </summary>
        private async void SaveAsAsyncFunction()
        {
            try
            {
                if (TheCurrentLayout != null)
                {
                    HabilitarBotones(false);
                    DataSet tempDS = new UserDataSet();
                    DataRow currentDiagram = TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(TheCurrentLayout.ID_Diagram);
                    DataRow drDiagram = tempDS.Tables[DT_Diagram.TABLE_NAME].NewRow();
                    drDiagram[DT_Diagram.ID_PROJECT] = currentDiagram[DT_Diagram.ID_PROJECT];
                    drDiagram[DT_Diagram.DIAGRAM_NAME] = currentDiagram[DT_Diagram.DIAGRAM_NAME] + " Copy";
                    tempDS.Tables[DT_Diagram.TABLE_NAME].Rows.Add(drDiagram);
                    foreach (var rowDamage in TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram))
                    {
                        DataRow drDamageDIag = tempDS.Tables[DT_Diagram_Damages.TABLE_NAME].NewRow();
                        drDamageDIag[DT_Diagram_Damages.COLOR] = rowDamage[DT_Diagram_Damages.COLOR];
                        drDamageDIag[DT_Diagram_Damages.DAMAGE] = rowDamage[DT_Diagram_Damages.DAMAGE];
                        drDamageDIag[DT_Diagram_Damages.ID_DAMAGE] = rowDamage[DT_Diagram_Damages.ID_DAMAGE];
                        drDamageDIag[DT_Diagram_Damages.ID_RISKTREE] = drDiagram[DT_Diagram.ID_DIAGRAM];
                        drDamageDIag[DT_Diagram_Damages.RISK_TREE] = rowDamage[DT_Diagram_Damages.RISK_TREE];
                        drDamageDIag[DT_Diagram_Damages.TOP_RISK] = rowDamage[DT_Diagram_Damages.TOP_RISK];
                        drDamageDIag[DT_Diagram_Damages.UM] = rowDamage[DT_Diagram_Damages.UM];
                        tempDS.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Add(drDamageDIag);
                    }
                    WindowTreeRisk riskTree = new WindowTreeRisk
                    {
                        Operation = General.UPDATE,
                        TopRiskTable = tempDS.Tables[DT_Diagram_Damages.TABLE_NAME].Copy(),
                        DRow = drDiagram,
                        Icon = Icon,
                        IDProject = IdProject,
                        Risk_TopRisk = tempDS.Tables[DT_Risk_Damages.TABLE_NAME].Copy()
                    };
                    riskTree.gridControls.IsEnabled = false;
                    if (riskTree.ShowDialog() == true)
                    {
                        await Task.Run(() =>
                        {
                            SavingAsController.CreateNewDiagram(TheCurrentLayout.Ds, tempDS, currentDiagram, drDiagram);
                            ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk();
                            DataSet temp = tempDS.GetChanges();
                            temp = ws.SaveRisk(temp);
                            tempDS.Merge(temp);
                            tempDS.AcceptChanges();
                            ws.Dispose();
                        });
                        this.Dispatcher.Invoke(() =>
                        {
                            RefreshData();
                            HabilitarBotones(true);
                            OpenDiagramFromDiagramList((decimal)drDiagram[DT_Diagram.ID_DIAGRAM]);
                        });
                    }
                    else
                    {
                        HabilitarBotones(true);
                    }
                    tempDS.Dispose();
                }
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(() =>
                {
                    HabilitarBotones(true);
                    MostrarErrorDialog(ex.Message);
                });
            }
        }

        #endregion

        #endregion

        #region Add,Edit,Delete,ImportExcel Diagram

        public void MostrarDialogYesNo(string textAlert)
        {
            YesNoDialog.IsOpen = true;
            yesNoContent.TextYesNoMessage.Text = textAlert;
        }

        private void OpenDiagramFromDiagramList(decimal DIAGRAM_ID)
        {
            MyLayoutDocumentt openDoc = OpenedDocuments.FirstOrDefault(docu => docu.ID_Diagram == DIAGRAM_ID);
            DiagramID = DIAGRAM_ID;

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
                    Ds = DsMain.Copy(),
                    MenuCM = MenuCM,
                    LoginUser = LoginUser,
                    MyWindow = this,
                    MenuGroupCM = MenuGroupCM,
                    MenuGroupRisk = MenuGroupRisk,
                    MenuGroupMixed = MenuGroupMixed,
                    IsPanEnable = isPanEnabled,
                    IdWBSFilter = IdWBSFilterSelected
                };
                WindowTreeRisk riskTree = new WindowTreeRisk
                {
                    Operation = General.UPDATE,
                    TopRiskTable = myly.Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Copy(),
                    DRow = myly.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(DIAGRAM_ID),
                    Icon = Icon,
                    IDProject = IdProject,
                    Risk_TopRisk = myly.Ds.Tables[DT_Risk_Damages.TABLE_NAME].Copy()
                };
                if (riskTree.ShowDialog() == true)
                {
                    DVRisk_Tree.Table.Rows.Find(DIAGRAM_ID)[DT_Diagram.DIAGRAM_NAME] = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                    myly.Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Merge(riskTree.TopRiskTable);
                    myly.Ds.Tables[DT_Risk_Damages.TABLE_NAME].Merge(riskTree.Risk_TopRisk);
                    myly.ID_Diagram = DiagramID;
                    SetNewDamageToEntireTree(myly.ID_Diagram, myly.Ds);
                    TheProgress.Visibility = Visibility.Visible;
                    HabilitarBotones(false);
                    myly.Title = riskTree.DRow[DT_Diagram.DIAGRAM_NAME].ToString();
                    if (LayoutDocPaneGroup.Children.Count > 0)
                    {
                        ((LayoutDocumentPane)LayoutDocPaneGroup.Children[0]).Children.Add(myly);
                    }
                    OpenedDocuments.Add(myly);
                    TheCurrentLayout = myly;
                    WBSOperations.AddWBSTopToDiagram(myly.Ds, DiagramID, DsWBS);
                    UpdateDiagram();
                    foreach (var item in OpenedDocuments)
                    {
                        item.ExitWorking();
                    }
                    TheCurrentLayout.EnterWorking();
                }
            }
        }
        
        public void ReOpenDiagram(decimal DiagramID)
        {
            try
            {
                MyLayoutDocumentt myly = new MyLayoutDocumentt
                {
                    SaveAsClosing = true,
                    MenuRisk = MenuRisk,
                    MenuMainRisk = MenuMainRisk,
                    //MenuRiskLimited = MenuRisk,
                    MenuCM = MenuCM,
                    Ds = DsMain.Copy(),
                    LoginUser = LoginUser,
                    MyWindow = this,
                    MenuGroupCM = MenuGroupCM,
                    MenuGroupRisk = MenuGroupRisk,
                    MenuGroupMixed = MenuGroupMixed,
                    IsPanEnable = isPanEnabled,
                    IdWBSFilter = IdWBSFilterSelected
                };
                myly.ID_Diagram = CreateAndOpenNewDiagram(DiagramID, DVRisk_Tree.Table.Rows.Find(DiagramID)[DT_Diagram.DIAGRAM_NAME].ToString(), myly);
                WindowTreeRisk riskTree = new WindowTreeRisk
                {
                    Operation = General.UPDATE,
                    TopRiskTable = myly.Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Copy(),
                    DRow = myly.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(myly.ID_Diagram),
                    Icon = Icon,
                    IDProject = IdProject,
                    //CM_TopRisk = myly.Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Copy(),
                    Risk_TopRisk = myly.Ds.Tables[DT_Risk_Damages.TABLE_NAME].Copy()
                };
                if (riskTree.ShowDialog() == true)
                {
                    myly.Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Merge(riskTree.TopRiskTable);
                    SetNewDamageToEntireTree(myly.ID_Diagram, myly.Ds);
                    TheProgress.Visibility = Visibility.Visible;
                    HabilitarBotones(false);
                    myly.Title = riskTree.TextName.Text;
                    if (LayoutDocPaneGroup.Children.Count > 0)
                    {
                        ((LayoutDocumentPane)LayoutDocPaneGroup.Children[0]).Children.Add(myly);
                    }
                    OpenedDocuments.Add(myly);
                    TheCurrentLayout = myly;
                    WBSOperations.AddWBSTopToDiagram(myly.Ds, DiagramID, DsWBS);
                    UpdateDiagram();
                    foreach (var item in OpenedDocuments)
                    {
                        item.ExitWorking();
                    }
                    TheCurrentLayout.EnterWorking();
                }
                IS_REOPEN_DIAGRAM = false;
            }
            catch (Exception ex)
            {
                IS_REOPEN_DIAGRAM = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        public decimal CreateAndOpenNewDiagram(decimal idDiagram, string diagramName, MyLayoutDocumentt destinyLayout)
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
                SavingAsController.CreateDiagramDamages(destinyLayout.Ds, drDiagram, originalLayout.Ds, originalLayout);

                SavingAsController.CreateRisks(originalLayout, destinyLayout.LinesList, MenuRisk);

                SavingAsController.CreateCMs(originalLayout, destinyLayout.LinesList, MenuCM);

                TreeOperation.Build_Tree(destinyLayout.LinesList);

                destinyLayout.CopyRisk = destinyLayout.LinesList.FirstOrDefault(p => p.IsRoot);

                DataRow drRisk = destinyLayout.Ds.Tables[DT_Risk.TABLE_NAME].NewRow();
                drRisk[DT_Risk.COMMENTS] = "Detail Main Risk";
                drRisk[DT_Risk.IS_ACTIVE] = true;
                drRisk[DT_Risk.ID_DIAGRAM] = drDiagram[DT_Diagram.ID_DIAGRAM];
                drRisk[DT_Risk.ISCOLLAPSED] = false;
                drRisk[DT_Risk.IS_ROOT] = true;
                drRisk[DT_Risk.NAMESHORT] = "Main Risk " + drDiagram[DT_Diagram.DIAGRAM_NAME];
                drRisk[DT_Risk.POSITION] = 0;
                drRisk[DT_Risk.PROBABILITY] = 0;
                drRisk[DT_Risk.IDRISK_FATHER] = 0;
                drRisk[DT_Risk.IS_CM] = false;
                destinyLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(drRisk);

                SavingAsController.CreateDamageWBSRoleRisks(destinyLayout.Ds, originalLayout.Ds, drRisk, drDiagram, drDiagram[DT_Diagram.DIAGRAM_NAME].ToString(), thecopiedline);

                destinyLayout.CopyRisk.ID = (decimal)drRisk[DT_Risk.ID];

                CopyPasteController.Paste(destinyLayout.CopyRisk, destinyLayout.Ds.Copy(), destinyLayout.Ds, (decimal)drDiagram[DT_Diagram.ID_DIAGRAM]);
                destinyLayout.CopyRisk.IdRiskFather = 0;
                destinyLayout.GridPaintLines.Children.Remove(destinyLayout.CopyRisk);
                destinyLayout.GridPaintLines.Children.Remove(destinyLayout.CopyRisk.TextPanel);
                return (decimal)drDiagram[DT_Diagram.ID_DIAGRAM];
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Set up damages values to all risks and countermeasure
        /// </summary>
        /// <param name="idDiagramaa">Fishbone diagram id</param>
        /// <param name="ds">Fishbone diagram dataset container</param>
        public void SetNewDamageToEntireTree(decimal idDiagramaa, DataSet ds)
        {
            foreach (DataRow damage in ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + idDiagramaa))
            {
                //For each diagram's risk
                foreach (DataRow itemRisk in ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + idDiagramaa))
                {
                    if (!(ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { itemRisk[DT_Risk.ID], damage[DT_Risk_Damages.ID_DAMAGE] })))
                    {
                        DataRow drDamage = ds.Tables[DT_Risk_Damages.TABLE_NAME].NewRow();
                        drDamage[DT_Risk_Damages.ID_DAMAGE] = damage[DT_Diagram_Damages.ID_DAMAGE];
                        drDamage[DT_Risk_Damages.VALUE] = 0;
                        drDamage[DT_Risk_Damages.TOP_RISK] = damage[DT_Diagram_Damages.TOP_RISK];
                        drDamage[DT_Risk_Damages.ID_RISK] = itemRisk[DT_Risk.ID];
                        drDamage[DT_Risk_Damages.ID_RISK_TREE] = itemRisk[DT_Risk.ID_DIAGRAM];
                        drDamage[DT_Risk_Damages.COLOR] = damage[DT_Diagram_Damages.COLOR];
                        drDamage[DT_Risk_Damages.DAMAGE] = damage[DT_Diagram_Damages.DAMAGE].ToString() + "(" + damage[DT_Diagram_Damages.UM].ToString() + ")";
                        ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Add(drDamage);
                    }
                    foreach (DataRow riskWBS in ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + itemRisk[DT_Risk.ID]))
                    {
                        if (WBSOperations.IsRiskWBSLow(riskWBS, DsWBS, ds.Tables[DT_RISK_WBS.TABLE_NAME]))
                        {
                            if (!(ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { riskWBS[DT_RISK_WBS.ID_WBS], damage[DT_Diagram_Damages.ID_DAMAGE], itemRisk[DT_Risk.ID] })))
                            {
                                DataRow drwbsDamage = ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].NewRow();
                                drwbsDamage[DT_WBS_RISK_DAMAGE.ID_DAMAGE] = damage[DT_Diagram_Damages.ID_DAMAGE];
                                drwbsDamage[DT_WBS_RISK_DAMAGE.DAMAGE] = damage[DT_Diagram_Damages.DAMAGE];
                                drwbsDamage[DT_WBS_RISK_DAMAGE.ID_WBS] = riskWBS[DT_RISK_WBS.ID_WBS];
                                drwbsDamage[DT_WBS_RISK_DAMAGE.WBS] = riskWBS[DT_RISK_WBS.WBS];
                                drwbsDamage[DT_WBS_RISK_DAMAGE.ID_RISK] = itemRisk[DT_Risk.ID];
                                drwbsDamage[DT_WBS_RISK_DAMAGE.WBS_USER] = riskWBS[DT_RISK_WBS.WBS_USER];
                                drwbsDamage[DT_WBS_RISK_DAMAGE.VALUE] = 0;
                                ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Add(drwbsDamage);
                            }
                        }
                        else
                        {
                            if (ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Contains(new object[] { riskWBS[DT_RISK_WBS.ID_WBS], damage[DT_Diagram_Damages.ID_DAMAGE], itemRisk[DT_Risk.ID] }))
                            {
                                ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Rows.Find(new object[] { riskWBS[DT_RISK_WBS.ID_WBS], damage[DT_Diagram_Damages.ID_DAMAGE], itemRisk[DT_Risk.ID] }).Delete();
                            }
                        }

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

        /// <summary>
        /// Draw updated fishbone diagram
        /// </summary>
        private void UpdateDiagram()
        {
            try
            {
                TheCurrentLayout.LoadComboDamage();
                TheCurrentLayout.LoadLines();
                TheCurrentLayout.LoadRectangles();
                TheCurrentLayout.DrawNumbers();
                TheCurrentLayout.SetLinesThickness();
                TheCurrentLayout.ScrollGridPaint.ScrollToRightEnd();
                TheCurrentLayout.ScrollGridPaint.ScrollToVerticalOffset(TheCurrentLayout.MainLine.Points[1].Y - 200);

                //TheCurrentLayout.BtnUndoneScope.Visibility = Visibility.Collapsed;

                CrossRiskRightTab(TheCurrentLayout.Ds);
                CrossCMRightTab(TheCurrentLayout.Ds);
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

        /// <summary>
        /// Delete selected diagram
        /// </summary>
        private void DeleteDiagram()
        {
            try
            {
                if (gridDiagramList.dgTreeDiagrams.SelectedIndex >= 0)
                {
                    decimal id = (decimal)DVRisk_Tree[gridDiagramList.dgTreeDiagrams.SelectedIndex].Row[DT_Diagram.ID_DIAGRAM];
                    if (OpenedDocuments.Count > 0 && OpenedDocuments.FindIndex(x => x.ID_Diagram == id) >= 0)
                    {
                        OpenedDocuments.Find(x => x.ID_Diagram == id).SaveAsClosing = false;
                        OpenedDocuments.Find(x => x.ID_Diagram == id).Close();
                    }
                    DVRisk_Tree[gridDiagramList.dgTreeDiagrams.SelectedIndex].Delete();

                    SaveData(DsMain, true);
                    IS_DELETING_DIAGRAM = false;
                }
            }
            catch (Exception ex)
            {
                IS_DELETING_DIAGRAM = false;
                MostrarErrorDialog(ex.Message);
            }
        }
        #endregion

        #region MenuRiskClick
        private void RiskCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (TheCurrentLayout != null)
                {
                    TheCurrentLayout.GridPaintLines.Children.Remove(TheCurrentLayout.Line_Created);
                    //System.Drawing.Color lnColor = System.Drawing.Color.FromArgb(int.Parse(TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram)[TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                    Color lnColor = ((SolidColorBrush)new BrushConverter().ConvertFrom(TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram)[TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString())).Color;

                    //HACER: comando add risk              
                    TheCurrentLayout.Line_Created = new RiskPolyLine(TheCurrentLayout.GridPaintLines, MenuRisk, false)
                    {
                        Stroke = new SolidColorBrush(lnColor),
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

        #region TabEditingRisk

        public void UpdateRiskTabInformation()
        {
            if (TheCurrentLayout != null)
            {
                if (TheCurrentLayout.Line_Selected != null)
                {
                    HasAccess = TheCurrentLayout.FullAccess(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    DataRow RiskRow = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID);
                    RiskLineName = RiskRow[DT_Risk.NAMESHORT].ToString();
                    wrapRisk.TextRiskDetail.Text = RiskRow[DT_Risk.COMMENTS].ToString();
                    
                    DvRiskWBS = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].DefaultView;
                    DvRiskWBS.RowFilter = DT_RISK_WBS.ID_RISK + " = " + RiskRow[DT_Risk.ID];
                    DvRiskWBS.Sort = DT_RISK_WBS.NIVEL;

                    DvRoleRisk = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLE_NAME].DefaultView;
                    DvRoleRisk.RowFilter = DT_Role_Risk.ID_RISK + " = " + RiskRow[DT_Risk.ID];
                    DvRiskDamages = TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].DefaultView;


                    //TODO 14- AQUI ESTA EL FILTRO, SI EL USUARIO NO ES WBS MAS BAJITO EN EL RISK, NO TENDRA NINGUNA ROW EN ESTA TABLA
                    //POR LO QUE NO APARECERA NADA, TAL Y COMO HEMOS VISTO EN LOS EJEMPLOS
                    //CASI LO MISMO QUE EN LA VENTANA RISK, PERO AQUI ES COMO DAR CLICK Y EDITAR
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
                    //DgRiskDamages.ItemsSource = DvRiskDamages;
                    CalculateProbability(RiskRow);
                }
            }
        }

        //TODO: 6-AQUI Y EN LA VENTANA RISK ES DONDE SE VALIDA EL TEMA DE LA PROBABILIDAD
        //PRIMERO VA A LA CLASE STATIC DE "WBSOperations" Y RESUELVE ESE PROBLEMA, LUEGO SE LO ASIGNA AL RIESGO PARA QUE SE ACTUALICE EL TEXTBOX DE LA PROBABILIDAD
        //Y LUEGO SE ACTUALIZA LA PROPIEDAD PROBABILITY EN LA TABLA Risk_Damage, PARA QUE SE ACTUALICE EN LA TABLA CRUZADA
        //CADA VEZ QUE ACTUALIZO LA TABLA Risk_Damage, DEBO MANDAR A CRUZAR LA TABLA NUEVAMENTE, PARA QUE SE ACTUALICE LA UI
        private void CalculateProbability(DataRow RiskRow)
        {
            try
            {
                if (TheCurrentLayout != null)
                {
                    Probability = WBSOperations.RiskWBSValidations(RiskRow, TheCurrentLayout.Ds, LoginUser, DsWBS, hasAccess, (bool)RiskRow[DT_Risk.IS_CM]);

                    RiskRow[DT_Risk.PROBABILITY] = Probability;
                    foreach (DataRow item in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
                    {
                        item[DT_Risk_Damages.PROBABILITY] = RiskRow[DT_Risk.PROBABILITY];
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }
        
        private void TabDelete_RISK_WBS_Row(DataRow fila)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    foreach (DataRow itemC in TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + TheCurrentLayout.Line_Selected.ID + " AND " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + fila[DT_WBS.ID_WBS]))
                    {
                        itemC.Delete();
                    }

                    //WBSOperations.SetDefaultWBSPrimary(fila, TheCurrentLayout.Ds, TheCurrentLayout.Line_Selected.ID);
                    WBSOperations.SetAsPrimaryWhoDelete(fila, TheCurrentLayout.Ds, LoginUser, TheCurrentLayout.Line_Selected.ID, DsWBS);
                    foreach (DataRow descendant in WBSOperations.MyWBSDescendants(DsWBS.Tables[DT_WBS.TABLE_NAME].Rows.Find(fila[DT_RISK_WBS.ID_WBS]), DsWBS.Tables[DT_WBS.TABLE_NAME]))
                    {
                        if (TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { TheCurrentLayout.Line_Selected.ID, descendant[DT_WBS.ID_WBS] }))
                        {
                            TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Find(new object[] { TheCurrentLayout.Line_Selected.ID, descendant[DT_WBS.ID_WBS] }).Delete();
                        }
                    }
                    fila.Delete();
                    foreach (DataRow itemWBS in TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + TheCurrentLayout.Line_Selected.ID))
                    {
                        if (WBSOperations.IsRiskWBSLow(itemWBS, DsWBS, TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME]))
                        {
                            WBSOperations.TabAddWBS_LINE_Damage(itemWBS, TheCurrentLayout.Line_Selected.ID, false, TheCurrentLayout.Ds);
                        }
                        else
                        {//TENGO QUE BORRAR EL DAMAGE_WBS_RISK, PUES YA NO ES LOWLEVEL
                            WBSOperations.TabDeleteWBS_LINE_Damage(itemWBS, TheCurrentLayout.Line_Selected.ID, false, TheCurrentLayout.Ds);
                        }
                    }
                    CalculateProbability(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    RefreshRiskDamageValues(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID));
                    IS_DELETING_Risk_WBS_ROW = false;
                }
            }
            catch (Exception ex)
            {
                IS_DELETING_Risk_WBS_ROW = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void RefreshRiskDamageValues(DataRow RiskRow)
        {
            foreach (DataRow itemDamage in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + RiskRow[DT_Risk.ID]))
            {//primero recorro los Daños de los riesgos
                decimal valor = 0;
                foreach (DataRow itemWBS in TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + RiskRow[DT_Risk.ID] + " AND " + DT_WBS_RISK_DAMAGE.ID_DAMAGE + " = " + itemDamage[DT_Risk_Damages.ID_DAMAGE]))
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
        }       

        private void Delete_Role(DataRow fila)
        {
            try
            {
                if (TheCurrentLayout != null && TheCurrentLayout.Line_Selected != null)
                {
                    foreach (var item in TreeOperation.GetOnlyMyChildrenWithCM(TheCurrentLayout.Line_Selected))
                    {
                        if (TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLE_NAME].Rows.Contains(new object[] { item.ID, fila[DT_Role.IDROL_COLUMN] }))
                        {
                            TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLE_NAME].Rows.Find(new object[] { item.ID, fila[DT_Role.IDROL_COLUMN] }).Delete();
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

        /// <summary>
        /// Delete a line as Risk
        /// </summary>
        private void DeleteRisk(RiskPolyLine line)
        {
            try
            {
                TreeOperation.DeleteLine(line, TheCurrentLayout.Ds);
            }
            catch (Exception ex)
            {
                IS_DELETING_RISK = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void LoadDataAfterDeleRisk()
        {
            TheCurrentLayout.DropLines();
            TheCurrentLayout.DropRectangles();
            TheCurrentLayout.LoadLines();
            TheCurrentLayout.LoadRectangles();
            TheCurrentLayout.DrawNumbers();
            TheCurrentLayout.SetLinesThickness();
            CrossRiskRightTab(TheCurrentLayout.Ds);
            CrossCMRightTab(TheCurrentLayout.Ds);
            IS_DELETING_RISK = false;
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

        /// <summary>
        /// Code executed after Enable/Disable risk
        /// </summary>
        private void OnProjectChange()
        {
            TheCurrentLayout.DrawNumbers();
            //TextProbabilityChange(TheCurrentLayout.MainLine);
            TheCurrentLayout.SetLinesThickness();

            CrossRiskRightTab(TheCurrentLayout.Ds);
            CrossCMRightTab(TheCurrentLayout.Ds);
        }

        #endregion

        #region GroupCounterMeasure_Menu        

        private void DeletingCM_Group()
        {
            try
            {
                foreach (RiskPolyLine cm in TheCurrentLayout.CMGroupSelected)
                {
                    if (TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains(cm.ID))
                    {
                        TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(cm.ID).Delete();
                    }
                }
                TheCurrentLayout.ResetGroupCMSelection();
                TheCurrentLayout.DropLines();
                TheCurrentLayout.DropRectangles();
                TheCurrentLayout.LoadLines();
                TheCurrentLayout.LoadRectangles();
                TheCurrentLayout.DrawNumbers();
                //TextProbabilityChange(TheCurrentLayout.MainLine);
                TheCurrentLayout.SetLinesThickness();
                IS_DELETING_GROUP_CM = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_GROUP_CM = false;
                MostrarErrorDialog(ex.Message);
            }
        }       
        #endregion

        #region Functions
        #region CrossTab

        #region RiskCrossTab
        /// <summary>
        /// Update the right visualization panel with Risk Data
        /// </summary>
        /// <param name="myDs"></param>
        public async void CrossRiskRightTab(DataSet myDs)
        {
            //TODO: 03-EXPLICAR TABLA CRUZADA CON EL TEM DE LOS WBS
            try
            {
                IsCalculatingRisk = true;
                DataTable Dt_Cross_Risk = new DataTable();
                if (TheCurrentLayout != null)
                {
                    await Task.Run(() =>
                    {
                        Dt_Cross_Risk = myDs.Tables[DT_Risk_Damages.TABLE_NAME].Clone();
                        //TODO: 04-LA IMPORTANCIA ALTA DE ESTA VALIDACION, QUE PERMITE AL USUARIO CAMBIAR LOS VALORES EN LA MISMISIMA TABLA CRUZADA Y NO IR AL RIESGO EN EL DIAGRAMA
                        if (IdWBSFilterSelected != -1)
                        {
                            if (myDs.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_WBS + " = " + IdWBSFilterSelected).Any())
                            {
                                foreach (var rowRisk in myDs.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + TheCurrentLayout.ID_Diagram + " and " + DT_Risk.IS_CM + " = 0 and " + DT_Risk.IS_ROOT + " = 0"))
                                {
                                    if (myDs.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { rowRisk[DT_Risk.ID], IdWBSFilterSelected }))
                                    {
                                        foreach (DataRow rowRiskDamage in myDs.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK_TREE + " = " + TheCurrentLayout.ID_Diagram + " and " + DT_Risk_Damages.ID_RISK + " = " + rowRisk[DT_Risk.ID]))
                                        {
                                            DataRow drCrossRisk = Dt_Cross_Risk.NewRow();
                                            drCrossRisk[DT_Risk_Damages.COLOR] = rowRiskDamage[DT_Risk_Damages.COLOR];
                                            drCrossRisk[DT_Risk_Damages.DAMAGE] = rowRiskDamage[DT_Risk_Damages.DAMAGE];
                                            drCrossRisk[DT_Risk_Damages.FATHER] = rowRiskDamage[DT_Risk_Damages.FATHER];
                                            drCrossRisk[DT_Risk_Damages.GROUPE_NAME] = rowRiskDamage[DT_Risk_Damages.GROUPE_NAME];
                                            drCrossRisk[DT_Risk_Damages.ID_DAMAGE] = rowRiskDamage[DT_Risk_Damages.ID_DAMAGE];
                                            drCrossRisk[DT_Risk_Damages.ID_FATHER] = rowRiskDamage[DT_Risk_Damages.ID_FATHER];
                                            drCrossRisk[DT_Risk_Damages.ID_GROUPE] = rowRiskDamage[DT_Risk_Damages.ID_GROUPE];
                                            drCrossRisk[DT_Risk_Damages.ID_RISK] = rowRiskDamage[DT_Risk_Damages.ID_RISK];
                                            drCrossRisk[DT_Risk_Damages.ID_RISK_TREE] = rowRiskDamage[DT_Risk_Damages.ID_RISK_TREE];
                                            drCrossRisk[DT_Risk_Damages.ID_WBS] = rowRiskDamage[DT_Risk_Damages.ID_WBS];
                                            drCrossRisk[DT_Risk_Damages.IS_ROOT] = rowRiskDamage[DT_Risk_Damages.IS_ROOT];
                                            drCrossRisk[DT_Risk_Damages.PROBABILITY] = rowRiskDamage[DT_Risk_Damages.PROBABILITY];
                                            drCrossRisk[DT_Risk_Damages.RISK_NAMESHORT] = rowRiskDamage[DT_Risk_Damages.RISK_NAMESHORT];
                                            drCrossRisk[DT_Risk_Damages.RISK_TREE] = rowRiskDamage[DT_Risk_Damages.RISK_TREE];
                                            drCrossRisk[DT_Risk_Damages.STATUS] = rowRiskDamage[DT_Risk_Damages.STATUS];
                                            drCrossRisk[DT_Risk_Damages.TOP_RISK] = rowRiskDamage[DT_Risk_Damages.TOP_RISK];
                                            drCrossRisk[DT_Risk_Damages.USERNAME] = rowRiskDamage[DT_Risk_Damages.USERNAME];
                                            drCrossRisk[DT_Risk_Damages.VALUE] = rowRiskDamage[DT_Risk_Damages.VALUE];
                                            drCrossRisk[DT_Risk_Damages.WBS_NAME] = rowRiskDamage[DT_Risk_Damages.WBS_NAME];
                                            drCrossRisk[DT_Risk_Damages.CanEditName] = TheCurrentLayout.FullAccess(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(rowRisk[DT_Risk.ID]));
                                            if (IdWBSFilterSelected != -1)
                                            {
                                                if (TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { rowRisk[DT_Risk.ID], IdWBSFilterSelected }))
                                                {
                                                    DataRow theWBSRiskRow = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Find(new object[] { rowRisk[DT_Risk.ID], IdWBSFilterSelected });
                                                    drCrossRisk[DT_Risk_Damages.CanEditProbability] = UsuarioUsaWBSRisk((decimal)rowRisk[DT_Risk.ID]) && WBSOperations.IsRiskWBSLow(theWBSRiskRow, DsWBS, TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME]);
                                                }
                                            }
                                            else
                                            {
                                                drCrossRisk[DT_Risk_Damages.CanEditProbability] = false;
                                            }
                                            CrossTabController.AjustarProbabilidadRisk(drCrossRisk, myDs.Tables[DT_RISK_WBS.TABLE_NAME], IdWBSFilterSelected);
                                            CrossTabController.AjustarDamagesRisk(drCrossRisk, myDs.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME], IdWBSFilterSelected);
                                            Dt_Cross_Risk.Rows.Add(drCrossRisk);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (DataRow item in myDs.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK_TREE + " = " + TheCurrentLayout.ID_Diagram + " and " + DT_Risk.IS_CM + " = 0"))
                            {
                                Dt_Cross_Risk.ImportRow(item);
                            }
                        }

                        DataTable dtTemp = Dt_Cross_Risk.Copy();
                        DataColumn[] pkCC = new DataColumn[2];

                        pkCC[0] = new DataColumn(DT_Risk_Damages.ID_RISK, typeof(decimal));
                        pkCC[1] = new DataColumn(DT_Risk_Damages.ID_RISK_TREE, typeof(decimal));

                        Dt_Cross_Risk = General.CrossTable(ref dtTemp, DT_Risk_Damages.DAMAGE, new string[] { DT_Risk_Damages.VALUE }, pkCC);
                    });
                }
                this.Dispatcher.Invoke(() =>
                {
                    //TODO: 05-ESTA PARTE ES PARA AGREGAR ESPACIOS SEGUN LOS NIVELES EN QUE ESTA EL RIESGO Y LA CM, 
                    //SOLUCION EFECTIVA PERO NADA ELEGANTE, AQUI HAY QUE ESTUDIAR UN BINDING
                    foreach (var line in TheCurrentLayout.LinesList)
                    {
                        if (Dt_Cross_Risk.Select(DT_Risk_Damages.ID_RISK + " = " + line.ID).Any())
                        {
                            Dt_Cross_Risk.Select(DT_Risk_Damages.ID_RISK + " = " + line.ID).First()[DT_Risk_Damages.RISK_NAMESHORT] = Dt_Cross_Risk.Select(DT_Risk_Damages.ID_RISK + " = " + line.ID).First()[DT_Risk_Damages.RISK_NAMESHORT].ToString().Insert(0, TreeOperation.Spaces(line.MyLevel));
                        }
                    }
                    Dt_Cross_Risk.AcceptChanges();
                    CrossTabController.CleanDynamicRiskColumns(GridRisksCross.dgRisksCross);
                    GridRisksCross.dgRisksCross.AutoGenerateColumns = false;
                    AddDynamicRiskColumns(Dt_Cross_Risk);
                    TreeOperation.OrderTableHierarquical(Dt_Cross_Risk, TheCurrentLayout.LinesList, DT_Risk_Damages.ID_RISK);
                    DV_CrossRisk = new DataView(Dt_Cross_Risk);
                    UpdateGroupTab(myDs);
                    IsCalculatingRisk = false;
                });
                Dt_Cross_Risk.Dispose();
            }
            catch (Exception ex)
            {
                IsCalculatingRisk = false;
                MostrarErrorDialog(ex.Message + "- CruzandoTablaRisk");
            }
        }

        private void AddDynamicRiskColumns(DataTable Dt_Cross_Risk)
        {
            for (int i = 0; i < Dt_Cross_Risk.Columns.Count; i++)
            {
                switch (Dt_Cross_Risk.Columns[i].ToString())
                {
                    case DT_Risk_Damages.ID_FATHER:
                        break;
                    case DT_Risk_Damages.IS_ROOT:
                        break;
                    case DT_Risk_Damages.iS_CM:
                        break;
                    case DT_Risk_Damages.PROBABILITY:
                        break;
                    case DT_Risk_Damages.RISK_NAMESHORT:
                        break;
                    case DT_Risk_Damages.WBS_NAME:
                        break;
                    case DT_Risk_Damages.USERNAME:
                        break;
                    case DT_Risk_Damages.FATHER:
                        break;
                    case DT_Risk_Damages.STATUS:
                        break;
                    case DT_Risk_Damages.GROUPE_NAME:
                        break;
                    case DT_Risk_Damages.ID_WBS:
                        break;
                    case DT_Risk_Damages.ID_RISK_TREE:
                        break;
                    case "details":
                        break;
                    case DT_Risk_Damages.TOP_RISK:
                        break;
                    case DT_Risk_Damages.COLOR:
                        break;
                    case DT_Risk_Damages.RISK_TREE:
                        break;
                    case DT_Risk_Damages.ID_DAMAGE:
                        break;
                    case DT_Risk_Damages.ID_RISK:
                        break;
                    case DT_Risk_Damages.ID_GROUPE:
                        break;
                    case DT_Risk_Damages.CanEditName:
                        break;
                    case DT_Risk_Damages.CanEditProbability:
                        break;
                    default:
                        Binding columnBinding = new Binding(Dt_Cross_Risk.Columns[i].ToString());
                        if (Dt_Cross_Risk.Columns[i].DataType.Equals(typeof(decimal)))
                        {
                            columnBinding.Converter = new DecimalUIConverter();
                            columnBinding.ConverterCulture = CultureInfo.CurrentUICulture;
                            columnBinding.ConverterParameter = decimalConfig;
                        }

                        Binding enableBinding = new Binding(DT_Risk_Damages.CanEditProbability);

                        Style style = new Style(typeof(TextBox), ((TextBox)FindResource("damageTextBox")).Style);

                        var textCell = new FrameworkElementFactory(typeof(DamageText)) { Name = "DamageIn" + Dt_Cross_Risk.Columns[i].ToString() };
                        textCell.SetValue(StyleProperty, style);
                        textCell.SetBinding(DamageText.TextProperty, columnBinding);
                        textCell.SetBinding(DamageText.IsEnabledProperty, enableBinding);
                        textCell.SetValue(DamageText.DamageColumnProperty, Dt_Cross_Risk.Columns[i].ToString());
                        textCell.AddHandler(LostFocusEvent, new RoutedEventHandler(DamageValueCrossRisk_LostFocusEvent));

                        DataGridTemplateColumn tmp = new DataGridTemplateColumn
                        {
                            Header = Dt_Cross_Risk.Columns[i].ToString()
                        };

                        DataTrigger dtIsKeyBoarFocus = new DataTrigger() { Value = true };
                        Binding triggerIsKeyBoarFocusBinding = new Binding(TextBox.IsKeyboardFocusWithinProperty.Name) { ElementName = textCell.Name };                      
                        dtIsKeyBoarFocus.Setters.Add(new Setter(TextBox.TextProperty, new Binding(Dt_Cross_Risk.Columns[i].ToString()) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }, textCell.Name));
                        dtIsKeyBoarFocus.Binding = triggerIsKeyBoarFocusBinding;

                        DataTrigger dtIsFocused = new DataTrigger() { Value = true };
                        Binding triggerIsFocusedBinding = new Binding(IsFocusedProperty.Name) { ElementName = textCell.Name };
                        dtIsFocused.Setters.Add(new Setter(TextBox.TextProperty, new Binding(Dt_Cross_Risk.Columns[i].ToString()) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }, textCell.Name));
                        dtIsFocused.Binding = triggerIsFocusedBinding;

                        DataTemplate template = new DataTemplate() { VisualTree = textCell };
                        template.Triggers.Add(dtIsKeyBoarFocus);
                        template.Triggers.Add(dtIsFocused);

                        tmp.CellTemplate = template;
                        tmp.MinWidth = 100;
                        tmp.MaxWidth = 300;
                        GridRisksCross.dgRisksCross.Columns.Add(tmp);
                        break;                        
                }
            }
        }

        #endregion

        #region CMCrossTab        
        /// <summary>
        /// Generate and show countermeasure crosstable
        /// </summary>
        /// <param name="myDs"></param>
        public async void CrossCMRightTab(DataSet myDs)
        {
            try
            {
                IsCalculatingCM = true;
                DataTable Dt_Cross_CM = new DataTable();
                await Task.Run(() =>
                {
                    {
                        {
                            Dt_Cross_CM = myDs.Tables[DT_Risk_Damages.TABLE_NAME].Clone();

                            if (IdWBSFilterSelected != -1)
                            {
                                if (myDs.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_WBS + " = " + IdWBSFilterSelected).Any())
                                {
                                    foreach (var rowCM in myDs.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + TheCurrentLayout.ID_Diagram + " and " + DT_Risk.IS_CM + " = 1"))
                                    {
                                        if (myDs.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { rowCM[DT_Risk.ID], IdWBSFilterSelected }))
                                        {
                                            foreach (var rowCMDamage in myDs.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK_TREE + " = " + TheCurrentLayout.ID_Diagram + " and " + DT_Risk_Damages.ID_RISK + " = " + rowCM[DT_Risk.ID]))
                                            {
                                                DataRow drCrossCM = Dt_Cross_CM.NewRow();
                                                drCrossCM[DT_Risk_Damages.GROUPE_NAME] = rowCMDamage[DT_Risk_Damages.GROUPE_NAME];
                                                drCrossCM[DT_Risk_Damages.COLOR] = rowCMDamage[DT_Risk_Damages.COLOR];
                                                drCrossCM[DT_Risk_Damages.RISK_NAMESHORT] = rowCMDamage[DT_Risk_Damages.RISK_NAMESHORT];
                                                drCrossCM[DT_Risk_Damages.DAMAGE] = rowCMDamage[DT_Risk_Damages.DAMAGE];
                                                drCrossCM[DT_Risk_Damages.ID_FATHER] = rowCMDamage[DT_Risk_Damages.ID_FATHER];
                                                drCrossCM[DT_Risk_Damages.ID_RISK] = rowCMDamage[DT_Risk_Damages.ID_RISK];
                                                drCrossCM[DT_Risk_Damages.ID_DAMAGE] = rowCMDamage[DT_Risk_Damages.ID_DAMAGE];
                                                drCrossCM[DT_Risk_Damages.ID_GROUPE] = rowCMDamage[DT_Risk_Damages.ID_GROUPE];
                                                drCrossCM[DT_Risk_Damages.ID_RISK_TREE] = rowCMDamage[DT_Risk_Damages.ID_RISK_TREE];
                                                drCrossCM[DT_Risk_Damages.ID_WBS] = rowCMDamage[DT_Risk_Damages.ID_WBS];
                                                drCrossCM[DT_Risk_Damages.FATHER] = rowCMDamage[DT_Risk_Damages.FATHER];
                                                drCrossCM[DT_Risk_Damages.PROBABILITY] = rowCMDamage[DT_Risk_Damages.PROBABILITY];
                                                drCrossCM[DT_Risk_Damages.STATUS] = rowCMDamage[DT_Risk_Damages.STATUS];
                                                drCrossCM[DT_Risk_Damages.TOP_RISK] = rowCMDamage[DT_Risk_Damages.TOP_RISK];
                                                drCrossCM[DT_Risk_Damages.USERNAME] = rowCMDamage[DT_Risk_Damages.USERNAME];
                                                drCrossCM[DT_Risk_Damages.VALUE] = rowCMDamage[DT_Risk_Damages.VALUE];
                                                drCrossCM[DT_Risk_Damages.WBS_NAME] = rowCMDamage[DT_Risk_Damages.WBS_NAME];
                                                drCrossCM[DT_Risk_Damages.CanEditName] = TheCurrentLayout.FullAccess(TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(rowCM[DT_Risk.ID]));
                                                if (IdWBSFilterSelected != -1)
                                                {
                                                    if (TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { rowCM[DT_Risk.ID], IdWBSFilterSelected }))
                                                    {
                                                        DataRow theWBSRiskRow = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Find(new object[] { rowCM[DT_Risk.ID], IdWBSFilterSelected });
                                                        drCrossCM[DT_Risk_Damages.CanEditProbability] = UsuarioUsaWBSRisk((decimal)rowCM[DT_Risk.ID]) && WBSOperations.IsRiskWBSLow(theWBSRiskRow, DsWBS, TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME]);
                                                    }
                                                }
                                                else
                                                {
                                                    drCrossCM[DT_Risk_Damages.CanEditProbability] = false;
                                                }
                                                CrossTabController.AjustarProbabilidadRisk(drCrossCM, myDs.Tables[DT_RISK_WBS.TABLE_NAME], IdWBSFilterSelected);
                                                CrossTabController.AjustarDamagesRisk(drCrossCM, myDs.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME], IdWBSFilterSelected);
                                                Dt_Cross_CM.Rows.Add(drCrossCM);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (DataRow item in myDs.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK_TREE + " = " + TheCurrentLayout.ID_Diagram + " and " + DT_Risk.IS_CM + " = 1"))
                                {
                                    Dt_Cross_CM.ImportRow(item);
                                }
                            }
                            DataTable dtTemp = Dt_Cross_CM.Copy();
                            DataColumn[] pkCC = new DataColumn[2];

                            pkCC[0] = new DataColumn(DT_Risk_Damages.ID_RISK, typeof(decimal));
                            pkCC[1] = new DataColumn(DT_Risk_Damages.ID_RISK_TREE, typeof(decimal));

                            Dt_Cross_CM = General.CrossTable(ref dtTemp, DT_Risk_Damages.DAMAGE, new string[] { DT_Risk_Damages.VALUE }, pkCC);

                        }
                    }
                });
                this.Dispatcher.Invoke(() =>
                {
                    foreach (var line in TheCurrentLayout.LinesList)
                    {
                        if (Dt_Cross_CM.Select(DT_Risk_Damages.ID_RISK + " = " + line.ID).Any())
                        {
                            Dt_Cross_CM.Select(DT_Risk_Damages.ID_RISK + " = " + line.ID).First()[DT_Risk_Damages.RISK_NAMESHORT] = Dt_Cross_CM.Select(DT_Risk_Damages.ID_RISK + " = " + line.ID).First()[DT_Risk_Damages.RISK_NAMESHORT].ToString().Insert(0, TreeOperation.Spaces(line.MyLevel));
                        }
                    }
                    Dt_Cross_CM.AcceptChanges();
                    CrossTabController.CleanDynamicCMColumns(GridCrossCM.dgCrossCM);
                    GridCrossCM.dgCrossCM.AutoGenerateColumns = false;
                    AddDynamicCMColumns(Dt_Cross_CM);
                    TreeOperation.OrderTableHierarquical(Dt_Cross_CM, TheCurrentLayout.LinesList, DT_Risk_Damages.ID_RISK);
                    DV_Cross_CM = new DataView(Dt_Cross_CM);
                    //dgCrossCM.ItemsSource = DV_Cross_CM;
                    UpdateGroupTab(myDs);
                    IsCalculatingCM = false;
                });
                Dt_Cross_CM.Dispose();

            }
            catch (Exception ex)
            {
                IsCalculatingRisk = false;
                MostrarErrorDialog(ex.Message + "- CruzandoTablaCM");
            }
        }

        private void AddDynamicCMColumns(DataTable Dt_Cross_CM)
        {
            for (int i = 0; i < Dt_Cross_CM.Columns.Count; i++)
            {
                switch (Dt_Cross_CM.Columns[i].ToString())
                {
                    case DT_Risk_Damages.ID_FATHER:
                        break;
                    case DT_Risk_Damages.IS_ROOT:
                        break;
                    case DT_Risk_Damages.iS_CM:
                        break;
                    case DT_Risk_Damages.RISK_NAMESHORT:
                        break;
                    case DT_Risk_Damages.WBS_NAME:
                        break;
                    case DT_Risk_Damages.USERNAME:
                        break;
                    case DT_Risk_Damages.FATHER:
                        break;
                    case DT_Risk_Damages.STATUS:
                        break;
                    case DT_Risk_Damages.GROUPE_NAME:
                        break;
                    case DT_Risk_Damages.ID_WBS:
                        break;
                    case DT_Risk_Damages.ID_RISK_TREE:
                        break;
                    case "details":
                        break;
                    case DT_Risk_Damages.TOP_RISK:
                        break;
                    case DT_Risk_Damages.COLOR:
                        break;
                    case DT_Risk_Damages.RISK_TREE:
                        break;
                    case DT_Risk_Damages.ID_DAMAGE:
                        break;
                    case DT_Risk_Damages.ID_RISK:
                        break;
                    case DT_Risk_Damages.ID_GROUPE:
                        break;
                    case DT_Risk_Damages.CanEditName:
                        break;
                    case DT_Risk_Damages.CanEditProbability:
                        break;
                    case DT_Risk_Damages.PROBABILITY:
                        break;
                    default:
                        Binding columnBinding = new Binding(Dt_Cross_CM.Columns[i].ToString());
                        if (Dt_Cross_CM.Columns[i].DataType.Equals(typeof(decimal)))
                        {
                            columnBinding.Converter = new DecimalUIConverter();
                            columnBinding.ConverterCulture = CultureInfo.CurrentUICulture;
                            columnBinding.ConverterParameter = decimalConfig;
                        }

                        Binding enableBinding = new Binding(DT_Risk_Damages.CanEditProbability);

                        Style style = new Style(typeof(TextBox), ((TextBox)FindResource("damageTextBox")).Style);

                        var textCell = new FrameworkElementFactory(typeof(DamageText)) { Name = "DamageIn" + Dt_Cross_CM.Columns[i].ToString() };
                        textCell.SetValue(StyleProperty, style);
                        textCell.SetValue(DamageText.DamageColumnProperty, Dt_Cross_CM.Columns[i].ToString());
                        textCell.SetBinding(TextBox.TextProperty, columnBinding);
                        textCell.SetBinding(TextBox.IsEnabledProperty, enableBinding);

                        textCell.AddHandler(LostFocusEvent, new RoutedEventHandler(DamageValueCrossRisk_LostFocusEvent));

                        DataGridTemplateColumn tmp = new DataGridTemplateColumn
                        {
                            Header = Dt_Cross_CM.Columns[i].ToString()
                        };

                        DataTrigger dtIsKeyBoarFocus = new DataTrigger() { Value = true };
                        Binding triggerIsKeyBoarFocusBinding = new Binding(TextBox.IsKeyboardFocusWithinProperty.Name) { ElementName = textCell.Name };
                        dtIsKeyBoarFocus.Setters.Add(new Setter(TextBox.TextProperty, new Binding(Dt_Cross_CM.Columns[i].ToString()) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }, textCell.Name));
                        dtIsKeyBoarFocus.Binding = triggerIsKeyBoarFocusBinding;

                        DataTrigger dtIsFocused = new DataTrigger() { Value = true };
                        Binding triggerIsFocusedBinding = new Binding(IsFocusedProperty.Name) { ElementName = textCell.Name };
                        dtIsFocused.Setters.Add(new Setter(TextBox.TextProperty, new Binding(Dt_Cross_CM.Columns[i].ToString()) { UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }, textCell.Name));
                        dtIsFocused.Binding = triggerIsFocusedBinding;

                        DataTemplate template = new DataTemplate() { VisualTree = textCell };
                        template.Triggers.Add(dtIsKeyBoarFocus);
                        template.Triggers.Add(dtIsFocused);

                        tmp.CellTemplate = template;
                        //column.Binding = columnBinding;
                        tmp.MinWidth = 100;
                        tmp.MaxWidth = 300;
                        GridCrossCM.dgCrossCM.Columns.Add(tmp);
                        break;
                }
            }

        }

        #endregion

        #endregion

        #region Grouping
        /// <summary>
        /// Show cross table risk and risk group
        /// </summary>
        /// <param name="myDs"></param>
        public void UpdateGroupTab(DataSet myDs)
        {
            if (TheCurrentLayout != null)
            {
                //var query = CurrentLayout.LinesList.GroupBy(x => x.Group.IdGroup);
                List<LineGroup> listaGroup = new List<LineGroup>();
                foreach (var line in TheCurrentLayout.LinesList)
                {
                    if (!(listaGroup.Exists(lg => lg.IdGroup == line.Group.IdGroup)))
                    {
                        listaGroup.Add(line.Group);
                    }
                }
                groupPanel.Children.Clear();

                if (listaGroup != null)
                {
                    foreach (LineGroup lineGroup in listaGroup)
                    {
                        if (lineGroup.IdGroup != 0)
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add(new DataColumn("ID", typeof(decimal)));
                            dt.Columns.Add(new DataColumn("Element", typeof(string)));
                            dt.Columns.Add(new DataColumn("Name", typeof(string)));
                            dt.Columns.Add(new DataColumn("Father", typeof(string)));
                            dt.Columns.Add(new DataColumn("Activated", typeof(bool)));
                            dt.Columns.Add(new DataColumn("Probability", typeof(decimal)));
                            dt.TableName = myDs.Tables[DT_Groupe.TABLE_NAME].Rows.Find(lineGroup.IdGroup)[DT_Groupe.GROUPE_NAME].ToString();

                            foreach (var line in TheCurrentLayout.LinesList.Where(x => x.Group.IdGroup == lineGroup.IdGroup))
                            {
                                DataRow dr = dt.NewRow();
                                dr["Element"] = line.IsCM ? "CounterMeasure" : "Risk";
                                dr["Name"] = line.ShortName;
                                dr["ID"] = line.ID;
                                dr["Probability"] = line.Probability * 100;
                                dr["Activated"] = line.IsEnabled;
                                if (line.Father != null)
                                {
                                    dr["Father"] = line.Father.ShortName;
                                }
                                else { dr["Father"] = "Is Root!"; }

                                dt.Rows.Add(dr);
                            }
                            MyExpanderGroupItemm exgroup = new MyExpanderGroupItemm()
                            {
                                GroupName = lineGroup.GroupName,
                                IdGroup = lineGroup.IdGroup
                            };
                            exgroup.MyDV = new DataView(dt);
                            groupPanel.Children.Add(exgroup);
                            groupPanel.Children.Add(new MyBorderGroup());
                        }
                    }
                }
            }
        }


        #region GridGroupEvents

        private void Delete_Risk_From_Group_Tab(decimal idRiskToDeleteFromGroup)
        {        
            try
            {
                if (TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains(idRiskToDeleteFromGroup))
                {
                    DataRow riskRow = TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(idRiskToDeleteFromGroup);
                    riskRow[DT_Risk.GROUPE_NAME] = "None";
                    riskRow[DT_Risk.ID_GROUPE] = DBNull.Value;
                    TheCurrentLayout.LinesList.Find(x => x.ID == idRiskToDeleteFromGroup).Group = new LineGroup() { IdGroup = 0, GroupName = "None"};
                }
                UpdateGroupTab(TheCurrentLayout.Ds);
                IS_DELETING_RISK_FROM_GROUP_TAB = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_RISK_FROM_GROUP_TAB = false;
                MostrarErrorDialog(ex.Message);
            }
        }
        
        #endregion

        #region TreeViewGroupEvents
        private void RemoveGroupFromDatabase(decimal IDGroupe)
        {
            try
            {
                foreach (DataRow lineRow in TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_GROUPE + " = " + IDGroupe))
                {
                    if (lineRow.RowState != DataRowState.Deleted)
                    {
                        lineRow[DT_Risk.ID_GROUPE] = DBNull.Value;
                        lineRow[DT_Risk.GROUPE_NAME] = "None";
                    }
                }
                foreach (var line in TheCurrentLayout.LinesList.FindAll(x => x.Group.IdGroup == IDGroupe))
                {
                    line.Group.IdGroup = 0;
                    line.Group.GroupName = "None";
                }
                if (TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Contains(IDGroupe))
                {
                    TheCurrentLayout.Ds.Tables[DT_Groupe.TABLE_NAME].Rows.Find(IDGroupe).Delete();
                }
                UpdateGroupTab(TheCurrentLayout.Ds);
                IS_REMOVING_GROUP_FROM_DATABASE = false;
            }
            catch (Exception ex)
            {
                IS_REMOVING_GROUP_FROM_DATABASE = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        private void ClearGroup(decimal IdGroup)
        {
            try
            {
                foreach (DataRow line in TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_GROUPE + " = " + IdGroup + " and "
                                                                               + DT_Risk.ID_DIAGRAM + " = " + TheCurrentLayout.ID_Diagram))
                {
                    line[DT_Risk.GROUPE_NAME] = "None";
                    line[DT_Risk.ID_GROUPE] = DBNull.Value;
                    TheCurrentLayout.LinesList.Find(x => x.ID == (decimal)line[DT_Risk.ID]).Group.IdGroup = 0;
                    TheCurrentLayout.LinesList.Find(x => x.ID == (decimal)line[DT_Risk.ID]).Group.GroupName = "None";
                }
                UpdateGroupTab(TheCurrentLayout.Ds);
                IS_CLEANING_GROUP = false;
            }
            catch (Exception ex)
            {
                IS_CLEANING_GROUP = false;
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
                haspermission = UserHasPermisionOnThisRisk((decimal)elementDataRow["ID"]);

                if (!haspermission)
                {
                    return haspermission;
                }
            }
            return haspermission;
        }

        private bool UserHasPermisionOnThisRisk(decimal elementID)
        {
            bool haspermission = false;
            DataRow[] dr = TheCurrentLayout.Ds.Tables[DT_Role_Risk.TABLE_NAME].Select(DT_Role_Risk.ID_RISK + " = " + elementID.ToString());
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
                //if (e.Key == Key.F2)
                //{
                //    TheCurrentLayout.BrigIntoViewSelectedRiskPolyline(TheCurrentLayout.Line_Selected);
                //    TheCurrentLayout.UpdateSelectedPolyLineVisualInfo();
                //    TheCurrentLayout.EditSelectedPolyLineShorName();
                //}
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
        #endregion

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
                using (ServiceRiskController.WebServiceRisk risk = new ServiceRiskController.WebServiceRisk())
                {
                    DsMain.Merge(risk.GetRiskTreeString(new object[] { "%", IdProject }));
                }
                // UPDATE THE TABLE OF TREES AND ITS RESPECTIVE RISK OF EACH ONE
                DVRisk_Tree = DsMain.Tables[DT_Diagram.TABLE_NAME].DefaultView;
                gridDiagramList.dgTreeDiagrams.SelectedIndex = 0;
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

        public bool ExistRole(decimal number)
        {
            decimal num = 0;
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
                gridDiagramList.AddTree.IsEnabled = ExistRole(3);
                gridDiagramList.ImportExcel.IsEnabled = ExistRole(3);
                CanEditDiagram = ExistRole(4);
                CanDeleteDiagram = ExistRole(5);                
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
                    using (ServiceRiskController.WebServiceRisk ws = new ServiceRiskController.WebServiceRisk())
                    {
                        DataSet temp = ds.GetChanges();
                        temp = ws.SaveRisk(temp);
                        ds.Merge(temp);
                        ds.AcceptChanges();
                        RefreshData();
                        Cursor = Cursors.Arrow;
                        if (cartel)
                        {
                            MostrarInfoDialog("Data saved successfully!!!");
                        }
                        IS_SAVING_DATA = false;
                    }
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

        #region DgRisk_&_DgCM_Events

        /// <summary>
        /// Update the Damages Values when user changes it in the right panel
        /// </summary>
        private void DamageValueCrossRisk_LostFocusEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                if (e.Source != null && e.Source is DamageText theTextBox)
                {
                    if (theTextBox.DataContext != null && theTextBox.DataContext is DataRowView theDataView)
                    {
                        DataRow drRiskDamageSelected = theDataView.Row;

                        string column = theTextBox.DamageColumn;
                        if (TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + drRiskDamageSelected[DT_Risk_Damages.ID_RISK] + " and "
                            + DT_Risk_Damages.DAMAGE + " = '" + column.ToString() + "'").Any())
                        {
                            decimal theValue = 0;

                            if (string.IsNullOrWhiteSpace(theTextBox.Text) || theTextBox.Text == "-" || !General.IsNumeric(theTextBox.Text) || Convert.ToDecimal(theTextBox.Text) < 0)
                            {
                                theValue = 0;
                            }
                            else
                            {
                                theValue = Convert.ToDecimal(theTextBox.Text);
                            }
                            if (TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + drRiskDamageSelected[DT_Risk_Damages.ID_RISK] +
                                " and " + DT_WBS_RISK_DAMAGE.DAMAGE + " = '" + column.ToString() + "' and " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + IdWBSFilterSelected).Any())
                            {
                                TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + drRiskDamageSelected[DT_Risk_Damages.ID_RISK] +
                                " and " + DT_WBS_RISK_DAMAGE.DAMAGE + " = '" + column.ToString() + "' and " + DT_WBS_RISK_DAMAGE.ID_WBS + " = " + IdWBSFilterSelected).First()[DT_WBS_RISK_DAMAGE.VALUE] = theValue;
                            }
                            theValue = 0;
                            foreach (DataRow rowRISK_WBS_DAMAGE in TheCurrentLayout.Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME].Select(DT_WBS_RISK_DAMAGE.ID_RISK + " = " + drRiskDamageSelected[DT_Risk_Damages.ID_RISK] +
                                " and " + DT_WBS_RISK_DAMAGE.DAMAGE + " = '" + column.ToString() + "'"))
                            {
                                if (TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Contains(new object[] { drRiskDamageSelected[DT_Risk_Damages.ID_RISK], rowRISK_WBS_DAMAGE[DT_WBS_RISK_DAMAGE.ID_WBS] }))
                                {
                                    DataRow rowWbsRisk = TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Rows.Find(new object[] { drRiskDamageSelected[DT_Risk_Damages.ID_RISK], rowRISK_WBS_DAMAGE[DT_WBS_RISK_DAMAGE.ID_WBS] });

                                    if (WBSOperations.IsRiskWBSLow(rowWbsRisk, DsWBS, TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME]))
                                    {
                                        theValue += (decimal)rowRISK_WBS_DAMAGE[DT_WBS_RISK_DAMAGE.VALUE];
                                    }
                                }
                            }
                            TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLE_NAME].
                                Select(DT_Risk_Damages.ID_RISK + " = " + drRiskDamageSelected[DT_Risk_Damages.ID_RISK] +
                                " and " + DT_Risk_Damages.DAMAGE + " = '" + column.ToString() + "'").First()[DT_Risk_Damages.VALUE] = theValue;

                            TheCurrentLayout.ClearFilters();

                            TheCurrentLayout.DrawNumbers();
                            TheCurrentLayout.UpdateLinesValues();
                            TheCurrentLayout.SetLinesThickness();

                            CrossRiskRightTab(TheCurrentLayout.Ds);
                            CrossCMRightTab(TheCurrentLayout.Ds);
                            TheCurrentLayout.UpdateSelectedPolyLineVisualInfo();

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
        /// Update the Line name in the Table Risk, Risk_Damage and Risk_WBS
        /// </summary>
        /// <param name="theTextBox"></param>
        /// <param name="idRisk"></param>
        private void ChangeRiskNameInCrossTable(TextBox theTextBox, decimal idRisk)
        {
            if (TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains(idRisk))
            {
                TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(idRisk)[DT_Risk.NAMESHORT] = theTextBox.Text.TrimStart();
            }
            foreach (var damageRow in TheCurrentLayout.Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + idRisk))
            {
                damageRow[DT_Risk_Damages.RISK_NAMESHORT] = theTextBox.Text.TrimStart();
            }
            foreach (var wbsRow in TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + idRisk))
            {
                wbsRow[DT_RISK_WBS.RISK] = theTextBox.Text.TrimStart();
            }
            foreach (var wbsRow in TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.IDRISK_FATHER + " = " + idRisk))
            {
                wbsRow[DT_Risk.FATHER] = theTextBox.Text.TrimStart();
            }
            TheCurrentLayout.LinesList.Find(line => line.ID == idRisk).ShortName = theTextBox.Text.TrimStart();

            TextFilterCM = string.Empty;
            TextFilterRisk = string.Empty;

            CrossRiskRightTab(TheCurrentLayout.Ds);
            CrossCMRightTab(TheCurrentLayout.Ds);
            TheCurrentLayout.UpdateSelectedPolyLineVisualInfo();
        }

        /// <summary>
        /// Validate If the Line Name to change, exists in the Diagram
        /// </summary>
        /// <param name="proposedPolyLineName">Line Name to Validate</param>
        private bool? CanUseProposedPolyLineName(string proposedPolyLineName)
        {
            bool? result = null;
            WindowMessageYesNo yesNo = null;
            decimal RiskTreeID = (decimal)TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheCurrentLayout.Line_Selected.ID)[DT_Risk.ID_DIAGRAM];

            if (TheCurrentLayout.Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + RiskTreeID + " and "
                + DT_Risk.NAMESHORT + " = '" + proposedPolyLineName + "' and " + DT_Risk.ID + " <> " + TheCurrentLayout.Line_Selected.ID).Any())
            {
                yesNo = new WindowMessageYesNo("The name [" + proposedPolyLineName + "] Already exists in this diagram. Do you want to use it again?");
                yesNo.ShowDialog();
            }
            if (yesNo != null)
            {
                result = yesNo.DialogResult;
            }
            return result;
        }

        /// <summary>
        /// Validate IF the Loggued User use some WBS in the risk
        /// </summary>
        private bool UsuarioUsaWBSRisk(decimal idRisk)
        {
            return TheCurrentLayout.Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.USERNAME + " = '" + LoginUser + "'" + " and " + DT_RISK_WBS.ID_WBS + " = " + IdWBSFilterSelected + " and " + DT_RISK_WBS.ID_RISK + " = " + idRisk).Any();
        }

        #endregion

        #region WBSRegion
        public void RefreshWBS()
        {
            try
            {
                DsWBS = new UserDataSet();
                using (ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS())
                {
                    DsWBS.Merge(ws.GetAllWBSFiltered(new object[] { IdProject }));
                    //TranslateToTreeViewWBS(DsWBS);
                    LoadTreeview(DsWBS);
                    DV_WBS = DsWBS.Tables[DT_WBS.TABLE_NAME].DefaultView;
                    FillCBWBSFilter();
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void FillCBWBSFilter()
        {
            try
            {
                using (DataTable WBSHijos = new DataTable("WBSCompleto"))
                {
                    DataColumn idwbsColumn = new DataColumn(DT_WBS.ID_WBS, typeof(decimal));
                    DataColumn WbsColumn = new DataColumn(DT_WBS.USER_WBS, typeof(string));
                    WBSHijos.Columns.Add(idwbsColumn);
                    WBSHijos.Columns.Add(WbsColumn);
                    DataRow drWBSHijos = WBSHijos.NewRow();
                    drWBSHijos[DT_WBS.ID_WBS] = -1;
                    drWBSHijos[DT_WBS.USER_WBS] = "General-All";
                    WBSHijos.Rows.Add(drWBSHijos);
                    foreach (DataRow wbsRow in DsWBS.Tables[DT_WBS.TABLE_NAME].Rows)
                    {
                        drWBSHijos = WBSHijos.NewRow();
                        drWBSHijos[DT_WBS.ID_WBS] = wbsRow[DT_WBS.ID_WBS];
                        drWBSHijos[DT_WBS.USER_WBS] = wbsRow[DT_WBS.USERNAME] + " - " + wbsRow[DT_WBS.NIVEL] + wbsRow[DT_WBS.WBS_NAME];
                        WBSHijos.Rows.Add(drWBSHijos);
                    }
                    DvCBWBS = WBSHijos.DefaultView;
                    IdWBSFilterSelected = -1;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void LoadTreeview(DataSet ds)
        {
            WBSNodeList = new ObservableCollection<WBSNodes>();

            foreach (DataRow item in WBSOperations.GetTopWBS(ds))
            {                
                WBSNodes root = new WBSNodes()
                {
                    Name = item[DT_WBS.NIVEL] + " " + item[DT_WBS.WBS_NAME] + " [" + item[DT_WBS.USERNAME] + "]",
                    ID_WBS = (decimal)item[DT_WBS.ID_WBS],
                    CanEdit = LoginUser == item[DT_WBS.USERNAME].ToString(),
                    CanDelete = false,
                    IsEyedOff = IsWBSEyedOff
                };
                GetWBSChildren(ds, (decimal)item[DT_WBS.ID_WBS], root);

                mRootNodes.Add(root);
            }
        }

        public void GetWBSChildren(DataSet ds, decimal idFather, WBSNodes father)
        {
            foreach (DataRow item in ds.Tables[DT_WBS.TABLE_NAME].Select(DT_WBS.ID_FATHER + " = " + idFather))
            {
                WBSNodes wbs = new WBSNodes(father) 
                { 
                    Name = item[DT_WBS.NIVEL] + " " + item[DT_WBS.WBS_NAME] + " [" + item[DT_WBS.USERNAME] + "]", 
                    ID_WBS = (decimal)item[DT_WBS.ID_WBS],
                    CanDelete = father.CanEdit && LoginUser != item[DT_WBS.USERNAME].ToString(),
                    CanEdit = LoginUser == item[DT_WBS.USERNAME].ToString() || father.CanEdit,
                    IsEyedOff = IsWBSEyedOff
                };
                father.Children.Add(wbs);
                GetWBSChildren(ds, (decimal)item[DT_WBS.ID_WBS], wbs);
            }
        }

        /// <summary>
        /// Delete the selected WBS
        /// </summary>
        /// <param name="drWBS">The Selected Row of the WBS table to delete </param>
        private void DeleteWBS(DataRow drWBS)
        {
            try
            {
                WBSOperations.DeleteMyWBSChildren(WBSOperations.MyWBSChildren(drWBS, DsWBS.Tables[DT_WBS.TABLE_NAME]), DsWBS.Tables[DT_WBS.TABLE_NAME]);
                drWBS.Delete();
                if (DsWBS.HasChanges())
                {
                    using (ServiceWBS.WebServiceWBS ws = new ServiceWBS.WebServiceWBS())
                    {
                        DataSet temp = DsWBS.GetChanges();
                        temp = ws.SaveWBS(temp);
                        DsWBS.Merge(temp);
                        DsWBS.AcceptChanges();
                        RefreshWBS();
                    }
                }
                IS_DELETING_WBS = false;
            }
            catch (Exception ex)
            {
                IS_DELETING_WBS = false;
                MostrarErrorDialog(ex.Message);
            }
        }

        #endregion

        private void CloseOpenedDiagrams()
        {
            foreach (var contentToClose in _layoutRoot.Descendents().OfType<LayoutContent>().Where(d => (d.Parent is LayoutDocumentPane || d.Parent is LayoutDocumentFloatingWindow)).ToArray())
            {
                contentToClose.Close();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                CloseOpenedDiagrams();
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
                    if (IS_DELETING_GROUP_CM)
                    {
                        IS_DELETING_GROUP_CM = false;
                    }
                    if (IS_REMOVING_GROUP_FROM_DATABASE)
                    {
                        IS_REMOVING_GROUP_FROM_DATABASE = false;
                    }
                    if (IS_CLEANING_GROUP)
                    {
                        IS_CLEANING_GROUP = false;
                    }
                    if (IS_DELETING_RISK_FROM_GROUP_TAB)
                    {
                        IS_DELETING_RISK_FROM_GROUP_TAB = false;
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
                        DeleteRisk(TheCurrentLayout.Line_Selected);
                        LoadDataAfterDeleRisk();
                    }
                    if (IS_DELETING_GROUP_CM)
                    {
                        DeletingCM_Group();
                    }
                    if (IS_REMOVING_GROUP_FROM_DATABASE)
                    {
                        RemoveGroupFromDatabase(ID_Groupe);
                    }
                    if (IS_CLEANING_GROUP)
                    {
                        ClearGroup(ID_Groupe);
                    }
                    if (IS_DELETING_RISK_FROM_GROUP_TAB)
                    {
                        Delete_Risk_From_Group_Tab(IdRiskToDeleteFromGroup);
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
                        TabDelete_RISK_WBS_Row(Selected_Risk_WBS_Row);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarErrorDialog(ex.Message);
            }
        }

        private void LoginDialog_DialogOpened(object sender, MaterialDesignThemes.Wpf.DialogOpenedEventArgs eventArgs)
        {
            loginContent.TextUser.Focus();
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
                    return;
                }
                if (Equals(eventArgs.Parameter, true))
                {
                    if (IS_LOGIN)
                    {

                        if (loginContent.TextUser.Text != "")
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
                        tempObtenerAcceso.Merge(ws.AuthenticateUsers(loginContent.TextUser.Text, General.Encrypt(loginContent.TextPasword.Password), ref Mensaje, Environment.UserName,
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
            if (loginContent.TextUser.Text != "")
            {
                General gen = new General();
                AccessList = new List<decimal>();
                gen.Usser = loginContent.TextUser.Text;
                using (UserDataSet ds = new UserDataSet())
                {
                    ds.Merge(GetAccess());
                    if (ds.Tables[DT_User.TABLE_NAME].Rows.Count != 0)
                    {
                        LoginUser = ds.Tables[DT_User.TABLE_NAME].Rows[0][DT_User.USERNAME].ToString();
                        foreach (DataRow item in ds.Tables[DT_User_Operation.TABLE_NAME].Rows)
                        {
                            AccessList.Add((decimal)item[DT_User_Operation.ID_OPERATION_COLUMN]);
                        }
                        Title = "Ensure Risk | " + StringResources.CONNECTED_STRING + " " + loginContent.TextUser.Text;
                        LoginMenuItem.Header = StringResources.LOGOFF;
                        Flag_login = true;
                        loginContent.TextPasword.Clear();
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
                        loginContent.TextPasword.Clear();
                        MostrarErrorDialog(StringResources.WRONG_PASSWORD);
                        ValidateAccess();
                    }
                }
            }
            IS_LOGIN = false;
        }
        #endregion

        #region MENU_COMMANDS
        private RelayyCommand _Login_Command;
        public RelayyCommand Login_Command { get { return _Login_Command; } set { _Login_Command = value; OnPropertyChanged("Login_Command"); } }
        private void ImplementLogin_Command()
        {
            Login_Command = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        IS_LOGIN = true;
                        CloseOpenedDiagrams();
                        LoginDialog.IsOpen = true;
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _Project_Command;
        public RelayyCommand Project_Command { get { return _Project_Command; } set { _Project_Command = value; OnPropertyChanged("Project_Command"); } }
        private void ImplementProject_Command()
        {
            Project_Command = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        if (ExistRole(7))//If has access to Project List
                        {
                            WindowProjectList list = new WindowProjectList
                            {
                                Icon = Icon
                            };
                            list.ShowDialog();
                            using (ServiceProject.WebServiceProject webService = new ServiceProject.WebServiceProject())
                            {
                                DvCBProjects = new DataView(webService.GetAllProjects().Tables[DT_Project.TABLE_NAME]);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _User_Command;
        public RelayyCommand User_Command { get { return _User_Command; } set { _User_Command = value; OnPropertyChanged("User_Command"); } }
        private void ImplementUser_Command()
        {
            User_Command = new RelayyCommand(
                _ =>
                {
                    try
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
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _Role_Command;
        public RelayyCommand Role_Command { get { return _Role_Command; } set { _Role_Command = value; OnPropertyChanged("Role_Command"); } }
        private void ImplementRole_Command()
        {
            Role_Command = new RelayyCommand(
                _ =>
                {
                    try
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
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _Damage_Command;
        public RelayyCommand DamageList_Command { get { return _Damage_Command; } set { _Damage_Command = value; OnPropertyChanged("DamageList_Command"); } }
        private void ImplementDamageList_Command()
        {
            DamageList_Command = new RelayyCommand(
                _ =>
                {
                    try
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
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _DefaultRisk_Command;
        public RelayyCommand DefaultRiskList_Command { get { return _DefaultRisk_Command; } set { _DefaultRisk_Command = value; OnPropertyChanged("DefaultRiskList_Command"); } }
        private void ImplementDefaultRiskList_Command()
        {
            DefaultRiskList_Command = new RelayyCommand(
                _ =>
                {
                    try
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
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _Save_Command;
        public RelayyCommand SaveDiagram_Command { get { return _Save_Command; } set { _Save_Command = value; OnPropertyChanged("SaveDiagram_Command"); } }
        private void ImplementSaveDiagram_Command()
        {
            SaveDiagram_Command = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        if (TheCurrentLayout != null && TheCurrentLayout.Ds.HasChanges())
                        {
                            //TheProgress.Visibility = Visibility.Visible;
                            HabilitarBotones(false);
                            Salvando();
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _SaveAs_Command;
        public RelayyCommand SaveAs_Command { get { return _SaveAs_Command; } set { _SaveAs_Command = value; OnPropertyChanged("SaveAs_Command"); } }
        private void ImplementSaveAs_Command()
        {
            SaveAs_Command = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        SaveAsAsyncFunction();
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _RiskCommand;
        public RelayyCommand RiskCommand { get { return _RiskCommand; } set { _RiskCommand = value; OnPropertyChanged("RiskCommand"); } }
        private void ImplementRiskCommand()
        {
            RiskCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        if (TheCurrentLayout != null)
                        {
                            TheCurrentLayout.GridPaintLines.Children.Remove(TheCurrentLayout.Line_Created);
                            //System.Drawing.Color lnColor = System.Drawing.Color.FromArgb(int.Parse(TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram)[TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                            Color lnColor = ((SolidColorBrush)new BrushConverter().ConvertFrom(TheCurrentLayout.Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + TheCurrentLayout.ID_Diagram)[TheCurrentLayout.CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString())).Color;

                            //HACER: comando add risk              
                            TheCurrentLayout.Line_Created = new RiskPolyLine(TheCurrentLayout.GridPaintLines, MenuRisk, false)
                            {
                                Stroke = new SolidColorBrush(lnColor),
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
                });
        }

        private RelayyCommand _CMCommand;
        public RelayyCommand CMCommand { get { return _CMCommand; } set { _CMCommand = value; OnPropertyChanged("CMCommand"); } }
        private void ImplementCMCommand()
        {
            CMCommand = new RelayyCommand(
                _ =>
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
                });
        }

        private RelayyCommand _SettingsCommand;
        public RelayyCommand SettingsCommand { get { return _SettingsCommand; } set { _SettingsCommand = value; OnPropertyChanged("SettingsCommand"); } }
        private void ImplementSettingsCommand()
        {
            SettingsCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        WindowOption option = new WindowOption() { Icon = Icon };
                        option.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _LogsListCommand;
        public RelayyCommand LogsListCommand { get { return _LogsListCommand; } set { _LogsListCommand = value; OnPropertyChanged("LogsListCommand"); } }
        private void ImplementLogsListCommand()
        {
            LogsListCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        WindowLogs wl = new WindowLogs();
                        wl.Show();
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _ExporToImageCommand;
        public RelayyCommand ExporToImageCommand { get { return _ExporToImageCommand; } set { _ExporToImageCommand = value; OnPropertyChanged("ExporToImageCommand"); } }
        private void ImplementExporToImageCommand()
        {
            ExporToImageCommand = new RelayyCommand(
                _ =>
                {
                    try
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
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _ExporToExcelCommand;
        public RelayyCommand ExporToExcelCommand { get { return _ExporToExcelCommand; } set { _ExporToExcelCommand = value; OnPropertyChanged("ExporToExcelCommand"); } }
        private void ImplementExporToExcelCommand()
        {
            ExporToExcelCommand = new RelayyCommand(
                _ =>
                {
                    try
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
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _PanDragCommand;
        public RelayyCommand PanDragCommand { get { return _PanDragCommand; } set { _PanDragCommand = value; OnPropertyChanged("PanDragCommand"); } }
        private void ImplementPanDragCommand()
        {
            PanDragCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        if (TheCurrentLayout != null)
                        {
                            IsPanEnabled = !IsPanEnabled;
                            TheCurrentLayout.IsPanEnable = IsPanEnabled;
                            foreach (var diagram in OpenedDocuments)
                            {
                                diagram.IsPanEnable = IsPanEnabled;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _ShowWBSPannelCommand;
        public RelayyCommand ShowWBSPannelCommand { get { return _ShowWBSPannelCommand; } set { _ShowWBSPannelCommand = value; OnPropertyChanged("ShowWBSPannelCommand"); } }
        private void ImplementShowWBSPannelCommand()
        {
            ShowWBSPannelCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        AnchorWBSList.IsVisible = true;
                        AnchorWBSList.IsSelected = true;
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _ShowPropertiesPannelCommand;
        public RelayyCommand ShowPropertiesPannelCommand { get { return _ShowPropertiesPannelCommand; } set { _ShowPropertiesPannelCommand = value; OnPropertyChanged("ShowPropertiesPannelCommand"); } }
        private void ImplementShowPropertiesPannelCommand()
        {
            ShowPropertiesPannelCommand = new RelayyCommand(
                _ =>
                {
                    try
                    {
                        LayoutProperties.IsVisible = true;
                        LayoutProperties.IsSelected = true;
                    }
                    catch (Exception ex)
                    {
                        MostrarErrorDialog(ex.Message);
                    }
                });
        }

        private RelayyCommand _PrintDiagramCommand;
        public RelayyCommand PrintDiagramCommand { get { return _PrintDiagramCommand; } set { _PrintDiagramCommand = value; OnPropertyChanged("PrintDiagramCommand"); } }
        private void ImplementPrintDiagramCommand()
        {
            PrintDiagramCommand = new RelayyCommand(
                _ =>
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
                });
        }

        private void ReloadApplicationCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        #endregion
    }
    class Lang : DependencyObject
    {

    }

    class ReactiveObject {

        //private Dictionary<string, object> bag = new Dictionary<string, object>();

        //protected T Get<T>([CallerMemberName] string name = "") {
        //    return bag.TryGetValue(name, out T value) ? value : default;
        //}
    }


}
