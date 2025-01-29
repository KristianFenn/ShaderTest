using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace ShaderTest
{
    public class Game1 : Game
    {
        private const float _floorSize = 20f;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Model _sphere;
        private Model _teapot;
        private Effect _effect;
        private Effect _depthEffect;
        private SpriteFont _arial;
        private VertexBuffer _floorVertexBuffer;
        private IndexBuffer _floorIndexBuffer;
        private Vector3 _cameraPos;
        private Vector3 _lightPos;
        private Matrix _view;
        private Matrix _projection;
        private Matrix _lightView;
        private Matrix _worldToLight;
        private Matrix _worldToScreen;
        private Vector3 _lightPosInView;
        private Matrix _lightProjection;
        private Vector3 _lightRotateAxis;
        private Matrix _floorWorld;
        private Matrix _teapotWorld;
        private Matrix[] _sphereWorld;
        private Color[] _sphereColors;

        private RenderTarget2D _shadowMap;
        private BasicEffect _lineEffect;
        private KeyboardState _lastKb;
        private MouseState _lastMouse;

        public Game1()
        {
            _graphics = new(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _sphere = Content.Load<Model>("IcoSphere");
            _teapot = Content.Load<Model>("Teapot");
            _effect = Content.Load<Effect>("Shaders/Test");
            _depthEffect = Content.Load<Effect>("Shaders/Depth");
            _arial = Content.Load<SpriteFont>("Arial");

            _floorVertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColorNormal), 4, BufferUsage.WriteOnly);
            _floorIndexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);

            _floorVertexBuffer.SetData([
                new VertexPositionColorNormal(new Vector3(0, 0, 0), Color.Gray, Vector3.Up),
                new VertexPositionColorNormal(new Vector3(1, 0, 0), Color.Gray, Vector3.Up),
                new VertexPositionColorNormal(new Vector3(1, 0, 1), Color.Gray, Vector3.Up),
                new VertexPositionColorNormal(new Vector3(0, 0, 1), Color.Gray, Vector3.Up),
            ]);

            _floorIndexBuffer.SetData<short>([
                0, 1, 2,
                0, 2, 3
            ]);

            var halfFloor = _floorSize / 2;

            _floorWorld = Matrix.CreateScale(_floorSize)
                * Matrix.CreateTranslation(-halfFloor, -5f, -halfFloor);

            _teapotWorld = Matrix.CreateScale(2f) *
                Matrix.CreateTranslation(0f, -5f, 0f);

            _sphereWorld = [
                Matrix.CreateTranslation( 0f,  0f,  0f) * Matrix.CreateRotationZ(1f),
                Matrix.CreateTranslation( 2f,  0f,  2f),
                Matrix.CreateTranslation(-4f,  0f,  0f),
                Matrix.CreateTranslation( 0f,  2f, -2f),
                Matrix.CreateTranslation(-2f, -2f,  0f),
            ];

            _sphereColors = [
                Color.Red,
                Color.Green,
                Color.Blue,
                Color.Orange,
                Color.Orchid
            ];

            _cameraPos = new Vector3(3);
            _lightPos = new Vector3(10);

            _view = Matrix.CreateLookAt(_cameraPos, Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), GraphicsDevice.Viewport.AspectRatio, 1f, 200f);

            _lightView = Matrix.CreateLookAt(_lightPos, Vector3.Zero, Vector3.Up);
            _lightProjection = Matrix.CreateOrthographic(32, 32, 0.1f, 200f);
            _lightRotateAxis = Vector3.Normalize(Vector3.Cross(-_lightPos, Vector3.Up));

            _shadowMap = new RenderTarget2D(GraphicsDevice, 2048, 2048, false, SurfaceFormat.Single, DepthFormat.None);

            _lineEffect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true
            };
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var currentKb = Keyboard.GetState();
            var currentMouse = Mouse.GetState();

            if (currentKb.IsKeyDown(Keys.LeftAlt))
            {
                var moveScaled = (currentMouse.Position.ToVector2() - _lastMouse.Position.ToVector2()) * 0.01f;

                var cameraLeft = Vector3.Normalize(Vector3.Cross(Vector3.Up, -_cameraPos));

                var rotate = Quaternion.CreateFromAxisAngle(Vector3.Up, moveScaled.X)
                    * Quaternion.CreateFromAxisAngle(cameraLeft, -moveScaled.Y);

                _cameraPos = Vector3.Transform(_cameraPos, rotate);
            }

            if (currentKb.IsKeyDown(Keys.S))
            {
                var posNormalised = Vector3.Normalize(_cameraPos);

                _cameraPos += (posNormalised * 0.1f);
            }

            if (currentKb.IsKeyDown(Keys.W))
            {
                var posNormalised = Vector3.Normalize(_cameraPos);

                _cameraPos -= (posNormalised * 0.1f);
            }

            if (currentKb.IsKeyDown(Keys.A))
            {

                var lightRotateAmount = -0.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                var lightRotate = Quaternion.CreateFromAxisAngle(_lightRotateAxis, lightRotateAmount);

                _lightPos = Vector3.Transform(_lightPos, lightRotate);
            }

            if (currentKb.IsKeyDown(Keys.D))
            {

                var lightRotateAmount = 0.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                var lightRotate = Quaternion.CreateFromAxisAngle(_lightRotateAxis, lightRotateAmount);

                _lightPos = Vector3.Transform(_lightPos, lightRotate);
            }

            _view = Matrix.CreateLookAt(_cameraPos, Vector3.Zero, Vector3.Up);

            _lightView = Matrix.CreateLookAt(_lightPos, Vector3.Zero, Vector3.Up);

            _lightPosInView = Vector3.Normalize(Vector3.TransformNormal(_lightPos, _view));

            _worldToLight = _lightView * _lightProjection;
            _worldToScreen = _view * _projection;

            _lastKb = currentKb;
            _lastMouse = currentMouse;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            GraphicsDevice.SetRenderTarget(_shadowMap);
            GraphicsDevice.Clear(Color.White);

            // Shadow map
            //DrawSpheres(_depthEffect, _depthEffect.Techniques["RenderDepth"]);
            DrawTeapot(_depthEffect, _depthEffect.Techniques["RenderDepth"]);

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.Clear(Color.Black);

            _effect.Parameters["ShadowMapSampler+ShadowMap"].SetValue(_shadowMap);
            _effect.Parameters["LightPositionInViewSpace"].SetValue(_lightPosInView);

            DrawTeapot(_effect, _effect.Techniques["DrawShaded"]);
            //DrawSpheres(_effect, _effect.Techniques["DrawShaded"]);
            DrawFloor(_effect, _effect.Techniques["DrawShaded"]);

            _spriteBatch.Begin();

            _spriteBatch.Draw(_shadowMap, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 0.1f, SpriteEffects.None, 1f);
            //_spriteBatch.DrawString(_arial, _lightPosInView.ToString(), new Vector2(10f, 210f), Color.White);

            _spriteBatch.End();

            VertexPositionColor[] lightToOriginVertices = [
                new VertexPositionColor(Vector3.Zero, Color.Yellow),
                new VertexPositionColor(_lightPos, Color.Yellow),
            ];

            _lineEffect.View = _view;
            _lineEffect.Projection = _projection;

            foreach (var pass in _lineEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, lightToOriginVertices, 0, 1);
            }

            base.Draw(gameTime);
        }

        private void DrawFloor(Effect effect, EffectTechnique effectTechnique)
        {
            GraphicsDevice.SetVertexBuffer(_floorVertexBuffer);
            GraphicsDevice.Indices = _floorIndexBuffer;

            effect.Parameters["Color"].SetValue(Color.Gray.ToVector4());
            effect.Parameters["ModelToLight"].SetValue(_floorWorld * _worldToLight);
            effect.Parameters["ModelToScreen"].SetValue(_floorWorld * _worldToScreen);
            effect.Parameters["ModelToView"].SetValue(_floorWorld * _view);
            effect.Parameters["NormalToView"].SetValue(CalculateNormalMatrix(_floorWorld * _view));

            foreach (var pass in effectTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            }
        }

        private void DrawTeapot(Effect effect, EffectTechnique effectTechnique)
        {
            effect.Parameters["ModelToLight"].SetValue(_teapotWorld * _worldToLight);

            if (effect.Name == "Shaders/Test")
            {
                effect.Parameters["Color"].SetValue(Color.Red.ToVector4());
                effect.Parameters["ModelToView"].SetValue(_teapotWorld * _view);
                effect.Parameters["ModelToScreen"].SetValue(_teapotWorld * _worldToScreen);
                effect.Parameters["NormalToView"].SetValue(CalculateNormalMatrix(_teapotWorld * _view));
            }

            foreach (var mesh in _teapot.Meshes.SelectMany(m => m.MeshParts))
            {
                GraphicsDevice.SetVertexBuffer(mesh.VertexBuffer);
                GraphicsDevice.Indices = mesh.IndexBuffer;

                foreach (var pass in effectTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, mesh.VertexOffset, mesh.StartIndex, mesh.PrimitiveCount);
                }
            }
        }

        private static Matrix CalculateNormalMatrix(Matrix worldToView)
        {
            worldToView.Translation = Vector3.Zero;
            return Matrix.Transpose(Matrix.Invert(worldToView));
        }

        private void DrawSpheres(Effect effect, EffectTechnique effectTechnique)
        {
            for (int i = 0; i < _sphereWorld.Length; i++)
            {
                effect.Parameters["ModelToLight"].SetValue(_sphereWorld[i] * _worldToLight);

                if (effect.Name == "Shaders/Test")
                {
                    effect.Parameters["Color"].SetValue(_sphereColors[i].ToVector4());
                    effect.Parameters["ModelToView"].SetValue(_sphereWorld[i] * _view);
                    effect.Parameters["ModelToScreen"].SetValue(_sphereWorld[i] * _worldToScreen);
                    effect.Parameters["NormalToView"].SetValue(CalculateNormalMatrix(_sphereWorld[i] * _view));
                }

                foreach (var mesh in _sphere.Meshes.SelectMany(m => m.MeshParts))
                {
                    GraphicsDevice.SetVertexBuffer(mesh.VertexBuffer);
                    GraphicsDevice.Indices = mesh.IndexBuffer;

                    foreach (var pass in effectTechnique.Passes)
                    {
                        pass.Apply();
                        GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, mesh.VertexOffset, mesh.StartIndex, mesh.PrimitiveCount);
                    }
                }
            }
        }
    }
}
