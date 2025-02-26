using ShaderTest.Updatables;

namespace ShaderTest
{
    public static class GameDebug
    {
        public static class Content
        {
            public static BasicEffect Effect { get; set; }
            public static SpriteFont Font { get; set; }
        }

        private static VertexBuffer _axesVertices;
        private static IndexBuffer _axesIndices;

        public static void Initialize(GraphicsDevice graphicsDevice, SpriteFont debugFont)
        {
            Content.Font = debugFont;

            Content.Effect = new(graphicsDevice)
            {
                VertexColorEnabled = true
            };

            _axesVertices = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), 63, BufferUsage.WriteOnly);

            _axesVertices.SetData([
                ..CalculateAxesVertices(Vector3.UnitX, Color.Red),
                ..CalculateAxesVertices(Vector3.UnitY, Color.Green),
                ..CalculateAxesVertices(Vector3.UnitZ, Color.RoyalBlue)
            ]);

            _axesIndices = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, 24, BufferUsage.WriteOnly);

            short[] axisIndices = [
                0,19,
                .. Enumerable.Range(1, 20).Select(idx => (short)idx)
            ];

            _axesIndices.SetData(axisIndices);
        }

        private static VertexPositionColor[] CalculateAxesVertices(Vector3 axisDirection, Color axisColor)
        {
            var axisTickDirection = new Vector3(axisDirection.Y, axisDirection.X + axisDirection.Z, 0f) * 0.1f;
            var vertices = new VertexPositionColor[21];
            vertices[0] = new VertexPositionColor(Vector3.Zero, axisColor);

            for (var i = 1; i <= 10; i++)
            {
                var vtxPos = axisDirection * i;
                var vtxIndex = i * 2 - 1;
                vertices[vtxIndex] = new VertexPositionColor(vtxPos, axisColor);
                vertices[vtxIndex + 1] = new VertexPositionColor(vtxPos + axisTickDirection, axisColor);
            }

            return vertices;
        }

        public static void DrawAxesToScreen(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, SpriteFont font, Camera camera)
        {
            graphicsDevice.DepthStencilState = DepthStencilState.None;

            graphicsDevice.SetVertexBuffer(_axesVertices);
            graphicsDevice.Indices = _axesIndices;
            Content.Effect.View = camera.View;
            Content.Effect.Projection = camera.Projection;

            foreach (EffectPass pass in Content.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0, 11);
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 21, 0, 11);
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 42, 0, 11);
            }

            void DrawAxisLabel(Vector3 axisDirection, Color axisColor, string axisName)
            {
                var axisNameSize = font.MeasureString(axisName);
                var offset = axisNameSize / 2;

                var axisOffset = graphicsDevice.Viewport.Project(axisDirection * 10.5f, camera.Projection, camera.View, Matrix.Identity);
                spriteBatch.DrawString(font, axisName, new Vector2(axisOffset.X, axisOffset.Y), axisColor, 0f, offset, 1f, SpriteEffects.None, 0);
            }

            DrawAxisLabel(Vector3.UnitX, Color.Red, "X");
            DrawAxisLabel(Vector3.UnitY, Color.Green, "Y");
            DrawAxisLabel(Vector3.UnitZ, Color.RoyalBlue, "Z");
        }
    }
}
