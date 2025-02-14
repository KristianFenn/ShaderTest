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

                ImGui.ColorEdit3("Diffuse color", ref selectedBone.DiffuseEdit);
                ImGui.ColorEdit3("Specular color", ref selectedBone.SpecularEdit);
                ImGui.SliderFloat("Specular power", ref selectedBone.SpecularPower, 0.0f, 1.0f);
                ImGui.Checkbox("Textured", ref selectedBone.DrawTexture);
            }

            ImGui.End();
        }
    }
}
