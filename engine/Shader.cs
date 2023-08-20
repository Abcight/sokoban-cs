using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace Sokoban.Engine
{
    /// <summary>
    /// A helper class for interacting with OpenGL shader programs
    /// </summary>
    public class Shader
    {
        // internal opengl shader id
        private readonly int id;

        // cached opengl attribute ids
        private Dictionary<string, int> attribMap = new();

        public Shader(string vertexSource, string fragmentSource)
        {
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);

            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);

            GL.CompileShader(vertexShader);

            var infoLogVert = GL.GetShaderInfoLog(vertexShader);
            if (infoLogVert != string.Empty)
                Console.WriteLine(infoLogVert);

            GL.CompileShader(fragmentShader);

            var infoLogFrag = GL.GetShaderInfoLog(fragmentShader);

            if (infoLogFrag != string.Empty)
                Console.WriteLine(infoLogFrag);

            id = GL.CreateProgram();

            GL.AttachShader(id, vertexShader);
            GL.AttachShader(id, fragmentShader);
            GL.LinkProgram(id);

            GL.DetachShader(id, vertexShader);
            GL.DetachShader(id, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        /// <summary>
        /// Binds the OpenGL shader program
        /// </summary>
        public void Use()
        {
            GL.UseProgram(id);
        }

        /// <summary>
        /// Returns the location of a uniform variable
        /// </summary>
        public int GetUniformLocation(string attribName)
        {
            if (attribMap.ContainsKey(attribName))
                return attribMap[attribName];
            
            attribMap.Add(attribName, GL.GetUniformLocation(id, attribName));
            return attribMap[attribName];
        }

        /// <summary>
        /// Sets the matrix value for a shader uniform
        /// </summary>
        public void SetUniform(string name, Matrix4 matrix)
        {
            var location = GetUniformLocation(name);
            GL.UniformMatrix4(location, true, ref matrix);
        }
        
        /// <summary>
        /// Sets the color value for a shader uniform
        /// </summary>
        public void SetUniform(string name, Color4 color)
        {
            var location = GetUniformLocation(name);
            GL.Uniform4(location, new Color4(color.R, color.G, color.B, color.A));
        }

        /// <summary>
        /// Sets the 3d vector value for a shader uniform
        /// </summary>
        public void SetUniform(string name, Vector3 vec)
        {
            var location = GetUniformLocation(name);
            GL.Uniform3(location, vec);
        }
        
        /// <summary>
        /// Sets the 2d vector value for a shader uniform
        /// </summary>
        public void SetUniform(string name, Vector2 vec)
        {
            var location = GetUniformLocation(name);
            GL.Uniform2(location, vec);
        }

        /// <summary>
        /// Sets the integer value for a shader uniform
        /// </summary>
        public void SetUniform(string name, int i)
        {
            var location = GetUniformLocation(name);
            GL.Uniform1(location, i);
        }
        
        /// <summary>
        /// Sets the floating point value for a shader uniform
        /// </summary>
        public void SetUniform(string name, float f)
        {
            var location = GetUniformLocation(name);
            GL.Uniform1(location, f);
        }
    }
}