using OpenTK.Mathematics;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
    /// <summary>
    /// A movable obstacle, rendered on a grid
    /// </summary>
    public class Box : GridItem
    {
        private RenderItem item;

        public bool IsOnPlate
        {
            set
            {
                item.Offset = value ? new Vector2(4, 3) : new Vector2(5, 1);
            }
        }
        
        public Box()
        {
            MoveSpeed = 6;
            item = new RenderItem()
            {
                Position = SmoothPosition,
                Size = Vector2.One * 16f,
                Offset = new Vector2(5, 1),
                ShadowCaster = 1
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