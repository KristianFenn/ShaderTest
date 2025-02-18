using ImGuiNET;
using Microsoft.Xna.Framework;
using ShaderTest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Updatables
{
    public class EntityEdit(ShaderTestGame game) : Updatable(game), IHasUi
    {
        private readonly string[] Techniques = [
            "DrawTextured",
            "DrawTexturedRma",
            "DrawTexturedRmaNormal",
            "DrawNormals",
            "DrawMapNormals"
        ];

        private ModelEntity _selectedEntity = null;
        private string _selectedBone = "";

        public override void Update(GameTime gameTime)
        {
        }

        public void RenderUi()
        {
            ImGui.Begin("Entity");

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
                    foreach (var (name, _) in _selectedEntity.BoneParameters)
                    {
                        if (ImGui.Selectable(name, _selectedBone == name))
                        {
                            _selectedBone = name;
                        }
                    }
                    ImGui.EndCombo();
                }

                var selectedBone = _selectedEntity.BoneParameters[_selectedBone];

                if (ImGui.BeginCombo("Technique", selectedBone.Technique))
                {
                    foreach (var name in Techniques)
                    {
                        if (ImGui.Selectable(name, selectedBone.Technique == name))
                        {
                            selectedBone.Technique = name;
                        }
                    }
                    ImGui.EndCombo();
                }

                ImGui.SliderFloat("Metallic", ref selectedBone.Metallic, 0.0f, 1.0f);
                ImGui.SliderFloat("Roughness", ref selectedBone.Roughness, 0.0f, 1.0f);
                ImGui.SliderFloat("AmbientOcclusion", ref selectedBone.AmbientOcclusion, 0.0f, 1.0f);
            }

            ImGui.End();
        }
    }
}
