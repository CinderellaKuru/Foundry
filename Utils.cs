using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using OpenTK;
using Jitter.LinearMath;

namespace SMHEditor
{
    static class Convert
    {
        public static Vector3 ToTKVec3(JVector jVector)
        {
            return new Vector3(
                jVector.X,
                jVector.Y,
                jVector.Z );
        }
        public static JVector ToJVec3(Vector3 tkVector)
        {
            return new JVector(
                tkVector.X,
                tkVector.Y,
                tkVector.Z );
        }
        public static List<float> GetFloats(this List<Vector3> v)
        {
            List<float> f = new List<float>(v.Count);
            foreach (Vector3 vec in v)
            {
                f.Add(vec.X);
                f.Add(vec.Y);
                f.Add(vec.Z);
            }
            return f;
        }
    }

    static class Util
    {
        public static float Distance(JVector a, JVector b)
        {
            return Vector3.Distance(Convert.ToTKVec3(a), Convert.ToTKVec3(b));
        }
        public static bool DrawingPointIsInsideControl(System.Drawing.Point p, System.Windows.Forms.Control c)
        {
            if (p.X < c.PointToScreen(new Point(0, 0)).X ||
                p.X > c.PointToScreen(new Point(0, 0)).X + c.Width ||
                p.Y < c.PointToScreen(new Point(0, 0)).Y ||
                p.Y > c.PointToScreen(new Point(0, 0)).Y + c.Height) return false;
            else return true;
        }
        public static uint CalcAdler32(byte[] barr, int offs, int len)
        {
            const int mod = 65521;
            uint a = 1, b = 0;
            for (int i = offs; i < len + offs; i++)
            {
                byte c = barr[i];
                a = (a + c) % mod;
                b = (b + a) % mod;
            }
            return (b << 16) | a;
        }
    }
}

