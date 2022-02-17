using Jitter.LinearMath;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMHEditor.DockingModules.MapEditor
{
    public class Transform
    {
        public Transform()
        {
            position = new JVector(0, 0, 0);
            eulerAngles = new JVector(0, 0, 0);
        }
        public JVector position;
        public JVector eulerAngles;
        public Matrix4 GetModelMatrix()
        {
            Matrix4 mt, mr;

            Vector3 pos = Convert.ToTKVec3(position);
            Matrix4.CreateTranslation(ref pos, out mt);

            Quaternion rot = Quaternion.FromEulerAngles(eulerAngles.X, eulerAngles.Y, eulerAngles.Z);
            Matrix4.CreateFromQuaternion(ref rot, out mr);

            return mt * mr;
        }

        public void Translate(float x, float y, float z)
        {
            position += new JVector(x, y, z);
        }
        public void Rotate(float x, float y, float z)
        {
            eulerAngles += new JVector(x, y, z);
        }
    }
}
