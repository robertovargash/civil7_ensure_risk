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
            decimal scale = (max - min) / General.MaxThickness;

            int scalaVariable = 0;
            while (cost >= (scale * scalaVariable) + min && scalaVariable <= General.MaxThickness)
            {
                if (scalaVariable < 2)
                {
                    this.StrokeThickness = 2;
                }
                else
                {
                    this.StrokeThickness = scalaVariable;
                }
                scalaVariable += 2;
            }
        }
    }
}
