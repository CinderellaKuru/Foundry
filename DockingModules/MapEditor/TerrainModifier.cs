using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;
using OpenTK.Graphics.OpenGL4;
using static SMHEditor.DockingModules.MapEditor.MapEditorScene;

namespace SMHEditor.DockingModules.MapEditor
{
    public class TerrainModifierMesh : ViewportScene.SceneObject
    {
        protected struct Edge
        {
            public Edge(int i0, int i1)
            {
                index0 = i0;
                index1 = i1;
            }
            public int index0, index1;
        }
        
        private MapEditorScene owner;
        private Octree octree;
        private RigidBody rb;
        int vb, ib, edgeib;

        protected List<JVector> positions = new List<JVector>();
        protected List<TriangleVertexIndices> indices = new List<TriangleVertexIndices>();
        protected List<Edge> edges = new List<Edge>();
        protected List<Edge> outerEdges = new List<Edge>();

        public TerrainModifierMesh(MapEditorScene scene)
        {
            octree = new Octree(positions, indices);
            rb = new RigidBody(new TriangleMeshShape(octree));
            rb.Tag = "TerrainModifierMesh";
            rb.IsStatic = true;
            rb.AffectedByGravity = false;
            scene.physScene.AddBody(rb);
            scene.modifierMeshes.Add(this);

            owner = scene;

            vb = GL.GenBuffer();
            ib = GL.GenBuffer();
            edgeib = GL.GenBuffer();
        }
        
        protected TriangleVertexIndices AddTriangle(JVector p0, JVector p1, JVector p2)
        {
            int i0;
            if (positions.Contains(p0)) i0 = positions.IndexOf(p0);
            else
            {
                i0 = positions.Count;
                positions.Add(p0);
            }
            int i1;
            if (positions.Contains(p1)) i1 = positions.IndexOf(p1);
            else
            {
                i1 = positions.Count;
                positions.Add(p1);
            }
            int i2;
            if (positions.Contains(p2)) i2 = positions.IndexOf(p2);
            else
            {
                i2 = positions.Count;
                positions.Add(p2);
            }

            edges.Add(new Edge(i0, i1));
            edges.Add(new Edge(i1, i2));
            edges.Add(new Edge(i2, i0));

            TriangleVertexIndices t = new TriangleVertexIndices(i0, i1, i2);
            indices.Add(t);

            octree.SetTriangles(positions, indices);
            octree.BuildOctree();
            Shape s = new TriangleMeshShape(octree);
            s.Tag = octree;
            rb.Shape = s;
            

            UpdateBuffers();
            return t;
        }

        private void UpdateBuffers()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vb);
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Count * 12, positions.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * 12, indices.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            outerEdges.Clear();
            foreach (Edge e in edges)
            {
                int existsCount = 0;
                foreach (TriangleVertexIndices t in indices)
                {
                    if ((e.index0 == t.I0 ||
                         e.index0 == t.I1 ||
                         e.index0 == t.I2) 
                         &&
                        (e.index1 == t.I0 ||
                         e.index1 == t.I1 ||
                         e.index1 == t.I2))
                    {
                        existsCount++;
                    }
                }
                if (existsCount == 1) outerEdges.Add(e);
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, edgeib);
            GL.BufferData(BufferTarget.ElementArrayBuffer, outerEdges.Count * 8, outerEdges.ToArray(), BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
        public void Draw(int shader)
        {
            //rb.Position = transform.position;

            GL.UseProgram(shader);

            owner.camera.SetModelMatrix(transform.GetModelMatrix());
            owner.camera.UpdateCameraBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vb);
            int pos = GL.GetAttribLocation(shader, "POSITION");
            GL.VertexAttribPointer(pos, 3, VertexAttribPointerType.Float, false, 12, 0);
            GL.EnableVertexAttribArray(pos);

            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(1, 1);

            owner.camera.UpdateColorBuffer(0, 0, 0, 1);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, edgeib);
            GL.DrawElements(BeginMode.Lines, outerEdges.Count * 2, DrawElementsType.UnsignedInt, 0);

            GL.Disable(EnableCap.PolygonOffsetFill);

            owner.camera.UpdateColorBuffer(0, 0, .9f, 1);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib);
            GL.DrawElements(BeginMode.Triangles, indices.Count * 3, DrawElementsType.UnsignedInt, 0);
        }
    }
    class PlaneTerrainModifier : TerrainModifierMesh
    {
        public PlaneTerrainModifier(MapEditorScene scene, float halfWidth, float halfLength) : base(scene)
        {
            AddTriangle(
                new JVector(halfWidth, 0, halfLength),
                new JVector(halfWidth, 0, -halfLength),
                new JVector(-halfWidth, 0, -halfLength)
                );
            AddTriangle(
                new JVector(-halfWidth, 0, halfLength),
                new JVector(-halfWidth, 0, -halfLength),
                new JVector(halfWidth, 0, halfLength)
                );
        }
    }
}
