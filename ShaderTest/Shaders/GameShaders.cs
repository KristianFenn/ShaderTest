using Microsoft.Xna.Framework.Content;

namespace ShaderTest.Shaders
{
    public static class GameShaders
    {
        public static void Initialise(ContentManager content)
        {
            Pbr = new PbrEffect(content.Load<Effect>("Shaders/PBRTechniques"))
            {
                Name = "PBR"
            };
            Pbr.CurrentTechnique = Pbr.Techniques["Draw"];

            Deferred = new DeferredBufferEffect(content.Load<Effect>("Shaders/DeferredBuffer"))
            {
                Name = "Deferred"
            };
            Deferred.CurrentTechnique = Deferred.Techniques["DrawDeferredBuffers"];

            ShadowMap = new ShadowMapEffect(content.Load<Effect>("Shaders/Depth"))
            {
                Name = "Depth"
            };
            ShadowMap.CurrentTechnique = ShadowMap.Techniques["RenderDepth"];

            PbrDeferred = new PbrDeferredEffect(content.Load<Effect>("Shaders/PbrDeferred"))
            {
                Name = "PBR Deferred"
            };
            PbrDeferred.CurrentTechnique = PbrDeferred.Techniques["Draw"];

            Atmosphere = new AtmosphereEffect(content.Load<Effect>("Shaders/Atmosphere"))
            {
                Name = "Atmosphere"
            };
            Atmosphere.CurrentTechnique = Atmosphere.Techniques["Draw"];
        }

        public static PbrEffect Pbr { get; private set; }
        public static DeferredBufferEffect Deferred { get; private set; }
        public static ShadowMapEffect ShadowMap { get; private set; }
        public static PbrDeferredEffect PbrDeferred { get; private set; }
        public static AtmosphereEffect Atmosphere { get; private set; }
    }
}
