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
        public Point punto { get; set; }
        public bool terminar { get; set; }

        public PointToMoveRight()
        {
            punto = new Point(0, 0);
            terminar = false;
        }

        public PointToMoveRight(Point punto, bool terminar)
        {
            this.punto = punto;
            this.terminar = terminar;
        }
    }
}
