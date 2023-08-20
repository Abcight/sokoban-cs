using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Sokoban.Engine
{
    public class Font
    {
        private Dictionary<char, CharInfo> info = new();
        
        public Texture Texture;
        
        /// <summary>
        /// Class representing an SDF font.
        /// </summary>
        /// <param name="path">Path to the directory containing an SDF image called font.png and it's related font.json file.</param>
        public Font(string jsonPath, string texturePath)
        {
            // Make sure the OpenGL context is initialized
            if (Game.Instance != null)
            {
                Texture = AssetCache.LoadTexture(texturePath);
                Texture.MakeLinear();
            }
            
            string cfg = File.ReadAllText(jsonPath);
            JObject fontConfig = JObject.Parse(cfg);

            foreach (var chInfo in fontConfig["characters"])
            {
                char ch = chInfo["char"].ToString()[0];
                int width = int.Parse(chInfo["width"].ToString());
                int height = int.Parse(chInfo["height"].ToString());
                int advance = int.Parse(chInfo["xadvance"].ToString());
                int originx = int.Parse(chInfo["xoffset"].ToString());
                int originy = int.Parse(chInfo["yoffset"].ToString());
                int x = int.Parse(chInfo["x"].ToString());
                int y = int.Parse(chInfo["y"].ToString());

                CharInfo info = new CharInfo()
                {
                    Advance = advance,
                    Height = height,
                    OriginX = originx,
                    OriginY = originy,
                    Width = width,
                    X = x,
                    Y = y
                };
                
                this.info.Add(ch, info);
            }
        }

        /// <summary>
        /// Helper function for reading internal font information
        /// </summary>
        public CharInfo GetInfo(char ch)
        {
            if (info.ContainsKey(ch))
                return info[ch];
            return null;
        }


		/// <summary>
		/// Helper class for internal font information
		/// </summary>
		public class CharInfo
		{
			public int Width;
			public int Height;
			public int Advance;
			public int OriginX;
			public int OriginY;
			public int X;
			public int Y;
		}
	}
}