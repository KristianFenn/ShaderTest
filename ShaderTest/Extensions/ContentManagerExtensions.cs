using Microsoft.Xna.Framework.Content;
using System.IO;
using System.Reflection;

namespace ShaderTest.Extensions
{
    public static class ContentManagerExtensions
    {
        private static PropertyInfo _contentManagerLoadedAssets = typeof(ContentManager).GetProperty("LoadedAssets", BindingFlags.NonPublic | BindingFlags.Instance);
        public static IEnumerable<T> GetLoaded<T>(this ContentManager contentManager)
        {
            var loadedAssets = (Dictionary<string, object>)_contentManagerLoadedAssets.GetValue(contentManager);

            return loadedAssets.Where(a => a.Value is T)
                .Select(a => a.Value)
                .Cast<T>();
        }

        public static T TryLoad<T>(this ContentManager contentManager, string path)
        {
            var targetPath = Path.Join(contentManager.RootDirectory, $"{path}.xnb");

            if (File.Exists(targetPath))
            {
                return contentManager.Load<T>(path);
            }

            return default;
        }
    }
}
