using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;
using OpenTK;

namespace EvllyEngine
{
    public class Physics
    {
        public static Physics _Main;
        public DiscreteDynamicsWorld _World { get; set; }
        CollisionDispatcher dispatcher;
        DbvtBroadphase broadphase;
        CollisionConfiguration collisionConf;

        public Physics()
        {
            _Main = this;

            collisionConf = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConf);
            
            broadphase = new DbvtBroadphase();
            _World = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConf);
            _World.Gravity = new Vector3(0, -10, 0);
        }

        public void UpdatePhisics(float time)
        {
            _World.StepSimulation(time);
        }

        public void Dispose()
        {
            for (int i = _World.NumConstraints - 1; i >= 0; i--)
            {
                TypedConstraint constraint = _World.GetConstraint(i);
                _World.RemoveConstraint(constraint);
                constraint.Dispose();
            }

            _World.Dispose();
            broadphase.Dispose();
            if (dispatcher != null)
            {
                dispatcher.Dispose();
            }
            collisionConf.Dispose();
        }

        public static bool RayCast(Vector3 fromPosition, Vector3 toDirection, out ClosestRayResultCallback hit)
        {
            ClosestRayResultCallback callback = new ClosestRayResultCallback(ref fromPosition, ref toDirection);
            
            Physics._Main._World.RayTest(fromPosition, toDirection, callback);
            hit = callback;

            return callback.HasHit;
        }

        public static bool RayCastSphere(Vector3 fromPosition, Vector3 toDirection, out ClosestRayResultCallback hit)
        {
            ClosestRayResultCallback callback = new ClosestRayResultCallback(ref fromPosition, ref toDirection);

            Physics._Main._World.RayTest(fromPosition, toDirection, callback);
            hit = callback;

            return callback.HasHit;
        }

        public static bool RayCastAll(Vector3 fromPosition, Vector3 toDirection, out AllHitsRayResultCallback hit)
        {
            AllHitsRayResultCallback callback = new AllHitsRayResultCallback(fromPosition, toDirection);

            Physics._Main._World.RayTest(fromPosition, toDirection, callback);
            hit = callback;

            return callback.HasHit;
        }

        public static void AddCollisionObject(CollisionObject obj)
        {
            _Main._World.AddCollisionObject(obj);
        }

        public static void RemoveCollisionObject(CollisionObject obj)
        {
            _Main._World.RemoveCollisionObject(obj);
        }

        public static void AddRigidBody(BulletSharp.RigidBody obj)
        {
            _Main._World.AddRigidBody(obj);
        }

        public static void RemoveRigidBody(BulletSharp.RigidBody obj)
        {
            _Main._World.RemoveRigidBody(obj);
        }
    }
}

public enum ForceType : byte
{
    CentralForce, CentralImpulse, Torque, TorqueImpulse
}