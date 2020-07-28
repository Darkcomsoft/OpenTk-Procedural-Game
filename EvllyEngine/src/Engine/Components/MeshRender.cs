using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class MeshRender
    {
        public Mesh _mesh;
        public Shader _shader;
        private int IBO;
        private int VAO;

        public CullFaceMode _cullType;
        public bool Transparency = false;

        public Transform transform;

        public MeshRender(Transform transformParent,Mesh mesh, Shader shader)
        {
            _cullType = CullFaceMode.Front;

            transform = transformParent;

            _mesh = mesh;
            _shader = shader;

            if (_shader != null)
            {
                _shader.Use();
            }

            VAO = GL.GenVertexArray();
            IBO = GL.GenBuffer();
            int vbo = GL.GenBuffer();
            int dbo = GL.GenBuffer();
            int tbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _mesh._indices.Length * sizeof(int), _mesh._indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _mesh._vertices.Length * sizeof(float), _mesh._vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            //Colors
            GL.BindBuffer(BufferTarget.ArrayBuffer, dbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _mesh._Colors.Length * sizeof(float), _mesh._Colors, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);

            //Texture
            GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _mesh._texCoords.Length * sizeof(float), _mesh._texCoords, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(2);
        }

        public void Draw(FrameEventArgs e)
        {
            if (Transparency)
            {
                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.One);
                GL.Enable(EnableCap.Blend);
            }

            GL.CullFace(_cullType);

            if (_cullType != CullFaceMode.FrontAndBack)
            {
                GL.Enable(EnableCap.CullFace);
            }

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);

            _shader.GetTexture.Use();
            _shader.Use();

            _shader.SetMatrix4("world", transform.GetTransformWorld);
            _shader.SetMatrix4("view", Camera.Main.viewMatrix);
            _shader.SetMatrix4("projection", Camera.Main._projection);

            GL.DrawElements(BeginMode.Triangles, _mesh._indices.Length, DrawElementsType.UnsignedInt, 0);
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

        public void OnDestroy()
        {
            //_shader.Delete();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

            GL.DeleteBuffer(IBO);
            GL.DeleteVertexArray(VAO);

            _mesh = null;
            _shader = null;
            transform = null;
        }
    }
}