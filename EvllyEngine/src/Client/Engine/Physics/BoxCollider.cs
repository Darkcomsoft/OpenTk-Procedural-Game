using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.Entities.Prefabs;

namespace EvllyEngine
{
    public class BoxCollider
    {
        public Transform _parenntTransform;
        private Box BoxHandler;
        public Vector3 _Size;

        public BoxCollider(Transform ptransform)
        {
            _parenntTransform = ptransform;

            _Size = new Vector3(1,1,1);

            BoxHandler = new Box(new BEPUutilities.Vector3(ptransform.Position.X, ptransform.Position.Y, ptransform.Position.Z), _Size.X, _Size.Y, _Size.Z);

            Physics.Add(BoxHandler);
        }

        public BoxCollider(Transform ptransform, Vector3 Size)
        {
            _parenntTransform = ptransform;
            _Size = Size;

            BoxHandler = new Box(new BEPUutilities.Vector3(ptransform.Position.X, ptransform.Position.Y, ptransform.Position.Z), _Size.X, _Size.Y, _Size.Z);

            Physics.Add(BoxHandler);
        }

        public void OnDestroy()
        {
            Physics.Remove(BoxHandler);
        }
    }
}
