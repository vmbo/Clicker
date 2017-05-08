using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clicker
{
    public class Points
    {
        public Dictionary<string, Point[]> Curves;
        public Dictionary<string, Point[]> Poligons;

        string curvesFileName;

        public Points(string curvesFileName)
        {
            this.curvesFileName = curvesFileName;
            Curves = LoadCurves(curvesFileName);
            MakePoligines();
        }

        #region Curves

        public Dictionary<string, Point[]> LoadCurves(string pointsFileName)
        {
            try
            {
                return File.ReadAllLines(pointsFileName)
                    .Select(line => line.Split(':'))
                    .Where(ss => ss.Length == 2)
                    .Select(ss => new { key = ss[0].Trim(), val = HorTextToPointArray(ss[1]) })
                    .ToDictionary(v => v.key, v => v.val);
            }
            catch
            {
                return new Dictionary<string, Point[]>();
            }
        }

        internal void SaveCurve(string key, string text)
        {

            // save to dict
            Curves[key] = VerTextToPointArray(text);
            // save to file
            text = key + " : " + FlatText(text) + "\r\n";
            File.AppendAllText(curvesFileName, text);
        }

        internal Point[] GetScaledCurve(string key, int m)
        {
            return Scale(Curves[key], m);
        }

        
        #endregion Curves

        #region Points

        public static Point[] VerTextToPointArray(string text, int m = 1)
        {
            return HorTextToPointArray(FlatText(text), m);
        }

        // text is string like "22,22,33,33,44,44,"
        public static Point[] HorTextToPointArray(string text, int m = 1)
        {
            var ss = text.Split(',');
            var v = new List<Point>();

            for (int i = 0; i < ss.Length - 1; i += 2)
            {
                int x = Convert.ToInt32(ss[i]);
                int y = Convert.ToInt32(ss[i + 1]);
                v.Add(new Point(x * m, y * m));
            }
            return v.ToArray();
        }

        public static string FlatText(string text)
        {
            return new String(text.Where(c => !Char.IsWhiteSpace(c)).ToArray());
        }

        #endregion Points

        #region Poligones

        public void MakePoligines()
        {
            Poligons = new Dictionary<string, Point[]>();
            Poligons["Lviv"] = Join(1, -25, -24, -23, -22, -21);
            Poligons["Lutsk"] = Join(2, -26, 25);
            Poligons["Rivno"] = Join(3, -29, -28, -27, 24, 26 );
            Poligons["Zhytomyr"] = Join(4, -32, -31, -30, 29);
            Poligons["Kyiv"] = Join(5, 33, 34, 35, 36, 32);
        }

        public Point[] Join(params int[] keys)
        {
            List<Point> res = new List<Point>();
            foreach (int k in keys)
            {
                string key = Math.Abs(k).ToString();
                Point[] ps = new Point[Curves[key].Length];
                Array.Copy(Curves[key], ps, ps.Length);
                if (k < 0)
                    Array.Reverse(ps);
                res.AddRange(ps);
            }
            return res.ToArray();
        }

        static Point[] Scale (Point[] ps, int m) {
            return ps.Select(d => new Point { X = d.X * m, Y = d.Y * m }).ToArray();
        }

        public static bool IsInside(Point[] ps, Point p)
        {
            bool result = false;
            int j = ps.Count() - 1;
            for (int i = 0; i < ps.Count(); i++)
            {
                if (ps[i].Y < p.Y && ps[j].Y >= p.Y || ps[j].Y < p.Y && ps[i].Y >= p.Y)
                {
                    if (ps[i].X + (p.Y - ps[i].Y) / (ps[j].Y - ps[i].Y) * (ps[j].X - ps[i].X) < p.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }

        public string GetPoligonePointInto(Point p)
        {
            foreach (var pair in Poligons)
            {
                if (IsInside(pair.Value, p))
                    return pair.Key;
            }
            return null;
        }

        internal Point[] GetScaledPoligone(string key, int m)
        {
            return Scale(Poligons[key], m);
        }


        #endregion Poligones

    }
}
