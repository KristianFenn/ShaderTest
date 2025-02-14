using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ShaderTest.Updatables;
using System.Collections.Generic;

namespace ShaderTest.Shaders
{
    public class ShadedEffect(Effect cloneSource) : BaseEffect(cloneSource), IHasUi
    {
        private static readonly Matrix LightToShadowMap = Matrix.CreateScale(0.5f, -0.5f, 1f)
            * Matrix.CreateTranslation(0.5f, 0.5f, 0f);

        private string _technique = "Default";

        private static readonly Dictionary<string, string> _techniqueNames = new()
        {
            { "Default", "Default" },
            { "DrawShaded", "Flat colour" },
            { "DrawTextured", "Textured" },
            { "DrawNormals" , "Normals" },
            { "DrawIncidence" , "Incidence" },
            { "DrawSmPosition" , "Shadow map position" },
            { "DrawShadowSamples" , "Shadow samples" }
        };

        public override void ApplyRenderContext(Matrix world, RenderContext renderContext, EffectParameters effectParameters)
        {
            var modelToView = world * renderContext.View;
            Parameters["ModelToWorld"]?.SetValue(world);
            Parameters["ModelToView"].SetValue(modelToView);
            Parameters["NormalToView"].SetValue(McFaceMatrix.CalculateNormalMatrix(modelToView));
            Parameters["ModelToScreen"].SetValue(modelToView * renderContext.Projection);
            Parameters["LightPosition"].SetValue(renderContext.LightPosition);
            Parameters["ShadowMapSampler+ShadowMap"].SetValue(renderContext.ShadowMap);
            Parameters["ModelToShadowMap"].SetValue(world * renderContext.WorldToLight * LightToShadowMap);

            Parameters["DiffuseColor"].SetValue(effectParameters.DiffuseColor.ToVector4());
            Parameters["SpecularColor"].SetValue(effectParameters.SpecularColor.ToVector4());
            Parameters["SpecularPower"].SetValue(effectParameters.SpecularPower);
            Parameters["TextureSampler+Texture"].SetValue(effectParameters.Texture);

            if (_technique == "Default")
            {
                var technique = effectParameters.DrawTexture ? "DrawTextured" : "DrawShaded";
                CurrentTechnique = Techniques[technique];
            }
            else
            {
                CurrentTechnique = Techniques[_technique];
            }
        }

        public void RenderUi()
        {
            ImGui.Begin("Shader");
            if (ImGui.BeginCombo("Technique", _techniqueNames[_technique]))
            {
                foreach (var (technique, name) in _techniqueNames)
                {
                    if (ImGui.Selectable(name, _technique == technique))
                    {
                        _technique = technique;
                    }
                }
                ImGui.EndCombo();
            }
            ImGui.End();
        }
    }
}
