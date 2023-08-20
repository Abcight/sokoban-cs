using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Sokoban.Engine
{
	/// <summary>
	/// A helper class for interacting with OpenGL textures
	/// </summary>
	public class Texture
	{
		// internal opengl texture id
		private int id;
		
		/// <summary>
		/// The width of the texture.
		/// </summary>
		public int Width;
		
		/// <summary>
		/// The height of the texture.
		/// </summary>
		public int Height;

		/// <summary>
		/// Initializes the OpenGL texture using an rgba32 image
		/// </summary>
		public Texture(Image<Rgba32> image)
		{
			var img = image;

			var pixels = new List<byte>(4 * img.Width * img.Height);

			for (var y = 0; y < img.Height; y++)
			{
				var row = img.GetPixelRowSpan(y);

				for (var x = 0; x < img.Width; x++)
				{
					pixels.Add(row[x].R);
					pixels.Add(row[x].G);
					pixels.Add(row[x].B);
					pixels.Add(row[x].A);
				}
			}

			Width = image.Width;
			Height = image.Height;

			GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

			id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
				(int) TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
				(int) TextureMagFilter.Nearest);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, img.Width, img.Height, 0,
				PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
				(int) TextureWrapMode.ClampToEdge);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
				(int) TextureWrapMode.ClampToEdge);

			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}

		/// <summary>
		/// Enable linear filtering for the texture
		/// </summary>
		public void MakeLinear()
		{
			GL.BindTexture(TextureTarget.Texture2D, id);

			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
				(int) TextureMinFilter.LinearMipmapLinear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
				(int) TextureMagFilter.Linear);

			GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
		}

		/// <summary>
		/// Binds the OpenGL texture to the first texture unit
		/// </summary>
		public void Use()
		{
			Use(TextureUnit.Texture0);
		}


		/// <summary>
		/// Binds the OpenGL texture to a given texture unit
		/// </summary>
		public void Use(TextureUnit unit)
		{
			GL.ActiveTexture(unit);
			GL.BindTexture(TextureTarget.Texture2D, id);
		}
	}
}