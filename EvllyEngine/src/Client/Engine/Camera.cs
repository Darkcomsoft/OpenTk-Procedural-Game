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
using ProjectEvlly.src;
using ProjectEvlly.src.Engine.Sound;

namespace EvllyEngine
{
    public class Camera : IDisposable
    {
        public Transform _transformParent;
        public Transform _cameraTrnasform;

        public static Camera Main;

        private AudioListener _audioListener;

        public float _fildOfView = 75;
        public float _nearPlane = 0.1f;
        public float _farPlane = 1000;

        private float _aspectRatio;

        //Camera Location
        public Matrix4 viewMatrix;
        //camera Lens
        public Matrix4 _projection;

        public Vector3 finalTarget;

        public Vector3 cameraTarget;
        public Vector3 cameraDirection;
        public Vector3 up = Vector3.UnitY;
        public Vector3 cameraRight;
        public Vector3 cameraUp;

        public Camera(Transform transformParent)
        {
            Main = this;

            _cameraTrnasform = new Transform();
            _transformParent = transformParent;

            _aspectRatio = (float)Window.Instance.Width / (float)Window.Instance.Height;
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fildOfView), _aspectRatio, _nearPlane, _farPlane);

            _audioListener = new AudioListener();//Start a camera lister for audio

            UpdateViewMatrix();
        }

        public void UpdateCamera()
        {
            _aspectRatio = (float)Window.Instance.Width / (float)Window.Instance.Height;
            _projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fildOfView), _aspectRatio, _nearPlane, _farPlane);

            /*cameraTarget = Vector3.Zero;
            cameraDirection = Vector3.Normalize(_cameraTrnasform.Position - cameraTarget);
            up = Vector3.UnitY;
            cameraRight = Vector3.Normalize(Vector3.Cross(up, cameraDirection));
            cameraUp = Vector3.Cross(cameraDirection, cameraRight);*/

            UpdateViewMatrix();
            _audioListener.UpdatePosition(-viewMatrix.ExtractTranslation());
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

        public void Dispose()
        {
            _audioListener.Dispose();

            _transformParent = null;
            _cameraTrnasform = null;
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