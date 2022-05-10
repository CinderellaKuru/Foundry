using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUutilities;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using static SMHEditor.DockingModules.MapEditor.MapEditorScene;

namespace SMHEditor.DockingModules.MapEditor
{
    public class Temp : XYZGrabberTranslatable
    {
        public Transform t = new Transform();
        float[] verts = new float[24] {
    -0.2f, 0f, 0.2f,
    -0.2f, 3f, 0.2f,
    -0.2f, 0f, -0.2f,
    -0.2f, 3f, -0.2f,
    0.2f, 0f, 0.2f,
    0.2f, 3f, 0.2f,
    0.2f, 0f, -0.2f,
    0.2f, 3f, -0.2f,
};
        int[] inds = new int[36] {
    1, 2, 0,
    3, 6, 2,
    7, 4, 6,
    5, 0, 4,
    6, 0, 2,
    3, 5, 7,
    1, 3, 2,
    3, 7, 6,
    7, 5, 4,
    5, 1, 0,
    6, 4, 0,
    3, 1, 5,
};
        int vb, ib;
        public Temp()
        {
            vb = GL.GenBuffer();
            ib = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vb);
            GL.BufferData(BufferTarget.ArrayBuffer, 24 * 4, verts, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 36 * 4, inds, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        public void Draw(int shader)
        {
            GL.UseProgram(shader);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vb);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib);
            int pos = GL.GetAttribLocation(shader, "POSITION");
            GL.VertexAttribPointer(pos, 3, VertexAttribPointerType.Float, false, 12, 0);
            GL.EnableVertexAttribArray(pos);
            
            GL.DrawElements(BeginMode.Triangles, 36, DrawElementsType.UnsignedInt, 0);
        }
        public BEPUutilities.Vector3 GetGrabberPos()
        {
            return t.position;
        }
        public void Move(float x, float y, float z)
        {
            t.position += new BEPUutilities.Vector3(x, y, z);
        }
    }
}

//{
//    public class TerrainModifierMesh : XYZGrabberTranslatable
//    {
//        public static float PICK_RANGE = 1f;
//        protected struct Edge
//        {
//            public Edge(int i0, int i1)
//            {
//                index0 = i0;
//                index1 = i1;
//            }
//            public int index0, index1;
//        }

//        private MapEditorScene owner;
//        private Octree octree;
//        private TriangleMeshShape shape;
//        private RigidBody rb;
//        int vb, ib, edgeib;

//        protected List<Vector3> positions = new List<Vector3>();
//        protected List<TriangleVertexIndices> indices = new List<TriangleVertexIndices>();
//        protected List<Edge> edges = new List<Edge>();
//        protected List<Edge> outerEdges = new List<Edge>();
//        private List<int> selected = new List<int>();

//        public TerrainModifierMesh(MapEditorScene scene)
//        {
//            octree = new Octree(positions, indices);
//            shape = new TriangleMeshShape(octree);
//            rb = new RigidBody(shape);
//            rb.Tag = this;
//            rb.IsStatic = true;
//            rb.AffectedByGravity = false;
//            scene.physScene.AddBody(rb);
//            scene.modifierMeshes.Add(this);

//            owner = scene;

//            vb = GL.GenBuffer();
//            ib = GL.GenBuffer();
//            edgeib = GL.GenBuffer();
//        }
//        public bool SelectNearestPoint(Vector3 p)
//        {
//            int closest = -1;
//            float dist = 999999f;
//            for(int i = 0; i < positions.Count; i++)
//            {
//                if(Util.Distance(positions[i], p) < dist)
//                {
//                    dist = Util.Distance(positions[i], p);
//                    closest = i;
//                }
//            }
//            if (dist < PICK_RANGE)
//            {
//                selected.Clear();
//                selected.Add(closest);
//                return true;
//            }
//            else return false;
//        }
//        public void Move(float x, float y, float z)
//        {
//            foreach(int i in selected)
//            {
//                positions[i] += new Vector3(x, y, z);
//                UpdateBuffers();
//            }
//            octree.SetTriangles(positions, indices);
//            octree.BuildOctree();
//            shape = new TriangleMeshShape(octree);
//            rb.Shape = shape;

//        }
//        public Vector3 GetGrabberPos()
//        {
//            if (selected.Count > 0)
//                return positions[selected[0]];

//            else return new Vector3(0, 0, 0);
//        }

//        protected TriangleVertexIndices AddTriangle(Vector3 p0, Vector3 p1, Vector3 p2)
//        {
//            int i0;
//            if (positions.Contains(p0)) i0 = positions.IndexOf(p0);
//            else
//            {
//                i0 = positions.Count;
//                positions.Add(p0);
//            }
//            int i1;
//            if (positions.Contains(p1)) i1 = positions.IndexOf(p1);
//            else
//            {
//                i1 = positions.Count;
//                positions.Add(p1);
//            }
//            int i2;
//            if (positions.Contains(p2)) i2 = positions.IndexOf(p2);
//            else
//            {
//                i2 = positions.Count;
//                positions.Add(p2);
//            }

//            edges.Add(new Edge(i0, i1));
//            edges.Add(new Edge(i1, i2));
//            edges.Add(new Edge(i2, i0));

//            TriangleVertexIndices t = new TriangleVertexIndices(i0, i1, i2);
//            indices.Add(t);

//            //octree.SetTriangles(positions, indices);
//            octree.BuildOctree();
//            Shape s = new TriangleMeshShape(octree);
//            s.Tag = octree;
//            rb.Shape = s;


//            UpdateBuffers();
//            return t;
//        }

//        private void UpdateBuffers()
//        {
//            GL.BindBuffer(BufferTarget.ArrayBuffer, vb);
//            GL.BufferData(BufferTarget.ArrayBuffer, positions.Count * 12, positions.ToArray(), BufferUsageHint.StaticDraw);
//            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

//            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib);
//            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * 12, indices.ToArray(), BufferUsageHint.StaticDraw);
//            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

//            outerEdges.Clear();
//            foreach (Edge e in edges)
//            {
//                int existsCount = 0;
//                foreach (TriangleVertexIndices t in indices)
//                {
//                    if ((e.index0 == t.I0 ||
//                         e.index0 == t.I1 ||
//                         e.index0 == t.I2) 
//                         &&
//                        (e.index1 == t.I0 ||
//                         e.index1 == t.I1 ||
//                         e.index1 == t.I2))
//                    {
//                        existsCount++;
//                    }
//                }
//                if (existsCount == 1) outerEdges.Add(e);
//            }
//            GL.BindBuffer(BufferTarget.ElementArrayBuffer, edgeib);
//            GL.BufferData(BufferTarget.ElementArrayBuffer, outerEdges.Count * 8, outerEdges.ToArray(), BufferUsageHint.StaticDraw);
//            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
//        }
//        public void Draw(int shader)
//        {
//            GL.UseProgram(shader);

//            Matrix4 mat = Matrix4.CreateTranslation(new Vector3(0, 0, 0));
//            owner.camera.SetModelMatrix(mat);
//            owner.camera.UpdateCameraBuffer();

//            GL.BindBuffer(BufferTarget.ArrayBuffer, vb);
//            int pos = GL.GetAttribLocation(shader, "POSITION");
//            GL.VertexAttribPointer(pos, 3, VertexAttribPointerType.Float, false, 12, 0);
//            GL.EnableVertexAttribArray(pos);

//            GL.Enable(EnableCap.PolygonOffsetFill);
//            GL.PolygonOffset(1, 1);

//            owner.camera.UpdateColorBuffer(0, 0, 0, 1);
//            GL.BindBuffer(BufferTarget.ElementArrayBuffer, edgeib);
//            GL.DrawElements(BeginMode.Lines, outerEdges.Count * 2, DrawElementsType.UnsignedInt, 0);

//            GL.Disable(EnableCap.PolygonOffsetFill);

//            owner.camera.UpdateColorBuffer(0, 0, .9f, 1);
//            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib);
//            GL.DrawElements(BeginMode.Triangles, indices.Count * 3, DrawElementsType.UnsignedInt, 0);
//        }
//    }
//    class PlaneTerrainModifier : TerrainModifierMesh
//    {
//        public PlaneTerrainModifier(MapEditorScene scene, Vector3 pos, float halfWidth, float halfLength) : base(scene)
//        {
//            AddTriangle(
//                pos + new Vector3(halfWidth, 0, halfLength),
//                pos + new Vector3(halfWidth, 0, -halfLength),
//                pos + new Vector3(-halfWidth, 0, -halfLength)
//                );
//            AddTriangle(
//                pos + new Vector3(-halfWidth, 0, halfLength),
//                pos + new Vector3(-halfWidth, 0, -halfLength),
//                pos + new Vector3(halfWidth, 0, halfLength)
//                );
//        }
//    }
//}
