using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ProjectEvlly;
using ProjectEvlly.src;
using ProjectEvlly.src.Engine;
using ProjectEvlly.src.Engine.Render;
using ProjectEvlly.src.World;
using ProjectEvlly.src.World.Biomes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class Chunk : ScriptBase
    {
        public Transform transform;
        private double ChunkSeed;

        private ChunkState ChunkState;

        private Block[,] blocks;

        public ChunkMeshRender _meshRender;
        private WaterMeshRender _waterMeshRender;
        private MeshCollider _meshCollider;

        private List<Tree> _trees = new List<Tree>();

        private bool FirstChunkPopulation = true;
        private bool isReady = false;
        private Mesh ChunkMesh;
        private Mesh WaterMesh;

        private Queue<Action> ActionUpdateMesh = new Queue<Action>();
        private object LockActionUpdateMEsh;

        public Chunk(Vector3 position)
        {
            transform = new Transform();
            System.Random rand = new System.Random();

            transform.Position = position;

            blocks = new Block[MidleWorld.ChunkSize, MidleWorld.ChunkSize];

            LockActionUpdateMEsh = new object();

            FirstChunkPopulation = true;

            double a = rand.NextDouble();
            double b = rand.NextDouble();

            ChunkSeed = transform.Position.X * a + transform.Position.Z * b + 0;

            TickSystem.AddTick(this);

            PopulateVoxel();
        }

        public override void Tick()
        {
            while (ActionUpdateMesh.Count > 0)
            {
                lock (LockActionUpdateMEsh)
                {
                    ActionUpdateMesh.Dequeue().Invoke();
                }
            }
            base.Tick();
        }

        public override void Dispose()
        {
            isReady = false;

            for (int i = 0; i < _trees.Count; i++)
            {
                _trees[i].OnDestroy();
            }

            _trees.Clear();

            TickSystem.RemoveTick(this);

            if (_meshRender != null)
            {
                _meshRender.Dispose();
            }

            if (_waterMeshRender !=  null)
            {
                _waterMeshRender.Dispose();
            }

            if (ChunkMesh != null)
            {
                ChunkMesh.Clear();
            }

            if (WaterMesh != null)
            {
                WaterMesh.Clear();
            }

            if (_meshCollider != null)
            {
                _meshCollider.Dispose();
            }
        }

        public void UpdateStatus(ChunkState state)
        {
            ChunkState = state;

            switch (ChunkState)
            {
                case ProjectEvlly.src.ChunkState.noload:
                    break;
                case ProjectEvlly.src.ChunkState.nogameLogic:
                    break;
                case ProjectEvlly.src.ChunkState.noEntity:
                    break;
                case ProjectEvlly.src.ChunkState.AllGameLogic:
                    break;
                default:
                    break;
            }
        }

        private void PopulateVoxel()
        {
            for (int x = 0; x < MidleWorld.ChunkSize; x++)
            {
                for (int z = 0; z < MidleWorld.ChunkSize; z++)
                {
                    Vector3 pos = new Vector3(x + (int)transform.Position.X, z + (int)transform.Position.Z, 0);

                    MidleWorld.globalNoise.GradientPerturbFractal(ref pos.X, ref pos.Y, ref pos.Z);

                    blocks[x, z] = new Block(x + (int)transform.Position.X, z + (int)transform.Position.Z, transform.Position, MidleWorld.globalNoise.GetPerlin(pos.X, pos.Y) + MidleWorld.globalNoise2.GetPerlin(pos.X, pos.Y) * 50, this);
                }
            }

            ThreadMakeMesh();
        }

        private void ThreadMakeMesh()
        {
            MeshData data = new MeshData(blocks);

            if (data._vertices.Count > 0)
            {
                if (ChunkMesh != null)
                {
                    ChunkMesh.Clear();
                }

                ChunkMesh = new Mesh(data._vertices.ToArray(), data._UVs.ToArray(), new Color4[] { }, data._triangles.ToArray());

                VoxelMesh mesh = data.MakeWatersMehs(blocks);

                WaterMesh = new Mesh(mesh.verts, mesh.uvs, mesh.colors, mesh.indices);

                lock (LockActionUpdateMEsh)
                {
                    ActionUpdateMesh.Enqueue(() => FirstMakeMesh());
                }
            }

            data.Dispose();
        }

        private void FirstMakeMesh()
        {
            _meshRender = new ChunkMeshRender(transform, ChunkMesh, AssetsManager.GetShader("TerrainDefault"), AssetsManager.GetTexture("TileAtlas"));
            //_meshCollider = new MeshCollider(transform, ChunkMesh._vertices, ChunkMesh._indices);

            _meshRender.ViewBoxWitdh = 10;
            _meshRender.ViewBoxWitdh = 10;

            //Water
            _waterMeshRender = new WaterMeshRender(transform, WaterMesh, AssetsManager.GetShader("Water"), AssetsManager.GetTexture("Water"), AssetsManager.GetTexture("Water2"));
            _waterMeshRender.Transparency = true;

            _waterMeshRender.ViewBoxWitdh = 10;
            _waterMeshRender.ViewBoxWitdh = 10;

            for (int i = 0; i < _trees.Count; i++)
            {
                _trees[i].OnDestroy();
            }

            _trees.Clear();

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

            if (FirstChunkPopulation == true)
            {
                FirstChunkPopulation = false;
                UpdateMeshArround();
            }
        }

        private void SMakeMesh()
        {
            if (ChunkMesh != null)
            {
                if (_meshCollider != null)
                {
                    _meshCollider.UpdateCollider(transform, ChunkMesh);
                }
                else
                {
                    _meshCollider = new MeshCollider(transform, ChunkMesh._vertices, ChunkMesh._indices);
                }

                if (_meshRender != null && _waterMeshRender != null)
                {
                    _meshRender.UpdateMeshRender(ChunkMesh);
                }
                else
                {
                    _meshRender = new ChunkMeshRender(transform, ChunkMesh, AssetsManager.GetShader("TerrainDefault"), AssetsManager.GetTexture("TileAtlas"));

                    //Water
                    _waterMeshRender = new WaterMeshRender(transform, WaterMesh, AssetsManager.GetShader("Water"), AssetsManager.GetTexture("Water"), AssetsManager.GetTexture("Water2"));
                    _waterMeshRender.Transparency = true;
                }
            }
        }

        /// <summary>
        /// this is for update chunk around of this chunk
        /// </summary>
        private void UpdateMeshArround()
        {
            Chunk c_F = Game.MidleWorld.GetChunkAt((int)transform.Position.X , (int)transform.Position.Z + 1);
            Chunk c_B = Game.MidleWorld.GetChunkAt((int)transform.Position.X, (int)transform.Position.Z - 1);
            Chunk c_L = Game.MidleWorld.GetChunkAt((int)transform.Position.X - 1, (int)transform.Position.Z);
            Chunk c_R = Game.MidleWorld.GetChunkAt((int)transform.Position.X + 1, (int)transform.Position.Z);

            if (c_F != null)
            {
                Game.GetWorld.UpDateChunk(c_F.transform.Position);
            }

            if (c_B != null)
            {
                Game.GetWorld.UpDateChunk(c_B.transform.Position);
            }

            if (c_L != null)
            {
                Game.GetWorld.UpDateChunk(c_L.transform.Position);
            }

            if (c_R != null)
            {
                Game.GetWorld.UpDateChunk(c_R.transform.Position);
            }
        }

        public void UpdateMesh()
        {
            MeshData data = new MeshData(blocks);

            if (data._vertices.Count > 0)
            {
                if (ChunkMesh != null)
                {
                    ChunkMesh.Clear();
                }

                ChunkMesh = new Mesh(data._vertices.ToArray(), data._UVs.ToArray(), new Color4[] { }, data._triangles.ToArray());

                VoxelMesh mesh = data.MakeWatersMehs(blocks);

                WaterMesh = new Mesh(mesh.verts, mesh.uvs, mesh.colors, mesh.indices);

                lock (LockActionUpdateMEsh)
                {
                    ActionUpdateMesh.Enqueue(() => SMakeMesh());
                }
            }

            data.Dispose();
        }

        public Block[,] GetBlocksMap { get { return blocks; } }
        public ChunkState GetStatus { get { return ChunkState; } }
        public double GetSeed { get { return ChunkSeed; } }
    }

    public struct Block
    {
        public float height;

        public byte index;

        public int x;
        public int z;

        public TypeBlock Type;
        public TreeType treeType;
        public BiomeType TileBiome;

        public Vector3 Chunk;

        public Block(int _x, int _z, Vector3 chunkPosition, float _Height, Chunk chunk)
        {
            index = 0;
            x = _x;
            z = _z;
            System.Random rand = new System.Random(0 + x * z);
            height = _Height;

            Chunk = chunkPosition;

            #region BiomeGen
            HeatType HeatType;
            MoistureType MoistureType;

            float heatValue;
            float MoistureValue;

            float XX = x;
            float ZZ = z;

            MidleWorld.biomeNoise.GradientPerturbFractal(ref XX, ref ZZ);

            heatValue = Math.Abs(MidleWorld.biomeNoise.GetCellular(XX, ZZ));
            MoistureValue = Math.Abs(MidleWorld.biomeNoise.GetCellular(XX, ZZ));

            if (heatValue < GlobalData.ColdestValue)
            {
                HeatType = HeatType.Coldest;
            }
            else if (heatValue < GlobalData.ColderValue)
            {
                HeatType = HeatType.Colder;
            }
            else if (heatValue < GlobalData.ColdValue)
            {
                HeatType = HeatType.Cold;
            }
            else if (heatValue < GlobalData.WarmValue)
            {
                HeatType = HeatType.Warm;
            }
            else if (heatValue < GlobalData.WarmerValue)
            {
                HeatType = HeatType.Warmer;
            }
            else
            {
                HeatType = HeatType.Warmest;
            }
            ///
            if (MoistureValue < GlobalData.DryerValue)
            {
                MoistureType = MoistureType.Dryer;
            }
            else if (MoistureValue < GlobalData.DryValue)
            {
                MoistureType = MoistureType.Dry;
            }
            else if (MoistureValue < GlobalData.WetValue)
            {
                MoistureType = MoistureType.Wet;
            }
            else if (MoistureValue < GlobalData.WetterValue)
            {
                MoistureType = MoistureType.Wetter;
            }
            else if (MoistureValue < GlobalData.WettestValue)
            {
                MoistureType = MoistureType.Wettest;
            }
            else
            {
                MoistureType = MoistureType.Wettest;
            }

            TileBiome = GlobalData.BiomeTable[(int)MoistureType, (int)HeatType];
            #endregion

            treeType = TreeType.none;
            Type = TypeBlock.Air;

            if (height <= 0 && height >= -2f)
            {
                TileBiome = BiomeType.Bench;
                Type = TypeBlock.Sand;
            }
            else if (height < -2f)
            {
                Type = TypeBlock.Sand;
            }
            else
            {
                BiomeData biomeData;

                switch (TileBiome)
                {
                    case BiomeType.Grassland:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        break;
                    case BiomeType.Desert:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        biomeData._treeType = TreeType.none;
                        break;
                    case BiomeType.TropicalRainforest:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        break;
                    case BiomeType.Savanna:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        biomeData._treeType = TreeType.none;
                        break;
                    case BiomeType.Ice:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        break;
                    case BiomeType.Tundra:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        biomeData._treeType = TreeType.none;
                        break;
                    case BiomeType.Woodland:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        break;
                    case BiomeType.Bench:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        biomeData._treeType = TreeType.none;
                        break;
                    default:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        biomeData._treeType = TreeType.none;
                        break;
                }

                Type = biomeData._typeBlock;
                treeType = biomeData._treeType;
                height *= biomeData._Height % _Height;
            }
        }

        public Block[] GetNeighboors(bool diagonals = false)
        {
            Block[] neighbors;

            if (diagonals)
            {
                neighbors = new Block[8];

                neighbors[0] = Game.GetWorld.GetTileAt(x, z + 1);//cima
                neighbors[1] = Game.GetWorld.GetTileAt(x + 1, z);//direita
                neighbors[2] = Game.GetWorld.GetTileAt(x, z - 1);//baixo
                neighbors[3] = Game.GetWorld.GetTileAt(x - 1, z);//esquerda

                neighbors[4] = Game.GetWorld.GetTileAt(x + 1, z - 1);//corn baixo direita
                neighbors[5] = Game.GetWorld.GetTileAt(x - 1, z + 1);//corn cima esquerda
                neighbors[6] = Game.GetWorld.GetTileAt(x + 1, z + 1);//corn cima direita
                neighbors[7] = Game.GetWorld.GetTileAt(x - 1, z - 1);//corn baixo esuqerda

            }
            else
            {
                neighbors = new Block[6];

                neighbors[0] = Game.GetWorld.GetTileAt(x, z - 1);//Atras
                neighbors[1] = Game.GetWorld.GetTileAt(x, z + 1);//Frente
                neighbors[2] = Game.GetWorld.GetTileAt(x - 1, z);//esquerda
                neighbors[3] = Game.GetWorld.GetTileAt(x + 1, z);//direita
            }

            return neighbors;
        }

        public string ToString()
        {
            return string.Format("Hight:{0} Type:{1} Chunk:{2} Biome:{3} X:{4} Z:{5}", height, Type, Chunk, TileBiome, x, z);
        }
    }

    public interface IMarching
    {

        float Surface { get; set; }

        void Generate(IList<float> voxels, int width, int height, int depth, IList<Vector3> verts, IList<int> indices);

    }

    public class MeshData
    {
        public List<Vector3> _vertices;
        public List<Vector2> _UVs;
        public List<int> _triangles;
        public List<Color4> _colors;

        public bool _HaveWater;

        public MeshData(Block[,] tile)
        {
            _vertices = new List<Vector3>();
            _UVs = new List<Vector2>();
            _triangles = new List<int>();
            _colors = new List<Color4>();

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

                        _vertices.Add(new Vector3(x, tile[x, z].height, z));
                        _vertices.Add(new Vector3(x + 1, Right, z));
                        _vertices.Add(new Vector3(x, FrenteRight, z + 1));
                        _vertices.Add(new Vector3(x + 1, FrenteLeft, z + 1));
                        

                        _triangles.Add(0 + verticesNum);
                        _triangles.Add(1 + verticesNum);
                        _triangles.Add(2 + verticesNum);

                        _triangles.Add(2 + verticesNum);
                        _triangles.Add(1 + verticesNum);
                        _triangles.Add(3 + verticesNum);
                        verticesNum += 4;

                        _colors.Add(new Color4(1, 1, 1, 1));
                        _colors.Add(new Color4(1, 1, 1, 1));
                        _colors.Add(new Color4(1, 1, 1, 1));
                        _colors.Add(new Color4(1, 1, 1, 1));

                        _UVs.AddRange(AssetsManager.GetTileUV(tile[x, z].Type.ToString()));
                    }
                }
            }
        }

        public VoxelMesh MakeWaterMesh(Block[,] tile)
        {
            Vector3[] vertices;
            List<Vector2> uvs = new List<Vector2>();
            int[] triangles;
            List<Color4> colors = new List<Color4>();

            VoxelMesh mesh = new VoxelMesh();

            int widh = MidleWorld.ChunkSize + 1;

            vertices = new Vector3[widh * widh];
            for (int y = 0; y < widh; y++)
            {
                for (int x = 0; x < widh; x++)
                {
                    vertices[x + y * widh] = new Vector3(x, 0.0f, y);

                    colors.Add(new Color4(1, 1, 1, 1));

                    uvs.AddRange(AssetsManager.GetTileUV("Water"));
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

            mesh.verts = vertices;
            mesh.uvs = uvs.ToArray();
            mesh.indices = triangles;
            mesh.colors = colors.ToArray();

            return mesh;
        }

        public VoxelMesh MakeWatersMehs(Block[,] tile)
        {
            _vertices = new List<Vector3>();
            _UVs = new List<Vector2>();
            _triangles = new List<int>();
            _colors = new List<Color4>();

            VoxelMesh mesh = new VoxelMesh();

            int verticesNum = 0;

            for (int x = 0; x < MidleWorld.ChunkSize; x++)
            {
                for (int z = 0; z < MidleWorld.ChunkSize; z++)
                {
                    if (tile[x, z].Type == TypeBlock.Sand)
                    {
                        _vertices.Add(new Vector3(x, -1f, z));
                        _vertices.Add(new Vector3(x + 1, -1, z));
                        _vertices.Add(new Vector3(x, -1, z + 1));
                        _vertices.Add(new Vector3(x + 1, -1, z + 1));


                        _triangles.Add(0 + verticesNum);
                        _triangles.Add(1 + verticesNum);
                        _triangles.Add(2 + verticesNum);

                        _triangles.Add(2 + verticesNum);
                        _triangles.Add(1 + verticesNum);
                        _triangles.Add(3 + verticesNum);
                        verticesNum += 4;

                        _colors.Add(new Color4(1, 1, 1, 1));
                        _colors.Add(new Color4(1, 1, 1, 1));
                        _colors.Add(new Color4(1, 1, 1, 1));
                        _colors.Add(new Color4(1, 1, 1, 1));

                        _UVs.AddRange(AssetsManager.GetTileUV("Water"));
                    }
                }
            }

            mesh.verts = _vertices.ToArray();
            mesh.uvs = _UVs.ToArray();
            mesh.indices = _triangles.ToArray();
            mesh.colors = _colors.ToArray();

            return mesh;
        }

        public void Dispose()
        {
            _vertices.Clear();
            _UVs.Clear();
            _triangles.Clear();
            _colors.Clear();

            _vertices = null;
            _UVs = null;
            _triangles = null;
            _colors = null;
        }

        float GetTile(int x, int z, float hDeafult, Block[,] array)
        {
            Block block = Game.GetWorld.GetTileAt(x, z);

            if (block.Type != TypeBlock.Air)
            {
                return block.height;
            }
            return hDeafult;
        }
    }
}

public class VoxelMesh
{
    public Vector3[] verts;
    public int[] indices;
    public Vector2[] uvs;
    public Color4[] colors;

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