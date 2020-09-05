using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ProjectEvlly;
using ProjectEvlly.src;
using ProjectEvlly.src.Engine.Render;
using ProjectEvlly.src.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class Chunk
    {
        public Transform transform;
        private double ChunkSeed;

        private Block[,] blocks;

        public ChunkMeshRender _meshRender;
        private MeshCollider _meshCollider;

        private List<Tree> _trees = new List<Tree>();

        private Mesh ChunkMesh;
        private Queue<Action> ActionUpdateMesh = new Queue<Action>();
        private object LockActionUpdateMEsh;

        public Chunk(Vector3 position)
        {
            transform = new Transform();
            System.Random rand = new System.Random();

            transform.Position = position;

            blocks = new Block[MidleWorld.ChunkSize, MidleWorld.ChunkSize];

            LockActionUpdateMEsh = new object();

            double a = rand.NextDouble();
            double b = rand.NextDouble();

            ChunkSeed = transform.Position.X * a + transform.Position.Z * b + 0;

            ThreadMakeMesh();


            Game.Client.TickEvent += Update;
        }

        public void Update()
        {
            while (ActionUpdateMesh.Count > 0)
            {
                lock (LockActionUpdateMEsh)
                {
                    ActionUpdateMesh.Dequeue().Invoke();
                }
            }
        }

        public void OnDestroy()
        {
            for (int i = 0; i < _trees.Count; i++)
            {
                _trees[i].OnDestroy();
            }

            _trees.Clear();

            Game.Client.TickEvent -= Update;

            if (_meshRender != null)
            {
                RenderSystem.RemoveRenderItem(_meshRender);
            }

            if (ChunkMesh != null)
            {
                ChunkMesh.Clear();
            }

            if (_meshCollider != null)
            {
                _meshCollider.OnDestroy();
            }
        }

        private void ThreadMakeMesh()
        {
            for (int x = 0; x < MidleWorld.ChunkSize; x++)
            {
                for (int z = 0; z < MidleWorld.ChunkSize; z++)
                {
                    blocks[x, z] = new Block(x + (int)transform.Position.X, z + (int)transform.Position.Z, transform.Position, MidleWorld.globalNoise.GetPerlin(x + (int)transform.Position.X, z + (int)transform.Position.Z) * 50);
                }
            }

            MeshData data = new MeshData(blocks);

            if (data._vertices.Count > 0)
            {
                if (ChunkMesh != null)
                {
                    ChunkMesh.Clear();
                }

                ChunkMesh = new Mesh(data._vertices.ToArray(), data._UVs.ToArray(), new float[] { }, data._triangles.ToArray());

                lock (LockActionUpdateMEsh)
                {
                    ActionUpdateMesh.Enqueue(() => MakeMesh());
                }
            }
        }

        private void MakeMesh()
        {
            if (_meshCollider != null)
            {
                _meshCollider.OnDestroy();
            }

            if (_meshRender != null)
            {
                RenderSystem.RemoveRenderItem(_meshRender);
            }

            _meshRender = new ChunkMeshRender(transform, ChunkMesh, AssetsManager.GetShader("TerrainDefault"), AssetsManager.GetTexture("TileAtlas"));
            _meshCollider = new MeshCollider(transform, _meshRender._mesh._vertices, _meshRender._mesh._indices);

            RenderSystem.AddRenderItem(_meshRender);

            for (int x = 0; x < MidleWorld.ChunkSize; x++)
            {
                for (int z = 0; z < MidleWorld.ChunkSize; z++)
                {
                    if (blocks[x, z].treeType != TreeType.none)
                    {
                        _trees.Add(new Tree(new Vector3(blocks[x, z].x, blocks[x, z].height, blocks[x, z].z)));
                    }
                }
            }
        }

        public void RefreshMeshData()
        {
            Thread MakeMeshThread = new Thread(new ThreadStart(ThreadMakeMesh));
            MakeMeshThread.Start();
        }

        public Block[,] GetBlocksMap { get { return blocks; } }
    }

    public struct Block
    {
        public float height;

        public byte index;

        public int x;
        public int z;

        public TypeBlock Type;
        public TreeType treeType;

        public Vector3 Chunk;

        public Block(int _x, int _z, Vector3 chunkPosition, float _Height)
        {
            index = 0;
            x = _x;
            z = _z;
            System.Random rand = new System.Random(0 + x * z);
            height = _Height;

            Chunk = chunkPosition;

            treeType = TreeType.none;
            Type = TypeBlock.Air;

            if (_Height <= 0)
            {
                Type = TypeBlock.Dirt;
            }
            else
            {
                if (rand.Next(0, 20) == 10)
                {
                    Type = TypeBlock.Dirt;
                }
                else
                {
                    Type = TypeBlock.Grass;
                }

                if (rand.Next(0, 20) == 1)
                {
                    treeType = TreeType.Oak;
                }
            }
        }

        public string ToString()
        { 
            return "Hight: " + height + ", Index: " + index + ", Type: " + Type + " Chunk:" + Chunk;
        }
    }

    public interface IMarching
    {

        float Surface { get; set; }

        void Generate(IList<float> voxels, int width, int height, int depth, IList<Vector3> verts, IList<int> indices);

    }

    public class MeshData
    {
        public List<float> _vertices;
        public List<float> _UVs;
        public List<int> _triangles;
        public List<float> _colors;

        public bool _HaveWater;

        public MeshData(Block[,] tile)
        {
            _vertices = new List<float>();
            _UVs = new List<float>();
            _triangles = new List<int>();
            _colors = new List<float>();

            int verticesNum = 0;

            for (int x = 0; x < MidleWorld.ChunkSize; x++)
            {
                for (int z = 0; z < MidleWorld.ChunkSize; z++)
                {
                    if (tile[x, z].Type != TypeBlock.Air)
                    {
                        int xB = tile[x, z].x;
                        int zB = tile[x, z].z;

                        float Right = GetTile(xB + 1, zB, tile[x, z].height, tile);
                        float FrenteRight = GetTile(xB, zB + 1, tile[x, z].height, tile);
                        float FrenteLeft = GetTile(xB + 1, zB + 1, tile[x, z].height, tile);

                        _vertices.Add(x);
                        _vertices.Add(MidleWorld.globalNoise.GetPerlin(xB, zB) * 50);
                        _vertices.Add(z);
                        

                        _vertices.Add(x + 1);
                        _vertices.Add(MidleWorld.globalNoise.GetPerlin(xB + 1, zB) * 50);
                        _vertices.Add(z);
                        

                        _vertices.Add(x);
                        _vertices.Add(MidleWorld.globalNoise.GetPerlin(xB, zB + 1) * 50);
                        _vertices.Add(z + 1);
                        

                        _vertices.Add(x + 1);
                        _vertices.Add(MidleWorld.globalNoise.GetPerlin(xB + 1, zB + 1) * 50);
                        _vertices.Add(z + 1);
                        

                        _triangles.Add(0 + verticesNum);
                        _triangles.Add(1 + verticesNum);
                        _triangles.Add(2 + verticesNum);

                        _triangles.Add(2 + verticesNum);
                        _triangles.Add(1 + verticesNum);
                        _triangles.Add(3 + verticesNum);
                        //Color blockcolor = Get.GetColorTile(tile[x, z]);
                        verticesNum += 4;
                        _colors.Add(1);
                        _colors.Add(1);
                        _colors.Add(1);
                        _colors.Add(1);

                        _colors.Add(1);
                        _colors.Add(1);
                        _colors.Add(1);
                        _colors.Add(1);

                        _colors.Add(1);
                        _colors.Add(1);
                        _colors.Add(1);
                        _colors.Add(1);

                        _colors.Add(1);
                        _colors.Add(1);
                        _colors.Add(1);
                        _colors.Add(1);

                        _UVs.AddRange(AssetsManager.GetTileUV(tile[x, z].Type.ToString()));

                        /*_UVs.Add(0.15f);
                        _UVs.Add(0.066667f);

                        _UVs.Add(0.15f);
                        _UVs.Add(0f);

                        _UVs.Add(0.2f);
                        _UVs.Add(0.066667f);

                        _UVs.Add(0.2f);
                        _UVs.Add(0f);*/

                        //_UVs.AddRange(Game.AssetsManager.GetTileUVs(tile[x, z]));
                    }
                }
            }
        }

        public VoxelMesh MakeWaterMesh(Block[,] tile)
        {
            List<float> verticesFinal = new List<float>();
            Vector3[] vertices;
            List<float> uvs = new List<float>();
            int[] triangles;
            List<float> colors = new List<float>();

            VoxelMesh mesh = new VoxelMesh();

            int widh = MidleWorld.ChunkSize + 1;

            vertices = new Vector3[widh * widh];
            for (int y = 0; y < widh; y++)
            {
                for (int x = 0; x < widh; x++)
                {
                    vertices[x + y * widh] = new Vector3(x, 0.0f, y);

                    colors.Add(1);
                    colors.Add(1);
                    colors.Add(1);
                    colors.Add(1);

                    uvs.AddRange(AssetsManager.GetTileUV(tile[x, y].Type.ToString()));
                }
            }

            triangles = new int[3 * 2 * (widh * widh - widh - widh + 1)];
            int triangleVertexCount = 0;
            for (int vertex = 0; vertex < widh * widh - widh; vertex++)
            {
                if (vertex % widh != (widh - 1))
                {
                    // First triangle
                    int A = vertex;
                    int B = A + widh;
                    int C = B + 1;
                    triangles[triangleVertexCount] = A;
                    triangles[triangleVertexCount + 1] = B;
                    triangles[triangleVertexCount + 2] = C;
                    //Second triangle
                    B += 1;
                    C = A + 1;
                    triangles[triangleVertexCount + 3] = A;
                    triangles[triangleVertexCount + 4] = B;
                    triangles[triangleVertexCount + 5] = C;
                    triangleVertexCount += 6;
                }
            }

            int uvIndexCounter = 0;
            foreach (Vector3 vertex in vertices)
            {
                verticesFinal.Add(vertex.X);
                verticesFinal.Add(vertex.Y);
                verticesFinal.Add(vertex.Z);

                /*uvs[uvIndexCounter] = new Vector2(vertex.X, vertex.z);
                uvIndexCounter++;*/
            }

            mesh.verts = verticesFinal.ToArray();
            mesh.uvs = uvs.ToArray();
            mesh.indices = triangles;
            mesh.colors = colors.ToArray();

            return mesh;
        }

        float GetTile(int x, int z, float hDeafult, Block[,] array)
        {
            //Block block = Game.GetWorld.GetTileAt(x, z);

            //return block.height;

            return hDeafult;
        }
    }
}

public class VoxelMesh
{
    public float[] verts;
    public int[] indices;
    public float[] uvs;
    public float[] colors;

    public void ClearMesh()
    {
        verts = null;
        indices = null;
        uvs = null;
        colors = null;
    }
}

public struct TreeStruc
{
    public Vector3 position;
    public TreeType Treetype;

    public TreeStruc(Vector3 pos, TreeType treetype)
    {
        this.position = pos;
        this.Treetype = treetype;
    }
}