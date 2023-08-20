using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Sokoban.Gameplay
{
    /// <summary>
    /// A class used to hold savestate information at any given point in the game.
    /// </summary>
    public class SaveState
    {
        public string GridSnapshot;
        public int GridWidth;
        public int GridHeight;
        public int GridSteps;
        public int Score;
        public string LevelPath;
        public List<string> LevelPool;
        public int LevelIdx;

        [JsonIgnore] private string MainDir;

        [JsonIgnore]
        private string data;
        
        [JsonConstructor]
        public SaveState() { }

        /// <summary>
        /// Creates a new savestate and reads from a snapshot if it exists.
        /// </summary>
        /// <param name="idx">snapshot to read</param>
        public SaveState(int idx = 0)
        {
            if (!Directory.Exists("./Saves"))
                Directory.CreateDirectory("./Saves");

            string[] files = Directory.GetFiles("./Saves");
            if(files.Length > idx)
                data = File.ReadAllText(files[idx]);
        }
        
        /// <summary>
        /// Creates a new savestate and reads from a snapshot if it exists.
        /// </summary>
        /// <param name="idx">snapshot to read</param>
        public SaveState(string mainDir, int idx = 0)
        {
            MainDir = mainDir;
            
            if (!Directory.Exists($"./{MainDir}/saves"))
                Directory.CreateDirectory($"./{MainDir}/saves");

            string[] files = Directory.GetFiles($"./{MainDir}/saves");
            if(files.Length > idx)
                data = File.ReadAllText(files[idx]);
        }
        
        /// <summary>
        /// Takes a snapshot of the game information during the current frame.
        /// </summary>
        public void Snapshot()
        {
            GridSnapshot = GameGrid.Instance.Snapshot();
            GridWidth = GameGrid.Instance.Width;
            GridHeight = GameGrid.Instance.Height;
            GridSteps = GameGrid.Instance.Steps;
            Score = Globals.Score;
            LevelPath = EditorUtil.CurrentLevelFile;
            
            LevelPool = LevelManager.Instance.LevelPool;
            LevelIdx = LevelManager.Instance.CurrentLevelIndex;
            data = JsonConvert.SerializeObject(this, Formatting.Indented, 
                new JsonSerializerSettings 
                { 
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }

        /// <summary>
        /// Saves the snapshot into a file on the drive.
        /// </summary>
        public void Write()
        {
			if (!Directory.Exists($"./{MainDir}/saves"))
				Directory.CreateDirectory($"./{MainDir}/saves/");
			File.WriteAllText($"./{MainDir}/saves/{Globals.CurrentSave}", data);
        }

        /// <summary>
        /// Restores the game frame from the captured / loaded snapshot.
        /// </summary>
        public void Restore()
        {
            SaveState state = JsonConvert.DeserializeObject<SaveState>(data);
            LevelManager.Instance.LevelPool = state.LevelPool;
            LevelManager.Instance.CurrentLevelIndex = state.LevelIdx;
            Game.Instance.World = LevelManager.Instance.GetCurrent();
            GameGrid.Instance.LoadFromSnapshot(state.GridWidth, state.GridHeight, state.GridSnapshot);
            GameGrid.Instance.Steps = state.GridSteps;
            Globals.Score = state.Score;
        }
        
        public SaveState RestoreState()
        {
            SaveState state = JsonConvert.DeserializeObject<SaveState>(data);
            return state;
        }
    }
}