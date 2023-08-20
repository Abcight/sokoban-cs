using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sokoban.assets;
using Sokoban.gameplay;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
	/// <summary>
	/// A class for the main menu item display.
	/// Implemented as a finite state machine.
	/// </summary>
	public class MenuList : Actor
	{
		private Dictionary<string, Action> menuEntries = new();
		private List<Vector3> offsets = new();

		private List<string> oldEntries = new();

		private int selectionIdx;
		private double oldEntriesTime = 0;

		private double offsetTime;

		private Action switchAction;
		private SwitchTimer timer;

		private float alpha = 0;

		private int verticalOffsetIdx = 0;
		private float verticalOffset = 0;

		public MenuList() => setStateDefault();

		public override void Update()
		{
			Globals.Score = 0;
			
			if (switchAction != null)
				return;

			alpha += (float)Time.DeltaTime * 5;
			alpha = Math.Min(alpha, 1.0f);
			
			offsetTime += Time.DeltaTime;
			offsetTime = Math.Min(offsetTime, 1.0);
			
			oldEntriesTime += Time.DeltaTime * 10;
			oldEntriesTime = Math.Min(oldEntriesTime, 1.0);
			
			if (Input.GetKeyDown(Keys.S) || Input.GetKeyDown(Keys.Down))
			{
				selectionIdx++;
				offsetTime = 0;
			}

			if (Input.GetKeyDown(Keys.W) || Input.GetKeyDown(Keys.Up))
			{
				selectionIdx--;
				offsetTime = 0;
			}

			if (selectionIdx > 3)
				verticalOffsetIdx = selectionIdx - 3;
			else verticalOffsetIdx = 0;

			if (selectionIdx < 0)
				selectionIdx = menuEntries.Count - 1;
			if (selectionIdx >= menuEntries.Count)
				selectionIdx = 0;

			if (Input.GetKeyDown(Keys.Enter) || Input.GetKeyDown(Keys.Space) || Input.GetKeyDown(Keys.KeyPadEnter))
			{
				string key = menuEntries.Keys.ElementAt(selectionIdx);
				menuEntries[key].Invoke();
			}
		}
	
		public override void Render()
		{
			// Switch the scenes if an action has been selected
			if (switchAction != null)
			{
				if(timer == null)
					timer = new SwitchTimer(switchAction);
				alpha -= (float) Time.DeltaTime * 10;
				timer.Update();
			}
			
			// Generate data to be sent as uniforms to the shaders
			Color4 selectedColor = new Color4(1, 1, 1, alpha);
			Color4 defaultColor = new Color4(1, 1, 1, 0.3f * alpha);
			
			FontSettings selectedSettings = new FontSettings();
			selectedSettings.Color = selectedColor;

			FontSettings defaultSettings = new FontSettings();
			defaultSettings.Color = defaultColor;
			
			Vector3 upLeftCorner = new Vector3(-140.0f, 55.0f, 0);
			FontUtil.DrawText(Labels.TITLE, upLeftCorner, 16, selectedSettings);

			FontSettings fadeSettings = new FontSettings();
			fadeSettings.Color.A = MathHelper.Clamp(1.0f - (float)oldEntriesTime, 0, 1) * alpha;

			// Vertical offset
			verticalOffset = (float)MathHelper.Lerp(verticalOffset, 15 * verticalOffsetIdx, 10 * Time.DeltaTime);
			
			// Old menu fade
			Vector3 fadeOffset = Vector3.Lerp(Vector3.Zero, -Vector3.UnitX * 10, (float) oldEntriesTime);
			Vector3 upLeftCornerFade = upLeftCorner;
			upLeftCornerFade -= Vector3.UnitY * 15;
			upLeftCornerFade += Vector3.UnitY * verticalOffset;
			foreach (var entry in oldEntries)
			{
				upLeftCornerFade -= Vector3.UnitY * 15;
				FontUtil.DrawText(entry, upLeftCornerFade + fadeOffset - Vector3.UnitZ, 10, fadeSettings);
			}

			// Queue the menu entries
			upLeftCorner -= Vector3.UnitY * 15;
			upLeftCorner += Vector3.UnitY * verticalOffset;
			upLeftCorner -= Vector3.UnitZ * 0.5f;
			for (int i = 0; i < menuEntries.Count; i++)
			{
				upLeftCorner -= Vector3.UnitY * 15;
				
				bool selected = i == selectionIdx;
				offsets[i] = Vector3.Lerp(offsets[i], selected ? Vector3.UnitX : Vector3.Zero, (float)offsetTime);
				
				FontUtil.DrawText(menuEntries.Keys.ElementAt(i), upLeftCorner + offsets[i], 10, selected ? selectedSettings : defaultSettings);
			}
		}

		/// <summary>
		/// Generate offsets for the menu entries
		/// </summary>
		private void setOffsets()
		{
			offsets = new List<Vector3>();
			for(int i = 0; i < menuEntries.Count; i++)
				offsets.Add(Vector3.Zero);
		}

		/// <summary>
		/// Begins a new immediate mode screen creation
		/// </summary>
		private void screenBegin()
		{
			dumpEntries();
			
			selectionIdx = 0;
			menuEntries.Clear();
		}

		/// <summary>
		/// Adds a menu entry to the current immediate mode screen
		/// </summary>
		private void screenOption(string name, Action action) => menuEntries.Add(name, action);

		/// <summary>
		/// Ends the current immediate mode screen creation
		/// </summary>
		private void screenEnd() => setOffsets();

		/// <summary>
		/// Transition to the file select state
		/// </summary>
		private void setStateFileSelect()
		{
			screenBegin();

			string[] files = Directory.GetFiles("./saves");
			for (int i = 0; i < files.Length; i++)
			{
				int idx = i;
				screenOption($"{Labels.SAVE_PREFIX} {idx}", () =>
				{
					Globals.CurrentSave = Path.GetFileName(files[idx]);
					SaveState state = new SaveState(idx);
					state.Restore();
					switchAction = ()=>
					{
						Game.Instance.World = LevelManager.Instance.GetCurrent();
					};
				});
			}
			
			screenOption(Labels.BACK, () => setStateContinue()); 
			
			screenEnd();
		}

		/// <summary>
		/// Transition to the rank view state
		/// </summary>
		private void setStateRankview()
		{
			screenBegin();
			
			for (int i = 0; i < Scoreboard.Instance.Scores.Count; i++)
			{
				int idx = i;
				string score = $"{Scoreboard.Instance.Scores[idx]}";
				if(!menuEntries.ContainsKey(score))
					screenOption(score, ()=>{});
			}
			
			screenOption(Labels.BACK, () => setStateContinue());
			
			screenEnd();
		}

		/// <summary>
		/// Transition to the continue state view
		/// </summary>
		private void setStateContinue()
		{
			screenBegin();
			
			Globals.ChallengeMode = true;
			
			screenOption(Labels.NEW_GAME, (() =>
			{
				Globals.CurrentSave = Directory.Exists("./saves") ? Directory.GetFiles("./saves").Length.ToString() : "0";
				switchAction = ()=>
				{
					LevelManager.Instance.CreatePoolByName(AssetLut.LEVEL_POOL_ADVENTURE);
					Game.Instance.World = LevelManager.Instance.GetNext();
				};
			}));

			if (Directory.Exists("./saves") && Directory.GetFiles("./saves").Length != 0)
				screenOption(Labels.CONTINUE, () => setStateFileSelect());
			
			if (Scoreboard.Instance.Scores.Count != 0)
				screenOption(Labels.LEADERBOARD, ()=>setStateRankview());   

			screenOption(Labels.BACK, new Action(() =>
			{
				Globals.ChallengeMode = false;
				setStateModuleSelect();
			}));
			
			screenEnd();
		}

		/// <summary>
		/// Transition to the difficulty view state
		/// </summary>
		private void setStateDifficulty()
		{
			screenBegin();
			
			Globals.ChallengeMode = false;
			
			screenOption(Labels.LEVEL_EASY, (() =>
			{
				switchAction = ()=>
				{
					LevelManager.Instance.CreatePool(AssetLut.LEVEL_POOL_EASY);
					Game.Instance.World = LevelManager.Instance.GetNext();
				};
			}));
			
			screenOption(Labels.LEVEL_MEDIUM, (() =>
			{
				switchAction = ()=>
				{
					LevelManager.Instance.CreatePool(AssetLut.LEVEL_POOL_MEDIUM);
					Game.Instance.World = LevelManager.Instance.GetNext();
				};
			}));
			
			screenOption(Labels.LEVEL_HARD, (() =>
			{
				switchAction = ()=>
				{
					LevelManager.Instance.CreatePool(AssetLut.LEVEL_POOL_HARD);
					Game.Instance.World = LevelManager.Instance.GetNext();
				};
			}));
			
			screenOption(Labels.BACK, () => setStateModuleSelect());
			
			screenEnd();
		}

		/// <summary>
		/// Transition to the editor level select view state
		/// </summary>
		private void setStateEditLevelSelect()
		{
			screenBegin();
			
			screenOption(Labels.EDITOR_NEW, (() =>
			{
				switchAction = ()=>
				{
					Game.Instance.World = LevelManager.Instance.GetEditor();
				};
			}));

			EditorUtil.AssertDirectories();
			string[] levels = EditorUtil.GetEditorLevels();
			for (int i = 0; i < levels.Length; i++)
			{
				int idx = i;
				screenOption($"{Labels.EDITOR_LEVEL_PREFIX} {levels[idx]}", (() =>
				{
					switchAction = ()=>
					{
						string content = File.ReadAllText($"./playerlevels/{levels[idx]}");
						EditorUtil.CurrentLevelFile = $"./playerlevels/{levels[idx]}";
						Game.Instance.World = LevelManager.Instance.GetEditor(content);
					};
				}));
			}
			
			screenOption(Labels.BACK, () => setStateEditorSelect());
			
			screenEnd();
		}
		
		/// <summary>
		/// Transition to the playermade level select view state
		/// </summary>
		private void setStateOwnLevelSelect()
		{
			screenBegin();
			
			EditorUtil.AssertDirectories();
			string[] levels = EditorUtil.GetEditorLevels();
			for (int i = 0; i < levels.Length; i++)
			{
				int idx = i;
				screenOption($"{Labels.EDITOR_LEVEL_PREFIX} {levels[idx]}", (() =>
				{
					switchAction = ()=>
					{
						Globals.CurrentSave = EditorUtil.GetEditorSaves().Length.ToString();
						string content = File.ReadAllText($"./playerlevels/{levels[idx]}");
						Game.Instance.World = LevelManager.Instance.GetLevel(content);
					};
				}));
			}
			
			screenOption(Labels.BACK, ()=>setStateEditorSelect());
			
			screenEnd();
		}

		/// <summary>
		/// Transition to the playermade level delete view state
		/// </summary>
		private void setStateLevelDelete()
		{
			screenBegin();
			
			EditorUtil.AssertDirectories();
			
			string[] levels = EditorUtil.GetEditorLevels();
			for (int i = 0; i < levels.Length; i++)
			{
				int idx = i;
				screenOption($"{Labels.EDITOR_LEVEL_PREFIX} {levels[idx]}", new Action(() =>
				{
					string path = $"./playerlevels/{levels[idx]}";
					File.Delete(path);
					setStateLevelDelete();
				}));
			}
			
			screenOption(Labels.BACK, new Action(() =>
			{
				setStateEditorSelect();
			}));
			
			screenEnd();
		}

		private void setStateEditorContinue()
		{
			screenBegin();
			
			string[] saves = EditorUtil.GetEditorSaves();
			for (int i = 0; i < saves.Length; i++)
			{
				int idx = i;
				screenOption($"{Labels.SAVE} {idx}", (() =>
				{
					Globals.Editor = true;
					
					switchAction = ()=>
					{
						Globals.CurrentSave = saves[idx];
						SaveState state = new SaveState("playerlevels", idx).RestoreState();
						LevelManager.Instance.LevelPool = new List<string>() { state.LevelPath };
						LevelManager.Instance.CurrentLevelIndex = 1;
						string snap = $"{state.GridWidth}:{state.GridHeight}:{state.GridSnapshot}";
						Game.Instance.World = LevelManager.Instance.GetLevel(snap);
					};
				}));
			}
			
			screenOption(Labels.BACK, new Action(() =>
			{
				Globals.Editor = false;
				setStateModuleSelect();
			}));
			
			screenEnd();
		}

		/// <summary>
		/// Transition to the editor mode select view state
		/// </summary>
		private void setStateEditorSelect()
		{
			screenBegin();

			Globals.Editor = true;
			
			EditorUtil.AssertDirectories();

			int lCount = EditorUtil.GetEditorLevels().Length;
			int sCount = EditorUtil.GetEditorSaves().Length;
			
			if(lCount != 0)
				screenOption(Labels.NEW_GAME, () => setStateOwnLevelSelect());

			if (sCount != 0)
				screenOption(Labels.CONTINUE, () => setStateEditorContinue());

			screenOption(Labels.EDITOR_EDIT, () => setStateEditLevelSelect());

			if(lCount != 0)
				screenOption(Labels.EDITOR_DELETE, ()=>setStateLevelDelete());
			
			screenOption(Labels.BACK, (() =>
			{
				Globals.Editor = false;
				setStateModuleSelect();
			}));
			
			screenEnd();
		}
		
		/// <summary>
		/// Transition to the module select view state
		/// </summary>
		private void setStateModuleSelect()
		{
			screenBegin();
			screenOption(Labels.MODULE_CHALLENGE, () => setStateContinue());
			screenOption(Labels.MODULE_SELECTOR, () => setStateDifficulty());
			screenOption(Labels.MODULE_EDITOR, () => setStateEditorSelect());
			screenOption(Labels.BACK, () => setStateDefault());
			screenEnd();
		}

		/// <summary>
		/// Transition to the settings view state
		/// </summary>
		private void setStateSettings()
		{
			screenBegin();
			
			screenOption($"VSync: {(Game.Instance.VSync == VSyncMode.On ? Labels.SETTINGS_YES : Labels.SETTINGS_NO)}", () =>
			{
				switch (Game.Instance.VSync)
				{
					case VSyncMode.Off:
						Game.Instance.VSync = VSyncMode.On;
						break;
					case VSyncMode.On:
						Game.Instance.VSync = VSyncMode.Off;
						break;
				}
				setStateSettings();
			});
			screenOption(Labels.BACK, () => setStateDefault());
			
			screenEnd();
		}

		/// <summary>
		/// Transition to the default main menu view state
		/// </summary>
		private void setStateDefault()
		{
			screenBegin();

			screenOption(Labels.PLAY, () => setStateModuleSelect());
			screenOption(Labels.SETTINGS, () => setStateSettings());
			screenOption(Labels.QUIT, () => Environment.Exit(0));
			
			screenEnd();
		}

		/// <summary>
		/// Dump the old menu entries so they may fade out
		/// </summary>
		private void dumpEntries()
		{
			oldEntriesTime = 0;
			oldEntries = new();
			foreach(var key in menuEntries.Keys)
				oldEntries.Add(key);
		}
	}
}