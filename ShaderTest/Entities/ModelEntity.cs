using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ShaderTest.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Entities
{
    public abstract class ModelEntity
    {
        public string Name { get; init; }
        public Matrix World { get; protected set; }
        public Model Model { get; protected set; }
        public Dictionary<string, BoneShaders> BoneShaders { get; protected set; } = [];
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

        public void Draw(GraphicsDevice graphicsDevice, RenderContext renderContext, BaseEffect effect = null)
        {
            Matrix[] boneMatrices = new Matrix[Model.Bones.Count];

            for (int boneIdx = 0; boneIdx < Model.Bones.Count; boneIdx++)
            {
                ModelBone bone = Model.Bones[boneIdx];

                var parentTransform = bone.Parent != null ? boneMatrices[bone.Parent.Index] : World;
                boneMatrices[boneIdx] = bone.Transform * parentTransform;
                var boneEffect = effect;

                if (boneEffect == null)
                {
                    if (!BoneShaders.TryGetValue(bone.Name, out var shaders))
                    {
                        shaders = BoneShaders["Default"];
                    }

                    boneEffect = shaders.CurrentShader;
                }

                boneEffect.ApplyRenderContext(boneMatrices[boneIdx], renderContext);

                DrawBoneWithEffect(graphicsDevice, bone, boneEffect);
            }
        }
    }
}
