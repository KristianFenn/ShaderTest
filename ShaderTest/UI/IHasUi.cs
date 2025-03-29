namespace ShaderTest.UI
{
    public interface IHasUi
    {
        string UiSectionName { get; }
        void RenderUi();
    }
}
