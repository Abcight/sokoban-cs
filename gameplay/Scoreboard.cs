using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Sokoban.Gameplay
{
    /// <summary>
    /// A class for managing the scoreboard data in module 2.
    /// </summary>
    public class Scoreboard
    {
        /// <summary>
        /// Scores in descending order.
        /// </summary>
        public List<int> Scores
        {
            get
            {
				return scores.OrderBy(x => x).Reverse().ToList();
            }
            set
            {
                scores = value;
            }
        }

        /// <summary>
        /// Singleton instance of the class.
        /// </summary>
        public static Scoreboard Instance
        {
            get
            {
                if (instance == null)
                    instance = new Scoreboard();
                return instance;
            }
        }

        // internal data
        private List<int> scores;
        private static Scoreboard instance;

        public void AddScore(int score) => scores.Add(score);

        /// <summary>
        /// Creates a scoreboard and loads data into it if the necessary file is found.
        /// </summary>
        public Scoreboard()
        {
            if (File.Exists("./scoreboard"))
            {
                string data = File.ReadAllText("./scoreboard");
                Scores = JsonConvert.DeserializeObject<List<int>>(data);
            }
            else Scores = new();
        }

        /// <summary>
        /// Writes the scoreboard data into a local file on the drive.
        /// </summary>
        public static void Write()
        {
            string data = JsonConvert.SerializeObject(Instance.Scores);
            File.WriteAllText("./scoreboard", data);
        }
    }
}