using ImGuiNET;
using ShaderTest.UI;

namespace ShaderTest.Updatables
{
    public class Sun(ShaderTestGame game) : Updatable(game), IHasUi
    {
        public Vector3 SunColor => _sunColor * _sunBrightness;
        public Vector3 Position { get; private set; }
        public Matrix View { get; private set; } = Matrix.Identity;
        public Matrix Projection { get; private set; } = Matrix.CreateOrthographic(48, 48, 0.1f, 200f);

        private const float MinutesPerDay = 1440f;

        private bool _runDayCycle = true;
        private float _timeOfDay = MinutesPerDay / 2;
        private float _dayLengthSeconds = 300f;
        private Vector3 _midnightPos = new(0, -50f, 0);
        private System.Numerics.Vector3 _sunColor = new(0.978f, 0.888f, 0.866f);
        private float _sunBrightness = 24f;

        private static readonly Vector3 RotateAxis = Vector3.Normalize(Vector3.Left + Vector3.Forward);

        public override void Update(GameTime gameTime)
        {
            if (_runDayCycle)
            {
                _timeOfDay += (MinutesPerDay / _dayLengthSeconds) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                _timeOfDay %= MinutesPerDay;
            }

            var rotate = MathHelper.TwoPi * _timeOfDay / MinutesPerDay;
            var lightRotate = Quaternion.CreateFromAxisAngle(RotateAxis, rotate);

            Position = Vector3.Transform(_midnightPos, lightRotate);
            View = Matrix.CreateLookAt(Position, Vector3.Zero, Vector3.Up);
        }

        public string UiSectionName => "Light";
        public void RenderUi()
        {
            ImGui.ColorEdit3("Sun colour", ref _sunColor);
            ImGui.SliderFloat("Sun strength", ref _sunBrightness, 10.0f, 40.0f);
            ImGui.SliderFloat("Day length", ref _dayLengthSeconds, 10f, 600f);
            ImGui.SliderFloat("Time of day", ref _timeOfDay, 0f, MinutesPerDay);
            ImGui.Checkbox("Run day cycle", ref _runDayCycle);
        }
    }
}
