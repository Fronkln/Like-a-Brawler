using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public class CoordinateMatrix
    {
        public Vector4 LeftDirection = new Vector4(1, 0, 0, 0);
        public Vector4 UpDirection = new Vector4(0, 1, 0, 0);
        public Vector4 ForwardDirection = new Vector4(0, 0, 1, 0);
        public Vector4 Coordinates = new Vector4(0, 0, 0, 1);

        public class Vector4
        {
            public float x, y, z, w;

            public Vector4(float x, float y, float z, float w)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.w = w;
            }
        }
    }
}
