using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvllyEngine
{
    public class Shader
    {
        public int _shaderProgram;
        private bool IsValid = true;

        private Dictionary<string, int> _uniformLocations;

        public Shader(ShaderFile fileShader) 
        {
            _uniformLocations = new Dictionary<string, int>();
            CompileShader(fileShader);
        }

        public void Delete()
        {
            if (IsValid)
            {
                GL.DeleteProgram(_shaderProgram);
                IsValid = false;
            }
        }

        public void Use()
        {
            if (IsValid)
            {
                GL.UseProgram(_shaderProgram);
            }
        }

        private int CompileShader(ShaderFile shaderData)
        {
            int vert_shader = GL.CreateShader(ShaderType.VertexShader);
            int frag_shader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vert_shader, shaderData._vertshader);
            GL.ShaderSource(frag_shader, shaderData._fragshader);

            GL.CompileShader(vert_shader);
            GL.CompileShader(frag_shader);

            int program = GL.CreateProgram();
            GL.AttachShader(program, vert_shader);
            GL.AttachShader(program, frag_shader);
            GL.LinkProgram(program);

            int status;
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out status);
            if (status == 0)
                Debug.LogError(String.Format("Error linking program: {0}", GL.GetProgramInfoLog(program)));

            GL.DeleteShader(vert_shader);
            GL.DeleteShader(frag_shader);

            GL.GetProgram(program, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(program, i, out _, out _);
                var location = GL.GetUniformLocation(program, key);
                _uniformLocations.Add(key, location);
            }

            GL.ValidateProgram(program);
            var glerror = GL.GetError();
            if (glerror != ErrorCode.NoError)
            {
                Debug.LogError("SHADER: Failed to create program : " + glerror);
                GL.DeleteProgram(program);
            }
            _shaderProgram = program;
            return program;
        }

        /// <summary>
        /// Set a uniform int on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetInt(string name, int data)
        {
            Use();
            if (_uniformLocations.TryGetValue(name, out int value))
            {
                GL.Uniform1(value, data);
            }
        }

        /// <summary>
        /// Set a uniform float on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetFloat(string name, float data)
        {
            Use();
            if (_uniformLocations.TryGetValue(name, out int value))
            {
                GL.Uniform1(value, data);
            }
        }

        public void Setbool(string name, bool data)
        {
            Use();
            if (_uniformLocations.TryGetValue(name, out int value))
            {
                GL.Uniform1(value, data ? 1 : 0);
            }
        }

        public void SetVector4(string name, Vector4 data)
        {
            Use();
            if (_uniformLocations.TryGetValue(name, out int value))
            {
                GL.Uniform4(value, data);
            }
        }

        /// <summary>
        /// Set a uniform Matrix4 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public void SetMatrix4d(string name, Matrix4d data)
        {
            Use();
            if (_uniformLocations.TryGetValue(name, out int value))
            {
                GL.UniformMatrix4(value, true, ref data);
            }
        }

        /// <summary>
        /// Set a uniform Matrix4 on this shader
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        /// <remarks>
        ///   <para>
        ///   The matrix is transposed before being sent to the shader.
        ///   </para>
        /// </remarks>
        public void SetMatrix4(string name, Matrix4 data)
        {
            Use();
            if (_uniformLocations.TryGetValue(name, out int value))
            {
                GL.UniformMatrix4(value, true, ref data);
            }
        }

        /// <summary>
        /// Set a uniform Vector3 on this shader.
        /// </summary>
        /// <param name="name">The name of the uniform</param>
        /// <param name="data">The data to set</param>
        public void SetVector3(string name, Vector3 data)
        {
            Use();
            if (_uniformLocations.TryGetValue(name, out int value))
            {
                GL.Uniform3(value, data);
            }
        }
    }
}
