using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace Noire.Graphics.D3D11 {
    public static class InputLayoutDescriptions {

        public static readonly InputElement[] PosColor = {
            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
            new InputElement("COLOR", 0, Format.R32G32B32A32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
        };

        public static readonly InputElement[] Pos = {
            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
        };

        public static readonly InputElement[] PosNorm = {
            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
            new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0, InputClassification.PerVertexData, 0)
        };

        public static readonly InputElement[] PosNormTex = {
            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
            new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0, InputClassification.PerVertexData, 0),
            new InputElement("TEXCOORD", 0, Format.R32G32_Float, 24, 0, InputClassification.PerVertexData, 0)
        };

        public static readonly InputElement[] PosNormTexTan = {
            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
            new InputElement("NORMAL", 0, Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
            new InputElement("TEXCOORD", 0, Format.R32G32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
            new InputElement("TANGENT", 0, Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData,0 )
        };

        public static readonly InputElement[] Particle = {
            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
            new InputElement("VELOCITY", 0, Format.R32G32B32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
            new InputElement("SIZE", 0, Format.R32G32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
            new InputElement("AGE", 0, Format.R32_Float, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
            new InputElement("TYPE", 0, Format.R32_UInt, InputElement.AppendAligned, 0, InputClassification.PerVertexData, 0),
        };

    }
}
