using ImGuiNET;
using MonoGame.ImGuiNet;

namespace ShaderTest.UI
{
    public class UiWindow
    {
        public ImGuiRenderer Renderer { get; init; }
        private Dictionary<string, List<IHasUi>> _tabs = [];
        private string _current;

        public UiWindow(Game game)
        {
            Renderer = new(game);
            Renderer.RebuildFontAtlas();
        }

        public void AddToTab(string tab, IHasUi ui)
        {
            if (!_tabs.TryGetValue(tab, out List<IHasUi> sections))
            {
                sections = [];
                _tabs.Add(tab, sections);
            }

            sections.Add(ui);
            _current ??= tab;
        }

        public void RenderUi(GameTime gameTime)
        {
            Renderer.BeginLayout(gameTime);

            ImGui.Begin("Debug");

            ImGui.BeginTabBar(_current);
            foreach (var (tab, _) in _tabs)
            {
                var flags = _current == tab ? ImGuiTabItemFlags.SetSelected : ImGuiTabItemFlags.None;
                if (ImGui.TabItemButton(tab, flags))
                {
                    _current = tab;
                }
            }
            ImGui.EndTabBar();

            var sections = _tabs[_current];

            foreach (var section in sections)
            {
                if (ImGui.CollapsingHeader(section.UiSectionName))
                {
                    section.RenderUi();
                }
            }

            ImGui.End();

            Renderer.EndLayout();
        }
    }
}
