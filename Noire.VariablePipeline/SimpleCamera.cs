using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.DirectInput;

namespace Noire.VariablePipeline {
    public sealed class SimpleCamera {

        public SimpleCamera() {
            //Position = -Vector3.UnitY * 200;
            Position = new Vector3(0, 30, -50);
            Rotation = Quaternion.LookAtLH(Position, Vector3.Zero, Vector3.UnitZ);
        }

        public void React(KeyboardState state) {
            var ratio = 15f;
            Quaternion q;

            if (state.IsPressed(Key.A)) {
                Position -= Vector3.UnitX * ratio;
            }
            if (state.IsPressed(Key.D)) {
                Position += Vector3.UnitX * ratio;
            }
            if (state.IsPressed(Key.W)) {
                Position += Vector3.UnitZ * ratio;
            }
            if (state.IsPressed(Key.S)) {
                Position -= Vector3.UnitZ * ratio;
            }
            Vector3 rotation = Vector3.Zero;
            if (state.IsPressed(Key.Right)) {
                rotation.Z += 0.05f;
            }
            if (state.IsPressed(Key.Left)) {
                rotation.Z -= 0.05f;
            }
            if (state.IsPressed(Key.Up)) {
                rotation.X += 0.05f;
            }
            if (state.IsPressed(Key.Down)) {
                rotation.X -= 0.05f;
            }
            q = Quaternion.RotationYawPitchRoll(rotation.Y, -rotation.X, rotation.Z);
            Rotation = q * Rotation;
        }

        public Matrix ViewMatrix {
            get {
                var m1 = Matrix.RotationQuaternion(Rotation);
                var m2 = Matrix.Translation(Position);
                var m3 = m2 * m1;
                m3.Invert();
                return m3;
            }
        }

        public Quaternion Rotation { get; set; }
        public Vector3 Position { get; set; }

    }
}
