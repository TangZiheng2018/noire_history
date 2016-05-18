using SharpDX;
using SharpDX.Direct3D11;

namespace Noire.Graphics.D3D11.Model {

    public delegate void ModelDrawDelegate(DeviceContext context, EffectPass pass, Matrix view, Matrix proj);

}
