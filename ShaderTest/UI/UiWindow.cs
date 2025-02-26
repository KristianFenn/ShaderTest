using ImGuiNET;
using MonoGame.ImGuiNet;

namespace ShaderTest.UI
{
    public class UiWindow
    {
        public ImGuiRenderer Renderer { get; init; }
        private List<IHasUi> _tabs = [];
        private string _current;

        public UiWindow(Game game)
        {
            Renderer = new(game);
            Renderer.RebuildFontAtlas();
        }

        public void AddTab(IHasUi ui)
        {
            _tabs.Add(ui);
            _current ??= ui.Name;
        }

        public void RenderUi(GameTime gameTime)
        {
            Renderer.BeginLayout(gameTime);

            ImGui.Begin("Debug");

            ImGui.BeginTabBar(_current);
            foreach (IHasUi ui in _tabs)
            {
                var flags = _current == ui.Name ? ImGuiTabItemFlags.SetSelected : ImGuiTabItemFlags.None;
                if (ImGui.TabItemButton(ui.Name, flags))
                {
                    _current = ui.Name;
                }
            }
            ImGui.EndTabBar();

            _tabs.Single(t => t.Name == _current).RenderUi();

            ImGui.End();

            Renderer.EndLayout();
        }
    }
}
