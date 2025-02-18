using Microsoft.Xna.Framework.Content;
using ShaderTest.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Entities
{
    public class EntityFactory(ContentManager content)
    {
        public T CreateEntity<T>(string name)
            where T : ModelEntity, new()
        {
            var entity = new T { Name = name };
            entity.LoadContent(content);
            return entity;
        }
    }
}
