using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Updatables
{
    public abstract class Updatable(Game game)
    {
        protected Game Game => game;
        public abstract void Update(GameTime gameTime);
    }
}
