using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sokoban.gameplay;
using Sokoban.Engine;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Sokoban.Gameplay
{
    /// <summary>
    /// A menu displayed when the game is paused
    /// </summary>
    public class PauseMenu : Actor
    {
        // internal menu entries
		private Dictionary<string, Action> menuEntries = new();
        
        // the id of current selected menu entry
        private int selectionIdx;

        // internally cached variables
        private FontSettings selectedSettings;
        private FontSettings defaultSettings;

        public PauseMenu(GameGrid grid)
        {
            if (grid is EditorGrid)
            {
                menuEntries.Add(Labels.PAUSE_TEST, () =>
                {
                    Game.Instance.World = LevelManager.Instance.GetPreview(((EditorGrid)grid).GetSnapshot());
                    Time.Pause = false;
                });
            }
            else
            {
                menuEntries.Add(Labels.PAUSE_RESET, () =>
                {
                    grid.Restart();
                    Time.Pause = false;
                });
            }

            if (grid is EditorPreviewGrid)
            {
                menuEntries.Add(Labels.PAUSE_EDITOR_RESET, () =>
                {
                    Game.Instance.World = LevelManager.Instance.GetEditor(((EditorPreviewGrid)grid).PreviewSnap);
                    Time.Pause = false;
                });
            }
            else
            {
                menuEntries.Add(Labels.PAUSE_MENU_RESET, () =>
                {
                    if (Globals.ChallengeMode)
                    {
                        SaveState state = new SaveState();
                        state.Snapshot();
                        state.Write();
                    }

                    if (Globals.Editor && !(grid is EditorGrid))
                    {
                        SaveState state = new SaveState("playerlevels");
                        state.Snapshot();
                        state.Write();
                    }
                    
                    if(grid is EditorGrid)
                        EditorUtil.SaveLevel(((EditorGrid)grid).GetSnapshot());
                    
                    Game.Instance.World = LevelManager.Instance.GetMenu();
                    Time.Pause = false;
                });
            }
            
            if (Globals.ChallengeMode)
            {
                menuEntries.Add(Labels.PAUSE_FINALIZE, () =>
                {
                    LevelManager.Instance.LevelPool.Clear();
                    Game.Instance.World = LevelManager.Instance.GetWin();
                    Time.Pause = false;
                });
            }
            
            menuEntries.Add(Labels.BACK, () => Time.Pause = false);
            
            Color4 selectedColor = new Color4(1, 1, 1, 1f);
            Color4 defaultColor = new Color4(1, 1, 1, 0.3f);
            
            selectedSettings = new FontSettings();
            selectedSettings.Color = selectedColor;

            defaultSettings = new FontSettings();
            defaultSettings.Color = defaultColor;
        }

        public override void Update()
        {
            if (!Time.Pause)
            {
                selectionIdx = 0;
                return;
            }
            
            if (Input.GetKeyDown(Keys.S) || Input.GetKeyDown(Keys.Down))
                selectionIdx++;
            if (Input.GetKeyDown(Keys.W) || Input.GetKeyDown(Keys.Up))
                selectionIdx--;
            
            if (selectionIdx < 0)
                selectionIdx = menuEntries.Count - 1;
            if (selectionIdx >= menuEntries.Count)
                selectionIdx = 0;
            
            if(Input.GetKeyDown(Keys.Enter) || Input.GetKeyDown(Keys.KeyPadEnter) || Input.GetKeyDown(Keys.Space))
                menuEntries.Values.ElementAt(selectionIdx).Invoke();
        }

        public override void Render()
        {
            if (!Time.Pause)
                return;
            
            Vector3 offset = Vector3.UnitY * 20;
            FontUtil.DrawText(Labels.PAUSE_TITLE, offset - Vector3.UnitX * FontUtil.MeasureWidth(Labels.PAUSE_TITLE) * 0.5f, 16);
            
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool selected = i == selectionIdx;
                offset -= Vector3.UnitY * 15;
                string text = menuEntries.Keys.ElementAt(i);
                FontUtil.DrawText(text, offset - Vector3.UnitX * FontUtil.MeasureWidth(text, 10) * 0.5f, 10, selected ? selectedSettings : defaultSettings);
            }
        }
    }
}