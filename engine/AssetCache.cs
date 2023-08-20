using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Sokoban.Engine
{
	/// <summary>
	/// A utility class for caching data loaded at runtime.
	/// </summary>
	public static class AssetCache
	{
		// Cached data is stored in dictionaries
		private static readonly Dictionary<string, string> stringData = new();
		private static readonly Dictionary<string, Image<Rgba32>> imageData = new();
		private static readonly Dictionary<string, Texture> textureData = new();

		/// <summary>
		/// Loads a text file into a string and caches the result.
		/// </summary>
		public static string LoadString(string path)
		{
			if (stringData.ContainsKey(path))
				return stringData[path];

			var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			var reader = new StreamReader(stream);
			var data = reader.ReadToEnd();
			stringData.Add(path, data);

			return data;
		}

		/// <summary>
		/// Loads a raw rgb32 image file (png) into and caches the result.
		/// </summary>
		public static Image<Rgba32> LoadImage(string path)
		{
			if (imageData.ContainsKey(path))
				return imageData[path];

			var data = Image.Load<Rgba32>(path);
			imageData.Add(path, data);

			return data;
		}

		/// <summary>
		/// Generates an OpenGL texture from the provided file path and caches the result.
		/// </summary>
		public static Texture LoadTexture(string path)
		{
			if (textureData.ContainsKey(path))
				return textureData[path];

			var source = LoadImage(path);

			var tex = new Texture(source);

			textureData.Add(path, tex);

			return tex;
		}

	}
}