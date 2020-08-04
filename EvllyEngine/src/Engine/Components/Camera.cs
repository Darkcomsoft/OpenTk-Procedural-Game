using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ProjectEvlly;

namespace EvllyEngine
{
    public class Camera
    {
        public Transform _transformParent;
        public Transform _cameraTrnasform;

        public static Camera Main;

        public float _fildOfView = 60;
        public float _nearPlane = 0.1f;
        public float _farPlane = 100;

        private float _aspectRatio;

        //Camera Location
        public Matrix4 viewMatrix;
        //camera Lens
        public Matrix4 _projection;

        public Vector3 finalTarget;

        public Camera(Transform transformParent)
        {
            Main = this;

            _cameraTrnasform = new Transform();
            _transformParent = transformParent;

            _aspectRatio = (float)Game.Instance.Width / (float)Game.Instance.Height;
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fildOfView), _aspectRatio, _nearPlane, _farPlane);

            UpdateViewMatrix();
        }

        public void UpdateCamera()
        {
            _aspectRatio = (float)Game.Instance.Width / (float)Game.Instance.Height;
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fildOfView), _aspectRatio, _nearPlane, _farPlane);

            /*Vector3 cameraTarget = Vector3.Zero;
            Vector3 cameraDirection = Vector3.Normalize(gameObject._transform._Position - cameraTarget);
            Vector3 up = Vector3.UnitY;
            Vector3 cameraRight = Vector3.Normalize(Vector3.Cross(up, cameraDirection));
            Vector3 cameraUp = Vector3.Cross(cameraDirection, cameraRight);*/

            UpdateViewMatrix();
		}

        private void UpdateViewMatrix()
        {
            var camRotation = Matrix3.CreateRotationX(_cameraTrnasform.Rotation.X + _transformParent.Rotation.X) * Matrix3.CreateRotationY(_cameraTrnasform.Rotation.Y + _transformParent.Rotation.Y) * Matrix3.CreateRotationZ(_cameraTrnasform.Rotation.Z + _transformParent.Rotation.Z);

            var camOriginalTarget = new Vector3(0, 0, 1);
            var camRotatedTarget = Vector3.Transform(camOriginalTarget, camRotation);
            finalTarget = new Vector3(_cameraTrnasform.Position + _transformParent.Position) + camRotatedTarget;

            var camOriginalUpVector = new Vector3(0, 1, 0);
            var camRotatedUpVector = Vector3.Transform(camOriginalUpVector, camRotation);

            viewMatrix = Matrix4.LookAt(_cameraTrnasform.Position + _transformParent.Position, finalTarget, camRotatedUpVector);
        }

        public void OnDestroy()
        {
            _transformParent = null;
            Main = null;
        }

        public static float WrapAngle(float angle)
        {
            if ((angle > -MathHelper.Pi) && (angle <= MathHelper.Pi))
                return angle;
            angle %= MathHelper.TwoPi;
            if (angle <= -MathHelper.Pi)
                return angle + MathHelper.TwoPi;
            if (angle > MathHelper.Pi)
                return angle - MathHelper.TwoPi;
            return angle;
        }
    }
}