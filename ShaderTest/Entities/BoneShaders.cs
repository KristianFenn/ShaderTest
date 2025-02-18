using ShaderTest.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
