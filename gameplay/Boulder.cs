using OpenTK.Mathematics;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
	/// <summary>
	/// An immovable obstacle, rendered on a grid
	/// </summary>
	public class Boulder : Actor
	{
		public Vector2i GridPosition;
		public Vector3 RenderPosition;

		private RenderItem item;

		public Boulder()
		{
			item = new RenderItem()
			{
				Position = RenderPosition,
				Size = Vector2.One * 16f,
				Offset = new Vector2(7, 2)
			};
		}

		public override void Render()
		{
			item.Position = RenderPosition;
			Renderer.Instance.Queue(item);
		}
	}
}