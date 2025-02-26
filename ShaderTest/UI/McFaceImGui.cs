using ImGuiNET;

namespace ShaderTest.UI
{
    public static class McFaceImGui
    {
        private static IEnumerable<Texture2D> _textures;

        public static void Initialise(IEnumerable<Texture2D> textures)
        {
            _textures = textures;
        }

        public static bool Checkbox(string name, bool value)
        {
            ImGui.Checkbox(name, ref value);
            return value;
        }

        public static Color ColorEdit3(string name, Color color)
        {
            var vec3 = new System.Numerics.Vector3(color.R, color.G, color.B);
            ImGui.ColorEdit3(name, ref vec3);
            return new Color(vec3.X, vec3.Y, vec3.Z, color.A);
        }

        public static float SliderFloat(string name, float value, float min, float max)
        {
            ImGui.SliderFloat(name, ref value, min, max);
            return value;
        }

        public static Texture2D TextureCombo(string name, Texture2D texture)
        {
            var selected = texture;

            if (ImGui.BeginCombo(name, texture?.Name ?? "N/A"))
            {
                foreach (var tex in _textures)
                {
                    if (ImGui.Selectable(tex.Name))
                    {
                        selected = tex;
                    }
                }
                ImGui.EndCombo();
            }

            return selected;
        }
    }
}
