using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Updatables
{
    public class Camera : Updatable
    {
        private Vector3 _cameraDir;
        private Vector3 _cameraPos;

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public Camera(Game game) : base(game)
        {
            View = Matrix.Identity;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), Game.GraphicsDevice.Viewport.AspectRatio, 0.1f, 200f);

            _cameraDir = new Vector3(3);
            _cameraDir = Vector3.Forward;
        }

        public override void Update(GameTime gameTime)
        {
            var currentMouse = Mouse.GetState();
            var currentKb = Keyboard.GetState();

            if (Game.IsActive)
            {
                var moveScaled = (currentMouse.Position.ToVector2() - new Vector2(100f)) * 0.005f;
                Mouse.SetPosition(100, 100);

                var cameraLeft = Vector3.Normalize(Vector3.Cross(Vector3.Up, _cameraDir));

                var rotate = Quaternion.CreateFromAxisAngle(Vector3.Up, -moveScaled.X)
                    * Quaternion.CreateFromAxisAngle(cameraLeft, moveScaled.Y);

                _cameraDir = Vector3.Transform(_cameraDir, rotate);


                if (currentKb.IsKeyDown(Keys.W))
                {
                    _cameraPos += (_cameraDir * 0.1f);
                }

                if (currentKb.IsKeyDown(Keys.S))
                {
                    _cameraPos -= (_cameraDir * 0.1f);
                }

                if (currentKb.IsKeyDown(Keys.A))
                {
                    _cameraPos += (cameraLeft * 0.1f);
                }

                if (currentKb.IsKeyDown(Keys.D))
                {
                    _cameraPos -= (cameraLeft * 0.1f);
                }
            }

            View = Matrix.CreateLookAt(_cameraPos, _cameraPos + _cameraDir, Vector3.Up);
        }
    }
}
