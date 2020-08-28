using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class Transform
    {
        private Vector3 _Position;
        private Quaternion _Rotation;
        private Vector3 _Size;

        public Transform() 
        {
            _Position = new Vector3(0,0,0);
            _Rotation = Quaternion.Identity;
            _Size = new Vector3(1, 1, 1);
        }
        public Transform(Vector3 position, Quaternion rotation, Vector3 size)
        {
            _Position = position;
            _Rotation = rotation;
            _Size = size;
        }
        public Transform(Vector3 position, Quaternion rotation)
        {
            _Position = position;
            _Rotation = rotation;
            _Size = Vector3.One;
        }

        public Transform(Vector3 position)
        {
            _Position = position;
            _Rotation = Quaternion.Identity;
            _Size = Vector3.One;
        }

        public Matrix4 PositionMatrix { get { return Matrix4.CreateTranslation(_Position); } }
        public Matrix4 RotationMatrix { get { return Matrix4.CreateRotationX(_Rotation.X) * Matrix4.CreateRotationY(_Rotation.Y) * Matrix4.CreateRotationZ(_Rotation.Z); } }
        public Matrix4 GetTransformWorld 
        { 
            get 
            {
                return Matrix4.CreateScale(_Size) * RotationMatrix * PositionMatrix;
            } 
        }

        public Vector3 Position
        {
            get
            {
                return _Position;
            }
            set { _Position = value; }
        }
        public Quaternion Rotation
        {
            get
            {
                return _Rotation;
            }
            set { _Rotation = value; }
        }
        public Vector3 Size
        {
            get
            {
                return _Size;
            }
            set { _Size = value; }
        }
    }
}
