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
using ProjectEvlly.src.Engine;

namespace ProjectEvlly.src.World
{
    public class WaterMeshRender : RenderEntityBase
    {
        public Mesh _mesh;
        public Shader _shader;
        public Texture _WaterOnetexture;
        public Texture _WaterTwotexture;
        private int IBO, VAO, vbo, dbo, tbo, nbo;

        public CullFaceMode _cullType;
        public bool Transparency = false;
        private bool isReady;

        private int TextureID = 0;

        private int animTime = 0;

        public WaterMeshRender(Transform transformParent, Mesh mesh, Shader shader, Texture texture, Texture texture2)
        {
            isReady = false;
            _cullType = CullFaceMode.FrontAndBack;

            transform = transformParent;

            _mesh = mesh;

            if (_mesh != null)
            {
                _shader = shader;
                _WaterOnetexture = texture;
                _WaterTwotexture = texture2;

                shader.SetInt("texture0", 0);

                /*if (_shader != null)
                {
                    _shader.Use();
                }*/

                System.Random random = new Random(transform.GetHashCode());

                animTime = random.Next();

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
                GL.BufferData(BufferTarget.ArrayBuffer, _mesh._vertices.Length * Vector3.SizeInBytes, _mesh._vertices, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(0);

                //Colors(Vectro4|Color)
                GL.BindBuffer(BufferTarget.ArrayBuffer, dbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _mesh._Colors.Length * sizeof(float), _mesh._Colors, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(1);

                //Mesh Normals(Vector3)
                GL.BindBuffer(BufferTarget.ArrayBuffer, nbo);
                GL.BufferData(BufferTarget.ArrayBuffer, _mesh._Normals.Length * Vector3.SizeInBytes, _mesh._Normals, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(2);

                isReady = true;

                TickSystem.AddRenderItemT(this);
            }
        }

        public override void TickRender(float time)
        {
            if (_shader != null && Camera.Main != null && isReady)
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

                if (animTime % 20 == 15)
                {
                    if (TextureID == 0)
                    {
                        TextureID = 1;
                    }
                    else
                    {
                        TextureID = 0;
                    }
                }

                if (TextureID == 0)
                {
                    if (_WaterOnetexture != null)
                    {
                        _WaterOnetexture.Use(TextureUnit.Texture0);
                    }
                }
                else
                {
                    if (_WaterTwotexture != null)
                    {
                        _WaterTwotexture.Use(TextureUnit.Texture0);
                    }
                }

                _shader.Use();

                _shader.SetMatrix4("world", transform.GetTransformWorld);
                _shader.SetMatrix4("view", Camera.Main.viewMatrix);
                _shader.SetMatrix4("projection", Camera.Main._projection);
                _shader.SetFloat("time", Time._Tick);

                //Set The Fog Values(this need to be in all mesh with use fog)
                _shader.SetFloat("FOG_Density", Fog.Density);
                _shader.SetFloat("FOG_Gradiante", Fog.Distance);
                _shader.SetVector4("FOG_Color", Fog.FogColor);

                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IBO);
                GL.DrawElements(Window.GetGLBeginMode, _mesh._indices.Length, DrawElementsType.UnsignedInt, 0);
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

                animTime++;
                
                if (animTime >= 10000)
                {
                    animTime = 0;
                }
            }
            base.TickRender(time);
        }

        public override void Dispose()
        {
            if (isReady)
            {
                /* _mesh.Clear();
                 _mesh = null;

                 _shader = null;

                 _WaterOnetexture = null;
                 _WaterTwotexture = null;*/

                TickSystem.RemoveRenderItemT(this);

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
