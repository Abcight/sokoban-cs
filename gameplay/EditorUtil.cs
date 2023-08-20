using System.IO;

namespace Sokoban.Gameplay
{
    public static class EditorUtil
    {
        public static string CurrentLevelFile;
        
        public static void AssertDirectories()
        {
            if (!Directory.Exists("./playerlevels"))
                Directory.CreateDirectory("./playerlevels");
            
            if (!Directory.Exists("./playerlevels/saves"))
                Directory.CreateDirectory("./playerlevels/saves");

            if (CurrentLevelFile == null)
            {
                var files = GetEditorLevels();
                CurrentLevelFile = $"./playerlevels/{files.Length}";
            }
        }

        public static void SaveLevel(string snapshot)
        {
            AssertDirectories();
            File.WriteAllText(CurrentLevelFile, snapshot);
        }

        public static string[] GetEditorLevels()
        {
            string[] playerLevels = Directory.GetFiles("./playerlevels");
            string[] pl = new string[playerLevels.Length];
            for (int i = 0; i < playerLevels.Length; i++)
            {
                FileInfo info = new FileInfo(playerLevels[i]);
                pl[i] = info.Name;
            }
            
            return pl;
        }
        
        public static string[] GetEditorSaves()
        {
            string[] playerLevels = Directory.GetFiles("./playerlevels/saves/");
            string[] pl = new string[playerLevels.Length];
            for (int i = 0; i < playerLevels.Length; i++)
            {
                FileInfo info = new FileInfo(playerLevels[i]);
                pl[i] = info.Name;
            }
            
            return pl;
        }
    }
}