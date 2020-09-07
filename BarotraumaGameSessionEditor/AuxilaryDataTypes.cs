using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarotraumaGameSessionEditor
{
    public struct Vector2D
    {
        public float X;
        public float Y;

        public Vector2D(string PackedCoordinateString)
        {
            string[] SeparatedCoordinatesString = PackedCoordinateString.Split(',');

            this.X = float.Parse(SeparatedCoordinatesString[0]);
            this.Y = float.Parse(SeparatedCoordinatesString[1]);
        }

        public Vector2D(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public static Vector2D operator +(Vector2D A, Vector2D B) => new Vector2D(A.X + B.X, A.Y + B.Y);
        public static Vector2D operator -(Vector2D A, Vector2D B) => new Vector2D(A.X - B.X, A.Y - B.Y);
        public static Vector2D operator +(Vector2D A) => A;
        public static Vector2D operator -(Vector2D A) => new Vector2D(-A.X, -A.Y);

        public static Vector2D operator *(Vector2D A, Vector2D B) => new Vector2D(A.X * B.X, A.Y * B.Y);
        public static Vector2D operator /(Vector2D A, Vector2D B) => new Vector2D(A.X / B.X, A.Y / B.Y);
        public static Vector2D operator *(Vector2D A, float B) => new Vector2D(A.X * B, A.Y * B);
        public static Vector2D operator *(float A, Vector2D B) => B * A;
        public static Vector2D operator /(Vector2D A, float B) => new Vector2D(A.X / B, A.Y / B);

        public override string ToString()
        {
            return X.ToString() + "," + Y.ToString();
        }
    }

    public struct IntTuple
    {
        public int A, B;

        public IntTuple(int A, int B)
        {
            this.A = A;
            this.B = B;
        }

        public bool Contains(int Value)
        {
            return A == Value || B == Value;
        }

        public int GetOther(int Value)
        {
            System.Diagnostics.Debug.Assert(Contains(Value));

            if (A == Value)
            {
                return B;
            }

            return A;
        }
    }
}
