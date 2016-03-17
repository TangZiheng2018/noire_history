using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace Noire.VariablePipeline {
    public sealed class SimpleCamera2 {

        public SimpleCamera2() {
        }

        public void React(KeyboardState state, Matrix currentViewMatrix) {
            Vector4 tempV;
            if (state.IsPressed(Key.W)) {
                camPosition -= (camTarget);
                camPosition *= 0.95f;
                camPosition += camTarget;
            }
            if (state.IsPressed(Key.S)) {
                camPosition -= camTarget;
                camPosition /= 0.95f;
                camPosition += camTarget;
            }
            if (state.IsPressed(Key.A)) {
                camPosition -= camTarget;
                camTarget.X -= 1f;
                camPosition += camTarget;
            }
            if (state.IsPressed(Key.D)) {
                camPosition -= camTarget;
                camTarget.X += 1f;
                camPosition += camTarget;
            }
            if (state.IsPressed(Key.Up)) {
                camPosition -= camTarget;
                tempV = Vector3.Transform(
                    camPosition,
                    Matrix.RotationQuaternion(
                        Quaternion.RotationAxis(
                            new Vector3(
                            currentViewMatrix.M11,
                            currentViewMatrix.M21,
                            currentViewMatrix.M31),
                            angle)));
                camPosition.X = tempV.X + camTarget.X;
                camPosition.Y = tempV.Y + camTarget.Y;
                camPosition.Z = tempV.Z + camTarget.Z;
            }
            if (state.IsPressed(Key.Down)) {
                camPosition -= camTarget;
                tempV = Vector3.Transform(
                    camPosition,
                    Matrix.RotationQuaternion(
                        Quaternion.RotationAxis(
                            new Vector3(
                            currentViewMatrix.M11,
                            currentViewMatrix.M21,
                            currentViewMatrix.M31),
                            -angle)));
                camPosition.X = tempV.X + camTarget.X;
                camPosition.Y = tempV.Y + camTarget.Y;
                camPosition.Z = tempV.Z + camTarget.Z;
            }
            if (state.IsPressed(Key.Left)) {
                camPosition -= camTarget;
                tempV = Vector3.Transform(
                    camPosition,
                    Matrix.RotationQuaternion(Quaternion.RotationAxis(
                        new Vector3(currentViewMatrix.M12,
                                    currentViewMatrix.M22,
                                    currentViewMatrix.M32),
                        angle)));
                camPosition.X = tempV.X + camTarget.X;
                camPosition.Y = tempV.Y + camTarget.Y;
                camPosition.Z = tempV.Z + camTarget.Z;
            }
            if (state.IsPressed(Key.Right)) {
                camPosition -= camTarget;
                tempV = Vector3.Transform(
                    camPosition,
                    Matrix.RotationQuaternion(Quaternion.RotationAxis(
                        new Vector3(currentViewMatrix.M12,
                                    currentViewMatrix.M22,
                                    currentViewMatrix.M32),
                        -angle)));
                camPosition.X = tempV.X + camTarget.X;
                camPosition.Y = tempV.Y + camTarget.Y;
                camPosition.Z = tempV.Z + camTarget.Z;
            }
        }

        public Matrix ViewMatrix {
            get {
                return Matrix.LookAtLH(camPosition, camTarget, camUp);
            }
        }

        private Vector3 camPosition = new Vector3(0, 30, -50);
        private Vector3 camTarget = new Vector3(0, 0, 0);
        private Vector3 camUp = new Vector3(0, 1, 0);
        private float angle = MathUtil.DegreesToRadians(5);

    }
}
