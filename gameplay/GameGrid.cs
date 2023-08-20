using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Sokoban.Engine;
using System.IO;
using System.Linq;

namespace Sokoban.Gameplay
{
    /// <summary>
    /// The main gameplay class.
    /// Manages items spread across a grid and their placement on the screen.
    /// </summary>
    public class GameGrid : Actor
    {
        /// <summary>
        /// Width of the grid
        /// </summary>
        public int Width;
        
        /// <summary>
        /// Height of the grid
        /// </summary>
        public int Height;

        /// <summary>
        /// The amount of steps the player has taken on the grid.
        /// </summary>
        public int Steps = 0;
        
        /// <summary>
        /// The amount of points the player has accumulated on the grid.
        /// </summary>
        public int Points => (int)(Math.Max(100 - (Math.Max(Steps - 10, 0) / 2), 0) * 0.5f);

        /// <summary>
        /// The singleton instance of the grid
        /// </summary>
        public static GameGrid Instance;
        
        /// <summary>
        /// Player grid actor
        /// </summary>
        public Player player = new Player();
        
        /// <summary>
        /// A map of actors placed on the grid
        /// </summary>
        public List<List<Actor>> map = new ();
        
        /// <summary>
        /// Box grid actors
        /// </summary>        
        public List<Box> boxes = new();
        
        /// <summary>
        /// Boulder grid actors
        /// </summary>  
        public List<Boulder> boulders = new();
        
        /// <summary>
        /// Plates grid actors
        /// </summary>  
        public List<Vector2i> plates = new();

        public SwitchTimer Timer;
        public bool SwitchOut = false;

        public GameGrid(int width = 10, int height = 9)
        {
            Instance = this;
            
            Width = width;
            Height = height;
            
            for (int i = 0; i < width; i++)
            {
                map.Add(new List<Actor>());
                for (int j = 0; j < height; j++)
                    map[i].Add(null);
            }

            Box box = new Box();
            box.GridPosition = new Vector2i(2, 2);
            boxes.Add(box);
            map[2][2] = box;
            
            plates.Add(new Vector2i(3, 5));

            map[0][0] = player;
        }

        /// <summary>
        /// Instantiates actors on the grid based on the provided snapshot in a custom format.
        /// The grid's size is adjusted too.
        /// </summary>
        public void LoadFromSnapshot(int width, int height, string snapshot)
        {
            boxes = new List<Box>();
            plates = new List<Vector2i>();
            boulders = new List<Boulder>();
            
            Width = width;
            Height = height;

            string[] split = snapshot.Split(':');
            snapshot = split.Last();

            map = new();
            for (int i = 0; i < Width; i++)
            {
                map.Add(new List<Actor>());
                for (int j = 0; j < Height; j++)
                    map[i].Add(null);
            }

            int chIdx = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    map[x][y] = null;

                    int value = int.Parse(snapshot[chIdx].ToString());
                    if (value - 5 >= 0)
                    {
                        value -= 5;
                        plates.Add(new Vector2i(x, y));
                    }
                    
                    if (value == 2)
                    {
                        Box box = new Box();
                        map[x][y] = box;
                        box.GridPosition = new Vector2i(x, y);
                        boxes.Add(box);
                    }
                    
                    if (value == 3)
                    {
                        Boulder boulder = new Boulder();
                        map[x][y] = boulder;
                        boulder.GridPosition = new Vector2i(x, y);
                        boulders.Add(boulder);
                    }

                    if (value == 1)
                    {
                        player.GridPosition= new Vector2i(x, y);
                        map[x][y] = player;
                    }

                    chIdx++;
                }
            }
            
            foreach(var box in boxes)
                if (IsPlate(box.GridPosition.X, box.GridPosition.Y))
                    box.IsOnPlate = true;
        }
        
        /// <summary>
        /// Instantiates actors on the grid based on the provided snapshot in a custom format.
        /// The grid's size is read from the snapshot string.
        /// </summary>
        public void LoadFromSnapshot(string snapshot)
        {
            boxes = new List<Box>();
            plates = new List<Vector2i>();
            boulders = new List<Boulder>();

            if(File.Exists(snapshot))
                snapshot = File.ReadAllText(snapshot);
            
            string[] split = snapshot.Split(':');
            
            Width = int.Parse(split[0]);
            Height = int.Parse(split[1]);
            snapshot = split[2];

            map = new();
            for (int i = 0; i < Width; i++)
            {
                map.Add(new List<Actor>());
                for (int j = 0; j < Height; j++)
                    map[i].Add(null);
            }

            int chIdx = 0;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    map[x][y] = null;

                    int value = int.Parse(snapshot[chIdx].ToString());
                    if (value - 5 >= 0)
                    {
                        value -= 5;
                        plates.Add(new Vector2i(x, y));
                    }
                    
                    if (value == 2)
                    {
                        Box box = new Box();
                        map[x][y] = box;
                        box.GridPosition = new Vector2i(x, y);
                        boxes.Add(box);
                    }
                    
                    if (value == 3)
                    {
                        Boulder boulder = new Boulder();
                        map[x][y] = boulder;
                        boulder.GridPosition = new Vector2i(x, y);
                        boulders.Add(boulder);
                    }

                    if (value == 1)
                    {
                        player.GridPosition= new Vector2i(x, y);
                        map[x][y] = player;
                    }

                    chIdx++;
                }
            }
            
            foreach(var box in boxes)
                if (IsPlate(box.GridPosition.X, box.GridPosition.Y))
                    box.IsOnPlate = true;
        }

        /// <summary>
        /// Takes a snapshot of the grid and encodes it in a custom format.
        /// </summary>
        public string Snapshot()
        {
            string str = string.Empty;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int value = 0;
                    if (map[x][y] is Player)
                        value += 1;
                    if (map[x][y] is Box)
                        value += 2;
                    if (map[x][y] is Boulder)
                        value += 3;
                    if (IsPlate(x, y))
                        value += 5;
                    str += value;
                }
            }

            return str;
        }

        /// <summary>
        /// Gets the actor of a grid at a given coordinate
        /// </summary>
        public Actor GetActor(int x, int y)
        {
            if (x < 0 || x >= Width)
                return null;
            if (y < 0 || y >= Height)
                return null;
            return map[x][y];
        }

        /// <summary>
        /// Moves the box on the grid in x axis by dx and in the y axis by dy.
        /// </summary>
        public bool MoveBox(Box box, int dx, int dy, bool teleport = false)
        {
            int tx = box.GridPosition.X + dx;
            int ty = box.GridPosition.Y + dy;

            if (tx < Width && tx >= 0)
            {
                if (GetActor(tx, box.GridPosition.Y) == null)
                {
                    map[box.GridPosition.X][box.GridPosition.Y] = null;
                    map[tx][box.GridPosition.Y] = box;
                    box.GridPosition = new Vector2i(tx, box.GridPosition.Y);
                    box.ResetMoveTime();
                    box.IsOnPlate = IsPlate(box.GridPosition.X, box.GridPosition.Y);
                    return true;
                }
            }
            
            if (ty < Height && ty >= 0)
            {
                if (GetActor(box.GridPosition.X, ty) == null)
                {
                    map[box.GridPosition.X][box.GridPosition.Y] = null;
                    map[box.GridPosition.X][ty] = box;
                    box.GridPosition = new Vector2i(box.GridPosition.X, ty);
                    box.ResetMoveTime();
                    box.IsOnPlate = IsPlate(box.GridPosition.X, box.GridPosition.Y);
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Moves the box on the grid in x axis by dx and in the y axis by dy.
        /// </summary>
        public bool IsPlate(int x, int y)
        {
            foreach(var plate in plates)
                if (plate.X == x && plate.Y == y)
                    return true;
            return false;
        }

        /// <summary>
        /// Moves the player on the grid in x axis by dx and in the y axis by dy.
        /// </summary>
        public void MovePlayer(int dx, int dy)
        {
            int tx = player.GridPosition.X + dx;
            int ty = player.GridPosition.Y + dy;

            if (GetActor(tx, ty) is Box)
                if (!MoveBox((Box) GetActor(tx, ty), dx, dy))
                    return;

            bool moved = false;

            if (tx < Width && tx >= 0 && dx != 0)
            {
                if (GetActor(tx, player.GridPosition.Y) == null)
                {
                    map[player.GridPosition.X][player.GridPosition.Y] = null;
                    map[tx][player.GridPosition.Y] = player;
                    player.GridPosition = new Vector2i(tx, player.GridPosition.Y);
                    player.ResetMoveTime();
                    if (dx == 1)
                        player.WalkDirection = Player.Direction.Right;
                    else if (dx == -1)
                        player.WalkDirection = Player.Direction.Left;

                    moved = true;
                }
            }
            
            if (ty < Height && ty >= 0 && dy != 0)
            {
                if (GetActor(player.GridPosition.X, ty) == null)
                {
                    map[player.GridPosition.X][player.GridPosition.Y] = null;
                    map[player.GridPosition.X][ty] = player;
                    player.GridPosition = new Vector2i(player.GridPosition.X, ty);
                    player.ResetMoveTime();
                    if (dy == 1)
                        player.WalkDirection = Player.Direction.Up;
                    else if (dy == -1)
                        player.WalkDirection = Player.Direction.Down;
                    
                    moved = true;
                }
            }

            if (moved)
                Steps++;
            
            // Check if the move warranted a victory
            if (CheckFinished())
                SwitchOut = true;
        }

        /// <summary>
        /// Returns true if all of the boxes have been aligned with the plates
        /// </summary>
        public bool CheckFinished()
        {
            foreach (var plate in plates)
            {
                bool hasBox = false;
                foreach(var box in boxes)
                    if (box.GridPosition == plate)
                        hasBox = true;
                if (!hasBox)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Restarts the level
        /// </summary>
        public virtual void Restart()
        {
            Timer = new SwitchTimer(() => Game.Instance.World = LevelManager.Instance.GetCurrent());
            SwitchOut = true;
        }

        #if DEBUG
        private void debugEnd()
        {
            for (int i = 0; i < plates.Count; i++)
            {
                boxes[i].GridPosition = plates[i];
            }
            MovePlayer(0, 0);
        }
        #endif

        /// <summary>
        /// Manages the code of switching out between levels
        /// </summary>
        public virtual bool ManageSwitchInOut()
        {
            if(!SwitchOut)
            {
                if (Timer == null)
                    Timer = new SwitchTimer((() =>
                    {
                        Globals.Score += Points;
                        Timer = new SwitchTimer(() =>
                        {
                            Game.Instance.World = LevelManager.Instance.GetNext();
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
        
        /// <summary>
        /// Updates the game flow
        /// </summary>
        public override void Update()
        {
            if (Time.Pause)
                return;

            if (!ManageSwitchInOut())
                return;

            if (Input.GetKeyDown(Keys.Escape))
                Time.Pause = true;
                        
            if (Input.GetKeyDown(Keys.W) || Input.GetKeyDown(Keys.Up))
                MovePlayer(0, 1);
            if (Input.GetKeyDown(Keys.S) || Input.GetKeyDown(Keys.Down))
                MovePlayer(0, -1);
            if (Input.GetKeyDown(Keys.A) || Input.GetKeyDown(Keys.Left))
                MovePlayer(-1, 0);
            if (Input.GetKeyDown(Keys.D) || Input.GetKeyDown(Keys.Right))
                MovePlayer(1, 0);
        }
        
        /// <summary>
        /// Calculates the pivot of the grid based on its width
        /// </summary>
        public Vector3 Pivot => new Vector3(-16 * Width / 2 + 3, -16 * 4, 0);

        /// <summary>
        /// Renders the grid items
        /// </summary>
        public override void Render()
        {
            Vector3 pivot = Pivot;

            player.RenderPosition = pivot + new Vector3(player.GridPosition.X, player.GridPosition.Y, 0) * 16;
            player.RenderPosition.Z -= 0.0005f;
            player.Render();

            foreach (var box in boxes)
            {
                box.RenderPosition = pivot + new Vector3(box.GridPosition.X, box.GridPosition.Y, 0) * 16;
                box.RenderPosition.Z -= 0.001f;
                box.Render();
            }
            
            foreach (var boulder in boulders)
            {
                boulder.RenderPosition = pivot + new Vector3(boulder.GridPosition.X * 16, boulder.GridPosition.Y * 16, -1);
                boulder.Render();
            }

            foreach (var plate in plates)
            {
                Renderer.Instance.Queue(new RenderItem()
                {
                    Position = pivot + new Vector3(plate.X * 16, plate.Y * 16, -1),
                    Size = Vector2.One * 16f,
                    Offset = new Vector2(5, 3)
                });
            }
            
            // Decorations
            for (int x = 0; x < Width + 2; x++)
            {
                for (int y = 0; y < Math.Min(Height + 2, 9); y++)
                {
                    if (x == 0 || x == Width + 1)
                    {
                        Renderer.Instance.Queue(new RenderItem()
                        {
                            Size = new Vector2(16, 16),
                            Position = pivot + new Vector3(x * 16 - 16, y * 16, -1),
                            Offset = new Vector2(8, 1)
                        });
                    }
                }
            }
        }
    }
}