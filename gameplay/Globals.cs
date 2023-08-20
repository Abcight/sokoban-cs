namespace Sokoban.Gameplay
{
    /// <summary>
    /// A helper class for storing the global game state
    /// </summary>
    public static class Globals
    {
        /// <summary>
        /// Whether or not we are in the adventure mode
        /// </summary>
        public static bool ChallengeMode = false;
        
        /// <summary>
        /// Whether or not we are in the level editor section
        /// </summary>
        public static bool Editor = false;
        
        /// <summary>
        /// Current player score
        /// </summary>
        public static int Score = 0;
        
        /// <summary>
        /// Current savefile
        /// </summary>
        public static string CurrentSave = string.Empty;
    }
}