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
using System.Runtime.InteropServices;
using System.Diagnostics;
using SMHEditor.ECF;
using DirectXTexNet;
using BulletSharp;
using static SMHEditor.Project.FileTypes.TerrainFile;
using Image = DirectXTexNet.Image;

namespace SMHEditor.DockingModules.MapEditor
{
    public class TerrainChunk : ViewportScene.SceneObject
    {
        public int X, Y;
        public Vector3[] positions = new Vector3[64 * 64];
        public Vector3[] normals = new Vector3[64 * 64];
        public float[] ao = new float[64 * 64];
        public byte[] alpha = new byte[64 * 64 * 2];
        int[] indices = new int[64 * 64 * 6];
        
        public TriangleMesh shape;
        public RigidBody body;

        public TerrainChunk(TerrainFileChunk tfc, int x, int y)
        {
            X = x;
            Y = y;
            //positions = tfc.positionData;

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


                    indices[(j * TerrainFile.maxVStride + i) * 2 + 0] = t0;
                    indices[(j * TerrainFile.maxVStride + i) * 2 + 1] = t1;
                }
            }
            
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
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count() * 4, indices, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            #endregion


            shape = new TriangleMeshShape(octree);
            body = new RigidBody(shape);
            body.Tag = "TerrainChunk";
            body.AffectedByGravity = false;
            body.IsStatic = true;
            body.Position = new JVector((maxVStride-1) * X, 0, (maxVStride-1) * Y);
            transform.position = body.Position;
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
        public void Draw(int shader)
        {
            GL.UseProgram(shader);

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
        public void UpdateNormals()
        {
            foreach(var t in indices)
            {
                JVector a = positions[t.I0];
                JVector b = positions[t.I1];
                JVector c = positions[t.I2];
                JVector v = JVector.Cross(b - a, c - a);
                normals[t.I0] += v;
                normals[t.I1] += v;
                normals[t.I2] += v;
            }
            for(int i =0; i < normals.Length; i++) { normals[i].Normalize(); }
        }
        public void UpdateCollision()
        {
            octree.SetTriangles(positions.ToList(), indices.ToList());
            octree.BuildOctree();
        }
    }
    
    public class MapEditorScene : ViewportScene
    {
        class CursorObject
        {
            public Transform transform = new Transform();
            public Transform lastTransform = new Transform();
            private ViewportScene owner;
            private float radius;

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
            public CursorObject(ViewportScene parent)
            {
                owner = parent;

                vb = GL.GenBuffer();
                ib = GL.GenBuffer();

                GL.BindBuffer(BufferTarget.ArrayBuffer, vb);
                GL.BufferData(BufferTarget.ArrayBuffer, 21 * 3 * 4, vData, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib);
                GL.BufferData(BufferTarget.ElementArrayBuffer, 39 * 3 * 4, iData, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                Shape sphere = new SphereShape(5f);
                radius = 5f;
            }
            public void Draw(int shader)
            {
                GL.UseProgram(shader);

                owner.camera.UpdateColorBuffer(.45f, .45f, .45f, 1);
                owner.camera.SetModelMatrix(transform.GetModelMatrix());
                owner.camera.UpdateCameraBuffer();

                GL.BindBuffer(BufferTarget.ArrayBuffer, vb);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib);
                int pos = GL.GetAttribLocation(shader, "POSITION");
                GL.VertexAttribPointer(pos, 3, VertexAttribPointerType.Float, false, 12, 0);
                GL.EnableVertexAttribArray(pos);
                GL.DrawElements(BeginMode.Triangles, 39 * 3, DrawElementsType.UnsignedInt, 0);
            }

            public float GetRadius() { return radius; }
            public void SetRadius(float rad)
            {
                radius = rad;
            }
        }
        class XYZGrabber
        {
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
            World grabberSpace;
            public RigidBody planeX;
            public RigidBody planeY;
            public RigidBody planeZ;

            ViewportScene owner;
            JVector position;
            public XYZGrabber(ViewportScene parent, JVector pos)
            {
                owner = parent;
                position = pos;

                vb = GL.GenBuffer();
                ib = GL.GenBuffer();

                GL.BindBuffer(BufferTarget.ArrayBuffer, vb);
                GL.BufferData(BufferTarget.ArrayBuffer, 24 * 4, verts, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib);
                GL.BufferData(BufferTarget.ElementArrayBuffer, 36 * 4, inds, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                
                grabberSpace = new World(new CollisionSystemSAP());

                planeX = new RigidBody(new BoxShape(10000f, .1f, 10000f));
                planeX.Tag = "XYZGrabberX";
                grabberSpace.AddBody(planeX);

                planeY = new RigidBody(new BoxShape(10000f, 10000f, .1f));
                planeY.Tag = "XYZGrabberY";
                grabberSpace.AddBody(planeY);

                planeZ = new RigidBody(new BoxShape(10000f, .1f, 10000f));
                planeZ.Tag = "XYZGrabberZ";
                grabberSpace.AddBody(planeZ);
            }
            public void Draw(int shader)
            {
                GL.UseProgram(shader);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vb);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ib);
                int pos = GL.GetAttribLocation(shader, "POSITION");
                GL.VertexAttribPointer(pos, 3, VertexAttribPointerType.Float, false, 12, 0);
                GL.EnableVertexAttribArray(pos);

                Transform t = new Transform();
                float dist = Vector3.Distance(Convert.ToTKVec3(position), Convert.ToTKVec3(owner.camera.t.position)) / 20f;
                Matrix4 scl = Matrix4.CreateScale(dist);

                #region Transforms
                //X
                t.position = new JVector(1, 0, 0) + position;
                t.eulerAngles = new JVector(0, 0, -1.57f);
                owner.camera.SetModelMatrix(scl * t.GetModelMatrix());
                owner.camera.UpdateCameraBuffer();
                owner.camera.UpdateColorBuffer(1f, 0, 0, 1f);
                GL.DrawElements(BeginMode.Triangles, 36, DrawElementsType.UnsignedInt, 0);

                //Y
                t.position = new JVector(0, 1, 0) + position;
                t.eulerAngles = new JVector(0, 0, 0);
                owner.camera.SetModelMatrix(scl * t.GetModelMatrix());
                owner.camera.UpdateCameraBuffer();
                owner.camera.UpdateColorBuffer(0, 1f, 0, 1f);
                GL.DrawElements(BeginMode.Triangles, 36, DrawElementsType.UnsignedInt, 0);

                //Z
                t.position = new JVector(0, 0, 1) + position;
                t.eulerAngles = new JVector(1.57f, 0, 0);
                owner.camera.SetModelMatrix(scl * t.GetModelMatrix());
                owner.camera.UpdateCameraBuffer();
                owner.camera.UpdateColorBuffer(0, 0, 1f, 1f);
                GL.DrawElements(BeginMode.Triangles, 36, DrawElementsType.UnsignedInt, 0);
                #endregion

            }

            float lastX = 0f;
            public void BeginClickX(Camera camera, JVector rayPos, JVector rayDir)
            {
                var v3 = camera.t.position - camera.cameraTarget;
                float dir = (float)Math.Atan2(v3.Y, v3.Z);
                planeX.Orientation = JMatrix.CreateRotationX(dir);

                RigidBody rb; JVector nrm; float frac;
                grabberSpace.CollisionSystem.Raycast(rayPos, rayDir, RayX, out rb, out nrm, out frac);
                JVector hit = rayPos + (rayDir * frac);
                lastX = hit.X;
            }
            public void ProbeX(Camera camera, JVector rayPos, JVector rayDir)
            {
                var v3 = camera.t.position - camera.cameraTarget;
                float dir = (float)Math.Atan2(v3.Y, v3.Z);
                planeX.Orientation = JMatrix.CreateRotationX(dir);

                RigidBody rb; JVector nrm; float frac;
                grabberSpace.CollisionSystem.Raycast(rayPos, rayDir, RayX, out rb, out nrm, out frac);
                JVector hit = rayPos + (rayDir * frac);
                float dist = hit.X - lastX;
                if (linkedTransform != null)
                {
                    linkedTransform.position.X += dist;
                    position = linkedTransform.position;
                }
                lastX = hit.X;
            }
            private bool RayX(RigidBody body, JVector normal, float fraction)
            {
                if ((string)body.Tag == "XYZGrabberX") return true;
                else return false;
            }

            float lastY = 0f;
            public void BeginClickY(Camera camera, JVector rayPos, JVector rayDir)
            {
                var v3 = camera.t.position - camera.cameraTarget;
                float dir = (float)Math.Atan2(v3.Z, v3.X);
                planeX.Orientation = JMatrix.CreateRotationY(-dir + 1.5707f);

                RigidBody rb; JVector nrm; float frac;
                grabberSpace.CollisionSystem.Raycast(rayPos, rayDir, RayY, out rb, out nrm, out frac);
                JVector hit = rayPos + (rayDir * frac);
                lastY = hit.Y;
            }
            public void ProbeY(Camera camera, JVector rayPos, JVector rayDir)
            {
                var v3 = camera.t.position - camera.cameraTarget;
                float dir = (float)Math.Atan2(v3.Z, v3.X);
                planeX.Orientation = JMatrix.CreateRotationY(-dir + 1.5707f);

                RigidBody rb; JVector nrm; float frac;
                grabberSpace.CollisionSystem.Raycast(rayPos, rayDir, RayY, out rb, out nrm, out frac);
                JVector hit = rayPos + (rayDir * frac);
                float dist = hit.Y - lastY;
                if (linkedTransform != null)
                {
                    linkedTransform.position.Y += dist;
                    position = linkedTransform.position;
                }
                lastY = hit.Y;
            }
            private bool RayY(RigidBody body, JVector normal, float fraction)
            {
                if ((string)body.Tag == "XYZGrabberY") return true;
                else return false;
            }

            float lastZ = 0f;
            public void BeginClickZ(Camera camera, JVector rayPos, JVector rayDir)
            {
                var v3 = camera.t.position - camera.cameraTarget;
                float dir = (float)Math.Atan2(v3.Y ,v3.X);
                planeX.Orientation = JMatrix.CreateRotationZ(dir);

                RigidBody rb; JVector nrm; float frac;
                grabberSpace.CollisionSystem.Raycast(rayPos, rayDir, RayZ, out rb, out nrm, out frac);
                JVector hit = rayPos + (rayDir * frac);
                lastZ = hit.Z;
                Console.WriteLine(lastZ);
            }
            public void ProbeZ(Camera camera, JVector rayPos, JVector rayDir)
            {
                var v3 = camera.t.position - camera.cameraTarget;
                float dir = (float)Math.Atan2(v3.Y, v3.X);
                planeX.Orientation = JMatrix.CreateRotationZ(dir);

                RigidBody rb; JVector nrm; float frac;
                grabberSpace.CollisionSystem.Raycast(rayPos, rayDir, RayZ, out rb, out nrm, out frac);
                JVector hit = rayPos + (rayDir * frac);
                Console.WriteLine(hit);
                float dist = hit.Z - lastZ;
                if (linkedTransform != null)
                {
                    linkedTransform.position.Z += dist;
                    position = linkedTransform.position;
                }
                lastZ = hit.Z;
            }
            private bool RayZ(RigidBody body, JVector normal, float fraction)
            {
                if ((string)body.Tag == "XYZGrabberZ") return true;
                else return false;
            }

            public Transform linkedTransform;
        }

        public List<TerrainModifierMesh> modifierMeshes = new List<TerrainModifierMesh>();

        private CollisionConfiguration cfg;
        private CollisionDispatcher dispatcher;
        private BroadphaseInterface bpI;
        private ConstraintSolver cs;
        public DiscreteDynamicsWorld physScene;
        private TerrainSize size;
        private XYZGrabber grabber;

        public class ChunkIndexMap
        {
            public ChunkIndexMap(int _x, int _y, int _index) { x = _x; y = _y; index = _index; }
            public int x, y, index;
        }

        public int terrainShader;
        public int helperShader;
        public int lineShader;
        public MapEditorScene() : base(false)
        {
            cfg = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(cfg);
            bpI = new DbvtBroadphase();
            physScene = new DiscreteDynamicsWorld(dispatcher, bpI, cs, cfg);

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

            #region Line Shader
            lineShader = GL.CreateProgram();
            int lineVS = GL.CreateShader(ShaderType.VertexShader);
            //int lineGS = GL.CreateShader(ShaderType.GeometryShader);
            int lineFS = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(lineVS, File.ReadAllText("Shaders/lineShader.vs"));
            //GL.ShaderSource(lineGS, File.ReadAllText("Shaders/lineShader.gs"));
            GL.ShaderSource(lineFS, File.ReadAllText("Shaders/lineShader.fs"));
            GL.CompileShader(lineVS);
            //GL.CompileShader(lineGS);
            GL.CompileShader(lineFS);
            GL.AttachShader(lineShader, lineVS);
            //GL.AttachShader(lineShader, lineGS);
            GL.AttachShader(lineShader, lineFS);
#if DEBUG
            Console.WriteLine(GL.GetShaderInfoLog(lineVS));
            //Console.WriteLine(GL.GetShaderInfoLog(lineGS));
            Console.WriteLine(GL.GetShaderInfoLog(lineFS));
#endif
            GL.LinkProgram(lineShader);
            #endregion

            cursor = new CursorObject(this);
            temp = new CursorObject(this);

            grabber = new XYZGrabber(this, new JVector(0,0,0));

            PlaneTerrainModifier m = new PlaneTerrainModifier(this, 3f, 3f);
            m.transform.position = new JVector(10f, 1f, 14f);
        }

        public TerrainChunk[,] chunks;
        public void LoadFile(TerrainFile trn)
        {
            size = trn.Size;
            chunks = new TerrainChunk[(int)trn.Size, (int)trn.Size];
            foreach (TerrainFileChunk tfc in trn.chunks)
            {
                chunks[tfc.X, tfc.Y] = new TerrainChunk(tfc, tfc.X, tfc.Y);
                physScene.AddBody(chunks[tfc.X, tfc.Y].body);
            }
        }
        public TerrainFile Save()
        {
            TerrainFile trn = new TerrainFile();
            // do save here
            return trn;
        }

        #region Export
        // Export //
        //Returns a vertex's offset from its original point in the grid.
        JVector GetRelPos(int x, int y)
        {
            int cx = x / maxVStride;
            int cy = y / maxVStride;
            JVector v = chunks[cx, cy].positions[(x - (cx * maxVStride)) * maxVStride + (y - (cy * maxVStride))];
            v.X -= x - (cx * maxVStride);
            v.Z -= y - (cy * maxVStride);
            return v;
        }
        JVector GetNormal(int x, int y)
        {
            int cx = x / maxVStride;
            int cy = y / maxVStride;
            JVector v = chunks[cx, cy].normals[(x - (cx * maxVStride)) * maxVStride + (y - (cy * maxVStride))];
            return v;
        }
        float GetAO(int x, int y)
        {
            int cx = x / maxVStride;
            int cy = y / maxVStride;
            float ao = chunks[cx, cy].ao[x - (cx * maxVStride) + (y - (cy * maxVStride))];
            return ao;
        }
        JBBox GetRelBBox()
        {
            JVector min = new JVector(0, 0, 0);
            JVector max = new JVector(0, 0, 0);
            for(int i = 0; i < (int)size * maxVStride; i++)
            {
                for (int j = 0; j < (int)size * maxVStride; j++)
                {
                    JVector rel = GetRelPos(i, j);
                    if (rel.X < min.X) min.X = rel.X;
                    if (rel.Y < min.Y) min.X = rel.X;
                    if (rel.Z < min.Z) min.X = rel.X;
                    if (rel.X > max.X) max.X = rel.X;
                    if (rel.Y > max.Y) max.Y = rel.Y;
                    if (rel.Z > max.Z) max.Z = rel.Z;
                }
            }
            return new JBBox(min, max);
        }
        float rangeCompact(float r)
        {
            int mR = (int)(r + 1);
            if (mR % 2 == 0)
                return (float)mR;

            return (float)((int)r);
        }
        public UInt32 PackPos(JVector pos, JVector mid, JVector vrange)
        {
            Int32 outval = 0;
            //X = 11bits
            //Y = 11bits
            //Z = 10bits
            const int bitMax10 = 1023; //0x3ff
            const int bitMax9 = 511;   //0x1ff;


            //scale based upon range
            ushort range = (ushort)((((pos.X + mid.X) / vrange.X) * (bitMax10)));
            ushort output = (ushort)(range & bitMax10);
            outval |= output << 20;

            range = (ushort)(((pos.Y + mid.Y) / vrange.Y) * (bitMax10));
            output = (ushort)(range & bitMax10);
            outval |= output << 10;

            range = (ushort)((((pos.Z + mid.Z) / vrange.Z) * (bitMax10)));
            output = (ushort)(range & bitMax10);
            outval |= output;

            return (UInt32)outval;
        }
        public UInt32 PackNormal(JVector norm)
        {
            Int32 outval = 0;
            //X = 11bits
            //Y = 11bits
            //Z = 10bits
            const int bitMax10 = 1023; //0x3ff
            const int bitMax9 = 511;   //0x1ff;

            //scale based upon range
            ushort range = (ushort)((((norm.X + 1) * 0.5f) * (bitMax10)));
            ushort output = (ushort)(range & bitMax10);
            outval |= output << 20;

            range = (ushort)(((norm.Y + 1) * 0.5f) * (bitMax10));
            output = (ushort)(range & bitMax10);
            outval |= output << 10;

            range = (ushort)((((norm.Z + 1) * 0.5f) * (bitMax10)));
            output = (ushort)(range & bitMax10);
            outval |= output;

            return (UInt32)outval;
        }
        public void ExportXTD(string path)
        {
            FileStream fs = File.Open("D:\\StumpyHWDEMod\\SMEditorTests\\scenario\\skirmish\\design\\blood_gulch\\blood_gulch.xtd.clean", FileMode.Open);
            byte[] buff = new byte[fs.Length];
            fs.Read(buff, 0, (int)fs.Length);

            #region Chunk Headers
            ECFFile ecf = new ECFFile();

            #region XTD Header
            ECFChunk xtdHeader = new ECFChunk(0x0000000000001111);
            xtdHeader.data.AddRange(BitConverter.GetBytes(12).Reverse()); //version
            xtdHeader.data.AddRange(BitConverter.GetBytes((int)size * maxVStride).Reverse()); //numXVerts;
            xtdHeader.data.AddRange(BitConverter.GetBytes((int)size).Reverse()); //numXChunks
            xtdHeader.data.AddRange(BitConverter.GetBytes(1f).Reverse()); //tileScale
            ecf.AddChunk(xtdHeader);
            #endregion

            JVector wmin = JVector.Zero, wmax = JVector.Zero;

            #region Chunks
            foreach(TerrainChunk c in chunks)
            {
                ECFChunk chunkHeader = new ECFChunk(0x0000000000002222);
                chunkHeader.data.AddRange(BitConverter.GetBytes(c.X).Reverse());
                chunkHeader.data.AddRange(BitConverter.GetBytes(c.Y).Reverse());
                chunkHeader.data.AddRange(BitConverter.GetBytes(maxVStride).Reverse());
                JVector cmin = c.shape.BoundingBox.Min + new JVector(c.X * maxVStride, 0, c.Y * maxVStride) - new JVector(2, 2, 2);
                JVector cmax = c.shape.BoundingBox.Max + new JVector(c.X * maxVStride, 0, c.Y * maxVStride) + new JVector(2, 2, 2);
                chunkHeader.data.AddRange(BitConverter.GetBytes(cmin.X).Reverse());
                chunkHeader.data.AddRange(BitConverter.GetBytes(cmin.Y).Reverse());
                chunkHeader.data.AddRange(BitConverter.GetBytes(cmin.Z).Reverse());
                chunkHeader.data.AddRange(BitConverter.GetBytes(cmax.X).Reverse());
                chunkHeader.data.AddRange(BitConverter.GetBytes(cmax.Y).Reverse());
                chunkHeader.data.AddRange(BitConverter.GetBytes(cmax.Z).Reverse());
                chunkHeader.data.Add(0x01); //canCastShadows
                chunkHeader.padding = 3;
                ecf.AddChunk(chunkHeader);

                if (cmin.X < wmin.X) wmin.X = cmin.X;
                if (cmin.Y < wmin.Y) wmin.Y = cmin.Y;
                if (cmin.Z < wmin.Z) wmin.Z = cmin.Z;

                if (cmax.X > wmax.X) wmax.X = cmax.X;
                if (cmax.Y > wmax.Y) wmax.Y = cmax.Y;
                if (cmax.Z > wmax.Z) wmax.Z = cmax.Z;
            }
            #endregion

            xtdHeader.data.AddRange(BitConverter.GetBytes(wmin.X).Reverse());
            xtdHeader.data.AddRange(BitConverter.GetBytes(wmin.Y).Reverse());
            xtdHeader.data.AddRange(BitConverter.GetBytes(wmin.Z).Reverse());
            xtdHeader.data.AddRange(BitConverter.GetBytes(wmax.X).Reverse());
            xtdHeader.data.AddRange(BitConverter.GetBytes(wmax.Y).Reverse());
            xtdHeader.data.AddRange(BitConverter.GetBytes(wmax.Z).Reverse());
            #endregion

            #region Atlas
            ECFChunk atlas = new ECFChunk(0x0000000000008888);

            JBBox relBBox = GetRelBBox();
            Console.WriteLine(relBBox.Min);
            Console.WriteLine(relBBox.Max);
            JVector max = relBBox.Min;
            JVector mid = relBBox.Max;
            JVector range = (max - mid) * 2.1f;

            mid = range * .5f;
            mid.X = rangeCompact(mid.X);
            mid.Y = rangeCompact(mid.Y);
            mid.Z = rangeCompact(mid.Z);
            range = mid * 2.1f;

            if (range.X == 0) { mid.X = 0; range.X = 1; }
            if (range.Y == 0) { mid.Y = 0; range.Y = 1; }
            if (range.Z == 0) { mid.Z = 0; range.Z = 1; }

            JVector diff = relBBox.Min - relBBox.Max;
            int qDiff = (int)(diff.Y / 100) * 2;
            JVector adjustedMid = mid - new JVector(0, qDiff / 10.0f, 0);

            atlas.data.AddRange(BitConverter.GetBytes(adjustedMid.X).Reverse());
            atlas.data.AddRange(BitConverter.GetBytes(adjustedMid.Y).Reverse());
            atlas.data.AddRange(BitConverter.GetBytes(adjustedMid.Z).Reverse());
            atlas.data.AddRange(BitConverter.GetBytes(0));
            atlas.data.AddRange(BitConverter.GetBytes(range.X).Reverse());
            atlas.data.AddRange(BitConverter.GetBytes(range.Y).Reverse());
            atlas.data.AddRange(BitConverter.GetBytes(range.Z).Reverse());
            atlas.data.AddRange(BitConverter.GetBytes(0));

            for (int i = 0; i < (int)size * maxVStride; i++)
            {
                for (int j = 0; j < (int)size * maxVStride; j++)
                {
                    atlas.data.AddRange(BitConverter.GetBytes(PackPos(GetRelPos(i, j), mid, range)));
                }
            }
            for (int i = 0; i < (int)size * maxVStride; i++)
            {
                for (int j = 0; j < (int)size * maxVStride; j++)
                {
                    atlas.data.AddRange(BitConverter.GetBytes(PackNormal(GetNormal(i, j))));
                }
            }

            ECFChunk ao = new ECFChunk(0x000000000000CCCC);
            byte[] aoTex;
            unsafe
            {
                List<byte> aoBuff = new List<byte>();
                for (int i = 0; i < (int)size * maxVStride; i++)
                    for (int j = 0; j < (int)size * maxVStride; j++)
                    {
                        aoBuff.Add(1);
                        aoBuff.Add(1);
                        aoBuff.Add(1);
                        aoBuff.Add((byte)(GetAO(i, j) * 255));
                    }

                Image aoImg;
                fixed (byte* b = aoBuff.ToArray())
                {
                    aoImg = new Image(1024, 1024, DXGI_FORMAT.BC3_UNORM, 8, 8, (IntPtr)b, null);
                }
                int len = (int)size * (int)size * maxVStride * maxVStride * 4;
                aoTex = new byte[len];
                Marshal.Copy(aoImg.Pixels, aoTex, 0, len);
                ao.data.AddRange(aoTex);
            }

            ECFChunk alpha = new ECFChunk(0x000000000000DDDD);
            for (int i = 0; i < 4096 * 2 * 64; i++) { alpha.data.Add(0xFF); }
            ECFChunk tess = new ECFChunk(0x000000000000AAAA);
            tess.data.AddRange(BitConverter.GetBytes(0x00000000).Reverse());
            tess.data.AddRange(BitConverter.GetBytes(0x00000000).Reverse());
            ECFChunk light = new ECFChunk(0x000000000000BBBB);
            light.data.AddRange(BitConverter.GetBytes(4096 * 2 * 64).Reverse());
            for (int i = 0; i < 4096 * 2 * 64; i++) { light.data.Add(0x00); }

            ecf.AddChunk(atlas);
            ecf.AddChunk(ao);
            ecf.AddChunk(alpha);
            ecf.AddChunk(tess);
            //ecf.AddChunk(light);

            #endregion

            ecf.Save(path);

            #region temp
            for (int i = 0; i < (4 * 8) + 4096 * 4 * 256 * 2; i++)
            {
                buff[16576 + i] = atlas.data[i];
            }
            for (int i = 0; i < 4096 * 256 / 2; i+=8)
            {
                buff[8405216 + i + 0] = 0xff;
                buff[8405216 + i + 1] = 0;
                buff[8405216 + i + 2] = 0;
                buff[8405216 + i + 3] = 0;
                buff[8405216 + i + 4] = 0;
                buff[8405216 + i + 5] = 0;
                buff[8405216 + i + 6] = 0;
                buff[8405216 + i + 7] = 0;
            }

            FileStream o = File.Create("D:\\StumpyHWDEMod\\SMEditorTests\\scenario\\skirmish\\design\\blood_gulch\\blood_gulch.xtd");
            o.Write(buff, 0, buff.Length);
            o.Close();
            #endregion
        }
        #endregion

        // Drawing //
        public override void DrawScene()
        {
            base.DrawScene();

            host.SetRenderTarget(ViewportPage.RenderTarget.PICK);
            grabber.Draw(helperShader);
            host.SetRenderTarget(ViewportPage.RenderTarget.DEFAULT);

            UpdateCursor();

            List<TerrainChunk> cs = new List<TerrainChunk>();
            foreach (var v in GetVertsInCursor())
            {
                //chunks[v.x, v.y].positions[v.index] += new JVector(0, 1f, 0);
                if(!cs.Contains(chunks[v.x, v.y])) cs.Add(chunks[v.x, v.y]);
            }

            foreach(TerrainChunk c in cs)
            {
                c.UpdateNormals();
                c.UpdateBuffers();
                //c.UpdateCollision();
            }

            if(OpenTK.Input.Keyboard.GetState().IsKeyDown(OpenTK.Input.Key.K))
            {
                ExportXTD("D:\\StumpyHWDEMod\\SMEditorTests\\scenario\\skirmish\\design\\blood_gulch\\blood_gulch.xtd");
            }


            // // DRAW // //

            foreach (TerrainChunk c in chunks)
            {
                camera.SetModelMatrix(c.transform.GetModelMatrix());
                camera.UpdateCameraBuffer();
                c.Draw(terrainShader);
            }

            cursor.Draw(helperShader);
            temp.Draw(helperShader);

            foreach(var m in modifierMeshes)
            {
                m.Draw(lineShader);
            }

            host.SetDepthTest(false);
            grabber.Draw(helperShader);
            host.SetDepthTest(true);
        }

        // Cursor
        private static float CURSOR_RAY_LENGTH = 10000f;
        CursorObject cursor;
        CursorObject temp;
        enum MouseState
        {
            Up = 0,
            Down = 1,
            ClickedX,
            ClickedY,
            ClickedZ,
            Terraforming
        };
        MouseState mouseState;
        public void UpdateCursor()
        {
            if (host == null) return;

            var mouse = OpenTK.Input.Mouse.GetCursorState();
            Point clientPos = host.viewport.glControl.PointToClient(new Point(mouse.X, mouse.Y));
            
            cursor.lastTransform.position = new JVector(cursor.transform.position.X, cursor.transform.position.Y, cursor.transform.position.Z);

            #region State
            float normX = (clientPos.X - (host.viewport.glControl.Width / 2f)) / (float)host.viewport.glControl.Width;
            float normY = (clientPos.Y - (host.viewport.glControl.Height / 2f)) / (float)host.viewport.glControl.Height;
            var vx = (2 * normX) / camera.mData[0].M11;
            var vy = (-2 * normY) / camera.mData[0].M22;

            var invM = camera.mData[1].Inverted();

            JVector rayPos = Convert.ToJVec3(Vector3.TransformPosition(new Vector3(0, 0, 0), invM));
            JVector rayDir = Convert.ToJVec3(Vector3.TransformNormal(new Vector3(vx, vy, -1.0F), invM));

            if (mouse.IsButtonUp(OpenTK.Input.MouseButton.Left)
                && mouseState != MouseState.Terraforming)
            {
                mouseState = MouseState.Up;
            }

            if (mouse.IsButtonDown(OpenTK.Input.MouseButton.Left) && mouseState == MouseState.Up)
            {
                uint pick = host.Pick(clientPos.X, host.viewport.glControl.Height - clientPos.Y);
                switch (pick)
                {
                    case 0xff0000ff:
                        mouseState = MouseState.ClickedX;
                        grabber.BeginClickX(camera, rayPos, rayDir);
                        break;
                    case 0xff00ff00:
                        mouseState = MouseState.ClickedY;
                        grabber.BeginClickY(camera, rayPos, rayDir);
                        break;
                    case 0xffff0000:
                        mouseState = MouseState.ClickedZ;
                        grabber.BeginClickZ(camera, rayPos, rayDir);
                        break;
                    default:
                        {
                            mouseState = MouseState.Up;
                            float frac;
                            JVector nrm;
                            RigidBody rb;
                            if (physScene.CollisionSystem.Raycast(rayPos, rayDir * CURSOR_RAY_LENGTH, ModifierRay, out rb, out nrm, out frac))
                            {
                                cursor.transform.position = rayPos + (rayDir * CURSOR_RAY_LENGTH * frac);
                                if(rb.Shape.Tag is Octree)
                                {
                                    Octree o = rb.Shape.Tag as Octree;
                                    List<int> tris = new List<int>();
                                    Console.WriteLine(o.GetTrianglesIntersectingRay(tris, rayPos, rayDir));
                                }
                            }
                            break;
                        }
                }
                
            }

            if (mouseState == MouseState.Up)
            {
                
            }
            if (mouseState == MouseState.Terraforming)
            {
            }
            if (mouseState == MouseState.ClickedX)
            {
                grabber.linkedTransform = temp.transform;
                grabber.ProbeX(camera, rayPos, rayDir);
            }
            if (mouseState == MouseState.ClickedY)
            {
                grabber.linkedTransform = temp.transform;
                grabber.ProbeY(camera, rayPos, rayDir);
            }
            if (mouseState == MouseState.ClickedZ)
            {
                grabber.linkedTransform = temp.transform;
                grabber.ProbeZ(camera, rayPos, rayDir);
            }

            #endregion
        }

        private bool ModifierRay(RigidBody body, JVector normal, float fraction)
        {
            if ((string)body.Tag == "TerrainModifierMesh") return true;
            else return false;
        }
        
        // Editing
        public List<ChunkIndexMap> GetVertsInCursor()
        {
            List<ChunkIndexMap> maps = new List<ChunkIndexMap>();

            //foreach (TerrainChunk c in chunks)
            //{
            //    if (physScene.CollisionSystem.CheckBoundingBoxes(cursor.rb, c.body))
            //    {
            //        for (int i = 0; i < TerrainFile.maxVStride * TerrainFile.maxVStride; i++)
            //        {
            //            if (Util.Distance(c.positions[i] + c.transform.position, cursor.transform.position) < cursor.GetRadius())
            //            {
            //                maps.Add(new ChunkIndexMap(c.X, c.Y, i));
            //            }
            //        }
            //    }
            //}
            return maps;
        }
    }
}
