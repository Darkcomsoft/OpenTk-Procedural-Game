using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace EvllyEngine
{
    public class Mesh
    {
        public float[] _vertices;
        public int[] _indices;
        public float[] _texCoords;
        public float[] _Colors;
        public float[] _Normals;

        public Mesh(Mesh newmesh)
        {
            _vertices = newmesh._vertices;
            _indices = newmesh._indices;
            _texCoords = newmesh._texCoords;
            _Colors = newmesh._Colors;

            OptimizeMesh();
            _Normals = CalculateNormals();
        }

        public Mesh() 
        {
            _vertices = new float[]
            {
                 //Position          Texture coordinates
                 0.5f,  0.5f, 0.0f, // top right
                 0.5f, -0.5f, 0.0f, // bottom right
                -0.5f, -0.5f, 0.0f, // bottom left
                -0.5f,  0.5f, 0.0f // top left
            };

            _indices = new int[]
            {
                0, 1, 3,   // first triangle
                1, 2, 3    // second triangle
            };

            _Colors = new float[] {
                1.0f,1.0f,1.0f,1.0f,
                1.0f,1.0f,1.0f,1.0f,
                1.0f,1.0f,1.0f,1.0f
            };

            _texCoords = new float[] {
                1.0f, 1.0f,
                1.0f, 0.0f,
                0.0f, 0.0f,
                0.0f, 1.0f
            };

            OptimizeMesh();
            _Normals = CalculateNormals();
        }
        public Mesh(float[] vertices, int[] indices, float[] colors)
        {
            _vertices = vertices;
            _indices = indices;
            _Colors = colors;

            OptimizeMesh();
            _Normals = CalculateNormals();
        }
        public Mesh(float[] vertices, int[] indices)
        {
            _vertices = vertices;
            _indices = indices;

            OptimizeMesh();
            _Normals = CalculateNormals();
        }

        public Mesh(float[] vertices, float[] textures, float[] colors, int[] indices)
        {
            _vertices = vertices;
            _indices = indices;
            _texCoords = textures;
            _Colors = colors;

            OptimizeMesh();
            _Normals = CalculateNormals();
        }

        public Mesh(float[] vertices, Vector3[] normals, float[] textures, float[] colors, int[] indices)
        {
            _vertices = vertices;
            _indices = indices;
            _texCoords = textures;
            _Colors = colors;

            OptimizeMesh();
            _Normals = CalculateNormals();
        }

        public void OptimizeMesh()
        {
            /*List<Vector3> vertTo = new List<Vector3>();
            List<uint> indTo = new List<uint>();

            List<float> finalvert = new List<float>();
            List<int> finalindice = new List<int>();

            for (int i = 0; i < _vertices.Length; i += 3)
            {
                vertTo.Add(new Vector3(_vertices[i], _vertices[i + 1], _vertices[i + 2]));
            }

            for (int i = 0; i < _indices.Length; i++)
            {
                indTo.Add((uint)_indices[i]);
            }

             Tuple<Vector3[], uint[]> u = MeshOptimizer.MeshOperations.Optimize<Vector3>(vertTo.ToArray(), indTo.ToArray(), sizeof(float) * 3);

            for (int i = 0; i < u.Item1.Length; i++)
            {
                finalvert.Add(u.Item1[i].X);
                finalvert.Add(u.Item1[i].Y);
                finalvert.Add(u.Item1[i].Z);
            }

            for (int i = 0; i < u.Item2.Length; i++)
            {
                finalindice.Add((int)u.Item2[i]);
            }

            _vertices = finalvert.ToArray();
            _indices = finalindice.ToArray();*/
        }

        public float[] CalculateNormals()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<float> finalNormals = new List<float>();


            for (int i = 0; i < _vertices.Length; i+=3)
            {
                vertices.Add(new Vector3(_vertices[i], _vertices[i + 1], _vertices[i + 2]));
            }

            Vector3[] normals = new Vector3[vertices.Count];
            int[] inds = _indices;

            // Compute normals for each face
            for (int i = 0; i < inds.Length; i += 3)
            {
                Vector3 v1 = vertices[inds[i]];
                Vector3 v2 = vertices[inds[i + 1]];
                Vector3 v3 = vertices[inds[i + 2]];

                // The normal is the cross product of two sides of the triangle
                normals[inds[i]] += Vector3.Cross(v2 - v1, v3 - v1);
                normals[inds[i + 1]] += Vector3.Cross(v2 - v1, v3 - v1);
                normals[inds[i + 2]] += Vector3.Cross(v2 - v1, v3 - v1);
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                normals[i] = normals[i].Normalized();

                finalNormals.Add(normals[i].X);
                finalNormals.Add(normals[i].Y);
                finalNormals.Add(normals[i].Z);
            }

            return finalNormals.ToArray();
        }

        public void Clear()
        {
            _vertices = null;
            _indices = null;
            _texCoords = null;
            _Colors = null;
        }
    }
}
