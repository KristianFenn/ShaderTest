using ImGuiNET;
using ShaderTest.Extensions;
using ShaderTest.Shaders;
using ShaderTest.UI;

namespace ShaderTest.Entities
{
    public class Skybox : IHasUi
    {
        private VertexBuffer _cubeVertices;
        private IndexBuffer _cubeIndices;
        private System.Numerics.Vector3 _scatteringCoefficients = new(1.0f, 0.7f, 0.5f);
        private Matrix _skyboxWorld;
        private float _falloff = 2.0f;

        public string UiSectionName => "Skybox";

        public void Initialise(GraphicsDevice graphicsDevice)
        {
            _cubeVertices = new VertexBuffer(graphicsDevice, typeof(VertexPosition), 8, BufferUsage.WriteOnly);
            _cubeIndices = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 36, BufferUsage.WriteOnly);

            _cubeVertices.SetData([
                // TOP
                new VertexPosition(new(-0.5f,  0.5f, -0.5f)),
                new VertexPosition(new( 0.5f,  0.5f, -0.5f)),
                new VertexPosition(new( 0.5f,  0.5f,  0.5f)),
                new VertexPosition(new(-0.5f,  0.5f,  0.5f)),

                // BOTTOM
                new VertexPosition(new(-0.5f, -0.5f, -0.5f)),
                new VertexPosition(new( 0.5f, -0.5f, -0.5f)),
                new VertexPosition(new( 0.5f, -0.5f,  0.5f)),
                new VertexPosition(new(-0.5f, -0.5f,  0.5f)),
            ]);

            _cubeIndices.SetData<short>([
                // TOP
                0, 3, 2, 0, 2, 1,
                // LEFT
                0, 3, 7, 0, 7, 4,
                // BACK
                0, 4, 5, 0, 5, 1,
                // RIGHT
                2, 5, 1, 2, 6, 5,
                // FRONT
                2, 3, 7, 2, 7, 6,
                // BOTTOM
                5, 4, 7, 5, 7, 6
            ]);

            _skyboxWorld = Matrix.CreateScale(100f);
        }

        public void Draw(GraphicsDevice graphicsDevice, RenderContext context)
        {
            graphicsDevice.SetVertexBuffer(_cubeVertices);
            graphicsDevice.Indices = _cubeIndices;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;

            GameShaders.Atmosphere.ScaterringCoefficients = _scatteringCoefficients.ToMonoGame();
            GameShaders.Atmosphere.ScatterFalloff = _falloff;
            GameShaders.Atmosphere.ApplyRenderContext(_skyboxWorld, context, default);
            GameShaders.Atmosphere.CurrentTechnique.Passes[0].Apply();

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 12);
        }

        public void RenderUi()
        {
            ImGui.SliderFloat3("Scaterring coefficients", ref _scatteringCoefficients, 0.0f, 1.0f);
            ImGui.SliderFloat("Scaterring falloff", ref _falloff, 1.0f, 20.0f);
        }
    }
}
