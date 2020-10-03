using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Data;

namespace EnsureBusinesss.Business
{
    public class RiskPolyLine : ArrowPolyline
    {
        #region Constatntes
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
        public const double horizontalShiftLabelX = 180;
        public const double angle = 67.625;
        public const double basicX = 3;
        public const double basicY = 7;
        #endregion

        #region Properties
        #region Booleans


        //public static readonly DependencyProperty FromTopProperty =
        //   DependencyProperty.Register("FromTop", typeof(bool), typeof(RiskPolyLine), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));

        //public static readonly DependencyProperty IsRootProperty =
        //   DependencyProperty.Register("IsRoot", typeof(bool), typeof(RiskPolyLine), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

        //public static readonly DependencyProperty OcultoProperty =
        //   DependencyProperty.Register("Oculto", typeof(bool), typeof(RiskPolyLine), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        //public static readonly DependencyProperty IsCMProperty =
        //   DependencyProperty.Register("IsCM", typeof(bool), typeof(RiskPolyLine), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));

        //public static readonly DependencyProperty CollapsedProperty =
        //   DependencyProperty.Register("Collapsed", typeof(bool), typeof(RiskPolyLine), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        //public static readonly DependencyProperty FullAccessProperty =
        //  DependencyProperty.Register("FullAccess", typeof(bool), typeof(RiskPolyLine), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        //public static readonly DependencyProperty IsDiagonalProperty =
        //  DependencyProperty.Register("IsDiagonal", typeof(bool), typeof(RiskPolyLine), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IsActivatedProperty =
            DependencyProperty.Register("IsActivated", typeof(bool), typeof(RiskPolyLine), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool IsActivated
        {
            set { SetValue(IsActivatedProperty, value); }
            get { return (bool)GetValue(IsActivatedProperty); }
        }

        public bool FromTop
        {
            set; /*{ SetValue(FromTopProperty, value); }*/
            get; /*{ return (bool)GetValue(FromTopProperty); }*/
        }
        public bool IsRoot
        {
            set;/* { SetValue(IsRootProperty, value); }*/
            get; /*{ return (bool)GetValue(IsRootProperty); }*/
        }
        public bool Hidden
        {
            set; /*{ SetValue(OcultoProperty, value); }*/
            get; /*{ return (bool)GetValue(OcultoProperty); }*/
        }
        public bool IsCM
        {
            set; /*{ SetValue(IsCMProperty, value); }*/
            get; /*{ return (bool)GetValue(IsCMProperty); }*/
        }
        public bool Collapsed
        {
            set; /*{ SetValue(CollapsedProperty, value); }*/
            get; /*{ return (bool)GetValue(CollapsedProperty); }*/
        }
        public bool FullAccess
        {
            set; /*{ SetValue(FullAccessProperty, value); }*/
            get; /*{ return (bool)GetValue(FullAccessProperty); }*/
        }
        public bool IsDiagonal
        {
            set; /*{ SetValue(IsDiagonalProperty, value); }*/
            get; /*{ return (bool)GetValue(IsDiagonalProperty); }*/
        }
        public bool IsMoving
        {
            set; /*{ SetValue(IsDiagonalProperty, value); }*/
            get; /*{ return (bool)GetValue(IsDiagonalProperty); }*/
        }
        #endregion

        #region Numeric

        public double XTreme { get; set; }

        public double YxTreme { get; set; }
       
        public decimal IdRiskFather { get; set; }

        public int MyLevel { get; set; }
        public decimal ID { get; set; }

        public static readonly DependencyProperty MinProperty =
          DependencyProperty.Register("Min", typeof(decimal), typeof(RiskPolyLine), new FrameworkPropertyMetadata(Convert.ToDecimal(0), FrameworkPropertyMetadataOptions.AffectsMeasure));

        public decimal Min
        {
            set { SetValue(MinProperty, value); }
            get { return (decimal)GetValue(MinProperty); }
        }

        public static readonly DependencyProperty MaxProperty =
          DependencyProperty.Register("Max", typeof(decimal), typeof(RiskPolyLine), new FrameworkPropertyMetadata(Convert.ToDecimal(0), FrameworkPropertyMetadataOptions.AffectsMeasure));

        public decimal Max
        {
            set { SetValue(MaxProperty, value); }
            get { return (decimal)GetValue(MaxProperty); }
        }

        public static readonly DependencyProperty AcDamageProperty =
          DependencyProperty.Register("AcDamage", typeof(decimal), typeof(RiskPolyLine), new FrameworkPropertyMetadata(Convert.ToDecimal(0), FrameworkPropertyMetadataOptions.AffectsMeasure));

        public decimal AcDamage
        {
            set { SetValue(AcDamageProperty, value); }
            get { return (decimal)GetValue(AcDamageProperty); }
        }

        public static readonly DependencyProperty AcValueProperty =
           DependencyProperty.Register("AcValue", typeof(decimal), typeof(RiskPolyLine), new FrameworkPropertyMetadata(Convert.ToDecimal(0), FrameworkPropertyMetadataOptions.AffectsMeasure));

        public decimal AcValue
        {
            set { SetValue(AcValueProperty, value); }
            get { return (decimal)GetValue(AcValueProperty); }
        }

        public static readonly DependencyProperty OwnValueProperty =
           DependencyProperty.Register("OwnValue", typeof(decimal), typeof(RiskPolyLine), new FrameworkPropertyMetadata(Convert.ToDecimal(0), FrameworkPropertyMetadataOptions.AffectsMeasure));

        public decimal OwnValue
        {
            set { SetValue(OwnValueProperty, value); }
            get { return (decimal)GetValue(OwnValueProperty); }
        }       

        public static readonly DependencyProperty AcLikeProperty =
           DependencyProperty.Register("AcLike", typeof(decimal), typeof(RiskPolyLine), new FrameworkPropertyMetadata(Convert.ToDecimal(0), FrameworkPropertyMetadataOptions.AffectsMeasure));

        public decimal AcLike
        {
            set { SetValue(AcLikeProperty, value); }
            get { return (decimal)GetValue(AcLikeProperty); }
        }

        public static readonly DependencyProperty ProbabilityProperty =
            DependencyProperty.Register("Probability", typeof(decimal), typeof(RiskPolyLine), new FrameworkPropertyMetadata(Convert.ToDecimal(0), FrameworkPropertyMetadataOptions.AffectsMeasure));

        public decimal Probability
        {
            set { SetValue(ProbabilityProperty, value); }
            get { return (decimal)GetValue(ProbabilityProperty); }
        }

        public int Position { get; set; }

        #endregion

        #region Stringss
        public static readonly DependencyProperty ShortNameProperty =
            DependencyProperty.Register("ShortName", typeof(string), typeof(RiskPolyLine), new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsMeasure));

        public string ShortName
        {
            set { SetValue(ShortNameProperty, value); }
            get { return (string)GetValue(ShortNameProperty); }
        }
        #endregion
        #endregion
        public Point StartDrawPoint { get; set; }
        public RiskPolyLine Father { get; set; }
        public LineGroup Group { get; set; }
        public LabelPolyLine MyName { get; set; }
        public LBorder TextPanel { get; set; }

        public List<RiskPolyLine> Children { get; set; }
        public PictureBoxPolyLine Expand { get; set; }
        public Grid MyContainer { get; set; }
        public List<SegmentPolyLine> Segments { get; set; }

        /// <summary>
        /// Es el punto donde esta la flecha originalmente
        /// </summary>
        private Point OriginalStartPoint { get; set; }
        public bool OriginalStartPointHasValue { get; set; } = false;

        public RiskPolyLine()
        {
            //Segments = new List<RiskPolyLine>();
            Segments = new List<SegmentPolyLine>();
            StrokeStartLineCap = PenLineCap.Round;
            StrokeEndLineCap = PenLineCap.Round;
        }
        public RiskPolyLine(Grid Container, ContextMenu Menu, bool isCMI)
        {
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
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.Black)
            };
            MyName.TextTrimming = TextTrimming.CharacterEllipsis;
            MyName.TextWrapping = TextWrapping.Wrap;
            Expand = new PictureBoxPolyLine
            {
                Risk = this,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = Visibility.Visible
            };

            StackPanel stkTextPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top                
               
            };

            TextPanel = new LBorder
            {
                CornerRadius = new CornerRadius(1),
                BorderBrush = this.Stroke,
                BorderThickness = new Thickness(1),                
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            stkTextPanel.Children.Add(Expand);
            stkTextPanel.Children.Add(MyName);
            TextPanel.Child = stkTextPanel;
            Container.Children.Add(TextPanel);

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            Children = new List<RiskPolyLine>();
            if (isCMI)
            {
                StrokeDashArray = new DoubleCollection { 5, 2 };
                ArrowEnds = ArrowEnds.None;
                Stroke = new SolidColorBrush(Colors.Black);
            }
            Segments = new List<SegmentPolyLine>();
            StrokeStartLineCap = PenLineCap.Round;
            StrokeEndLineCap = PenLineCap.Round;
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
            DrawLabelLineName();
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

        private void DefinyTextPosition()
        {
            if (IsDiagonal)
            {
                if (FromTop)
                {
                    double extra = (MyName.Text.Length > 26) ? 13 : -5;
                    this.TextPanel.Margin = new Thickness(Points[1].X - diagonalShiftLabelX + extra + StrokeThickness, Points[1].Y - diagonalShiftLabelY + 40, 0, 0);
                }
                else
                {
                    double extra = (MyName.Text.Length > 26) ? -0 : -0;
                    this.TextPanel.Margin = new Thickness(Points[1].X - diagonalShiftLabelX + extra + StrokeThickness, Points[1].Y + diagonalShiftLabelY - 55, 0, 0);
                }
            }
            else
            {
                this.TextPanel.Margin = new Thickness(Points[1].X - horizontalShiftLabelX, Points[1].Y + StrokeThickness, 0, 0);
            }
        }

        private void DrawLabelLineName()
        {
            this.MyName.Line = this;
        }
               
        protected override Geometry DefiningGeometry
        {            
            get
            {
                MyName.MaxWidth = 180;
                TextPanel.RenderTransformOrigin = new Point(0, 0);

                MyName.MaxHeight = 40;
                if (IsRoot || IsMoving)
                {
                    TextPanel.Visibility = Visibility.Collapsed;
                }
                if (Children.Count <= 0 || IsRoot || IsCM)
                {
                    Expand.Visibility = Visibility.Collapsed;
                }
                if (Collapsed)
                {
                    Expand.Source = new BitmapImage(new Uri(General.CONTRAIDO));
                    foreach (var item in TreeOperation.GetOnlyMyChildrenWithCM(this))
                    {
                        item.Hidden = true;
                    }
                    foreach (var item in Segments)
                    {
                        item.Visibility = Visibility.Hidden;
                    }
                }
                else
                {
                    Expand.Source = new BitmapImage(new Uri(General.EXPANDIDO));
                }
                if (IsActivated)
                {
                    MyName.Text = ShortName;
                }
                else
                {
                    MyName.Text = "(Disabled)" + ShortName;
                    
                }
                if (ShortName == string.Empty)
                {
                    TextPanel.Visibility = Visibility.Collapsed;
                }
                DefinyTextPosition();
                TextPanel.BorderBrush = Stroke;
                if (!IsCM)
                {
                    TextPanel.Background = new SolidColorBrush(Color.FromArgb(50, ((SolidColorBrush)Stroke).Color.R, ((SolidColorBrush)Stroke).Color.G, ((SolidColorBrush)Stroke).Color.B));
                }
                if (Hidden)
                {
                    TextPanel.Visibility = Visibility.Collapsed;
                    foreach (var itemseg in Segments)
                    {
                        itemseg.Visibility = Visibility.Collapsed;
                    }
                    Visibility = Visibility.Collapsed;
                }
                if (IsCM)
                {
                    foreach (var itemseg in Segments)
                    {
                        itemseg.Visibility = Visibility.Collapsed;
                    }
                }
                TextPanel.IDRisk = ID;
                TextPanel.Risk = ShortName;
                return base.DefiningGeometry;
            }
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

            decimal skoda = value / 10;

            if (cost <= skoda)
            {
                this.StrokeThickness = 2;
            }
            else
            {
                if (cost > skoda && cost <= 4 * skoda)
                {
                    //this.StrokeThickness = 2;
                    this.StrokeThickness = 4;
                }
                else
                {
                    if (cost > 4 * skoda && cost <= 6 * skoda)
                    {
                        this.StrokeThickness = 6;
                    }
                    else
                    {
                        if (cost > 6 * skoda && cost <= 8 * skoda)
                        {
                            this.StrokeThickness = 8;
                            //this.StrokeThickness = 6;
                        }
                        else
                        {
                            if (cost > 8 * skoda && cost <= 10 * skoda)
                            {
                                this.StrokeThickness = 10;
                                //this.StrokeThickness = 8;

                            }
                            else
                            {
                                if (cost > 10 * skoda && cost <= 12 * skoda)
                                {
                                    this.StrokeThickness = 12;
                                    //this.StrokeThickness = 8;
                                }
                                else
                                {
                                    if (cost > 12 * skoda && cost <= 14 * skoda)
                                    {
                                        this.StrokeThickness = 14;
                                        //this.StrokeThickness = 8;
                                    }
                                    else
                                    {
                                        if (cost > 14 * skoda && cost <= 16 * skoda)
                                        {
                                            this.StrokeThickness = 16;
                                            //this.StrokeThickness = 8;
                                        }
                                        else
                                        {
                                            if (cost > 16 * skoda && cost <= 18 * skoda)
                                            {
                                                this.StrokeThickness = 18;
                                                //this.StrokeThickness = 8;
                                            }
                                            else
                                            {
                                                this.StrokeThickness = General.MaxThickness;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public double XTremee()
        {
            double xtremo = XTreme;
            if (!(Collapsed))
            {
                xtremo = TreeOperation.GetMeAndMyChildrenWithCM(this).OrderBy(x => x.Points[0].X).First().Points[0].X;
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
                    ytremo = TreeOperation.GetMeAndMyChildrenWithCM(this).OrderBy(x => x.Points[0].Y).First().Points[0].Y;
                }
                else
                {
                    ytremo = TreeOperation.GetMeAndMyChildrenWithCM(this).OrderBy(x => x.Points[0].Y).First().Points[0].Y;
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
                ytremo = TreeOperation.GetMeAndMyChildrenWithCM(this).OrderBy(x => x.Points[0].Y).First().Points[0].Y;
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
            TextPanel.Visibility = aVisibility;
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
                    xtremo = TreeOperation.GetMeAndMyChildrenWithCM(this).OrderBy(x => x.MyMinXPoint().X).First().MyMinXPoint();
                }
                else
                {
                    //RiskPolyLine rtremo = TreeOperation.GetMeAndAllChildsWithCM(this).Where(x => (x.Points[0].Y > altura)).OrderBy(x => x.Points[0].X).FirstOrDefault();
                    RiskPolyLine rtremo = TreeOperation.GetMeAndMyChildrenWithCM(this).Where(x => (x.MyMinXPoint().Y > altura)).OrderBy(x => x.MyMinXPoint().X).FirstOrDefault();
                    //xtremo = (rtremo != null) ? rtremo.Points[0] : xtremo;
                    xtremo = (rtremo != null) ? rtremo.MyMinXPoint() : xtremo;
                }
            }
            return xtremo;
        }

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
            SegmentClear();
            if (Children != null && Children.Any())
            {
                foreach (RiskPolyLine line in Children)
                {
                    line.AllSegmentClear();
                }
            }
        }

        public void SegmentClear()
        {
            if (Segments != null && Segments.Any())
            {
                foreach (var segment in Segments)
                {
                    MyContainer.Children.Remove(segment);
                }
                Segments.Clear();
            }
        }

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

        public void OnThicknessChange()
        {
            if (!IsCM && !IsRoot && !Hidden)
            {
                if (!OriginalStartPointHasValue)
                {
                    OriginalStartPoint = Points[1];
                    OriginalStartPointHasValue = true;
                }

                if (IsDiagonal)
                {
                    double StrokeThicknessToApply = (GetStrokeThicknessInPosition(Father.Children.IndexOf(this)) / 2 + StrokeThickness + StrokeThickness / 6);

                    StrokeThicknessToApply = FromTop ? StrokeThicknessToApply : -StrokeThicknessToApply;

                    double m = (OriginalStartPoint.Y - Points[0].Y) / (OriginalStartPoint.X - Points[0].X);
                    double b = OriginalStartPoint.Y - m * OriginalStartPoint.X;
                    //double X = OriginalStartPoint.X - StrokeThicknessToApply;
                    //double Y = m * X + b;
                    double Y = OriginalStartPoint.Y - StrokeThicknessToApply;
                    double X = (Y - b) / m;

                    Points[1] = new Point(X, Y);
                }
                else
                {
                    //double StrokeThicknessToApply = Father.StrokeThickness / 2 + StrokeThickness + StrokeThickness / 4;
                    double StrokeThicknessToApply = GetStrokeThicknessInPosition(Father.Children.IndexOf(this)) / 2 + StrokeThickness + StrokeThickness / 4;

                    double X = OriginalStartPoint.X - StrokeThicknessToApply;
                    StartDrawPoint = new Point(X, OriginalStartPoint.Y);
                    Points[1] = StartDrawPoint;
                }
                foreach (var polyLine in Children)
                {
                    if (!polyLine.IsCM)
                    {
                        polyLine.OnThicknessChange();
                    }
                }
            }
        }

        public double GetStrokeThicknessInPosition(int pos)
        {
            double visualParentStrokeThickness;
            if (pos > 0 && Father.Segments.Count > 0 && Father.Segments.Count > pos)
            {
                visualParentStrokeThickness = Father.Segments.ElementAt(pos - 1).StrokeThickness;
            }
            else
            {
                visualParentStrokeThickness = Father.StrokeThickness;
            }
            return visualParentStrokeThickness;
        }
    }
}
