using OpenTK;
using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class RigidBody
    {
        public GameObject gameObject;
        public float _Mass;
        public bool _Static;
        private BulletSharp.RigidBody rigidBodyObject;
        public CollisionShape _Shape;

        public RigidBody(GameObject Object)
        {
            gameObject = Object;
            _Mass = 1;
            _Static = false;
            _Shape = new CapsuleShape(0.5f, 1);
            rigidBodyObject = LocalCreateRigidBody(_Mass, Matrix4.CreateTranslation(gameObject._transform.Position), _Shape);
            rigidBodyObject.CollisionFlags = CollisionFlags.CharacterObject;
            rigidBodyObject.Friction = 0.1f;
            rigidBodyObject.SetDamping(0,0);
        }

        public void Update()
        {
            rigidBodyObject.Activate();
            //rigidBodyObject.WorldTransform = gameObject._transform.RotationMatrix * rigidBodyObject.WorldTransform * Matrix4.CreateScale(gameObject._transform._Size);
        }

        public BulletSharp.RigidBody LocalCreateRigidBody(float mass, Matrix4 startTransform, CollisionShape shape)
        {
            bool isDynamic = (mass != 0.0f);

            Vector3 localInertia = Vector3.Zero;
            if (isDynamic || _Static == false)
            {
                shape.CalculateLocalInertia(mass, out localInertia);
            }

            DefaultMotionState myMotionState = new DefaultMotionState(startTransform);

            RigidBodyConstructionInfo rbInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);
            BulletSharp.RigidBody body = new BulletSharp.RigidBody(rbInfo);
            
            Physics.AddRigidBody(body);

            return body;
        }

        /// <summary>
        /// this is for moving the rigidbody with Physics, the rigidbody whill interact with collision
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Vector3 direction, bool gravity)
        {
            if (gravity)
            {
                rigidBodyObject.LinearVelocity = new Vector3(direction.X, rigidBodyObject.Gravity.Y, direction.Z);
            }
            else
            {
                rigidBodyObject.LinearVelocity = direction;
            }
            rigidBodyObject.AngularFactor = Vector3.Zero;
            rigidBodyObject.AngularVelocity = Vector3.Zero;
            //rigidBodyObject.WorldTransform = gameObject._transform.RotationMatrix * rigidBodyObject.WorldTransform * Matrix4.CreateScale(gameObject._transform._Size);
        }

        /// <summary>
        /// Just move the RigidBody without Physics, just translate in 3d space
        /// </summary>
        /// <param name="direction"></param>
        public void MoveNoPhysics(Vector3 direction)
        {
            rigidBodyObject.Translate(direction);
        }

        public void Force(Vector3 direction, ForceType forceType)
        {
            switch (forceType)
            {
                case ForceType.CentralForce:
                    rigidBodyObject.ApplyCentralForce(direction);
                    break;
                case ForceType.CentralImpulse:
                    rigidBodyObject.ApplyCentralImpulse(direction);
                    break;
                case ForceType.Torque:
                    rigidBodyObject.ApplyTorque(direction);
                    break;
                case ForceType.TorqueImpulse:
                    rigidBodyObject.ApplyTorqueImpulse(direction);
                    break;
                default:
                    rigidBodyObject.ApplyForce(direction, gameObject._transform.Position);
                    break;
            }
        }

        public void OnDestroy()
        {
            Physics.RemoveRigidBody(rigidBodyObject);
            _Shape.Dispose();
            gameObject = null;
            _Shape = null;
        }

        public Matrix4 GetWorld { get { return rigidBodyObject.WorldTransform; } }
    }
}
