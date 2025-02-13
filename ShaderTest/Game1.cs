using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ShaderTest.Entities;
using ShaderTest.Shaders;
using ShaderTest.Updatables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShaderTest
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ShadedEffect _shadedEffect;
        private ShadowMapEffect _shadowMapEffect;
        private SpriteFont _arial;

        private RenderTarget2D _shadowMap;

        private List<ModelEntity> _entities;
        private GameStats _stats;
        private Sun _sun;
        private List<Updatable> _updatable;
        private Camera _camera;

        public Game1()
        {
            _graphics = new(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 2560;
            _graphics.PreferredBackBufferHeight = 1440;
            _graphics.IsFullScreen = true;
            _graphics.HardwareModeSwitch = false;
            _graphics.PreferMultiSampling = true;
            _graphics.SynchronizeWithVerticalRetrace = true;
            
            IsMouseVisible = false;
            IsFixedTimeStep = false;

            _graphics.PreparingDeviceSettings += (object sender, PreparingDeviceSettingsEventArgs e) =>
            {
                e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 16;
            };
        }

        protected override void Initialize()
        {
            base.Initialize();
            _stats = new GameStats(GraphicsDevice);
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _shadedEffect = new ShadedEffect(Content.Load<Effect>("Shaders/Test"));
            _shadowMapEffect = new ShadowMapEffect(Content.Load<Effect>("Shaders/Depth"));
            _arial = Content.Load<SpriteFont>("Arial");
            _shadowMapEffect.CurrentTechnique = _shadowMapEffect.Techniques["RenderDepth"];

            _entities = [];
            _updatable = [];

            _shadowMap = new RenderTarget2D(GraphicsDevice, 4096, 4096, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

            _entities.Add(new GroundEntity(Content));
            _entities.Add(new CampfireEntity(Content));
            _entities.Add(new CarEntity(Content));
            _entities.Add(new TentEntity(Content));

            _sun = new Sun(this);
            _updatable.Add(_sun);

            _camera = new Camera(this);
            _updatable.Add(_camera);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var updateable in _updatable)
            {
                updateable.Update(gameTime);
            }

            _stats.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;

            // Shadow map
            GraphicsDevice.SetRenderTarget(_shadowMap);
            GraphicsDevice.Clear(Color.White);

            var renderContext = new RenderContext(_camera.View, _camera.Projection, _sun.View, _sun.Projection, _sun.Position, _shadowMap);

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
                entity.Draw(GraphicsDevice, _shadedEffect, renderContext);
            }

            _spriteBatch.Begin();
            _stats.Draw(gameTime, _spriteBatch, GraphicsDevice, _arial);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
