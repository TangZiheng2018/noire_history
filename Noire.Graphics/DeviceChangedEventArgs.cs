using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D9;

namespace Noire.Graphics {
    public sealed class DeviceChangedEventArgs : EventArgs {

        public DeviceChangedEventArgs(Device oldDevice, Device newDevice) {
            OldDevice = oldDevice;
            NewDevice = newDevice;
        }

        public Device OldDevice { get; }

        public Device NewDevice { get; }

    }
}
