using Petzold.Media2D;
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

        //protected override void OnMouseEnter(MouseEventArgs e)
        //{
        //    base.OnMouseEnter(e);
        //    if (Father != null)
        //    {
        //        Father.OnMouseEnterStrokeThickness();
        //    }
        //}
        //protected override void OnMouseLeave(MouseEventArgs e)
        //{
        //    base.OnMouseLeave(e);
        //    if (Father != null)
        //    {
        //        Father.OnMouseLeaveStrokeThickness();
        //    }
        //}
    }
}
