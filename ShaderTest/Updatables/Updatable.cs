namespace ShaderTest.Updatables
{
    public abstract class Updatable(ShaderTestGame game)
    {
        protected ShaderTestGame Game => game;
        public abstract void Update(GameTime gameTime);
    }
}
