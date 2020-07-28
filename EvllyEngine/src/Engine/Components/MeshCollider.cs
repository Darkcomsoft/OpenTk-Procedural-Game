using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp;

namespace EvllyEngine
{
    public class MeshCollider
    {
        private TriangleMeshShape _shape;
        private CollisionObject _collisionObject;
        private StridingMeshInterface meshInter;

        TriangleIndexVertexArray indexVertexArray;
        TriangleIndexVertexArray indexVertexArray2;
        GImpactMeshShape gImpactMeshShape;
        BvhTriangleMeshShape triangleMeshShape;

        List<Vector3> MeshVertices = new List<Vector3>();
        List<int> MeshIndices = new List<int>();

        public MeshCollider(MeshRender meshRender)
        {
            MeshIndices.AddRange(meshRender._mesh._indices);

            for (int i = 0; i < meshRender._mesh._vertices.Length; i += 3)
            {
                Vector3 pos = new Vector3(meshRender._mesh._vertices[i], meshRender._mesh._vertices[i + 1], meshRender._mesh._vertices[i + 2]);
                MeshVertices.Add(pos);
            }
            //ANTIGO ANTIGO ANTIGO

            // Initialize TriangleIndexVertexArray with float array
            /*indexVertexArray = new TriangleIndexVertexArray(MeshIndices.ToArray(), MeshVertices.ToArray());
            gImpactMeshShape = new GImpactMeshShape(indexVertexArray);
            gImpactMeshShape.CalculateLocalInertia(1.0f);
            gImpactMesh = CreateBody(1.0f, gImpactMeshShape, Vector3.Zero);*/


            indexVertexArray2 = new TriangleIndexVertexArray(MeshIndices.ToArray(), MeshVertices.ToArray());
            triangleMeshShape = new BvhTriangleMeshShape(indexVertexArray2, true);

            _collisionObject = new CollisionObject();
            _collisionObject.WorldTransform = meshRender.transform.RotationMatrix * meshRender.transform.PositionMatrix * Matrix4.CreateScale(meshRender.transform.Size);
            _collisionObject.CollisionShape = triangleMeshShape;

            //TestTriangleArray(indexVertexArray);
            //TestTriangleArray(indexVertexArray2);

            Vector3 aabbMin, aabbMax;
            //gImpactMeshShape.GetAabb(Matrix4.Identity, out aabbMin, out aabbMax);
            triangleMeshShape.GetAabb(Matrix4.Identity, out aabbMin, out aabbMax);

            Physics.AddCollisionObject(_collisionObject);
        }

        public void Update()
        {
            //_collisionObject.WorldTransform = gameObject._transform.RotationMatrix * gameObject._transform.PositionMatrix * Matrix4.CreateScale(gameObject._transform._Size);
        }

        public void OnDestroy()
        {
            Physics.RemoveCollisionObject(_collisionObject);

            /*gImpactMesh.MotionState.Dispose();
            triangleMesh.MotionState.Dispose();
            gImpactMesh.Dispose();
            triangleMesh.Dispose();*/
            //gImpactMeshShape.Dispose();
            triangleMeshShape.Dispose();
            //indexVertexArray.Dispose();
            indexVertexArray2.Dispose();

            MeshVertices.Clear();
            MeshIndices.Clear();
        }
    }
}
