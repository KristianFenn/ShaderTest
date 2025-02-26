namespace ShaderTest.Updatables
{
    public class MouseInputHandler(ShaderTestGame game) : Updatable(game)
    {
        private MouseState _lastState;
        private MouseState _currentState;
        private Vector2 _lockPosition = game.GraphicsDevice.Viewport.Bounds.Size.ToVector2() / 2;

        public bool LockMouse { get; set; }
        public Vector2 Move { get; private set; }
        public Vector2 Position { get; private set; }
        public bool InScreenBounds { get; private set; }
        public Ray Ray { get; private set; }

        public bool IsButtonDown(MouseButton button)
        {
            return button switch
            {
                MouseButton.Left => _currentState.LeftButton == ButtonState.Pressed,
                MouseButton.Right => _currentState.RightButton == ButtonState.Pressed,
                MouseButton.Middle => _currentState.MiddleButton == ButtonState.Pressed,
                _ => throw new NotSupportedException("Invalid mouse button."),
            };
        }

        public bool IsButtonPressed(MouseButton button)
        {
            return button switch
            {
                MouseButton.Left => _currentState.LeftButton == ButtonState.Pressed && _lastState.LeftButton == ButtonState.Released,
                MouseButton.Right => _currentState.RightButton == ButtonState.Pressed && _lastState.RightButton == ButtonState.Released,
                MouseButton.Middle => _currentState.MiddleButton == ButtonState.Pressed && _lastState.MiddleButton == ButtonState.Released,
                _ => throw new NotSupportedException("Invalid mouse button."),
            };
        }

        public bool IsButtonUp(MouseButton button)
        {
            return button switch
            {
                MouseButton.Left => _currentState.LeftButton == ButtonState.Released,
                MouseButton.Right => _currentState.RightButton == ButtonState.Released,
                MouseButton.Middle => _currentState.MiddleButton == ButtonState.Released,
                _ => throw new NotSupportedException("Invalid mouse button."),
            };
        }

        public bool IsButtonReleased(MouseButton button)
        {
            return button switch
            {
                MouseButton.Left => _currentState.LeftButton == ButtonState.Released && _lastState.LeftButton == ButtonState.Pressed,
                MouseButton.Right => _currentState.RightButton == ButtonState.Released && _lastState.RightButton == ButtonState.Pressed,
                MouseButton.Middle => _currentState.MiddleButton == ButtonState.Released && _lastState.MiddleButton == ButtonState.Pressed,
                _ => throw new NotSupportedException("Invalid mouse button."),
            };
        }

        public override void Update(GameTime gameTime)
        {
            _lastState = _currentState;
            _currentState = Mouse.GetState();

            if (LockMouse)
            {
                Mouse.SetPosition((int)_lockPosition.X, (int)_lockPosition.Y);
                Position = _lockPosition;
                Move = _currentState.Position.ToVector2() - _lockPosition;
                Game.IsMouseVisible = false;
            }
            else
            {
                var lastLocation = Position;
                Position = _currentState.Position.ToVector2();
                Move = Position - lastLocation;
                Game.IsMouseVisible = true;
            }

            InScreenBounds = Game.GraphicsDevice.Viewport.Bounds.Contains(Position);

            if (InScreenBounds)
            {
                var nearPoint = Game.GraphicsDevice.Viewport.Unproject(new Vector3(Position.X, Position.Y, 0f), Game.Camera.Projection, Game.Camera.View, Matrix.Identity);
                var farPoint = Game.GraphicsDevice.Viewport.Unproject(new Vector3(Position.X, Position.Y, 0f), Game.Camera.Projection, Game.Camera.View, Matrix.Identity);
                var direction = Vector3.Normalize(farPoint - nearPoint);
                Ray = new Ray(nearPoint, direction);
            }
            else
            {
                Ray = default;
            }
        }
    }
}
