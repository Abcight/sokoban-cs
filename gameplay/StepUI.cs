using OpenTK.Mathematics;
using Sokoban.gameplay;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
	/// <summary>
	/// Actor that renders the amount of steps the player made, as well as current score per-level.
	/// </summary>
	public class StepUI : Actor
	{
		public override void Render()
		{
			FontSettings settings = new FontSettings();
			settings.Color.A = 0.8f;
			
			Vector3 bottomLeft = new Vector3(-150, -70, 0);
			string text = $"{Labels.CHALLENGE_STEPS}: {GameGrid.Instance.Steps}";
			FontUtil.DrawText(text, bottomLeft - Vector3.UnitX, 8, settings);
			
			settings = new FontSettings();
			settings.Color.A = 0.3f;
			bottomLeft += new Vector3(0, -10, 0);
			text = $"{Labels.CHALLENGE_SCORE}: {GameGrid.Instance.Points}";
			FontUtil.DrawText(text, bottomLeft - Vector3.UnitX, 6, settings);
		}
	}
}