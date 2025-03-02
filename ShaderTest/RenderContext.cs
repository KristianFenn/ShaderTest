using ShaderTest.Updatables;

namespace ShaderTest
{
    public class RenderContext(Camera camera, Sun sun)
    {
        public Matrix View { get; } = camera.View;
        public Matrix Projection { get; } = camera.Projection;
        public Vector3 CameraPosition { get; } = camera.Position;
        public Vector3 LightColor { get; } = sun.SunColor;
        public float Gamma { get; } = camera.Gamma;
        public float Exposure { get; } = camera.Exposure;
        public float NearClip { get; set; } = camera.NearClip;
        public float FarClip { get; } = camera.FarClip;

        public Matrix WorldToScreen { get; } = camera.View * camera.Projection;
        public Matrix WorldToLight { get; } = sun.View * sun.Projection;
        public Vector3 LightPosition { get; } = Vector3.TransformNormal(sun.Position, camera.View);
    }
}
