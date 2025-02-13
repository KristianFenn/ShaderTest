using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace ShaderTest.Entities
{
    public class GameStats
    {
        private const string _formatUpdateString = "UPS: {0:0.00} ({1:0.00}ms)\nFPS: {2:0.00} ({3:0.00}ms)";
        private const int _samples = 100;

        private float[] _updateTimes = new float[_samples];
        private int _nextUpdateIndex = 0;

        private float[] _drawTimes = new float[_samples];
        private int _nextDrawIndex = 0;

        private Vector2 _drawPos;

        public GameStats(GraphicsDevice graphicsDevice)
        {
            var left = graphicsDevice.Viewport.Width - 600f;
            var top = 25f;

            _drawPos = new Vector2(left, top);
        }

        public void Update(GameTime gameTime)
        {
            _updateTimes[_nextUpdateIndex] = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            _nextUpdateIndex = (_nextUpdateIndex + 1) % _samples;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, SpriteFont spriteFont)
        {
            _drawTimes[_nextDrawIndex] = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            _nextDrawIndex = (_nextDrawIndex + 1) % _samples;

            var averageUpdateMs = _updateTimes.Sum() / _samples;
            var averageDrawMs = _drawTimes.Sum() / _samples;

            var ups = 1000f / averageUpdateMs;
            var fps = 1000f / averageDrawMs;

            var statsString = string.Format(_formatUpdateString, ups, averageUpdateMs, fps, averageDrawMs);

            spriteBatch.DrawString(
                spriteFont: spriteFont,
                text: statsString,
                position: _drawPos,
                color: Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: .5f,
                effects: SpriteEffects.None,
                layerDepth: 0f);
        }
    }
}
