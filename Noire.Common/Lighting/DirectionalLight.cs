﻿using System.Runtime.InteropServices;
using SharpDX;

namespace Noire.Common.Lighting {
    [StructLayout(LayoutKind.Sequential)]
    public struct DirectionalLight {

        public Color4 Ambient;
        public Color4 Diffuse;
        public Color4 Specular;
        public Vector3 Direction;
        private readonly float Pad;

        public static readonly int Stride = Marshal.SizeOf(typeof(DirectionalLight));

    }
}
