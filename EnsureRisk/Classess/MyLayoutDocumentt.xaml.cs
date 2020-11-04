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
using ERDock.Layout;

namespace EnsureRisk.Classess
{
    /// <summary>
    /// Interaction logic for MyLayoutDocumentt.xaml
    /// This class visualize fishbone diagram, diagram minimap
    /// </summary>
    public partial class MyLayoutDocumentt : LayoutDocument
    {
        public string LoginUser { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Main_Y { get; set; }
        public Point PointSelected { get; set; }
        private double zoomValue;
        public double ZoomValue { get { return zoomValue; } set { zoomValue = value; RaisePropertyChanged("ZoomValue"); } }
        private decimal idDamageSelected;
        private DataView dvDamage;
        public DataView DvDamage { get => dvDamage; set { dvDamage = value; RaisePropertyChanged("DvDamage"); } }
        public decimal IdDamageSelected { get => idDamageSelected; set { idDamageSelected = value; RaisePropertyChanged("IdDamageSelected"); } }
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
        public bool Copiando { get { return ((MainWindow)MyWindow).COPIANDO; } }
        public bool IsExportingToExcel { get; set; }
        public LineGroup GroupSelected { get; set; }
        public decimal ID_Diagram { get; set; }
        //public Popin Popin { get; set; }
        public Point MIdPoint { get; set; }
        public DataSet Ds { get; set; }
        public DataRow DrDiagram { get; set; }
        public Window MyWindow { get; set; }
        private readonly BackgroundWorker exportToExcelWorker = new BackgroundWorker();
        #endregion        

        #region Menus
        public ContextMenu MenuRisk { get; set; }
        //public ContextMenu MenuRiskLimited { get; set; }
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
        public Dictionary<decimal, bool> LinesListCMState { get; set; }
        #endregion

        #region RiskPolyLines
        public RiskPolyLine ScopeLine { get; set; }
        public RiskPolyLine CopyRisk { get; set; }
        public RiskPolyLine Line_Selected { get; set; }
        public RiskPolyLine MainLine { get; set; }
        public RiskPolyLine Line_Created { get; set; }
        public RiskPolyLine LineInMoving { get; set; }
        #endregion
        /// <summary>
        /// Initializes a new instance of the MyLayoutDocumentt class
        /// </summary>
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
            LinesListCMState = new Dictionary<decimal, bool>();
            //IsSelected = true;
            Loose = true;
            Binding myBinding = new Binding
            {
                Source = SliderZoom,
                Path = new PropertyPath("Value"),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };
            BindingOperations.SetBinding(TheZoomComboBox, ComboBox.TextProperty, myBinding);
            TheZoomComboBox.Text = "100";
            MoviendoRisk = MoviendoCM = NameEditing = ChoosingCM = ChoosingRisk = IsRootSelected = SelectingToGroup = Creando = false;            
            EnterWorking();
            exportToExcelWorker.WorkerReportsProgress = true;
            exportToExcelWorker.WorkerSupportsCancellation = true;
            exportToExcelWorker.DoWork += ExportToExcelWorker_DoWork;
            exportToExcelWorker.ProgressChanged += ExportToExcelWorker_ProgressChanged;
            exportToExcelWorker.RunWorkerCompleted += ExportToExcelWorker_RunWorkerCompleted;
            CbFilterTopR.DataContext = this;
            MiniMapGHT.DataContext = this;
            MiniMapGHT.MapSource = this.ScrollGridPaint;
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
                        if (new WindowMessageYesNo("Do you want to save the changes on [" + this.Title + "]").ShowDialog() == true)
                        {
                            ((MainWindow)MyWindow).SaveData(Ds, true);
                        }
                        else
                        {
                            Ds.RejectChanges();
                        }
                    }
                }
                if (((MainWindow)MyWindow).OpenedDocuments.Contains(this))
                {
                    ((MainWindow)MyWindow).OpenedDocuments.Remove(this);
                } 
                ((MainWindow)MyWindow).ShowCMData = false;
                ((MainWindow)MyWindow).ShowRiskData = false;
                ((MainWindow)MyWindow).DV_CrossRisk.Table.Clear();
                ((MainWindow)MyWindow).DV_Cross_CM.Table.Clear();
            }
            catch (Exception ex)
            {
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

        //TODO: Documentar
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
                    //((MainWindow)MyWindow).TextProbabilityChange(LinesList.Find(x => x.ID == ScopeLine.ID));
                    Title = LinesList.Find(x => x.ID == ScopeLine.ID).ShortName;
                    //((MainWindow)MyWindow).TextDiagram.Text = LinesList.Find(x => x.ID == ScopeLine.ID).ShortName;
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
        /// <summary>
        /// Update Diagram damage dropdown
        /// </summary>
        public void LoadComboDamage()
        {
            try
            {
                DataRow[] thisTopRisk = Ds.Tables[DT_Diagram_Damages.TABLE_NAME]
                        .Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram);

                //CbFilterTopR.SelectedValuePath = DT_Diagram_Damages.ID_DAMAGE;
                DvDamage = thisTopRisk.CopyToDataTable().DefaultView;
                //CbFilterTopR.ItemsSource = thisTopRisk.CopyToDataTable().DefaultView;
                //CbFilterTopR.DisplayMemberPath = DT_Diagram_Damages.DAMAGE;
                CbFilterTopR.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        /// <summary>
        /// Adjust draw panel size to show fishbone diagram.
        /// </summary>
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


                int cantidadRectangulos = Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram).Length;
                double altoRectangulos = cantidadRectangulos * 30;
                double anchoRectangulos = cantidadRectangulos * 15;

                double altoLineas = LinesList.OrderByDescending(rl => rl.Points[1].Y).First().Points[1].Y;
                double anchoLineas = LinesList.OrderByDescending(rl => rl.Points[1].X).First().Points[1].X;
                double valorMasAlto = (altoRectangulos > (altoLineas / 2)) ? (altoRectangulos * 2) : altoLineas;

                GridPaintLines.Height = valorMasAlto + 300;
                GridPaintLines.Width = anchoLineas + anchoRectangulos + 300;

                if (altoRectangulos > (altoLineas / 2))
                {
                    double desplY = altoRectangulos - (altoLineas / 2);
                    TreeOperation.MoveEntireTree(LinesList, 0, (int)desplY, Rectangles);
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        /// <summary>
        /// Show damage properties like damage name, accumulated damage, damage risk, damage countermeasure and maximum damage into rectangle
        /// </summary>
        public void LoadRectangles()
        {
            try
            {
                RiskPolyLine MyMainLine = IsScoping ? LinesList.Find(x => x.ID == ScopeLine.ID) : MainLine;
                Rectangles = new List<MyDamage>();
                DataRow[] thisTopRisk = Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram);

                double puntoinicialY = MyMainLine.Points[1].Y - 45;
                puntoinicialY -= thisTopRisk.Length * 30;
                double puntoinicialX = MyMainLine.Points[1].X - 5 + General.MaxThickness;
                puntoinicialX += thisTopRisk.Length * 15;

                //foreach (DataRow item in thisTopRisk)
                for (int i = thisTopRisk.Length - 1; i >= 0; i--)
                {
                    DataRow item = thisTopRisk[i];
                    //System.Drawing.Color colorete = System.Drawing.Color.FromArgb(int.Parse(item[DT_Diagram_Damages.COLOR].ToString()));
                    //Color mediaColor = Color.FromArgb(colorete.A, colorete.R, colorete.G, colorete.B);
                    Color mediaColor = ((SolidColorBrush)new BrushConverter().ConvertFrom(item[DT_Diagram_Damages.COLOR].ToString())).Color;

                    MyDamage rectangle = new MyDamage(GridPaintLines, new Point(puntoinicialX, puntoinicialY),
                                                        item[DT_Diagram_Damages.DAMAGE].ToString(),
                                                        StringResources.ACUM_VALUE,
                                                        StringResources.ACUM_DAMAGE,
                                                        StringResources.ACUM_RISK_DAMAGE,
                                                        StringResources.ACUM_CM_DAMAGE,
                                                        mediaColor, (decimal)item[DT_Diagram_Damages.ID_DAMAGE])
                    { UM = item[DT_Diagram_Damages.UM].ToString() };


                    rectangle.MouseDown += MyDamage_MouseDown;
                    Rectangles.Add(rectangle);
                    puntoinicialY += 30;
                    puntoinicialX -= 15;
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        public void ReLoadRectangles()
        {
            try
            {
                RiskPolyLine MyMainLine = IsScoping ? LinesList.Find(x => x.ID == ScopeLine.ID) : MainLine;

                MyDamage[] MyRectangles = new MyDamage[Rectangles.Count];
                Rectangles.CopyTo(MyRectangles);
                Rectangles.Clear();

                double puntoinicialY = MyMainLine.Points[1].Y - 45;
                puntoinicialY -= MyRectangles.Length * 30;
                double puntoinicialX = MyMainLine.Points[1].X - 5 + General.MaxThickness;
                puntoinicialX += MyRectangles.Length * 15;

                //for (int i = MyRectangles.Length - 1; i >= 0; i--)
                for (int i = 0; i < MyRectangles.Length; i++)
                {
                    MyDamage item = MyRectangles[i];
                    Color mediaColor = item.Colorr;

                    MyDamage rectangle = new MyDamage(GridPaintLines, new Point(puntoinicialX, puntoinicialY),
                                                        item.HeadeValue.Text,
                                                        item.CalculateValue.Text,
                                                        StringResources.ACUM_DAMAGE,
                                                        StringResources.ACUM_RISK_DAMAGE,
                                                        StringResources.ACUM_CM_DAMAGE,
                                                        mediaColor, item.ID_TopRisk)
                    { UM = item.UM };

                    rectangle.MouseDown += MyDamage_MouseDown;
                    Rectangles.Add(rectangle);
                    puntoinicialY += 30;
                    puntoinicialX -= 15;

                }

            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        ///// <summary>
        ///// Version anterior, carga los rectangulos en una fila vertical
        ///// </summary>
        //public void LoadRectanglesVertical()
        //{
        //    try
        //    {
        //        RiskPolyLine MyMainLine = IsScoping ? LinesList.Find(x => x.ID == ScopeLine.ID) : MainLine;
        //        Rectangles = new List<MyDamage>();
        //        DataRow[] thisTopRisk = Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram);
        //        double formula = 90 * thisTopRisk.Length / 2;
        //        double puntoinicialY = MyMainLine.Points[1].Y - formula;
        //        foreach (DataRow item in thisTopRisk)
        //        {
        //            Color mediaColor = ((SolidColorBrush)new BrushConverter().ConvertFrom(item[DT_Diagram_Damages.COLOR].ToString())).Color;

        //            //System.Drawing.Color colorete = System.Drawing.Color.FromArgb(int.Parse(item[DT_Diagram_Damages.COLOR].ToString()));
        //            //Color mediaColor = Color.FromArgb(colorete.A, colorete.R, colorete.G, colorete.B);

        //            MyDamage rectangle = new MyDamage(GridPaintLines,
        //            new Point(MyMainLine.Points[1].X + 5 + General.MaxThickness, puntoinicialY),
        //            item[DT_Diagram_Damages.DAMAGE].ToString(),
        //            StringResources.ACUM_VALUE,
        //            StringResources.ACUM_DAMAGE,
        //            StringResources.ACUM_RISK_DAMAGE,
        //            StringResources.ACUM_CM_DAMAGE,
        //            mediaColor, (decimal)item[DT_Diagram_Damages.ID_DAMAGE]);

        //            rectangle.MouseDown += MyDamage_MouseDown;
        //            Rectangles.Add(rectangle);
        //            puntoinicialY += 90;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MostrarDialog(ex.Message);
        //    }
        //}

            /// <summary>
            /// Update value damage properties
            /// </summary>
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
                        RiskValue = FishHeadController.CalcDiagramDamageValue(Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(MyMainLine.ID),
                            Ds.Tables[DT_Risk.TABLE_NAME], item.ID_TopRisk, Ds.Tables[DT_Risk_Damages.TABLE_NAME],
                            Ds.Tables[DT_CounterM.TABLE_NAME], Ds.Tables[DT_CounterM_Damage.TABLE_NAME]);
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
                                    if (Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Contains(new object[] { itemI.ID, item.ID_TopRisk }))
                                    {
                                        value = (decimal)Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Find(new object[] { itemI.ID, item.ID_TopRisk })[DT_CounterM_Damage.VALUE];
                                    }
                                    AcumDamage += value;
                                }
                            }
                            else
                            {
                                if (itemI.IsActivated)
                                {
                                    if (Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { itemI.ID, item.ID_TopRisk }))
                                    {
                                        value = (decimal)Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Find(new object[] { itemI.ID, item.ID_TopRisk })[DT_Risk_Damages.VALUE];
                                    }
                                    AcumDamage += value * FishHeadController.AcumulatedLikelihood(itemI);
                                }
                            }
                        }
                        decimal AcumulatedRiskDamage = LinesList.Sum(Risk => (!Risk.IsCM ? Risk.AcValue : 0));
                        decimal AcumulatedCounterMeasureDamage = LinesList.Sum(Risk => (Risk.IsCM ? Risk.AcValue : 0));

                        item.ExpecteDamage.Text = StringResources.ACUM_DAMAGE + General.MyRound(AcumDamage, 2).ToString() + " " + item.UM;
                        item.RisksDamageValue.Text = String.Format(StringResources.ACUM_RISK_DAMAGE, AcumulatedRiskDamage.ToString(), item.UM);
                        item.CounterMesuareDamageValue.Text = String.Format(StringResources.ACUM_CM_DAMAGE, AcumulatedCounterMeasureDamage.ToString(), item.UM);
                        item.CalculateValue.Text = StringResources.ACUM_VALUE + General.MyRound(RiskValue, 2).ToString() + " " + item.UM;
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        /// <summary>
        /// Update the line Thickness of the Diagram according with its values
        /// </summary>
        public void SetLinesThickness()
        {
            try
            {
                if (!(CbFilterTopR.SelectedValue is null))
                {
                    //IdDamageSelected = IdDamageSelected;
                    if (IdDamageSelected != 0)
                    {
                        General.UpdateLinesThickness(LinesList);
                        foreach (RiskPolyLine polyLine in LinesList)
                        {
                            if (!polyLine.IsCM)
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

        /// <summary>
        /// Create fishbone main line.
        /// </summary>
        /// <param name="dr">Datarow that contains main line data</param>
        /// <param name="lnColor">Main line color</param>
        public void AddMainLine(DataRow dr, Color lnColor)
        {
            try
            {
                MainLine = new RiskPolyLine(GridPaintLines, MenuMainRisk, false)
                {
                    Stroke = new SolidColorBrush(lnColor),
                    IsDiagonal = false,
                    IsRoot = true,
                    IsCM = false,
                    FromTop = false,
                    StrokeThickness = General.MaxThickness,
                    ID = (decimal)dr[DT_Risk.ID],
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
                MainLine.MouseEnter += MainLine_MouseEnter;
                MainLine.MouseLeave += MainLine_MouseLeave;
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

        private void MainLine_MouseLeave(object sender, MouseEventArgs e)
        {
            try
            {
                SetLinesThickness();
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        private void MainLine_MouseEnter(object sender, MouseEventArgs e)
        {
            try
            {
                RiskPolyLine TheLine;
                if (sender is SegmentPolyLine)
                {
                    TheLine = ((SegmentPolyLine)sender).Father;
                }
                else
                {
                    TheLine = (RiskPolyLine)sender;
                }
                SetLineThickness(TheLine);
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        /// <summary>
        /// Delete all the rectangle of the FishHead in the Diagram
        /// </summary>
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
                    foreach (SegmentPolyLine sg in item.Segments)
                    {
                        GridPaintLines.Children.Remove(sg);//removing the line
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

        /// <summary>
        /// Create risk and counter measure shapes and arrange all of their. Show fishbone diagram.
        /// </summary>
        public void LoadLines()
        {
            try
            {
                LinesListCMState.Clear();

                LinesList.Clear();
                System.Windows.Media.Color color = ((SolidColorBrush)new BrushConverter().ConvertFrom(Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Rows.Find(new object[] { ID_Diagram, IdDamageSelected })[DT_Diagram_Damages.COLOR].ToString())).Color;
                AddMainLine(Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + ID_Diagram + " and " + DT_Risk.IS_ROOT + " = " + 1).First(), color);

                CreateRiskShapes(color);
                CreateCounterMeasureShapes();

                TreeOperation.Build_Tree(LinesList);
                TreeOperation.DrawEntireDiagramAsFishBone(MainLine);
                FixDrawPanel();
                UpdateLinesValues();
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

        /// <summary>
        /// Create and set up counter measure shapes
        /// </summary>
        private void CreateCounterMeasureShapes()
        {
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
                RiskPolyLine cmline = haspermission ? CreateCounterMeasureShape(((MainWindow)MyWindow).TheCurrentLayout.GridPaintLines, MenuCM, true, item) : CreateCounterMeasureShape(((MainWindow)MyWindow).TheCurrentLayout.GridPaintLines, null, true, item);
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
                LinesListCMState.Add(Convert.ToDecimal(cmline.ID), cmline.IsActivated);
            }
        }

        /// <summary>
        /// Create and set up risk shapes
        /// </summary>
        /// <param name="color">Risk line color</param>
        private void CreateRiskShapes(Color color)
        {
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
                    RiskPolyLine riskLine = haspermission ? CreateRiskShape(((MainWindow)MyWindow).TheCurrentLayout.GridPaintLines, MenuRisk, false, item) : CreateRiskShape(((MainWindow)MyWindow).TheCurrentLayout.GridPaintLines, null, false, item);
                    SetPolyLineGroup(riskLine, item);

                    riskLine.Collapsed = (bool)item[DT_Risk.ISCOLLAPSED];
                    if (((bool)item[DT_Risk.ENABLED]))
                    {
                        ((MenuItem)MenuRisk.Items[(int)MenuRiskItems.Enable]).ToolTip = StringResources.DisableValue;

                        riskLine.ShortName = item[DT_Risk.NAMESHORT].ToString();
                        if (haspermission)
                        {
                            riskLine.SetColor(new SolidColorBrush(color));
                        }
                        else
                        {
                            riskLine.SetColor(new SolidColorBrush(Color.FromArgb(80, color.R, color.G, color.B)));
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

                    if (Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { riskLine.ID, IdDamageSelected }))
                    {
                        riskLine.OwnValue = (decimal)Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Find(new object[] { riskLine.ID, IdDamageSelected })[DT_Risk_Damages.VALUE];
                    }
                    LinesList.Add(riskLine);
                }
            }
        }

        private decimal CalculateOwnValueRisk(decimal idRisk, decimal idDamage)
        {
            if (Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { idRisk, idDamage }))
            {
                return (decimal)Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Find(new object[] { idRisk, idDamage })[DT_Risk_Damages.VALUE];
            }
            else
            {
                return 0;
            }
        }

        private decimal CalculateAcumDamageRisk(RiskPolyLine line, decimal IdDamageSelected)
        {
            decimal AcumDamage = CalculateOwnValueRisk(line.ID, IdDamageSelected) * FishHeadController.AcumulatedLikelihood(line);
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
                                if (Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Contains(new object[] { hijo.ID, IdDamageSelected }))
                                {
                                    value = (decimal)Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Find(new object[] { hijo.ID, IdDamageSelected })[DT_CounterM_Damage.VALUE];
                                }
                                AcumDamage += value;
                                value = 0;
                            }
                            else
                            {
                                if (Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { hijo.ID, IdDamageSelected }))
                                {
                                    value = (decimal)Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Find(new object[] { hijo.ID, IdDamageSelected })[DT_Risk_Damages.VALUE];
                                }
                                AcumDamage += value * FishHeadController.AcumulatedLikelihood(hijo);
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

        /// <summary>
        /// Update risw and counter measure properties values like acumulated likelihood, damage, acumulated damage. 
        /// </summary>
        public void UpdateLinesValues()
        {
            foreach (var item in LinesList)
            {
                if (!item.IsCM)
                {
                    decimal al = FishHeadController.AcumulatedLikelihood(item);
                    decimal valor = FishHeadController.CalcDiagramDamageValue(Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID),
                        Ds.Tables[DT_Risk.TABLE_NAME], IdDamageSelected, Ds.Tables[DT_Risk_Damages.TABLE_NAME],
                            Ds.Tables[DT_CounterM.TABLE_NAME], Ds.Tables[DT_CounterM_Damage.TABLE_NAME]);
                    decimal myvalue = 0;
                    if (Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { item.ID, IdDamageSelected }))
                    {
                        myvalue = (decimal)Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Find(new object[] { item.ID, IdDamageSelected })[DT_Risk_Damages.VALUE];
                    }
                    decimal AcumDamage = 0;
                    foreach (var itemI in TreeOperation.GetMeAndMyChildrenWithCM(item))
                    {
                        if (itemI.IsActivated)
                        {
                            decimal value = 0;
                            if (itemI.IsCM)
                            {
                                if (itemI.IsActivated)
                                {
                                    if (Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Contains(new object[] { itemI.ID, IdDamageSelected }))
                                    {
                                        value = (decimal)Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Find(new object[] { itemI.ID, IdDamageSelected })[DT_CounterM_Damage.VALUE];
                                    }
                                    AcumDamage += value;
                                }
                            }
                            else
                            {
                                if (Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Contains(new object[] { itemI.ID, IdDamageSelected }))
                                {
                                    value = (decimal)Ds.Tables[DT_Risk_Damages.TABLE_NAME].Rows.Find(new object[] { itemI.ID, IdDamageSelected })[DT_Risk_Damages.VALUE];
                                }
                                AcumDamage += value * FishHeadController.AcumulatedLikelihood(itemI);
                            }
                        }
                    }

                    item.AcLike = al;
                    item.AcValue = valor;
                    item.OwnValue = myvalue;
                    item.AcDamage = AcumDamage;
                }
            }
        }

        /// <summary>
        /// Set menu risk to all risk
        /// </summary>
        /// <param name="listaComplete"></param>
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
                        //item.SetMenu(MenuRisk);
                        item.FullAccess = false;
                    }
                }
            }
            ResetLinesMenu(lista, MenuRisk);
        }

        public bool IsPrimaryInLine(RiskPolyLine line)
        {
            bool access = false;
            if (line.IsCM)
            {
                if (Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + line.ID + " AND " + DT_CM_WBS.IS_PRIMARY + " = 1").Any())
                {
                    if (Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + line.ID + " AND " + DT_CM_WBS.IS_PRIMARY + " = 1").First()[DT_CM_WBS.USERNAME].ToString() == LoginUser)
                    {
                        access = true;
                    }
                }
            }
            else
            {
                if (Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + line.ID + " AND " + DT_RISK_WBS.IS_PRIMARY + " = 1").Any())
                {
                    if (Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + line.ID + " AND " + DT_RISK_WBS.IS_PRIMARY + " = 1").First()[DT_RISK_WBS.USERNAME].ToString() == LoginUser)
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
                foreach (DataRow drCMWBS in Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + line.ID))
                {
                    if (dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + drCMWBS[DT_CM_WBS.ID_WBS]).Any())
                    {
                        if (dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + drCMWBS[DT_CM_WBS.ID_WBS]).Any())
                        {
                            decimal idPadre = (decimal)dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + drCMWBS[DT_CM_WBS.ID_WBS]).First()[DT_WBS_STRUCTURE.ID_FATHER];
                            if (Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + line.ID).CopyToDataTable().Select(DT_CM_WBS.ID_WBS + " = " + idPadre).Any())
                            {
                                DataRow drreturn = Ds.Tables[DT_CM_WBS.TABLE_NAME].Select(DT_CM_WBS.ID_CM + " = " + line.ID).CopyToDataTable().Select(DT_CM_WBS.ID_WBS + " = " + idPadre).First();
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
                foreach (DataRow drRISKWBS in Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + line.ID))
                {
                    if (dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + drRISKWBS[DT_RISK_WBS.ID_WBS]).Any())
                    {
                        decimal idPadre = (decimal)dsWBS.Tables[DT_WBS_STRUCTURE.TABLE_NAME].Select(DT_WBS_STRUCTURE.ID_CHILD + " = " + drRISKWBS[DT_RISK_WBS.ID_WBS]).First()[DT_WBS_STRUCTURE.ID_FATHER];
                        if (Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + line.ID).CopyToDataTable().Select(DT_RISK_WBS.ID_WBS + " = " + idPadre).Any())
                        {
                            DataRow drreturn = Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + line.ID).CopyToDataTable().Select(DT_RISK_WBS.ID_WBS + " = " + idPadre).First();
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
            bool access = IsPrimaryInLine(line) || line.IsRoot;
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
                        if (Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + item.ID + " AND " + DT_RISK_WBS.IS_PRIMARY + " = 1").Any())
                        {
                            if (Ds.Tables[DT_RISK_WBS.TABLE_NAME].Select(DT_RISK_WBS.ID_RISK + " = " + item.ID + " AND " + DT_RISK_WBS.IS_PRIMARY + " = 1").First()[DT_RISK_WBS.USERNAME].ToString() == LoginUser)
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

        /// <summary>
        /// Set events to segments
        /// </summary>
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
                        segmentLine.MouseEnter += MainLine_MouseEnter;
                        segmentLine.MouseLeave += MainLine_MouseLeave;
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
                    IdGroup = (decimal)item[DT_Risk.ID_GROUPE],
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
                IdRiskFather = (decimal)itemDataRow[DT_CounterM.ID_RISK],
                ID = (decimal)itemDataRow[DT_CounterM.ID],
                Probability = (decimal)itemDataRow[DT_CounterM.PROBABILITY] / 100,
                IsActivated = (bool)itemDataRow[DT_CounterM.ENABLED]
            };
        }

        private RiskPolyLine CreateRiskShape(Grid gridPaint, ContextMenu contextMenu, bool isCMI, DataRow itemDataRow)
        {
            return new RiskPolyLine(gridPaint, contextMenu, isCMI)
            {
                ShortName = itemDataRow[DT_Risk.NAMESHORT].ToString(),
                ID = (decimal)itemDataRow[DT_Risk.ID],
                Position = (int)itemDataRow[DT_Risk.POSITION],
                Collapsed = (bool)itemDataRow[DT_Risk.ISCOLLAPSED],
                Probability = (decimal)itemDataRow[DT_Risk.PROBABILITY] / 100,
                IsActivated = (bool)itemDataRow[DT_Risk.ENABLED],
                StrokeThickness = 2,
                IsCM = isCMI,
                IdRiskFather = (decimal)itemDataRow[DT_Risk.IDRISK_FATHER],
                FullAccess = true
            };
        }

        //public void DrawEntireDiagram()
        //{
        //    try
        //    {
        //        if (Ds.HasChanges())
        //        {
        //            if (ID_Diagram != 0)
        //            {
        //                CleanFishBone();
        //                LoadFishBone();
        //                DrawNumbersAndLineThickness();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MostrarDialog(ex.Message);
        //    }
        //}

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
                            //System.Drawing.Color drawingCColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram)[CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                            System.Windows.Media.Color color = ((SolidColorBrush)new BrushConverter().ConvertFrom(Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram)[CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString())).Color;
                            if (TengoPermiso(item))
                            {
                                item.SetColor(new SolidColorBrush(color));
                            }
                            else
                            {
                                item.SetColor(new SolidColorBrush(Color.FromArgb(80, color.R, color.G, color.B)));
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
                CMEnter(CMHoover, new Point(e.GetPosition(TheMainGrid).X, e.GetPosition(TheMainGrid).Y), true);
            }
            if (sender is LabelPolyLine CMLabelHoover)
            {
                CMEnter(CMLabelHoover.Line, new Point(e.GetPosition(TheMainGrid).X, e.GetPosition(TheMainGrid).Y), true);
            }
        }

        public void CMEnter(RiskPolyLine CMLine, Point pointToShowPopup, bool showPopup)
        {
            try
            {
                CMLine.StrokeThickness = 3;
                CMLine.StrokeThickness = 3;
                decimal valor;
                if (Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Contains(new object[] { CMLine.ID, IdDamageSelected }))
                {
                    valor = (decimal)Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Rows.Find(new object[] { CMLine.ID, IdDamageSelected })[DT_CounterM_Damage.VALUE];
                }
                else
                {
                    valor = 0;
                }
                string Value = General.MyRound(valor, 4).ToString();

                string probability = General.MyRound(CMLine.Probability * 100, 2).ToString() + " %";

                //Popin = new Popin(GridPaintLines, pointToShowPopup, "CM: " + CMLine.ShortName, probability, Value)
                //{
                //    Visibility = Visibility.Visible
                //};
                if (!showPopup)
                {
                    MostrarPopCMWindow(new Point(pointToShowPopup.X - ScrollGridPaint.ContentHorizontalOffset, pointToShowPopup.Y - ScrollGridPaint.ContentVerticalOffset), CMLine.ShortName, probability, Value);
                }
                else
                {
                    MostrarPopCMWindow(pointToShowPopup, CMLine.ShortName, probability, Value);
                }
                if ((bool)Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(CMLine.ID)[DT_CounterM.ENABLED])
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
            OcultarPopCMWindow();
            //if (Popin != null)
            //{
            //    Popin.Visibility = Visibility.Collapsed;
            //    GridPaintLines.Children.Remove(Popin);
            //    Popin = null;
            //}
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
                        TreeOperation.LinePositionByClick(Line_Selected, e.GetPosition(GridPaintLines), Ds);
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
        private void SelectOneRisk(object sender, MouseButtonEventArgs e, decimal ID_Sender)
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
                    ID_Project = (decimal)Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.ID_PROJECT],
                    CMRow = Ds.Tables[DT_CounterM.TABLE_NAME].NewRow(),
                    Ds = Ds,
                    LOGIN_USER = LoginUser,
                    Operation = General.INSERT,
                    RowFather = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID),
                    RiskTreeID = ID_Diagram,
                    RiskPadre = Line_Selected,
                    MyCM = Ds.Tables[DT_CounterM.TABLE_NAME].Copy()
                };
                windowCM.HasAccess = true;
                windowCM.Probability = 0;
                if (windowCM.ShowDialog() == true)
                {
                    Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Add(windowCM.CMRow);
                    Line_Created.ID = (decimal)windowCM.CMRow[DT_CounterM.ID];

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
                    ID_PROJECT = (decimal)Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.ID_PROJECT],
                    Ds = Ds,
                    Operation = General.INSERT,
                    RowFather = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID),
                    RiskTreeID = (decimal)Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.ID_DIAGRAM],
                    RiskSelected = Line_Selected,
                    MyRisks = Ds.Tables[DT_Risk.TABLE_NAME],
                    LOGIN_USER = LoginUser,
                };
                wrisk.HasAccess = true;
                if (wrisk.ShowDialog() == true)
                {
                    Line_Created.ID = (decimal)wrisk.RiskRow[DT_Risk.ID];
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
                decimal ID_Sender;
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
                decimal ID_Sender;
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

        private void MouseDownPress(object sender, MouseButtonEventArgs e, decimal ID_Sender, RiskPolyLine TheLine)
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
                                Color drawColor = ((SolidColorBrush)new BrushConverter().ConvertFrom(Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_DAMAGE + " = " + IdDamageSelected).First()[DT_Diagram_Damages.COLOR].ToString())).Color;
                                //System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_DAMAGE + " = " + IdDamageSelected).First()[DT_Risk_Damages.COLOR].ToString()));
                                if (TengoPermiso(Line_Selected) && FullAccess(Line_Selected))
                                {
                                    Line_Selected.SetMenu(MenuRisk);
                                    Line_Selected.FullAccess = true;
                                    Line_Selected.SetColor(new SolidColorBrush(drawColor));
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
            ((MainWindow)MyWindow).LSelected = Line_Selected.ShortName;
            ((MainWindow)MyWindow).TSelected = Line_Selected.IsCM ? "Counter Measure" : "Risk";
            ((MainWindow)MyWindow).ShowCMData = Line_Selected != null && Line_Selected.IsCM;
            ((MainWindow)MyWindow).ShowRiskData = Line_Selected != null && !Line_Selected.IsCM;
            if (Line_Selected != null)
            {
                if (!Line_Selected.IsCM)
                {
                    ((MainWindow)MyWindow).UpdateRiskTabInformation();
                }
                else
                {
                    ((MainWindow)MyWindow).UpdateCMTabInformation();
                }
            }
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
            if (MoviendoRisk)
            {
                LineInMoving.IsDiagonal = false;
                LineInMoving.NewDrawAtPoint(LineInMoving.StartDrawPoint);
            }
            OcultarPopWindow();
            //if (Popin != null)
            //{
            //    GridPaintLines.Children.Remove(Popin);
            //    Popin.Visibility = Visibility.Collapsed;
            //    Popin = null;
            //}
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
            RiskEnter(TheLine, new Point(e.GetPosition(TheMainGrid).X, e.GetPosition(TheMainGrid).Y), true);
        }

        private void Segment_MouseHover(object sender, MouseEventArgs e)
        {
            SegmentPolyLine TheLine;
            TheLine = (SegmentPolyLine)sender;
            SetLineThickness(TheLine.Father);
            RiskEnter(TheLine.Father, new Point(e.GetPosition(TheMainGrid).X, e.GetPosition(TheMainGrid).Y), true);
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

        public void RiskEnter(RiskPolyLine TheLine, Point pointToShowPopup, bool showPopup)
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
                        string AcumValue = General.MyRound(TheLine.AcValue, 4).ToString();
                        string Valuee = General.MyRound(TheLine.OwnValue, 2).ToString();
                        string ED = General.MyRound(TheLine.AcDamage, 4).ToString();
                        string probability = General.MyRound(TheLine.Probability * 100, 2).ToString() + " %";
                        string EL = General.MyRound(TheLine.AcLike * 100, 2).ToString() + " %";
                        if (!showPopup)
                        {
                            Point point = new Point(pointToShowPopup.X - ScrollGridPaint.ContentHorizontalOffset, pointToShowPopup.Y - ScrollGridPaint.ContentVerticalOffset);
                            MostrarPopWindow(point, TheLine.ShortName, probability, Valuee, ED, AcumValue, EL);
                        }
                        else
                        {
                            MostrarPopWindow(pointToShowPopup, TheLine.ShortName, probability, Valuee, ED, AcumValue, EL);
                        }
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
                else
                {
                    if (MoviendoRisk)
                    {
                        LineInMoving.IsDiagonal = !TheLine.IsDiagonal;
                        LineInMoving.FromTop = TheLine.FromTop;
                        LineInMoving.DrawSingleLine();
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

        /// <summary>
        /// Insert Risk in the Diagram, Calculating according the siblings and the user click position
        /// </summary>
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

        private void MoveRisk(RiskPolyLine destinationPolyLine, Point point)
        {
            if (FullAccess(destinationPolyLine))
            {

                //Eliminar el Risk que se mueve de su padre
                Line_Selected.Father.Children.Remove(Line_Selected);

                //Reestablecer la posición de los PolyLine en su padre
                SetPolyLinePosition(Line_Selected.Father.Children);
                if (new WindowMessageYesNo("Do you want to Move the selected items with all their properties(WBS, damages, probabilities etc.)?").ShowDialog() == true)
                {
                    Line_Selected.Father = destinationPolyLine;
                    //Line_Selected.Position = destinationPolyLine.Children.Count - 1;
                    DataSet ImportDSs = Ds.Copy();
                    DataRow drNewRisk = CopyPasteController.CopyValuesFromLineSource(Line_Selected, ImportDSs, Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(destinationPolyLine.ID), true,
                                                                                        ID_Diagram, ((MainWindow)MyWindow).DsWBS, LinesList);
                    //CopyPasteOps.EstablecerValorDelHijoAlPadre(Line_Selected, ImportDSs, Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(destinationPolyLine.ID), ID_Diagram, ((MainWindow)MyWindow).DsWBS);
                    CopyPasteController.SetValuesFromChildToFather(drNewRisk, ImportDSs, Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(destinationPolyLine.ID), ((MainWindow)MyWindow).DsWBS);
                    Ds = ImportDSs;
                    ImportDSs.Dispose();
                    MoviendoRisk = false;
                    RiskPolyLine Line_Created = new RiskPolyLine
                    {
                        ID = (decimal)drNewRisk[DT_Risk.ID],
                        IsCM = false,
                        ShortName = "LineCreated",
                        Father = destinationPolyLine,
                        IdRiskFather = destinationPolyLine.ID
                    };
                    InsertRisk(Line_Created, destinationPolyLine, point);

                    RiskPolyLine linetoDel = new RiskPolyLine
                    {
                        ID = Line_Selected.ID,
                        IsCM = Line_Selected.IsCM
                    };
                    TreeOperation.DeleteLine(linetoDel, Ds);
                    GridPaintLines.Children.Remove(LineInMoving);
                    GridPaintLines.Children.Remove(LineInMoving.TextPanel);
                    LineInMoving = null;
                }
                else
                {

                    Line_Selected.Father = destinationPolyLine;
                    DataSet ImportDSs = Ds.Copy();
                    DataRow drNewRisk = CopyPasteController.CopyRiskWithoutSourceData(Line_Selected, ImportDSs, Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(destinationPolyLine.ID), true,
                                            ID_Diagram, ((MainWindow)MyWindow).DsWBS);
                    Ds = ImportDSs;
                    ImportDSs.Dispose();
                    //GlobalListCopy = new List<RiskPolyLine>();
                    MoviendoRisk = false;
                    RiskPolyLine Line_Created = new RiskPolyLine
                    {
                        ID = (decimal)drNewRisk[DT_Risk.ID],
                        IsCM = false,
                        ShortName = "LineCreated",
                        Father = destinationPolyLine,
                        IdRiskFather = destinationPolyLine.ID
                    };
                    InsertRisk(Line_Created, destinationPolyLine, point);
                    RiskPolyLine linetoDel = new RiskPolyLine
                    {
                        ID = Line_Selected.ID,
                        IsCM = Line_Selected.IsCM
                    };
                    TreeOperation.DeleteLine(linetoDel, Ds);
                    GridPaintLines.Children.Remove(LineInMoving);
                    GridPaintLines.Children.Remove(LineInMoving.TextPanel);
                    LineInMoving = null;
                }
                Line_Selected = destinationPolyLine;
            }
        }

        private void MoveCounterMeasure(RiskPolyLine destinationPolyLine, Point point)
        {
            if (FullAccess(destinationPolyLine))
            {
                //Eliminar la CM que se mueve de su padre
                Line_Selected.Father.Children.Remove(Line_Selected);

                //Reestablecer la posición de los PolyLine en su padre
                SetPolyLinePosition(Line_Selected.Father.Children);
                if (new WindowMessageYesNo("Do you want to Move the selected item with all their properties(WBS, damages, probabilities etc.)?").ShowDialog() == true)
                {
                    Line_Selected.Father = destinationPolyLine;
                    //Line_Selected.Position = destinationPolyLine.Children.Count - 1;
                    DataSet ImportDSs = Ds.Copy();
                    DataRow drNewCM = CopyPasteController.SetValuesOriginalAndNewCopiedCM(Line_Selected, ImportDSs, Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(destinationPolyLine.ID), ID_Diagram, ((MainWindow)MyWindow).DsWBS);

                    Ds.Merge(ImportDSs);
                    ImportDSs.Dispose();
                    MoviendoRisk = false;
                    RiskPolyLine Line_Created = new RiskPolyLine
                    {
                        ID = (decimal)drNewCM[DT_CounterM.ID],
                        IsCM = true,
                        ShortName = "LineCreated",
                        Father = destinationPolyLine,
                        IdRiskFather = destinationPolyLine.ID
                    };
                    InsertCM(Line_Created, destinationPolyLine, point);

                    RiskPolyLine linetoDel = new RiskPolyLine
                    {
                        ID = Line_Selected.ID,
                        IsCM = Line_Selected.IsCM
                    };
                    TreeOperation.DeleteLine(linetoDel, Ds);

                }
                else
                {

                    Line_Selected.Father = destinationPolyLine;
                    DataSet ImportDSs = Ds.Copy();
                    DataRow drNewCM = CopyPasteController.SetNewCMValues(Line_Selected, ImportDSs, Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(destinationPolyLine.ID), ID_Diagram, ((MainWindow)MyWindow).DsWBS);
                    Ds = ImportDSs;
                    ImportDSs.Dispose();
                    //GlobalListCopy = new List<RiskPolyLine>();
                    MoviendoRisk = false;
                    RiskPolyLine Line_Created = new RiskPolyLine
                    {
                        ID = (decimal)drNewCM[DT_CounterM.ID],
                        IsCM = true,
                        ShortName = "LineCreated",
                        Father = destinationPolyLine,
                        IdRiskFather = destinationPolyLine.ID
                    };
                    InsertCM(Line_Created, destinationPolyLine, point);
                    RiskPolyLine linetoDel = new RiskPolyLine
                    {
                        ID = Line_Selected.ID,
                        IsCM = Line_Selected.IsCM
                    };
                    TreeOperation.DeleteLine(linetoDel, Ds);

                }
                GridPaintLines.Children.Remove(LineInMoving);
                if (LineInMoving != null && LineInMoving.TextPanel != null)
                {
                    GridPaintLines.Children.Remove(LineInMoving.TextPanel);
                }
                LineInMoving = null;
                Line_Selected = destinationPolyLine;
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
            //((MainWindow)MyWindow).TextProbabilityChange(MainLine);
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
            ((MainWindow)MyWindow).CrossRiskRightTab(Ds);
            ((MainWindow)MyWindow).CroosCMRightTab(Ds);
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
                    decimal ID_Sender;
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
                                ID_PROJECT = (decimal)Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.ID_PROJECT],
                                Operation = General.INSERT,
                                RowFather = Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID),
                                RiskTreeID = ID_Diagram,
                                RiskSelected = Line_Selected,
                                MyRisks = Ds.Tables[DT_Risk.TABLE_NAME].Copy()
                            };
                            wrisk.HasAccess = true;
                            if (wrisk.ShowDialog() == true)
                            {
                                RiskPolyLine Line_Created = new RiskPolyLine
                                {
                                    ID = (decimal)wrisk.RiskRow[DT_Risk.ID],
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
                                ((MainWindow)MyWindow).CrossRiskRightTab(Ds);
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
                            if (Line_Selected.IdRiskFather != TheLine.ID && Line_Selected.ID != TheLine.ID)
                            {
                                //Mover el riesgo de un Padre a otro (cambiar de padre)
                                MoviendoRisk = false;
                                MoveRisk(TheLine, e.GetPosition(GridPaintLines));
                            }
                            else
                            {
                                if (Line_Selected.IdRiskFather == ID_Sender)
                                {
                                    //Mover el riesgo en el mismo Padre (Cambiar la posición, reordenar)
                                    MoviendoRisk = false;
                                    ReorderRisk(TheLine, e.GetPosition(GridPaintLines));
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
                            DrawFishBone();
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
                decimal ID_Sender;
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
                    if (Line_Selected.IdRiskFather != TheLine.ID && Line_Selected.ID != TheLine.ID)
                    {
                        //Mover el riesgo de un Padre a otro (cambiar de padre)
                        MoviendoRisk = false;
                        MoveRisk(TheLine, e.GetPosition(GridPaintLines));
                    }
                    else
                    {
                        if (Line_Selected.IdRiskFather == ID_Sender)
                        {
                            //Mover el riesgo en el mismo Padre (Cambiar la posición, reordenar)
                            MoviendoRisk = false;
                            ReorderRisk(TheLine, e.GetPosition(GridPaintLines));
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
                    DrawFishBone();
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
                MiniMapGHT.Height = ScrollGridPaint.ViewportHeight * 40 / 100;
                MiniMapGHT.Width = ScrollGridPaint.ViewportWidth * 40 / 100;
                MiniMapGHT.MapSource = new ScrollViewer();
                MiniMapGHT.MapSource = this.ScrollGridPaint;
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
                MiniMapGHT.Height = ScrollGridPaint.ViewportHeight * 30 / 100;
                MiniMapGHT.Width = ScrollGridPaint.ViewportWidth * 30 / 100;
            }
        }

        private void BtnMinusZoom_Click(object sender, RoutedEventArgs e)
        {
            if (SliderZoom.Value > 10)
            {
                SliderZoom.Value -= 10;
                MiniMapGHT.Height = ScrollGridPaint.ViewportHeight * 30 / 100;
                MiniMapGHT.Width = ScrollGridPaint.ViewportWidth * 30 / 100;
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
        private void CbFilterTopR_DropDownClosed(object sender, EventArgs e)
        {
            try
            {
                if (LinesList.Count > 0)
                {
                    if (!(CbFilterTopR.SelectedValue is null))
                    {
                        //System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Risk_Damages.TABLENAME].Select(DT_Risk_Damages.ID_DAMAGE + " = " + IdDamageSelected).First()[DT_Risk_Damages.COLOR].ToString()));
                        Color mediaColor = ((SolidColorBrush)new BrushConverter().ConvertFrom(Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_DAMAGE + " = " + IdDamageSelected).First()[DT_Diagram_Damages.COLOR].ToString())).Color;
                        int index = Rectangles.FindIndex(x => x.ID_TopRisk.Equals(IdDamageSelected));

                        foreach (RiskPolyLine item in LinesList)
                        {
                            if (!(item.IsCM))
                            {
                                if (item.IsActivated)
                                {
                                    if (item.IsRoot)
                                    {
                                        item.SetColor(new SolidColorBrush(mediaColor));
                                    }
                                    else
                                    {
                                        if (TengoPermiso(item))
                                        {
                                            item.SetColor(new SolidColorBrush(mediaColor));
                                        }
                                        else
                                        {
                                            item.SetColor(new SolidColorBrush(Color.FromArgb(80, mediaColor.R, mediaColor.G, mediaColor.B)));
                                        }
                                    }
                                    item.UpdateSegmentsStroke();
                                }
                            }
                        }
                        UpdateLinesValues();
                        SetLinesThickness();
                        MoveElementsInArray(Rectangles, Rectangles.Count, index);
                        ReLoadRectangles();
                        DrawNumbers();
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
                        if (!string.IsNullOrEmpty(TextChangeName.Text))
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
                            if (!string.IsNullOrEmpty(TextChangeName.Text))
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

        private void ManageTextChangeProperties(string valueTextProperty, Visibility aVisibility)
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
                    foreach (DataRow item in Ds.Tables[DT_CounterM_Damage.TABLE_NAME].Select(DT_CounterM_Damage.ID_COUNTERM + " = " + Line_Selected.ID))
                    {
                        item[DT_CounterM_Damage.COUNTERM_NAMESHORT] = polyLineName;
                    }
                }
                else
                {
                    Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.NAMESHORT] = polyLineName;
                    foreach (DataRow item in Ds.Tables[DT_Risk_Damages.TABLE_NAME].Select(DT_Risk_Damages.ID_RISK + " = " + Line_Selected.ID))
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
                    decimal RiskTreeID = (decimal)Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.ID_DIAGRAM];

                    if (Ds.Tables[DT_Risk.TABLE_NAME].Select(DT_Risk.ID_DIAGRAM + " = " + RiskTreeID + " and "
                        + DT_Risk.NAMESHORT + " = '" + proposedPolyLineName + "' and " + DT_Risk.ID + " <> " + Line_Selected.ID).Any())
                    {
                        yesNo = new WindowMessageYesNo("The name [" + proposedPolyLineName + "] Already exists in this diagram. Do you want to use it again?");
                        yesNo.ShowDialog();
                    }
                }
                else
                {
                    decimal RiskTreeID = (decimal)Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.IdRiskFather)[DT_Risk.ID_DIAGRAM];

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

                //if (((MainWindow)MyWindow).TheCurrentLayout.ID_Diagram != ID_Diagram)
                //{
                //    ((MainWindow)MyWindow).TheCurrentLayout = this;
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
                                    if (MoviendoRisk)
                                    {
                                        LineInMoving.NewDrawAtPoint(e.GetPosition(GridPaintLines));
                                        TreeOperation.MoveLines(new List<RiskPolyLine>() { LineInMoving }, e.GetPosition(GridPaintLines).X - LineInMoving.Points[1].X - 25, e.GetPosition(GridPaintLines).Y - LineInMoving.Points[1].Y);
                                        X = e.GetPosition(GridPaintLines).X;
                                        Y = e.GetPosition(GridPaintLines).Y;
                                        Main_Y = MainLine.Points[0].Y;
                                    }
                                    else
                                    {
                                        MoviendoRisk = true;
                                        LinesMoving = new List<RiskPolyLine>();
                                        LinesMoving.AddRange(TreeOperation.GetMeAndMyChildrenWithCM(Line_Selected));
                                        foreach (var item in LinesMoving)
                                        {
                                            item.Hidden = true;
                                        }
                                        GridPaintLines.Children.Remove(LineInMoving);
                                        //System.Drawing.Color lnColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram)[CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString()));
                                        System.Windows.Media.Color color = ((SolidColorBrush)new BrushConverter().ConvertFrom(Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram)[CbFilterTopR.SelectedIndex][DT_Diagram_Damages.COLOR].ToString())).Color;

                                        LineInMoving = new RiskPolyLine(GridPaintLines, MenuRisk, false)
                                        {
                                            Stroke = new SolidColorBrush(color),
                                            StrokeThickness = 3,
                                            IsMoving = true
                                        };
                                        LineInMoving.NewDrawAtPoint(new Point(X, Y), "");
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
                if (((MainWindow)MyWindow).TheCurrentLayout.ID_Diagram != ID_Diagram)
                {
                    ((MainWindow)MyWindow).TheCurrentLayout = this;
                    ((MainWindow)MyWindow).UpdateMiniMapSource();
                    foreach (var item in ((MainWindow)MyWindow).OpenedDocuments)
                    {
                        item.ExitWorking();
                    }
                    this.EnterWorking();
                    //CleanFishBone();
                    //LoadFishBone();
                    //DrawNumbersAndLineThickness();
                    //((MainWindow)MyWindow).TextProbabilityChange(MainLine);
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

        /// <summary>
        /// Export to excel diagram data
        /// </summary>
        /// <param name="fileName">File name</param>
        public void ExportToExcel(string fileName)
        {
            this.TheProgressBar.IsIndeterminate = false;
            this.TheProgressBar.Minimum = 0;
            this.TheProgressBar.Maximum = 100;
            //this.TheProgressBar.Visibility = Visibility.Visible;
            //RiskAndCm[] acumulatedValueList = LinesList.Select((risk, RiskAndCm) => new RiskAndCm { isCM = risk.IsCM, id = risk.ID, value = risk.AcValue }).ToArray();
            WindowSelection frmSelection = new WindowSelection
            {
                Dt = Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_RISKTREE + " = " + ID_Diagram).CopyToDataTable(),
                DcolumToShow = new string[] { DT_Diagram_Damages.DAMAGE },
                DcolumToShowAlias = new string[] { DT_Diagram_Damages.DAMAGE },
                Title = "List of Damages",
                ColumnToFilter = DT_Diagram_Damages.DAMAGE,
                FilterString = "Damage"
            };
            if (frmSelection.ShowDialog() == true)
            {
                using (RiskTreeDataSetTrader riskTreeDataSetTrader = new RiskTreeDataSetTrader(this.Ds, this.ID_Diagram, LinesList, frmSelection.RowsSelected.ToArray()))
                {
                    using (ExportRiskTree exportRiskTree = new ExportRiskTree(riskTreeDataSetTrader, fileName))
                    {
                        exportToExcelWorker.RunWorkerAsync(exportRiskTree);
                        this.IsExportingToExcel = true;
                    }
                }
            }
        }
        void ExportToExcelWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ExportRiskTree exportRiskTree = (ExportRiskTree)e.Argument;

            //exportRiskTree.Export(sender as BackgroundWorker, e);

            exportRiskTree.ExportShortExcel(sender as BackgroundWorker, e);
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
                if (((MainWindow)MyWindow).TheCurrentLayout != null)
                {
                    if (((MainWindow)MyWindow).TheCurrentLayout.ID_Diagram != ID_Diagram)
                    {
                        ((MainWindow)MyWindow).TheCurrentLayout = this;
                        ((MainWindow)MyWindow).UpdateMiniMapSource();
                        foreach (var item in ((MainWindow)MyWindow).OpenedDocuments)
                        {
                            item.ExitWorking();
                        }
                        this.EnterWorking(); 
                        ((MainWindow)MyWindow).ShowRiskData = false;
                        ((MainWindow)MyWindow).ShowCMData = false;
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
                Title = Ds.Tables[DT_Diagram.TABLE_NAME].Rows.Find(ID_Diagram)[DT_Diagram.DIAGRAM_NAME].ToString();
                SetLinesThickness();
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        private void MyDamage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (LinesList.Count > 0)
                {
                    IdDamageSelected = (decimal)((MyDamage)sender).ID_TopRisk;
                    int index = Rectangles.FindIndex(x => x.ID_TopRisk.Equals(IdDamageSelected));

                    //System.Drawing.Color drawColor = System.Drawing.Color.FromArgb(int.Parse(Ds.Tables[DT_Diagram_Damages.TABLENAME].Select(DT_Diagram_Damages.ID_DAMAGE + " = " + IdDamageSelected).First()[DT_Diagram_Damages.COLOR].ToString()));
                    System.Windows.Media.Color drawColor = ((SolidColorBrush)new BrushConverter().ConvertFrom(Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_DAMAGE + " = " + IdDamageSelected).First()[DT_Diagram_Damages.COLOR].ToString())).Color;

                    foreach (RiskPolyLine item in LinesList)
                    {
                        if (Ds.Tables[DT_Diagram_Damages.TABLE_NAME].Select(DT_Diagram_Damages.ID_DAMAGE + " = " + IdDamageSelected).Any())
                        {
                            if (!(item.IsCM))
                            {
                                if (item.IsActivated)
                                {
                                    if (item.IsRoot)
                                    {
                                        item.SetColor(new SolidColorBrush(drawColor));
                                    }
                                    else
                                    {
                                        if (TengoPermiso(item))
                                        {
                                            item.SetColor(new SolidColorBrush(drawColor));
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

                    //IdDamageSelected
                    //Select(DT_Risk_Damages.ID_DAMAGE + " = " + IdDamageSelected)
                    //Rectangle r = Rectangles.Select(DT_Risk_Damages.ID_DAMAGE + " = " + IdDamageSelected).First();

                    MoveElementsInArray(Rectangles, Rectangles.Count, index);
                    ReLoadRectangles();
                    DrawNumbers();
                }
            }
            catch (Exception ex)
            {
                MostrarDialog(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arr"> lista de damage</param>
        /// <param name="n"> longitud del array</param>
        /// <param name="k"> posicion hasta la cual se va a mover el array, es el damage seleccionado</param>
        public void MoveElementsInArray(List<MyDamage> lista, int n, int k)
        {
            MyDamage[] arr = lista.ToArray();
            //for (int i = 0; i < k; i++)
            for (int i = k; i < n - 1; i++)
            {
                // acomodo uno en cada iteracion
                MyDamage elem = arr[n - 1];
                for (int j = n - 1; j >= 1; j--)
                    arr[j] = arr[j - 1];
                arr[0] = elem;
            }
            Rectangles.Clear();
            for (int i = 0; i < arr.Length; i++)
            {
                Rectangles.Add(arr[i]);
            }
        }

        private void ScrollGridPaint_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateMiniMapSize();
        }

        private void ScrollGridPaint_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            UpdateMiniMapSize();
        }

        private void ScrollGridPaint_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateMiniMapSize();
        }

        private void TheZoomComboBox_DropDownClosed(object sender, EventArgs e)
        {
            UpdateMiniMapSize();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            MiniMapGHT.Visibility = Visibility.Visible;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            MiniMapGHT.Visibility = Visibility.Hidden;
        }

        //this method updates minimap size- most likely, minimap view will be scaled to fit
        //the window - so there will be some minimun extra space visible on the left and right sides or above and below actual
        //mini map view
        private void UpdateMiniMapSize()
        {
            bool resetToDefault = true;
            if (this.ScrollGridPaint != null)
            {
                //calculate current width to height ratio on the minimap
                double height = ScrollGridPaint.ViewportHeight * 40 / 100;
                double width = ScrollGridPaint.ViewportWidth * 40 / 100;

                if (ScrollGridPaint.ExtentWidth != 0 && ScrollGridPaint.ExtentHeight != 0)
                {
                    //get width to height ratio from map source - we want to display our minimap in the same ratio
                    double widthToHeightRatio = ScrollGridPaint.ExtentWidth / ScrollGridPaint.ExtentHeight;


                    //ideally - it should be 1 - whole view perfectly fits minimap 
                    double minimapWidthToHeightRatio = (height * widthToHeightRatio) / (width > 1.0 ? width : 1.0);

                    //if value is greater than one - we have to reduce height
                    if (minimapWidthToHeightRatio > 1.0)
                    {
                        double margin = (height - (height / minimapWidthToHeightRatio));

                        MiniMapGHT.Height = height - margin;
                        MiniMapGHT.Width = width;

                        resetToDefault = false;
                    }
                    //if value is less than one - we have to reduce width
                    else if (minimapWidthToHeightRatio < 1.0)
                    {
                        double margin = (width - (width * minimapWidthToHeightRatio));

                        MiniMapGHT.Height = height;
                        MiniMapGHT.Width = width - margin;

                        resetToDefault = false;
                    }
                }
                //perfect match or nothing to display - no need to setup margins
                if (resetToDefault)
                {
                    MiniMapGHT.Height = height;
                    MiniMapGHT.Width = width;
                }
            }
        }
    }
}
