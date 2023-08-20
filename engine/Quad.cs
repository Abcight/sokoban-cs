using OpenTK.Graphics.OpenGL;

namespace Sokoban.Engine
{
	/// <summary>
	/// A class representing a 3 dimensional quad surface.
	/// Automatically generates the necessary OpenGL buffers and information required to render itself onto the screen.
	/// </summary>
	public class Quad
	{
		// internal opengl buffer index
		private int vao;

		public Quad()
		{
			vao = GL.GenVertexArray();

			GL.BindVertexArray(vao);

			GL.BindVertexArray(0);
			GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
			GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
		}

		/// <summary>
		/// Binds the internal OpenGL vertex array object.
		/// </summary>
		public void Use() => GL.BindVertexArray(vao);

		/// <summary>
		/// Executes a drawcall through OpenGL.
		/// </summary>
		public void Draw() => GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
	}
}