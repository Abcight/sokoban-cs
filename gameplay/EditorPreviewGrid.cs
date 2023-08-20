namespace Sokoban.Gameplay
{
    /// <summary>
    /// Grid used when previewing an editor level
    /// </summary>
    public class EditorPreviewGrid : GameGrid
    {
        public string PreviewSnap;
        
        public EditorPreviewGrid(string previewSnapshot)
        {
            PreviewSnap = previewSnapshot;
            LoadFromSnapshot(previewSnapshot);
        }
        
        public override void Restart()
        {
            Timer = new SwitchTimer(() => Game.Instance.World = LevelManager.Instance.GetPreview(PreviewSnap));
            SwitchOut = true;
        }

        public override bool ManageSwitchInOut()
        {
            if(!SwitchOut)
            {
                if (Timer == null)
                    Timer = new SwitchTimer((() =>
                    {
                        Globals.Score += Points;
                        Timer = new SwitchTimer(() =>
                        {
                            Game.Instance.World = LevelManager.Instance.GetEditor(PreviewSnap);
                        });
                    }), true);

                if (Timer.Value != 0)
                {
                    Timer.Update();
                    return false;
                }
            }
            else
            {
                Timer.Update();
                return false;
            }

            return true;
        }
    }
}