using ShaderTest.Shaders;

namespace ShaderTest.Entities
{
    public class BoneShaders
    {
        private int _current = 0;

        public BaseEffect CurrentShader
        {
            get => ConfiguredShaders[_current];
            set => _current = Array.IndexOf(ConfiguredShaders, value);
        }

        public BaseEffect[] ConfiguredShaders { get; set; }
    }
}
