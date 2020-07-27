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
        public GameObject gameObject;
        private BoxShape _shape;
        private CollisionObject _collisionObject;
        public Vector3 _Size;

        public BoxCollider(GameObject obj)
        {
            gameObject = obj;
            _Size = new Vector3(1,1,1);
            _shape = new BoxShape(_Size.X, _Size.Y, _Size.Z);

            _collisionObject = new CollisionObject();
            _collisionObject.WorldTransform = gameObject._transform.RotationMatrix * gameObject._transform.PositionMatrix * Matrix4.CreateScale(gameObject._transform.Size);
            _collisionObject.CollisionShape = _shape;

            Physics.AddCollisionObject(_collisionObject);
        }

        public BoxCollider(GameObject obj, Vector3 Size)
        {
            gameObject = obj;
            _Size = Size;
            _shape = new BoxShape(_Size.X, _Size.Y, _Size.Z);

            _collisionObject = new CollisionObject();
            _collisionObject.WorldTransform = gameObject._transform.RotationMatrix * gameObject._transform.PositionMatrix * Matrix4.CreateScale(gameObject._transform.Size);
            _collisionObject.CollisionShape = _shape;

            Physics.AddCollisionObject(_collisionObject);
        }

        public void Update()
        {
            _collisionObject.WorldTransform = gameObject._transform.RotationMatrix * gameObject._transform.PositionMatrix * Matrix4.CreateScale(gameObject._transform.Size);
        }

        public void OnDestroy()
        {
            Physics.RemoveCollisionObject(_collisionObject);
            _shape.Dispose();
            _shape = null;
        }
    }
}
