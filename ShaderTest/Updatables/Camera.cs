using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ShaderTest.Updatables
{
    public class Camera : Updatable
    {
        private readonly Vector2 MouseClampPos;
        private const float CameraSpeed = 5.0f;
        private Vector3 _cameraDir;
        private Vector3 _cameraPos;

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public Camera(ShaderTestGame game) : base(game)
        {
            View = Matrix.Identity;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), Game.GraphicsDevice.Viewport.AspectRatio, 0.1f, 200f);

            _cameraDir = new Vector3(3);
            _cameraDir = Vector3.Forward;

            var (w, h) = Game.GraphicsDevice.Viewport.Bounds.Size;

            MouseClampPos = new Vector2(w / 2, h / 2);
        }

        public override void Update(GameTime gameTime)
        {
            var currentKb = Keyboard.GetState();

            if (Game.IsActive)
            {
                var cameraLeft = Vector3.Normalize(Vector3.Cross(Vector3.Up, _cameraDir));
                Game.Mouse.LockMouse = false;

                if (currentKb.IsKeyDown(Keys.LeftAlt) && Game.Mouse.InScreenBounds)
                {
                    var moveScaled = Game.Mouse.Move * 0.005f;
                    Game.Mouse.LockMouse = true;
                    var rotate = Quaternion.CreateFromAxisAngle(Vector3.Up, -moveScaled.X)
                        * Quaternion.CreateFromAxisAngle(cameraLeft, moveScaled.Y);

                    _cameraDir = Vector3.Transform(_cameraDir, rotate);

                    Game.IsMouseVisible = false;
                }

                var appliedSpeed = CameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (currentKb.IsKeyDown(Keys.W))
                {
                    _cameraPos += (_cameraDir * appliedSpeed);
                }

                if (currentKb.IsKeyDown(Keys.S))
                {
                    _cameraPos -= (_cameraDir * appliedSpeed);
                }

                if (currentKb.IsKeyDown(Keys.A))
                {
                    _cameraPos += (cameraLeft * appliedSpeed);
                }

                if (currentKb.IsKeyDown(Keys.D))
                {
                    _cameraPos -= (cameraLeft * appliedSpeed);
                }
            }

            View = Matrix.CreateLookAt(_cameraPos, _cameraPos + _cameraDir, Vector3.Up);
        }
    }
}
