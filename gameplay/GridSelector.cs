using OpenTK.Mathematics;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
	/// <summary>
	/// An actor representing the selection box in the level editor
	/// </summary>
	public class GridSelector : GridItem
	{
		private RenderItem item;

		public GridSelector()
		{
			item = new RenderItem()
			{
				Offset = new Vector2(0,2),
				Position = SmoothPosition,
				Size = Vector2.One * 16f
			};
		}
		
		public override void Render()
		{
			base.Render();
			item.Position = SmoothPosition;
			Renderer.Instance.Queue(item);
		}
	}
}