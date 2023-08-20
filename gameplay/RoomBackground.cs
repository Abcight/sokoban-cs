using System.Collections.Generic;
using Sokoban.Engine;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace Sokoban.Gameplay
{
	/// <summary>
	/// A class used for generating the level background
	/// </summary>
	public class RoomBackground : Actor
	{
		private Vector2 TOP_LEFT_CORNER = new Vector2(4, 0);
		private Vector2 TOP_MIDDLE = new Vector2(5, 0);
		private Vector2 TOP_RIGHT_CORNER = new Vector2(6, 0);
		private Vector2 MIDDLE_LEFT = new Vector2(4, 1);
		private Vector2 MIDDLE_RIGHT = new Vector2(6, 1);
		private Vector2 BOTTOM_LEFT_CORNER = new Vector2(4, 2);
		private Vector2 BOTTOM_MIDDLE = new Vector2(5, 2);
		private Vector2 BOTTOM_RIGHT_CORNER = new Vector2(6, 2);

		private List<RenderItem> staticItems = new List<RenderItem>();
		private List<Vector2> lightPositions = new List<Vector2>();
		
		public RoomBackground()
		{
			for (int x = 0; x < 20; x++)
			{
				for (int y = 0; y < 11; y++)
				{
					Vector2 offset = Vector2.Zero;

					if (y == 0)
						offset = BOTTOM_MIDDLE;
					else if (y == 10)
						offset = TOP_MIDDLE;
					
					if (x == 0)
					{
						if(y==0)
							offset = BOTTOM_LEFT_CORNER;
						else if (y == 10)
							offset = TOP_LEFT_CORNER;
						else
							offset = MIDDLE_LEFT;
					}
					
					if (x == 19)
					{
						if(y==10)
							offset = TOP_RIGHT_CORNER;
						else if (y == 0)
							offset = BOTTOM_RIGHT_CORNER;
						else
							offset = MIDDLE_RIGHT;
					}
					
					staticItems.Add(new RenderItem()
					{
						Position = new Vector3(x * 16 - 160 + 8, y * 16 - 90 + 8 + 2, -20),
						Size = Vector2.One * 16,
						Offset = offset
					});
					
					if(x > 2 && x < 18 && y != 0 && y != 10 && x % 3 == 0)
						lightPositions.Add(new Vector2(x * 16 - 160 + 8, y * 16 - 90 + 8 + 2));
					
					if(x > 2 && x < 18 && y != 0 && y == 10 && x % 3 == 0)
						staticItems.Add(new RenderItem()
						{
							Position = new Vector3(x * 16 - 160 + 8, y * 16 - 90 + 8 + 2, -8f),
							Size = Vector2.One * 16,
							Offset = new Vector2(7, 0)
						});
				}
			}
			
			staticItems.Add(new RenderItem()
			{
				Position = new Vector3(1.2f * 16 - 160 + 8, 10 * 16 - 90 + 8 + 2, -7),
				Size = Vector2.One * 16,
				Offset = new Vector2(7, 1)
			});
			
			staticItems.Add(new RenderItem()
			{
				Position = new Vector3(18.2f * 16 - 160, 10 * 16 - 90 + 8 + 2, -7),
				Size = Vector2.One * 16,
				Offset = new Vector2(7, 1)
			});
		}

		public override void Render()
		{
			foreach(var item in staticItems)
				Renderer.Instance.Queue(item);
			
			foreach(var light in lightPositions)
				Renderer.Instance.QueueLight(light);
		}
	}
}