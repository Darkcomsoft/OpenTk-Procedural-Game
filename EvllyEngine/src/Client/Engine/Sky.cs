using EvllyEngine;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEvlly.src.Engine.Render;
using ProjectEvlly.src.game;

namespace ProjectEvlly
{
    public class Sky : System.IDisposable
    {
        public Mesh _mesh;
        public Shader _shader;

        private Transform _transform;

        private int IBO, VAO, vbo, tbo;

        public Transform transform;

        public Sky(Shader shader)
        {
            _mesh = AssetsManager.GetMesh("SkySphere");

            _shader = shader;

            _transform = new Transform();

            _transform.Position = new Vector3(0,35,0);
            _transform.Rotation = new Quaternion(MathHelper.DegreesToRadians(90),0,0,1);
            _transform.Size = new Vector3(10, 10, 10);

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
        }

        public void Tick()
        {
            if (Camera.Main != null)
            {
                _transform.Position = new Vector3(0, 5, 0);
            }
        }

        public void Draw()
        {
            if (_shader != null && Camera.Main != null)
            {
                GL.CullFace(CullFaceMode.Front);

                GL.Enable(EnableCap.CullFace);
                GL.Enable(EnableCap.TextureCubeMap);
                GL.Disable(EnableCap.DepthTest);

                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);

                _shader.Use();

                _shader.SetMatrix4("world", Matrix4.CreateScale(50, 50, 50) * Camera.Main._transformParent.PositionMatrix);
                _shader.SetMatrix4("view", Camera.Main.viewMatrix);
                _shader.SetMatrix4("projection", Camera.Main._projection);

                _shader.SetColor("SKY_Color", Environment.SkyColor);
                _shader.SetColor("SKY_HoriColor", Environment.SkyHorizonColor);

                GL.DrawElements(BeginMode.Triangles, _mesh._indices.Length, DrawElementsType.UnsignedInt, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.BindVertexArray(0);

                GL.Enable(EnableCap.DepthTest);
                GL.Disable(EnableCap.TextureCubeMap);
                GL.Disable(EnableCap.CullFace);
            }
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
        }
    }
}
