using DataMapping.Data;
using EnsureBusinesss;
using EnsureBusinesss.Business;
using EnsureRisk.Export;
using EnsureRisk.Export.Trader;
using EnsureRisk.Resources;
using EnsureRisk.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Layout;

namespace EnsureRisk.Classess
{
    /// <summary>
    /// Interaction logic for MyLayoutDocumentt.xaml
    /// </summary>
    public partial class MyLayoutDocumentt : LayoutDocument
    {
        public string LoginUser { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Main_Y { get; set; }
        public Point PointSelected { get; set; }
        public int IdDamageSelected { get; set; }
        #region Booleans
        public bool IsScoping { get; set; }
        public bool MoviendoRisk { get; set; }
        public bool NameEditing { get; set; }
        public bool MoviendoCM { get; set; }
        public bool Loose { get; set; }
        public bool ChoosingCM { get; set; }
        public bool ChoosingRisk { get; set; }
        public bool IsRootSelected { get; set; }
        public bool SaveAsClosing { get; set; }
        public bool SelectingToGroup { get; set; }
        public bool Creando { get; set; }
        public bool Copiando { get; set; }
        public bool IsExportingToExcel { get; set; }
        public LineGroup GroupSelected { get; set; }
        public int ID_Diagram { get; set; }
        public Popin Popin { get; set; }
        public Point MIdPoint { get; set; }
        public DataSet Ds { get; set; }
        public DataRow DrDiagram { get; set; }
        public Window MyWindow { get; set; }
        private readonly BackgroundWorker exportToExcelWorker = new BackgroundWorker();
        #endregion        

        #region Menus
        public ContextMenu MenuRisk { get; set; }
        public ContextMenu MenuRiskLimited { get; set; }
        public ContextMenu MenuMainRisk { get; set; }
        public ContextMenu MenuCM { get; set; }
        public ContextMenu MenuGroupRisk { get; set; }
        public ContextMenu MenuGroupCM { get; set; }
        public ContextMenu MenuGroupMixed { get; set; }
        #endregion

        #region Listas
        public List<RiskPolyLine> LinesList { get; set; }
        public List<RiskPolyLine> LinesMoving { get; set; }
        public List<MyDamage> Rectangles { get; set; }
        public List<RiskPolyLine> ListCopy { get; set; }
        public List<RiskPolyLine> RiskGroupSelected { get; set; }
        public List<RiskPolyLine> CMGroupSelected { get; set; }
        public Dictionary<int, bool> LinesListCMState { get; set; }
        #endregion

        #region RiskPolyLines
        public RiskPolyLine ScopeLine { get; set; }
        public RiskPolyLine CopyRisk { get; set; }
        public RiskPolyLine Line_Selected { get; set; }
        public RiskPolyLine MainLine { get; set; }
        public RiskPolyLine Line_Created { get; set; }
        public RiskPolyLine LineInMoving { get; set; }
        #endregion
        public MyLayoutDocumentt() : base()
        {
            InitializeComponent();
            MIdPoint = new Point(GridPaintLines.Width - 180, GridPaintLines.Height / 2);
            Ds = new UserDataSet();
            LinesList = new List<RiskPolyLine>();
            LinesMoving = new List<RiskPolyLine>();
            Rectangles = new List<MyDamage>();
            RiskGroupSelected = new List<RiskPolyLine>();
            CMGroupSelected = new List<RiskPolyLine>();
            ListCopy = new List<RiskPolyLine>();
            LinesListCMState = new Dictionary<int, bool>();
            MainLine = new RiskPolyLine(GridPaintLines, MenuMainRisk, false);
            IsSelected = true;
            Loose = true;
            MoviendoRisk = MoviendoCM = NameEditing = ChoosingCM = ChoosingRisk = IsRootSelected = SelectingToGroup = Creando = Copiando = false;
            Binding myBinding = new Binding
            {
                Source = SliderZoom,
                Path = new PropertyPath("Value"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(TheZoomComboBox, ComboBox.TextProperty, myBinding);
            TheZoomComboBox.Text = "100";
            EnterWorking();
            exportToExcelWorker.WorkerReportsProgress = true;
            exportToExcelWorker.WorkerSupportsCancellation = true;
            exportToExcelWorker.DoWork += ExportToExcelWorker_DoWork;
            exportToExcelWorker.ProgressChanged += ExportToExcelWorker_ProgressChanged;
            exportToExcelWorker.RunWorkerCompleted += ExportToExcelWorker_RunWorkerCompleted;
            this.Closing += MyLayoutDocument_Closed;
        }

        public void MostrarYesNo(string text)
        {
            ((MainWindow)MyWindow).MostrarDialogYesNo(text);
        }

        private void MyLayoutDocument_Closed(object sender, EventArgs e)
        {
            try
            {
                if (exportToExcelWorker.IsBusy && exportToExcelWorker.WorkerSupportsCancellation)
                {
                    exportToExcelWorker.CancelAsync();
                }
                if (SaveAsClosing)
                {
                    if (Ds.HasChanges())
                    {
                        ((MainWindow)MyWindow).IS_SAVING_DATA = true;
                        MostrarYesNo("Do you want to save the changes on [" + this.Title + "] ?");
                    }
                }
                if (((MainWindow)MyWindow).OpenedDocuments.Contains(this))
                {
                    ((MainWindow)MyWindow).OpenedDocuments.Remove(this);
                    ((MainWindow)MyWindow).MiniMap.MapSource = new ScrollViewer();
                    ((MainWindow)MyWindow).P.TheCurrentLayout = null;
                }
                ((MainWindow)MyWindow).DV_CrossRisk.Table.Clear();
                ((MainWindow)MyWindow).DV_Cross_CM.Table.Clear();
            }
            catch (Exception ex)
            {
                ((MainWindow)MyWindow).IS_SAVING_DATA = false;
                MostrarDialog(ex.Message);
            }
        }

        public void MostrarPopWindow(Point pointToShow, string lineName, string probability, string value, string acumDamage, string acumValue, string EL)
        {
            MyPopWindow.Visibility = Visibility.Visible;
            MyPopWindow.Margin = new Thickness(pointToShow.X, pointToShow.Y, 0, 0);
            MyPopWindow.TextRiskName.Text = lineName;
            MyPopWindow.TextProb.Text = probability;
            MyPopWindow.TextValue.Text = value;
            MyPopWindow.TextAcumDamage.Text = acumDamage;
            MyPopWindow.TextAcumValue.Text = acumValue;
            MyPopWindow.TextEL.Text = EL;
        }

        public void OcultarPopWindow()
        {
            MyPopWindow.Visibility = Visibility.Collapsed;
        }

        public void MostrarPopCMWindow(Point pointToShow, string lineName, string probability, string value)
        {
            MyPopCMWindow.Visibility = Visibility.Visible;
            MyPopCMWindow.Margin = new Thickness(pointToShow.X, pointToShow.Y, 0, 0);
            MyPopCMWindow.TextRiskName.Text = lineName;
            MyPopCMWindow.TextProb.Text = probability;
            MyPopCMWindow.TextValue.Text = value;
        }

        public void OcultarPopCMWindow()
        {
            MyPopCMWindow.Visibility = Visibility.Collapsed;
        }

        public void Scope()
        {
            if (IsScoping)
            {
                try
                {
                    foreach (RiskPolyLine item in LinesList)
                    {
                        item.Visibility = Visibility.Hidden;
                        item.TextPanel.Visibility = Visibility.Hidden;
                        foreach (var itemi in item.Segments)
                        {
                            itemi.Visibility = Visibility.Hidden;
                        }
                    }
                    foreach (RiskPolyLine item in TreeOperation.GetMeAndMyChildrenWithCM(LinesList.Find(x => x.ID == ScopeLine.ID)))
                    {
                        item.Visibility = Visibility.Visible;
                        item.TextPanel.Visibility = Visibility.Visible;
                        foreach (var itemi in item.Segments)
                        {
                            itemi.Visibility = Visibility.Visible;
                        }
                    }
                    LoadRectangles();

                    DrawNumbers();
                    ((MainWindow)MyWindow).TextProbabilityChange(LinesList.Find(x => x.ID == ScopeLine.ID));
                    Title = LinesList.Find(x => x.ID == ScopeLine.ID).ShortName;
                    ((MainWindow)MyWindow).TextDiagram.Text = LinesList.Find(x => x.ID == ScopeLine.ID).ShortName;
                    LinesList.Find(x => x.ID == ScopeLine.ID).ExtrasVisibility(Visibility.Hidden);

                    BtnUndoneScope.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    MostrarDialog(ex.Message);
                }
            }
        }

        private void MostrarDialog(string message)
        {
            ((MainWindow)MyWindow).MostrarErrorDialog(message);
        }

        public void EnterWorking()
        {
            TheMainGrid.Margin = new Thickness(3);
        }
        public void ExitWorking()
        {
            TheMainGrid.Margin = new Thickness(0);
        }

        private bool TengoPermiso(RiskPolyLine linea)
        {
            try
            {
                bool haspermission = false;
                if (linea.IsCM)
                {
                    DataRow[] dr = Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + linea.ID.ToString());

                    foreach (DataRow itemii in dr)
                    {
                        if (Ds.Tables[DT_User_Role.TABLE_NAME].Select(DT_User_Role.ROLE + " = '" + itemii[DT_Role_Risk.Role] + "' and " + DT_User_Role.USER + " = '" + LoginUser + "'").Any())
                        {
                            haspermission = true;
                            break;
                        }
                    }
                }
                else
                {
                    DataRow[] dr = Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + linea.ID.ToString());

                    foreach (DataRow itemii in dr)
                    {
                        if (Ds.Tables[DT_User_Role.TABLE_NAME].Select(DT_User_Role.ROLE + " = '" + itemii[DT_Role_Risk.Role] + "' and " + DT_User_Role.USER + " = '" + LoginUser + "'").Any())
                        {
                            haspermission = true;
                            break;
                        }
                    }
                }
                return haspermission;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region FuncionesDibujo
        public void LoadComboDamage()
        {
            try
            {
                DataRow[] thisTopRisk = Ds.Tables[DT_Diagram_Damages.TABLENAME]
                        .Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram);

                CbFilterTopR.SelectedValuePath = DT_Diagram_Damages.ID_DAMAGE;
                CbFilterTopR.ItemsSource = thisTopRisk.CopyToDataTable().DefaultView;
                CbFilterTopR.DisplayMemberPath = DT_Diagram_Damages.DAMAGE;
                CbFilterTopR.SelectedIndex = 0;
                IdDamageSelected = (int)CbFilterTopR.SelectedValue;
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        public void FixDrawPanel()
        {
            try
            {
                RiskPolyLine firstX = LinesList.OrderBy(rl => rl.Points[0].X).First();
                RiskPolyLine firstY = LinesList.OrderBy(rl => rl.Points[0].Y).First();

                if (firstX.Points[0].X < 0 && firstY.Points[0].Y < 0)
                {
                    TreeOperation.MoveEntireTree(LinesList, (int)(Math.Ceiling(Math.Abs(firstX.Points[0].X) / 70) * 70), (int)(Math.Ceiling(Math.Abs(firstY.Points[0].Y) / 210) * 210), Rectangles);
                }
                else
                {
                    if (firstX.Points[0].X < 0)
                    {
                        TreeOperation.MoveEntireTree(LinesList, (int)(Math.Ceiling(Math.Abs(firstX.Points[0].X) / 70) * 70), 0, Rectangles);
                    }
                    if (firstY.Points[0].Y < 0)
                    {
                        TreeOperation.MoveEntireTree(LinesList, 0, (int)(Math.Ceiling(Math.Abs(firstY.Points[0].Y) / 210) * 210), Rectangles);
                    }
                }
                GridPaintLines.Width = LinesList.OrderByDescending(rl => rl.Points[1].X).First().Points[1].X + 500;
                GridPaintLines.Height = LinesList.OrderByDescending(rl => rl.Points[1].Y).First().Points[1].Y + 300;
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        //TODO: PINTA LOS RECTANGULOS
        public void LoadRectangles()
        {
            try
            {
                RiskPolyLine MyMainLine = IsScoping ? LinesList.Find(x => x.ID == ScopeLine.ID) : MainLine;
                Rectangles = new List<MyDamage>();
                DataRow[] thisTopRisk = Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram);
                double formula = 90 * thisTopRisk.Length / 2;
                double puntoinicialY = MyMainLine.Points[1].Y - formula;
                foreach (DataRow item in thisTopRisk)
                {
                    System.Drawing.Color colorete = System.Drawing.Color.FromArgb(int.Parse(item[DT_Diagram_Damages.COLOR].ToString()));
                    Color mediaColor = Color.FromArgb(colorete.A, colorete.R, colorete.G, colorete.B);

                    MyDamage rectangle = new MyDamage(GridPaintLines,
                    new Point(MyMainLine.Points[1].X + 5 + General.MaxThickness, puntoinicialY),
                    item[DT_Diagram_Damages.DAMAGE].ToString() + " (" + item[DT_Diagram_Damages.UM].ToString() + ")",
                    StringResources.ACUM_VALUE + "(" + item[DT_Diagram_Damages.UM].ToString() + ") :",
                    StringResources.ACUM_DAMAGE, mediaColor, (Int32)item[DT_Diagram_Damages.ID_DAMAGE], item[DT_Diagram_Damages.UM].ToString());

                    Rectangles.Add(rectangle);
                    puntoinicialY += 90;
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        public void DrawNumbers()
        {
            try
            {
                RiskPolyLine MyMainLine = IsScoping ? LinesList.Find(x => x.ID == ScopeLine.ID) : MainLine;
                foreach (MyDamage item in Rectangles)
                {
                    decimal RiskValue = 0;
                    if (Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains(MyMainLine.ID))
                    {
                        RiskValue = General.CalculateTopRiskTreeValue(Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(MyMainLine.ID),
                            Ds.Tables[DT_Risk.TABLE_NAME], item.ID_TopRisk, Ds.Tables[DT_Risk_Damages.TABLENAME],
                            Ds.Tables[DT_CounterM.TABLE_NAME], Ds.Tables[DT_CounterM_Damage.TABLENAME]);
                    }
                    if (LinesList.Count > 0)
                    {
                        decimal AcumDamage = 0;
                        foreach (var itemI in TreeOperation.GetMeAndMyChildrenWithCM(LinesList.Find(rl => rl.ID == MyMainLine.ID)))
                        {
                            decimal value = 0;
                            if (itemI.IsCM)
                            {
                                if (itemI.IsActivated)
                                {
                                    if (Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Contains(new object[] { itemI.ID, item.ID_TopRisk }))
                                    {
                                        value = (Decimal)Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Find(new object[] { itemI.ID, item.ID_TopRisk })[DT_CounterM_Damage.VALUE];
                                    }
                                    AcumDamage += value;
                                }
                            }
                            else
                            {
                                if (itemI.IsActivated)
                                {
                                    if (Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { itemI.ID, item.ID_TopRisk }))
                                    {
                                        value = (Decimal)Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Find(new object[] { itemI.ID, item.ID_TopRisk })[DT_Risk_Damages.VALUE];
                                    }
                                    AcumDamage += value * General.AcumulatedLikelihood(itemI);
                                }
                            }
                        }
                        item.CalculateValue.Text = StringResources.ACUM_VALUE + "(" + item.UM + ") : " + General.MyRound(RiskValue, 2).ToString();
                        item.ExpecteDamage.Text = StringResources.ACUM_DAMAGE + General.MyRound(AcumDamage, 2).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        public void SetLinesThickness()
        {
            try
            {
                if (!(CbFilterTopR.SelectedValue is null))
                {
                    IdDamageSelected = (int)CbFilterTopR.SelectedValue;
                    if (IdDamageSelected != 0)
                    {
                        General.UpdateThickness(LinesList);
                        foreach (RiskPolyLine polyLine in LinesList)
                        {
                            if (!polyLine.IsCM && !polyLine.IsRoot)
                            {
                                polyLine.OnThicknessChange();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        public void AddMainLine(DataRow dr, System.Drawing.Color lnColor)
        {
            try
            {
                MainLine = new RiskPolyLine(GridPaintLines, MenuMainRisk, false)
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(lnColor.A, lnColor.R, lnColor.G, lnColor.B)),
                    IsDiagonal = false,
                    IsRoot = true,
                    IsCM = false,
                    FromTop = false,
                    StrokeThickness = General.MaxThickness,
                    ID = (int)dr[DT_Risk.ID],
                    Probability = (decimal)dr[DT_Risk.PROBABILITY] / 100,
                    ShortName = "Total Risk",
                    MyLevel = 0
                };
                MainLine.Group = new LineGroup()
                {
                    IdGroup = 0,
                    GroupName = "None"
                };

                //adding the 3 points of the risk. The Main line the angle = 180º
                MainLine.Points[0] = (new Point(MIdPoint.X - 500, MIdPoint.Y));
                MainLine.Points[1] = (new Point(MainLine.Points[0].X + 200, MainLine.Points[0].Y));
                //MainLine.Points[2] = (new Point(MainLine.Points[1].X + 10, MainLine.Points[1].Y));

                MainLine.MouseDown += PrincipalLine_MouseDown;
                MainLine.MyName.MouseDown += PrincipalLine_MouseDown;

                MainLine.MouseUp += MainLine_MouseUp;
                MainLine.MyName.MouseUp += MainLine_MouseUp;
                MainLine.IdRiskFather = 0;
                LinesList = new List<RiskPolyLine>//Adding the line to the list that will control all the diagram´s lines.
                {
                    MainLine //adding the 'principal_line' to the list of Line that exist in the main Form
                };
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        public void DropRectangles()
        {
            try
            {
                List<MyDamage> myDamages = new List<MyDamage>();
                foreach (var item in GridPaintLines.Children)
                {
                    if (item is MyDamage)
                    {
                        myDamages.Add((MyDamage)item);
                    }
                }
                foreach (MyDamage item in myDamages)
                {
                    GridPaintLines.Children.Remove(item);
                }
                Rectangles.Clear();
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }

        }

        /// <summary>
        /// Delete all the lines of the list in the Draw Panel, Erasing the list
        /// </summary>
        public void DropLines()
        {
            try
            {
                foreach (RiskPolyLine item in LinesList)
                {
                    // TODO Yordan, limpiar segmentos
                    foreach (SegmentPolyLine sg in item.Segments)
                    {
                        GridPaintLines.Children.Remove(sg);//removing the line
                        //item.TextPanel.Children.Clear();
                        item.TextPanel.Child = null;
                    }

                    //in his cicle we remove any risk of previous risk tree drawing in the main form.
                    GridPaintLines.Children.Remove(item);//removing the line
                    item.TextPanel.Child = null;
                    GridPaintLines.Children.Remove(item.TextPanel);
                }
                LinesList.Clear();
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        public void LoadLines()
        {
            try
            {
                ((MainWindow)MyWindow).P.TheCurrentLayout.GridPaintLines.Children.Clear();

                LinesListCMState.Clear();
                if (CbFilterTopR.SelectedIndex >= 0)
                {
                    LinesList.Clear();
                    System.Drawing.Color drawingCColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram)[CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                    AddMainLine(Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + ID_Diagram + " and " + DT_Risk.IS_ROOT + " = " + 1).First(), drawingCColor);

                    foreach (DataRow item in Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + ID_Diagram))
                    {
                        if (!((bool)item[DT_Risk.IS_ROOT]))
                        {
                            DataRow[] dr = Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + item[DT_Risk.ID].ToString());
                            bool haspermission = false;
                            foreach (DataRow itemii in dr)
                            {
                                if (Ds.Tables[DT_User_Role.TABLE_NAME].Select(DT_User_Role.ROLE + " = '" + itemii[DT_Role_Risk.Role] + "' and " + DT_User_Role.USER + " = '" + LoginUser + "'").Any())
                                {
                                    haspermission = true;
                                    break;
                                }
                            }
                            RiskPolyLine riskLine = haspermission ? CreateRiskShape(((MainWindow)MyWindow).P.TheCurrentLayout.GridPaintLines, MenuRisk, false, item) : CreateRiskShape(((MainWindow)MyWindow).P.TheCurrentLayout.GridPaintLines, null, false, item);
                            SetPolyLineGroup(riskLine, item);

                            riskLine.Collapsed = (bool)item[DT_Risk.ISCOLLAPSED];
                            if (((bool)item[DT_Risk.ENABLED]))
                            {
                                ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.Enable]).ToolTip = StringResources.DisableValue;

                                riskLine.ShortName = item[DT_Risk.NAMESHORT].ToString();
                                if (haspermission)
                                {
                                    riskLine.SetColor(new SolidColorBrush(Color.FromArgb(drawingCColor.A, drawingCColor.R, drawingCColor.G, drawingCColor.B)));
                                }
                                else
                                {
                                    riskLine.SetColor(new SolidColorBrush(Color.FromArgb(80, drawingCColor.R, drawingCColor.G, drawingCColor.B)));
                                }
                            }
                            else
                            {
                                ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.Enable]).ToolTip = StringResources.EnableValue;
                                riskLine.ShortName = item[DT_Risk.NAMESHORT].ToString();
                                riskLine.SetColor(new SolidColorBrush(Colors.Gray));
                            }
                            riskLine.MyName.AttachDoubleClick(riskLine.MyName, Risk_LabelName_MouseDoubleClick);

                            riskLine.MouseUp += RiskLine_MouseUp;
                            riskLine.MouseDown += R_MouseDown_Event;//click en el Riesgo

                            riskLine.MouseLeave += Risk_MouseLeave;
                            riskLine.MyName.MouseLeave += Risk_MouseLeave;

                            riskLine.MouseEnter += Risk_MouseHover;
                            riskLine.MyName.MouseEnter += Risk_MouseHover;

                            riskLine.MyName.MouseUp += RiskLine_MouseUp;
                            riskLine.MyName.MouseDown += R_MouseDown_Event;//Click en el label

                            riskLine.Expand.MouseLeftButtonDown += Expand_MouseLeftButtonDown;

                            if (Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { riskLine.ID, (Int32)CbFilterTopR.SelectedValue }))
                            {
                                riskLine.OwnValue = (decimal)Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Find(new object[] { riskLine.ID, IdDamageSelected })[DT_Risk_Damages.VALUE];
                            }
                            LinesList.Add(riskLine);
                        }
                    }
                    foreach (DataRow item in Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK_TREE + " = " + ID_Diagram))
                    {
                        DataRow[] dr = Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + item[DT_CounterM.ID].ToString());
                        bool haspermission = false;
                        foreach (DataRow itemii in dr)
                        {
                            if (Ds.Tables[DT_User_Role.TABLE_NAME].Select(DT_User_Role.ROLE + " = '" + itemii[DT_Role_Risk.Role] + "' and " + DT_User_Role.USER + " = '" + LoginUser + "'").Any())
                            {
                                haspermission = true;
                                break;
                            }
                        }
                        RiskPolyLine cmline = haspermission ? CreateCounterMeasureShape(((MainWindow)MyWindow).P.TheCurrentLayout.GridPaintLines, MenuCM, true, item) : CreateCounterMeasureShape(((MainWindow)MyWindow).P.TheCurrentLayout.GridPaintLines, null, true, item);
                        SetPolyLineGroup(cmline, item);

                        if (((bool)item[DT_CounterM.ENABLED]))
                        {
                            ((MenuItem)MenuCM.Items[(int)MenuCMm.Enable]).ToolTip = StringResources.DisableValue;
                            cmline.ShortName = item[DT_CounterM.NAMESHORT].ToString();

                            if (!(haspermission))
                            {
                                cmline.SetColor(new SolidColorBrush(Colors.Gray));
                            }
                        }
                        else
                        {
                            ((MenuItem)MenuCM.Items[(int)MenuCMm.Enable]).ToolTip = StringResources.EnableValue;

                            cmline.ShortName = item[DT_CounterM.NAMESHORT].ToString();
                            cmline.SetColor(new SolidColorBrush(Colors.Gray));
                        }

                        cmline.MouseLeave += Cmline_MouseLeave; ;
                        cmline.MyName.MouseLeave += Cmline_MouseLeave;

                        cmline.MouseEnter += Cmline_MouseEnter; ;
                        cmline.MyName.MouseEnter += Cmline_MouseEnter;

                        //// asignar el label y el clic al mismo controlador de evento
                        cmline.MyName.MouseDown += Cmline_MouseDown;
                        cmline.MouseDown += Cmline_MouseDown;

                        cmline.MyName.AttachDoubleClick(cmline.MyName, CM_LabelName_MouseDoubleClick);

                        LinesList.Add(cmline);
                        LinesListCMState.Add(Convert.ToInt32(cmline.ID), cmline.IsActivated);
                    }

                    TreeOperation.Build_Tree(LinesList);
                    TreeOperation.DrawEntireDiagramAsFishBone(MainLine);
                    FixDrawPanel();
                    UpdateLinesValues();
                }
                SetRightMenu(LinesList);
                foreach (var item in MainLine.Segments)
                {
                    item.ContextMenu = MenuMainRisk;
                }
                SetEventsToSegments();
                Scope();
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        private decimal CalculateOwnValueRisk(int idRisk, int idDamage)
        {
            if (Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { idRisk, idDamage }))
            {
                return (decimal)Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Find(new object[] { idRisk, idDamage })[DT_Risk_Damages.VALUE];
            }
            else
            {
                return 0;
            }
        }

        private decimal CalculateAcumDamageRisk(RiskPolyLine line, int IdDamageSelected)
        {
            decimal AcumDamage = CalculateOwnValueRisk(line.ID, IdDamageSelected) * General.AcumulatedLikelihood(line);
            decimal value = 0;
            if (line.IsActivated)
            {
                foreach (var hijo in line.Children)
                {
                    if (hijo.IsActivated)
                    {
                        if (hijo.IsLeaf())
                        {
                            if (hijo.IsCM)
                            {
                                if (Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Contains(new object[] { hijo.ID, IdDamageSelected }))
                                {
                                    value = (decimal)Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Find(new object[] { hijo.ID, IdDamageSelected })[DT_CounterM_Damage.VALUE];
                                }
                                AcumDamage += value;
                                value = 0;
                            }
                            else
                            {
                                if (Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { hijo.ID, IdDamageSelected }))
                                {
                                    value = (decimal)Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Find(new object[] { hijo.ID, IdDamageSelected })[DT_Risk_Damages.VALUE];
                                }
                                AcumDamage += value * General.AcumulatedLikelihood(hijo);
                                value = 0;
                            }
                        }
                        else
                        {
                            return AcumDamage + CalculateAcumDamageRisk(hijo, IdDamageSelected);
                        }
                    }                    
                }
                return AcumDamage;
            }
            else
            {
                return 0;
            }
        }

        public void UpdateLinesValues()
        {
            foreach (var item in LinesList)
            {
                if (!item.IsCM)
                {
                    item.AcLike = General.AcumulatedLikelihood(item);
                    item.AcValue = General.CalculateTopRiskTreeValue(Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID),
                        Ds.Tables[DT_Risk.TABLE_NAME], IdDamageSelected, Ds.Tables[DT_Risk_Damages.TABLENAME],
                            Ds.Tables[DT_CounterM.TABLE_NAME], Ds.Tables[DT_CounterM_Damage.TABLENAME]);
                    item.OwnValue = CalculateOwnValueRisk(item.ID, IdDamageSelected);
                    item.AcDamage = CalculateAcumDamageRisk(item, IdDamageSelected); ;
                }
            }
        }

        public void SetRightMenu(List<RiskPolyLine> listaComplete)
        {
            List<RiskPolyLine> lista = new List<RiskPolyLine>();
            foreach (var item in listaComplete)
            {
                if (!(item.IsCM) && !item.IsRoot)
                {
                    if (!(FullAccess(item)))
                    {
                        lista.Add(item);
                        item.SetMenu(MenuRiskLimited);
                        item.FullAccess = false;
                    }
                }
            }
            ResetLinesMenu(lista, MenuRiskLimited);
        }

        public bool IsPrimaryInLine(RiskPolyLine line)
        {
            bool access = false;
            if (line.IsCM)
            {
                if (Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + line.ID + " AND " + DT_CM_WBS.IS_PRIMARY + " = 1").Any())
                {
                    if (Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + line.ID + " AND " + DT_CM_WBS.IS_PRIMARY + " = 1").First()[DT_CM_WBS.USERNAME].ToString() == LoginUser)
                    {
                        access = true;
                    }
                }
            }
            else
            {
                if (Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + line.ID + " AND " + DT_RISK_WBS.IS_PRIMARY + " = 1").Any())
                {
                    if (Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + line.ID + " AND " + DT_RISK_WBS.IS_PRIMARY + " = 1").First()[DT_RISK_WBS.USERNAME].ToString() == LoginUser)
                    {
                        access = true;
                    }
                }
            }
            return access;
        }

        public bool BuscarAncestrosWBS(DataSet dsWBS, RiskPolyLine line)
        {
            bool access = false;
            if (line.IsCM)
            {
                foreach (DataRow drCMWBS in Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + line.ID))
                {
                    if (dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + drCMWBS[DT_CM_WBS.ID_WBS]).Any())
                    {
                        if (dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + drCMWBS[DT_CM_WBS.ID_WBS]).Any())
                        {
                            int idPadre = (Int32)dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + drCMWBS[DT_CM_WBS.ID_WBS]).First()[DT_WBS_STRUCTURE.ID_FATHER];
                            if (Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + line.ID).CopyToDataTable().Select(DT_CM_WBS.ID_WBS + " = " + idPadre).Any())
                            {
                                DataRow drreturn = Ds.Tables[DT_CM_WBS.TABLENAME].Select(DT_CM_WBS.ID_CM + " = " + line.ID).CopyToDataTable().Select(DT_CM_WBS.ID_WBS + " = " + idPadre).First();
                                if (drreturn[DT_CM_WBS.USERNAME].ToString() == LoginUser)
                                {
                                    access = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (DataRow drRISKWBS in Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + line.ID))
                {
                    if (dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + drRISKWBS[DT_RISK_WBS.ID_WBS]).Any())
                    {
                        int idPadre = (Int32)dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + drRISKWBS[DT_RISK_WBS.ID_WBS]).First()[DT_WBS_STRUCTURE.ID_FATHER];
                        if (Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + line.ID).CopyToDataTable().Select(DT_RISK_WBS.ID_WBS + " = " + idPadre).Any())
                        {
                            DataRow drreturn = Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + line.ID).CopyToDataTable().Select(DT_RISK_WBS.ID_WBS + " = " + idPadre).First();
                            if (drreturn[DT_RISK_WBS.USERNAME].ToString() == LoginUser)
                            {
                                access = true;
                            }
                        }
                    }
                }
            }
            return access;
        }

        public bool FullAccess(RiskPolyLine line)
        {
            bool access = IsPrimaryInLine(line);
            if (!access)
            {
                ServiceWBS.WebServiceWBS wsWBS = new ServiceWBS.WebServiceWBS();
                DataSet dsWBS = wsWBS.GetAllWBS().Copy();
                wsWBS.Dispose();
                access = BuscarAncestrosWBS(dsWBS, line);
                if (!access)
                {
                    List<RiskPolyLine> ancestors = new List<RiskPolyLine>();
                    BuscarAncestrosRisk(line, ancestors);
                    //Aqui evaluo si el usuario autenticado, es primary en algun nivel superior.
                    foreach (var item in ancestors)
                    {
                        if (Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + item.ID + " AND " + DT_RISK_WBS.IS_PRIMARY + " = 1").Any())
                        {
                            if (Ds.Tables[DT_RISK_WBS.TABLENAME].Select(DT_RISK_WBS.ID_RISK + " = " + item.ID + " AND " + DT_RISK_WBS.IS_PRIMARY + " = 1").First()[DT_RISK_WBS.USERNAME].ToString() == LoginUser)
                            {
                                access = true;
                                break;
                            }
                        }
                    }
                }
            }
            return access;
        }

        public void BuscarAncestrosRisk(RiskPolyLine line, List<RiskPolyLine> ancestors)
        {
            if (line.IdRiskFather != 0 || line.Father != null)
            {
                ancestors.Add(line.Father);
                BuscarAncestrosRisk(line.Father, ancestors);
            }
        }

        private void SetEventsToSegments()
        {
            foreach (var item in LinesList)
            {
                foreach (var segmentLine in item.Segments)
                {
                    if (!item.IsRoot)
                    {
                        segmentLine.MouseLeave += Risk_MouseLeave;
                        segmentLine.MouseEnter += Segment_MouseHover;

                        segmentLine.MouseDown += MouseDown_Event; //click en el segmento
                        segmentLine.MouseUp += RiskLine_MouseUp;
                    }
                    else
                    {
                        segmentLine.MouseDown += PrincipalLine_MouseDown; //click en el segmento
                        segmentLine.MouseUp += MainLine_MouseUp;
                    }
                }
            }
        }

        private static void SetPolyLineGroup(RiskPolyLine polyLine, DataRow item)
        {
            if (item[DT_Risk.ID_GROUPE] != DBNull.Value)
            {
                polyLine.Group = new LineGroup()
                {
                    IdGroup = (int)item[DT_Risk.ID_GROUPE],
                    GroupName = item[DT_Risk.GROUPE_NAME].ToString()
                };
            }
            else
            {
                polyLine.Group = new LineGroup()
                {
                    IdGroup = 0,
                    GroupName = item[DT_Risk.GROUPE_NAME].ToString()
                };
            }
        }

        private RiskPolyLine CreateCounterMeasureShape(Grid gridPaint, ContextMenu contextMenu, bool isCMI, DataRow itemDataRow)
        {
            return new RiskPolyLine(gridPaint, contextMenu, isCMI)
            {
                IsCM = isCMI,
                Position = (int)itemDataRow[DT_CounterM.POSITION],
                ShortName = itemDataRow[DT_CounterM.NAMESHORT].ToString(),
                IdRiskFather = (int)itemDataRow[DT_CounterM.ID_RISK],
                ID = (int)itemDataRow[DT_CounterM.ID],
                Probability = (decimal)itemDataRow[DT_CounterM.PROBABILITY] / 100,
                IsActivated = (bool)itemDataRow[DT_CounterM.ENABLED]
            };
        }

        private RiskPolyLine CreateRiskShape(Grid gridPaint, ContextMenu contextMenu, bool isCMI, DataRow itemDataRow)
        {
            return new RiskPolyLine(gridPaint, contextMenu, isCMI)
            {
                ShortName = itemDataRow[DT_Risk.NAMESHORT].ToString(),
                ID = (int)itemDataRow[DT_Risk.ID],
                Position = (int)itemDataRow[DT_Risk.POSITION],
                Collapsed = (bool)itemDataRow[DT_Risk.ISCOLLAPSED],
                Probability = (decimal)itemDataRow[DT_Risk.PROBABILITY] / 100,
                IsActivated = (bool)itemDataRow[DT_Risk.ENABLED],
                StrokeThickness = 2,
                IsCM = isCMI,
                IdRiskFather = (int)itemDataRow[DT_Risk.IDRISK_FATHER],
                FullAccess = true
            };
        }

        public void DrawEntireDiagram()
        {
            try
            {
                if (Ds.HasChanges())
                {
                    if (ID_Diagram != 0)
                    {
                        CleanFishBone();
                        LoadFishBone();
                        DrawNumbersAndLineThickness();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        #endregion

        #region Agrupando
        public void ResetGroupCMSelection()
        {
            try
            {
                ChoosingCM = false;
                ResetLinesColor(CMGroupSelected);
                ResetLinesMenu(CMGroupSelected, MenuCM);
                CMGroupSelected.Clear();
                if (ChoosingRisk)
                {
                    if (RiskGroupSelected.Count == 1)
                    {
                        ResetLinesMenu(RiskGroupSelected, MenuRisk);
                        SetRightMenu(RiskGroupSelected);
                    }
                    else
                    {
                        ResetLinesMenu(RiskGroupSelected, MenuGroupRisk);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }
        public void ResetGroupRiksSelection()
        {
            try
            {
                ChoosingRisk = false;
                ResetLinesColor(RiskGroupSelected);
                ResetLinesMenu(RiskGroupSelected, MenuRisk);
                SetRightMenu(RiskGroupSelected);
                RiskGroupSelected.Clear();
                if (ChoosingCM)
                {
                    if (CMGroupSelected.Count == 1)
                    {
                        ResetLinesMenu(CMGroupSelected, MenuCM);
                    }
                    else
                    {
                        ResetLinesMenu(CMGroupSelected, MenuGroupCM);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }
        private void ResetLinesColor(List<RiskPolyLine> linesSelected)
        {
            try
            {
                foreach (var item in linesSelected)
                {
                    if (item.IsCM)
                    {
                        if (item.IsActivated)
                        {
                            item.SetColor(new SolidColorBrush(Colors.Black));
                        }
                        else
                        {
                            item.SetColor(new SolidColorBrush(Colors.Gray));
                        }
                    }
                    else
                    {
                        if (item.IsActivated)
                        {
                            System.Drawing.Color drawingCColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram)[CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                            if (TengoPermiso(item))
                            {
                                item.SetColor(new SolidColorBrush(Color.FromArgb(drawingCColor.A, drawingCColor.R, drawingCColor.G, drawingCColor.B)));
                            }
                            else
                            {
                                item.SetColor(new SolidColorBrush(Color.FromArgb(80, drawingCColor.R, drawingCColor.G, drawingCColor.B)));
                            }
                        }
                        else
                        {
                            item.SetColor(new SolidColorBrush(Colors.Gray));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }
        public void ResetLinesMenu(List<RiskPolyLine> lista, ContextMenu menuParam)
        {
            try
            {
                foreach (var item in lista)
                {
                    item.SetMenu(menuParam);
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        #endregion

        #region EventosCM
        private void SelectOneCM(object sender)
        {
            try
            {
                if (sender is RiskPolyLine)
                {
                    Line_Selected = ((RiskPolyLine)sender);
                }
                else
                {
                    Line_Selected = ((LabelPolyLine)sender).Line;
                }
                DataRow[] dr = Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + Line_Selected.ID);
                bool haspermission = false;
                foreach (DataRow item in dr)
                {
                    if (Ds.Tables[DT_User_Role.TABLE_NAME].Select(DT_User_Role.ROLE + " = '" + item[DT_Role_CM.Role] + "' and " + DT_User_Role.USER + " = '" + LoginUser + "'").Any())
                    {
                        haspermission = true;
                        break;
                    }
                }
                if (!haspermission)
                {
                    MenuCM.IsEnabled = false;
                }
                else
                {
                    MenuCM.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        /// <summary>
        /// Trigger when mouse Cursor move over the Line or Label of the CM
        /// </summary>
        private void Cmline_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is RiskPolyLine CMHoover)
            {
                CMEnter(CMHoover, new Point(e.GetPosition(GridPaintLines).X, e.GetPosition(GridPaintLines).Y));
            }
            if (sender is LabelPolyLine CMLabelHoover)
            {
                CMEnter(CMLabelHoover.Line, new Point(e.GetPosition(GridPaintLines).X, e.GetPosition(GridPaintLines).Y));
            }
        }

        public void CMEnter(RiskPolyLine CMLine, Point pointToShowPopup)
        {
            try
            {
                CMLine.StrokeThickness = 3;
                CMLine.StrokeThickness = 3;
                decimal valor;
                if (Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Contains(new object[] { CMLine.ID, (Int32)CbFilterTopR.SelectedValue }))
                {
                    valor = (Decimal)Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Find(new object[] { CMLine.ID, (Int32)CbFilterTopR.SelectedValue })[DT_CounterM_Damage.VALUE];
                }
                else
                {
                    valor = 0;
                }

                string Value = "Value: " + General.MyRound(valor, 4);

                string probability = "Reduction.: " + General.MyRound(CMLine.Probability * 100, 2).ToString() + " %";

                Popin = new Popin(GridPaintLines, pointToShowPopup, "CM: " + CMLine.ShortName, probability, Value)
                {
                    Visibility = Visibility.Visible
                };
                if ((Boolean)Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(CMLine.ID)[DT_CounterM.ENABLED])
                {
                    ((MenuItem)MenuCM.Items[(int)MenuCMm.Enable]).ToolTip = StringResources.DisableValue;
                }
                else
                {
                    ((MenuItem)MenuCM.Items[(int)MenuCMm.Enable]).ToolTip = StringResources.EnableValue;
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }

        }

        /// <summary>
        /// Trigger when mouse cursor leave the CM
        /// </summary>
        private void Cmline_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                if (sender is LabelPolyLine cmLabelLeave)
                {
                    CMLeave(cmLabelLeave.Line);
                }
                if (sender is RiskPolyLine cmLeave)
                {
                    CMLeave(cmLeave);
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        public void CMLeave(RiskPolyLine CMLeave)
        {
            if (CMLeave != null)
            {
                CMLeave.StrokeThickness = 2;
            }

            if (Popin != null)
            {
                Popin.Visibility = Visibility.Collapsed;
                GridPaintLines.Children.Remove(Popin);
                Popin = null;
            }
        }

        private void Cmline_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Loose = false;
                if (MoviendoRisk || MoviendoCM)
                {
                    if (MoviendoRisk)
                    {
                        MoviendoRisk = false;
                        MoviendoCM = false;
                        TreeOperation.DetectarMiPosicionActual(Line_Selected, e.GetPosition(GridPaintLines), Ds);
                        DrawFishBone();
                        ((MainWindow)MyWindow).NormalArrowCursor();
                    }
                    if (MoviendoCM)
                    {
                        MoviendoRisk = false;
                        MoviendoCM = false;
                        DrawFishBone();
                        ((MainWindow)MyWindow).NormalArrowCursor();
                    }
                }
                else
                {
                    // ctrl + click countermeasure
                    if (e.LeftButton == MouseButtonState.Pressed && Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        ChoosingCM = true;
                        SelectOneCM(sender);

                        if (!CMGroupSelected.Contains(Line_Selected))
                        {
                            if (ChoosingRisk)
                            {
                                ResetLinesMenu(RiskGroupSelected, MenuGroupMixed);
                                Line_Selected.SetMenu(MenuGroupMixed);

                            }
                            else
                            {
                                Line_Selected.SetMenu(MenuGroupCM);
                            }
                            Line_Selected.SetColor(new SolidColorBrush(Colors.LightSkyBlue));
                            CMGroupSelected.Add(Line_Selected);
                        }
                        else
                        {
                            Line_Selected.SetMenu(MenuCM);
                            Line_Selected.SetColor(new SolidColorBrush((Line_Selected.IsActivated) ? Colors.Black : Colors.Gray));
                            CMGroupSelected.Remove(Line_Selected);
                            if (CMGroupSelected.Count == 0)
                            {
                                ResetGroupCMSelection();
                            }
                        }
                    }
                    else
                    {
                        if (SelectingToGroup)
                        {
                            if (GroupSelected != null)
                            {
                                if (sender is RiskPolyLine)
                                {
                                    Line_Selected = ((RiskPolyLine)sender);
                                }
                                else
                                {
                                    Line_Selected = ((LabelPolyLine)sender).Line;
                                }
                                Line_Selected.SetColor(new SolidColorBrush(Colors.LightSkyBlue));
                                CMGroupSelected.Add(Line_Selected);
                                Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_CounterM.ID_GROUPE] = GroupSelected.IdGroup;
                                Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_CounterM.GROUPE_NAME] = GroupSelected.GroupName;
                                LinesList.Find(x => x.ID == Line_Selected.ID).Group = GroupSelected;
                                UpdateGridRiskAndGridCM();
                            }
                        }
                        else
                        {
                            switch (e.LeftButton == MouseButtonState.Pressed)
                            {
                                case true: // click izquierdo sin control presionado
                                    if (ChoosingCM || ChoosingRisk)
                                    { // si estaba seleccionando multiple y di click
                                        ResetGroupCMSelection();
                                        ResetGroupRiksSelection();
                                        SelectOneCM(sender);
                                    }
                                    else
                                    {
                                        SelectOneCM(sender);
                                    }
                                    break;
                                case false: // click derecho sin control presionado
                                    SelectOneCM(sender);
                                    if (ChoosingCM || ChoosingRisk) // si estaba seleccionando limpio seleccion si no esta en la seleccion actual
                                    {
                                        if (ChoosingCM && ChoosingRisk) //click derecho en una cm y esta una seleccion mixta activa
                                        {
                                            if (!CMGroupSelected.Contains(Line_Selected)) // si click derecho  una cm que no esta en la seleccion actual
                                            {
                                                ResetGroupCMSelection();
                                                ResetGroupRiksSelection();
                                            }
                                            else
                                            {
                                                var query = from item in RiskGroupSelected
                                                            where (Boolean)item.IsActivated == true
                                                            select item;
                                                List<RiskPolyLine> result = query.ToList<RiskPolyLine>();

                                                var queryCM = from item in CMGroupSelected
                                                              where (Boolean)item.IsActivated == true
                                                              select item;
                                                List<RiskPolyLine> resultCM = queryCM.ToList<RiskPolyLine>();

                                                if (result.Count > 0 || resultCM.Count > 0)
                                                {
                                                    // si hay al menos una "Enabled" envio true, para desactivarlas todas
                                                    ((MenuItem)MenuGroupMixed.Items[(int)MenuGroupCMR.Enable]).ToolTip = StringResources.DisableValue;
                                                }
                                                else
                                                {
                                                    ((MenuItem)MenuGroupMixed.Items[(int)MenuGroupCMR.Enable]).ToolTip = StringResources.EnableValue;
                                                }
                                            }

                                        }
                                        else if (ChoosingCM) //click derecho en una cm y esta una seleccion de cm activa
                                        {
                                            if (!CMGroupSelected.Contains(Line_Selected)) // si click derecho  una cm que no esta en la seleccion actual
                                            {
                                                ResetGroupCMSelection();
                                            }
                                            else
                                            {
                                                var query = from item in CMGroupSelected
                                                            where (Boolean)item.IsActivated == true
                                                            select item;
                                                List<RiskPolyLine> result = query.ToList<RiskPolyLine>();

                                                if (result.Count > 0)
                                                {
                                                    // si hay al menos una "Enabled" envio true, para desactivarlas todas
                                                    ((MenuItem)MenuGroupCM.Items[(int)MenuGroupCMm.Enable]).ToolTip = StringResources.DisableValue;
                                                }
                                                else
                                                {
                                                    ((MenuItem)MenuGroupCM.Items[(int)MenuGroupCMm.Enable]).ToolTip = StringResources.EnableValue;
                                                }
                                            }
                                        }
                                        else //click derecho en una cm y esta una seleccion de riesgos activa
                                        {
                                            ResetGroupRiksSelection(); // limpio lista de riesgos porque di click derecho en una cm
                                        }
                                    }
                                    else
                                    {
                                        SelectOneCM(sender);
                                    }
                                    break;
                            }
                        }
                    }
                }
                UpdateSelectedPolyLineVisualInfo();
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        private void CM_LabelName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (TengoPermiso(Line_Selected) && Line_Selected.IsActivated)
                {
                    NameEditing = true;
                    Loose = true;
                    Line_Selected.ExtrasVisibility(Visibility.Hidden);
                    TextChangeName.Background = new SolidColorBrush(Colors.Black);
                    TextChangeName.Foreground = new SolidColorBrush(Colors.White);
                    TextChangeName.Margin = Line_Selected.TextPanel.Margin;
                    ManageTextChangeProperties(Line_Selected.ShortName, Visibility.Visible);
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        public void EditSelectedPolyLineShorName()
        {
            try
            {
                if (TengoPermiso(Line_Selected) && Line_Selected.IsActivated)
                {
                    NameEditing = true;
                    Loose = true;
                    Line_Selected.MyName.Visibility = Visibility.Hidden;
                    TextChangeName.Margin = Line_Selected.TextPanel.Margin;
                    TextChangeName.AcceptsReturn = false;
                    //TextChangeName.Style = ((TextBox)((MainWindow)MyWindow).FindResource("TextName")).Style;
                    TextChangeName.Background = new SolidColorBrush(Colors.Black);
                    TextChangeName.Foreground = new SolidColorBrush(Colors.White);
                    ManageTextChangeProperties(Line_Selected.ShortName, Visibility.Visible);
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }
        #endregion

        #region EventosRisk
        private void SelectOneRisk(object sender, MouseButtonEventArgs e, int ID_Sender)
        {
            try
            {
                if (sender is RiskPolyLine)
                {
                    Line_Selected = ((RiskPolyLine)sender);
                }
                else if (sender is SegmentPolyLine)
                {
                    Line_Selected = ((SegmentPolyLine)sender).Father;
                }
                else
                {
                    Line_Selected = ((LabelPolyLine)sender).Line;
                }
                PointSelected = e.GetPosition(GridPaintLines);
                //p.LSelected = Line_Selected.ShortName;
                //p.TSelected = Line_Selected.IsCM ? "CounterMeasure" : "Risk";
                DataRow[] dr = Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + ID_Sender);
                bool haspermission = false;
                foreach (DataRow item in dr)
                {
                    if (Ds.Tables[DT_User_Role.TABLE_NAME].Select(DT_User_Role.ROLE + " = '" + item[DT_Role_Risk.Role] + "' and " + DT_User_Role.USER + " = '" + LoginUser + "'").Any())
                    {
                        haspermission = true;
                        break;
                    }
                }
                if (!haspermission)
                {
                    Line_Selected.ContextMenu = null;
                    foreach (SegmentPolyLine sItem in Line_Selected.Segments)
                    {
                        sItem.ContextMenu = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        private void CreateRisk()
        {
            if (Line_Created.IsCM)
            {
                WindowCM windowCM = new WindowCM()
                {
                    ID_Project = (int)Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.ID_PROJECT],
                    CMRow = Ds.Tables[DT_CounterM.TABLE_NAME].NewRow(),
                    DsCM = Ds,
                    LOGIN_USER = LoginUser,
                    CM_RoleTable = Ds.Tables[DT_Role_CM.TABLENAME].Copy(),
                    CM_WBS_Table = Ds.Tables[DT_CM_WBS.TABLENAME].Copy(),
                    WBS_CM_Damage = Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME].Copy(),
                    TopRiskTable = Ds.Tables[DT_CounterM_Damage.TABLENAME].Copy(),
                    Operation = General.INSERT,
                    RowFather = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID),
                    RiskTreeID = ID_Diagram,
                    RiskPadre = Line_Selected,
                    MyCM = Ds.Tables[DT_CounterM.TABLE_NAME].Copy()
                };
                windowCM.Pi.HasAccess = true;
                windowCM.Probability = 0;
                if (windowCM.ShowDialog() == true)
                {
                    Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Add(windowCM.CMRow);
                    Ds.Tables[DT_CounterM_Damage.TABLENAME].Merge(windowCM.TopRiskTable);
                    Ds.Tables[DT_CM_WBS.TABLENAME].Merge(windowCM.CM_WBS_Table);
                    Ds.Tables[DT_Role_CM.TABLENAME].Merge(windowCM.CM_RoleTable);

                    Line_Created.ID = (int)windowCM.CMRow[DT_CounterM.ID];

                    Line_Created.Father = Line_Selected;
                    Line_Created.IdRiskFather = Line_Selected.ID;
                    //Line_Selected.Children.Add(Line_Created);
                    Line_Created.FromTop = Line_Selected.FromTop;
                    Line_Created.IsDiagonal = !Line_Selected.IsDiagonal;
                    Line_Created.ShortName = windowCM.CMRow[DT_CounterM.NAMESHORT].ToString();
                    InsertCM(Line_Created, Line_Selected, Line_Created.Points[1]);
                    DrawFishBone();
                }
            }
            else
            {
                WindowRisk wrisk = new WindowRisk()
                {
                    RiskRow = Ds.Tables[DT_Risk.TABLE_NAME].NewRow(),
                    ID_PROJECT = (int)Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.ID_PROJECT],
                    Ds = Ds,
                    Risk_RoleTable = Ds.Tables[DT_Role_Risk.TABLENAME],
                    Risk_DamageTable = Ds.Tables[DT_Risk_Damages.TABLENAME],
                    Risk_WBS_Table = Ds.Tables[DT_RISK_WBS.TABLENAME],
                    Operation = General.INSERT,
                    RowFather = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID),
                    RiskTreeID = (int)Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.ID_DIAGRAM],
                    RiskSelected = Line_Selected,
                    MyRisks = Ds.Tables[DT_Risk.TABLE_NAME].Copy(),
                    LOGIN_USER = LoginUser,
                    WBS_RISK_Damage = Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME],
                    WBS_CM_Damage = Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME],
                    CM_DamageTable = Ds.Tables[DT_CounterM_Damage.TABLENAME]
                };
                wrisk.Pi.HasAccess = true;
                if (wrisk.ShowDialog() == true)
                {
                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(wrisk.RiskRow);
                    DataRow rowstructure = Ds.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                    rowstructure[DT_RiskStructure.IDRISK] = wrisk.RiskRow[DT_Risk.ID];
                    rowstructure[DT_RiskStructure.IDRISK_FATHER] = wrisk.RowFather[DT_Risk.ID];
                    Ds.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);
                    Line_Created.ID = (int)wrisk.RiskRow[DT_Risk.ID];
                    Line_Created.Father = Line_Selected;
                    Line_Created.IdRiskFather = Line_Selected.ID;
                    Line_Created.FromTop = Line_Selected.FromTop;
                    Line_Created.IsDiagonal = !Line_Selected.IsDiagonal;
                    Line_Created.ShortName = wrisk.RiskRow[DT_Risk.NAMESHORT].ToString();
                    InsertRisk(Line_Created, Line_Selected, Line_Created.Points[1]);
                    DrawFishBone();
                }
            }
        }

        private void MouseDown_Event(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Loose = false;
                int ID_Sender;
                bool IsRoot_Sender;
                RiskPolyLine TheLine;

                ID_Sender = ((SegmentPolyLine)sender).Father.ID;
                IsRoot_Sender = ((SegmentPolyLine)sender).Father.IsRoot;
                TheLine = ((SegmentPolyLine)sender).Father;

                MouseDownPress(sender, e, ID_Sender, TheLine);
            }
            catch (Exception ex)
            {
                MostrarDialog("S_MouseDown_Event: " + ex.Message);
            }
        }
        private void R_MouseDown_Event(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Loose = false;
                int ID_Sender;
                bool IsRoot_Sender;
                RiskPolyLine TheLine;
                if (sender is RiskPolyLine)
                {
                    ID_Sender = ((RiskPolyLine)sender).ID;
                    IsRoot_Sender = ((RiskPolyLine)sender).IsRoot;
                    TheLine = (RiskPolyLine)sender;
                }
                else
                {
                    ID_Sender = ((LabelPolyLine)sender).Line.ID;
                    IsRoot_Sender = ((LabelPolyLine)sender).Line.IsRoot;
                    TheLine = ((LabelPolyLine)sender).Line;
                }
                MouseDownPress(sender, e, ID_Sender, TheLine);
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        private void MouseDownPress(object sender, MouseButtonEventArgs e, int ID_Sender, RiskPolyLine TheLine)
        {
            if (Creando)
            {
                SelectOneRisk(sender, e, ID_Sender);
                if (FullAccess(Line_Selected))
                {
                    CreateRisk();
                }
                else
                {
                    MostrarDialog("No Access Granted to do this Operation");
                }

                GridPaintLines.Children.Remove(Line_Created);
                Line_Created = null;
                Creando = false;
                Loose = true;
            }
            else
            {
                if (MoviendoRisk)
                {
                    if (Line_Selected.IdRiskFather != ID_Sender && Line_Selected.ID != ID_Sender)
                    {
                        MoviendoRisk = false;
                        MoveRisk(TheLine, e.GetPosition(GridPaintLines));
                    }
                    else
                    {
                        MoviendoRisk = false;
                        ReorderRisk(TheLine, e.GetPosition(GridPaintLines));
                    }
                    DrawFishBone();
                    ((MainWindow)MyWindow).NormalArrowCursor();
                }
                else
                {
                    if (MoviendoCM)
                    {
                        if (Line_Selected.IdRiskFather != ID_Sender && Line_Selected.ID != ID_Sender)
                        {
                            MoviendoCM = false;
                            MoveCounterMeasure(TheLine, e.GetPosition(GridPaintLines));
                            DrawFishBone();
                            ((MainWindow)MyWindow).NormalArrowCursor();
                        }
                        else
                        {
                            MoviendoCM = false;
                            ReorderCounterMeasure(TheLine, e.GetPosition(GridPaintLines));
                            DrawFishBone();
                            ((MainWindow)MyWindow).NormalArrowCursor();
                        }
                    }
                    else
                    {
                        if (e.LeftButton == MouseButtonState.Pressed && Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            // ctrl + click risk
                            ChoosingRisk = true;
                            SelectOneRisk(sender, e, ID_Sender);

                            if (!RiskGroupSelected.Contains(Line_Selected))
                            {
                                if (TengoPermiso(Line_Selected) && FullAccess(Line_Selected))
                                {
                                    if (ChoosingCM)
                                    {
                                        ResetLinesMenu(CMGroupSelected, MenuGroupMixed);
                                        Line_Selected.SetMenu(MenuGroupMixed);
                                    }
                                    else
                                    {
                                        Line_Selected.SetMenu(MenuGroupRisk);
                                    }
                                    Line_Selected.SetColor(new SolidColorBrush(Colors.LightSkyBlue));
                                    RiskGroupSelected.Add(Line_Selected);
                                }
                            }
                            else
                            {
                                System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_DAMAGE + " = " + (Int32)CbFilterTopR.SelectedValue).First()[DT_Risk_Damages.COLOR].ToString()));
                                if (TengoPermiso(Line_Selected) && FullAccess(Line_Selected))
                                {
                                    Line_Selected.SetMenu(MenuRisk);
                                    Line_Selected.FullAccess = true;
                                    Line_Selected.SetColor(new SolidColorBrush(Color.FromArgb(drawColor.A, drawColor.R, drawColor.G, drawColor.B)));
                                    SetRightMenu(new List<RiskPolyLine>() { Line_Selected });
                                }
                                else
                                {
                                    Line_Selected.SetColor(new SolidColorBrush(Color.FromArgb(80, drawColor.R, drawColor.G, drawColor.B)));
                                    Line_Selected.SetMenu(null);
                                }

                                RiskGroupSelected.Remove(Line_Selected);

                                if (RiskGroupSelected.Count == 0)
                                {
                                    ResetGroupRiksSelection();
                                }
                            }
                        }
                        else
                        {
                            if (SelectingToGroup)
                            {
                                if (GroupSelected != null)
                                {
                                    if (sender is RiskPolyLine)
                                    {
                                        Line_Selected = ((RiskPolyLine)sender);
                                    }
                                    else
                                    {
                                        Line_Selected = ((LabelPolyLine)sender).Line;
                                    }
                                    Line_Selected.SetColor(new SolidColorBrush(Colors.LightSkyBlue));
                                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.ID_GROUPE] = GroupSelected.IdGroup;
                                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.GROUPE_NAME] = GroupSelected.GroupName;
                                    LinesList.Find(x => x.ID == Line_Selected.ID).Group = GroupSelected;
                                    RiskGroupSelected.Add(Line_Selected);
                                    UpdateGridRiskAndGridCM();
                                }
                            }
                            else
                            {
                                switch (e.LeftButton == MouseButtonState.Pressed)
                                {
                                    case true: // click izquierdo sin control presionado
                                        SelectOneRisk(sender, e, ID_Sender);
                                        if (ChoosingCM || ChoosingRisk)
                                        { // si estaba seleccionando multiple y di click izq
                                            ResetGroupCMSelection();
                                            ResetGroupRiksSelection();
                                        }
                                        break;

                                    case false: // click derecho sin control presionado en un risk
                                        SelectOneRisk(sender, e, ID_Sender);
                                        if (ChoosingCM || ChoosingRisk) // si estaba seleccionando limpio seleccion
                                        {
                                            LimiparSeleccionAll();
                                        }
                                        break;
                                }
                            }
                        }

                    }
                }
            }
            UpdateSelectedPolyLineVisualInfo();
        }

        public void LimiparSeleccionAll()
        {
            if (ChoosingCM && ChoosingRisk) //click derecho en un riesgo y esta una seleccion mixta activa
            {
                if (!RiskGroupSelected.Contains(Line_Selected)) // si click derecho en un riesgo que no esta en la seleccion actual
                {
                    ResetGroupCMSelection();
                    ResetGroupRiksSelection();
                }
                else
                {
                    var query = from item in RiskGroupSelected
                                where (Boolean)item.IsActivated == true
                                select item;
                    List<RiskPolyLine> result = query.ToList<RiskPolyLine>();

                    var queryCM = from item in CMGroupSelected
                                  where (Boolean)item.IsActivated == true
                                  select item;
                    List<RiskPolyLine> resultCM = queryCM.ToList<RiskPolyLine>();

                    if (result.Count > 0 || resultCM.Count > 0)
                    {
                        // si hay al menos una "Enabled" envio true, para desactivarlas todas
                        ((MenuItem)MenuGroupMixed.Items[(int)MenuGroupCMR.Enable]).ToolTip = StringResources.DisableValue;
                    }
                    else
                    {
                        ((MenuItem)MenuGroupMixed.Items[(int)MenuGroupCMR.Enable]).ToolTip = StringResources.EnableValue;
                    }
                }
            }
            else if (ChoosingCM) //click derecho en un riesgo y esta una seleccion de cm activa
            {
                ResetGroupCMSelection(); // limpio lista de cm porque di click derecho en un riesgo

            }
            else //click derecho en un riesgo  y esta una seleccion de riesgos activa
            {
                if (!RiskGroupSelected.Contains(Line_Selected))
                {
                    ResetGroupRiksSelection();
                }
                else
                {
                    var query = from item in RiskGroupSelected
                                where (Boolean)item.IsActivated == true
                                select item;
                    List<RiskPolyLine> result = query.ToList<RiskPolyLine>();

                    if (result.Count > 0)
                    {
                        // si hay al menos una "Enabled" envio true, para desactivarlas todas
                        ((MenuItem)MenuGroupRisk.Items[(int)MenuGroupR.Enable]).ToolTip = StringResources.DisableValue;
                    }
                    else
                    {
                        ((MenuItem)MenuGroupRisk.Items[(int)MenuGroupR.Enable]).ToolTip = StringResources.EnableValue;
                    }
                }
            }
        }
        public void UpdateSelectedPolyLineVisualInfo()
        {
            ((MainWindow)MyWindow).P.LSelected = Line_Selected.ShortName;
            ((MainWindow)MyWindow).P.TSelected = Line_Selected.IsCM ? "Counter Measure" : "Risk";
        }

        private void Risk_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                RiskLeave();
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        public void RiskLeave()
        {
            SetLinesThickness();
            if (Creando)
            {
                Line_Created.IsDiagonal = false;
                Line_Created.NewDrawAtPoint(Line_Created.StartDrawPoint);
            }
            if (Popin != null)
            {
                GridPaintLines.Children.Remove(Popin);
                Popin.Visibility = Visibility.Collapsed;
                Popin = null;
            }
        }

        private void Risk_MouseHover(object sender, MouseEventArgs e)
        {
            RiskPolyLine TheLine;
            if (sender is LabelPolyLine)
            {
                TheLine = ((LabelPolyLine)sender).Line;
            }
            else
            {
                TheLine = (RiskPolyLine)sender;
            }
            SetLineThickness(TheLine);
            RiskEnter(TheLine, new Point(e.GetPosition(GridPaintLines).X, e.GetPosition(GridPaintLines).Y));
        }

        private void Segment_MouseHover(object sender, MouseEventArgs e)
        {
            SegmentPolyLine TheLine;
            TheLine = (SegmentPolyLine)sender;
            SetLineThickness(TheLine.Father);
            RiskEnter(TheLine.Father, new Point(e.GetPosition(GridPaintLines).X, e.GetPosition(GridPaintLines).Y));
        }

        private void SetLineThickness(RiskPolyLine TheLine)
        {
            if (TheLine != null)
            {
                TheLine.StrokeThickness = General.MaxThickness;
                // se recorre la lista de todos los hermanos, incluyendose el que se genero el evento.
                foreach (SegmentPolyLine segmentLine in TheLine.Segments)
                {
                    segmentLine.StrokeThickness = General.MaxThickness;
                }
            }
        }

        public void RiskEnter(RiskPolyLine TheLine, Point pointToShowPopup)
        {
            try
            {
                if (!(MoviendoRisk || MoviendoCM))
                {
                    if (Creando)
                    {
                        Line_Created.IsDiagonal = !TheLine.IsDiagonal;
                        Line_Created.FromTop = TheLine.FromTop;
                        Line_Created.DrawSingleLine();
                    }
                    else
                    {
                        decimal el = TheLine.AcLike;
                        decimal valor = TheLine.AcValue;
                        string AcumValue = StringResources.ACUM_VALUE + General.MyRound(valor, 4);
                        string Valuee = "";
                        Valuee = StringResources.VALUE + General.MyRound(TheLine.OwnValue, 2).ToString();
                        string ED = StringResources.ACUM_DAMAGE + General.MyRound(TheLine.AcDamage, 4);
                        string probability = StringResources.PROBABILITY + General.MyRound(TheLine.Probability * 100, 2).ToString() + " %";
                        string EL = StringResources.ACUM_LIKELIHOOD + General.MyRound(el * 100, 2).ToString() + " %";
                        Popin = new Popin(GridPaintLines, pointToShowPopup, "Risk: " + TheLine.ShortName, probability, EL, Valuee, AcumValue, ED)
                        {
                            Visibility = Visibility.Visible
                        };
                        if ((bool)Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheLine.ID)[DT_Risk.ENABLED])
                        {
                            ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.Enable]).ToolTip = StringResources.DisableValue;
                        }
                        else
                        {
                            ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.Enable]).ToolTip = StringResources.EnableValue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }

        }

        private void Risk_LabelName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (TengoPermiso(Line_Selected) && Line_Selected.IsActivated && FullAccess(Line_Selected))
                {
                    NameEditing = true;
                    Loose = true;
                    Line_Selected.ExtrasVisibility(Visibility.Hidden);
                    TextChangeName.Background = new SolidColorBrush(Colors.Black);
                    TextChangeName.Foreground = new SolidColorBrush(Colors.White);
                    TextChangeName.Margin = Line_Selected.TextPanel.Margin;
                    ManageTextChangeProperties(Line_Selected.ShortName, Visibility.Visible);
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        private void RiskLine_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Loose = true;
                RiskPolyLine riskPolyLine;
                if (sender is RiskPolyLine)
                {
                    riskPolyLine = (RiskPolyLine)sender;
                }
                else if (sender is SegmentPolyLine)
                {
                    riskPolyLine = ((SegmentPolyLine)sender).Father;
                }
                else
                {
                    riskPolyLine = ((LabelPolyLine)sender).Line;
                }
                if (MoviendoRisk)
                {
                    // Se mueve un riesgo

                    //checking if user don't bad click out of line for moving
                    //chekea que el usuario no de un mal clic 
                    if (Line_Selected.IdRiskFather != riskPolyLine.ID && Line_Selected.ID != riskPolyLine.ID)
                    {
                        //Mover el riesgo de un Padre a otro (cambiar de padre)
                        MoviendoRisk = false;
                        MoveRisk(riskPolyLine, e.GetPosition(GridPaintLines));
                    }
                    else
                    {
                        //Mover el riesgo en el mismo Padre (Cambiar la posición, reordenar)
                        MoviendoRisk = false;
                        ReorderRisk(riskPolyLine, e.GetPosition(GridPaintLines));
                    }
                    DrawFishBone();
                }
                else
                {
                    if (MoviendoCM)
                    {
                        // Se mueve una contramedida

                        if (Line_Selected.IdRiskFather != riskPolyLine.ID && Line_Selected.ID != riskPolyLine.ID)
                        {
                            //Mover la contramedida de un Padre a otro (cambiar de padre)
                            MoviendoCM = false;
                            MoveCounterMeasure(riskPolyLine, e.GetPosition(GridPaintLines));
                        }
                        else
                        {
                            //Mover la contramedida en el mismo Padre (Cambiar la posición, reordenar)
                            MoviendoCM = false;
                            ReorderCounterMeasure(riskPolyLine, e.GetPosition(GridPaintLines));
                        }
                        DrawFishBone();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        public void InsertRisk(RiskPolyLine insertedRisk, RiskPolyLine destinationPolyLine, Point point)
        {
            int pos = TreeOperation.DetectClickPosition(point, destinationPolyLine);
            int lastCounterMeasurePosition = TreeOperation.LastCounterMeasurePosition(destinationPolyLine.Children);
            if (pos <= lastCounterMeasurePosition)
            {
                pos = lastCounterMeasurePosition + 1;
            }

            //Insertar el Risk en su nuevo padre (el PolyLine destino)
            destinationPolyLine.Children.Insert(pos, insertedRisk);
            SetPolyLinePosition(destinationPolyLine.Children);
        }
        private void MoveRisk(RiskPolyLine destinationPolyLine, Point point)
        {
            if (FullAccess(destinationPolyLine))
            {

                //Eliminar el Risk que se mueve de su padre
                Line_Selected.Father.Children.Remove(Line_Selected);

                //Reestablecer la posición de los PolyLine en su padre
                SetPolyLinePosition(Line_Selected.Father.Children);

                //Obtener posición en que debe insertarse el Risk dentro del PolyLine destino (su nuevo padre)
                int pos = TreeOperation.DetectClickPosition(point, destinationPolyLine);
                int lastCounterMeasurePosition = TreeOperation.LastCounterMeasurePosition(destinationPolyLine.Children);
                if (pos <= lastCounterMeasurePosition)
                {
                    pos = lastCounterMeasurePosition + 1;
                }

                //Insertar el Risk en su nuevo padre (el PolyLine destino)
                destinationPolyLine.Children.Insert(pos, Line_Selected);

                //Actualizar el padre al Riesk
                Line_Selected.Father = destinationPolyLine;
                Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.IDRISK_FATHER] = destinationPolyLine.ID;
                Ds.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + Line_Selected.ID).First()[DT_RiskStructure.IDRISK_FATHER] = destinationPolyLine.ID;

                //Reestablecer la posición de los PolyLine en su nuevo padre
                SetPolyLinePosition(destinationPolyLine.Children);

                DataRow[] drRoleRisk = Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + destinationPolyLine.ID);
                foreach (DataRow item in drRoleRisk)
                {
                    foreach (RiskPolyLine itemRiskMoving in LinesMoving)
                    {
                        if (itemRiskMoving.IsCM)
                        {
                            if (!(Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + itemRiskMoving.ID + " AND " + DT_Role_CM.Role + " = '" + item[DT_Role_Risk.Role].ToString() + "'").Any()))
                            {
                                DataRow drRoleCM = Ds.Tables[DT_Role_CM.TABLENAME].NewRow();
                                drRoleCM[DT_Role_CM.ID_CM] = itemRiskMoving.ID;
                                drRoleCM[DT_Role_CM.NAME_SHORT] = itemRiskMoving.ShortName;
                                drRoleCM[DT_Role_CM.Role] = item[DT_Role_Risk.Role];
                                drRoleCM[DT_Role_CM.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                                Ds.Tables[DT_Role_CM.TABLENAME].Rows.Add(drRoleCM);
                            }
                        }
                        else
                        {
                            if (!(Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + itemRiskMoving.ID + " AND " + DT_Role_Risk.Role + " = '" + item[DT_Role_Risk.Role].ToString() + "'").Any()))
                            {
                                DataRow drRoleRiskk = Ds.Tables[DT_Role_Risk.TABLENAME].NewRow();
                                drRoleRiskk[DT_Role_Risk.ID_RISK] = itemRiskMoving.ID;
                                drRoleRiskk[DT_Role_Risk.NAME_SHORT] = itemRiskMoving.ShortName;
                                drRoleRiskk[DT_Role_Risk.Role] = item[DT_Role_Risk.Role];
                                drRoleRiskk[DT_Role_Risk.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                                Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRoleRiskk);
                            }
                        }
                    }
                }
                RiskPolyLine linetoDel = new RiskPolyLine
                {
                    ID = Line_Selected.ID,
                    IsCM = Line_Selected.IsCM
                };

                TreeOperation.CreateCopyOfLine(Line_Selected, destinationPolyLine.ID, Ds);
                TreeOperation.DeleteLine(linetoDel, Ds);

                GridPaintLines.Children.Remove(LineInMoving);
                LineInMoving = null;
            }
        }

        public void InsertCM(RiskPolyLine insertedCM, RiskPolyLine destinationPolyLine, Point point)
        {
            int pos = TreeOperation.DetectClickPosition(point, destinationPolyLine);
            int lastCounterMeasurePosition = TreeOperation.LastCounterMeasurePosition(destinationPolyLine.Children);
            if (pos > lastCounterMeasurePosition)
            {
                pos = lastCounterMeasurePosition + 1;
            }

            //Insertar la CM en su nuevo padre (el PolyLine destino)
            destinationPolyLine.Children.Insert(pos, insertedCM);
            SetPolyLinePosition(destinationPolyLine.Children);
        }

        private void MoveCounterMeasure(RiskPolyLine destinationPolyLine, Point point)
        {
            if (FullAccess(destinationPolyLine))
            {
                //Eliminar la CM que se mueve de su padre
                Line_Selected.Father.Children.Remove(Line_Selected);

                //Reestablecer la posición de los PolyLine en su padre
                SetPolyLinePosition(Line_Selected.Father.Children);

                //Obtener posición en que debe insertarse la CM dentro del PolyLine destino (su nuevo padre)
                int pos = TreeOperation.DetectClickPosition(point, destinationPolyLine);
                int lastCounterMeasurePosition = TreeOperation.LastCounterMeasurePosition(destinationPolyLine.Children);
                if (pos > lastCounterMeasurePosition)
                {
                    pos = lastCounterMeasurePosition + 1;
                }

                //Insertar la CM en su nuevo padre (el PolyLine destino)
                destinationPolyLine.Children.Insert(pos, Line_Selected);

                //Actualizar el padre al CM
                Line_Selected.Father = destinationPolyLine;
                Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_CounterM.ID_RISK] = destinationPolyLine.ID;

                //Reestablecer la posición de los PolyLine en su nuevo padre
                SetPolyLinePosition(destinationPolyLine.Children);

                DataRow[] drRoleRisk = Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + destinationPolyLine.ID);
                foreach (DataRow item in drRoleRisk)
                {
                    foreach (RiskPolyLine itemRiskMoving in LinesMoving)
                    {
                        if (!(Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + itemRiskMoving.ID + " AND " + DT_Role_CM.Role + " = '" + item[DT_Role_Risk.Role].ToString() + "'").Any()))
                        {
                            DataRow drRole = Ds.Tables[DT_Role_CM.TABLENAME].NewRow();
                            drRole[DT_Role_CM.ID_CM] = itemRiskMoving.ID;
                            drRole[DT_Role_CM.NAME_SHORT] = itemRiskMoving.ShortName;
                            drRole[DT_Role_CM.Role] = item[DT_Role_Risk.Role];
                            drRole[DT_Role_CM.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                            Ds.Tables[DT_Role_CM.TABLENAME].Rows.Add(drRole);
                        }
                    }
                }
                RiskPolyLine linetoDel = new RiskPolyLine
                {
                    ID = Line_Selected.ID,
                    IsCM = Line_Selected.IsCM
                };
                TreeOperation.CreateCopyOfLine(Line_Selected, destinationPolyLine.ID, Ds);
                TreeOperation.DeleteLine(linetoDel, Ds);
            }
        }

        /// <summary>
        /// Reorder risk into same father
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReorderRisk(RiskPolyLine risk, Point point)
        {
            risk.Children.Remove(Line_Selected);
            SetPolyLinePosition(risk.Children);

            int pos = TreeOperation.DetectClickPosition(point, risk);
            int lastCounterMeasurePosition = TreeOperation.LastCounterMeasurePosition(risk.Children);
            if (pos <= lastCounterMeasurePosition)
            {
                pos = lastCounterMeasurePosition + 1;
            }
            GridPaintLines.Children.Remove(LineInMoving);
            risk.Children.Insert(pos, Line_Selected);
            SetPolyLinePosition(risk.Children);
        }

        /// <summary>
        /// Reorder risk into same father
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReorderCounterMeasure(RiskPolyLine risk, Point point)
        {
            risk.Children.Remove(Line_Selected);
            SetPolyLinePosition(risk.Children);

            int pos = TreeOperation.DetectClickPosition(point, risk);
            int lastCounterMeasurePosition = TreeOperation.LastCounterMeasurePosition(risk.Children);
            if (pos > lastCounterMeasurePosition)
            {
                pos = lastCounterMeasurePosition + 1;
            }

            risk.Children.Insert(pos, Line_Selected);
            SetPolyLinePosition(risk.Children);
        }

        public void DrawFishBone()
        {
            CleanFishBone();
            LoadFishBone();
            DrawNumbersAndLineThickness();
            ((MainWindow)MyWindow).TextProbabilityChange(MainLine);
            UpdateGridRiskAndGridCM();
        }

        private void CleanFishBone()
        {
            DropLines();
            DropRectangles();
        }

        private void LoadFishBone()
        {
            LoadLines();
            LoadRectangles();
        }

        private void DrawNumbersAndLineThickness()
        {
            DrawNumbers();
            SetLinesThickness();
        }

        private void UpdateGridRiskAndGridCM()
        {
            ((MainWindow)MyWindow).CruzarTablaRisk(Ds);
            ((MainWindow)MyWindow).CruzarTablaCM(Ds);
        }

        public void SetPolyLinePosition(List<RiskPolyLine> Children)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].IsCM)
                {
                    Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Children[i].ID)[DT_CounterM.POSITION] = i;
                }
                else
                {
                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Children[i].ID)[DT_Risk.POSITION] = i;
                }
                Children[i].Position = i;
            }
        }

        private void Expand_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (((PictureBoxPolyLine)sender).Risk.Collapsed)
                {
                    ((PictureBoxPolyLine)sender).Risk.Collapsed = false;
                }
                else
                {
                    ((PictureBoxPolyLine)sender).Risk.Collapsed = true;
                }
                Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(((PictureBoxPolyLine)sender).Risk.ID)[DT_Risk.ISCOLLAPSED] = ((PictureBoxPolyLine)sender).Risk.Collapsed;

                CleanFishBone();
                LoadFishBone();
                DrawNumbersAndLineThickness();
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        #endregion

        #region EventosMainLine
        private void PrincipalLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!MoviendoCM)
                {
                    MenuRisk.IsEnabled = true;
                    int ID_Sender;
                    bool IsRoot_Sender;
                    RiskPolyLine TheLine;
                    if (sender is RiskPolyLine)
                    {
                        ID_Sender = ((RiskPolyLine)sender).ID;
                        IsRoot_Sender = ((RiskPolyLine)sender).IsRoot;
                        TheLine = (RiskPolyLine)sender;
                    }
                    else
                    {
                        ID_Sender = ((SegmentPolyLine)sender).Father.ID;
                        IsRoot_Sender = ((SegmentPolyLine)sender).Father.IsRoot;
                        TheLine = ((SegmentPolyLine)sender).Father;
                    }
                    if (Creando)
                    {
                        Loose = false;
                        SelectOneRisk(sender, e, ID_Sender);
                        if (!(Line_Created.IsCM))
                        {
                            WindowRisk wrisk = new WindowRisk()
                            {
                                RiskRow = Ds.Tables[DT_Risk.TABLE_NAME].NewRow(),
                                Ds = Ds,
                                LOGIN_USER = LoginUser,
                                ID_PROJECT = (int)Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.ID_PROJECT],
                                Risk_RoleTable = Ds.Tables[DT_Role_Risk.TABLENAME],
                                WBS_RISK_Damage = Ds.Tables[DT_WBS_RISK_DAMAGE.TABLE_NAME],
                                WBS_CM_Damage = Ds.Tables[DT_WBS_CM_Damage.TABLE_NAME],
                                Risk_WBS_Table = Ds.Tables[DT_RISK_WBS.TABLENAME],
                                Risk_DamageTable = Ds.Tables[DT_Risk_Damages.TABLENAME],
                                CM_DamageTable = Ds.Tables[DT_CounterM_Damage.TABLENAME],
                                Operation = General.INSERT,
                                RowFather = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID),
                                RiskTreeID = ID_Diagram,
                                RiskSelected = Line_Selected,
                                MyRisks = Ds.Tables[DT_Risk.TABLE_NAME].Copy()
                            };
                            wrisk.Pi.HasAccess = true;
                            if (wrisk.ShowDialog() == true)
                            {
                                Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(wrisk.RiskRow);
                                DataRow rowstructure = Ds.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                                rowstructure[DT_RiskStructure.IDRISK] = wrisk.RiskRow[DT_Risk.ID];
                                rowstructure[DT_RiskStructure.IDRISK_FATHER] = wrisk.RowFather[DT_Risk.ID];
                                Ds.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);
                                RiskPolyLine Line_Created = new RiskPolyLine
                                {
                                    ID = (int)wrisk.RiskRow[DT_Risk.ID],
                                    IsCM = false,
                                    ShortName = "LineCreated",
                                    Father = Line_Selected,
                                    IdRiskFather = Line_Selected.ID
                                };
                                InsertRisk(Line_Created, Line_Selected, PointSelected);

                                DropLines();
                                DropRectangles();
                                LoadLines();
                                LoadRectangles();
                                DrawNumbers();
                                SetLinesThickness();
                                ((MainWindow)MyWindow).TextProbabilityChange(MainLine);
                                ((MainWindow)MyWindow).CruzarTablaRisk(Ds);
                            }
                        }
                        GridPaintLines.Children.Remove(Line_Created);
                        Line_Created = null;
                        Creando = false;
                        Loose = true;
                    }
                    else
                    {
                        if (MoviendoRisk)
                        {
                            if (Line_Selected.IdRiskFather != ID_Sender && Line_Selected.ID != ID_Sender)
                            {
                                MoviendoRisk = false;
                                if (Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains(Line_Selected.ID))
                                {
                                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.IDRISK_FATHER] = ID_Sender;
                                }
                                Ds.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + Line_Selected.ID).First()[DT_RiskStructure.IDRISK_FATHER] = ID_Sender;
                                foreach (var item in LinesList.FindAll(rl => rl.IdRiskFather == Line_Selected.IdRiskFather && rl.Position > Line_Selected.Position).ToList())
                                {
                                    item.Position -= 1;
                                    if (Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains(item.ID))
                                    {
                                        Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.POSITION] = item.Position;
                                    }
                                    if (Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Contains(item.ID))
                                    {
                                        Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.POSITION] = item.Position;
                                    }
                                }
                                Line_Selected.Position = LinesList.Find(x => x.ID == ID_Sender).Children.Count;

                                Line_Selected.Father = LinesList.Find(x => x.ID == ID_Sender);

                                Line_Selected.Father.Children.Add(Line_Selected);

                                Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.POSITION] = Line_Selected.Position;
                                DataRow[] drRoleRisk = Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + ID_Sender);
                                foreach (DataRow item in drRoleRisk)
                                {
                                    foreach (RiskPolyLine itemRisk in LinesMoving)
                                    {
                                        if (itemRisk.IsCM)
                                        {
                                            if (!(Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + itemRisk.ID + " AND " + DT_Role_CM.Role + " = '" + item[DT_Role_Risk.Role].ToString() + "'").Any()))
                                            {
                                                DataRow drRoleCM = Ds.Tables[DT_Role_CM.TABLENAME].NewRow();
                                                drRoleCM[DT_Role_CM.ID_CM] = itemRisk.ID;
                                                drRoleCM[DT_Role_CM.NAME_SHORT] = itemRisk.ShortName;
                                                drRoleCM[DT_Role_CM.Role] = item[DT_Role_Risk.Role];
                                                drRoleCM[DT_Role_CM.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                                                Ds.Tables[DT_Role_CM.TABLENAME].Rows.Add(drRoleCM);
                                            }
                                        }
                                        else
                                        {
                                            if (!(Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + itemRisk.ID + " AND " + DT_Role_Risk.Role + " = '" + item[DT_Role_Risk.Role].ToString() + "'").Any()))
                                            {
                                                DataRow drRoleRiskk = Ds.Tables[DT_Role_Risk.TABLENAME].NewRow();
                                                drRoleRiskk[DT_Role_Risk.ID_RISK] = itemRisk.ID;
                                                drRoleRiskk[DT_Role_Risk.NAME_SHORT] = itemRisk.ShortName;
                                                drRoleRiskk[DT_Role_Risk.Role] = item[DT_Role_Risk.Role];
                                                drRoleRiskk[DT_Role_Risk.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                                                Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRoleRiskk);
                                            }
                                        }
                                    }
                                }
                                TreeOperation.DetectarMiPosicionActualInMain(Line_Selected, e.GetPosition(GridPaintLines), Ds);
                                DrawFishBone();
                            }
                            else
                            {
                                if (Line_Selected.IdRiskFather == ((RiskPolyLine)sender).ID)
                                {
                                    //MoviendoRisk = false;
                                    //TreeOperation.DetectarMiPosicionActual(Line_Selected, e.GetPosition(GridPaintLines), Ds);
                                    //DrawFishBone();
                                    GridPaintLines.Children.Remove(LineInMoving);
                                    MoviendoRisk = false;
                                    Creando = false;
                                    Line_Created = null;
                                    MoviendoCM = false;
                                    LineInMoving = null;
                                    CleanFishBone();
                                    LoadFishBone();
                                    DrawNumbersAndLineThickness();
                                    FixDrawPanel();
                                    UpdateGridRiskAndGridCM();
                                }
                                else
                                {
                                    MoviendoRisk = false;
                                    MoviendoCM = false;
                                    DrawFishBone();
                                }
                            }
                        }
                        else
                        {
                            Line_Selected = MainLine;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        private void MainLine_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Loose = true;
                int ID_Sender;
                bool IsRoot_Sender;
                RiskPolyLine TheLine;
                if (sender is RiskPolyLine)
                {
                    //Line_Selected = (RiskPolyLine)sender;
                    ID_Sender = ((RiskPolyLine)sender).ID;
                    IsRoot_Sender = ((RiskPolyLine)sender).IsRoot;
                    TheLine = (RiskPolyLine)sender;
                }
                else
                {
                    //MainLine = ((LabelPolyLine)sender).Line;
                    ID_Sender = ((SegmentPolyLine)sender).Father.ID;
                    IsRoot_Sender = ((SegmentPolyLine)sender).Father.IsRoot;
                    TheLine = ((SegmentPolyLine)sender).Father;
                }
                if (MoviendoRisk)
                {
                    if (Line_Selected.IdRiskFather != ID_Sender && Line_Selected.ID != ID_Sender)
                    {
                        Line_Selected.IdRiskFather = ID_Sender;
                        Line_Selected.Position = TheLine.Children.Count;
                        MoviendoRisk = false;
                        if (Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains(Line_Selected.ID))
                        {
                            Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.IDRISK_FATHER] = ID_Sender;
                        }
                        Ds.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + Line_Selected.ID).First()[DT_RiskStructure.IDRISK_FATHER] = ID_Sender;
                        DataRow[] drRoleRisk = Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + ID_Sender);
                        DataRow[] drRoleCM = Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + ID_Sender);
                        foreach (DataRow item in drRoleRisk)
                        {
                            foreach (RiskPolyLine itemRisk in LinesMoving)
                            {
                                if (itemRisk.IsCM)
                                {
                                    if (!(Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + itemRisk.ID + " AND " + DT_Role_CM.Role + " = '" + item[DT_Role_Risk.Role].ToString() + "'").Any()))
                                    {
                                        DataRow drRole = Ds.Tables[DT_Role_CM.TABLENAME].NewRow();
                                        drRole[DT_Role_CM.ID_CM] = itemRisk.ID;
                                        drRole[DT_Role_CM.NAME_SHORT] = itemRisk.ShortName;
                                        drRole[DT_Role_CM.Role] = item[DT_Role_Risk.Role];
                                        drRole[DT_Role_CM.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                                        Ds.Tables[DT_Role_CM.TABLENAME].Rows.Add(drRole);
                                    }
                                }
                                else
                                {
                                    if (!(Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + itemRisk.ID + " AND " + DT_Role_Risk.Role + " = '" + item[DT_Role_Risk.Role].ToString() + "'").Any()))
                                    {
                                        DataRow drRole = Ds.Tables[DT_Role_Risk.TABLENAME].NewRow();
                                        drRole[DT_Role_Risk.ID_RISK] = itemRisk.ID;
                                        drRole[DT_Role_Risk.NAME_SHORT] = itemRisk.ShortName;
                                        drRole[DT_Role_Risk.Role] = item[DT_Role_Risk.Role];
                                        drRole[DT_Role_Risk.IDROL_COLUMN] = item[DT_Role_Risk.IDROL_COLUMN];
                                        Ds.Tables[DT_Role_Risk.TABLENAME].Rows.Add(drRole);
                                    }
                                }
                            }
                        }
                        DrawFishBone();
                        GridPaintLines.Children.Remove(LineInMoving);
                        LineInMoving = null;
                    }
                    else
                    {
                        if (Line_Selected.IdRiskFather == ID_Sender)
                        {
                            //MoviendoRisk = false;
                            //Cursor = Cursors.Arrow;
                            //if (!(TheLine.Children.Count == 1 && Line_Selected == TheLine.Children[0]))
                            //{
                            //    TreeOperation.DetectarMiPosicionActual(Line_Selected, e.GetPosition(GridPaintLines), Ds);
                            //}
                            //DrawFishBone();
                            GridPaintLines.Children.Remove(LineInMoving);
                            MoviendoRisk = false;
                            Creando = false;
                            Line_Created = null;
                            MoviendoCM = false;
                            LineInMoving = null;
                            CleanFishBone();
                            LoadFishBone();
                            DrawNumbersAndLineThickness();
                            FixDrawPanel();
                            UpdateGridRiskAndGridCM();
                        }
                        else
                        {
                            MoviendoRisk = false;
                            MoviendoCM = false;
                            //Cursor = Cursors.Arrow;
                            TreeOperation.DrawEntireDiagramAsFishBone(MainLine);
                            //TreeOperation.FixMainLine(LinesList, MainLine);
                            FixDrawPanel();
                            SetLinesThickness();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }
        #endregion

        #region  Slider&Zoom
        private void SliderZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                SliderZoom.Value = Convert.ToDouble(General.MyRound(Convert.ToDecimal(SliderZoom.Value), 0));

                if (!(CanvasMain.LayoutTransform is ScaleTransform st))
                {
                    st = new ScaleTransform();
                    CanvasMain.LayoutTransform = st;
                }
                st.ScaleX = st.ScaleY = SliderZoom.Value / 100;
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        private void BtnPlusZoom_Click(object sender, RoutedEventArgs e)
        {
            if (SliderZoom.Value < 200)
            {
                SliderZoom.Value += 10;
            }
        }

        private void BtnMinusZoom_Click(object sender, RoutedEventArgs e)
        {
            if (SliderZoom.Value > 10)
            {
                SliderZoom.Value -= 10;
            }
        }

        private void SliderZoom_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && SliderZoom.Value < 200)
            {
                SliderZoom.Value += 10;
            }

            if (e.Delta < 0 && SliderZoom.Value > 10)
            {
                SliderZoom.Value -= 10;
            }
        }

        public void BrigIntoViewSelectedRiskPolyline(RiskPolyLine line)
        {
            double midHeight = ScrollGridPaint.ActualHeight / 3;
            double midWith = ScrollGridPaint.ActualWidth / 3;
            if (line != null)
            {
                double x0;
                double x1;

                double y0;
                double y1;
                if (line.FromTop)
                {
                    y0 = line.Points[1].Y + midHeight;
                    if (line.ActualHeight > ScrollGridPaint.ActualHeight)
                    {
                        y1 = line.Points[1].Y - midHeight;
                    }
                    else
                    {
                        y1 = line.Points[0].Y - midHeight;
                    }
                    x0 = line.Points[1].X + midWith;
                    if (line.ActualWidth > ScrollGridPaint.ActualWidth)
                    {
                        x1 = line.Points[1].X - midWith;
                    }
                    else
                    {
                        x1 = line.Points[0].X - midWith;
                    }
                }
                else
                {
                    y0 = line.Points[1].Y - midHeight;
                    if (line.ActualHeight > ScrollGridPaint.ActualHeight)
                    {
                        y1 = line.Points[1].Y + midHeight;
                    }
                    else
                    {
                        y1 = line.Points[0].Y + midHeight;
                    }
                    x0 = line.Points[1].X + midWith;
                    if (line.ActualWidth > ScrollGridPaint.ActualWidth)
                    {
                        x1 = line.Points[1].X - midWith;
                    }
                    else
                    {
                        x1 = line.Points[0].X - midWith;
                    }
                }

                Rect rectangleRPL = new Rect(new Point(x0, y0), new Point(x1, y1));
                line.BringIntoView(rectangleRPL);
            }
        }


        #endregion
        //TODO:COMO CAMBIAR COLOR AL ARBOL COMPLETO
        private void CbFilterTopR_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (LinesList.Count > 0)
                {
                    if (!(CbFilterTopR.SelectedValue is null))
                    {
                        IdDamageSelected = (Int32)CbFilterTopR.SelectedValue;
                        System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_DAMAGE + " = " + (Int32)CbFilterTopR.SelectedValue).First()[DT_Risk_Damages.COLOR].ToString()));

                        foreach (RiskPolyLine item in LinesList)
                        {
                            if (Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_DAMAGE + " = " + (Int32)CbFilterTopR.SelectedValue).Any())
                            {
                                if (!(item.IsCM))
                                {
                                    if (item.IsActivated)
                                    {
                                        if (item.IsRoot)
                                        {
                                            item.SetColor(new SolidColorBrush(Color.FromArgb(drawColor.A, drawColor.R, drawColor.G, drawColor.B)));
                                        }
                                        else
                                        {
                                            if (TengoPermiso(item))
                                            {
                                                item.SetColor(new SolidColorBrush(Color.FromArgb(drawColor.A, drawColor.R, drawColor.G, drawColor.B)));
                                            }
                                            else
                                            {
                                                item.SetColor(new SolidColorBrush(Color.FromArgb(80, drawColor.R, drawColor.G, drawColor.B)));
                                            }
                                        }
                                        item.UpdateSegmentsStroke();
                                    }
                                }
                            }
                        }
                        UpdateLinesValues();
                        SetLinesThickness();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        #region TextChangeName
        private void TextChangeName_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((TextBox)sender).Visibility == Visibility.Visible)
            {
                Dispatcher.BeginInvoke((Action)delegate
                {
                    Keyboard.Focus(TextChangeName);
                }, DispatcherPriority.Render);
            }
        }

        private void TextChangeName_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (NameEditing)
                {
                    Boolean? canUsePolyLineName = CanUseProposedPolyLineName(TextChangeName.Text);
                    if (!canUsePolyLineName.HasValue || canUsePolyLineName.HasValue && canUsePolyLineName.Value)
                    {
                        if (TextChangeName.Text != string.Empty)
                        {
                            UpdatePolyLineName(TextChangeName.Text);
                        }
                    }
                    ManageTextChangeProperties(String.Empty, Visibility.Hidden);
                    if (Line_Selected != null)
                    {
                        Line_Selected.ExtrasVisibility(Visibility.Visible);
                    }
                }
                NameEditing = false;
            }
            catch (Exception ex)
            {
                NameEditing = false;
                MostrarDialog(ex.Message);
            }
        }

        private void TextChangeName_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (NameEditing)
                {
                    if (e.Key == Key.Enter)
                    {
                        Boolean? canUsePolyLineName = CanUseProposedPolyLineName(TextChangeName.Text);
                        if (!canUsePolyLineName.HasValue || canUsePolyLineName.HasValue && canUsePolyLineName.Value)
                        {
                            if (TextChangeName.Text != string.Empty)
                            {
                                UpdatePolyLineName(TextChangeName.Text);
                            }
                        }
                        ManageTextChangeProperties(string.Empty, Visibility.Collapsed);
                        NameEditing = false;
                        if (Line_Selected != null)
                        {
                            Line_Selected.ExtrasVisibility(Visibility.Visible);
                        }
                    }
                    if (e.Key == Key.Escape)
                    {
                        ManageTextChangeProperties(string.Empty, Visibility.Collapsed);
                        if (Line_Selected != null)
                        {
                            Line_Selected.ExtrasVisibility(Visibility.Visible);
                        }
                        NameEditing = false;
                    }
                }
            }
            catch (Exception ex)
            {
                NameEditing = false;
                MostrarDialog(ex.Message);
            }
        }

        private void ManageTextChangeProperties(String valueTextProperty, Visibility aVisibility)
        {
            TextChangeName.Text = valueTextProperty;
            TextChangeName.Visibility = aVisibility;
        }

        private void UpdatePolyLineName(String polyLineName)
        {
            try
            {
                if (Line_Selected.IsCM)
                {
                    Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_CounterM.NAMESHORT] = polyLineName;
                    foreach (DataRow item in Ds.Tables[DT_CounterM_Damage.TABLENAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + Line_Selected.ID))
                    {
                        item[DT_CounterM_Damage.COUNTERM_NAMESHORT] = polyLineName;
                    }
                }
                else
                {
                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.NAMESHORT] = polyLineName;
                    foreach (DataRow item in Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_RISK + " = " + Line_Selected.ID))
                    {
                        item[DT_Risk_Damages.RISK_NAMESHORT] = polyLineName;
                    }

                }
                Line_Selected.ShortName = polyLineName;
                ClearFilters();
                UpdateGridRiskAndGridCM();
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        public void ClearFilters()
        {
            ((MainWindow)MyWindow).txtFilterCM.Clear();
            ((MainWindow)MyWindow).txtFilterRisk.Clear();
        }

        private bool? CanUseProposedPolyLineName(string proposedPolyLineName)
        {
            try
            {
                bool? result = null;
                WindowMessageYesNo yesNo = null;
                if (!(Line_Selected.IsCM))
                {
                    int RiskTreeID = (int)Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.ID_DIAGRAM];

                    if (Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + RiskTreeID + " and "
                        + DT_Risk.NAMESHORT + " = '" + proposedPolyLineName + "' and " + DT_Risk.ID + " <> " + Line_Selected.ID).Any())
                    {
                        yesNo = new WindowMessageYesNo("The name [" + proposedPolyLineName + "] Already exists in this diagram. Do you want to use it again?");
                        yesNo.ShowDialog();
                    }
                }
                else
                {
                    int RiskTreeID = (int)Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.IdRiskFather)[DT_Risk.ID_DIAGRAM];

                    if (Ds.Tables[DT_CounterM.TABLE_NAME].Select(DT_CounterM.ID_RISK_TREE + " = " + RiskTreeID + " and "
                        + DT_CounterM.NAMESHORT + " = '" + proposedPolyLineName + "' and " + DT_CounterM.ID + " <> " + Line_Selected.ID).Any())
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GridEvents

        /// <summary>
        /// Trigger when the mouse move over the Paint Area
        /// </summary>
        private void GridPaintLines_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {

                //if (((MainWindow)MyWindow).P.TheCurrentLayout.ID_Diagram != ID_Diagram)
                //{
                //    ((MainWindow)MyWindow).P.TheCurrentLayout = this;
                //}
                //((MainWindow)MyWindow).UpdateMiniMapSource();
                //foreach (var item in ((MainWindow)MyWindow).OpenedDocuments)
                //{
                //    item.ExitWorking();
                //}
                //this.EnterWorking();
                if (!(NameEditing))
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (!(Loose))
                        {
                            if (FullAccess(Line_Selected))
                            {
                                if (Line_Selected.IsCM)
                                {
                                    if (!(MoviendoCM))
                                    {
                                        MoviendoCM = true;
                                        LinesMoving = new List<RiskPolyLine>() { Line_Selected };
                                    }
                                    TreeOperation.MoveLines(LinesMoving, e.GetPosition(GridPaintLines).X - Line_Selected.Points[1].X - 35, e.GetPosition(GridPaintLines).Y - Line_Selected.Points[1].Y);
                                    X = e.GetPosition(GridPaintLines).X;
                                    Y = e.GetPosition(GridPaintLines).Y;
                                    Main_Y = MainLine.Points[0].Y;
                                }
                                else
                                {
                                    {
                                        MoviendoRisk = true;
                                        LinesMoving = new List<RiskPolyLine>();
                                        LinesMoving.AddRange(TreeOperation.GetMeAndMyChildrenWithCM(Line_Selected));
                                        foreach (var item in LinesMoving)
                                        {
                                            item.Oculto = true;
                                        }
                                        GridPaintLines.Children.Remove(LineInMoving);
                                        System.Drawing.Color lnColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram)[CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                                        LineInMoving = new RiskPolyLine(GridPaintLines, MenuRisk, false)
                                        {
                                            Stroke = new SolidColorBrush(System.Windows.Media.Color.FromArgb(lnColor.A, lnColor.R, lnColor.G, lnColor.B)),
                                            StrokeThickness = 3,
                                            IsMoving = true
                                        };
                                        LineInMoving.NewDrawAtPoint(new Point(X, Y), "");
                                    }
                                    if (MoviendoRisk)
                                    {
                                        LineInMoving.NewDrawAtPoint(e.GetPosition(GridPaintLines));
                                        TreeOperation.MoveLines(new List<RiskPolyLine>() { LineInMoving }, e.GetPosition(GridPaintLines).X - LineInMoving.Points[1].X - 25, e.GetPosition(GridPaintLines).Y - LineInMoving.Points[1].Y);
                                        X = e.GetPosition(GridPaintLines).X;
                                        Y = e.GetPosition(GridPaintLines).Y;
                                        Main_Y = MainLine.Points[0].Y;
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        if (MoviendoRisk || MoviendoCM)
                        {
                            if (MoviendoRisk)
                            {
                                LineInMoving.NewDrawAtPoint(e.GetPosition(GridPaintLines));
                                TreeOperation.MoveLines(new List<RiskPolyLine>() { LineInMoving }, e.GetPosition(GridPaintLines).X - LineInMoving.Points[1].X - 25, e.GetPosition(GridPaintLines).Y - LineInMoving.Points[1].Y);
                                X = e.GetPosition(GridPaintLines).X;
                                Y = e.GetPosition(GridPaintLines).Y;
                                Main_Y = MainLine.Points[0].Y;
                            }
                            if (MoviendoCM)
                            {
                                TreeOperation.MoveLines(LinesMoving, e.GetPosition(GridPaintLines).X - Line_Selected.Points[1].X - 35, e.GetPosition(GridPaintLines).Y - Line_Selected.Points[1].Y);
                                X = e.GetPosition(GridPaintLines).X;
                                Y = e.GetPosition(GridPaintLines).Y;
                                Main_Y = MainLine.Points[0].Y;
                            }

                        }
                        else
                        {
                            if (Creando)
                            {
                                Line_Created.NewDrawAtPoint(e.GetPosition(GridPaintLines));
                                TreeOperation.MoveLines(new List<RiskPolyLine>() { Line_Created }, e.GetPosition(GridPaintLines).X - Line_Created.Points[1].X - 25, e.GetPosition(GridPaintLines).Y - Line_Created.Points[1].Y);
                                X = e.GetPosition(GridPaintLines).X;
                                Y = e.GetPosition(GridPaintLines).Y;
                                Main_Y = MainLine.Points[0].Y;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        /// <summary>
        /// Trigger when mouse click in Draw Area
        /// </summary>
        private void GridPaintLines_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (((MainWindow)MyWindow).P.TheCurrentLayout.ID_Diagram != ID_Diagram)
                {
                    ((MainWindow)MyWindow).P.TheCurrentLayout = this;
                    ((MainWindow)MyWindow).UpdateMiniMapSource();
                    foreach (var item in ((MainWindow)MyWindow).OpenedDocuments)
                    {
                        item.ExitWorking();
                    }
                    this.EnterWorking();
                    //CleanFishBone();
                    //LoadFishBone();
                    //DrawNumbersAndLineThickness();
                    ((MainWindow)MyWindow).TextProbabilityChange(MainLine);
                }
                UpdateGridRiskAndGridCM();
                if (Creando)
                {
                    GridPaintLines.Children.Remove(Line_Created);
                    Line_Created.TextPanel.Child = null;
                    GridPaintLines.Children.Remove(Line_Created.TextPanel);
                    Creando = false;
                }
                if (MoviendoRisk || MoviendoCM || Creando)
                {
                    if (MoviendoRisk)
                    {
                        GridPaintLines.Children.Remove(LineInMoving);
                    }
                    MoviendoRisk = false;
                    Creando = false;
                    Line_Created = null;
                    MoviendoCM = false;
                    LineInMoving = null;
                    CleanFishBone();
                    LoadFishBone();
                    DrawNumbersAndLineThickness();
                    FixDrawPanel();
                    UpdateGridRiskAndGridCM();
                }
                else
                {
                    if (SelectingToGroup)
                    {
                        SelectingToGroup = false;
                        GroupSelected = null;
                    }
                    if (e.OriginalSource is System.Windows.Controls.Grid && Keyboard.Modifiers != ModifierKeys.Control)
                    {
                        if (ChoosingRisk && !ChoosingCM)
                        {
                            ResetGroupRiksSelection();
                        }
                        else if (!ChoosingRisk && ChoosingCM)
                        {
                            ResetGroupCMSelection();
                        }
                        else if (ChoosingRisk && ChoosingCM)
                        {
                            ResetGroupRiksSelection();
                            ResetGroupCMSelection();
                        }
                        else
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        private void GridPaintLines_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Loose = true;
                if (MoviendoRisk || MoviendoCM)
                {
                    if (MoviendoRisk)
                    {
                        GridPaintLines.Children.Remove(LineInMoving);
                    }
                    MoviendoRisk = false;
                    Creando = false;
                    MoviendoCM = false;
                    DrawFishBone();
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        private void GridPaintLines_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                if (Keyboard.Modifiers != ModifierKeys.Control)
                    return;

                if (e.Delta > 0 && SliderZoom.Value < 200)
                {
                    SliderZoom.Value += 10;
                }

                if (e.Delta < 0 && SliderZoom.Value > 10)
                {
                    SliderZoom.Value -= 10;
                }
                Point mouseAtImage = e.GetPosition(GridPaintLines);
                Point mouseAtScrollViewer = e.GetPosition(ScrollGridPaint);

                if (!(CanvasMain.LayoutTransform is ScaleTransform st))
                {
                    st = new ScaleTransform();
                    CanvasMain.LayoutTransform = st;
                }

                if (e.Delta > 0)
                {
                    st.ScaleX = st.ScaleY = st.ScaleX + 0.1;
                    if (st.ScaleX > 2)
                        st.ScaleX = st.ScaleY = 2;
                }
                else
                {
                    st.ScaleX = st.ScaleY = st.ScaleX - 0.1;
                    if (st.ScaleX < 0.1)
                        st.ScaleX = st.ScaleY = 0.1;
                }
                #region [this step is critical for offset]
                ScrollGridPaint.ScrollToHorizontalOffset(0);
                ScrollGridPaint.ScrollToVerticalOffset(0);
                MyWindow.UpdateLayout();
                #endregion

                Vector offset = GridPaintLines.TranslatePoint(mouseAtImage, ScrollGridPaint) - mouseAtScrollViewer;
                ScrollGridPaint.ScrollToHorizontalOffset(offset.X);
                ScrollGridPaint.ScrollToVerticalOffset(offset.Y);
                MyWindow.UpdateLayout();
                e.Handled = true;
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        #endregion

        #region Export to Excel

        public void ExportToExcel(string fileName)
        {
            this.TheProgressBar.IsIndeterminate = false;
            this.TheProgressBar.Minimum = 0;
            this.TheProgressBar.Maximum = 100;
            //this.TheProgressBar.Visibility = Visibility.Visible;

            using (RiskTreeDataSetTrader riskTreeDataSetTrader = new RiskTreeDataSetTrader(this.Ds, this.ID_Diagram))
            {
                using (ExportRiskTree exportRiskTree = new ExportRiskTree(riskTreeDataSetTrader, fileName))
                {
                    exportToExcelWorker.RunWorkerAsync(exportRiskTree);
                    this.IsExportingToExcel = true;
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
            this.TheProgressBar.Value = e.ProgressPercentage;
        }
        void ExportToExcelWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.TheProgressBar.Value = 100;
            IFormatProvider formatProvider = CultureInfo.CurrentUICulture;
            ((MainWindow)MyWindow).MostrarInfoDialog(string.Format(formatProvider, "Diagram {0} was saved as excel file!", this.Title));
            this.TheProgressBar.Visibility = Visibility.Collapsed;
            this.TheProgressBar.Value = 0;
            this.TheProgressBar.IsIndeterminate = true;
            this.IsExportingToExcel = false;
        }

        #endregion

        private void LayoutDocument_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LayoutDocument_IsActiveChanged(object sender, EventArgs e)
        {
            try
            {
                if (((MainWindow)MyWindow).P.TheCurrentLayout != null)
                {
                    if (((MainWindow)MyWindow).P.TheCurrentLayout.ID_Diagram != ID_Diagram)
                    {
                        ((MainWindow)MyWindow).P.TheCurrentLayout = this;
                        ((MainWindow)MyWindow).UpdateMiniMapSource();
                        foreach (var item in ((MainWindow)MyWindow).OpenedDocuments)
                        {
                            item.ExitWorking();
                        }
                        this.EnterWorking();
                        ((MainWindow)MyWindow).TextProbabilityChange(MainLine);
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        private void BtnUndoneScope_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IsScoping = false;
                DropLines();
                DropRectangles();
                LoadLines();
                LoadRectangles();
                DrawNumbers();
                BtnUndoneScope.Visibility = Visibility.Hidden;
                //TextDiagram.Text = P.TheCurrentLayout.Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(P.TheCurrentLayout.ID_Diagram)[DT_Diagram.DIAGRAM_NAME].ToString();
                Title = Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.DIAGRAM_NAME].ToString();
                //TextProbabilityChange(P.TheCurrentLayout.MainLine);
                SetLinesThickness();
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }
    }
}
