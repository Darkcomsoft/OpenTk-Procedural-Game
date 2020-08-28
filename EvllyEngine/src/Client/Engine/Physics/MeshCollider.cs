using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.BroadPhaseEntries;
using BEPUutilities;

namespace EvllyEngine
{
    public class MeshCollider
    {
        private StaticMesh MeshHandler;

        public MeshCollider(Transform transform, float[] vertices, int[] indices)
        {
            List<BEPUutilities.Vector3> points = new List<BEPUutilities.Vector3>();

            for (int i = 0; i < vertices.Length; i += 3)
            {
                points.Add(new BEPUutilities.Vector3(vertices[i], vertices[i + 1], vertices[i + 2]));
            }

            MeshHandler = new StaticMesh(points.ToArray(), indices, new AffineTransform(new BEPUutilities.Vector3(transform.Position.X, transform.Position.Y, transform.Position.Z)));
            Physics.Add(MeshHandler);
        }

        public void OnDestroy()
        {
            Physics.Remove(MeshHandler);
        }
    }
}
