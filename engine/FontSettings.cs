using OpenTK.Mathematics;

namespace Sokoban.Engine
{
    /// <summary>
    /// Class used to hold uniform variables for the font shader when dispatching a new font draw job.
    /// </summary>
    public class FontSettings
    {
        private static FontSettings defaultSettings;

        public static FontSettings Default
        {
            get
            {
                if (defaultSettings == null)
                {
                    defaultSettings = new FontSettings
                    {
                        Color = Color4.White
                    };
                }

                return defaultSettings;
            }
        }
    
    
        public Color4 Color = Color4.White;
    }
}