using Microsoft.Xna.Framework.Content;
using ShaderTest.Shaders;

namespace ShaderTest.Entities
{
    public abstract class ModelEntity
    {
        public string Name { get; init; }

        private Matrix _world;
        public Matrix World 
        {
            get => _world;
            set => _world = value;
        }

        public Model Model { get; protected set; }
        public Dictionary<string, Material> Materials { get; protected set; } = [];
        public abstract bool IncludeInShadowMap { get; }

        public abstract void LoadContent(ContentManager content);

        public virtual void Update(GameTime gameTime)
        {

        }

        private void DrawBoneWithEffect(GraphicsDevice graphicsDevice, ModelBone bone, BaseEffect effect)
        {
            foreach (ModelMeshPart mesh in bone.Meshes.SelectMany(m => m.MeshParts))
            {
                graphicsDevice.SetVertexBuffer(mesh.VertexBuffer);
                graphicsDevice.Indices = mesh.IndexBuffer;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, mesh.VertexOffset, mesh.StartIndex, mesh.PrimitiveCount);
                }
            }
        }

        public void Draw(GraphicsDevice graphicsDevice, RenderContext renderContext, BaseEffect effect)
        {
            Matrix[] boneMatrices = new Matrix[Model.Bones.Count];

            for (int boneIdx = 0; boneIdx < Model.Bones.Count; boneIdx++)
            {
                ModelBone bone = Model.Bones[boneIdx];

                var parentTransform = bone.Parent != null ? boneMatrices[bone.Parent.Index] : World;
                boneMatrices[boneIdx] = bone.Transform * parentTransform;

                if (!Materials.TryGetValue(bone.Name, out var material))
                {
                    material = Materials["Default"];
                }

                effect.ApplyRenderContext(boneMatrices[boneIdx], renderContext, material);

                DrawBoneWithEffect(graphicsDevice, bone, effect);
            }
        }
    }
}
