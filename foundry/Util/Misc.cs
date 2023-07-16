using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundry.Util
{
	public static class Misc
	{
		public static float Clamp(this float f, float min, float max)
		{
			if (min > max) return f;
			if (f < min) return min;
			if (f > max) return max;
			return f;
		}

		public static float ToFloat16(byte HO, byte LO)
		{
			var intVal = BitConverter.ToInt32(new byte[] { HO, LO, 0, 0 }, 0);

			int mant = intVal & 0x03ff;
			int exp = intVal & 0x7c00;
			if (exp == 0x7c00) exp = 0x3fc00;
			else if (exp != 0)
			{
				exp += 0x1c000;
				if (mant == 0 && exp > 0x1c400)
					return BitConverter.ToSingle(BitConverter.GetBytes((intVal & 0x8000) << 16 | exp << 13 | 0x3ff), 0);
			}
			else if (mant != 0)
			{
				exp = 0x1c400;
				do
				{
					mant <<= 1;
					exp -= 0x400;
				} while ((mant & 0x400) == 0);
				mant &= 0x3ff;
			}
			return BitConverter.ToSingle(BitConverter.GetBytes((intVal & 0x8000) << 16 | (exp | mant) << 13), 0);
		}

		public static System.Drawing.Drawing2D.Matrix Inverted(this System.Drawing.Drawing2D.Matrix m)
		{
			System.Drawing.Drawing2D.Matrix ret = m.Clone();
			ret.Invert();
			return ret;
        }
	}
}
