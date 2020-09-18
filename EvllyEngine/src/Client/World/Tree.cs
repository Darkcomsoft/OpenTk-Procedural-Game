using EvllyEngine;
using OpenTK;
using ProjectEvlly.src;
using ProjectEvlly.src.Engine.Render;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class Tree
    {
        public Transform transform;
        public int HP, MaxHP;

        private MeshRender _meshRender;
        private BoxCollider _boxCollider;

        public Tree(Vector3 position)
        {
            HP = 100;
            MaxHP = 100;

            transform = new Transform();

            System.Random rand = new System.Random();

            float a = (float)rand.NextDouble();
            float b = (float)rand.NextDouble();

            float ChunkSeed = position.X * a + position.Z * b + Time._DeltaTime;

            transform.Position = position;
            transform.Rotation = new Quaternion(MathHelper.DegreesToRadians((float)new System.Random((int)ChunkSeed).Next(0, 10)), MathHelper.DegreesToRadians((float)new System.Random((int)ChunkSeed).Next(0,90)), MathHelper.DegreesToRadians((float)new System.Random((int)ChunkSeed).Next(0, 5)));

            float size = ProjectEvlly.src.Utility.Random.Range(1.5f, 2f, (int)ChunkSeed);

            transform.Size = new Vector3(1, 1, 1);

            _meshRender = new MeshRender(transform, AssetsManager.GetMesh("oak"), AssetsManager.GetShader("Default"), AssetsManager.GetTexture("SpritesTreeHigt"));

            _meshRender.ViewBoxWitdh = 3;
            _meshRender.ViewBoxWitdh = 5;

            //_meshRender.Transparency = true;
            _meshRender._cullType = OpenTK.Graphics.OpenGL.CullFaceMode.FrontAndBack;

            _boxCollider = new BoxCollider(transform, new Vector3(0.7f, 5, 0.7f));
        }

        public void OnDestroy()
        {
            _boxCollider.OnDestroy();

            transform = null;

            _meshRender = null;
            _boxCollider = null;
        }
    }
}
