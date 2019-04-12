using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer
{
    class DragHelper
    {
        public bool isDrag { get; set; }
        public bool isSelected { get; set; }

        public Point startPositionPoint = new Point();
        public Point currentPositionPoint = new Point();

        public int GetDistance()
        {
            return (int)Math.Sqrt(Math.Pow(startPositionPoint.X - currentPositionPoint.X, 2) + Math.Pow(startPositionPoint.Y - currentPositionPoint.Y, 2));
        }
    }
}