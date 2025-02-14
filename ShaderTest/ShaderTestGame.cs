using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.ImGuiNet;
using ShaderTest.Entities;
using ShaderTest.Shaders;
using ShaderTest.Updatables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShaderTest
{
    public class ShaderTestGame : Game
    {
        public Camera Camera { get; private set; }
        public MouseInputHandler Mouse { get; private set; }
        public List<ModelEntity> Entities { get; private set; }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private ShadedEffect _shadedEffect;
        private ShadowMapEffect _shadowMapEffect;
        private SpriteFont _arial;

        private RenderTarget2D _shadowMap;
        private Texture2D _pixel;
        private GameStats _stats;
        private Sun _sun;
        private List<Updatable> _updatable;
        private List<IHasUi> _ui;
        private static ImGuiRenderer _imgui;

        public ShaderTestGame()
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
            _imgui = new ImGuiRenderer(this);
            _stats = new GameStats(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Entities = [];
            _updatable = [];
            _ui = [];

            _imgui.RebuildFontAtlas();

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _shadedEffect = new ShadedEffect(Content.Load<Effect>("Shaders/Test"));
            _shadowMapEffect = new ShadowMapEffect(Content.Load<Effect>("Shaders/Depth"));
            _arial = Content.Load<SpriteFont>("Arial");
            _shadowMapEffect.CurrentTechnique = _shadowMapEffect.Techniques["RenderDepth"];

            _ui.Add(_shadedEffect);

            _shadowMap = new RenderTarget2D(GraphicsDevice, 4096, 4096, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents);

            Entities.Add(new GroundEntity(Content, "Ground"));
            Entities.Add(new CampfireEntity(Content, "Campfire"));
            Entities.Add(new CarEntity(Content, "Car"));
            Entities.Add(new TentEntity(Content, "Tent"));

            _sun = new Sun(this);
            _updatable.Add(_sun);
            _ui.Add(_sun);

            Camera = new Camera(this);
            _updatable.Add(Camera);

            Mouse = new MouseInputHandler(this);
            _updatable.Add(Mouse);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData([Color.White]);

            var editor = new EntityEdit(this);
            _updatable.Add(editor);
            _ui.Add(editor);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (var updateable in _updatable)
            {
                updateable.Update(gameTime);
            }

            foreach (var entity in Entities)
            {
                entity.Update(gameTime);
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

            var renderContext = new RenderContext(Camera.View, Camera.Projection, _sun.View, _sun.Projection, _sun.Position, _shadowMap);

            foreach (var entity in Entities)
            {
                if (!entity.IncludeInShadowMap) continue;

                entity.Draw(GraphicsDevice, _shadowMapEffect, renderContext);
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            GraphicsDevice.Clear(Color.Black);

            foreach (var entity in Entities)
            {
                entity.Draw(GraphicsDevice, _shadedEffect, renderContext);
            }

            _spriteBatch.Begin();
            _stats.Draw(gameTime, _spriteBatch, GraphicsDevice, _arial);

            _spriteBatch.End();

            _imgui.BeginLayout(gameTime);

            foreach (var ui in _ui)
            {
                ui.RenderUi();
            }

            _imgui.EndLayout();

            base.Draw(gameTime);
        }
    }
}
