using ImGuiNET;
using Microsoft.Xna.Framework;
using ShaderTest.Entities;
using ShaderTest.Shaders;
using ShaderTest.Updatables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.UI
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
            }

            ImGui.End();
        }
    }
}
