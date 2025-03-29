using ImGuiNET;
using ShaderTest.Entities;
using ShaderTest.Updatables;

namespace ShaderTest.UI
{
    public class EntityEdit(ShaderTestGame game) : Updatable(game), IHasUi
    {
        private ModelEntity _selectedEntity = null;
        private string _selectedBone = "";

        public string UiSectionName => "Entity";

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
                    foreach (var (name, _) in _selectedEntity.Materials)
                    {
                        if (ImGui.Selectable(name, _selectedBone == name))
                        {
                            _selectedBone = name;
                        }
                    }
                    ImGui.EndCombo();
                }

                var material = _selectedEntity.Materials[_selectedBone];

                ImGui.Checkbox("Use texture", ref material.UseTexture);
                McFaceImGui.TextureCombo("Texture", ref material.Texture);
                ImGui.Checkbox("Use RMA map", ref material.UsePbrMap);
                McFaceImGui.TextureCombo("RMA map", ref material.PbrMap);
                ImGui.Checkbox("Use normal map", ref material.UseNormalMap);
                McFaceImGui.TextureCombo("Normal map", ref material.NormalMap);
                McFaceImGui.ColorEdit3("Albedo", ref material.Albedo);
                ImGui.SliderFloat("Roughness", ref material.Roughness, 0.0f, 1.0f);
                ImGui.SliderFloat("Metallic", ref material.Metallic, 0.0f, 1.0f);
                ImGui.SliderFloat("Ambient occlusion", ref material.AmbientOcclusion, 0.0f, 1.0f);
            }
        }
    }
}
