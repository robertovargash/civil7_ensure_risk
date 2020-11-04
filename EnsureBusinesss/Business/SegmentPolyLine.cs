using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EnsureBusinesss.Business
{
    public class SegmentPolyLine : ArrowPolyline
    {

        private Point _startDrawPoint;
        private Point _endDrawPoint;
        public RiskPolyLine Father { get; set; }
        public decimal Value { get; set; }
        public Point StartDrawPoint
        {
            get => _startDrawPoint;
            set
            {
                _startDrawPoint = value;
                Points[0] = value;
            }
        }
        public Point EndDrawPoint
        {
            get => _endDrawPoint;
            set
            {
                _endDrawPoint = value;
                Points[1] = value;
            }
        }
        public Grid MyContainer { get; set; }
        public SegmentPolyLine(RiskPolyLine father)
        {
            Father = father;
            ArrowEnds = ArrowEnds.None;
            //Stroke = new SolidColorBrush(Colors.Orange);
            Stroke = father.Stroke;
            ContextMenu = father.ContextMenu;

            //StrokeThickness = father.StrokeThickness;
            StrokeThickness = 1;
            Points.Add(new Point());
            Points.Add(new Point());

            Value = 0;
            StrokeStartLineCap = PenLineCap.Round;
        }

        protected override Geometry DefiningGeometry
        {
            get
            {
                if (Father.Hidden)
                {
                    Visibility = Visibility.Collapsed;
                }
                return base.DefiningGeometry;
            }
        }

        /// <summary>
        /// Sets Segment PolyLine width
        /// </summary>
        /// <param name="cost"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
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

    }
}
