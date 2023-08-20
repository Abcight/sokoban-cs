using OpenTK.Mathematics;
using Sokoban.Engine;
using Sokoban.gameplay;

namespace Sokoban.Gameplay
{
    /// <summary>
    /// UI Display shown in the topmost corner when the editor window is active
    /// </summary>
    public class EditorUI : Actor
    {
        public override void Render()
        {
            FontSettings transparent = new FontSettings();
            transparent.Color = new Color4(128, 128, 128, 128);

            void drawSmallText(string text, int size, float y)
            {
                float width = FontUtil.MeasureWidth(text, size);
                FontUtil.DrawText(text, new Vector3(151 - width, y, 0), 5, transparent);
            }
            
            FontUtil.DrawText(Labels.EDITOR_DELETE, new Vector3(105, 60, -20), 14);
            FontUtil.DrawText(Labels.EDITOR_CONTROLS, new Vector3(90, 45, 0), 10, transparent);
            drawSmallText(Labels.EDITOR_MOVEMENT, 5, 35);
            drawSmallText(Labels.EDITOR_REMOVE, 5, 28);
            drawSmallText(Labels.EDITOR_PLAYER, 5, 21);
            drawSmallText(Labels.EDITOR_CHEST, 5, 14);
            drawSmallText(Labels.EDITOR_OBSTACLE, 5, 7);
            drawSmallText(Labels.EDITOR_PLATE, 5, 0);
            drawSmallText(Labels.EDITOR_MINUS, 5, -7);
            drawSmallText(Labels.EDITOR_PLUS, 5, -14);
        }
    }
}