using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvllyEngine;
using ProjectEvlly.src.Engine.Render;
using ProjectEvlly.src.Utility;

namespace ProjectEvlly.src.World
{
    public class ChunkMeshRender : RenderEntityBase
    {
        public Mesh _Mesh;
        public Shader _shader;
        public Texture _texture;
        private int IBO, VAO, vbo, dbo, tbo, nbo;
        private int IndiceLength;

        public CullFaceMode _cullType;
        public bool Transparency = false;
        private bool isReady;

        public ChunkMeshRender(Transform transformParent, Mesh mesh, Shader shader, Texture texture)
        {
            isReady = false;
            _cullType = CullFaceMode.Front;

            transform = transformParent;

            _Mesh = new Mesh(mesh);

            if (_Mesh != null)
            {
                _shader = shader;
                _texture = texture;

                shader.SetInt("texture0", 0);

                /*if (_shader != null)
                {
                    _shader.Use();
                }*/

                IndiceLength = _Mesh._indices.Length;

                IBO = GL.GenBuffer();
                VAO = GL.GenVertexArray();

                vbo = GL.GenBuffer();
                dbo = GL.GenBuffer();
                tbo = GL.GenBuffer();
                nbo = GL.GenBuffer();

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _Mesh._indices.Length * sizeof(int), _Mesh._indices, BufferUsageHint.StreamDraw);

                GL.BindVertexArray(VAO);

                //Vertices(Vector3)
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _Mesh._vertices.Length * Vector3.SizeInBytes, _Mesh._vertices, BufferUsageHint.StreamDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);

                //Colors(Vectro4|Color)
                GL.BindBuffer(BufferTarget.ArrayBuffer, dbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _Mesh._Colors.Length * sizeof(float), _Mesh._Colors, BufferUsageHint.StreamDraw);
                GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(1);

                //Texture(Vector2)
                GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _Mesh._texCoords.Length * Vector2.SizeInBytes, _Mesh._texCoords, BufferUsageHint.StreamDraw);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(2);

                //Mesh Normals(Vector3)
                GL.BindBuffer(BufferTarget.ArrayBuffer, nbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _Mesh._Normals.Length * Vector3.SizeInBytes, _Mesh._Normals, BufferUsageHint.StreamDraw);
                GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(3);

                isReady = true;
            }
        }

        public override void TickRender(float time)
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

                _shader.Use();

                _shader.SetMatrix4("world", transform.GetTransformWorld);
                _shader.SetMatrix4("view", Camera.Main.viewMatrix);
                _shader.SetMatrix4("projection", Camera.Main._projection);

                //Set The Fog Values(this need to be in all mesh with use fog)
                _shader.SetFloat("FOG_Density", Fog.Density);
                _shader.SetFloat("FOG_Gradiante", Fog.Distance);
                _shader.SetVector4("FOG_Color", Fog.FogColor);

                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
                GL.DrawElements(Window.GetGLBeginMode, IndiceLength, DrawElementsType.UnsignedInt, 0);
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

        public void UpdateMeshRender(Mesh mesh)
        {
            if (isReady)
            {
                /*_mesh.Clear();
                _mesh = null;

                _shader = null;
                _texture = null;*/
                
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

                /*GL.DeleteBuffer(IBO);

                GL.DeleteBuffer(vbo);
                GL.DeleteBuffer(dbo);
                GL.DeleteBuffer(tbo);
                GL.DeleteBuffer(nbo);

                GL.DeleteVertexArray(VAO);*/
                Utilitys.CheckGLError("Destroy Chunk MeshRender");
                isReady = false;

                _Mesh.Clear();
                _Mesh = null;
            }

            _Mesh = new Mesh(mesh);

            if (_Mesh != null)
            {
                IndiceLength = _Mesh._indices.Length;

                /*IBO = GL.GenBuffer();
                VAO = GL.GenVertexArray();

                vbo = GL.GenBuffer();
                dbo = GL.GenBuffer();
                tbo = GL.GenBuffer();
                nbo = GL.GenBuffer();*/

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, _Mesh._indices.Length * sizeof(int), _Mesh._indices, BufferUsageHint.StreamDraw);

                GL.BindVertexArray(VAO);

                //Vertices(Vector3)
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _Mesh._vertices.Length * Vector3.SizeInBytes, _Mesh._vertices, BufferUsageHint.StreamDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);

                //Colors(Vectro4|Color)
                GL.BindBuffer(BufferTarget.ArrayBuffer, dbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _Mesh._Colors.Length * sizeof(float), _Mesh._Colors, BufferUsageHint.StreamDraw);
                GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(1);

                //Texture(Vector2)
                GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _Mesh._texCoords.Length * Vector2.SizeInBytes, _Mesh._texCoords, BufferUsageHint.StreamDraw);
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(2);

                //Mesh Normals(Vector3)
                GL.BindBuffer(BufferTarget.ArrayBuffer, nbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _Mesh._Normals.Length * Vector3.SizeInBytes, _Mesh._Normals, BufferUsageHint.StreamDraw);
                GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(3);

                isReady = true;
            }
        }

        public override void Dispose()
        {
            if (isReady)
            {
                isReady = false;

                _Mesh.Clear();
                _Mesh = null;

                /*_mesh.Clear();
                _mesh = null;

                _shader = null;
                _texture = null;*/

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
            base.Dispose();
        }
    }
}
