using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Updatables
{
    public abstract class Updatable(ShaderTestGame game)
    {
        protected ShaderTestGame Game => game;
        public abstract void Update(GameTime gameTime);
    }
}
