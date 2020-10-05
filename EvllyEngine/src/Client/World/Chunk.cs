using EvllyEngine;
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

                    blocks[x, z] = new Block(x + (int)transform.Position.X, z + (int)transform.Position.Z, transform.Position, MidleWorld.globalNoise.GetPerlin(pos.X, pos.Y) * 50, this);
                }
            }

            //Vblocks = PopulateVoxel(transform.Position);

            ///ThreadMakeMesh();
            UpdateMeshArround();
        }

        #region MarchingCube
        public List<Vector3> Vertices = new List<Vector3>();
        public List<int> Indices = new List<int>();

        public void MakeMesh(Vector3 position, float[] cube)
        {
            // Get the configuration index of this cube.
            int configIndex = GetCubeConfiguration(cube);

            // If the configuration of this cube is 0 or 255 (completely inside the terrain or completely outside of it) we don't need to do anything.
            if (configIndex == 0 || configIndex == 255)
                return;

            // Loop through the triangles. There are never more than 5 triangles to a cube and only three vertices to a triangle.
            int edgeIndex = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int p = 0; p < 3; p++)
                {

                    // Get the current indice. We increment triangleIndex through each loop.
                    int indice = MarchingCubeData.TriangleTable[configIndex, edgeIndex];

                    // If the current edgeIndex is -1, there are no more indices and we can exit the function.
                    if (indice == -1)
                        return;

                    // Get the vertices for the start and end of this edge.
                    Vector3 vert1 = position + MarchingCubeData.CornerTable[MarchingCubeData.EdgeIndexes[indice, 0]];
                    Vector3 vert2 = position + MarchingCubeData.CornerTable[MarchingCubeData.EdgeIndexes[indice, 1]];

                    Vector3 vertPosition;

                    // Get the terrain values at either end of our current edge from the cube array created above.
                    float vert1Sample = cube[MarchingCubeData.EdgeIndexes[indice, 0]];
                    float vert2Sample = cube[MarchingCubeData.EdgeIndexes[indice, 1]];

                    // Calculate the difference between the terrain values.
                    float difference = vert2Sample - vert1Sample;

                    // If the difference is 0, then the terrain passes through the middle.
                    difference = (MarchingCubeData.terrainSurface - vert1Sample) / difference;

                    // Calculate the point along the edge that passes through.
                    vertPosition = vert1 + ((vert2 - vert1) * difference);


                    Indices.Add(VertForIndice(vertPosition));

                    edgeIndex++;

                }
            }
        }

        float Lerp(float a, float b, float blend)
        {
            Vector2 valueA = new Vector2(a, a);
            Vector2 valueB = new Vector2(b, b);

            return Vector2.Lerp(valueA, valueB, blend).X;
        }

        int GetCubeConfiguration(float[] cube)
        {

            // Starting with a configuration of zero, loop through each point in the cube and check if it is below the terrain surface.
            int configurationIndex = 0;
            for (int i = 0; i < 8; i++)
            {

                // If it is, use bit-magic to the set the corresponding bit to 1. So if only the 3rd point in the cube was below
                // the surface, the bit would look like 00100000, which represents the integer value 32.
                if (cube[i] < MarchingCubeData.terrainSurface)
                    configurationIndex |= 1 << i;

            }

            return configurationIndex;

        }

        /*float SampleTerrain(Vector3 point)
        {
            return Vblocks[(int)point.X, (int)point.Y, (int)point.Z].density;
        }*/

        int VertForIndice(Vector3 vert)
        {
            // Loop through all the vertices currently in the vertices list.
            for (int i = 0; i < Vertices.Count; i++)
            {
                // If we find a vert that matches ours, then simply return this index.
                if (Vertices[i] == vert)
                {
                    return i;
                }
            }

            // If we didn't find a match, add this vert to the list and return last index.
            Vertices.Add(vert);
            //uvs.Add(new Vector2(terrainMap[point.x, point.y, point.z].textureID, 0));
            return Vertices.Count - 1;

        }
        #endregion

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

                    _meshRender.ViewBoxWitdh = 10;
                    _meshRender.ViewBoxHeight = 10;

                    //Water
                    _waterMeshRender = new WaterMeshRender(transform, WaterMesh, AssetsManager.GetShader("Water"), AssetsManager.GetTexture("Water"), AssetsManager.GetTexture("Water2"));
                    _waterMeshRender.Transparency = true;

                    _waterMeshRender.ViewBoxWitdh = 10;
                    _waterMeshRender.ViewBoxHeight = 10;
                }
            }

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

        float heatValue;
        float MoistureValue;

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

            

            float XX = x;
            float ZZ = z;

            MidleWorld.biomeNoise.GradientPerturbFractal(ref XX, ref ZZ);

            heatValue = Math.Abs(MidleWorld.biomeNoise.GetCellular(XX, ZZ));
            MoistureValue = Math.Abs(MidleWorld.biomeNoise.GetCellular(XX - ZZ, ZZ - XX));

            if (heatValue <= GlobalData.ColdestValue)
            {
                HeatType = HeatType.Coldest;
            }
            else if (heatValue <= GlobalData.ColderValue)
            {
                HeatType = HeatType.Colder;
            }
            else if (heatValue <= GlobalData.ColdValue)
            {
                HeatType = HeatType.Cold;
            }
            else if (heatValue <= GlobalData.WarmValue)
            {
                HeatType = HeatType.Warm;
            }
            else if (heatValue <= GlobalData.WarmerValue)
            {
                HeatType = HeatType.Warmer;
            }
            else
            {
                HeatType = HeatType.Warmest;
            }
            ///
            if (MoistureValue <= GlobalData.DryerValue)
            {
                MoistureType = MoistureType.Dryer;
            }
            else if (MoistureValue <= GlobalData.DryValue)
            {
                MoistureType = MoistureType.Dry;
            }
            else if (MoistureValue <= GlobalData.WetValue)
            {
                MoistureType = MoistureType.Wet;
            }
            else if (MoistureValue <= GlobalData.WetterValue)
            {
                MoistureType = MoistureType.Wetter;
            }
            else if (MoistureValue <= GlobalData.WettestValue)
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
                        biomeData._treeType = TreeType.none;
                        break;
                    case BiomeType.Desert:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        biomeData._treeType = TreeType.none;
                        biomeData._typeBlock = TypeBlock.Sand;
                        break;
                    case BiomeType.TropicalRainforest:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        biomeData._treeType = TreeType.none;
                        break;
                    case BiomeType.Savanna:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        biomeData._treeType = TreeType.none;
                        break;
                    case BiomeType.Ice:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        biomeData._typeBlock = TypeBlock.Snow;
                        biomeData._treeType = TreeType.none;
                        break;
                    case BiomeType.Tundra:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        biomeData._treeType = TreeType.none;
                        biomeData._Height *= 4;
                        biomeData._typeBlock = TypeBlock.Dirt;
                        break;
                    case BiomeType.Woodland:
                        biomeData = OakForest.GetBiome(x, z, chunk);
                        biomeData._Height *= 2;
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
                height += biomeData._Height + height;
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
            return string.Format("Hight:{0} Type:{1} Chunk:{2} Biome:{3} X:{4} Z:{5} : heatValue:{6} MoistureValue:{7}", height, Type, Chunk, TileBiome, x, z, heatValue, MoistureValue);
        }

        #region BiomeTransition
        /*public static float GetHeight(int x, int z, float height, BiomeType biomeType)
        {
            float high = height;

            for (int cx = -3; cx < 4; ++cx) for (int cz = -3; cz < 4; ++cz)
                    high += getBiomeHeiughtThingy(x + cx, z + cz);
            high /= 25;

            return high;
        }

        private static float getBiomeHeiughtThingy(int x, int z)
        {
            return MakeTransition(x, z, GetVizinhos(x, z));
        }

        static float lerp(float point1, float point2, float alpha)
        {
            return point1 + alpha * (point2 - point1);
        }

        public static float MakeTransition(int x, int z, BiomeType tilebiome)
        {
            switch (tilebiome)
            {
                default:
                    return 0;
            }
        }*/
        #endregion
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

public struct VoxelBlock
{
    public float density;

    public VoxelBlock(float density)
    {
        this.density = density;
    }
}

public struct VoxelMeshData
{
    public int[] Indices;
    public Vector3[] Vertices;

    public VoxelMeshData(int[] indices, Vector3[] vertices)
    {
        Indices = indices;
        Vertices = vertices;
    }
}

public static class MarchingCubeData
{
    public static float terrainSurface = 0.0f;

    public static Vector3[] CornerTable = new Vector3[8] {

        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 1, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(1, 1, 1),
        new Vector3(0, 1, 1)

    };

    public static Vector3[,] EdgeTable = new Vector3[12, 2] {

        { new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 0.0f) },
        { new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 0.0f) },
        { new Vector3(0.0f, 1.0f, 0.0f), new Vector3(1.0f, 1.0f, 0.0f) },
        { new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 1.0f, 0.0f) },
        { new Vector3(0.0f, 0.0f, 1.0f), new Vector3(1.0f, 0.0f, 1.0f) },
        { new Vector3(1.0f, 0.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f) },
        { new Vector3(0.0f, 1.0f, 1.0f), new Vector3(1.0f, 1.0f, 1.0f) },
        { new Vector3(0.0f, 0.0f, 1.0f), new Vector3(0.0f, 1.0f, 1.0f) },
        { new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 1.0f) },
        { new Vector3(1.0f, 0.0f, 0.0f), new Vector3(1.0f, 0.0f, 1.0f) },
        { new Vector3(1.0f, 1.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f) },
        { new Vector3(0.0f, 1.0f, 0.0f), new Vector3(0.0f, 1.0f, 1.0f) }

    };

    public static int[,] EdgeIndexes = new int[12, 2] {

        {0, 1}, {1, 2}, {3, 2}, {0, 3}, {4, 5}, {5, 6}, {7, 6}, {4, 7}, {0, 4}, {1, 5}, {2, 6}, {3, 7}

    };

    public static int[,] TriangleTable = new int[,] {

        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
        {3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},
        {3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
        {3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
        {9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
        {9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
        {2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
        {8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
        {9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
        {4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
        {3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
        {1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
        {4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
        {4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
        {5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
        {2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
        {9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
        {0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
        {2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
        {10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
        {4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
        {5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
        {5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
        {9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
        {0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
        {1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
        {10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
        {8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
        {2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
        {7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
        {2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
        {11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
        {5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
        {11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
        {11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
        {1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
        {9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
        {5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
        {2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
        {5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
        {6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
        {3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
        {6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
        {5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
        {1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
        {10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
        {6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
        {8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
        {7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
        {3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
        {5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
        {0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
        {9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
        {8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
        {5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
        {0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
        {6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
        {10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},
        {10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
        {8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
        {1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
        {0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
        {10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
        {3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
        {6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
        {9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
        {8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
        {3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
        {6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
        {0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
        {10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
        {10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
        {2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
        {7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
        {7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
        {2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
        {1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
        {11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
        {8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
        {0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
        {7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
        {10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
        {2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
        {6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
        {7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
        {2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
        {1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
        {10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
        {10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
        {0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
        {7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
        {6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
        {8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},
        {9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
        {6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
        {4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
        {10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
        {8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
        {0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
        {1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},
        {8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
        {10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
        {4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
        {10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
        {5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
        {11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
        {9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
        {6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
        {7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
        {3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
        {7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
        {3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
        {6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
        {9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
        {1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
        {4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
        {7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
        {6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
        {3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
        {0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
        {6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
        {0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
        {11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
        {6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
        {5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
        {9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
        {1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
        {1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
        {10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
        {0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
        {5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
        {10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
        {11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
        {9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
        {7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
        {2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
        {8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
        {9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
        {9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
        {1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
        {9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
        {9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
        {5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
        {0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
        {10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
        {2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
        {0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
        {0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
        {9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
        {5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
        {3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
        {5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
        {8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
        {0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
        {9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
        {1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
        {3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
        {4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
        {9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
        {11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
        {11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
        {2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
        {9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
        {3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
        {1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
        {4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
        {4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
        {3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
        {0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
        {9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
        {1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}

    };
}