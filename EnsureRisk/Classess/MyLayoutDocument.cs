using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;
using System.Windows.Controls;
using System.Windows;
using EnsureBusinesss.Business;
using System.Collections.Generic;
using EnsureBusinesss;
using System.Linq;
using System.Windows.Media;
using DataMapping.Data;
using System.Windows.Media.Imaging;
using System;
using EnsureRisk.Resources;
using System.Data;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using EnsureRisk.Windows;
using System.ComponentModel;
using EnsureRisk.Export.Trader;
using EnsureRisk.Export;
using System.Globalization;

namespace EnsureRisk.Classess
{
    public class MyLayoutDocument : LayoutDocument
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
        public bool SelectingToGroup { get; set; }
        public bool Creando { get; set; }
        public bool Copiando { get; set; }
        public bool IsExportingToExcel { get; set; }
        private readonly BackgroundWorker exportToExcelWorker = new BackgroundWorker();
        #endregion        

        #region Menus
        public ContextMenu MenuRisk { get; set; }
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
        public Dictionary<Int32, Boolean> LinesListCMState { get; set; }
        #endregion

        #region RiskPolyLines
        public RiskPolyLine ScopeLine { get; set; }
        public RiskPolyLine CopyRisk { get; set; }
        public RiskPolyLine Line_Selected { get; set; }
        public RiskPolyLine MainLine { get; set; }
        public RiskPolyLine Line_Created { get; set; }
        #endregion

        #region Componentes
        public ScrollViewerDiagram ScrollGridPaint { get; set; }
        public GridPaint GridPaintLines { get; set; }
        public LGrid CanvasMain { get; set; }
        public MyGrid TheMainGrid { get; set; }
        public MyGrid TheTopGrid { get; set; }

        public GridToolBar TheGridToolbar { get; set; }
        public ProgressBar TheProgressBar { get; set; }
        public ComboBox TheZoomComboBox { get; set; }
        public LTextBox TextChangeName { get; set; }
        public ComboBox CbFilterTopR { get; set; }
        public TextBlock TBPercentage { get; set; }
        public Button BtMinus { get; set; }
        public Button BtMPlus { get; set; }
        public Slider SliderZoom { get; set; }

        #endregion


        //protected override void OnIsSelectedChanged(bool oldValue, bool newValue)
        //{
        //    if (newValue)
        //        Parent;
        //}

        public LineGroup GroupSelected { get; set; }
        public int ID_Diagram { get; set; }
        public Popin Popin { get; set; }
        public Point MIdPoint { get; set; }
        public DataSet Ds { get; set; }
        public DataRow DrDiagram { get; set; }
        public Window MyWindow { get; set; }
        public MyLayoutDocument() : base()
        {
            try
            {
                this.CanClose = true;
                this.CanFloat = true;
                this.CanMove = true;
                CanvasMain = new LGrid() { MyOwner = this };
                CbFilterTopR = new LComboBox() { MyOwner = this, IsEditable = false, HorizontalContentAlignment = HorizontalAlignment.Right };

                TheMainGrid = new MyGrid() { ID_Diagram = this.ID_Diagram, Background = new SolidColorBrush(Colors.White) };
                TheTopGrid = new MyGrid() { ID_Diagram = this.ID_Diagram, Background = new SolidColorBrush(Colors.Red) };
                TheTopGrid.Children.Add(TheMainGrid);
                GridPaintLines = new GridPaint() { MyOwner = this };
                RowDefinition row0 = new RowDefinition() { };
                RowDefinition row1 = new RowDefinition() { Height = new GridLength(50) };
                TheMainGrid.RowDefinitions.Add(row0);
                TheMainGrid.RowDefinitions.Add(row1);
                MIdPoint = new Point(GridPaintLines.Width - 180, GridPaintLines.Height / 2);
                ScrollGridPaint = new ScrollViewerDiagram() { MyOwner = this, ID_Diagram = this.ID_Diagram, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };
                ScrollGridPaint.AddHijo(CanvasMain);

                CanvasMain.Children.Add(GridPaintLines);
                TheMainGrid.Children.Add(ScrollGridPaint);
                TheMainGrid.SetTheRow(ScrollGridPaint, 0);

                TheGridToolbar = new GridToolBar() { ID_Diagram = this.ID_Diagram };

                TheMainGrid.Children.Add(TheGridToolbar);
                TheMainGrid.SetTheRow(TheGridToolbar, 1);
                Content = TheTopGrid;

                TheProgressBar = new ProgressBar() { VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Stretch, IsIndeterminate = true, Visibility = Visibility.Hidden };
                StackPanel stk = new StackPanel() { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left };
                stk.Children.Add(new TextBlock() { Text = "Damages: ", VerticalAlignment = VerticalAlignment.Center });
                stk.Children.Add(CbFilterTopR);
                CbFilterTopR.DropDownClosed += CbFilterTopR_DropDownClosed;
                TheGridToolbar.Children.Add(stk);
                TheGridToolbar.SetTheColumn(stk, 0);

                TheGridToolbar.Children.Add(TheProgressBar);
                TheGridToolbar.SetTheColumn(TheProgressBar, 1);

                TheZoomComboBox = new LComboBox() { Name = "Zoom" + this.ID_Diagram, MyOwner = this, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Right, HorizontalContentAlignment = HorizontalAlignment.Right, IsEditable = true };
                TheZoomComboBox.Items.Add(new ComboBoxItem() { Content = "10" });
                TheZoomComboBox.Items.Add(new ComboBoxItem() { Content = "25" });
                TheZoomComboBox.Items.Add(new ComboBoxItem() { Content = "50" });
                TheZoomComboBox.Items.Add(new ComboBoxItem() { Content = "75" });
                TheZoomComboBox.Items.Add(new ComboBoxItem() { Content = "80" });
                TheZoomComboBox.Items.Add(new ComboBoxItem() { Content = "100" });
                TheZoomComboBox.Items.Add(new ComboBoxItem() { Content = "150" });
                TheZoomComboBox.Items.Add(new ComboBoxItem() { Content = "200" });
                TheZoomComboBox.Text = "100";
                TheGridToolbar.Children.Add(TheZoomComboBox);
                TheGridToolbar.SetTheColumn(TheZoomComboBox, 2);

                TBPercentage = new TextBlock() { VerticalAlignment = VerticalAlignment.Center, Text = "%" };
                TheGridToolbar.Children.Add(TBPercentage);
                TheGridToolbar.SetTheColumn(TBPercentage, 3);

                BtMinus = new LButton()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Content = new MaterialDesignThemes.Wpf.PackIcon()
                    {
                        Kind = MaterialDesignThemes.Wpf.PackIconKind.MinusCircleOutline
                    },
                    MyOwner = this
                };
                BtMinus.Click += BtnMinusZoom_Click;
                TheGridToolbar.Children.Add(BtMinus);
                TheGridToolbar.SetTheColumn(BtMinus, 4);

                SliderZoom = new LSlider()
                {
                    Name = "Slider" + ID_Diagram,
                    MyOwner = this,
                    Minimum = 10,
                    Maximum = 200,
                    LargeChange = 20,
                    TickFrequency = 10,
                    Value = 100,
                    SmallChange = 10,
                    TickPlacement = System.Windows.Controls.Primitives.TickPlacement.TopLeft
                    //Poner eventos
                };
                SliderZoom.ValueChanged += SliderZoom_ValueChanged;
                SliderZoom.MouseWheel += SliderZoom_MouseWheel;
                TheGridToolbar.Children.Add(SliderZoom);
                TheGridToolbar.SetTheColumn(SliderZoom, 5);

                BtMPlus = new LButton()
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Content = new MaterialDesignThemes.Wpf.PackIcon()
                    {
                        Kind = MaterialDesignThemes.Wpf.PackIconKind.PlusCircleOutline
                    },
                    MyOwner = this
                };
                BtMPlus.Click += BtnPlusZoom_Click;
                TheGridToolbar.Children.Add(BtMPlus);
                TheGridToolbar.SetTheColumn(BtMPlus, 6);
                GridPaintLines.Width = 200;
                GridPaintLines.Height = 200;
                GridPaintLines.MouseMove += GridPaintLines_MouseMove;
                GridPaintLines.MouseDown += GridPaintLines_MouseDown;
                GridPaintLines.MouseUp += GridPaintLines_MouseUp;
                GridPaintLines.MouseWheel += GridPaintLines_MouseWheel;
                this.Closing += MyLayoutDocument_Closed;
                MIdPoint = new Point(GridPaintLines.Width - 180, GridPaintLines.Height / 2);
                Ds = new UserDataSet();
                LinesList = new List<RiskPolyLine>();
                LinesMoving = new List<RiskPolyLine>();
                Rectangles = new List<MyDamage>();
                RiskGroupSelected = new List<RiskPolyLine>();
                CMGroupSelected = new List<RiskPolyLine>();
                ListCopy = new List<RiskPolyLine>();
                LinesListCMState = new Dictionary<Int32, Boolean>();
                MainLine = new RiskPolyLine(GridPaintLines, MenuMainRisk, false);
                ScrollGridPaint.ScrollToVerticalOffset(GridPaintLines.Height / 2);
                ScrollGridPaint.ScrollToHorizontalOffset(GridPaintLines.Width);
                TextChangeName = new LTextBox() { MyOwner = this, Name = "TextChangeName", MaxLength = 250, VerticalAlignment = VerticalAlignment.Top, HorizontalAlignment = HorizontalAlignment.Left, Width = 100, Visibility = Visibility.Hidden };
                TextChangeName.KeyDown += TextChangeName_KeyDown;
                TextChangeName.LostFocus += TextChangeName_LostFocus;
                TextChangeName.IsVisibleChanged += TextChangeName_IsVisibleChanged;
                GridPaintLines.Children.Add(TextChangeName);
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
                BindingOperations.SetBinding(TheZoomComboBox, LComboBox.TextProperty, myBinding);
                EnterWorking();

                exportToExcelWorker.WorkerReportsProgress = true;
                exportToExcelWorker.WorkerSupportsCancellation = true;
                exportToExcelWorker.DoWork += ExportToExcelWorker_DoWork;
                exportToExcelWorker.ProgressChanged += ExportToExcelWorker_ProgressChanged;
                exportToExcelWorker.RunWorkerCompleted += ExportToExcelWorker_RunWorkerCompleted;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
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
                        item.MyName.Visibility = Visibility.Hidden;
                        if (!(item.IsCM))
                        {
                            (item).Expand.Visibility = Visibility.Collapsed;
                        }
                    }
                    //TreeOperation.Build_Tree(LinesList);
                    foreach (RiskPolyLine item in TreeOperation.GetMeAndAllChildsWithCM(LinesList.Find(x => x.ID == ScopeLine.ID)))
                    {
                        item.Visibility = Visibility.Visible;
                        item.MyName.Visibility = Visibility.Visible;
                        if (!(item.IsCM))
                        {
                            item.Expand.Visibility = Visibility.Visible;
                            item.Expand.Visibility = item.Children.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                        }
                    }
                    //DropRectangles();

                    LoadRectangles();

                    DrawNumbers();
                    ((MainWindow)MyWindow).TextProbabilityChange(LinesList.Find(x => x.ID == ScopeLine.ID));
                    Title = LinesList.Find(x => x.ID == ScopeLine.ID).ShortName;
                    ((MainWindow)MyWindow).TextDiagram.Text = LinesList.Find(x => x.ID == ScopeLine.ID).ShortName;
                    LinesList.Find(x => x.ID == ScopeLine.ID).ExtrasVisibility(Visibility.Hidden);

                    ((MainWindow)MyWindow).BtnBackward.Visibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    new WindowMessageOK(ex.Message).ShowDialog();
                }
            }
        }

        public void EnterWorking()
        {
            TheMainGrid.Margin = new Thickness(3);
        }
        public void ExitWorking()
        {
            TheMainGrid.Margin = new Thickness(0);
        }

        private void CbFilterTopR_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (LinesList.Count > 0)
                {
                    LineThickness();
                    if (!(CbFilterTopR.SelectedValue is null))
                    {
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
                                            item.Stroke = new SolidColorBrush(Color.FromArgb(drawColor.A, drawColor.R, drawColor.G, drawColor.B));
                                        }
                                        else
                                        {
                                            if (TengoPermiso(item))
                                            {
                                                item.Stroke = new SolidColorBrush(Color.FromArgb(drawColor.A, drawColor.R, drawColor.G, drawColor.B));
                                            }
                                            else
                                            {
                                                item.Stroke = new SolidColorBrush(Color.FromArgb(80, drawColor.R, drawColor.G, drawColor.B));
                                            }
                                        }
                                        item.UpdateSegmentsStroke();
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

        private void MyLayoutDocument_Closed(object sender, EventArgs e)
        {
            try
            {
                if (exportToExcelWorker.IsBusy && exportToExcelWorker.WorkerSupportsCancellation)
                {
                    exportToExcelWorker.CancelAsync();
                }
                if (Ds.HasChanges())
                {
                    if (new WindowMessageYesNo("Do you want to save the changes on [" + this.Title + "]").ShowDialog() == true)
                    {
                        ((MainWindow)MyWindow).SaveData(Ds, true);
                    }
                    else
                    {
                        Ds.RejectChanges();
                    }
                }
                ((MainWindow)MyWindow).OpenedDocuments.Remove(this);
                ((MainWindow)MyWindow).DV_CrossRisk.Table.Clear();
                ((MainWindow)MyWindow).DV_Cross_CM.Table.Clear();
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
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

        #region TextChangeName
        private void TextChangeName_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (((LTextBox)sender).Visibility == Visibility.Visible)
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
                        UpdatePolyLineName(TextChangeName.Text);
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                        //EndingEditTextChangeName(textChangeName.Text);
                        Boolean? canUsePolyLineName = CanUseProposedPolyLineName(TextChangeName.Text);
                        if (!canUsePolyLineName.HasValue || canUsePolyLineName.HasValue && canUsePolyLineName.Value)
                        {
                            UpdatePolyLineName(TextChangeName.Text);
                        }
                        ManageTextChangeProperties(String.Empty, Visibility.Hidden);
                        NameEditing = false;
                    }
                    if (e.Key == Key.Escape)
                    {
                        ManageTextChangeProperties(String.Empty, Visibility.Hidden);
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                DrawEntireDiagram();
                UpdateGridRiskAndGridCM();
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        public void ClearFilters()
        {
            ((MainWindow)MyWindow).txtFilterCM.Clear();
            ((MainWindow)MyWindow).txtFilterRisk.Clear();
        }

        private bool? CanUseProposedPolyLineName(String proposedPolyLineName)
        {
            try
            {
                bool? result = null;
                WindowMessageYesNo yesNo = null;
                if (!(Line_Selected.IsCM))
                {
                    //Es un riesgo
                    int RiskTreeID = (Int32)Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.ID_DIAGRAM];

                    if (Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + RiskTreeID + " and "
                        + DT_Risk.NAMESHORT + " = '" + proposedPolyLineName + "' and " + DT_Risk.ID + " <> " + Line_Selected.ID).Any())
                    {
                        yesNo = new WindowMessageYesNo("The name [" + proposedPolyLineName + "] Already exists in this diagram. Do you want to use it again?");
                        yesNo.ShowDialog();
                    }
                }
                else
                {
                    //Es una contramedida
                    int RiskTreeID = (Int32)Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.IdRiskFather)[DT_Risk.ID_DIAGRAM];

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
                IdDamageSelected = (Int32)CbFilterTopR.SelectedValue;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        public void FixDrawPanel()
        {
            try
            {
                if (LinesList.OrderBy(rl => rl.Points[0].X).First().Points[0].X < 0)
                {
                    //while any point of the diagram is below of the visual Grid, move the entre diagram to de Right
                    while (LinesList.OrderBy(rl => rl.Points[0].X).First().Points[0].X < 0)
                    {
                        TreeOperation.MoveEntireTree(LinesList, 70, 0, Rectangles);
                    }
                }
                GridPaintLines.Width = LinesList.OrderByDescending(rl => rl.Points[1].X).First().Points[1].X + 500;
                if (LinesList.OrderBy(rl => rl.Points[0].Y).First().Points[0].Y < 0)
                {
                    while (LinesList.OrderBy(rl => rl.Points[0].Y).First().Points[0].Y < 0)
                    {
                        TreeOperation.MoveEntireTree(this.LinesList, 0, 210, Rectangles);
                    }
                }
                GridPaintLines.Height = LinesList.OrderByDescending(rl => rl.Points[1].Y).First().Points[1].Y + 300;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

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
                    new Point(MyMainLine.Points[1].X + 5, puntoinicialY),
                    item[DT_Diagram_Damages.DAMAGE].ToString() + " (" + item[DT_Diagram_Damages.UM].ToString() + ")",
                    StringResources.ACUM_VALUE + "(" + item[DT_Diagram_Damages.UM].ToString() + ") :",
                    StringResources.ACUM_DAMAGE, mediaColor, (Int32)item[DT_Diagram_Damages.ID_DAMAGE], item[DT_Diagram_Damages.UM].ToString());

                    Rectangles.Add(rectangle);
                    puntoinicialY += 90;
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                        foreach (var itemI in TreeOperation.GetMeAndAllChildsWithCM(LinesList.Find(rl => rl.ID == MyMainLine.ID)))
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
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        public void LineThickness()
        {
            try
            {
                if (!(CbFilterTopR.SelectedValue is null))
                {
                    IdDamageSelected = (Int32)CbFilterTopR.SelectedValue;
                    if (IdDamageSelected != 0)
                    {
                        DataTable dtRisk = Ds.Tables[DT_Risk.TABLE_NAME].Clone();
                        foreach (var item in Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + ID_Diagram))
                        {
                            dtRisk.ImportRow(item);
                        }
                        General.UpdateThickness(IdDamageSelected, dtRisk, LinesList, Ds.Tables[DT_Risk_Damages.TABLENAME],
                                       Ds.Tables[DT_CounterM.TABLE_NAME], Ds.Tables[DT_CounterM_Damage.TABLENAME]);
                    }

                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        public void AddMainLine(DataRow dr, System.Drawing.Color lnColor, string DiagramName)
        {
            try
            {
                MainLine = new RiskPolyLine(GridPaintLines, MenuMainRisk, false)
                {
                    //aca lo del color
                    Stroke = new SolidColorBrush(Color.FromArgb(lnColor.A, lnColor.R, lnColor.G, lnColor.B)),
                    IsRoot = true,
                    IsCM = false,
                    FromTop = false,
                    StrokeThickness = 6,
                    ID = (Int32)dr[DT_Risk.ID],
                    Probability = (Decimal)dr[DT_Risk.PROBABILITY] / 100,
                    ShortName = "Total Risk",
                    MyLevel = 0
                };
                MainLine.Group = new LineGroup()
                {
                    IdGroup = null,
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
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        public void DropRectangles()
        {
            try
            {
                //foreach (MyDamage item in Rectangles)
                //{
                //    GridPaintLines.Children.Remove(item);
                //}
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                        item.ElStackPannel.Children.Clear();
                        //GridPaintLines.Children.Remove(sg.ElStackPannel);
                    }

                    //in his cicle we remove any risk of previous risk tree drawing in the main form.
                    GridPaintLines.Children.Remove(item);//removing the line
                    item.ElStackPannel.Children.Clear();
                    GridPaintLines.Children.Remove(item.ElStackPannel);
                }
                LinesList.Clear();
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        public void LoadLines()
        {
            try
            {
                LinesListCMState.Clear();
                if (CbFilterTopR.SelectedIndex >= 0)
                {
                    System.Drawing.Color drawingCColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram)[CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                    AddMainLine(Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + ID_Diagram + " and " + DT_Risk.IS_ROOT + " = " + 1).First(), drawingCColor, Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.DIAGRAM_NAME].ToString());

                    foreach (DataRow item in Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + ID_Diagram))
                    {
                        if (!((Boolean)item[DT_Risk.IS_ROOT]))
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
                            //RiskPolyLine riskLine = haspermission ? CreateRiskShape(GridPaintLines, MenuRisk, false, item) : CreateRiskShape(GridPaintLines, null, false, item);
                            RiskPolyLine riskLine = haspermission ? CreateRiskShape(((MainWindow)MyWindow).CurrentLayout.GridPaintLines, MenuRisk, false, item) : CreateRiskShape(((MainWindow)MyWindow).CurrentLayout.GridPaintLines, null, false, item);
                            SetPolyLineGroup(riskLine, item);

                            if ((Boolean)item[DT_Risk.ISCOLLAPSED])
                            {
                                riskLine.Expand.Source = new BitmapImage(new Uri(General.CONTRAIDO));
                            }
                            else
                            {
                                riskLine.Expand.Source = new BitmapImage(new Uri(General.EXPANDIDO));
                            }

                            if (((Boolean)item[DT_Risk.ENABLED]))
                            {
                                ((MenuItem)MenuRisk.Items[9]).ToolTip = StringResources.DisableValue;
                                riskLine.DrawEntireLine(item[DT_Risk.NAMESHORT].ToString());//this method draw a Line in the Main Form
                                                                                                    //with its respective LabelName.
                                                                                                    //aca el color de los roles   


                                if (haspermission)
                                {
                                    riskLine.Stroke = new SolidColorBrush(Color.FromArgb(drawingCColor.A, drawingCColor.R, drawingCColor.G, drawingCColor.B));
                                }
                                else
                                {
                                    riskLine.Stroke = new SolidColorBrush(Color.FromArgb(80, drawingCColor.R, drawingCColor.G, drawingCColor.B));
                                }
                            }
                            else
                            {
                                ((MenuItem)MenuRisk.Items[9]).ToolTip = StringResources.EnableValue;
                                riskLine.DrawEntireLine("(Disabled)" + item[DT_Risk.NAMESHORT].ToString());//this method draw a Line in the Main Form
                                                                                                                   //with its respective LabelName.
                                riskLine.Stroke = new SolidColorBrush(Colors.Gray);
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

                            riskLine.MyName.Text = riskLine.ShortName.ToString();
                            if (Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { riskLine.ID, (Int32)CbFilterTopR.SelectedValue }))
                            {
                                riskLine.MyOwnValue = (decimal)Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Find(new object[] { riskLine.ID, IdDamageSelected })[DT_Risk_Damages.VALUE];

                                //riskLine.Class = riskLine.MyOwnValue / General.RangeOfClass(LinesList.Find(m => m.IsRoot == true).ID, Ds, IdDamageSelected);
                                //riskLine.Class = General.MyRound(riskLine.Class, 0);
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
                        RiskPolyLine cmline = haspermission ? CreateCounterMeasureShape(((MainWindow)MyWindow).CurrentLayout.GridPaintLines, MenuCM, true, item) : CreateCounterMeasureShape(((MainWindow)MyWindow).CurrentLayout.GridPaintLines, null, true, item);
                        SetPolyLineGroup(cmline, item);

                        if (((Boolean)item[DT_CounterM.ENABLED]))
                        {
                            ((MenuItem)MenuCM.Items[3]).ToolTip = StringResources.DisableValue;
                            cmline.DrawEntireLine(item[DT_CounterM.NAMESHORT].ToString());//this method draw a Line in the Main Form
                                                                                                  //with its respective LabelName.

                            if (!(haspermission))
                            {
                                cmline.Stroke = new SolidColorBrush(Colors.Gray);
                            }
                        }
                        else
                        {
                            ((MenuItem)MenuCM.Items[3]).ToolTip = StringResources.EnableValue;
                            cmline.DrawEntireLine("(Disabled)" + item[DT_CounterM.NAMESHORT].ToString());//this method draw a Line in the Main Form
                                                                                                                 //with its respective LabelName.
                            cmline.Stroke = new SolidColorBrush(Colors.Gray);
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
                    //TreeOperation.FixMainLine(LinesList, MainLine);
                    FixDrawPanel();
                    foreach (var item in LinesList)
                    {
                        if (item.Collapsed)
                        {
                            foreach (RiskPolyLine itemi in TreeOperation.GetOnlyMyAllChildsWithCM(item))
                            {
                                itemi.Visibility = Visibility.Hidden;
                                itemi.ElStackPannel.Visibility = Visibility.Hidden;
                            }
                            item.Expand.Visibility = item.Children.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                        }
                        else
                        {
                            item.Expand.Visibility = item.Children.Count == 0 ? Visibility.Collapsed : Visibility.Visible;
                        }
                    }
                }
                SetEventsToSegments();
                Scope();
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }
        private void SetEventsToSegments()
        {
            foreach (var item in LinesList)
            {
                if (!item.IsRoot)
                {
                    foreach (var segmentLine in item.Segments)
                    {
                        segmentLine.MouseLeave += Risk_MouseLeave;
                        segmentLine.MouseEnter += Segment_MouseHover;

                        segmentLine.MouseDown += S_MouseDown_Event; //click en el segmento
                        segmentLine.MouseUp += RiskLine_MouseUp;
                    }
                }
            }
            Console.WriteLine("ok");
        }
        private static void SetPolyLineGroup(RiskPolyLine polyLine, DataRow item)
        {
            if (item[DT_Risk.ID_GROUPE] != DBNull.Value)
            {
                polyLine.Group = new LineGroup()
                {
                    IdGroup = (Int32)item[DT_Risk.ID_GROUPE],
                    GroupName = item[DT_Risk.GROUPE_NAME].ToString()
                };
            }
            else
            {
                polyLine.Group = new LineGroup()
                {
                    IdGroup = null,
                    GroupName = item[DT_Risk.GROUPE_NAME].ToString()
                };
            }
        }
        private RiskPolyLine CreateCounterMeasureShape(GridPaint gridPaint, ContextMenu contextMenu, bool isCMI, DataRow itemDataRow)
        {
            return new RiskPolyLine(gridPaint, contextMenu, isCMI)
            {
                IsCM = isCMI,
                Position = (Int32)itemDataRow[DT_CounterM.POSITION],
                ShortName = itemDataRow[DT_CounterM.NAMESHORT].ToString(),
                IdRiskFather = (Int32)itemDataRow[DT_CounterM.ID_RISK],
                ID = (Int32)itemDataRow[DT_CounterM.ID],
                Probability = (Decimal)itemDataRow[DT_CounterM.PROBABILITY] / 100,
                IsActivated = (Boolean)itemDataRow[DT_CounterM.ENABLED]
            };
        }
        private RiskPolyLine CreateRiskShape(GridPaint gridPaint, ContextMenu contextMenu, bool isCMI, DataRow itemDataRow)
        {
            return new RiskPolyLine(gridPaint, contextMenu, isCMI)
            {
                ShortName = itemDataRow[DT_Risk.NAMESHORT].ToString(),
                ID = (Int32)itemDataRow[DT_Risk.ID],
                Position = (Int32)itemDataRow[DT_Risk.POSITION],
                Collapsed = (Boolean)itemDataRow[DT_Risk.ISCOLLAPSED],
                Probability = (Decimal)itemDataRow[DT_Risk.PROBABILITY] / 100,
                IsActivated = (Boolean)itemDataRow[DT_Risk.ENABLED],
                StrokeThickness = 2,
                IsCM = isCMI,
                IdRiskFather = (Int32)itemDataRow[DT_Risk.IDRISK_FATHER],
                //thicknessProvider = thicknessProvider
            };
        }

        public void DrawEntireDiagram()
        {
            try
            {
                if (Ds.HasChanges())
                {
                    //((MainWindow)MyWindow).DsMain = Ds;
                    //((MainWindow)MyWindow).SaveData(false);
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                //UpdateCMCounText(-1 * p.CMCount);
                if (ChoosingRisk)
                {
                    if (RiskGroupSelected.Count == 1)
                    {
                        ResetLinesMenu(RiskGroupSelected, MenuRisk);
                    }
                    else
                    {
                        ResetLinesMenu(RiskGroupSelected, MenuGroupRisk);
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }
        public void ResetGroupRiksSelection()
        {
            try
            {
                ChoosingRisk = false;
                ResetLinesColor(RiskGroupSelected);
                ResetLinesMenu(RiskGroupSelected, MenuRisk);
                RiskGroupSelected.Clear();
                //UpdateRiskCounText(-1 * p.RiskCount);
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                            item.Stroke = new SolidColorBrush(Colors.Black);
                        }
                        else
                        {
                            item.Stroke = new SolidColorBrush(Colors.Gray);
                        }
                    }
                    else
                    {
                        if (item.IsActivated)
                        {
                            System.Drawing.Color drawingCColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram)[CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                            if (TengoPermiso(item))
                            {
                                item.Stroke = new SolidColorBrush(Color.FromArgb(drawingCColor.A, drawingCColor.R, drawingCColor.G, drawingCColor.B));
                            }
                            else
                            {
                                item.Stroke = new SolidColorBrush(Color.FromArgb(80, drawingCColor.R, drawingCColor.G, drawingCColor.B));
                            }
                        }
                        else
                        {
                            item.Stroke = new SolidColorBrush(Colors.Gray);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }
        public void ResetLinesMenu(List<RiskPolyLine> lista, ContextMenu menuParam)
        {
            // para asginar el menu a cada elemento de la lista antes de limiparla
            try
            {
                foreach (var item in lista)
                {
                    item.ContextMenu = menuParam;
                    item.MyName.ContextMenu = menuParam;
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
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

                string probability = "Prob.: " + General.MyRound(CMLine.Probability * 100, 2).ToString() + " %";

                Popin = new Popin(GridPaintLines, pointToShowPopup, "CM: " + CMLine.ShortName, probability, Value)
                {
                    Visibility = Visibility.Visible
                };
                if ((Boolean)Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(CMLine.ID)[DT_CounterM.ENABLED])
                {
                    ((MenuItem)MenuCM.Items[3]).ToolTip = StringResources.DisableValue;
                }
                else
                {
                    ((MenuItem)MenuCM.Items[3]).ToolTip = StringResources.EnableValue;
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                        //aca se lanza el evento cuando se selecciona CTRL+CLICK 
                        ChoosingCM = true;
                        SelectOneCM(sender);

                        if (!CMGroupSelected.Contains(Line_Selected))
                        {
                            if (ChoosingRisk) // seleccion mixta
                            {
                                ResetLinesMenu(RiskGroupSelected, MenuGroupMixed); // buscar forma mas eficiente, a partir de la segunda vez es innecesario
                                Line_Selected.ContextMenu = MenuGroupMixed;
                                Line_Selected.MyName.ContextMenu = MenuGroupMixed;
                            }
                            else
                            {
                                Line_Selected.ContextMenu = MenuGroupCM;
                                Line_Selected.MyName.ContextMenu = MenuGroupCM;
                            }
                            Line_Selected.Stroke = new SolidColorBrush(Colors.LightSkyBlue);
                            CMGroupSelected.Add(Line_Selected);
                            //UpdateCMCounText(1);
                        }
                        else
                        {
                            Line_Selected.ContextMenu = MenuCM;
                            Line_Selected.MyName.ContextMenu = MenuCM;

                            Line_Selected.Stroke = new SolidColorBrush((Line_Selected.IsActivated) ? Colors.Black : Colors.Gray);
                            CMGroupSelected.Remove(Line_Selected);
                            //UpdateCMCounText(-1);
                            if (CMGroupSelected.Count == 0)
                            {
                                ResetGroupCMSelection();
                            }
                        }
                    }
                    else // click izquierdo o derecho sin control presionado
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
                                //p.LSelected = Line_Selected.ShortName;
                                //p.TSelected = Line_Selected.IsCM ? "CounterMeasure" : "Risk";
                                Line_Selected.Stroke = new SolidColorBrush(Colors.LightSkyBlue);
                                CMGroupSelected.Add(Line_Selected);
                                Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_CounterM.ID_GROUPE] = GroupSelected.IdGroup;
                                Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_CounterM.GROUPE_NAME] = GroupSelected.GroupName;
                                LinesList.Find(x => x.ID == Line_Selected.ID).Group = GroupSelected;
                                //((MainWindow)MyWindow).SaveData();
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
                                                    ((MenuItem)MenuGroupMixed.Items[0]).ToolTip = StringResources.DisableValue;
                                                }
                                                else
                                                {
                                                    ((MenuItem)MenuGroupMixed.Items[0]).ToolTip = StringResources.EnableValue;
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
                                                    ((MenuItem)MenuGroupCM.Items[0]).ToolTip = StringResources.DisableValue;
                                                }
                                                else
                                                {
                                                    ((MenuItem)MenuGroupCM.Items[0]).ToolTip = StringResources.EnableValue;
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                    Line_Selected.MyName.Visibility = Visibility.Hidden;
                    TextChangeName.Margin = Line_Selected.ElStackPannel.Margin;
                    TextChangeName.Width = 150;
                    TextChangeName.MaxLength = 250;
                    TextChangeName.Background = new SolidColorBrush(Colors.Black);
                    TextChangeName.Foreground = new SolidColorBrush(Colors.White);
                    ManageTextChangeProperties(Line_Selected.ShortName, Visibility.Visible);
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                    TextChangeName.Margin = Line_Selected.ElStackPannel.Margin;
                    TextChangeName.Width = 150;
                    TextChangeName.MaxLength = 250;
                    TextChangeName.Background = new SolidColorBrush(Colors.Black);
                    TextChangeName.Foreground = new SolidColorBrush(Colors.White);
                    ManageTextChangeProperties(Line_Selected.ShortName, Visibility.Visible);
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }
        #endregion

        #region EventosRisk
        private void SelectOneRisk(object sender, MouseButtonEventArgs e, int ID_Sender, bool IsRoot_Sender)
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
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }

        }

        private void createRisk(int ID_Sender)
        {
            if (Line_Created.IsCM)
            {
                WindowCM windowCM = new WindowCM()
                {
                    CMRow = Ds.Tables[DT_CounterM.TABLE_NAME].NewRow(),
                    DsCM = Ds,
                    CM_RoleTable = Ds.Tables[DT_Role_CM.TABLENAME].Copy(),
                    CM_WBS_Table = Ds.Tables[DT_CM_WBS.TABLENAME].Copy(),
                    TopRiskTable = Ds.Tables[DT_CounterM_Damage.TABLENAME].Copy(),
                    Operation = General.INSERT,
                    RowFather = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(ID_Sender),
                    RiskTreeID = ID_Diagram,
                    RiskPadre = Line_Selected,
                    //Icon = Icon,
                    MyCM = Ds.Tables[DT_CounterM.TABLE_NAME].Copy()
                };
                windowCM.Probability = 0;
                if (windowCM.ShowDialog() == true)
                {
                    Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Add(windowCM.CMRow);
                    Ds.Tables[DT_CounterM_Damage.TABLENAME].Merge(windowCM.TopRiskTable);
                    Ds.Tables[DT_CM_WBS.TABLENAME].Merge(windowCM.CM_WBS_Table);
                    Ds.Tables[DT_Role_CM.TABLENAME].Merge(windowCM.CM_RoleTable);

                    Line_Created.ID = (Int32)windowCM.CMRow[DT_CounterM.ID];

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
                    ID_PROJECT = (Int32)Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.ID_PROJECT],
                    Ds = Ds,
                    Risk_RoleTable = Ds.Tables[DT_Role_Risk.TABLENAME].Copy(),
                    Risk_DamageTable = Ds.Tables[DT_Risk_Damages.TABLENAME].Copy(),
                    Risk_WBS_Table = Ds.Tables[DT_RISK_WBS.TABLENAME].Copy(),
                    Operation = General.INSERT,
                    RowFather = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID),
                    RiskTreeID = (Int32)Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.ID_DIAGRAM],
                    RiskSelected = Line_Selected,
                    //Icon = Icon,
                    MyRisks = Ds.Tables[DT_Risk.TABLE_NAME].Copy()
                };
                if (wrisk.ShowDialog() == true)
                {
                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(wrisk.RiskRow);
                    DataRow rowstructure = Ds.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                    rowstructure[DT_RiskStructure.IDRISK] = wrisk.RiskRow[DT_Risk.ID];
                    rowstructure[DT_RiskStructure.IDRISK_FATHER] = wrisk.RowFather[DT_Risk.ID];
                    Ds.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);
                    Ds.Tables[DT_Risk_Damages.TABLENAME].Merge(wrisk.Risk_DamageTable);
                    Ds.Tables[DT_RISK_WBS.TABLENAME].Merge(wrisk.Risk_WBS_Table);
                    Ds.Tables[DT_Role_Risk.TABLENAME].Merge(wrisk.Risk_RoleTable);

                    Line_Created.ID = (Int32)wrisk.RiskRow[DT_Risk.ID];
                    Line_Created.Father = Line_Selected;
                    Line_Created.IdRiskFather = Line_Selected.ID;
                    //Line_Selected.Children.Add(Line_Created);
                    Line_Created.FromTop = Line_Selected.FromTop;
                    Line_Created.IsDiagonal = !Line_Selected.IsDiagonal;
                    Line_Created.ShortName = wrisk.RiskRow[DT_Risk.NAMESHORT].ToString();
                    InsertRisk(Line_Created, Line_Selected, Line_Created.Points[1]);
                    DrawFishBone();
                }
            }
        }


        private void S_MouseDown_Event(object sender, MouseButtonEventArgs e)
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

                MouseDownPress(sender, e, ID_Sender, IsRoot_Sender, TheLine);
            }
            catch (Exception ex)
            {
                new WindowMessageOK("S_MouseDown_Event: " + ex.Message).ShowDialog();
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
                MouseDownPress(sender, e, ID_Sender, IsRoot_Sender, TheLine);
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void MouseDownPress(object sender, MouseButtonEventArgs e, int ID_Sender, bool IsRoot_Sender, RiskPolyLine TheLine)
        {
            if (Creando)
            {
                SelectOneRisk(sender, e, ID_Sender, IsRoot_Sender);

                createRisk(ID_Sender);

                GridPaintLines.Children.Remove(Line_Created);
                Line_Created = null;
                Creando = false;
                Loose = true;
            }
            else
            {
                if (MoviendoRisk)
                {
                    // Se mueve un riesgo

                    //checking if user don't bad click out of line for moving
                    //chekea que el usuario no de un mal clic 
                    if (Line_Selected.IdRiskFather != ID_Sender && Line_Selected.ID != ID_Sender)
                    {
                        //Mover el riesgo de un Padre a otro (cambiar de padre)
                        MoviendoRisk = false;
                        MoveRisk(TheLine, e.GetPosition(GridPaintLines));
                    }
                    else
                    {
                        //Mover el riesgo en el mismo Padre (Cambiar la posición, reordenar)
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
                        // Se mueve una contramedida

                        if (Line_Selected.IdRiskFather != ID_Sender && Line_Selected.ID != ID_Sender)
                        {
                            //Mover la contramedida de un Padre a otro (cambiar de padre)
                            MoviendoCM = false;
                            MoveCounterMeasure(TheLine, e.GetPosition(GridPaintLines));
                            DrawFishBone();
                            ((MainWindow)MyWindow).NormalArrowCursor();
                        }
                        else
                        {
                            //Mover la contramedida en el mismo Padre (Cambiar la posición)
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
                            SelectOneRisk(sender, e, ID_Sender, IsRoot_Sender);

                            if (!RiskGroupSelected.Contains(Line_Selected))
                            {
                                if (TengoPermiso(Line_Selected))
                                {
                                    if (ChoosingCM) // seleccion mixta
                                    {
                                        ResetLinesMenu(CMGroupSelected, MenuGroupMixed); // buscar forma mas eficiente, a partir de la segunda vez es innecesario
                                        Line_Selected.ContextMenu = MenuGroupMixed;
                                        Line_Selected.MyName.ContextMenu = MenuGroupMixed;
                                    }
                                    else
                                    {
                                        Line_Selected.ContextMenu = MenuGroupRisk;
                                        Line_Selected.MyName.ContextMenu = MenuGroupRisk;
                                    }
                                    Line_Selected.Stroke = new SolidColorBrush(Colors.LightSkyBlue);
                                    RiskGroupSelected.Add(Line_Selected);
                                    //UpdateRiskCounText(1);
                                }
                            }
                            else
                            {
                                System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_DAMAGE + " = " + (Int32)CbFilterTopR.SelectedValue).First()[DT_Risk_Damages.COLOR].ToString()));
                                if (TengoPermiso(Line_Selected))
                                {
                                    Line_Selected.Stroke = new SolidColorBrush(Color.FromArgb(drawColor.A, drawColor.R, drawColor.G, drawColor.B));
                                    Line_Selected.ContextMenu = MenuRisk;
                                    Line_Selected.MyName.ContextMenu = MenuRisk;
                                }
                                else
                                {
                                    Line_Selected.Stroke = new SolidColorBrush(Color.FromArgb(80, drawColor.R, drawColor.G, drawColor.B));
                                    Line_Selected.ContextMenu = null;
                                    Line_Selected.MyName.ContextMenu = null;
                                }

                                RiskGroupSelected.Remove(Line_Selected);
                                //UpdateRiskCounText(-1);

                                if (RiskGroupSelected.Count == 0) // si haciendo ctrl+click elimine la lista de riesgos seleccionados
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
                                    Line_Selected.Stroke = new SolidColorBrush(Colors.LightSkyBlue);
                                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.ID_GROUPE] = GroupSelected.IdGroup;
                                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.GROUPE_NAME] = GroupSelected.GroupName;
                                    LinesList.Find(x => x.ID == Line_Selected.ID).Group = GroupSelected;
                                    RiskGroupSelected.Add(Line_Selected);
                                    UpdateGridRiskAndGridCM();
                                    //p.LSelected = Line_Selected.ShortName;
                                    //p.TSelected = Line_Selected.IsCM ? "CounterMeasure" : "Risk";
                                }
                            }
                            else
                            {
                                switch (e.LeftButton == MouseButtonState.Pressed)
                                {
                                    case true: // click izquierdo sin control presionado
                                        SelectOneRisk(sender, e, ID_Sender, IsRoot_Sender);
                                        if (ChoosingCM || ChoosingRisk)
                                        { // si estaba seleccionando multiple y di click izq
                                            ResetGroupCMSelection();
                                            ResetGroupRiksSelection();
                                        }
                                        break;

                                    case false: // click derecho sin control presionado en una cm
                                        SelectOneRisk(sender, e, ID_Sender, IsRoot_Sender);
                                        if (ChoosingCM || ChoosingRisk) // si estaba seleccionando limpio seleccion
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
                                                        ((MenuItem)MenuGroupMixed.Items[0]).ToolTip = StringResources.DisableValue;
                                                    }
                                                    else
                                                    {
                                                        ((MenuItem)MenuGroupMixed.Items[0]).ToolTip = StringResources.EnableValue;
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
                                                        ((MenuItem)MenuGroupRisk.Items[0]).ToolTip = StringResources.DisableValue;
                                                    }
                                                    else
                                                    {
                                                        ((MenuItem)MenuGroupRisk.Items[0]).ToolTip = StringResources.EnableValue;
                                                    }
                                                }
                                            }
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
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        public void RiskLeave()
        {
            LineThickness();
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
            TheLine.StrokeThickness = 10;
            // se recorre la lista de todos los hermanos, incluyendose el que se genero el evento.
            foreach (SegmentPolyLine segmentLine in TheLine.Segments)
            {
                segmentLine.StrokeThickness = 10;
            }

        }

        public void RiskEnter(RiskPolyLine TheLine, Point pointToShowPopup)
        {
            try
            {
                //TheLine.StrokeThickness = 10;
                if (!(MoviendoRisk || MoviendoCM))
                {
                    if (Creando)
                    {
                        //Point p = Line_Created.StartDrawPoint;
                        Line_Created.IsDiagonal = !TheLine.IsDiagonal;
                        Line_Created.FromTop = TheLine.FromTop;
                        Line_Created.DrawSingleLine();
                    }
                    decimal el = General.AcumulatedLikelihood(TheLine);
                    decimal valor = General.CalculateTopRiskTreeValue(Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheLine.ID),
                        Ds.Tables[DT_Risk.TABLE_NAME], (Int32)CbFilterTopR.SelectedValue, Ds.Tables[DT_Risk_Damages.TABLENAME],
                            Ds.Tables[DT_CounterM.TABLE_NAME], Ds.Tables[DT_CounterM_Damage.TABLENAME]);
                    string AcumValue = StringResources.ACUM_VALUE + General.MyRound(valor, 4);
                    string Valuee = "";
                    if (Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { TheLine.ID, (Int32)CbFilterTopR.SelectedValue }))
                    {
                        Valuee = StringResources.VALUE + Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Find(new object[] { TheLine.ID, (Int32)CbFilterTopR.SelectedValue })[DT_Risk_Damages.VALUE].ToString();
                    }
                    else
                    {
                        Valuee = StringResources.VALUE + "0";
                    }

                    decimal AcumDamage = 0;
                    foreach (var itemI in TreeOperation.GetMeAndAllChildsWithCM(TheLine))
                    {
                        if (itemI.IsActivated)
                        {
                            decimal value = 0;
                            if (itemI.IsCM)
                            {
                                if (itemI.IsActivated)
                                {
                                    if (Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Contains(new object[] { itemI.ID, (Int32)CbFilterTopR.SelectedValue }))
                                    {
                                        value = (Decimal)Ds.Tables[DT_CounterM_Damage.TABLENAME].Rows.Find(new object[] { itemI.ID, (Int32)CbFilterTopR.SelectedValue })[DT_CounterM_Damage.VALUE];
                                    }
                                    AcumDamage += value;
                                }
                            }
                            else
                            {
                                if (Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Contains(new object[] { itemI.ID, (Int32)CbFilterTopR.SelectedValue }))
                                {
                                    value = (Decimal)Ds.Tables[DT_Risk_Damages.TABLENAME].Rows.Find(new object[] { itemI.ID, (Int32)CbFilterTopR.SelectedValue })[DT_Risk_Damages.VALUE];
                                }
                                AcumDamage += value * General.AcumulatedLikelihood(itemI);
                            }
                        }
                    }
                    string ED = StringResources.ACUM_DAMAGE + General.MyRound(AcumDamage, 4);
                    string probability = StringResources.PROBABILITY + General.MyRound(TheLine.Probability * 100, 2).ToString() + " %";
                    string EL = StringResources.ACUM_LIKELIHOOD + General.MyRound(el * 100, 2).ToString() + " %";
                    Popin = new Popin(GridPaintLines, pointToShowPopup, "Risk: " + TheLine.ShortName, probability, EL, Valuee, AcumValue, ED)
                    {
                        Visibility = Visibility.Visible
                    };
                    if ((Boolean)Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(TheLine.ID)[DT_Risk.ENABLED])
                    {
                        ((MenuItem)MenuRisk.Items[9]).ToolTip = StringResources.DisableValue;
                    }
                    else
                    {
                        ((MenuItem)MenuRisk.Items[9]).ToolTip = StringResources.EnableValue;
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }

        }

        private void Risk_LabelName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (TengoPermiso(Line_Selected) && Line_Selected.IsActivated)
                {
                    NameEditing = true;
                    Loose = true;
                    Line_Selected.ExtrasVisibility(Visibility.Hidden);
                    TextChangeName.Width = 150;
                    TextChangeName.MaxLength = 250;
                    TextChangeName.Background = new SolidColorBrush(Colors.Black);
                    TextChangeName.Foreground = new SolidColorBrush(Colors.White);
                    TextChangeName.Margin = Line_Selected.ElStackPannel.Margin;
                    ManageTextChangeProperties(Line_Selected.ShortName, Visibility.Visible);
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
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

            TreeOperation.BackupLine(Line_Selected, destinationPolyLine.ID, Ds);
            TreeOperation.DeleteLine(Line_Selected, LinesList, Ds);
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

            TreeOperation.BackupLine(Line_Selected, destinationPolyLine.ID, Ds);
            TreeOperation.DeleteLine(Line_Selected, LinesList, Ds);
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

        private void ReorderCounterMeasure(RiskPolyLine sourceRisk, RiskPolyLine destinationRisk, Point point)
        {
            sourceRisk.Children.Remove(Line_Selected);
            SetPolyLinePosition(sourceRisk.Children);

            int pos = TreeOperation.DetectClickPosition(point, destinationRisk);
            int lastCounterMeasurePosition = TreeOperation.LastCounterMeasurePosition(destinationRisk.Children);
            if (pos > lastCounterMeasurePosition)
            {
                pos = lastCounterMeasurePosition + 1;
            }

            destinationRisk.Children.Insert(pos, Line_Selected);
            SetPolyLinePosition(destinationRisk.Children);
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
            LineThickness();
        }

        private void UpdateGridRiskAndGridCM()
        {
            ((MainWindow)MyWindow).CruzarTablaRisk(Ds);
            ((MainWindow)MyWindow).CruzarTablaCM(Ds);
        }

        public void SetLineSelectedChildrenPosition()
        {
            for (int i = 0; i < Line_Selected.Children.Count; i++)
            {
                if (Line_Selected.Children[i].IsCM)
                {
                    Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(Line_Selected.Children[i].ID)[DT_CounterM.POSITION] = i;
                }
                else
                {
                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.Children[i].ID)[DT_Risk.POSITION] = i;
                }
                Line_Selected.Children[i].Position = i;
            }
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
                    ((PictureBoxPolyLine)sender).Source = new BitmapImage(new Uri(General.EXPANDIDO));
                    ((PictureBoxPolyLine)sender).Risk.Collapsed = false;
                }
                else
                {
                    ((PictureBoxPolyLine)sender).Source = new BitmapImage(new Uri(General.CONTRAIDO));
                    ((PictureBoxPolyLine)sender).Risk.Collapsed = true;
                }
                Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(((PictureBoxPolyLine)sender).Risk.ID)[DT_Risk.ISCOLLAPSED] = ((PictureBoxPolyLine)sender).Risk.Collapsed;

                CleanFishBone();
                LoadFishBone();
                DrawNumbersAndLineThickness();
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                    //Loose = false;
                    //idRiskSelected = MainLine.ID;
                    //isRootSelected = true;
                    //Line_Selected = MainLine;
                    MenuRisk.IsEnabled = true;
                    int ID_Sender;
                    bool IsRoot_Sender;
                    //Loose = true;
                    if (sender is RiskPolyLine)
                    {
                        //Line_Selected = (RiskPolyLine)sender;
                        ID_Sender = ((RiskPolyLine)sender).ID;
                        IsRoot_Sender = ((RiskPolyLine)sender).IsRoot;
                    }
                    else
                    {
                        //MainLine = ((LabelPolyLine)sender).Line;
                        ID_Sender = ((LabelPolyLine)sender).Line.ID;
                        IsRoot_Sender = ((LabelPolyLine)sender).Line.IsRoot;
                    }
                    if (Creando)
                    {
                        Loose = false;
                        SelectOneRisk(sender, e, ID_Sender, IsRoot_Sender);
                        if (!(Line_Created.IsCM))
                        {
                            WindowRisk wrisk = new WindowRisk()
                            {
                                RiskRow = Ds.Tables[DT_Risk.TABLE_NAME].NewRow(),
                                ID_PROJECT = (Int32)Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.ID_PROJECT],
                                Ds = Ds,
                                Risk_RoleTable = Ds.Tables[DT_Role_Risk.TABLENAME].Copy(),
                                Risk_DamageTable = Ds.Tables[DT_Risk_Damages.TABLENAME].Copy(),
                                Risk_WBS_Table = Ds.Tables[DT_RISK_WBS.TABLENAME].Copy(),
                                Operation = General.INSERT,
                                RowFather = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID),
                                RiskTreeID = (Int32)Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.ID_DIAGRAM],
                                RiskSelected = Line_Selected,
                                //Icon = Icon,
                                MyRisks = Ds.Tables[DT_Risk.TABLE_NAME].Copy()
                            };
                            if (wrisk.ShowDialog() == true)
                            {
                                if (LinesList.FindAll(rl => rl.IdRiskFather == Line_Selected.ID).Any())
                                {
                                    wrisk.RiskRow[DT_Risk.POSITION] = Line_Selected.Children.Count;
                                }
                                else
                                {
                                    wrisk.RiskRow[DT_Risk.POSITION] = 0;
                                }
                                Ds.Tables[DT_Risk.TABLE_NAME].Rows.Add(wrisk.RiskRow);
                                DataRow rowstructure = Ds.Tables[DT_RiskStructure.TABLE_NAME].NewRow();
                                rowstructure[DT_RiskStructure.IDRISK] = wrisk.RiskRow[DT_Risk.ID];
                                rowstructure[DT_RiskStructure.IDRISK_FATHER] = wrisk.RowFather[DT_Risk.ID];
                                Ds.Tables[DT_RiskStructure.TABLE_NAME].Rows.Add(rowstructure);
                                Ds.Tables[DT_Risk_Damages.TABLENAME].Merge(wrisk.Risk_DamageTable);
                                Ds.Tables[DT_RISK_WBS.TABLENAME].Merge(wrisk.Risk_WBS_Table);
                                Ds.Tables[DT_Role_Risk.TABLENAME].Merge(wrisk.Risk_RoleTable);
                                DrawFishBone();
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
                            //checking if user don't bad click out of line for moving
                            //chekea que el usuario no de un mal clic 
                            if (Line_Selected.IdRiskFather != ID_Sender && Line_Selected.ID != ID_Sender)
                            {
                                MoviendoRisk = false;
                                if (Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains(Line_Selected.ID))
                                {
                                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.IDRISK_FATHER] = ID_Sender;
                                }
                                Ds.Tables[DT_RiskStructure.TABLE_NAME].Select(DT_RiskStructure.IDRISK + " = " + Line_Selected.ID).First()[DT_RiskStructure.IDRISK_FATHER] = ID_Sender;
                                foreach (var item in LinesList.FindAll(rl => rl.IdRiskFather == Line_Selected.IdRiskFather && rl.Position > Line_Selected.Position).ToList())
                                {//decremento en 1 la posicion de los risk, pa las cm no hace falta pues estan delante
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
                                //Cursor = Cursors.Arrow;
                                TreeOperation.DetectarMiPosicionActualInMain(Line_Selected, e.GetPosition(GridPaintLines), Ds);
                                DrawFishBone();
                            }
                            else
                            {
                                if (Line_Selected.IdRiskFather == ((RiskPolyLine)sender).ID)
                                {
                                    MoviendoRisk = false;
                                    //Cursor = Cursors.Arrow;
                                    TreeOperation.DetectarMiPosicionActual(Line_Selected, e.GetPosition(GridPaintLines), Ds);
                                    DrawFishBone();
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
                new WindowMessageOK(ex.Message).ShowDialog();
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
                    ID_Sender = ((LabelPolyLine)sender).Line.ID;
                    IsRoot_Sender = ((LabelPolyLine)sender).Line.IsRoot;
                    TheLine = ((LabelPolyLine)sender).Line;
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
                    }
                    else
                    {
                        if (Line_Selected.IdRiskFather == ID_Sender)
                        {
                            MoviendoRisk = false;
                            //Cursor = Cursors.Arrow;
                            if (!(TheLine.Children.Count == 1 && Line_Selected == TheLine.Children[0]))
                            {
                                TreeOperation.DetectarMiPosicionActual(Line_Selected, e.GetPosition(GridPaintLines), Ds);
                                //DrawFishBone();
                            }
                            DrawFishBone();
                        }
                        else
                        {
                            MoviendoRisk = false;
                            MoviendoCM = false;
                            //Cursor = Cursors.Arrow;
                            TreeOperation.DrawEntireDiagramAsFishBone(MainLine);
                            //TreeOperation.FixMainLine(LinesList, MainLine);
                            FixDrawPanel();
                            LineThickness();
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

        #region GridEvents

        /// <summary>
        /// Trigger when the mouse move over the Paint Area
        /// </summary>
        private void GridPaintLines_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {

                if (((MainWindow)MyWindow).CurrentLayout.ID_Diagram != ID_Diagram)
                {
                    ((MainWindow)MyWindow).CurrentLayout = this;
                    //Ds = ((MainWindow)MyWindow).DsMain;
                    //DrawFishBone();
                }

                foreach (var item in ((MainWindow)MyWindow).OpenedDocuments)
                {
                    item.ExitWorking();
                }
                this.EnterWorking();
                if (!(NameEditing))
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        if (!(Loose))
                        {
                            if (Line_Selected.IsCM)
                            {
                                if (!(MoviendoCM))
                                {
                                    MoviendoCM = true;
                                    LinesMoving = new List<RiskPolyLine>() { Line_Selected };
                                    //Cursor = Cursors.Hand;
                                    //VerticalMenu win = new VerticalMenu("Moving");
                                    //win.ShowDialog();
                                }
                                TreeOperation.MoveLines(LinesMoving, e.GetPosition(GridPaintLines).X - Line_Selected.Points[1].X - 35, e.GetPosition(GridPaintLines).Y - Line_Selected.Points[1].Y);
                                X = e.GetPosition(GridPaintLines).X;
                                Y = e.GetPosition(GridPaintLines).Y;
                                Main_Y = MainLine.Points[0].Y;
                            }
                            else
                            {
                                if (!(MoviendoRisk))
                                {
                                    MoviendoRisk = true;
                                    LinesMoving = new List<RiskPolyLine>();
                                    LinesMoving.AddRange(TreeOperation.GetMeAndAllChildsWithCM(Line_Selected));
                                    //Cursor = Cursors.Hand;
                                    //VerticalMenu win = new VerticalMenu("Moving");
                                    //win.ShowDialog();
                                }
                                if (MoviendoRisk)
                                {
                                    TreeOperation.MoveLines(LinesMoving, e.GetPosition(GridPaintLines).X - Line_Selected.Points[1].X - 35, e.GetPosition(GridPaintLines).Y - Line_Selected.Points[1].Y);
                                    X = e.GetPosition(GridPaintLines).X;
                                    Y = e.GetPosition(GridPaintLines).Y;
                                    Main_Y = MainLine.Points[0].Y;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (MoviendoRisk || MoviendoCM)
                        {
                            TreeOperation.MoveLines(LinesMoving, e.GetPosition(GridPaintLines).X - Line_Selected.Points[1].X - 35, e.GetPosition(GridPaintLines).Y - Line_Selected.Points[1].Y);
                            X = e.GetPosition(GridPaintLines).X;
                            Y = e.GetPosition(GridPaintLines).Y;
                            Main_Y = MainLine.Points[0].Y; //Si se movio el arbol, regista la posicion de las lineas
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
                            else
                            {

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
        /// Trigger when mouse click in Draw Area
        /// </summary>
        private void GridPaintLines_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (((MainWindow)MyWindow).CurrentLayout.ID_Diagram != ID_Diagram)
                {
                    ((MainWindow)MyWindow).CurrentLayout = this;
                    //Ds = ((MainWindow)MyWindow).DsMain;
                    CleanFishBone();
                    LoadFishBone();
                    DrawNumbersAndLineThickness();
                    ((MainWindow)MyWindow).TextProbabilityChange(MainLine);
                }

                UpdateGridRiskAndGridCM();
                foreach (var item in ((MainWindow)MyWindow).OpenedDocuments)
                {
                    item.ExitWorking();
                }
                this.EnterWorking();
                if (Creando)
                {
                    GridPaintLines.Children.Remove(Line_Created);
                    Line_Created.ElStackPannel.Children.Clear();
                    GridPaintLines.Children.Remove(Line_Created.ElStackPannel);
                    Creando = false;
                }
                if (MoviendoRisk || MoviendoCM || Creando)
                {
                    MoviendoRisk = false;
                    Creando = false;
                    Line_Created = null;
                    MoviendoCM = false;
                    //Cursor = Cursors.Arrow;
                    //TreeOperation.DrawEntireDiagramAsFishBone(MainLine);
                    //TreeOperation.FixMainLine(LinesList, MainLine);
                    CleanFishBone();
                    LoadFishBone();
                    DrawNumbersAndLineThickness();
                    FixDrawPanel();
                    //LineThickness();
                    UpdateGridRiskAndGridCM();
                }
                else
                {
                    if (SelectingToGroup)
                    {
                        SelectingToGroup = false;
                        GroupSelected = null;
                        //Cursor = Cursors.Arrow;
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
                            // clic en el area amarilla sin seleccion multiple
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void GridPaintLines_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Loose = true;
                if (MoviendoRisk || MoviendoCM)
                {
                    MoviendoRisk = false;
                    Creando = false;
                    MoviendoCM = false;
                    DrawFishBone();
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                //BrigIntoViewSelectedRiskPolyline();
                Point mouseAtImage = e.GetPosition(GridPaintLines); // ScrollViewer_CanvasMain.TranslatePoint(middleOfScrollViewer, Canvas_Main);
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

                Vector offset = GridPaintLines.TranslatePoint(mouseAtImage, ScrollGridPaint) - mouseAtScrollViewer; // (Vector)middleOfScrollViewer;
                ScrollGridPaint.ScrollToHorizontalOffset(offset.X);
                ScrollGridPaint.ScrollToVerticalOffset(offset.Y);
                MyWindow.UpdateLayout();

                e.Handled = true;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
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
                new WindowMessageOK(ex.Message).ShowDialog();
            }
        }

        private void BtnPlusZoom_Click(object sender, RoutedEventArgs e)
        {
            if (SliderZoom.Value < 200)
            {
                SliderZoom.Value += 10;
            }
            //BrigIntoViewSelectedRiskPolyline();
        }

        private void BtnMinusZoom_Click(object sender, RoutedEventArgs e)
        {
            if (SliderZoom.Value > 10)
            {
                SliderZoom.Value -= 10;
            }
            //BrigIntoViewSelectedRiskPolyline();
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

                //System.Windows.Shapes.Line myLine = new System.Windows.Shapes.Line();
                //myLine.Stroke = System.Windows.Media.Brushes.Black;
                //myLine.Fill = System.Windows.Media.Brushes.SkyBlue;
                //myLine.X1 = x0;
                //myLine.Y1 = y0;
                //myLine.X2 = x1;
                //myLine.Y2 = y1;
                //GridPaintLines.Children.Add(myLine);

                line.BringIntoView(rectangleRPL);
            }
        }


        #endregion

        #region Export to Excel

        public void ExportToExcel(string fileName)
        {
            this.TheProgressBar.IsIndeterminate = false;
            this.TheProgressBar.Minimum = 0;
            this.TheProgressBar.Maximum = 100;
            this.TheProgressBar.Visibility = Visibility.Visible;

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
            new WindowMessageInformation(String.Format(formatProvider, "Diagram {0} was saved as excel file!", this.Title)).ShowDialog();
            //MessageBox.Show(String.Format(formatProvider, "Diagram {0} was saved as excel file!", this.Title));
            this.TheProgressBar.Visibility = Visibility.Hidden;
            this.TheProgressBar.Value = 0;
            this.TheProgressBar.IsIndeterminate = true;

            this.IsExportingToExcel = false;
        }
        #endregion
    }
}
