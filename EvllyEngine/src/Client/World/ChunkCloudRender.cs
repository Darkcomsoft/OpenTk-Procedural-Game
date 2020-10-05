using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvllyEngine;
using ProjectEvlly.src.Engine.Render;
using ProjectEvlly.src.Utility;
using ProjectEvlly.src.Engine;

namespace ProjectEvlly.src.World
{
    public class ChunkCloudRender : RenderEntityBase
    {
        private Mesh mesh;
        public Shader C_shader;
        public Texture C_texture;

        private int C_IBO, C_VAO, C_vbo, C_tbo;

        public CullFaceMode _cullType;
        public bool Transparency = false;
        private bool isReady;

        public ChunkCloudRender(Transform transformParent)
        {
            isReady = false;
            _cullType = CullFaceMode.Front;

            transform = new Transform();
            transform.Position = new Vector3(transformParent.Position.X, 50, transformParent.Position.Z);
            transform.Rotation = new Quaternion(MathHelper.DegreesToRadians(90), 0, 0, 1);
            transform.Size = new Vector3(10,10,1);

            C_shader = AssetsManager.GetShader("Cloud");
            C_texture = AssetsManager.GetTexture("Cloud");

            CreteCloudMesh();

            if (mesh != null)
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
                GL.BufferData(BufferTarget.ElementArrayBuffer, mesh._indices.Length * sizeof(int), mesh._indices, BufferUsageHint.StaticDraw);

                GL.BindVertexArray(C_VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, C_vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, mesh._vertices.Length * Vector3.SizeInBytes, mesh._vertices, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);

                //Texture
                GL.BindBuffer(BufferTarget.ArrayBuffer, C_tbo);
                GL.BufferData(BufferTarget.ArrayBuffer, mesh._texCoords.Length * Vector2.SizeInBytes, mesh._texCoords, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(1);

                TickSystem.AddRenderItem(this);

                isReady = true;
            }
        }

        public override void TickRender(float time)
        {
            if (C_shader != null && Camera.Main != null && isReady)
            {
                GL.Enable(EnableCap.CullFace);
                GL.Enable(EnableCap.Blend);
                GL.CullFace(CullFaceMode.Front);

                GL.BindVertexArray(C_VAO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, C_IBO);

                GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

                if (C_texture != null)
                {
                    C_texture.Use();
                }

                C_shader.Use();

                C_shader.SetMatrix4("world", transform.GetTransformWorld);
                C_shader.SetFloat("time", (float)Time._DTime);

                C_shader.SetMatrix4("view", Camera.Main.viewMatrix);
                C_shader.SetMatrix4("projection", Camera.Main._projection);

                C_shader.SetFloat("FOG_Density", Environment.Density);
                C_shader.SetFloat("FOG_Gradiante", Environment.Distance);
                C_shader.SetColor("FOG_Color", Environment.FogColor);

                GL.DrawElements(Window.GetGLBeginMode, mesh._indices.Length, DrawElementsType.UnsignedInt, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.BindVertexArray(0);

                GL.Disable(EnableCap.Blend);
                GL.Disable(EnableCap.CullFace);
            }
            base.TickRender(time);
        }

        public override void Dispose()
        {
            if (isReady)
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

                GL.DeleteBuffer(C_IBO);

                GL.DeleteBuffer(C_vbo);
                GL.DeleteBuffer(C_tbo);

                GL.DeleteVertexArray(C_VAO);
            }
            base.Dispose();
        }

        private void CreteCloudMesh()
        {
            mesh = new Mesh();

            mesh._vertices = new Vector3[]
            {
                 //Position          Texture coordinates
                 new Vector3(0.5f,  0.5f, 0.0f), // top right
                 new Vector3(0.5f, -0.5f, 0.0f), // bottom right
                new Vector3(-0.5f, -0.5f, 0.0f), // bottom left
                new Vector3(-0.5f,  0.5f, 0.0f) // top left
            };

            mesh._indices = new int[]
            {
                0, 1, 3,   // first triangle
                1, 2, 3    // second triangle
            };

            mesh._texCoords = new Vector2[] {
               new Vector2( 1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f)
            };
        }
    }
}
