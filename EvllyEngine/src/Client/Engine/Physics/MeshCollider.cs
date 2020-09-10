using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.BroadPhaseEntries;

namespace EvllyEngine
{
    public class MeshCollider : IDisposable
    {
        private StaticMesh MeshHandler;

        public MeshCollider(Transform transform, Vector3[] vertices, int[] indices)
        {
            BEPUutilities.Vector3[] points = new BEPUutilities.Vector3[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                points[i] = new BEPUutilities.Vector3(vertices[i].X, vertices[i].Y, vertices[i].Z);
            }

            MeshHandler = new StaticMesh(points, indices, new BEPUutilities.AffineTransform(new BEPUutilities.Vector3(transform.Position.X, transform.Position.Y, transform.Position.Z)));
            Physics.Add(MeshHandler);
        }

        public void UpdateCollider(Transform transform, Mesh mesh)
        {
            if (mesh != null)
            {
                Mesh _mesh = mesh;

                if (MeshHandler != null)
                {
                    Physics.Remove(MeshHandler);
                    MeshHandler = null;
                }

                BEPUutilities.Vector3[] points = new BEPUutilities.Vector3[_mesh._vertices.Length];

                for (int i = 0; i < _mesh._vertices.Length; i++)
                {
                    points[i] = new BEPUutilities.Vector3(_mesh._vertices[i].X, _mesh._vertices[i].Y, _mesh._vertices[i].Z);
                }

                MeshHandler = new StaticMesh(points, _mesh._indices, new BEPUutilities.AffineTransform(new BEPUutilities.Vector3(transform.Position.X, transform.Position.Y, transform.Position.Z)));
                Physics.Add(MeshHandler);

                _mesh = null;
            }
        }

        public void Dispose()
        {
            if (MeshHandler != null)
            {
                Physics.Remove(MeshHandler);
                MeshHandler = null;
            }
        }
    }
}
