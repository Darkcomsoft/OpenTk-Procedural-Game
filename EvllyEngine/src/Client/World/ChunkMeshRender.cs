using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvllyEngine;


namespace ProjectEvlly.src.World
{
    public class ChunkMeshRender
    {
        public Mesh _mesh;
        public Shader _shader;
        public Texture _texture;
        public Texture _texture2;
        public Texture _texture3;
        public Texture _texture4;
        private int IBO, VAO, vbo, dbo, tbo, nbo;

        public CullFaceMode _cullType;
        public bool Transparency = false;
        private bool isReady;
        public Transform transform;

        public ChunkMeshRender(Transform transformParent, Mesh mesh, Shader shader, Texture texture, Texture texture2, Texture texture3, Texture texture4)
        {
            isReady = false;
            _cullType = CullFaceMode.Front;

            transform = transformParent;

            _mesh = mesh;

            if (_mesh != null)
            {
                _shader = shader;
                _texture = texture;
                _texture2 = texture2;
                _texture3 = texture3;
                _texture4 = texture4;

                shader.SetInt("texture0", 0);
                shader.SetInt("texture1", 1);
                shader.SetInt("texture2", 2);
                shader.SetInt("texture3", 3);

                /*if (_shader != null)
                {
                    _shader.Use();
                }*/

                IBO = GL.GenBuffer();
                VAO = GL.GenVertexArray();

                vbo = GL.GenBuffer();
                dbo = GL.GenBuffer();
                tbo = GL.GenBuffer();
                nbo = GL.GenBuffer();

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _mesh._indices.Length * sizeof(int), _mesh._indices, BufferUsageHint.StaticDraw);

                GL.BindVertexArray(VAO);

                //Vertices(Vector3)
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _mesh._vertices.Length * sizeof(float), _mesh._vertices, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);

                //Colors(Vectro4|Color)
                GL.BindBuffer(BufferTarget.ArrayBuffer, dbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _mesh._Colors.Length * sizeof(float), _mesh._Colors, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(1);

                //Texture(Vector2)
                GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _mesh._texCoords.Length * sizeof(float), _mesh._texCoords, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(2);

                //Mesh Normals(Vector3)
                GL.BindBuffer(BufferTarget.ArrayBuffer, nbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _mesh._Normals.Length * sizeof(float), _mesh._Normals, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(3);

                isReady = true;
            }
        }

        public void Draw()
        {
            if (_shader != null && Camera.Main != null && isReady)
            {
                if (Transparency)
                {
                    GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.ConstantAlpha);
                    GL.Enable(EnableCap.Blend);
                }

                if (_cullType != CullFaceMode.FrontAndBack)
                {
                    GL.CullFace(_cullType);
                    GL.Enable(EnableCap.CullFace);
                }

                if (_texture != null)
                {
                    _texture.Use(TextureUnit.Texture0);
                }

                if (_texture2 != null)
                {
                    _texture2.Use(TextureUnit.Texture1);
                }

                if (_texture3 != null)
                {
                    _texture3.Use(TextureUnit.Texture2);
                }

                if (_texture4 != null)
                {
                    _texture4.Use(TextureUnit.Texture3);
                }

                _shader.Use();

                _shader.SetMatrix4("world", transform.GetTransformWorld);
                _shader.SetMatrix4("view", Camera.Main.viewMatrix);
                _shader.SetMatrix4("projection", Camera.Main._projection);

                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
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
        }

        public void OnDestroy()
        {
            if (isReady)
            {
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
            }
        }
    }
}
