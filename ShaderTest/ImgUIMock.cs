using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Just here to make it easier to migrate back to imgui.net when required.

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ImGuiNET
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public static class ImGui
    {
        public static void SliderFloat(string v1, ref float falloff, float v2, float v3) { }
        public static void SliderFloat3(string v1, ref System.Numerics.Vector3 scatteringCoefficients, float v2, float v3) { }
        public static void Image(nint textureRef, System.Numerics.Vector2 vector2) { }
        public static void Checkbox(string v, ref bool drawDeferred) { }
        public static bool BeginCombo(string v, string name) => false;
        public static bool Selectable(string name, bool v) => false;
        public static void EndCombo() { }
        public static void ColorEdit3(string name, ref System.Numerics.Vector3 vec3) { }
        public static void Begin(string v) { }
        public static void BeginTabBar(string current) { }
        public static bool TabItemButton(string tab, ImGuiTabItemFlags flags) => false;
        public static void EndTabBar() { }
        public static bool CollapsingHeader(string uiSectionName) => false;
        public static void End() { }
    }

    public enum ImGuiTabItemFlags
    {
        SetSelected,
        None
    }
}

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace MonoGame.ImGuiNet
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    public class ImGuiRenderer(Game game)
    {
        public nint BindTexture(Texture2D selectedTexture) => 0;
        public void BeginLayout(GameTime gameTime) { }
        public void RebuildFontAtlas() { }
        public void EndLayout() { }
    }
}
