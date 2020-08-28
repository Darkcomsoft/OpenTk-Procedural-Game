using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class BoxCollider
    {
        public Transform _parenntTransform;
        public Vector3 _Size;

        public BoxCollider(Transform ptransform)
        {
            _parenntTransform = ptransform;
        }

        public BoxCollider(Transform ptransform, Vector3 Size)
        {
            _parenntTransform = ptransform;
        }

        public void OnDestroy()
        {

        }
    }
}
