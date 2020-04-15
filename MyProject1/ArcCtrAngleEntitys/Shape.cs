using MyProject1.FileEntitys;
using System;
using System.Collections.Generic;
using System.Linq;
/*
*2020/03/25
*author:CTw
*
*
*
*/


/************************************************************************/
/* 
 *
 *这里写了这么多类的原因是解耦！解耦！解耦！
 * 高度抽象是因为可以代码复用等等
 * 
 * 总之~设计模式很重要
 * 
 * 
 * 其实我这个设计还不够好，当图形种类多了后，类就会越来越多0
 * 
 * 
 * 其实可以注意到一个细节：对于一个图形来说，什么K啊，b啊，圆的中心都不重要
 * 什么最重要，解析式啊！方程啊！最重要了。
 * 那么我们可以设计这样一个类，举个例子
 * 
 * myline=FactoryShape("Line").Set("y=2*x+3")
 * myline.getx(1)
 * output:y===5
 * myline.get(5)
 * output:x==1
 *
 * 
 * myCircle=FactoryShape("Circle").set("(x-2)^2+(y-2)^2==4")
 * myCircle.getx(5)
 * output:x==2-sqrt(5)*i//x==2+sqrt(5)*i
 * 
 * 
 * 
 * 更一般的我们甚至不需要什么Line,Circle
 * 直接
 * 
 * shape=Factory.set(y=x^x)
 * shape.gety(2)
 * output:y==4
 * 
 * 
 * 
 * 这就是所谓的符号计算的做法了，难度系数堪比编译器，大量引用了正则表达式等等
 * 投机取巧的做法是使用mathematica做C#脚本（伪后端）
 * 
 * 
 * 这里精力有限就没这么设计了
 * /
/************************************************************************/



namespace MyProject1.ArcCtrAngleEntitys
{


    /*
     * 
     * 
     * 
     *这个接口是用了decimal类型
     * Why? 因为double 损失精度16位有点损失精度，而decimal128位不损失精度
     * 
     * 这个接口定义一个list而不是普通的decimal
     * Why?
     * 因为对于方程来说，对于一个x可能有多个y与之匹配，这点与函数截然不同，注意！
     * 
     */



    public interface Shape
    {
        List<decimal> Getx(decimal y);
        List<decimal> Gety(decimal x);
    }

    //抽象类   
    /*
     * 
     * 
     *这个类是一个纯虚类，可以这么理解Range表示一个范围，凡是线段，弧线，不规则图形，在平面坐标轴上都是有范围的，
     * 如果想要表示一个图形在坐标轴上，那么必定将这个图像拆封成若干个线段和其他的弧线等等（PS：傅里叶展开不算，理论上傅里叶级数无限展开可以绘制出
     * 任何图形，这边太bt了）
     * 
     * 
     * 
     * 
     * 
     * 
     */

    public abstract class Range : IComparable, IEquatable<Range>
    {
        private Point startPoint;
        private Point endPoint;

        public Range(Range range)
        {
            startPoint = range.startPoint;
            endPoint = range.endPoint;
        }

        public Range(Point startpoint, Point endpoint)
        {
            startPoint = startpoint;
            endPoint = endpoint;
        }
        public Point StartPoint { get => startPoint; set => startPoint = value; }
        public Point EndPoint { get => endPoint; set => endPoint = value; }

        public virtual int CompareTo(object obj)
        {
            return ((IComparable)startPoint).CompareTo(obj);
        }

        public virtual bool Equals(Range other)
        {
            return startPoint.Equals(other.StartPoint) && EndPoint.Equals(other.startPoint);


        }
    }
    public struct Point : IComparable, IEquatable<Point>
    {
        private decimal x;
        private decimal y;

        public Point((decimal, decimal) point)
        {
            x = point.Item1;
            y = point.Item2;
        }


        public bool MyCompareTo(Point obj)
        {
            decimal x1, y1;
            decimal x2, y2;
            x1 = X;
            x2 = obj.X;
            y1 = Y;
            y2 = obj.Y;
            if (x1 <= x2 && y1 <= y2)
            {
                return true;
            }

            return false;

        }


        public Point(decimal x, decimal y)
        {
            this.x = x;
            this.y = y;
        }

        public Point(Point point)
        {
            x = point.x;
            y = point.y;
        }

        public decimal X { get => x; set => x = value; }
        public decimal Y { get => y; set => y = value; }

        public int CompareTo(object obj)
        {
            if (!(obj is Point))
            {
                throw new Exception("error");
            }

            Point any = (Point)obj;
            return x.CompareTo(any.x);
        }

        public override string ToString()
        {
            return string.Format("x={0},y={1}", x, y);
        }

        public bool Equals(Point other)
        {
            return x == other.x && y == other.y;
        }
    }
    /*
     *
     *这个Line类仅仅声明了一条直线（不是线段<====>线段有两个点即起点和终点） 
     * 
     * 
     * Why?为什么要这样做
     * 
     * 
     * 降低耦合性，增强代码复用，因为直线可以成为线段，也可以成为其他的，比如：射线，向量等等
     * 
     * Why？为啥要实现Shape接口呢
     * 
     * 对于所有的函数来说，在平面上（x,y）的坐标都是需要知道了
     * 我们可以这样说，如果这个东东是一个函数的话，那他必定要知道(x,y)的下标，不然要这个函数干嘛
     * 
     * 
     * 
     * Why?为啥要实现一个IEqutable<Line>而不实现IComparable呢？
     * 
     * 其实我是想过实现IComarable的，但仔细思考了下，对两条直线进行比较大小是没有任何意义的
     * 但比较这两条直线的解析式是有意义的，如果解析式是一样的话，表示这两个直线是同一条直线，即共线
     * 
     * 
     * 
     * 建议学弟们可以了解下matlab,Mathematica等等数学软件，不需要太了解，能当作超级计算器使用就行了
     * 本来我是想做一个mahematica的脚本，内嵌到C#中，使得C#可以进行符号计算，但工程量有点大，代码可能
     * 耦合性较高，（PS：主要是没做好架构）后面会给一个Mathematica的例子，C#调用这个软件，命令行调用
     * WSTP做法，没有干，有兴趣试试
     * 
     */


    public class Line : Shape, IEquatable<Line>
    {
        private decimal k;
        private decimal b;
        private decimal x1;
        public decimal K => k;
        public decimal B => b;
        public string Val => ToString();


        public bool IsVertical()
        {
            if (k == decimal.MaxValue)
            {
                return true;
            }

            return false;
        }


        public Line(Line myline)
        {
            k = myline.k;
            b = myline.b;
            x1 = decimal.MaxValue;
        }


        //一种方法是直接y=k*x+b（debug）用
        public Line(decimal k, decimal b)
        {
            this.k = k;
            this.b = b;
            x1 = decimal.MaxValue;
        }
        //value_tuple方法
        //k=(y1-y2)/(x1-x2);
        //b=y1-k*x1
        public Line((Point, Point) point)
        {
            decimal y1, y2, x1, x2;
            y1 = point.Item1.Y;
            y2 = point.Item2.Y;
            x1 = point.Item1.X;
            x2 = point.Item2.X;
            k = (y1 - y2) / (x1 - x2);
            b = y1 - k * x1;
            this.x1 = decimal.MaxValue;
            if (x1 - x2 == 0)
            {
                this.x1 = x1;
                b = decimal.MaxValue;
                k = decimal.MaxValue;
            }

        }


        //两点式子,跟上面一个，多一个接口罢了
        public Line(Point startpoint, Point endpoint)
        {
            decimal y1, y2, x1, x2;
            y1 = startpoint.Y;
            y2 = endpoint.Y;
            x1 = startpoint.X;
            x2 = endpoint.X;
            k = (y1 - y2) / (x1 - x2);
            b = y1 - k * x1;
            this.x1 = decimal.MaxValue;
            if (x1 - x2 == 0)
            {
                this.x1 = x1;
                b = decimal.MaxValue;
                k = decimal.MaxValue;
            }

        }

        //重写string方法，他显示的是这个直线的解析式子
        //这里有正负号的一些问题，统一输出y=k*x+b的形式
        public override string ToString()
        {
            string AnalyticFormula;
            AnalyticFormula = string.Format("y={0}*x+{1}", k, b);
            if (x1 != decimal.MaxValue)
            {
                AnalyticFormula = string.Format("x={0}", x1);
            }

            return AnalyticFormula;
        }
        //y=k*x+b的反函数
        //x=(-b+y)/k Mathematica算出的！，不是手算的！

        public List<decimal> Getx(decimal y)
        {
            decimal x = 0;
            x = (-b + y) / k;
            return new List<decimal> { x };
        }
        //y1=k*x+b...没啥好解释的
        public List<decimal> Gety(decimal x)
        {
            decimal y = 0;
            y = k * x + b;
            return new List<decimal> { y };
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Line);
        }

        public bool Equals(Line other)
        {
            return other != null &&
                   k == other.k &&
                   b == other.b &&
                   K == other.K &&
                   B == other.B;

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(k, b, K, B, Val);
        }

        public static bool operator ==(Line line1, Line line2)
        {
            return EqualityComparer<Line>.Default.Equals(line1, line2);
        }

        public static bool operator !=(Line line1, Line line2)
        {
            return !(line1 == line2);
        }
    }
    public class Circle : Shape, IEquatable<Circle>
    {
        private decimal centerX;
        private decimal centerY;
        private decimal r;
        public decimal CenterX => centerX;
        public decimal CenterY => centerY;
        public decimal R => r;
        public string Val => ToString();

        public Circle(Circle circle)
        {

            centerX = circle.centerX;
            centerY = circle.centerY;
            r = circle.r;
        }

        public Circle(Point center, decimal r)
        {
            centerX = center.X;
            centerY = center.Y;
            this.r = r;

        }
        public Circle(Point center, Point x)
        {
            centerX = center.X;
            centerY = center.Y;
            decimal x1 = CenterX - x.X;
            decimal y1 = CenterY - x.Y;

            r = (decimal)Math.Sqrt(decimal.ToDouble(x1 * x1 + y1 * y1));
        }
        public Circle(decimal a, decimal b, decimal r)
        {
            centerX = a;
            centerY = b;
            this.r = r;
        }

        public Circle((Point, Point) points)
        {
            centerX = points.Item1.X;
            centerY = points.Item1.Y;
            decimal x1 = CenterX - points.Item2.X;
            decimal y1 = CenterY - points.Item2.Y;
            r = (decimal)Math.Sqrt(decimal.ToDouble(x1 * x1 + y1 * y1));

        }
        public bool Equals(Circle other)
        {
            return centerX == other.centerX
                 && centerY == other.centerY
                 && r == other.r;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Circle);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //Mathematica计算出来的，万岁！
        public List<decimal> Getx(decimal y)
        {
            decimal a, b;
            a = centerX;
            b = centerY;
            decimal x1, x2;
            decimal tmp =
            x1 = a - (decimal)Math.Sqrt(decimal.ToDouble(-b * b + r * r + 2 * b * y - y * y));
            x2 = a + (decimal)Math.Sqrt(decimal.ToDouble(-b * b + r * r + 2 * b * y - y * y));
            return new List<decimal> { x1, x2 };
        }
        public List<decimal> Gety(decimal x)
        {
            decimal a, b;
            a = centerX;
            b = centerY;
            decimal y1, y2;
            y1 = b - (decimal)Math.Sqrt(decimal.ToDouble(-a * a + r * r + 2 * a * x - x * x));
            y2 = b + (decimal)Math.Sqrt(decimal.ToDouble(-a * a + r * r + 2 * a * x - x * x));
            return new List<decimal> { y1, y2 };
        }
        public override string ToString()
        {
            string val = string.Format("(x-{0})^2+(y-{1})^2=={2}^2", centerX, CenterY, r);
            return val;
        }
    }







    public class ShapeSegment<T> : Range, Shape where T : Shape
    {

        public Shape shape;
        // public ShapeSegment(Point start, Point end, T shape) : base(start, end)
        // {
        //     this.shape = shape;
        // }

        public ShapeSegment(ShapeSegment<T> shapeSegment) : base(shapeSegment.StartPoint, shapeSegment.EndPoint)
        {
            this.shape = shapeSegment.shape;
        }


        public ShapeSegment(Point start, Point end, params object[] args) : base(start, end)
        {
            shape = (Shape)Activator.CreateInstance(typeof(T), args);
        }

        public List<decimal> Getx(decimal y)
        {
            return shape.Getx(y);
        }

        public List<decimal> Gety(decimal x)
        {
            return shape.Gety(x);
        }
    }

    /*
     *这里采用的是工厂方式生产点，这里其实可以再进一步的抽象的，我实现的是简单工厂
     *可以将工厂在抽象成工厂组
     * 
     * 
     * 
     *
     * 但在这个项目中没必要这么复杂，有兴趣的话你们可以试试
     * 
     */

    public static class FactoryShape<T> where T : Range
    {
        private static T shape;
        static FactoryShape()
        {

        }

        public static T Create(params object[] args)
        {

            shape = (T)Activator.CreateInstance(typeof(T), args);
            return shape;
        }
    }





    /*
     * 
     * 
     * 
     * 
     *其实前面这么多类就是位下面这个类做准备的，下面这个类看起来很短小，其实干货满满 
     * 
     * 
     * 
     * 首先初始化使用了一个parms的object，说明他是不定参数的不定类型的
     * 其次调用了工厂类生产shape，（PS：可以把他理解为委托构造函数）
     * for example:
     * 一般要生产点，我们首先要生产线段或者弧线，如下：
     * LineSegment lineSegment = new LineSegment(new Point(1, 5), new Point(200, 403));
     * CircleSegment circleSegment = new CircleSegment(new Point(0, 0), new Point(0, 5), new Point(5, 0));
     * 
     * 然后我们再通过自己编写的类来依次产生点，比如生产线段的类叫LinePoint 生产弧线的类叫 circlePoint
     * 那么就是
     * 
     * LinePoint=new LinePoint(step,lineSegment)
     * circlePoint=new circlePoint(step,Circlement）
     * .....
     * 那么后面有其他的方程呢，比如圆锥曲线等等...那岂不是要写越来越多的类！
     * 
     *
     * 这是两个继承range的类，这样调用我们就要分类讨论，显然不优雅，而通过工厂的设计模式，可以这样写
     * 
     * 
     * 
     * 
     */



    public class ShapePoint<T> where T : Range
    {
        private Shape shape;
        private List<Point> shapePointsX;
        private Point start;
        private Point end;
        private decimal step;
        public ShapePoint(decimal step, params object[] args)
        {
            T myshape = FactoryShape<T>.Create(args);
            start = myshape.StartPoint;
            end = myshape.EndPoint;

            shape = (Shape)myshape;
            this.step = step;
        }


        public ShapePoint<T> GetAlly()
        {
            decimal max = Math.Max(start.Y, End.Y);
            decimal min = Math.Min(start.Y, End.Y);
            shapePointsX = new List<Point>();
            Console.WriteLine("{0},{1}", max, min);
            for (decimal i = start.X; i < end.X; i += step)
            {

                List<decimal> ally = shape.Gety(i);
                shapePointsX = shapePointsX.Concat(ally.Select(y => new Point(i, y))).Where(point => point.Y >= min && point.Y <= max).ToList();
            }
            return this;

        }

        public List<Point> ShapePointsX => shapePointsX;
        public Point Start => start;
        public Point End => end;
        public Shape Shape { get => shape; set => shape = value; }
    }









}
