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
        private ShadedEffect _shadedEffect;
        private ShadowMapEffect _shadowMapEffect;
        private SpriteFont _arial;
        private Vector3 _cameraPos;
        private Vector3 _cameraDir;
        private Vector3 _lightPos;
        private Vector3 _pointLightPos;
        private Matrix _view;
        private Matrix _projection;
        private Matrix _lightView;
        private Matrix _lightProjection;
        private Vector3 _lightRotateAxis;
        private Matrix _pointLightView;
        private Matrix _pointLightProjection;
        private Matrix _floorWorld;

        private RenderTarget2D _shadowMap;
        private RenderTarget2D _cubeMap;
        private BasicEffect _lineEffect;
        private float _lightRotateAmountPerSec;
        private KeyboardState _lastKb;
        private MouseState _lastMouse;

        private List<SimpleEntity> _entities;

        public Game1()
        {
            _graphics = new(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
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
            _shadedEffect = new ShadedEffect(Content.Load<Effect>("Shaders/Test"));
            _shadowMapEffect = new ShadowMapEffect(Content.Load<Effect>("Shaders/Depth"));
            _arial = Content.Load<SpriteFont>("Arial");

            _shadedEffect.Technique = ShadedEffectTechniques.DrawShaded;
            _shadowMapEffect.CurrentTechnique = _shadowMapEffect.Techniques["RenderDepth"];

            _entities = [];

            _cameraPos = new Vector3(3);
            _cameraDir = Vector3.Forward;
            _lightPos = new Vector3(10);
            _pointLightPos = new Vector3(5);

            _view = Matrix.CreateLookAt(_cameraPos, Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), GraphicsDevice.Viewport.AspectRatio, 0.1f, 200f);

            _lightView = Matrix.CreateLookAt(_lightPos, Vector3.Zero, Vector3.Up);
            _lightProjection = Matrix.CreateOrthographic(48, 48, 0.1f, 200f);
            _lightRotateAxis = Vector3.Normalize(Vector3.Cross(-_lightPos, Vector3.Up));

            _pointLightView = Matrix.CreateLookAt(_pointLightPos, Vector3.Zero, Vector3.Up);
            _pointLightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi, 1f, 1f, 200f);

            _shadowMap = new RenderTarget2D(GraphicsDevice, 4096, 4096, false, SurfaceFormat.Single, DepthFormat.Depth24);

            _lineEffect = new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true
            };

            _lightRotateAmountPerSec = (MathHelper.TwoPi / 120f);

            //InitialiseTeapot();
            InitialiseGround();
            InitialiseCampfire();
            InitialiseTent();
            InitialiseCar();
            //InitialiseCabinet();
            //InitialiseSpheres();
        }

        private Model LoadWithTexture(string path, out Texture2D texture)
        {
            var model = Content.Load<Model>(path);
            texture = Content.Load<Texture2D>($"{path}.Color");
            return model;
        }

        private void InitialiseGround()
        {
            var model = LoadWithTexture("Models/Ground/Ground", out Texture2D texture);
            _entities.Add(new SimpleEntity(model, Matrix.CreateScale(2f) * Matrix.CreateTranslation(3f, -2f, 0f), true, Color.White, Color.Black, texture));
        }

        private void InitialiseCar()
        {
            var model = LoadWithTexture("Models/Car/Car", out Texture2D texture);
            _entities.Add(new SimpleEntity(model, Matrix.CreateScale(0.4f) * Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(-20f)) * Matrix.CreateTranslation(5f, -2f, 0f), true, Color.White, Color.White, texture));
        }

        private void InitialiseTent()
        {
            var model = LoadWithTexture("Models/Tent/Tent", out Texture2D texture);
            _entities.Add(new SimpleEntity(model, 
                Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(120f))
                * Matrix.CreateTranslation(3f, -2f, 4f), 
                true, Color.White, new Color(0.5f, 0.5f, 0.5f), texture));
        }

        private void InitialiseCampfire()
        {
            var model = LoadWithTexture("Models/Campfire/Campfire", out Texture2D texture);
            _entities.Add(new SimpleEntity(model, Matrix.CreateTranslation(0f, -2f, 0f), true, Color.White, Color.Black, texture));
        }


        private void InitialiseTeapot()
        {
            var teapotModel = Content.Load<Model>("Teapot");
            _entities.Add(new SimpleEntity(teapotModel, Matrix.CreateScale(2f) * Matrix.CreateTranslation(0f, -5f, 0f), true, Color.Cyan, Color.White));
        }

        private void InitialiseCabinet()
        {
            var cabinetModel = LoadWithTexture("Ground", out Texture2D cabinetTexture);
            _entities.Add(new SimpleEntity(cabinetModel, Matrix.CreateScale(2f) * Matrix.CreateTranslation(0f, -2f, 0f), true, Color.White, Color.White, cabinetTexture));
        }

        private void InitialiseSpheres()
        {
            var sphereModel = Content.Load<Model>("UVSphere");

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
                _entities.Add(new SimpleEntity(sphereModel, sphereWorld[i], true, sphereColors[i], Color.White));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var currentKb = Keyboard.GetState();
            var currentMouse = Mouse.GetState();

            var moveScaled = (currentMouse.Position.ToVector2() - _lastMouse.Position.ToVector2()) * 0.005f;

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

            var lightRotateAmount = _lightRotateAmountPerSec * (float)gameTime.ElapsedGameTime.TotalSeconds;

            var lightRotate = Quaternion.CreateFromAxisAngle(_lightRotateAxis, lightRotateAmount);

            _lightPos = Vector3.Transform(_lightPos, lightRotate);

            _view = Matrix.CreateLookAt(_cameraPos, _cameraPos + _cameraDir, Vector3.Up);
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
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            GraphicsDevice.Clear(Color.Black);

            foreach (var entity in _entities)
            {
                _shadedEffect.DiffuseColor = entity.DiffuseColor;
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

            base.Draw(gameTime);
        }
    }
}
