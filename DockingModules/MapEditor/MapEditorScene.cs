using SMHEditor.Project.FileTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using OpenTK;
using System.Drawing;
using Jitter.Collision;
using Jitter;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using System.Runtime.InteropServices;
using Jitter.Dynamics;

namespace SMHEditor.DockingModules.MapEditor
{
    public class TerrainChunk : ViewportScene.SceneObject
    {
        public int X, Y;
        public JVector[] positions = new JVector[64 * 64];
        public JVector[] normals = new JVector[64 * 64];
        TriangleVertexIndices[] indices = new TriangleVertexIndices[64 * 64 * 2];

        public Octree octree;
        public TriangleMeshShape shape;
        public RigidBody body;

        public TerrainChunk(TerrainFileChunk tfc, int x, int y)
        {
            X = x;
            Y = y;
            positions = tfc.positionData;

            // Gen indices
            for (int j = 0; j < TerrainFile.maxVStride - 1; j++)
            {
                for (int i = 0; i < TerrainFile.maxVStride - 1; i++)
                {
                    int i1, i2, i3, i4, i5, i6;

                    int row1 = i * TerrainFile.maxVStride;
                    int row2 = (i + 1) * TerrainFile.maxVStride;

                    i1 = row1 + j;
                    i2 = row1 + j + 1;
                    i3 = row2 + j;

                    i4 = row2 + j + 1;
                    i5 = row2 + j;
                    i6 = row1 + j + 1;

                    TriangleVertexIndices t0 = new TriangleVertexIndices(i1, i2, i3);
                    TriangleVertexIndices t1 = new TriangleVertexIndices(i4, i5, i6);
                    indices[(j * TerrainFile.maxVStride + i) * 2 + 0] = t0;
                    indices[(j * TerrainFile.maxVStride + i) * 2 + 1] = t1;
                }
            }

            UpdateAABB();
            UpdateNormals();

            #region Buffers
            vertexBuffers.Add("pos", GL.GenBuffer());
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffers["pos"]);
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Count() * 12, positions, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            vertexBuffers.Add("normals", GL.GenBuffer());
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffers["normals"]);
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Count() * 12, normals, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            indexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count() * 12, indices, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            #endregion

            octree = new Octree(positions.ToList(), indices.ToList());
            octree.BuildOctree();
            shape = new TriangleMeshShape(octree);
            body = new RigidBody(shape);
            body.AffectedByGravity = false;
            body.IsStatic = true;
            body.Position = new JVector(TerrainFile.maxVStride * X, 0, TerrainFile.maxVStride * Y);
        }

        public void UpdateBuffers()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffers["pos"]);
            GL.BufferData(BufferTarget.ArrayBuffer, positions.Count() * 12, positions, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffers["normals"]);
            GL.BufferData(BufferTarget.ArrayBuffer, normals.Count() * 12, normals, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
        public override void Draw(int shader)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffers["pos"]);
            int pos = GL.GetAttribLocation(shader, "POSITION");
            GL.VertexAttribPointer(pos, 3, VertexAttribPointerType.Float, false, 12, 0);
            GL.EnableVertexAttribArray(pos);

            int norm = GL.GetAttribLocation(shader, "NORMALS");
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffers["normals"]);
            GL.VertexAttribPointer(norm, 3, VertexAttribPointerType.Float, false, 12, 0);
            GL.EnableVertexAttribArray(norm);

            GL.DrawElements(BeginMode.Triangles, 4096*6, DrawElementsType.UnsignedInt, 0);
        }
        public void UpdateAABB()
        {

        }
        public void UpdateNormals()
        {

        }
        public void UpdateCollision()
        {
            octree.SetTriangles(positions.ToList(), indices.ToList());
            octree.BuildOctree();
        }
    }
    
    public class MapEditorScene : ViewportScene
    {
        class CursorObject : SceneObject
        {
            public RigidBody rb;

            float[] vData = new float[] {
0.000000F, 0.000000F, 0.000000F,
0.000000F, 0.000000F, 0.000000F,
0.000000F, 1.993729F, -1.000000F,
0.781832F, 1.993729F, -0.623490F,
0.974928F, 1.993729F, 0.222521F,
0.433884F, 1.993729F, 0.900969F,
-0.433884F, 1.993729F, 0.900969F,
-0.974928F, 1.993729F, 0.222521F,
-0.781832F, 1.993729F, -0.623490F,
-0.232554F, 2.165461F, 0.053079F,
-0.186494F, 2.165461F, -0.148724F,
-0.103496F, 2.165461F, 0.214912F,
0.103496F, 2.165461F, 0.214912F,
0.142345F, 5.531382F, 0.295583F,
-0.142345F, 5.531382F, 0.295583F,
0.186494F, 2.165461F, -0.148724F,
0.232554F, 2.165461F, 0.053079F,
0.000000F, 2.165461F, -0.238535F,
0.256496F, 5.531382F, -0.204548F,
0.000000F, 5.531382F, -0.328070F,
-0.256496F, 5.531382F, -0.204548F,
-0.319846F, 5.531382F, 0.073004F,
0.319846F, 5.531382F, 0.073004F };
            int[] iData = new int[] {
1, 2, 3,
1, 3, 4,
1, 4, 5,
1, 5, 6,
1, 6, 7,
7, 10, 8,
1, 7, 8,
1, 8, 2,
11, 13, 14,
5, 11, 6,
4, 15, 16,
3, 17, 15,
8, 17, 2,
6, 9, 7,
5, 16, 12,
21, 14, 13,
15, 22, 16,
10, 19, 17,
9, 14, 21,
16, 13, 12,
15, 19, 18,
9, 20, 10,
7, 9, 10,
11, 12, 13,
5, 12, 11,
4, 3, 15,
3, 2, 17,
8, 10, 17,
6, 11, 9,
5, 4, 16,
22, 18, 13,
18, 19, 13,
19, 20, 13,
20, 21, 13,
15, 18, 22,
10, 20, 19,
9, 11, 14,
16, 22, 13,
15, 17, 19,
9, 21, 20 };

            int vb, ib;
            public CursorObject()
            {
                vb = GL.GenBuffer();
                ib = GL.GenBuffer();

                GL.BindBuffer(BufferTarget.ArrayBuffer, vb);
                GL.BufferData(BufferTarget.ArrayBuffer, 21 * 3 * 4, vData, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib);
                GL.BufferData(BufferTarget.ElementArrayBuffer, 39 * 3 * 4, iData, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                rb = new RigidBody(new SphereShape(5f));
            }
            public override void Draw(int shader)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vb);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib);
                int pos = GL.GetAttribLocation(shader, "POSITION");
                GL.VertexAttribPointer(pos, 3, VertexAttribPointerType.Float, false, 12, 0);
                GL.EnableVertexAttribArray(pos);
                GL.DrawElements(BeginMode.Triangles, 39 * 3, DrawElementsType.UnsignedInt, 0);
            }
        }

        World physScene;
        CollisionSystem colSystem;

        public class ChunkIndexMap
        {
            public ChunkIndexMap(int _x, int _y, int _index) { x = _x; y = _y; index = _index; }
            public int x, y, index;
        }

        public int terrainShader;
        public int helperShader;
        public MapEditorScene() : base(false)
        {
            colSystem = new CollisionSystemSAP();
            physScene = new World(colSystem);

            #region Terrain Shader
            terrainShader = GL.CreateProgram();
            int terrainVS = GL.CreateShader(ShaderType.VertexShader);
            int terrainFS = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(terrainVS, File.ReadAllText("Shaders/terrainShader.vs"));
            GL.ShaderSource(terrainFS, File.ReadAllText("Shaders/terrainShader.fs"));
            GL.CompileShader(terrainVS);
            GL.CompileShader(terrainFS);
            GL.AttachShader(terrainShader, terrainVS);
            GL.AttachShader(terrainShader, terrainFS);
#if DEBUG
            Console.WriteLine(GL.GetShaderInfoLog(terrainVS));
            Console.WriteLine(GL.GetShaderInfoLog(terrainFS));
#endif
            GL.LinkProgram(terrainShader);
            #endregion

            #region Helper Shader
            helperShader = GL.CreateProgram();
            int helperVS = GL.CreateShader(ShaderType.VertexShader);
            int helperFS = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(helperVS, File.ReadAllText("Shaders/helperShader.vs"));
            GL.ShaderSource(helperFS, File.ReadAllText("Shaders/helperShader.fs"));
            GL.CompileShader(helperVS);
            GL.CompileShader(helperFS);
            GL.AttachShader(helperShader, helperVS);
            GL.AttachShader(helperShader, helperFS);
#if DEBUG
            Console.WriteLine(GL.GetShaderInfoLog(helperVS));
            Console.WriteLine(GL.GetShaderInfoLog(helperFS));
#endif
            GL.LinkProgram(helperShader);
            #endregion

            objects[terrainShader] = new List<SceneObject>();
            objects[helperShader] = new List<SceneObject>();

            cursor = new CursorObject();
            objects[helperShader].Add(cursor);
            physScene.AddBody(cursor.rb);
        }

        public TerrainChunk[,] chunks;
        public void LoadFile(TerrainFile trn)
        {
            chunks = new TerrainChunk[(int)trn.Size, (int)trn.Size];
            foreach (TerrainFileChunk tfc in trn.chunks)
            {
                chunks[tfc.X, tfc.Y] = new TerrainChunk(tfc, tfc.X, tfc.Y);
                objects[terrainShader].Add(chunks[tfc.X, tfc.Y]);
                physScene.AddBody(chunks[tfc.X, tfc.Y].body);
            }
        }
        public TerrainFile Save()
        {
            TerrainFile trn = new TerrainFile();
            // do save here
            return trn;
        }

        // Drawing
        public override void DrawScene()
        {
            UpdateCursorState();

            if(OpenTK.Input.Mouse.GetState().IsButtonDown(OpenTK.Input.MouseButton.Left))
            {
                foreach(var v in GetVertsInCursor())
                {
                    TerrainChunk c = chunks[v.x, v.y];

                    c.UpdateBuffers();
                }
            }

            //last
            base.DrawScene();
        }

        // Cursor
        private static float CURSOR_RAY_LENGTH = 200f;
        CursorObject cursor;
        public void UpdateCursorState()
        {
            if (host == null) return;

            var mouse = OpenTK.Input.Mouse.GetCursorState();

            float normX = (host.viewport.glControl.PointToClient(new Point(mouse.X, 0)).X - (host.viewport.glControl.Width / 2f)) / (float)host.viewport.glControl.Width;
            float normY = (host.viewport.glControl.PointToClient(new Point(0, mouse.Y)).Y - (host.viewport.glControl.Height / 2f)) / (float)host.viewport.glControl.Height;
            var vx = (2 * normX) / camera.mData[0].M11;
            var vy = (-2 * normY) / camera.mData[0].M22;

            var invM = camera.mData[1].Inverted();

            JVector pos = Convert.ToJVec3(Vector3.TransformPosition(new Vector3(0, 0, 0), invM));
            JVector dir = Convert.ToJVec3(Vector3.TransformNormal(new Vector3(vx, vy, -1.0F), invM));

            RigidBody body;
            JVector nrm;
            float frac;
            if (physScene.CollisionSystem.Raycast(pos, dir * CURSOR_RAY_LENGTH, null, out body, out nrm, out frac))
            {
                cursor.transform.position = pos + frac * (dir * CURSOR_RAY_LENGTH);
            }
        }
        
        // Editing
        public List<ChunkIndexMap> GetVertsInCursor()
        {
            List<ChunkIndexMap> maps = new List<ChunkIndexMap>();

            //foreach(TerrainChunk c in chunks)
            //{
            //    if(physScene.CollisionSystem.CheckBoundingBoxes(cursor.rb, c.body))
            //    {
            //        for(int i = 0; i < TerrainFile.maxVStride * TerrainFile.maxVStride; i++)
            //        {
            //            if (Util.Distance(c.positions[i], cursor.transform.position) < 5f)
            //            {
            //                maps.Add(new ChunkIndexMap(c.X, c.Y, i));
            //            }
            //        }
            //    }
            //}
            Console.WriteLine(maps.Count());
            return maps;
        }
    }
}
