using EvllyEngine;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEvlly.src.Engine.Render
{
    public class ShowCaseModel : RenderEntityBase
    {
        private MeshCollider meshCollider;

        public string _mesh;
        public string _shader;
        public string _texture;
        private int IBO, VAO, vbo, dbo, tbo, nbo;

        public CullFaceMode _cullType;
        public bool Transparency = false;

        public ShowCaseModel(string meshModel, string shader, string texture, bool haveCollision = true)
        {
            transform = new Transform();
            transform.Position = new Vector3(30,1,30);
            //transform.Rotation = new Quaternion(MathHelper.DegreesToRadians(-90), 0, 0, 1);

            _mesh = meshModel;
            _shader = shader;
            _texture = texture;

            SetUpGL();

            if (haveCollision)
            {
                meshCollider = new MeshCollider(transform, AssetsManager.GetMesh(_mesh)._vertices, AssetsManager.GetMesh(_mesh)._indices);
            }

            ViewBoxWitdh = 100;
            ViewBoxHeight = 100;
        }

        public override void Dispose()
        {
            TickSystem.RemoveRenderItem(this);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(1);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(2);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(3);

            GL.DeleteBuffer(IBO);

            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(dbo);
            GL.DeleteBuffer(tbo);
            GL.DeleteBuffer(nbo);

            GL.DeleteVertexArray(VAO);

            if (meshCollider != null)
            {
                meshCollider.Dispose();
            }
        }

        private void SetUpGL()
        {
            _cullType = CullFaceMode.Front;

            IBO = GL.GenBuffer();
            VAO = GL.GenVertexArray();

            vbo = GL.GenBuffer();
            dbo = GL.GenBuffer();
            tbo = GL.GenBuffer();
            nbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, AssetsManager.GetMesh(_mesh)._indices.Length * sizeof(int), AssetsManager.GetMesh(_mesh)._indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, AssetsManager.GetMesh(_mesh)._vertices.Length * Vector3.SizeInBytes, AssetsManager.GetMesh(_mesh)._vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            //Colors
            GL.BindBuffer(BufferTarget.ArrayBuffer, dbo);
            GL.BufferData(BufferTarget.ArrayBuffer, AssetsManager.GetMesh(_mesh)._Colors.Length * sizeof(float), AssetsManager.GetMesh(_mesh)._Colors, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);

            //Texture
            GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
            GL.BufferData(BufferTarget.ArrayBuffer, AssetsManager.GetMesh(_mesh)._texCoords.Length * Vector2.SizeInBytes, AssetsManager.GetMesh(_mesh)._texCoords, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(2);

            GL.BindBuffer(BufferTarget.ArrayBuffer, nbo);
            GL.BufferData(BufferTarget.ArrayBuffer, AssetsManager.GetMesh(_mesh)._Normals.Length * Vector3.SizeInBytes, AssetsManager.GetMesh(_mesh)._Normals, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(3);

            TickSystem.AddRenderItem(this);
        }

        public override void TickRender(float time)
        {
            if (_shader != null && Camera.Main != null)
            {
                if (Transparency)
                {
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
                    GL.Enable(EnableCap.Blend);
                }

                if (_cullType != CullFaceMode.FrontAndBack)
                {
                    GL.CullFace(_cullType);
                    GL.Enable(EnableCap.CullFace);
                }

                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);

                AssetsManager.UseTexture(_texture);

                AssetsManager.UseShader(_shader);

                AssetsManager.ShaderSet(_shader, "world", transform.GetTransformWorld);
                AssetsManager.ShaderSet(_shader, "view", Camera.Main.viewMatrix);
                AssetsManager.ShaderSet(_shader, "projection", Camera.Main._projection);

                //Set The Fog Values(this need to be in all mesh with use fog)
                AssetsManager.ShaderSet(_shader, "FOG_Density", Environment.Density);
                AssetsManager.ShaderSet(_shader, "FOG_Gradiante", Environment.Distance);
                AssetsManager.ShaderSet(_shader, "FOG_Color", Environment.FogColor);

                AssetsManager.ShaderSet(_shader, "AmbienceColor", Environment.AmbienceColor);

                GL.DrawElements(Window.GetGLBeginMode, AssetsManager.GetMesh(_mesh)._indices.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.BindVertexArray(0);

                if (_cullType != CullFaceMode.FrontAndBack)
                {
                    GL.Disable(EnableCap.CullFace);
                }

                if (Transparency)
                {
                    GL.Disable(EnableCap.Blend);
                }
            }
            base.TickRender(time);
        }
    }
}
