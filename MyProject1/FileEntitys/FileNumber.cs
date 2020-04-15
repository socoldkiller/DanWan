using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/************************************************************************/
/* 
 * //  [3/23/2020 CTw]
 * //重构代码，大量使用linq，简化代码，降低耦合性
 * 
 *
 *  
 */
/************************************************************************/


namespace MyProject1.FileEntitys
{
    //处理从文件中的数字并读出来
    public class FileNumber
    {
        //  private ValueTuple<Point, Point> line;
        // private ValueTuple<Point, Point, Point> circle;
        private string path;
        //()是ValueTuple类型
        private List<(Point, Point)> linesPoint;
        private List<(Point, Point, Point)> circlesPoint;
        private string text;

        //这三个只能看看，不能写
        public List<(Point, Point)> LinesPoint { get => linesPoint;  }
        public List<(Point, Point, Point)> CirclesPoint { get => circlesPoint;}
        public string Path { get => path;}

        //debug 这个Text不应该暴露给用户
        //public string Text { get => text; set => text = value; }

        //指定属性,first代表直线起点，last代表直线终点
        private static (Point first, Point last) line(Point first, Point last)
        {
            return (first, last);
        }

        //指定属性,first代表圆的起点，last代表圆的终点，center代表圆的中心
        private static (Point first, Point last, Point center) circle(Point first, Point last, Point center)
        {
            return (first, last, center);
        }

        public FileNumber(string path)
        {
            this.path = path;
            text = File.ReadAllText(path);
            GetNumber();
        }

        /*
         *lines读取的字符串里所有Line的地方
         *circles读取ArcCtrangle的地方下面的3行（包括自己）debug方便 
         * for example:字符串->
         * <-------------------->
         * fsadfasff
         * JDSF
         * ArcCtrAngle(
         *   1.223554,21251.2254
         *   623131.25,5151.541
         *   2515212.254,1512.444
         * )
         * fsadfas
         * 
         * <------------------->
         * result:
         * 
         * ArcCtrAngle(
         *   1.223554,21251.2254
         *   623131.25,5151.541
         *   2515212.254,1512.444   
         * 
         * 
         * 
         * 之所以都是array是因为后面读取的下标的时间复杂度为O(n),如果是list(链表)那么读取下标的时间复杂度为O(n^2)
         * 正则表达式的含义是读取小数和整数（非负）
         * 
         * concat连接两个数组，之所以要连接是因为可以用一个循环来同时处理，然后求出两个数组的长度，下面那个循环做下特判就可以了
         * 
         * 
         * 特别注意：每次添加完数据后都要清空临时链表，不然就会导致前面的数据加入到后面的数组中，发生错误
         * 
         * 
         */
        private void GetNumber()
        {
           
            linesPoint = new List<(Point, Point)>();
            circlesPoint = new List<(Point, Point, Point)>();
            string[] tmp = text.Split('\n').ToArray();
            string[] lines = tmp.Where(t => t.IndexOf("line") != -1).ToArray();
            string[] circles = tmp.Select((t, index) => new { value = t, index = index })
                                  .Where(p => tmp[p.index - 0].IndexOf("Arc") != -1 ||
                                              tmp[p.index - 1].IndexOf("Arc") != -1 ||
                                              tmp[p.index - 2].IndexOf("Arc") != -1 ||
                                              tmp[p.index - 3].IndexOf("Arc") != -1
                                             ).Select(x => x.value).ToArray();
            int linesize = lines.Length;
            int circlesize = circles.Length;

            //line在前面，cirlce在后面
            string[] allArcCtrAngle = lines.Concat(circles).ToArray();
            Regex regex = new Regex(@"([1-9]\d*\.?\d*)|(0\.\d*[1-9])");
            List<double> mytmp = new List<double>();
            for (int i = 0; i < allArcCtrAngle.Length; i++) 
            {
                Match m = regex.Match(allArcCtrAngle[i]);
                while (m.Success)
                {
                    mytmp.Add(double.Parse(m.Groups[0].Value));
                    m = m.NextMatch();
                }
                
                if (mytmp.Count == 4 && i < linesize)
                {
                    linesPoint.Add(line(new Point(mytmp[0], mytmp[1]), new Point(mytmp[2], mytmp[3])));
                    mytmp.Clear();
                }
                else if(mytmp.Count==6)
                {
                    circlesPoint.Add(circle(new Point(mytmp[0], mytmp[1]), new Point(mytmp[2], mytmp[3]), new Point(mytmp[4], mytmp[5])));
                    mytmp.Clear();
                }
            }
        }
      

    }
}
