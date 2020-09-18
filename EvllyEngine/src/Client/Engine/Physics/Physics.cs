using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using System.Runtime.CompilerServices;
using BEPUphysics;
using BEPUphysics.BroadPhaseEntries;
using BEPUutilities.Threading;

namespace EvllyEngine
{
    public class Physics
    {
        public static Physics _Main;

        public const float GravityX = 0;
        public const float GravityY = -9.81f;
        public const float GravityZ = 0;

        ///////New Engine
        private Space space;

        public Physics()
        {
            _Main = this;

            var parallelLooper = new ParallelLooper();
            //This section lets the engine know that it can make use of multithreaded systems
            //by adding threads to its thread pool.
            if (Environment.ProcessorCount > 1)
            {
                for (int i = 0; i < Environment.ProcessorCount; i++)
                {
                    parallelLooper.AddThread();
                }
            }

            space = new Space(parallelLooper);

            space.ForceUpdater.Gravity = new BEPUutilities.Vector3(0, -9.81f, 0);

            /*_BufferPool = new BufferPool();

            var targetThreadCount = Math.Max(1, Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
            _ThreadDispatcher = new SimpleThreadDispatcher(targetThreadCount);

            _Simulation = Simulation.Create(_BufferPool, new NarrowPhaseCallbacks(), new PoseIntegratorCallbacks(new System.Numerics.Vector3(GravityX, GravityY, GravityZ)), new PositionLastTimestepper());*/
        }

        public void UpdatePhisics(float time)
        {
            //_Simulation.Timestep(1 / 60f, _ThreadDispatcher);
            space.Update(time);
        }

        public void Dispose()
        {
            //New Engine
            
            /*_Simulation.Dispose();
            _BufferPool.Clear();
            _ThreadDispatcher.Dispose();*/
        }

        [Obsolete("This is for now Obsolete, we no more use BulletPhysics")]
        public static bool RayCast(Vector3 fromPosition, Vector3 toDirection)
        {
            return false;
        }

        [Obsolete("This is for now Obsolete, we no more use BulletPhysics")]
        public static bool RayCastSphere(Vector3 fromPosition, Vector3 toDirection)
        {
            return false;
        }

        [Obsolete("This is for now Obsolete, we no more use BulletPhysics")]
        public static bool RayCastAll(Vector3 fromPosition, Vector3 toDirection)
        {
            return false;
        }

        public static void Add(ISpaceObject obj)
        {
            Physics._Main.space.Add(obj);
        }

        public static void Remove(ISpaceObject obj)
        {
            Physics._Main.space.Remove(obj);
        }
    }
}

public enum ForceType : byte
{
    CentralForce, CentralImpulse, Torque, TorqueImpulse
}