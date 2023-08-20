using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sokoban.gameplay;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
    /// <summary>
    /// A grid used by the editor
    /// </summary>
    public class EditorGrid : GameGrid
    {
        // grid components
        private EditorUI ui = new EditorUI();
        private GridSelector selector = new GridSelector();
        
        private int width = 14;
        private List<List<GridSpace>> flags = new List<List<GridSpace>>();

        public EditorGrid(string snapshot)
        {
            LoadFromSnapshot(snapshot);
            
            // Generate a flag grid
            flags = new List<List<GridSpace>>();
            for (int i = 0; i < 14; i++)
            {
                flags.Add(new List<GridSpace>());
                for (int j = 0; j < 9; j++)
                {
                    if (i < Width)
                    {
                        if(map[i][j] is Player)
                            flags[i].Add(GridSpace.Player);
                        else if(map[i][j] is Box)
                            flags[i].Add(GridSpace.Box);
                        else if(map[i][j] is Boulder)
                            flags[i].Add(GridSpace.Boulder);
                        else if(IsPlate(i, j))
                            flags[i].Add(GridSpace.Plate);
                        else flags[i].Add(0);
                    }
                    else flags[i].Add(0);
                }
            }

            width = Width;
        }

        /// <summary>
        /// Serializes the grid's state into a snapshot
        /// </summary>
        public string GetSnapshot()
        {
            string snap = $"{width}:9:";
            for(int i = 0; i < width; i++)
				for (int j = 0; j < 9; j++)
					snap += (int)flags[i][j];
            
            return snap;
        }

        private void recalculate() => LoadFromSnapshot(GetSnapshot());

        private void setWidth(int w)
        {
            width = w;
            width = Math.Clamp(width, 1, 14);
            recalculate();
        }

        public override void Update()
        {
            if (Time.Pause)
                return;
            
            ManageSwitchInOut();
            
            ui.Update();

            if (Input.GetKeyDown(Keys.Escape))
                Time.Pause = true;

			updateSelector();
			updateLevelSize();
			updateLevelItems();
        }

		private void updateSelector()
		{
			if (Input.GetKeyDown(Keys.W) || Input.GetKeyDown(Keys.Up))
				selector.GridPosition.Y++;
			if (Input.GetKeyDown(Keys.S) || Input.GetKeyDown(Keys.Down))
				selector.GridPosition.Y--;
			if (Input.GetKeyDown(Keys.A) || Input.GetKeyDown(Keys.Left))
				selector.GridPosition.X--;
			if (Input.GetKeyDown(Keys.D) || Input.GetKeyDown(Keys.Right))
				selector.GridPosition.X++;

			selector.GridPosition.X = Math.Clamp(selector.GridPosition.X, 0, Width - 1);
			selector.GridPosition.Y = Math.Clamp(selector.GridPosition.Y, 0, Height - 1);

			selector.RenderPosition = Pivot + new Vector3(selector.GridPosition.X, selector.GridPosition.Y, 0) * 16;
			selector.Update();
		}

		private void updateLevelSize()
		{
			if (Input.GetKeyDown(Keys.Minus))
				setWidth(width - 1);
			if (Input.GetKeyDown(Keys.Equal))
				setWidth(width + 1);
		}

		private void updateLevelItems()
		{
			int x = selector.GridPosition.X;
			int y = selector.GridPosition.Y;
			GridSpace val = flags[x][y];

			if (Input.GetKeyDown(Keys.D1))
				flags[x][y] = GridSpace.Empty;

			if (Input.GetKeyDown(Keys.D2))
			{
				for (int i = 0; i < width; i++)
					for (int j = 0; j < 9; j++)
						if (flags[i][j] == GridSpace.Player)
							flags[i][j] = GridSpace.Empty;
				flags[x][y] = GridSpace.Player;
			}
			
			if (Input.GetKeyDown(Keys.D3))
				flags[x][y] = GridSpace.Box;

			if (Input.GetKeyDown(Keys.D4))
				flags[x][y] = GridSpace.Boulder;

			if (Input.GetKeyDown(Keys.D5))
				flags[x][y] = GridSpace.Plate;

			if (val != flags[x][y])
				recalculate();
		}

        public override void Render()
        {
            base.Render();
            ui.Render();
            selector.Render();
        }
    }
}