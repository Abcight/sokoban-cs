using OpenTK.Mathematics;
using Sokoban.gameplay;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
	/// <summary>
	/// An actor used to display a watermark in the menu
	/// </summary>
	public class MenuWatermark : Actor
	{
		public override void Render()
		{
			FontSettings settings = new FontSettings();
			settings.Color.A = 0.4f;
			
			Vector3 bottomRightCorner = new Vector3(100.0f, -82, 0);
			FontUtil.DrawText(Labels.PAGE_URL, bottomRightCorner, 5, settings);
		}
	}
}