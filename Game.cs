using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sokoban.Engine;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using Sokoban.Gameplay;
using Image = OpenTK.Windowing.Common.Input.Image;
using Sokoban.assets;

namespace Sokoban
{
    /// <summary>
    /// A class used to handle the game window and its events
    /// </summary>
    public class Game : GameWindow
    {
        public static Game Instance;

        public World World;
        
        public Game(NativeWindowSettings settings, GameWindowSettings gwSettings) : base(gwSettings, settings)
        {
            Title = "Sokoban";
            Size = new Vector2i(1280, 720);
            VSync = VSyncMode.On;
            Instance = this;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Size = e.Size;
        }

        protected override void OnLoad()
        {
            World = LevelManager.Instance.GetMenu();
            
            loadIconMonochrome(AssetLut.TEXTURE_ICON);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.FramebufferSrgb);

            GL.ClearColor(50f / 255.0f, 111f / 255.0f, 168f / 255.0f, 1.0f);
        }
        
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
			Time.RecordTick();

            World?.Update();

            Input.Update();
        }

        
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            World?.Render();
            Renderer.Instance.Render();

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }

		private void loadIconMonochrome(string path)
		{
			FileStream fs = File.OpenRead(path);
			MemoryStream ms = new MemoryStream();
			fs.CopyTo(ms);

			byte[] iconBytes = ms.ToArray().Reverse().ToArray();
			List<byte> newBytes = new List<byte>();

			for (int i = 16 * 4; i < iconBytes.Length; i++)
				newBytes.Add(iconBytes[i]);

			Image img = new Image(16, 16, newBytes.ToArray());
			Icon = new WindowIcon(img);
		}
	}
}