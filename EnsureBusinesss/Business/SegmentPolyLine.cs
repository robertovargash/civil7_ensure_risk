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
        private RiskPolyLine Father;
        private Point _startDrawPoint;
        private Point _endDrawPoint;
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
            Stroke = new SolidColorBrush(Colors.Orange);

            StrokeThickness = father.StrokeThickness;
            Points.Add(new Point());
            Points.Add(new Point());
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (Father != null && !Father.IsRoot)
            {
                Father.OnMouseEnterStrokeThickness();
            }
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (Father != null && !Father.IsRoot)
            {
                Father.OnMouseLeaveStrokeThickness();
            }
        }
    }
}
