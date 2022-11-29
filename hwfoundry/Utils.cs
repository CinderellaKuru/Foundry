using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;
using OpenTK;

namespace Foundry
{
    static class Convert
    {
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

