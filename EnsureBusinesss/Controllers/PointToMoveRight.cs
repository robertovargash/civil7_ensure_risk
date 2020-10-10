using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EnsureBusinesss
{
    class PointToMoveRight
    {
        public Point Punto { get; set; }
        public bool Terminar { get; set; }

        public PointToMoveRight()
        {
            Punto = new Point(0, 0);
            Terminar = false;
        }

        public PointToMoveRight(Point punto, bool terminar)
        {
            this.Punto = punto;
            this.Terminar = terminar;
        }
    }
}
