using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarotraumaGameSessionEditor
{
    public class Vector2D
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

        public override string ToString()
        {
            return X.ToString() + "," + Y.ToString();
        }
    }
}
