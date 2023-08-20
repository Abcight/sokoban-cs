using System;
using System.IO;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sokoban.gameplay;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
    /// <summary>
    /// An actor that displays the winscreen information
    /// </summary>
    public class WinInterface : Actor
    {
        private SwitchTimer timer;
        private bool next = false;
        private DateTime startTime;

        private float opacity = 0;
        private float opacityEnd = 0;

        public WinInterface() => startTime = DateTime.Now;

        public override void Render()
        {
			removeSaveState();

            opacity += (float)Time.DeltaTime * 3;
            opacity = Math.Min(opacity, 1);

            FontSettings settings = new FontSettings();
            settings.Color.A = opacity;

            FontSettings settingsEnd = new FontSettings();
            settingsEnd.Color.A = opacityEnd * 0.4f;

			if (timer == null)
				return;

			if (!next && Globals.ChallengeMode)
			{
				double t = Math.Max((DateTime.Now - startTime).TotalSeconds * 0.5f, 0);
				t = Math.Min(t, 1);
				int score = (int)MathHelper.Lerp(0, Globals.Score, t);

				Vector3 offset = Vector3.UnitY * 15;
				FontUtil.DrawText(Labels.GAME_END, offset - Vector3.UnitX * FontUtil.MeasureWidth(Labels.GAME_END) * 0.5f, 16, settings);

				offset -= Vector3.UnitY * 15;
				string scoreText = $"{Labels.GAME_SCORE}: {score}";
				FontUtil.DrawText(scoreText, offset - Vector3.UnitX * FontUtil.MeasureWidth(scoreText, 10) * 0.5f, 10, settings);

				if (t == 1.0)
				{
					opacityEnd += (float)Time.DeltaTime * 3;
					opacityEnd = Math.Min(opacityEnd, 1.0f);
					offset -= Vector3.UnitY * 20;
					string text = Labels.GAME_RETURN;
					FontUtil.DrawText(text, offset - Vector3.UnitX * FontUtil.MeasureWidth(text, 8) * 0.5f, 8, settingsEnd);
				}

				if (Input.GetKeyDown(Keys.Enter) || Input.GetKeyDown(Keys.KeyPadEnter) ||
					Input.GetKeyDown(Keys.Space))
				{
					if (t == 1.0f)
						next = true;
					else
						startTime = DateTime.UnixEpoch;
				}
			}
			else
			{
				timer = new SwitchTimer(() =>
				{
					if (Globals.ChallengeMode)
					{
						Scoreboard.Instance.AddScore(Globals.Score);
						Scoreboard.Write();
					}
					Globals.Score = 0;
					Game.Instance.World = LevelManager.Instance.GetMenu();
				}, true);
			}

			timer.Update();
        }

		private void removeSaveState()
		{
			if (Globals.CurrentSave != string.Empty)
			{
				if (Directory.Exists("./saves") && !Globals.Editor)
				{
					if (File.Exists($"./saves/{Globals.CurrentSave}"))
						File.Delete($"./saves/{Globals.CurrentSave}");
				}
				else if (Directory.Exists("./playerlevels/saves"))
				{
					if (File.Exists($"./playerlevels/saves/{Globals.CurrentSave}"))
						File.Delete($"./playerlevels/saves/{Globals.CurrentSave}");
				}
			}
		}
    }
}