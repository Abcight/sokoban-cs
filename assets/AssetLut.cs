namespace Sokoban.assets
{
	/// <summary>
	/// A lookup table for various asset paths used in the project.
	/// </summary>
	public static class AssetLut
	{
		// Fonts
		public const string FONT_POPPINS_JSON =						"./assets/fonts/poppins/font.json";
		public const string FONT_POPPINS_TEXTURE =					"./assets/fonts/poppins/font.png";

		// Shaders
		public const string SHADER_COMPOSITE_VERTEX =				"./assets/shaders/composite.vert";
		public const string SHADER_COMPOSITE_FRAGMENT =				"./assets/shaders/composite.frag";
		public const string SHADER_COMPOSITE_LIGHTCUT_FRAGMENT =	"./assets/shaders/composite_lightcut.frag";
		public const string SHADER_QUAD_VERTEX =					"./assets/shaders/quad.vert";
		public const string SHADER_QUAD_FRAGMENT =					"./assets/shaders/quad.frag";
		public const string SHADER_QUAD_LIGHT_FRAGMENT =			"./assets/shaders/quad_light.frag";
		public const string SHADER_QUAD_UI =						"./assets/shaders/quad_ui.frag";

		// Levels
		public const string LEVEL_POOL_ADVENTURE =					"./assets/levels/adv/";
		public const string LEVEL_POOL_EASY =						"./assets/levels/easy/";
		public const string LEVEL_POOL_MEDIUM =						"./assets/levels/medium/";
		public const string LEVEL_POOL_HARD =						"./assets/levels/hard/";

		// Misc textures
		public const string TEXTURE_ATLAS =							"./assets/textures/atlas.png";
		public const string TEXTURE_ICON =							"./assets/textures/icon.ico";
	}
}
