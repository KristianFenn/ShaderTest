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
        private Matrix _pointLightView;
        private Matrix _pointLightProjection;

        private RenderTarget2D _shadowMap;
        private RenderTarget2D _cubeMap;

        private List<SimpleEntity> _entities;
        private Quaternion _lightRotatePerSec;

        public Game1()
        {
            _graphics = new(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferMultiSampling = true;
            IsMouseVisible = false;

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
            _lightPos = new Vector3(1, 10, 0);
            _pointLightPos = new Vector3(5);

            _view = Matrix.CreateLookAt(_cameraPos, Vector3.Zero, Vector3.Up);
            _projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60f), GraphicsDevice.Viewport.AspectRatio, 0.1f, 200f);

            _lightView = Matrix.CreateLookAt(_lightPos, Vector3.Zero, Vector3.Up);
            _lightProjection = Matrix.CreateOrthographic(48, 48, 0.1f, 200f);

            _pointLightView = Matrix.CreateLookAt(_pointLightPos, Vector3.Zero, Vector3.Up);
            _pointLightProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi, 1f, 1f, 200f);

            _shadowMap = new RenderTarget2D(GraphicsDevice, 4096, 4096, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

            var lightRotateRadsPerSec = (MathHelper.TwoPi / 300f);
            _lightRotatePerSec = Quaternion.CreateFromAxisAngle(Vector3.Left + Vector3.Forward, lightRotateRadsPerSec);

            //InitialiseTeapot();
            InitialiseGround();
            InitialiseCampfire();
            InitialiseCar();
            InitialiseTent();
            //InitialiseCabinet();
            //InitialiseSpheres();
        }


        private void InitialiseGround()
        {
            var model = Content.Load<Model>("Models/Ground/Ground");

            var ground = new SimpleEntity(model, Matrix.CreateScale(2f) * Matrix.CreateTranslation(3f, -2f, 0f), true, Color.White, 0.1f, Color.Brown);

            ground.Textures.Add("Ground", Content.Load<Texture2D>("Models/Ground/Ground.Color"));

            _entities.Add(ground);
        }

        private void InitialiseCar()
        {
            var model = Content.Load<Model>("Models/Car/Car");

            var car = new SimpleEntity(model, Matrix.CreateScale(0.4f) * Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(-20f)) * Matrix.CreateTranslation(5f, -2f, 0f), true, Color.White, 1f);

            car.Textures.Add("Car", Content.Load<Texture2D>("Models/Car/Car.Color"));
            car.Textures.Add("Wheel.FL", Content.Load<Texture2D>("Models/Car/Tyre.Color"));

            _entities.Add(car);
        }

        private void InitialiseTent()
        {
            var model = Content.Load<Model>("Models/Tent/Tent");

            var tent = (new SimpleEntity(model,
                Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(120f))
                * Matrix.CreateTranslation(3f, -2f, 4f),
                true, Color.White, 0.3f));

            tent.Textures.Add("Tent", Content.Load<Texture2D>("Models/Tent/Tent.Color"));
            tent.Textures.Add("Rope.R", Content.Load<Texture2D>("Models/Tent/Tent.Color"));

            _entities.Add(tent);
        }

        private void InitialiseCampfire()
        {
            var model = Content.Load<Model>("Models/Campfire/Campfire");
            var campfire = new SimpleEntity(model, Matrix.CreateTranslation(0f, -2f, 0f), true, Color.White, 0.1f);

            var campfireTexture = Content.Load<Texture2D>("Models/Campfire/Campfire.Color");

            campfire.Textures.Add("Stones", campfireTexture);
            campfire.Textures.Add("Logs", campfireTexture);
            campfire.Textures.Add("Ash", campfireTexture);

            _entities.Add(campfire);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var currentKb = Keyboard.GetState();
            var currentMouse = Mouse.GetState();

            if (IsActive)
            {
                var moveScaled = (currentMouse.Position.ToVector2() - new Vector2(100f)) * 0.005f;
                Mouse.SetPosition(100, 100);

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
            }

            _lightPos = Vector3.Transform(_lightPos, _lightRotatePerSec * (float)gameTime.ElapsedGameTime.TotalSeconds);

            _view = Matrix.CreateLookAt(_cameraPos, _cameraPos + _cameraDir, Vector3.Up);
            _lightView = Matrix.CreateLookAt(_lightPos, Vector3.Zero, Vector3.Up);

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
                _shadedEffect.SpecularColor = entity.SpecularColor;
                _shadedEffect.SpecularPower = entity.SpecularPower;

                entity.Draw(GraphicsDevice, _shadedEffect, renderContext);
            }

            base.Draw(gameTime);
        }
    }
}
