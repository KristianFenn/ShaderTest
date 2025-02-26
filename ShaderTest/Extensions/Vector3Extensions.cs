using MgVector3 = Microsoft.Xna.Framework.Vector3;
using NumericsVector3 = System.Numerics.Vector3;

namespace ShaderTest.Extensions
{
    public static class Vector3Extensions
    {
        public static MgVector3 ToMonoGame(this NumericsVector3 vector)
        {
            return new MgVector3(vector.X, vector.Y, vector.Z);
        }
    }
}
