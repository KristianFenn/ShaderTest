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
        private RenderTarget2D _albedoMap;
        private RenderTarget2D _normalMap;
        private RenderTarget2D _pbrMap;
        private RenderTarget2D _depthMap;
        private RenderTargetBinding[] _deferredRenderTargetBindings;
        private VertexBuffer _fullScreenQuad;

        public ShaderTestGame()
        {
            _graphics = new(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = false;
            _graphics.HardwareModeSwitch = false;
            _graphics.PreferMultiSampling = false;
            _graphics.SynchronizeWithVerticalRetrace = true;

            IsMouseVisible = false;
            IsFixedTimeStep = false;
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

            Camera = new Camera(this);
            _updatable.Add(Camera);
            _uiWindow.AddTab(Camera);

            _arial = Content.Load<SpriteFont>("Arial");
            _shadowMap = new RenderTarget2D(GraphicsDevice, 4096, 4096, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents)
            {
                Name = "ShadowMap"
            };

            var vs = GraphicsDevice.Viewport.Bounds.Size;

            _albedoMap = new RenderTarget2D(GraphicsDevice, vs.X, vs.Y, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents)
            {
                Name = "Albedo"
            };

            _normalMap = new RenderTarget2D(GraphicsDevice, vs.X, vs.Y, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents)
            {
                Name = "Normal"
            };

            _pbrMap = new RenderTarget2D(GraphicsDevice, vs.X, vs.Y, false, SurfaceFormat.Color, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents)
            {
                Name = "PBR"
            };

            _depthMap = new RenderTarget2D(GraphicsDevice, vs.X, vs.Y, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.PlatformContents)
            {
                Name = "Depth"
            };

            GameShaders.Pbr.ShadowMap = _shadowMap;
            GameShaders.PbrDeferred.ShadowMap = _shadowMap;

            GameShaders.PbrDeferred.AlbedoMap = _albedoMap;
            GameShaders.PbrDeferred.NormalMap = _normalMap;
            GameShaders.PbrDeferred.PBRMap = _pbrMap;
            GameShaders.PbrDeferred.DepthMap = _depthMap;

            _deferredRenderTargetBindings =
            [
                new RenderTargetBinding(_albedoMap),
                new RenderTargetBinding(_normalMap),
                new RenderTargetBinding(_depthMap),
                new RenderTargetBinding(_pbrMap),
            ];

            _fullScreenQuad = new VertexBuffer(GraphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);

            _fullScreenQuad.SetData([
                new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0)),
            ]);

            var entityFactory = new EntityFactory(Content);

            Entities.Add(entityFactory.CreateEntity<GroundEntity>("Ground"));
            Entities.Add(entityFactory.CreateEntity<CampfireEntity>("Campfire"));
            Entities.Add(entityFactory.CreateEntity<CarEntity>("Car"));
            Entities.Add(entityFactory.CreateEntity<TentEntity>("Tent"));

            _sun = new Sun(this);
            _updatable.Add(_sun);
            _uiWindow.AddTab(_sun);

            Mouse = new MouseInputHandler(this);
            _updatable.Add(Mouse);

            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData([Color.White]);

            var editor = new EntityEdit(this);

            _updatable.Add(editor);
            _uiWindow.AddTab(editor);

            var loadedTextures = Content.GetLoaded<Texture2D>();
            McFaceImGui.Initialise([
                _albedoMap,
                _normalMap,
                _depthMap,
                _pbrMap,
                _shadowMap,
                .. loadedTextures
            ]);

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

            var renderContext = new RenderContext(Camera, _sun);

            foreach (var entity in Entities)
            {
                if (!entity.IncludeInShadowMap) continue;

                entity.Draw(GraphicsDevice, renderContext, GameShaders.ShadowMap);
            }

            GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            if (Camera.DrawDeferred)
            {
                GraphicsDevice.SetRenderTargets(_deferredRenderTargetBindings);
                GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.Black, GraphicsDevice.Viewport.MaxDepth, 0);
                GraphicsDevice.BlendState = BlendState.Opaque;
                GraphicsDevice.DepthStencilState = DepthStencilState.None;
                GraphicsDevice.SetVertexBuffer(_fullScreenQuad);

                GameShaders.Deferred.CurrentTechnique = GameShaders.Deferred.Techniques["ClearDeferredBuffers"];
                GameShaders.Deferred.CurrentTechnique.Passes[0].Apply();
                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);

                GameShaders.Deferred.CurrentTechnique = GameShaders.Deferred.Techniques["DrawDeferredBuffers"];
                GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                foreach (var entity in Entities)
                {
                    entity.Draw(GraphicsDevice, renderContext, GameShaders.Deferred);
                }

                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.DepthStencilState = DepthStencilState.None;
                GraphicsDevice.Clear(Color.Black);

                GraphicsDevice.SetVertexBuffer(_fullScreenQuad);

                GameShaders.PbrDeferred.ApplyRenderContext(Matrix.Identity, renderContext, default);
                GameShaders.PbrDeferred.CurrentTechnique.Passes[0].Apply();

                GraphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            }
            else
            {
                GraphicsDevice.SetRenderTarget(null);
                GraphicsDevice.Clear(Color.Black);

                foreach (var entity in Entities)
                {
                    entity.Draw(GraphicsDevice, renderContext, GameShaders.Pbr);
                }
            }

            _spriteBatch.Begin();
            _stats.Draw(gameTime, _spriteBatch, GraphicsDevice, _arial);
            _spriteBatch.End();

            _uiWindow.RenderUi(gameTime);

            base.Draw(gameTime);
        }
    }
}
