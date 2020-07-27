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

        public GameObject _gameObject;
        public Transform Root;
        public bool IsRoot;

        public Transform() { IsRoot = true; }
        public Transform(Transform tran) : this() { IsRoot = true; }
        public Transform(Vector3 position, Quaternion rotation, Vector3 size)
        {
            _Position = position;
            _Rotation = rotation;
            _Size = size;
            IsRoot = true;
        }
        public Transform(Vector3 position, Quaternion rotation)
        {
            _Position = position;
            _Rotation = rotation;
            _Size = Vector3.One;
            IsRoot = true;
        }

        public Matrix4 PositionMatrix { get { return Matrix4.CreateTranslation(_Position); } }
        public Matrix4 RotationMatrix { get { return Matrix4.CreateRotationX(MathHelper.DegreesToRadians(_Rotation.X)) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(_Rotation.Y)) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(_Rotation.Z)); } }
        public Matrix4 GetTransformWorld 
        { 
            get 
            { 
                if (IsRoot)
                {
                    if (_gameObject.HaveRigid)
                    {
                        return _gameObject.GetRigidBody().GetWorld * Matrix4.CreateScale(_Size);
                    }
                    else
                    {
                        return RotationMatrix * PositionMatrix * Matrix4.CreateScale(_Size);
                    }
                }
                else
                {
                    //applyParent position/rotation/size plus the chuld position localposition/rotation/size
                    _Position = Root._Position;
                    return (RotationMatrix * PositionMatrix * Matrix4.CreateScale(_Size)) + (Root.RotationMatrix * Root.PositionMatrix * Matrix4.CreateScale(Root._Size));
                }
            } 
        }

        public Vector3 Position
        {
            get
            {
                if (IsRoot)
                {
                    if (_gameObject.HaveRigid)
                    {
                        _Position = _gameObject.GetRigidBody().GetWorld.ExtractTranslation();
                        return _Position;
                    }
                    else
                    {
                        return _Position;
                    }
                }
                else
                {
                    return _Position + Root.Position;
                }
            }
            set { _Position = value; }
        }
        public Quaternion Rotation
        {
            get
            {
                if (IsRoot)
                {
                    return _Rotation;
                }
                else
                {
                    return _Rotation + Root._Rotation;
                }
            }
            set { _Rotation = value; }
        }
        public Vector3 Size
        {
            get
            {
                if (IsRoot)
                {
                    return _Size;
                }
                else
                {
                    return _Size + Root._Size;
                }
            }
            set { _Size = value; }
        }

        public void Update()
        {

        }

        public void SetChild(Transform root)
        {
            Root = root;
            IsRoot = false;
        }
        public void RemoveChild()
        {
            Root = null;
            IsRoot = true;
        }
    }
}
