using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShaderTest.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest
{
    public class SimpleEntity(Model model, Matrix world, bool includeInShadowMap, Color diffuseColor, Color specularColor, Texture2D texture = null)
    {
        public Matrix World { get; set; } = world;
        public Model Model { get; set; } = model;
        public bool IncludeInShadowMap { get; init; } = includeInShadowMap;
        public Color DiffuseColor { get; } = diffuseColor;
        public Color SpecularColor { get; } = specularColor;
        public Texture2D Texture { get; } = texture;

        public void Draw(GraphicsDevice graphicsDevice, BaseEffect effect, RenderContext renderContext)
        {
            Matrix[] boneMatrices = new Matrix[Model.Bones.Count];

            for (int boneIdx = 0; boneIdx < Model.Bones.Count; boneIdx++)
            {
                ModelBone bone = Model.Bones[boneIdx];
                var parentTransform = bone.Parent != null ? boneMatrices[bone.Parent.Index] : World;
                boneMatrices[boneIdx] = bone.Transform * parentTransform;
                effect.ApplyRenderContext(boneMatrices[boneIdx], renderContext);

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
        }
    }
}
