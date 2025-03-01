using ShaderTest.Entities;
using System.Runtime.CompilerServices;

namespace ShaderTest.Shaders
{
    public abstract class BaseEffect(Effect cloneSource) : Effect(cloneSource)
    {
        protected EffectParameter GetParameter([CallerMemberName] string name = null)
        {
            return Parameters[name];
        }

        public abstract void ApplyRenderContext(Matrix world, RenderContext renderContext, Material material);
    }
}
