using ImGuiNET;
using MonoGame.ImGuiNet;

namespace ShaderTest.UI
{
    public class TextureView(ImGuiRenderer renderer) : IHasUi
    {
        private static readonly Dictionary<Texture2D, nint> _refCache = [];
        private Texture2D _selectedTexture;

        public string Name => "Textures";

        public void RenderUi()
        {
            _selectedTexture = McFaceImGui.TextureCombo("Texture", _selectedTexture);

            if (_selectedTexture == null) return;

            if (!_refCache.TryGetValue(_selectedTexture, out nint textureRef))
            {
                textureRef = renderer.BindTexture(_selectedTexture);
                _refCache.Add(_selectedTexture, textureRef);
            }

            ImGui.Image(textureRef, new System.Numerics.Vector2(400f));
        }
    }
}
