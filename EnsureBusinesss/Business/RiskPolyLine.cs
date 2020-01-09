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
        public bool IsActivated { get; set; }
        public LineGroup Group { get; set; }
        public int IdRiskFather { get; set; }
        public string ShortName { get; set; }
        public bool FromTop { get; set; }
        public bool IsRoot { get; set; }
        public int Size { get; set; }
        public decimal Value { get; set; }
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

        //public static RoutedEvent DobleClick;
        public RiskPolyLine()
        {

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
                    this.ElStackPannel.Margin = new Thickness(StartDrawPoint.X - RiskPolyLine.diagonalShiftLabelX + extra, StartDrawPoint.Y - RiskPolyLine.diagonalShiftLabelY, 0, 0);
                    RotateTransform rotateTransform1 = new RotateTransform(AngleX);
                    this.ElStackPannel.RenderTransform = rotateTransform1;
                }
                else
                {
                    double extra = (LabelName.Length > 26) ? -19 : -19;
                    this.ElStackPannel.Margin = new Thickness(StartDrawPoint.X - RiskPolyLine.diagonalShiftLabelX + extra, StartDrawPoint.Y + RiskPolyLine.diagonalShiftLabelY - 10, 0, 0);
                    RotateTransform rotateTransform1 = new RotateTransform(-AngleX);
                    this.ElStackPannel.RenderTransform = rotateTransform1;
                }
            }
            else
            {
                this.ElStackPannel.Margin = new Thickness(StartDrawPoint.X - RiskPolyLine.horizontalShiftLabelX, StartDrawPoint.Y + 1, 0, 0);
            }

            if (IsCM)
            {
                Expand.Visibility = Visibility.Collapsed;
            }
            this.MyName.Line = this;
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
                    xtremo = TreeOperation.GetMeAndAllChildsWithCM(this).OrderBy(x => x.Points[0].X).First().Points[0];
                }
                else
                {
                    RiskPolyLine rtremo = TreeOperation.GetMeAndAllChildsWithCM(this).Where(x => (x.Points[0].Y > altura)).OrderBy(x => x.Points[0].X).FirstOrDefault();
                    xtremo = (rtremo != null) ? rtremo.Points[0] : xtremo;
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
        public int HorizontalMaxCountChildren()
        {
            int result = 0;
            if (!(Collapsed))
            {
                result = TreeOperation.GetMeAndAllChildsWithCM(this).Where(x => (x.IsDiagonal == false)).Count();
            }
            return result;
        }
        public int VerticalMaxCountChildren()
        {
            int result = 0;
            if (!(Collapsed))
            {
                result = TreeOperation.GetMeAndAllChildsWithCM(this).Where(x => (x.IsDiagonal == true)).Count();
            }
            return result;
        }

        //public Point HorizontalMaxXTremee(Double Y)
        //{
        //    Point xtremo = new Point(XTreme, YxTreme);
        //    if (!(Collapsed))
        //    {
        //        xtremo = TreeOperation.GetMeAndAllChildsWithCM(this).Where(x => (x.IsDiagonal == false) && (x.Points[0].Y<Y)).OrderBy(x => x.Points[0].X).Last().Points[0];
        //    }
        //    return xtremo;
        //}
    }
}
