using ImGuiNET;
using ShaderTest.Shaders;
using ShaderTest.UI;

namespace ShaderTest.Updatables
{
    public class Camera : Updatable, IHasUi
    {
        private readonly float _cameraSpeed = 5.0f;
        private float _gamma = 2.2f;
        private float _exposure = 1.0f;
        private float _fov = 60f;
        private Vector3 _cameraDir;
        private bool _drawDeferred = true;

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }
        public Vector3 Position { get; private set; }
        public float Gamma => _gamma;
        public float Exposure => _exposure;
        public string Name => "Camera";
        public bool DrawDeferred => _drawDeferred;
        public float NearClip => 0.1f;
        public float FarClip => 200f;

        public Camera(ShaderTestGame game) : base(game)
        {
            View = Matrix.Identity;
            Projection = Matrix.Identity;

            Position = new Vector3(10);
            _cameraDir = Vector3.Normalize(-Position);
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

                var appliedSpeed = _cameraSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (currentKb.IsKeyDown(Keys.W))
                {
                    Position += (_cameraDir * appliedSpeed);
                }

                if (currentKb.IsKeyDown(Keys.S))
                {
                    Position -= (_cameraDir * appliedSpeed);
                }

                if (currentKb.IsKeyDown(Keys.A))
                {
                    Position += (cameraLeft * appliedSpeed);
                }

                if (currentKb.IsKeyDown(Keys.D))
                {
                    Position -= (cameraLeft * appliedSpeed);
                }
            }

            View = Matrix.CreateLookAt(Position, Position + _cameraDir, Vector3.Up);
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fov), Game.GraphicsDevice.Viewport.AspectRatio, NearClip, FarClip);
        }

        public void RenderUi()
        {
            ImGui.SliderFloat("Field of view", ref _fov, 10f, 120f);
            ImGui.SliderFloat("Gamma", ref _gamma, 1.0f, 5.0f);
            ImGui.SliderFloat("Exposure", ref _exposure, 0.1f, 10f);
            
            if (ImGui.CollapsingHeader("Shader"))
            {
                ImGui.Checkbox("Deferred", ref _drawDeferred);

                BaseEffect effect = GameShaders.Pbr;

                if (_drawDeferred)
                {
                    effect = GameShaders.PbrDeferred;
                }

                if (ImGui.BeginCombo("Technique", effect.CurrentTechnique.Name))
                {
                    foreach (var technique in effect.Techniques)
                    {
                        if (ImGui.Selectable(technique.Name, effect.CurrentTechnique == technique))
                        {
                            effect.CurrentTechnique = technique;
                        }
                    }
                    ImGui.EndCombo();
                }

            }
        }
    }
}
