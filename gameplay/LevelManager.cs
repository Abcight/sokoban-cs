using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
	/// <summary>
	/// A utility class for managing level creation / loading
	/// </summary>
	public class LevelManager
	{
		public static LevelManager Instance
		{
			get
			{
				if (instance == null)
					instance = new LevelManager();
				return instance;
			}
			set
			{
				instance = value;
			}
		}

		private static LevelManager instance;

		public List<string> LevelPool = new List<string>();
		public int CurrentLevelIndex = 0;

		/// <summary>
		/// Creates a level pool from level files present in the directory at the specified path
		/// </summary>
		public void CreatePool(string path)
		{
			CurrentLevelIndex = 0;
			string[] files = Directory.GetFiles(path);
			LevelPool = files.OrderBy(x => Guid.NewGuid()).ToList();
		}
		
		/// <summary>
		/// Creates a level pool from level files present in the directory at the specified path and then orders them by their name
		/// </summary>
		public void CreatePoolByName(string path)
		{
			CurrentLevelIndex = 0;
			string[] files = Directory.GetFiles(path);
			LevelPool = files.OrderBy(f => int.Parse(Path.GetFileName(f).Split('.')[0])).ToList();
		}

		/// <summary>
		/// Gets the next level from the level pool
		/// </summary>
		public World GetNext()
		{
			CurrentLevelIndex++;
			if(CurrentLevelIndex <= LevelPool.Count)
			{
				string path = LevelPool[CurrentLevelIndex-1];
				World world = GetCurrent();
				return world;
			}
			return GetWin();
		}
		
		/// <summary>
		/// Gets the current level from the level pool
		/// </summary>
		public World GetCurrent()
		{
			string path = LevelPool[CurrentLevelIndex-1];

			World world = new World();
			GameGrid grid = new GameGrid();
			grid.LoadFromSnapshot(path);
			
			world.Actors.Add(new RoomBackground());
			world.Actors.Add(new PauseMenu(grid));

			if(Globals.ChallengeMode)
				world.Actors.Add(new StepUI());
			world.Actors.Add(grid);

			return world;
		}

		/// <summary>
		/// Constructs the level editor world
		/// </summary>
		public World GetEditor(string snapshot = "")
		{
			World world = new World();

			if(snapshot == string.Empty)
			{
				string s = "14:9:";
				for (int i = 0; i < 14 * 9; i++)
					s += "0";
				snapshot = s;
			}
			
			EditorGrid grid = new EditorGrid(snapshot);
			
			world.Actors.Add(new RoomBackground());
			world.Actors.Add(new PauseMenu(grid));
			world.Actors.Add(new EditorUI());
			world.Actors.Add(grid);

			return world;
		}
		
		/// <summary>
		/// Loads a level from a given grid snapshot
		/// </summary>
		public World GetLevel(string snapshot)
		{
			World world = new World();
			GameGrid grid = new GameGrid();
			grid.LoadFromSnapshot(snapshot);
			
			world.Actors.Add(new RoomBackground());
			world.Actors.Add(new PauseMenu(grid));
			world.Actors.Add(grid);

			return world;
		}

		/// <summary>
		/// Loads a level preview from a given grid snapshot
		/// </summary>
		public World GetPreview(string snapshot)
		{
			World world = new World();
			EditorPreviewGrid grid = new EditorPreviewGrid(snapshot);
			
			grid.LoadFromSnapshot(snapshot);
			
			world.Actors.Add(new RoomBackground());
			world.Actors.Add(new PauseMenu(grid));
			world.Actors.Add(new EditorUI());
			world.Actors.Add(grid);

			return world;
		}
		
		/// <summary>
		/// Constructs the win screen world
		/// </summary>
		public World GetWin()
		{
			World world = new World();

			world.Actors.Add(new RoomBackground());
			world.Actors.Add(new WinInterface());
			
			return world;
		}

		/// <summary>
		/// Constructs the main menu world
		/// </summary>
		public World GetMenu()
		{
			World world = new World();

			world.Actors.Add(new RoomBackground());
			world.Actors.Add(new FakeGrid());
			world.Actors.Add(new MenuList());
			world.Actors.Add(new MenuWatermark());

			return world;
		}
	}
}