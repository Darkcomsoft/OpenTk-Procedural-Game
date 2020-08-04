using EvllyEngine;
using OpenTK;
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

            float ChunkSeed = position.X * a + position.Z * b + Time._Time;

            transform.Position = position;
            transform.Rotation = new Quaternion(MathHelper.DegreesToRadians((float)new System.Random((int)ChunkSeed).Next(0, 10)), MathHelper.DegreesToRadians((float)new System.Random((int)ChunkSeed).Next(0,90)), MathHelper.DegreesToRadians((float)new System.Random((int)ChunkSeed).Next(0, 5)));

            transform.Size = new Vector3(ProjectEvlly.src.Utility.Random.Range(1.5f, 2f, (int)ChunkSeed), ProjectEvlly.src.Utility.Random.Range(1.5f, 2f, (int)ChunkSeed), ProjectEvlly.src.Utility.Random.Range(1.5f, 2f, (int)ChunkSeed));

            _meshRender = new MeshRender(transform, AssetsManager.GetMesh("oak"), AssetsManager.GetShader("Default"), AssetsManager.GetTexture("SpritesTreeHigt"));
            //_meshRender.Transparency = true;
            _meshRender._cullType = OpenTK.Graphics.OpenGL.CullFaceMode.FrontAndBack;

            //_boxCollider = new BoxCollider(transform, new Vector3(1,1,1));

            Game.Instance.TransparentDrawUpdate += Draw;
        }

        public void Draw(FrameEventArgs e)
        {
            _meshRender.Draw(e);
        }

        public void OnDestroy()
        {
            Game.Instance.TransparentDrawUpdate -= Draw;

            //_boxCollider.OnDestroy();
            _meshRender.OnDestroy();
        }
    }
}
