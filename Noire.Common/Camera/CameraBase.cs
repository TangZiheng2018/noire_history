using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Noire.Common.Camera {
    public abstract class CameraBase {

        public Vector3 Position { get; set; }
        public Vector3 Right { get; protected set; }
        public Vector3 Up { get; protected set; }
        public Vector3 Look { get; protected set; }
        public float NearZ { get; protected set; }
        public float FarZ { get; protected set; }
        public float Aspect { get; protected set; }
        public float FovY { get; protected set; }

        public float FovX {
            get {
                var halfWidth = 0.5f * NearWindowWidth;
                return 2.0f * MathF.Atan(halfWidth / NearZ);
            }
        }

        public float NearWindowWidth => Aspect * NearWindowHeight;

        public float NearWindowHeight { get; protected set; }

        public float FarWindowWidth => Aspect * FarWindowHeight;

        public float FarWindowHeight { get; protected set; }
        public Matrix ViewMatrix { get; protected set; }
        public Matrix ProjectionMatrix { get; protected set; }

        public Matrix ViewProjectionMatrix => ViewMatrix * ProjectionMatrix;

        public Plane[] FrustumPlanes => _frustum.Planes;

        public abstract void LookAt(Vector3 eye, Vector3 target, Vector3 up);
        public abstract void Strafe(float rightDistance);
        public abstract void Walk(float frontDistance);
        public abstract void Pitch(float angle);
        public abstract void Yaw(float angle);
        public abstract void Zoom(float dr);
        public abstract void UpdateViewMatrix();

        public void LookAt(Vector3 target, Vector3 up) {
            LookAt(Position, target, up);
        }

        public void LookAt(Vector3 target) {
            LookAt(Position, target, Up);
        }

        public void LookAtZUp(Vector3 target) {
            LookAt(Position, target, Vector3.UnitZ);
        }

        public bool IsBoundingBoxVisible(BoundingBox box) => _frustum.Intersects(box) != IntersectionState.NoIntersection;

        protected CameraBase() {
            Position = Vector3.Zero;
            //Right = new Vector3(1, 0, 0);
            //Up = new Vector3(0, 1, 0);
            //Look = new Vector3(0, 0, 1);
            ViewMatrix = Matrix.Identity;
            ProjectionMatrix = Matrix.Identity;
        }

        protected Frustum _frustum;

    }

}
