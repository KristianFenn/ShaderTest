using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderTest
{
    public class RenderContext(Matrix view, Matrix projection, Vector3 cameraPosition, Matrix lightView, Matrix lightProjection, Vector3 lightPosition, Vector3 lightColour, Texture2D shadowMap)
    {
        public Matrix View { get; } = view;
        public Matrix Projection { get; } = projection;
        public Vector3 CameraPosition { get; } = cameraPosition;
        public Vector3 LightColor { get; } = lightColour;
        public Matrix WorldToScreen { get; } = view * projection;
        public Matrix WorldToLight { get; } = lightView * lightProjection;
        public Vector3 LightPosition { get; } = Vector3.TransformNormal(lightPosition, view);
        public Texture2D ShadowMap { get; } = shadowMap;
    }
}
