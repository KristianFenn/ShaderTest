using ImGuiNET;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Updatables
{
    public class Sun(ShaderTestGame game) : Updatable(game), IHasUi
    {
        public Vector3 Position { get; private set; }
        public Matrix View { get; private set; } = Matrix.Identity;
        public Matrix Projection { get; private set; } = Matrix.CreateOrthographic(48, 48, 0.1f, 200f);

        private const float MinutesPerDay = 1440f;

        private bool _runDayCycle = true;
        private float _timeOfDay = MinutesPerDay / 2;
        private float _dayLengthSeconds = 300f;
        private Vector3 _midnightPos = new(0, -10, 0);
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

        public void RenderUi()
        {
            ImGui.Begin("Sunlight");
            ImGui.SliderFloat("Day length", ref _dayLengthSeconds, 10f, 600f);
            ImGui.SliderFloat("Time of day", ref _timeOfDay, 0f, MinutesPerDay);
            ImGui.Checkbox("Run day cycle", ref _runDayCycle);
            ImGui.End();
        }
    }
}
