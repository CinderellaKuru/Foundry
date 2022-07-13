using Jitter.LinearMath;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector3 = BEPUutilities.Vector3;

namespace SMHEditor.DockingModules.MapEditor
{
    public class Transform
    {
        public Transform()
        {
            position = new Vector3(0, 0, 0);
            eulerAngles = new Vector3(0, 0, 0);
            scale = new Vector3(1, 1, 1);
        }
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale;
        public Matrix4 GetModelMatrix()
        {
            Matrix4 mt, mr, ms;

            OpenTK.Vector3 scl = new OpenTK.Vector3(scale.X, scale.Y, scale.Z);
            Matrix4.CreateScale(ref scl, out ms);

            Quaternion rot = Quaternion.FromEulerAngles(eulerAngles.X, eulerAngles.Y, eulerAngles.Z);
            Matrix4.CreateFromQuaternion(ref rot, out mr);

            OpenTK.Vector3 pos = new OpenTK.Vector3(position.X, position.Y, position.Z);
            Matrix4.CreateTranslation(ref pos, out mt);

            return ms * mr * mt;
        }

        public void Translate(float x, float y, float z)
        {
            position += new Vector3(x, y, z);
        }
        public void Rotate(float x, float y, float z)
        {
            eulerAngles += new Vector3(x, y, z);
        }
    }
}
