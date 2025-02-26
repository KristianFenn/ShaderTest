using Microsoft.Xna.Framework.Content;

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
