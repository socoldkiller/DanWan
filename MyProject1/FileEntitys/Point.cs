using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProject1.FileEntitys
{
    public class Point : IComparable, IEquatable<Point>
    {
        double x;
        double y;

        public Point((double, double) point)
        {
            this.x = point.Item1;
            this.y = point.Item2;
        }


        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Point(Point point)
        {
            this.x = point.x;
            this.y = point.y;
        }

        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }

        public int CompareTo(object obj)
        {
            if (!(obj is Point))
                throw new Exception("error");
            Point any = obj as Point;
            return x.CompareTo(any.x);
        }



        public bool Equals(Point other)
        {
            return this.x == other.x && this.y == other.y;
        }
    }
}
