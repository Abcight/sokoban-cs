using OpenTK.Mathematics;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
    /// <summary>
    /// A player actor
    /// </summary>
    public class Player : GridItem
    {
        /// <summary>
        /// The direction in which the player should be rendered
        /// </summary>
        public Direction WalkDirection;

        private RenderItem item;

        public enum Direction
        {
            Down, Up, Left, Right
        }

        public Player()
        {
            item = new RenderItem()
            {
                Position = SmoothPosition,
                Size = Vector2.One * 16f,
                ShadowCaster = 1
            };
        }

        public override void Render()
        {
            base.Render();
            item.Offset = getOffset();
            item.Position = SmoothPosition;
            Renderer.Instance.Queue(item);
        }
        
        private Vector2 getOffset()
        {
            var frame = 1;
            if (MoveTime > 0.25f)
                frame = 2;
            if (MoveTime > 0.5f)
                frame = 0;
            if (MoveTime > 0.75f)
                frame = 1;
            return new Vector2(MoveTime == 1 ? 1 : (int)frame + 1, (int)WalkDirection);
        }
    }
}