using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class Chunk : ScriptBase
    {
        public Vector3 chunkPosition;
        public MeshRender _MeshRender;
        private double ChunkSeed;
        public Block[,] Blocks;

        public override void Start()
        {

            base.Start();
        }

        public override void OnDestroy()
        {
            Blocks = null;
            _MeshRender = null;
            base.OnDestroy();
        }

        public void StartUpChunk()
        {
            Blocks = new Block[World.ChunkSize, World.ChunkSize];

            chunkPosition = gameObject._transform.Position;

            System.Random rand = new System.Random();

            double a = rand.NextDouble();
            double b = rand.NextDouble();

            ChunkSeed = chunkPosition.X * a + chunkPosition.Z * b + 0;

            /*Thread PopVoxelThread = new Thread(new ThreadStart(ThreadPopulateVoxel));
            PopVoxelThread.Start();*/

            ThreadPopulateVoxel();

            //UpdateMeshChunk();
            //StartCoroutine(QueeObjects());
            //Game.World.LoadNewChunks(this);
        }

        #region ThreadRegion
        private void ThreadPopulateVoxel()
        {
            byte index = 0;
            for (int x = 0; x < World.ChunkSize; x++)
            {
                for (int z = 0; z < World.ChunkSize; z++)
                {
                    Blocks[x, z] = new Block(x + (int)chunkPosition.X, z + (int)chunkPosition.Z, new Vector3(chunkPosition), World.globalNoise.GetPerlin(x + (int)chunkPosition.X, z + (int)chunkPosition.Z) * 20);
                    Blocks[x, z].index = index;
                    index++;
                }
            }

            ThreadMakeMesh();
        }

        private void ThreadMakeMesh()
        {
            MeshData data = new MeshData(Blocks);

            Mesh mesh = new Mesh();

            mesh.Clear();

            mesh._vertices = data._vertices.ToArray();
            mesh._indices = data._triangles.ToArray();
            mesh._texCoords = data._UVs.ToArray();
            mesh._Colors = data._colors.ToArray();

            //mesh.RecalculateNormals();

            gameObject.AddMeshRender(new MeshRender(gameObject, mesh, World.instance.Shader));
            gameObject.AddMeshCollider();

            data._vertices.Clear();
            data._triangles.Clear();
            data._UVs.Clear();
            data._colors.Clear();

            data = null;
        }
        #endregion

        public void RefreshMeshData()
        {
            Thread MakeMeshThread = new Thread(new ThreadStart(ThreadMakeMesh));
            MakeMeshThread.Start();
        }

    }

    public struct Block
    {
        public float hight;

        public byte index;

        public int x;
        public int z;

        public TypeBlock Type;

        public Block(int _x, int _z, Vector3 chunkPosition, float _density)
        {
            index = 0;
            x = _x;
            z = _z;
            System.Random rand = new System.Random(0 + x * z);
            hight = _density;

            Type = TypeBlock.Grass;
        }
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

            for (int x = 0; x < World.ChunkSize; x++)
            {
                for (int z = 0; z < World.ChunkSize; z++)
                {
                    if (tile[x, z].Type != TypeBlock.Air)
                    {
                        if (tile[x, z].Type == TypeBlock.WaterFloor)
                        {
                            _HaveWater = true;
                        }

                        int xB = tile[x, z].x;
                        int zB = tile[x, z].z;

                        float Right = GetTile(xB + 1, zB, tile[x, z].hight, tile);
                        float FrenteRight = GetTile(xB, zB + 1, tile[x, z].hight, tile);
                        float FrenteLeft = GetTile(xB + 1, zB + 1, tile[x, z].hight, tile);

                        _vertices.Add(x);
                        _vertices.Add(World.globalNoise.GetPerlin(xB,zB) * 20);
                        _vertices.Add(z);
                        

                        _vertices.Add(x + 1);
                        _vertices.Add(World.globalNoise.GetPerlin(xB + 1, zB) * 20);
                        _vertices.Add(z);
                        

                        _vertices.Add(x);
                        _vertices.Add(World.globalNoise.GetPerlin(xB, zB + 1) * 20);
                        _vertices.Add(z + 1);
                        

                        _vertices.Add(x + 1);
                        _vertices.Add(World.globalNoise.GetPerlin(xB + 1, zB + 1) * 20);
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

                        _UVs.Add(0);
                        _UVs.Add(0.05f);

                        _UVs.Add(0);
                        _UVs.Add(0);

                        _UVs.Add(0.05f);
                        _UVs.Add(0.05f);

                        _UVs.Add(0.05f);
                        _UVs.Add(0);

                        //_UVs.AddRange(Game.AssetsManager.GetTileUVs(tile[x, z]));
                    }
                }
            }
        }

        /*public VoxelMesh MakeWaterMesh(Block[,] tile)
        {
            Vector3[] vertices;
            Vector2[] uvs;
            int[] triangles;
            List<Color> colors = new List<Color>();

            VoxelMesh mesh = new VoxelMesh();

            int widh = World.ChunkSize + 1;

            vertices = new Vector3[widh * widh];
            for (int y = 0; y < widh; y++)
            {
                for (int x = 0; x < widh; x++)
                {
                    vertices[x + y * widh] = new Vector3(x, 0.0f, y);

                    colors.Add(new Color(1, 1, 1, 0));
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

            uvs = new Vector2[widh * widh];
            int uvIndexCounter = 0;
            foreach (Vector3 vertex in vertices)
            {
                uvs[uvIndexCounter] = new Vector2(vertex.x, vertex.z);
                uvIndexCounter++;
            }

            mesh.verts = vertices;
            mesh.uvs = uvs;
            mesh.indices = triangles;
            mesh.colors = colors.ToArray();

            return mesh;
        }*/

        float GetTile(int x, int z, float hDeafult, Block[,] array)
        {
            Block block = World.instance.GetTileAt(x, z);

            if (block.Type != TypeBlock.Air)
            {
                return block.hight;
            }

            return hDeafult;
        }
    }
}

public enum TypeBlock : byte
{
    Air, RockGround, RockHole,
    Grass, Water, GoldStone, IronStone,
    Rock, DirtGrass, Sand,
    Bloco, Dirt, DirtRoad, IceWater, Snow, LightBlockON,
    BeachSand, WaterFloor, JungleGrass, WasteSand
}