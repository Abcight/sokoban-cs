using OpenTK.Mathematics;
using Sokoban.assets;
using static Sokoban.Engine.Font;

namespace Sokoban.Engine
{
	/// <summary>
	/// A utility class for working with a default font
	/// </summary>
	public static class FontUtil
	{
		// Default font information
		private static Font font;
		private static Shader fontShader;

		/// <summary>
		/// Measures the width a of a given text without rendering it
		/// </summary>
		/// <param name="text">The text to be measured.</param>
		/// <param name="size">Font size of the text.</param>
		/// <returns>Predicted screenspace width of the text.</returns>
		public static float MeasureWidth(string text, int size = 16)
		{
			assertLoaded(false);

			float xoff = 0;
			for (int i = 0; i < text.Length; i++)
			{
				float scale = size / 32.0f;
				
				char c = text[i];
				
				if (c == ' ')
					xoff += 15 * scale;
				
				CharInfo charDefinition = font.GetInfo(c);
				if (charDefinition == null)
					continue;

				xoff += charDefinition.Advance * scale;
			}

			return xoff;
		}
		
		/// <summary>
		/// Dispatches a text draw job to the current render context
		/// </summary>
		/// <param name="text">Text to draw</param>
		/// <param name="pos">Screenspace position of the text</param>
		/// <param name="size">Size of the text</param>
		/// <param name="settings">Uniform variables for the shader</param>
		public static void DrawText(string text, Vector3 pos, int size = 16, FontSettings settings = null)
		{
			assertLoaded();

			float xoff = 0;
			for (int i = 0; i < text.Length; i++)
			{
				float scale = size / 32.0f;
				
				char c = text[i];

				if (c == ' ')
					xoff += 15 * scale;

				CharInfo charDefinition = font.GetInfo(c);
				if (charDefinition == null)
					continue;

				var offset = new Vector2(charDefinition.X, charDefinition.Y);
				offset.X /= font.Texture.Width;
				offset.Y /= font.Texture.Height;
				
				var sz = new Vector2(charDefinition.Width, charDefinition.Height);
				
				var s = new Vector2(sz.X, sz.Y);
				s.X /= font.Texture.Width;
				s.Y /= font.Texture.Height;
				
				sz *= scale;

				float x = xoff + charDefinition.Width * 0.5f * scale;

				float dip = charDefinition.OriginY - charDefinition.Height;
				float y = -charDefinition.Height * scale * 0.5f - charDefinition.OriginY * scale + size;

				Vector3 position = new Vector3(x, y, -i * 0.1f);
				
				RenderItem item = new RenderItem
				{
					Size = sz,
					Position = pos + position,
					Offset = offset,
					TileSize = s,
					Texture = font.Texture,
					Shader = fontShader,
					FontSettings = settings
				};

				xoff += charDefinition.Advance * scale;

				Renderer.Instance.QueueUI(item);
			}
		}

		private static void assertLoaded(bool gl = true)
		{
			if (font == null)
			{
				font = new Font(AssetLut.FONT_POPPINS_JSON, AssetLut.FONT_POPPINS_TEXTURE);
				
				if(gl)
					fontShader = new Shader(
						AssetCache.LoadString(AssetLut.SHADER_QUAD_VERTEX),
						AssetCache.LoadString(AssetLut.SHADER_QUAD_UI)
					);
			}
		}
	}
}