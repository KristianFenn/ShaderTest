using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShaderTest.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShaderTest
{
    public class Game1 : Game
    {
        private const float _floorSize = 20f;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ShadedEffect _shadedEffect;
        private ShadowMapEffect _shadowMapEffect;
        private SpriteFont _arial;
        private VertexBuffer _floorVertexBuffer;
        private IndexBuffer _floorIndexBuffer;
        private Vector3 _cameraPos;
        private Vector3 _lightPos;
        private Matrix _view;
        private Matrix _projection;
        private Matrix _lightView;
        private Matrix _lightProjection;
        private Vector3 _lightRotateAxis;
        private Matrix _floorWorld;

        private RenderTarget2D _shadowMap;
        private BasicEffect _lineEffect;
        private KeyboardState _lastKb;
        private MouseState _lastMouse;

        private List<SimpleEntity> _entities;

        public Game1()
        {
            _graphics = new(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.PreferMultiSampling = true;

            _graphics.PreparingDeviceSettings += (object sender, PreparingDeviceSettingsEventArgs e) =>
            {
                e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 16;
            };
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var sphere = Content.Load<Model>("UVSphere");
            var teapot = Content.Load<Model>("Teapot");
            var cabinet = Content.Load<Model>("Cabinet");
            _shadedEffect = new ShadedEffect(Content.Load<Effect>("Shaders/Test"));
            _shadowMapEffect = new ShadowMapEffect(Content.Load<Effect>("Shaders/Depth"));
            _arial = Content.Load<SpriteFont>("Arial");

            _shadedEffect.Technique = ShadedEffectTechniques.DrawShaded;
            _shadowMapEffect.CurrentTechnique = _shadowMapEffect.Techniques["RenderDepth"];

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

            _entities = new List<SimpleEntity>();

            _floorWorld = Matrix.CreateScale(_floorSize)
                * Matrix.CreateTranslation(-halfFloor, -5f, -halfFloor);

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

            //InitialiseTeapot(teapot);
            InitialiseCabinet(cabinet);
            //InitialiseSpheres(sphere);
        }

        private void InitialiseTeapot(Model teapotModel)
        {
            _entities.Add(new SimpleEntity(teapotModel, Matrix.CreateScale(2f) * Matrix.CreateTranslation(0f, -5f, 0f), true, Color.Cyan));
        }

        private void InitialiseCabinet(Model cabinetModel)
        {
            var effect = (BasicEffect)cabinetModel.Meshes[0].Effects[0];
            _entities.Add(new SimpleEntity(cabinetModel, Matrix.CreateScale(2f) * Matrix.CreateTranslation(0f, -2f, 0f), true, Color.White, effect.Texture));
        }

        private void InitialiseSpheres(Model sphereModel)
        {
            Matrix[] sphereWorld = [
                Matrix.CreateTranslation( 0f,  0f,  0f) * Matrix.CreateRotationZ(1f),
                Matrix.CreateTranslation( 2f,  0f,  2f),
                Matrix.CreateTranslation(-4f,  0f,  0f),
                Matrix.CreateTranslation( 0f,  2f, -2f),
                Matrix.CreateTranslation(-2f, -2f,  0f),
            ];

            Color[] sphereColors = [
                Color.Red,
                Color.Green,
                Color.Blue,
                Color.Orange,
                Color.Orchid
            ];

            for (int i = 0; i < sphereWorld.Length; i++)
            {
                _entities.Add(new SimpleEntity(sphereModel, sphereWorld[i], true, sphereColors[i]));
            }
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

            _lastKb = currentKb;
            _lastMouse = currentMouse;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            // Shadow map
            GraphicsDevice.SetRenderTarget(_shadowMap);
            GraphicsDevice.Clear(Color.White);

            var renderContext = new RenderContext(_view, _projection, _lightView, _lightProjection, _lightPos, _shadowMap);

            foreach (var entity in _entities)
            {
                if (!entity.IncludeInShadowMap) continue;

                entity.Draw(GraphicsDevice, _shadowMapEffect, renderContext);
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            GraphicsDevice.Clear(Color.Black);

            DrawFloor(_shadedEffect, renderContext);

            foreach (var entity in _entities)
            {
                _shadedEffect.Color = entity.Color;
                _shadedEffect.Texture = entity.Texture;

                if (_shadedEffect.Texture != null)
                {
                    _shadedEffect.Technique = ShadedEffectTechniques.DrawTextured;
                }
                else
                {
                    _shadedEffect.Technique = ShadedEffectTechniques.DrawShaded;
                }

                entity.Draw(GraphicsDevice, _shadedEffect, renderContext);
            }

            _spriteBatch.Begin();

            _spriteBatch.Draw(_shadowMap, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 0.1f, SpriteEffects.None, 1f);

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

        private void DrawFloor(ShadedEffect effect, RenderContext context)
        {
            GraphicsDevice.SetVertexBuffer(_floorVertexBuffer);
            GraphicsDevice.Indices = _floorIndexBuffer;

            effect.ApplyRenderContext(_floorWorld, context);
            effect.Technique = ShadedEffectTechniques.DrawShaded;

            effect.Parameters["Color"].SetValue(Color.Gray.ToVector4());

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 2);
            }
        }
    }
}
