using EvllyEngine;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEvlly.src.Engine.Render;

namespace ProjectEvlly
{
    public class Sky : IDisposable
    {
        public Mesh _mesh;
        public Mesh _Cloudmesh;
        public Shader _shader;
        public Shader C_shader;
        public Texture _texture;
        public Texture C_texture;

        private Transform _transform;
        private Transform _transform2;

        private int IBO, VAO, vbo, tbo, C_IBO, C_VAO, C_vbo, C_tbo;

        public Transform transform;

        public Sky(Shader shader, Texture texture)
        {
            _mesh = AssetsManager.GetMesh("SkyCube");
            CreteCloudMesh();
            _shader = shader;
            _texture = texture;

            C_shader = AssetsManager.GetShader("Cloud");
            C_texture = AssetsManager.GetTexture("Cloud");

            _transform = new Transform();

            _transform.Position = new Vector3(0,50,0);
            _transform.Rotation = new Quaternion(MathHelper.DegreesToRadians(90),0,0,1);
            _transform.Size = new Vector3(550, 550, 550);

            _transform2 = new Transform();

            _transform2.Position = new Vector3(0, 50, 0);
            _transform2.Rotation = new Quaternion(MathHelper.DegreesToRadians(90), 0, 0, 1);
            _transform2.Size = new Vector3(550, 550, 550);

            if (_shader != null)
            {
                _shader.Use();
            }

            VAO = GL.GenVertexArray();
            IBO = GL.GenBuffer();
            vbo = GL.GenBuffer();
            tbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _mesh._indices.Length * sizeof(int), _mesh._indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _mesh._vertices.Length * Vector3.SizeInBytes, _mesh._vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            //Texture
            GL.BindBuffer(BufferTarget.ArrayBuffer, tbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _mesh._texCoords.Length * Vector2.SizeInBytes, _mesh._texCoords, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);

            GL_BindCloudMesh();
        }

        public void Tick()
        {
            if (Camera.Main != null)
            {
                _transform.Position = new Vector3(Camera.Main._transformParent.Position.X, 150, Camera.Main._transformParent.Position.Z);
                _transform2.Position = new Vector3(Camera.Main._transformParent.Position.X, 200, Camera.Main._transformParent.Position.Z);
            }
        }

        public void Draw(FrameEventArgs e)
        {
            if (_shader != null && Camera.Main != null)
            {
                /*GL.CullFace(CullFaceMode.Front);

                GL.Enable(EnableCap.CullFace);
                GL.Disable(EnableCap.DepthTest);

                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);

                if (_texture != null)
                {
                    _texture.Use();
                }

                _shader.Use();

                _shader.SetMatrix4("world", Matrix4.CreateScale(50, 50, 50) * Camera.Main._transformParent.PositionMatrix);
                _shader.SetMatrix4("view", Camera.Main.viewMatrix);
                _shader.SetMatrix4("projection", Camera.Main._projection);

                GL.DrawElements(Window.GetGLBeginMode, _mesh._indices.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.BindVertexArray(0);

                GL.Enable(EnableCap.DepthTest);
                GL.Disable(EnableCap.CullFace);*/

                DrawCloud();
            }
        }

        private void CreteCloudMesh()
        {
            _Cloudmesh = new Mesh();

            _Cloudmesh._vertices = new Vector3[]
            {
                 //Position          Texture coordinates
                 new Vector3(0.5f,  0.5f, 0.0f), // top right
                 new Vector3(0.5f, -0.5f, 0.0f), // bottom right
                new Vector3(-0.5f, -0.5f, 0.0f), // bottom left
                new Vector3(-0.5f,  0.5f, 0.0f) // top left
            };

            _Cloudmesh._indices = new int[]
            {
                0, 1, 3,   // first triangle
                1, 2, 3    // second triangle
            };

            _Cloudmesh._texCoords = new Vector2[] {
               new Vector2( 1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f)
            };
        }

        private void GL_BindCloudMesh()
        {
            if (C_shader != null)
            {
                C_shader.Use();
            }

            C_VAO = GL.GenVertexArray();
            C_IBO = GL.GenBuffer();
            C_vbo = GL.GenBuffer();
            C_tbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, C_IBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _Cloudmesh._indices.Length * sizeof(int), _Cloudmesh._indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(C_VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, C_vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _Cloudmesh._vertices.Length * Vector3.SizeInBytes, _Cloudmesh._vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            //Texture
            GL.BindBuffer(BufferTarget.ArrayBuffer, C_tbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _Cloudmesh._texCoords.Length * Vector2.SizeInBytes, _Cloudmesh._texCoords, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);
        }

        private void DrawCloud()
        {
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.CullFace(CullFaceMode.Front);

            GL.BindVertexArray(C_VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, C_IBO);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            for (int i = 0; i < 2; i++)
            {
                if (C_texture != null)
                {
                    C_texture.Use();
                }

                C_shader.Use();

                if (i == 0)
                {
                    C_shader.SetMatrix4("world", _transform.GetTransformWorld);
                    C_shader.SetFloat("time", Time._Tick / 60);
                }
                else
                {
                    C_shader.SetMatrix4("world", _transform2.GetTransformWorld);
                    C_shader.SetFloat("time", Time._Tick / 30);
                }

                C_shader.SetMatrix4("view", Camera.Main.viewMatrix);
                C_shader.SetMatrix4("projection", Camera.Main._projection);

                C_shader.SetFloat("FOG_Density", Fog.Density);
                C_shader.SetFloat("FOG_Gradiante", Fog.Distance);
                C_shader.SetVector4("FOG_Color", Fog.FogColor);

                GL.DrawElements(Window.GetGLBeginMode, _Cloudmesh._indices.Length, DrawElementsType.UnsignedInt, 0);
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.CullFace);
        }

        private void DeleteCloud()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(1);

            GL.DeleteBuffer(C_IBO);

            GL.DeleteBuffer(C_vbo);
            GL.DeleteBuffer(C_tbo);

            GL.DeleteVertexArray(C_VAO);
        }

        public void Dispose()
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DisableVertexAttribArray(1);

            GL.DeleteBuffer(IBO);

            GL.DeleteBuffer(vbo);
            GL.DeleteBuffer(tbo);

            GL.DeleteVertexArray(VAO);

            DeleteCloud();
        }
    }
}
