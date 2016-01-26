using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.Direct3D9;
using SharpDX.Windows;

namespace Noire.Graphics
{

    public sealed class RenderManager : IDisposable
    {

        public RenderManager(Control control)
        {
            _control = control;
            _direct3D = new Direct3D();
            _screenTarget = new RenderTarget(this, 0);
            _stage = new Stage(this);
        }

        /// <summary>
        /// 执行与释放或重置非托管资源关联的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            NoireUtilities.SafeDispose(ref _screenTarget);
            NoireUtilities.SafeDispose(ref _direct3D);
            _control = null;
        }

        public void Update()
        {
            _stage.Update(_screenTarget);
        }

        public void Render()
        {
            _screenTarget.Clear();
            _screenTarget.Device.BeginScene();
            _stage.Render(_screenTarget);
            _screenTarget.Device.EndScene();
        }

        public void Run()
        {
            using (var renderLoop = new RenderLoop(_control))
            {
                while (renderLoop.NextFrame())
                {
                    Update();
                    Render();
                    _screenTarget.Present();
                }
            }
        }

        public Direct3D Direct3D => _direct3D;

        public Control Control => _control;

        public Stage Stage => _stage;

        public RenderTarget Screen => _screenTarget;

        private Control _control;
        private Direct3D _direct3D;
        private Stage _stage;
        private RenderTarget _screenTarget;

    }

}
