using ImGuiNET;
using ShaderTest.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Shaders
{
    public class RenderControl : IHasUi
    {
        private bool _drawDeferred = true;
        public bool DrawDeferred => _drawDeferred;

        public string UiSectionName => "Shader";

        public void RenderUi()
        {
            ImGui.Checkbox("Deferred", ref _drawDeferred);

            BaseEffect effect = GameShaders.Pbr;

            if (_drawDeferred)
            {
                effect = GameShaders.PbrDeferred;
            }

            if (ImGui.BeginCombo("Technique", effect.CurrentTechnique.Name))
            {
                foreach (var technique in effect.Techniques)
                {
                    if (ImGui.Selectable(technique.Name, effect.CurrentTechnique == technique))
                    {
                        effect.CurrentTechnique = technique;
                    }
                }
                ImGui.EndCombo();
            }
        }
    }
}
