using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObjParser;


class Program
{
    static void Main(string[] args)
    {
        ObjParser.Obj o = new Obj();
        o.LoadObj("C:\\users\\jaken\\desktop\\arrow.obj");
        string final = "float[] verts = new float[" + o.VertexList.Count() * 3 + "] {\n";
        foreach (var p in o.VertexList)
        {
            final += "    " + p.X + "f, " + p.Y + "f, " + p.Z + "f,\n";
        }
        final += "};";
        Console.Write(final);
        Console.WriteLine();
        final = "int[] inds = new int [" + o.FaceList.Count() * 3 + "] {\n";
        foreach(var f in o.FaceList)
        {
            final += "    " + (f.VertexIndexList[0] - 1) + ", " + (f.VertexIndexList[1] - 1) + ", " + (f.VertexIndexList[2] - 1) + ",\n";
        }
        final += "};";
        Console.Write(final);
        Console.ReadLine();
    }
}
