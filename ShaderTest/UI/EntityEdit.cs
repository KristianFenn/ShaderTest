using ImGuiNET;
using ShaderTest.Entities;
using ShaderTest.Shaders;
using ShaderTest.Updatables;

namespace ShaderTest.UI
{
    public class EntityEdit(ShaderTestGame game) : Updatable(game), IHasUi
    {
        private ModelEntity _selectedEntity = null;
        private string _selectedBone = "";

        public string Name => "Entity";

        public override void Update(GameTime gameTime)
        {
        }

        public void RenderUi()
        {
            if (ImGui.BeginCombo("Entities", _selectedEntity?.Name ?? "N/A"))
            {
                foreach (var entity in Game.Entities)
                {
                    if (ImGui.Selectable(entity.Name, _selectedEntity == entity))
                    {
                        _selectedEntity = entity;
                        _selectedBone = "Default";
                    }
                }
                ImGui.EndCombo();
            }

            if (_selectedEntity != null)
            {
                if (ImGui.BeginCombo("Bone", _selectedBone))
                {
                    foreach (var (name, _) in _selectedEntity.BoneShaders)
                    {
                        if (ImGui.Selectable(name, _selectedBone == name))
                        {
                            _selectedBone = name;
                        }
                    }
                    ImGui.EndCombo();
                }

                var selectedBone = _selectedEntity.BoneShaders[_selectedBone];
                var selectedShader = selectedBone.CurrentShader;

                if (ImGui.BeginCombo("Shader", selectedShader.GetType().Name))
                {
                    foreach (var shader in selectedBone.ConfiguredShaders)
                    {
                        if (ImGui.Selectable(shader.GetType().Name, selectedShader == shader))
                        {
                            selectedBone.CurrentShader = shader;
                            selectedShader = shader;
                        }
                    }
                    ImGui.EndCombo();
                }

                var currentTechnique = selectedShader.CurrentTechnique;

                if (ImGui.BeginCombo("Technique", currentTechnique.Name))
                {
                    foreach (var technique in selectedShader.Techniques)
                    {
                        if (technique.Name == "common") continue;

                        if (ImGui.Selectable(technique.Name, currentTechnique == technique))
                        {
                            selectedBone.CurrentShader.CurrentTechnique = technique;
                        }
                    }
                    ImGui.EndCombo();
                }

                if (selectedShader is PbrEffect pbr)
                {
                    pbr.UseTexture = McFaceImGui.Checkbox("Use texture", pbr.UseTexture);
                    pbr.Texture = McFaceImGui.TextureCombo("Texture", pbr.Texture);
                    pbr.UseRmaMap = McFaceImGui.Checkbox("Use RMA map", pbr.UseRmaMap);
                    pbr.RmaMap = McFaceImGui.TextureCombo("RMA map", pbr.RmaMap);
                    pbr.UseNormalMap = McFaceImGui.Checkbox("Use normal map", pbr.UseNormalMap);
                    pbr.NormalMap = McFaceImGui.TextureCombo("Normal map", pbr.NormalMap);
                    pbr.Albedo = McFaceImGui.ColorEdit3("Albedo", pbr.Albedo);
                    pbr.Roughness = McFaceImGui.SliderFloat("Roughness", pbr.Roughness, 0.0f, 1.0f);
                    pbr.Metallic = McFaceImGui.SliderFloat("Metallic", pbr.Metallic, 0.0f, 1.0f);
                    pbr.AmbientOcclusion = McFaceImGui.SliderFloat("Ambient occlusion", pbr.AmbientOcclusion, 0.0f, 1.0f);
                }
                else if (selectedShader is BlinnPhongEffect blinnPhong)
                {
                    blinnPhong.UseTexture = McFaceImGui.Checkbox("Use texture", blinnPhong.UseTexture);
                    blinnPhong.Texture = McFaceImGui.TextureCombo("Texture", blinnPhong.Texture);
                    blinnPhong.DiffuseColor = McFaceImGui.ColorEdit3("Diffuse color", blinnPhong.DiffuseColor);
                    blinnPhong.SpecularColor = McFaceImGui.ColorEdit3("Specular color", blinnPhong.SpecularColor);
                    blinnPhong.SpecularPower = McFaceImGui.SliderFloat("Specular power", blinnPhong.SpecularPower, 0.0f, 1.0f);
                }
            }
        }
    }
}
