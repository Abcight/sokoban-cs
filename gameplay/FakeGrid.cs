using System;
using System.Collections.Generic;
using Sokoban.assets;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
    /// <summary>
    /// A fake grid used as a background for the menu
    /// </summary>
    public class FakeGrid : GameGrid
    {
        public FakeGrid() 
        {
            // These might look scary at first, but actually these are just
            // the moves the fake player makes in a menu background loop.
            
            // They aren't loaded from an external file because
			// it's not like these need to ever be edited.
            LoadFromSnapshot($"{AssetLut.LEVEL_POOL_MEDIUM}/18");
            actions = new List<Action>()
            {
                () => MovePlayer(1, 0),
                () => MovePlayer(1, 0),
                () => MovePlayer(1, 0),
                () => MovePlayer(1, 0),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, 1),
                () => MovePlayer(-1, 0),
                () => MovePlayer(-1, 0),
                () => MovePlayer(0, -1),
                () => MovePlayer(-1, 0),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
                () => MovePlayer(1, 0),
                () => MovePlayer(1, 0),
                () => MovePlayer(0, -1),
                () => MovePlayer(1, 0),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, 1),
                () => MovePlayer(1, 0),
                () => MovePlayer(1, 0),
                () => MovePlayer(0, 1),
                () => MovePlayer(1, 0),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, 1),
                () => MovePlayer(-1, 0),
                () => MovePlayer(-1, 0),
                () => MovePlayer(-1, 0),
                () => MovePlayer(-1, 0),
                () => MovePlayer(-1, 0),
                () => MovePlayer(-1, 0),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
                () => MovePlayer(1, 0),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
                () => MovePlayer(-1, 0),
                () => MovePlayer(-1, 0),
                () => MovePlayer(0, 1),
                () => MovePlayer(1, 0),
                () => MovePlayer(1, 0),
                () => MovePlayer(0, 1),
                () => MovePlayer(1, 0),
                () => MovePlayer(1, 0),
                () => MovePlayer(1, 0),
                () => MovePlayer(0, 1),
                () => MovePlayer(-1, 0),
                () => MovePlayer(0, 1),
                () => MovePlayer(-1, 0),
                () => MovePlayer(0, -1),
                () => MovePlayer(1, 0),
                () => MovePlayer(1, 0),
                () => MovePlayer(1, 0),
                () => MovePlayer(1, 0),
                () => MovePlayer(1, 0),
                () => MovePlayer(0, -1),
                () => MovePlayer(-1, 0),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, 1),
                () => MovePlayer(0, 1),
                () => MovePlayer(-1, 0),
                () => MovePlayer(-1, 0),
                () => MovePlayer(-1, 0),
                () => MovePlayer(-1, 0),
                () => MovePlayer(-1, 0),
                () => MovePlayer(-1, 0),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
                () => MovePlayer(-1, 0),
                () => MovePlayer(0, -1),
                () => MovePlayer(0, -1),
            };
        }

        private List<Action> actions = new();
        private double timer = 0;
        private Random random = new Random();
        private int idx = 0;
        
        public override void Update()
        {
            timer += Time.DeltaTime * random.NextDouble() * 12;
            if (timer >= 1.0)
            {
                timer = 0;
                if (idx < actions.Count)
                {
                    actions[idx].Invoke();
                    idx++;
                }
                else
                    idx = 0;
            }
        }
    }
}