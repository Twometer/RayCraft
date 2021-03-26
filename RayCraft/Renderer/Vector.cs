using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayCraft.Renderer
{
    public struct Vector
    {
        public float X;
        public float Y;
        public float Z;
        public float Length => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }


        public Vector Multiply(float v)
        {
            float nx = X * v;
            float ny = Y * v;
            float nz = Z * v;
            return new Vector(nx, ny, nz);
        }

        public void Add(Vector vec)
        {
            X += vec.X;
            Y += vec.Y;
            Z += vec.Z;
        }
    }
}
