using ShaderTest.Entities;
using ShaderTest.Extensions;
using ShaderTest.Shaders;
using ShaderTest.UI;
using ShaderTest.Updatables;

namespace ShaderTest
{
    public class ShaderTestGame : Game
    {
        public Camera Camera { get; private set; }
        public MouseInputHandler Mouse { get; private set; }
        public List<ModelEntity> Entities { get; private set; }

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _arial;

        private RenderTarget2D _shadowMap;
        private Texture2D _pixel;
        private GameStats _stats;
        private Sun _sun;
        private List<Updatable> _updatable;
        private UiWindow _uiWindow;

        public ShaderTestGame()
        {
            _graphics = new(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = false;
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
            _uiWindow = new UiWindow(this);
            _stats = new GameStats(GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Entities = [];
            _updatable = [];

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            GameShaders.Initialise(Content);

            _arial = Content.Load<SpriteFont>("Arial");
            _shadowMap = new RenderTarget2D(GraphicsDevice, 4096, 4096, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents)
            {
                Name = "ShadowMap"
            };

            var entityFactory = new EntityFactory(Content);

            Entities.Add(entityFactory.CreateEntity<GroundEntity>("Ground"));
            Entities.Add(entityFactory.CreateEntity<CampfireEntity>("Campfire"));
            Entities.Add(entityFactory.CreateEntity<CarEntity>("Car"));
            Entities.Add(entityFactory.CreateEntity<TentEntity>("Tent"));

            _sun = new Sun(this);
            _updatable.Add(_sun);
            _uiWindow.AddTab(_sun);

            Camera = new Camera(this);
            _updatable.Add(Camera);
            _uiWindow.AddTab(Camera);

            Mouse = new MouseInputHandler(this);
            _updatable.Add(Mouse);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData([Color.White]);

            var editor = new EntityEdit(this);

            _updatable.Add(editor);
            _uiWindow.AddTab(editor);

            var loadedTextures = Content.GetLoaded<Texture2D>();
            McFaceImGui.Initialise([_shadowMap, .. loadedTextures]);

            var textureView = new TextureView(_uiWindow.Renderer);
            _uiWindow.AddTab(textureView);

            GameDebug.Initialize(GraphicsDevice, _arial);
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

            var renderContext = new RenderContext(Camera.View, Camera.Projection, Camera.Position, Camera.Gamma, Camera.Exposure, _sun.View, _sun.Projection, _sun.Position, _sun.SunColor, _shadowMap);

            foreach (var entity in Entities)
            {
                if (!entity.IncludeInShadowMap) continue;

                entity.Draw(GraphicsDevice, renderContext, GameShaders.ShadowMap);
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            GraphicsDevice.Clear(Color.Black);

            foreach (var entity in Entities)
            {
                entity.Draw(GraphicsDevice, renderContext);
            }

            _spriteBatch.Begin();
            _stats.Draw(gameTime, _spriteBatch, GraphicsDevice, _arial);
            _spriteBatch.End();

            _uiWindow.RenderUi(gameTime);

            base.Draw(gameTime);
        }
    }
}
