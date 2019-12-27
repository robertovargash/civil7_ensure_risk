using DataMapping.Data;
using EnsureBusinesss;
using EnsureBusinesss.Business;
using EnsureRisk.Windows;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.AvalonDock.Layout;

namespace EnsureRisk.Classess
{
    /// <summary>
    /// Interaction logic for TheLayoutDocument.xaml
    /// </summary>
    public partial class TheLayoutDocument : LayoutDocument
    {
        public List<RiskPolyLine> LinesList { get; set; }
        public List<RiskPolyLine> LinesMoving { get; set; }
        public List<RiskPolyLine> ListCopy { get; set; }
        public List<RiskPolyLine> ListImport { get; set; }
        public List<RiskPolyLine> RiskGroupSelected { get; set; }
        public List<RiskPolyLine> CMGroupSelected { get; set; }
        public List<TopRisk> Rectangles { get; set; }

        public int ID_Diagram { get; set; }
        public Point MidPoint { get; set; }
        public RiskPolyLine MainLine { get; set; }
        public RiskPolyLine Line_Selected { get; set; }
        public ContextMenu MenuRisk { get; set; }
        public bool Loose { get; set; }
        public DataSet Ds { get; set; }

        private bool Flag_login, MoviendoCM, isRootSelected, ChoosingRisk, ChoosingCM, isPanEnabled, SelectingToGroup;

        private double X, Main_Y, Y;

        public int IdRiskSelected { get; set; }
        private LineGroup GroupSelected;
        private bool Moviendo = false;
        private bool Creando = false;

        private int Indexx = 0;
        private bool NameEditing = false;
        public bool Copiando { get; set; }
        public int IdPrincipalLine { get; set; }

        public TheLayoutDocument()
        {
            Loose = true;
            ScrollGridPaint.MyOwner = this;
            CanvasMain.MyOwner = this;
            GridPaintLines.MyOwner = this;
            textChangeName.MyOwner = this;
            cbFilterTopR.MyOwner = this;
            GridPaintLines.Width = 200;
            GridPaintLines.Height = 200;
            LinesList = new List<RiskPolyLine>();
            Rectangles = new List<TopRisk>();
            InitializeComponent();
        }

        #region Metodos

        public void AddMainLine(DataRow dr, System.Drawing.Color lnColor)
        {
            try
            {
                MainLine = new RiskPolyLine(GridPaintLines, MenuRisk, false)
                {
                    //aca lo del color
                    Stroke = new SolidColorBrush(Color.FromArgb(lnColor.A, lnColor.R, lnColor.G, lnColor.B)),
                    IsRoot = true,
                    IsCM = false,
                    FromTop = (Boolean)dr[DT_Risk.FROM_TOP_COLUMNA],
                    StrokeThickness = 6,
                    ID = (Int32)dr[DT_Risk.ID_COLUMNA],
                    Probability = (Decimal)dr[DT_Risk.PROBABILITY_COLUMN],
                    ShortName = dr[DT_Risk.NAMESHORT_COLUMNA].ToString(),
                    MyLevel = 0
                };
                MainLine.Group = new LineGroup()
                {
                    IdGroup = null,
                    GroupName = "None"
                };
                //adding the 3 points of the risk. The Main line the angle = 180º
                MainLine.Points[0] = (new Point(MidPoint.X - 500, MidPoint.Y));
                MainLine.Points[1] = (new Point(MainLine.Points[0].X + 200, MainLine.Points[0].Y));
                MainLine.MouseDown += PrincipalLine_MouseDown;
                MainLine.MouseUp += MainLine_MouseUp;
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

        public void LoadComboTopRisk()
        {
            try
            {
                DataRow[] thisTopRisk = Ds.Tables[DT_RiskTree_Damages.TABLENAME]
                        .Select(DT_RiskTree_Damages.ID_RISKTREE + " = " + ID_Diagram);

                cbFilterTopR.SelectedValuePath = DT_RiskTree_Damages.ID_DAMAGE;
                cbFilterTopR.ItemsSource = thisTopRisk.CopyToDataTable().DefaultView;
                cbFilterTopR.DisplayMemberPath = DT_RiskTree_Damages.DAMAGE;
                cbFilterTopR.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog(); ;
            }

        }

        public void LoadRectangles(RiskPolyLine Line)
        {
            try
            {
                DataRow[] thisTopRisk = Ds.Tables[DT_RiskTree_Damages.TABLENAME]
                        .Select(DT_RiskTree_Damages.ID_RISKTREE + " = " + ID_Diagram);
                double formula = 60 * thisTopRisk.Length / 2;
                double puntoinicialY = Line.Points[1].Y - formula;
                foreach (DataRow item in thisTopRisk)
                {
                    System.Drawing.Color colorete = System.Drawing.Color.FromArgb(int.Parse(item[DT_RiskTree_Damages.COLOR].ToString()));
                    Color mediaColor = Color.FromArgb(colorete.A, colorete.R, colorete.G, colorete.B);
                    TopRisk rectangle = new TopRisk(GridPaintLines,
                    new Point(Line.Points[1].X + 5, puntoinicialY),  "Acum. Val (" + item[DT_RiskTree_Damages.UM].ToString() + ") :", "Acum. Dam.:", mediaColor, (Int32)item[DT_RiskTree_Damages.ID_DAMAGE], item[DT_RiskTree_Damages.UM].ToString());
                    //rectangle.BringToFront();
                    Rectangles.Add(rectangle);
                    puntoinicialY += 60;
                }
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog(); ;
            }
        }

        public void FixDrawPanel(List<RiskPolyLine> lineas)
        {
            //Fix the Draw Panel at Diagram Size
            if (lineas.OrderBy(rl => rl.Points[0].X).First().Points[0].X < 0)
            {
                //while any point of the diagram is below of the visual Grid, move the entre diagram to de Right
                while (lineas.OrderBy(rl => rl.Points[0].X).First().Points[0].X < 0)
                {
                    TreeOperation.MoveEntireTree(LinesList, 70, 0, Rectangles);
                }
            }
            GridPaintLines.Width = lineas.OrderByDescending(rl => rl.Points[1].X).First().Points[1].X + 500;
            if (lineas.OrderBy(rl => rl.Points[0].Y).First().Points[0].Y < 0)
            {
                while (lineas.OrderBy(rl => rl.Points[0].Y).First().Points[0].Y < 0)
                {
                    TreeOperation.MoveEntireTree(LinesList, 0, 210, Rectangles);
                }
            }
            GridPaintLines.Height = lineas.OrderByDescending(rl => rl.Points[1].Y).First().Points[1].Y + 300;
        }


        #endregion

        #region MainLine
        private void PrincipalLine_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Loose = false;
                IdRiskSelected = MainLine.ID;
                isRootSelected = true;
                //Line_Selected = MainLine;
                MenuRisk.IsEnabled = true;
                int ID_Sender;
                bool IsRoot_Sender;
                Loose = true;
                if (sender is RiskPolyLine)
                {
                    ID_Sender = ((RiskPolyLine)sender).ID;
                    IsRoot_Sender = ((RiskPolyLine)sender).IsRoot;
                }
                else
                {
                    ID_Sender = ((LabelPolyLine)sender).Line.ID;
                    IsRoot_Sender = ((LabelPolyLine)sender).Line.IsRoot;
                }
                if (Moviendo)
                {
                    //checking if user don't bad click out of line for moving
                    //chekea que el usuario no de un mal clic 
                    if (Line_Selected.IdRiskFather != ID_Sender && Line_Selected.ID != ID_Sender)
                    {
                        Moviendo = false;
                        Ds.Tables[DT_RiskStructure.RiskStructure_TABLA].Select(DT_RiskStructure.IDRISK_COLUMNA + " = " + Line_Selected.ID).First()[DT_RiskStructure.IDRISKFATHER_COLUMNA] = ID_Sender;
                        foreach (var item in LinesList.FindAll(rl => rl.IdRiskFather == Line_Selected.IdRiskFather && rl.Position > Line_Selected.Position).ToList())
                        {//decremento en 1 la posicion de los risk, pa las cm no hace falta pues estan delante
                            item.Position -= 1;
                            if (Ds.Tables[DT_Risk.TABLE_NAME].Rows.Contains(item.ID))
                            {
                                Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(item.ID)[DT_Risk.POSITION_COLUMN] = item.Position;
                            }
                            if (Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Contains(item.ID))
                            {
                                Ds.Tables[DT_CounterM.TABLE_NAME].Rows.Find(item.ID)[DT_CounterM.POSITION_COLUMN] = item.Position;
                            }
                        }
                        Line_Selected.Position = LinesList.Find(x => x.ID == ID_Sender).Children.Count;

                        Line_Selected.Father = LinesList.Find(x => x.ID == ID_Sender);

                        Line_Selected.Father.Children.Add(Line_Selected);

                        Ds.Tables[DT_Risk.TABLE_NAME].Rows.Find(Line_Selected.ID)[DT_Risk.POSITION_COLUMN] = Line_Selected.Position;
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
                        //TreeOperation.DetectarMiPosicionActualInMain(Line_Selected, e.GetPosition(GridPaintLines), Ds);
                        //SaveData();
                        //DropLines(LinesList);
                        //DropRectangles();

                        //LoadLines(Ds);
                        //LoadRectangles(IdTreeRisk, MainLine);

                        //DrawNumbers(IdTreeRisk, Ds, IdPrincipalLine, Rectangles);
                        //LineThickness(LinesList);
                        //TextProbabilityChange(MainLine);
                        //CruzarTablaRisk(LinesList);
                        //CruzarTablaCM(LinesList);
                    }
                    else
                    {
                        //TreeOperation.DetectarMiPosicionActualInMain(Line_Selected, e.GetPosition(GridPaintLines), Ds);
                        //SaveData();
                        //DropLines(LinesList);
                        //DropRectangles();
                        //LoadLines(Ds);
                        //LoadRectangles(IdTreeRisk, MainLine);
                        //DrawNumbers(IdTreeRisk, Ds, IdPrincipalLine, Rectangles);
                        //LineThickness(LinesList);
                        //TextProbabilityChange(MainLine);
                        //CruzarTablaRisk(LinesList);
                        //CruzarTablaCM(LinesList);
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
                if (Moviendo)
                {
                    if (Line_Selected.IdRiskFather != ((RiskPolyLine)sender).ID && Line_Selected.ID != ((RiskPolyLine)sender).ID)
                    {
                        Line_Selected.IdRiskFather = ((RiskPolyLine)sender).ID;
                        Line_Selected.Position = ((RiskPolyLine)sender).Children.Count;
                        Moviendo = false;
                        Ds.Tables[DT_RiskStructure.RiskStructure_TABLA].Select(DT_RiskStructure.IDRISK_COLUMNA + " = " + Line_Selected.ID).First()[DT_RiskStructure.IDRISKFATHER_COLUMNA] = ((RiskPolyLine)sender).ID;
                        DataRow[] drRoleRisk = Ds.Tables[DT_Role_Risk.TABLENAME].Select(DT_Role_Risk.ID_RISK + " = " + ((RiskPolyLine)sender).ID);
                        DataRow[] drRoleCM = Ds.Tables[DT_Role_CM.TABLENAME].Select(DT_Role_CM.ID_CM + " = " + ((RiskPolyLine)sender).ID);
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
                        //Cursor = Cursors.Arrow;
                        //SaveData();
                        //DropLines(LinesList);
                        //DropRectangles();
                        //LoadLines(Ds);
                        //LoadRectangles(IdTreeRisk, MainLine);
                        //DrawNumbers(IdTreeRisk, Ds, IdPrincipalLine, Rectangles);
                        //LineThickness(LinesList);
                        //TextProbabilityChange(MainLine);
                        //CruzarTablaRisk(LinesList);
                        //CruzarTablaCM(LinesList);
                    }
                    else
                    {
                        if (Line_Selected.IdRiskFather == ((RiskPolyLine)sender).ID)
                        {
                            TreeOperation.DetectarMiPosicionActualInMain(Line_Selected, e.GetPosition(GridPaintLines), Ds);
                            //SaveData();
                            //DropLines(LinesList);
                            //DropRectangles();
                            //LoadLines(Ds);
                            //LoadRectangles(IdTreeRisk, MainLine);
                            //DrawNumbers(IdTreeRisk, Ds, IdPrincipalLine, Rectangles);
                            //LineThickness(LinesList);
                            //TextProbabilityChange(MainLine);
                            //CruzarTablaRisk(LinesList);
                            //CruzarTablaCM(LinesList);
                        }
                        else
                        {
                            Moviendo = false;
                            MoviendoCM = false;
                            //Cursor = Cursors.Arrow;
                            //TreeOperation.DrawEntireDiagramAsFishBone(MainLine);
                            //TreeOperation.FixMainLine(LinesList, MainLine);
                            //FixDrawPanel(LinesList);
                            //LineThickness(LinesList);
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

        private void GridPaintLines_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;

            if (e.Delta > 0 && sliderZoom.Value < 200)
            {
                sliderZoom.Value += 10;
            }

            if (e.Delta < 0 && sliderZoom.Value > 10)
            {
                sliderZoom.Value -= 10;
            }
            //BrigIntoViewSelectedRiskPolyline();
            Point mouseAtImage = e.GetPosition(GridPaintLines); // ScrollViewer_CanvasMain.TranslatePoint(middleOfScrollViewer, Canvas_Main);
            Point mouseAtScrollViewer = e.GetPosition(ScrollGridPaint);

            ScaleTransform st = CanvasMain.LayoutTransform as ScaleTransform;
            if (st == null)
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
            TheGrid.UpdateLayout();
            #endregion

            Vector offset = GridPaintLines.TranslatePoint(mouseAtImage, ScrollGridPaint) - mouseAtScrollViewer; // (Vector)middleOfScrollViewer;
            ScrollGridPaint.ScrollToHorizontalOffset(offset.X);
            ScrollGridPaint.ScrollToVerticalOffset(offset.Y);
            TheGrid.UpdateLayout();

            e.Handled = true;
        }

        private void GridPaintLines_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            try
            {
                //if (!(NameEditing))
                //{
                //    if (e.LeftButton == MouseButtonState.Pressed)
                //    {
                //        if (!(Loose))
                //        {
                //            if (Line_Selected.IsCM)
                //            {
                //                if (!(MoviendoCM))
                //                {
                //                    MoviendoCM = true;
                //                    LinesMoving = new List<RiskPolyLine>() { Line_Selected };
                //                    Cursor = Cursors.Hand;
                //                    //VerticalMenu win = new VerticalMenu("Moving");
                //                    //win.ShowDialog();
                //                }
                //                TreeOperation.MoveLines(LinesMoving, e.GetPosition(GridPaintLines).X - Line_Selected.Points[1].X - 35, e.GetPosition(GridPaintLines).Y - Line_Selected.Points[1].Y);
                //                X = e.GetPosition(GridPaintLines).X;
                //                Y = e.GetPosition(GridPaintLines).Y;
                //                Main_Y = MainLine.Points[0].Y;
                //            }
                //            else
                //            {
                //                if (!(Moviendo))
                //                {
                //                    Moviendo = true;
                //                    LinesMoving = new List<RiskPolyLine>();
                //                    LinesMoving.AddRange(TreeOperation.GetMeAndAllChildsWithCM(Line_Selected));
                //                    Cursor = Cursors.Hand;
                //                    //VerticalMenu win = new VerticalMenu("Moving");
                //                    //win.ShowDialog();
                //                }
                //                if (Moviendo)
                //                {
                //                    TreeOperation.MoveLines(LinesMoving, e.GetPosition(GridPaintLines).X - Line_Selected.Points[1].X - 35, e.GetPosition(GridPaintLines).Y - Line_Selected.Points[1].Y);
                //                    X = e.GetPosition(GridPaintLines).X;
                //                    Y = e.GetPosition(GridPaintLines).Y;
                //                    Main_Y = MainLine.Points[0].Y;
                //                }
                //            }
                //        }
                //    }
                //    else
                //    {
                //        if (Moviendo || MoviendoCM)
                //        {
                //            TreeOperation.MoveLines(LinesMoving, e.GetPosition(GridPaintLines).X - Line_Selected.Points[1].X - 35, e.GetPosition(GridPaintLines).Y - Line_Selected.Points[1].Y);
                //            X = e.GetPosition(GridPaintLines).X;
                //            Y = e.GetPosition(GridPaintLines).Y;
                //            Main_Y = MainLine.Points[0].Y;
                //        }
                //    }
                //    if (Creando)
                //    {
                //        Line_Created.DrawAtPoint(e.GetPosition(GridPaintLines), "");
                //        TreeOperation.MoveLines(new List<RiskPolyLine>() { Line_Created }, e.GetPosition(GridPaintLines).X - Line_Created.Points[1].X - 35, e.GetPosition(GridPaintLines).Y - Line_Created.Points[1].Y);
                //        X = e.GetPosition(GridPaintLines).X;
                //        Y = e.GetPosition(GridPaintLines).Y;
                //        Main_Y = MainLine.Points[0].Y;
                //    }
                //    //Si se movio el arbol, regista la posicion de las lineas
                //}
            }
            catch (Exception ex)
            {
                new WindowMessageOK(ex.Message).ShowDialog();
            }

        }

        private void GridPaintLines_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void GridPaintLines_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

        }

        private void textChangeName_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {

        }

        private void TextChangeName_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void TextChangeName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private void CbFilterTopR_DropDownClosed(object sender, System.EventArgs e)
        {

        }

        private void BtnMinusZoom_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sliderZoom.Value > 10)
            {
                sliderZoom.Value -= 10;
            }
        }

        private void BtnPlusZoom_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sliderZoom.Value < 200)
            {
                sliderZoom.Value += 10;
            }
        }

        private void SliderZoom_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            sliderZoom.Value = Convert.ToDouble(General.MyRound(Convert.ToDecimal(sliderZoom.Value), 0));

            ScaleTransform st = CanvasMain.LayoutTransform as ScaleTransform;
            if (st == null)
            {
                st = new ScaleTransform();
                CanvasMain.LayoutTransform = st;
            }
            txtZoom.Text = sliderZoom.Value.ToString();
            st.ScaleX = st.ScaleY = sliderZoom.Value / 100;
            e.Handled = true;
        }

        private void sliderZoom_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (e.Delta > 0 && sliderZoom.Value < 200)
            {
                sliderZoom.Value += 10;
            }

            if (e.Delta < 0 && sliderZoom.Value > 10)
            {
                sliderZoom.Value -= 10;
            }
        }
    }
}
