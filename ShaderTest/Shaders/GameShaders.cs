using Microsoft.Xna.Framework.Content;

namespace ShaderTest.Shaders
{
    public static class GameShaders
    {
        public static void Initialise(ContentManager content)
        {
            Pbr = new PbrEffect(content.Load<Effect>("Shaders/PBRTechniques"));
            Pbr.CurrentTechnique = Pbr.Techniques["DrawPos"];
            Pbr.Name = "PBR";

            Deferred = new DeferredBufferEffect(content.Load<Effect>("Shaders/DeferredBuffer"));
            Deferred.CurrentTechnique = Deferred.Techniques["DrawDeferredBuffers"];
            Deferred.Name = "Deferred";

            ShadowMap = new ShadowMapEffect(content.Load<Effect>("Shaders/Depth"));
            ShadowMap.CurrentTechnique = ShadowMap.Techniques["RenderDepth"];
            ShadowMap.Name = "Depth";

            PbrDeferred = new PbrDeferredEffect(content.Load<Effect>("Shaders/PbrDeferred"));
            PbrDeferred.CurrentTechnique = PbrDeferred.Techniques["DrawPos"];
            PbrDeferred.Name = "PBR Deferred";
        }

        public static PbrEffect Pbr { get; private set; }
        public static DeferredBufferEffect Deferred { get; private set; }
        public static ShadowMapEffect ShadowMap { get; private set; }
        public static PbrDeferredEffect PbrDeferred { get; private set; }
    }
}
