using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;

namespace EvllyEngine
{
    public class BoxCollider
    {
        public Transform _parenntTransform;
        private BoxShape _shape;
        private CollisionObject _collisionObject;
        public Vector3 _Size;

        public BoxCollider(Transform ptransform)
        {
            _parenntTransform = ptransform;

            _Size = new Vector3(1,1,1);
            _shape = new BoxShape(_Size.X, _Size.Y, _Size.Z);

            _collisionObject = new CollisionObject();
            _collisionObject.WorldTransform = _parenntTransform.PositionMatrix;
            _collisionObject.CollisionShape = _shape;

            _collisionObject.CollisionFlags = CollisionFlags.StaticObject;
            _collisionObject.ActivationState = ActivationState.DisableSimulation;
            

            Physics.AddCollisionObject(_collisionObject);
        }

        public BoxCollider(Transform ptransform, Vector3 Size)
        {
            _parenntTransform = ptransform;

            _Size = Size;
            _shape = new BoxShape(_Size.X, _Size.Y, _Size.Z);

            _collisionObject = new CollisionObject();
            _collisionObject.WorldTransform = _parenntTransform.GetTransformWorld;
            _collisionObject.CollisionShape = _shape;

            _collisionObject.ActivationState = ActivationState.WantsDeactivation;
            _collisionObject.CollisionFlags = CollisionFlags.StaticObject;

            Physics.AddCollisionObject(_collisionObject);
        }

        public void OnDestroy()
        {
            Physics.RemoveCollisionObject(_collisionObject);
            _shape.Dispose();
        }
    }
}
