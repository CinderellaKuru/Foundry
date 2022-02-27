using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Jitter.LinearMath;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace SMHEditor.DockingModules.MapEditor
{
    public class Camera
    {
        public Transform t = new Transform();
        public Matrix4[] mData = new Matrix4[3]; // 0=proj, 1=view, 2=model
        float[] colorData = new float[4] { 1, 1, 1, 1 }; //rgba
        public int mDataBuff, colorBuff;
        public float MoveSpeed = .1f;
        public int pickFBO;

        public Camera()
        {
            mData[0] = Matrix4.Identity;
            mData[1] = Matrix4.Identity;
            mData[2] = Matrix4.Identity;

            t.position = new JVector(0, 0, -1); //must come before any rotate/addradius calls.
            Rotate(0, .5f);
            AddRadius(25f);

            mDataBuff = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, mDataBuff);
            GL.BufferData(BufferTarget.UniformBuffer, Marshal.SizeOf(new Matrix4()) * 3, mData, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            colorBuff = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.UniformBuffer, colorBuff);
            GL.BufferData(BufferTarget.UniformBuffer, 16, colorData, BufferUsageHint.DynamicDraw);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);

            UpdateViewMatrix();
            UpdateProjMatrix();
            UpdateCameraBuffer();


        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Move fucntions.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public JVector cameraTarget = new JVector(0, 0, 0);
        public float cameraRadius = 0f;
        public void Rotate(float yaw, float pitch)
        {
            pitch = -pitch;

            JVector worldUp = new JVector(0, 1, 0);

            JVector cameraDirection = JVector.Normalize(t.position - cameraTarget);
            JVector cameraRight = JVector.Normalize(JVector.Cross(worldUp, cameraDirection));
            JVector cameraUp = JVector.Cross(cameraDirection, cameraRight);

            JVector cameraFocusVector = t.position - cameraTarget;

            if (cameraDirection.Y < -.9900f && pitch > 0) pitch = 0;
            if (cameraDirection.Y > .9900f && pitch < 0) pitch = 0;

            JMatrix pitchMat = JMatrix.CreateFromAxisAngle(cameraRight, pitch);
            JMatrix yawMat = JMatrix.CreateFromAxisAngle(worldUp, yaw);

            JVector vecOut;
            JVector.Transform(ref cameraFocusVector, ref yawMat, out vecOut);
            JVector.Transform(ref vecOut, ref pitchMat, out vecOut);

            t.position = vecOut + cameraTarget;
            UpdateViewMatrix();
        }
        public void AddRadius(float f)
        {
            float v = Vector3.Distance(Convert.ToTKVec3(t.position), Convert.ToTKVec3(cameraTarget));
            if (v + f < .25f && f < 0) return;

            cameraRadius += f;
            t.position = (JVector.Normalize(t.position - cameraTarget) * cameraRadius) + cameraTarget;
            UpdateViewMatrix();
        }
        public void MoveAbsolute(JVector v)
        {
            cameraTarget += v;
            t.position += v;
            UpdateViewMatrix();
        }
        public void MoveRelativeToScreen(float lr, float ud)
        {
            //get roation
            JVector rot = JVector.Normalize((t.position - cameraTarget));
            Matrix4 la = Matrix4.LookAt(Convert.ToTKVec3(t.position), Convert.ToTKVec3(cameraTarget), Vector3.UnitY);

            JVector right = new JVector(la.M11, 0, la.M31);
            JVector up = new JVector(la.M12, la.M22, la.M32);
            right.Normalize();
            up.Normalize();

            JVector moveLR = JVector.Normalize(new JVector(right.X, 0, right.Z)) * lr;
            JVector moveUD = JVector.Normalize(new JVector(up.X, 0, up.Z)) * ud;
            JVector move = moveLR + moveUD;

            cameraTarget += move * MoveSpeed;
            t.position += move * MoveSpeed;

            UpdateViewMatrix();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Update fucntions.
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Update()
        {
            //if (!Editor.mouseInBounds) return;

            //AddRadius(Input.mouseDeltaPos.Z / -10);
            ////left shift not pressed
            ////mmb pressed
            //if (!Input.KeyIsDown(Key.LeftShift) && Input.MMBPressed)
            //{
            //    Rotate(Input.mouseDeltaPos.X / 150F, Input.mouseDeltaPos.Y / 150F);
            //}

            ////left shift pressed
            ////mmb pressed, alt not pressed
            //if (Input.KeyIsDown(Key.LeftShift) && Input.MMBPressed && !Input.KeyIsDown(Key.LeftAlt))
            //{
            //    Editor.loopCursorInBounds = true;
            //    Editor.Update();
            //    MoveRelativeToScreen(-Input.mouseDeltaPos.X * 5F, Input.mouseDeltaPos.Y * 5F);
            //    Editor.loopCursorInBounds = false;
            //}
            ////mmb pressed, alt pressed
            //if (Input.KeyIsDown(Key.LeftShift) && Input.MMBPressed && Input.KeyIsDown(Key.LeftAlt))
            //{
            //    Editor.loopCursorInBounds = true;
            //    Editor.Update();
            //    MoveRelativeToScreen(-Input.mouseDeltaPos.X, Input.mouseDeltaPos.Y);
            //    Editor.loopCursorInBounds = false;
            //}

        }
        float dimX = 1, dimY = 1;
        public void SetScreenDims(int x, int y)
        {
            dimX = x; dimY = y;
        }
        public void SetModelMatrix(Matrix4 m)
        {
            mData[2] = m;
        }
        public void UpdateProjMatrix()
        {
            mData[0] = Matrix4.CreatePerspectiveFieldOfView(1.22173f,
                dimX / dimY, 
                0.01F, 10000f);
        }
        public void UpdateViewMatrix()
        {
            mData[1] = Matrix4.LookAt(
                Convert.ToTKVec3(t.position),
                Convert.ToTKVec3(cameraTarget),
                new Vector3(0, 1, 0));
        }
        public void UpdateCameraBuffer()
        {
            GL.BindBuffer(BufferTarget.UniformBuffer, mDataBuff);
            GL.BufferSubData(BufferTarget.UniformBuffer, (IntPtr)0, Marshal.SizeOf(new Matrix4()) * 3, mData);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }
        public void UpdateColorBuffer(float r, float g, float b, float a)
        {
            colorData[0] = r; colorData[1] = g; colorData[2] = b; colorData[3] = a;
            GL.BindBuffer(BufferTarget.UniformBuffer, colorBuff);
            GL.BufferSubData(BufferTarget.UniformBuffer, (IntPtr)0, 16, colorData);
            GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
