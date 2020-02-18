using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Petzold.Media2D;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Data;

namespace EnsureBusinesss.Business
{
    public class RiskPolyLine : ArrowPolyline
    {
        //private const int diagonalShiftX = 75;
        public const double diagonalShiftX = 0;
        //public const double diagonalShiftY = 175;
        public const double diagonalShiftY = 185;
        public const double horizontalShiftX = 200;
        public const double diagonalCMTailX = 9;
        public const double diagonalCMTailY = 21;
        public const double horizontalCMTailX = 30;

        public const double diagonalShiftLabelX = 50;
        public const double diagonalShiftLabelY = 185;
        public const double horizontalShiftLabelX = 200;
        public bool FullAccess { get; set; }
        public const double angle = 67.625;
        public int ID { get; set; }
        public const double basicX = 3;
        public const double basicY = 7;
        public double XTreme { get; set; }
        public double YxTreme { get; set; }
        public Point StartDrawPoint { get; set; }
        public Point EndDrawPoint { get; set; }
        public bool IsDiagonal { get; set; }
        public RiskPolyLine Father { get; set; }
        public double PxThickness { get; set; }
        public bool IsActivated { get; set; }
        public LineGroup Group { get; set; }
        public int IdRiskFather { get; set; }
        public string ShortName { get; set; }
        public bool FromTop { get; set; }
        public bool IsRoot { get; set; }
        public int Size { get; set; }

        public decimal AcValue { get; set; }
        public decimal OwnValue { get; set; }
        public decimal AcDamage { get; set; }
        public decimal AcLike { get; set; }

        public bool IsCM { get; set; }
        public bool Collapsed { get; set; }
        public decimal Probability { get; set; }
        public int Position { get; set; }
        public int MyLevel { get; set; }
        public LabelPolyLine MyName { get; set; }
        public StackPanel ElStackPannel { get; set; }
        public List<RiskPolyLine> Children { get; set; }
        public PictureBoxPolyLine Expand { get; set; }
        public Grid MyContainer { get; set; }
        //public decimal Class { get; set; }

        //public static RoutedEvent DobleClick;
        //public List<RiskPolyLine> Segments { get; set; }
        public List<SegmentPolyLine> Segments { get; set; }
        //public ThicknessProvider thicknessProvider { get; set; }
        public RiskPolyLine()
        {
            //Segments = new List<RiskPolyLine>();
            Segments = new List<SegmentPolyLine>();
        }
        public RiskPolyLine(Grid Container, ContextMenu Menu, bool isCMI)
        {
            Size = 1;
            //Points.Add(new Point());
            Points.Add(new Point());
            Points.Add(new Point());
            IsCM = isCMI;
            IsActivated = true;
            Container.Children.Add(this);
            MyContainer = Container;
            Panel.SetZIndex(this, 1);
            StrokeThickness = 2;
            Stroke = new SolidColorBrush(Colors.RoyalBlue);
            ContextMenu = Menu;
            MyName = new LabelPolyLine
            {
                Text = ShortName,
                ContextMenu = Menu,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            MyName.TextTrimming = TextTrimming.CharacterEllipsis;
            MyName.TextWrapping = TextWrapping.Wrap;
            Expand = new PictureBoxPolyLine
            {
                Risk = this,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = Visibility.Collapsed
            };
            ElStackPannel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top

            };
            ElStackPannel.Children.Add(Expand);
            ElStackPannel.Children.Add(MyName);
            Container.Children.Add(ElStackPannel);
            if (Collapsed)
            {
                Expand.Source = new BitmapImage(new Uri(General.CONTRAIDO));
            }
            else
            {
                Expand.Source = new BitmapImage(new Uri(General.EXPANDIDO));
            }

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Children = new List<RiskPolyLine>();
            if (isCMI)
            {
                StrokeDashArray = new DoubleCollection { 5, 2 };
                ArrowEnds = ArrowEnds.None;
                Stroke = new SolidColorBrush(Colors.Black);
            }
            //Segments = new List<RiskPolyLine>();
            Segments = new List<SegmentPolyLine>();
        }

        public bool IsLeaf()
        {
            return Children.Count == 0;
        }

        public void NewDrawAtPoint(Point StartPoint, string LabelName)
        {
            //Las flechas se dibujan del punto 0 hacia el punto 1. 
            //El punto 0 es el origen de la flecha. El punto 1 es donde termina la flecha
            StartDrawPoint = StartPoint;
            Points[1] = StartPoint;//punto donde se pone la flecha
            if (IsDiagonal)
            {
                Points[0] = new Point(Points[1].X - diagonalShiftX, Points[1].Y - diagonalShiftY);
            }
            else
            {
                Points[0] = new Point(Points[1].X - horizontalShiftX, Points[1].Y);
            }

            XTreme = Points[0].X;
            YxTreme = Points[0].Y;
            LabelLineName(LabelName, 22.375);
            ShortName = LabelName;
        }
        public void NewDrawAtPoint(Point StartPoint)
        {
            //Las flechas se dibujan del punto 0 hacia el punto 1. 
            //El punto 0 es el origen de la flecha. El punto 1 es donde termina la flecha
            StartDrawPoint = StartPoint;
            Points[1] = StartPoint;//punto donde se pone la flecha
            if (IsDiagonal)
            {
                if (FromTop)
                {
                    Points[0] = new Point(Points[1].X - 30, Points[1].Y - 70);
                }
                else
                {
                    Points[0] = new Point(Points[1].X - 30, Points[1].Y + 70);
                }
            }
            else
            {
                Points[0] = new Point(Points[1].X - 100, Points[1].Y);
            }
        }
        private void LabelLineName(string LabelName, double AngleX)
        {
            MyName.MaxWidth = 180;
            ElStackPannel.RenderTransformOrigin = new Point(0, 0);
            MyName.MaxHeight = 40;
            if (!(LabelName is null))
            {
                this.MyName.Text = LabelName;
            }

            if (IsDiagonal)
            {
                if (FromTop)
                {
                    double extra = (LabelName.Length > 26) ? 13 : -5;
                    //-2.5 * (RiskPolyLine.diagonalCMTailX) + line.Points[1].Y
                    //this.ElStackPannel.Margin = new Thickness(StartDrawPoint.X - RiskPolyLine.diagonalShiftLabelX + extra, StartDrawPoint.Y - RiskPolyLine.diagonalShiftLabelY, 0, 0);
                    this.ElStackPannel.Margin = new Thickness(Points[1].X - RiskPolyLine.diagonalShiftLabelX + extra, Points[1].Y - RiskPolyLine.diagonalShiftLabelY, 0, 0);
                    RotateTransform rotateTransform1 = new RotateTransform(AngleX);
                    this.ElStackPannel.RenderTransform = rotateTransform1;
                }
                else
                {
                    double extra = (LabelName.Length > 26) ? -19 : -19;
                    //this.ElStackPannel.Margin = new Thickness(StartDrawPoint.X - RiskPolyLine.diagonalShiftLabelX + extra, StartDrawPoint.Y + RiskPolyLine.diagonalShiftLabelY - 10, 0, 0);
                    this.ElStackPannel.Margin = new Thickness(Points[1].X - RiskPolyLine.diagonalShiftLabelX + extra, Points[1].Y + RiskPolyLine.diagonalShiftLabelY - 10, 0, 0);

                    RotateTransform rotateTransform1 = new RotateTransform(-AngleX);
                    this.ElStackPannel.RenderTransform = rotateTransform1;
                }
            }
            else
            {
                //this.ElStackPannel.Margin = new Thickness(StartDrawPoint.X - RiskPolyLine.horizontalShiftLabelX, StartDrawPoint.Y + 1, 0, 0);
                this.ElStackPannel.Margin = new Thickness(Points[1].X - RiskPolyLine.horizontalShiftLabelX, Points[1].Y + 1, 0, 0);

            }

            if (IsCM)
            {
                Expand.Visibility = Visibility.Collapsed;
            }
            this.MyName.Line = this;
        }

        public void SetMenu(ContextMenu Menu)
        {
            ContextMenu = Menu;
            MyName.ContextMenu = Menu;
            foreach (var item in Segments)
            {
                item.ContextMenu = Menu;
            }
        }

        public void SetColor(SolidColorBrush color)
        {
            Stroke = color;
            foreach (var item in Segments)
            {
                item.Stroke = color;
            }
        }
        public void SetThickness(decimal cost, decimal min, decimal max)
        {

            decimal value = max - min;

            decimal skoda = value / 3;

            if (cost <= skoda)
            {
                this.StrokeThickness = 1;
            }
            else
            {
                if (cost > skoda && cost <= 2 * skoda)
                {
                    //this.StrokeThickness = 2;
                    this.StrokeThickness = 1;
                }
                else
                {
                    if (cost > 2 * skoda && cost <= 3 * skoda)
                    {
                        this.StrokeThickness = 3;
                    }
                    else
                    {
                        if (cost > 3 * skoda && cost <= 4 * skoda)
                        {
                            //this.StrokeThickness = 4;
                            this.StrokeThickness = 6;
                        }
                        else
                        {
                            if (cost > 4 * skoda && cost <= 5 * skoda)
                            {
                                //this.StrokeThickness = 5;
                                this.StrokeThickness = 6;
                            }
                            else
                            {
                                this.StrokeThickness = 6;
                            }
                        }
                    }
                }
            }
            PxThickness = StrokeThickness;
            //if (Segments.Any())
            //{
            //    IEnumerable<RiskPolyLine> orderedChild = Children.OrderBy(pl => pl.Points[1].X);
            //    //int i = 0;
            //    int segmentIndex = Segments.Count - 1;
            //    SegmentPolyLine segment;
            //    SegmentPolyLine lastSegment = Segments[segmentIndex];
            //    lastSegment.StrokeThickness = 1;

            //    foreach (RiskPolyLine rpl in orderedChild)
            //    {
            //        segmentIndex--;
            //        if (segmentIndex >= 0)
            //        {
            //            segment = Segments[segmentIndex];

            //            if (segment.StrokeThickness < rpl.StrokeThickness)
            //            {
            //                segment.StrokeThickness = rpl.StrokeThickness;
            //            }
            //            if (segment.StrokeThickness < lastSegment.StrokeThickness)
            //            {
            //                segment.StrokeThickness = lastSegment.StrokeThickness;
            //            }
            //            lastSegment = segment;
            //        }
            //    }
            //}

            //foreach (SegmentPolyLine segment in Segments)
            //{
            //    segment.StrokeThickness = StrokeThickness;
            //}
        }
        public void DrawEntireLine(string LabelName)
        {
            //LabelLineName(LabelName);
            LabelLineName(LabelName, angle);
            ShortName = LabelName;
        }
        public void Increase()
        {
            if (IsDiagonal)
            {
                if (FromTop)
                {
                    Points[0] = new Point(Points[0].X - (basicX * 29), Points[0].Y - (basicY * 29));
                }
                else
                {
                    Points[0] = new Point(Points[0].X - (basicX * 29), Points[0].Y + (basicY * 29));
                }
            }
            else
            {
                Points[0] = new Point(Points[0].X - 210, Points[0].Y);
            }
            Size++;
            XTreme = Points[0].X;
            YxTreme = Points[0].Y;
        }
        public double XTremee()
        {
            double xtremo = XTreme;
            if (!(Collapsed))
            {
                xtremo = TreeOperation.GetMeAndAllChildsWithCM(this).OrderBy(x => x.Points[0].X).First().Points[0].X;

            }

            XTreme = xtremo;
            return XTreme;
        }
        public double YxTremee()
        {
            double ytremo = YxTreme;
            if (!(Collapsed))
            {
                if (FromTop)
                {
                    ytremo = TreeOperation.GetMeAndAllChildsWithCM(this).OrderBy(x => x.Points[0].Y).First().Points[0].Y;
                }
                else
                {
                    // TODO: revisar yxtremee esta fundio para la mitad de abajo
                    ytremo = TreeOperation.GetMeAndAllChildsWithCM(this).OrderBy(x => x.Points[0].Y).First().Points[0].Y;
                    //ytremo = TreeOperation.GetMeAndAllChildsWithCM(this).OrderByDescending(x => x.Points[0].Y).First().Points[0].Y;
                }
            }

            YxTreme = ytremo;
            return YxTreme;
        }
        public double AbsoluteYxTremee()
        {
            double ytremo = YxTreme;
            if (!(Collapsed))
            {
                ytremo = TreeOperation.GetMeAndAllChildsWithCM(this).OrderBy(x => x.Points[0].Y).First().Points[0].Y;
            }

            YxTreme = ytremo;
            return YxTreme;
        }
        public void DrawSingleLine()
        {
            if (IsDiagonal)
            {
                if (FromTop)
                {
                    Points[0] = new Point(Points[1].X - 30, Points[1].Y - 70);
                }
                else
                {
                    Points[0] = new Point(Points[1].X - 30, Points[1].Y + 70);
                }
            }
            else
            {
                Points[0] = new Point(Points[1].X - horizontalShiftX, Points[1].Y);
            }
        }
        /// <summary>
        /// Change Visibility for LabelName and Expand
        /// </summary>
        /// <param name="aVisibility"></param>
        public void ExtrasVisibility(System.Windows.Visibility aVisibility)
        {
            MyName.Visibility = aVisibility;
            if (Children.Count > 0)
            {
                Expand.Visibility = aVisibility;
            }
        }
        /// <summary>
        /// Dado una rama busca el mayor extremo x de sus hijos horizontales
        /// una rama es un hijo de mainline con toda su descendencia, 
        /// siempre es vertical
        /// </summary>
        /// <returns></returns>
        public Point HorizontalMaxXTremee(double altura)
        {
            Point xtremo = new Point(XTreme, YxTreme);
            if (!(Collapsed))
            {
                if (altura == 101)
                {
                    //xtremo = TreeOperation.GetMeAndAllChildsWithCM(this).OrderBy(x => x.Points[0].X).First().Points[0];
                    xtremo = TreeOperation.GetMeAndAllChildsWithCM(this).OrderBy(x => x.MyMinXPoint().X).First().MyMinXPoint();
                }
                else
                {
                    //RiskPolyLine rtremo = TreeOperation.GetMeAndAllChildsWithCM(this).Where(x => (x.Points[0].Y > altura)).OrderBy(x => x.Points[0].X).FirstOrDefault();
                    RiskPolyLine rtremo = TreeOperation.GetMeAndAllChildsWithCM(this).Where(x => (x.MyMinXPoint().Y > altura)).OrderBy(x => x.MyMinXPoint().X).FirstOrDefault();
                    //xtremo = (rtremo != null) ? rtremo.Points[0] : xtremo;
                    xtremo = (rtremo != null) ? rtremo.MyMinXPoint() : xtremo;
                }
            }
            return xtremo;
        }
        /// <summary>
        /// Dado una rama busca la cantidad de sus hijos horizontales
        /// una rama es un hijo de mainline con toda su descendencia, 
        /// siempre es vertical
        /// </summary>
        /// <returns></returns>
        //public int HorizontalMaxCountChildren()
        //{
        //    int result = 0;
        //    if (!(Collapsed))
        //    {
        //        result = TreeOperation.GetMeAndAllChildsWithCM(this).Where(x => (x.IsDiagonal == false)).Count();
        //    }
        //    return result;
        //}
        //public int VerticalMaxCountChildren()
        //{
        //    int result = 0;
        //    if (!(Collapsed))
        //    {
        //        result = TreeOperation.GetMeAndAllChildsWithCM(this).Where(x => (x.IsDiagonal == true)).Count();
        //    }
        //    return result;
        //}

        //public Point HorizontalMaxXTremee(Double Y)
        //{
        //    Point xtremo = new Point(XTreme, YxTreme);
        //    if (!(Collapsed))
        //    {
        //        xtremo = TreeOperation.GetMeAndAllChildsWithCM(this).Where(x => (x.IsDiagonal == false) && (x.Points[0].Y<Y)).OrderBy(x => x.Points[0].X).Last().Points[0];
        //    }
        //    return xtremo;
        //}
        public void ExtendHorizontal(double FromX)
        {
            CreateSegmentAt(new Point(FromX - horizontalShiftX, Points[0].Y));
        }
        public void ExtendVertical(double FromY)
        {
            CreateSegmentAt(new Point(Points[0].X, FromY - diagonalShiftY));
        }
        private void CreateSegmentAt(Point startPoint)
        {
            SegmentPolyLine segment = new SegmentPolyLine(this)
            {
                StartDrawPoint = startPoint
            };

            Segments.Add(segment);
            segment.EndDrawPoint = Children[Segments.Count - 1].Points[1];

            XTreme = segment.Points[0].X;
            YxTreme = segment.Points[0].Y;
            MyContainer.Children.Add(segment);
            segment.MyContainer = MyContainer;
            segment.ContextMenu = ContextMenu;
        }
        public void Move(int deltaX, int deltaY)
        {
            Points[0] = new Point(Points[0].X + deltaX, Points[0].Y + deltaY);
            Points[1] = new Point(Points[1].X + deltaX, Points[1].Y + deltaY);
            StartDrawPoint = Points[1];

            if (Segments.Any())
            {
                MoveSegments(deltaX, deltaY);
            }
            if (Points.Count == 3)
            {
                MoveTail(deltaX, deltaY);
            }
        }
        public void MoveSegments(int deltaX, int deltaY)
        {
            foreach (var segment in Segments)
            {
                segment.Points[0] = new Point(segment.Points[0].X + deltaX, segment.Points[0].Y + deltaY);
                segment.Points[1] = new Point(segment.Points[1].X + deltaX, segment.Points[1].Y + deltaY);
                StartDrawPoint = segment.Points[1];
            }
        }
        private void MoveTail(int deltaX, int deltaY)
        {
            Points[2] = new Point(Points[2].X + deltaX, Points[2].Y + deltaY);
        }
        private List<SegmentPolyLine> GetChildAndMeSegments()
        {
            List<SegmentPolyLine> segmentsToReturn = new List<SegmentPolyLine>();

            if (Segments != null && Segments.Any())
            {
                segmentsToReturn.AddRange(Segments);
            }
            foreach (RiskPolyLine child in Children)
            {
                segmentsToReturn.AddRange(child.GetChildAndMeSegments());
            }
            return segmentsToReturn;
        }

        //public Double SegmentMinX()
        //{
        //    double minX = 0;
        //    List<SegmentPolyLine> segments;
        //    if (!(Collapsed))
        //    {
        //        segments = GetChildAndMeSegments();
        //        if (segments.Any())
        //        {
        //            minX = segments.Min(s => s.Points[0].X);
        //        }
        //    }
        //    return minX;
        //}

        //public Double PointMinX()
        //{
        //    double minX = 0;
        //    List<SegmentPolyLine> segments;
        //    if (!(Collapsed))
        //    {
        //        segments = GetChildAndMeSegments();
        //        if (segments.Any())
        //        {
        //            minX = segments.Min(s => s.Points[0].X);
        //        }
        //    }
        //    return minX;
        //}
        public Point MyMinXPoint()
        {
            if (Segments != null && Segments.Any())
            {
                return Segments.OrderBy(s => s.Points[0].X).First().Points[0];
            }
            else
            {
                return Points[0];
            }
        }
        public void AllSegmentClear()
        {
            if (Segments != null && Segments.Any())
            {
                Segments.Clear();
                if (Children != null && Children.Any())
                {
                    foreach (RiskPolyLine line in Children)
                    {
                        line.AllSegmentClear();
                    }
                }
            }
        }
        public void SegmentClear()
        {
            if (Segments != null && Segments.Any())
            {
                Segments.Clear();
            }
        }
        //protected override void OnMouseEnter(MouseEventArgs e)
        //{
        //    base.OnMouseEnter(e);
        //    if (Father != null)
        //    {
        //        OnMouseEnterStrokeThickness();
        //    }
        //}
        //protected override void OnMouseLeave(MouseEventArgs e)
        //{
        //    base.OnMouseLeave(e);
        //    if (!IsRoot)
        //    {
        //        //StrokeThickness = Segments == null ? 1 : (Father != null ? Father.StrokeThickness : 1);
        //        //OnMouseEnterStrokeThickness();
        //        OnMouseLeaveStrokeThickness();
        //    }
        //}
        //public virtual void OnMouseEnterStrokeThickness()
        //{
        //    if (!IsCM && !IsRoot)
        //    {
        //        StrokeThickness = 10;
        //        foreach (SegmentPolyLine segment in Segments)
        //        {
        //            segment.StrokeThickness = StrokeThickness;
        //        }
        //    }
        //}

        //public virtual void OnMouseLeaveStrokeThickness()
        //{
        //    if (!IsCM && !IsRoot)
        //    {
        //        thicknessProvider.UpdateThickness();
        //        //foreach (SegmentPolyLine segment in Segments)
        //        //{
        //        //    segment.StrokeThickness = StrokeThickness;
        //        //}
        //    }
        //}
        //private bool IsMainLine()
        //{ 
        //    return Father == null;
        //}
        public void AddTail()
        {
            if (!IsCM && Children.Any())
            {
                Point starPointAt = (Segments != null && Segments.Any()) ? Segments.Last().Points[0] : Points[0];

                if (IsDiagonal)
                {
                    if (FromTop)
                    {
                        CreateSegmentAt(new Point(starPointAt.X - General.basicX * 5, 2.5 * (-General.basicX * 5) + starPointAt.Y));
                    }
                    else
                    {
                        CreateSegmentAt(new Point(starPointAt.X - General.basicX * 5, -2.5 * (-General.basicX * 5) + starPointAt.Y));
                    }
                }
                else
                {
                    CreateSegmentAt(new Point(starPointAt.X - 25, starPointAt.Y));
                }

                starPointAt = Segments.Last().Points[0];

                XTreme = starPointAt.X;
                YxTreme = starPointAt.Y;

                AddChildrenTail();
            }
        }
        private void AddChildrenTail()
        {
            foreach (var item in Children)
            {
                item.AddTail();
            }
        }
        public void UpdateSegmentsStrokeThickness()
        {
            if (Segments.Any())
            {
                IEnumerable<RiskPolyLine> orderedChild = Children.OrderBy(pl => pl.Points[1].X);
                //int i = 0;
                int segmentIndex = Segments.Count - 1;
                SegmentPolyLine segment;
                SegmentPolyLine lastSegment = Segments[segmentIndex];
                lastSegment.StrokeThickness = 1;

                foreach (RiskPolyLine rpl in orderedChild)
                {
                    segmentIndex--;
                    if (segmentIndex >= 0)
                    {
                        segment = Segments[segmentIndex];
                        segment.StrokeThickness = 1;
                        if (!rpl.IsCM)
                        {
                            if (segment.StrokeThickness < rpl.StrokeThickness)
                            {
                                segment.StrokeThickness = rpl.StrokeThickness;
                            }
                            if (segment.StrokeThickness < lastSegment.StrokeThickness)
                            {
                                segment.StrokeThickness = lastSegment.StrokeThickness;
                            }
                        }
                        else
                        {
                            segment.StrokeThickness = lastSegment.StrokeThickness;
                        }
                        lastSegment = segment;
                    }
                }
            }
        }

        public void UpdateSegmentsStroke()
        {
            if (Segments != null)
            {
                foreach (var segment in Segments)
                {
                    segment.Stroke = Stroke;
                }
            }
        }
    }
}
