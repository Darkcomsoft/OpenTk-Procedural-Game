using EvllyEngine;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.World
{
    public class Tree
    {
        public Transform transform;
        public int HP, MaxHP;

        private MeshRender _meshRender;

        public Tree(Vector3 position)
        {
            HP = 100;
            MaxHP = 100;

            transform = new Transform();
            transform.Position = position;

            _meshRender = new MeshRender(transform, new Mesh(), new Shader(AssetsManager.instance.GetShader("Default")));
            _meshRender._cullType = OpenTK.Graphics.OpenGL.CullFaceMode.FrontAndBack;
        }

        public void Draw(FrameEventArgs e)
        {
            _meshRender.Draw(e);
        }

        public void OnDestroy()
        {
            _meshRender.OnDestroy();

            transform = null;
            _meshRender = null;
        }
    }
}
