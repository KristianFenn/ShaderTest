using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Updatables
{
    public class Sun(Game game) : Updatable(game)
    {
        public Vector3 Position { get; private set; } = new Vector3(1, 10, 0);
        public Matrix View { get; private set; } = Matrix.Identity;
        public Matrix Projection { get; private set; } = Matrix.CreateOrthographic(48, 48, 0.1f, 200f);

        private const float SecondsPerDay = 300f;
        private static readonly Vector3 RotateAxis = Vector3.Normalize(Vector3.Left + Vector3.Forward);

        public override void Update(GameTime gameTime)
        {
            var lightRotateAmount = (MathHelper.TwoPi / SecondsPerDay) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            var lightRotate = Quaternion.CreateFromAxisAngle(RotateAxis, lightRotateAmount);

            Position = Vector3.Transform(Position, lightRotate);
            View = Matrix.CreateLookAt(Position, Vector3.Zero, Vector3.Up);
        }
    }
}
