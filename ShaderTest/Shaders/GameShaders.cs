using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest.Shaders
{
    public static class GameShaders
    {
        public static void Initialise(ContentManager content)
        {
            BlinnPhong = new BlinnPhongEffect(content.Load<Effect>("Shaders/BlinnPhongTechniques"));
            BlinnPhong.CurrentTechnique = BlinnPhong.Techniques["DrawBlinnPhong"];
            BlinnPhong.Name = "Blinn-Phong";

            Pbr = new PbrEffect(content.Load<Effect>("Shaders/PBRTechniques"));
            Pbr.CurrentTechnique = Pbr.Techniques["DrawPBR"];
            Pbr.Name = "PBR";

            ShadowMap = new ShadowMapEffect(content.Load<Effect>("Shaders/Depth"));
            ShadowMap.CurrentTechnique = ShadowMap.Techniques["RenderDepth"];
            ShadowMap.Name = "Depth";
        }

        public static BlinnPhongEffect BlinnPhong { get; private set; }
        public static PbrEffect Pbr { get; private set; }
        public static ShadowMapEffect ShadowMap { get; private set; }
    }
}
