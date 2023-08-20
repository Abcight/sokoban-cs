using System;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Sokoban.Engine
{
    /// <summary>
    /// A helper class for tracking mouse and keyboard input.
    /// </summary>
    public static class Input
    {
        public static bool ShowCursor = false;

        /// <summary>
        /// Returns true if the provided key was pressed during the last frame but not in the frame before that.
        /// </summary>
        /// <param name="key">An enum value representing the key to test for</param>
        public static bool GetKey(Keys key)
        {
            if (Game.Instance == null)
                return false;
            
            return Game.Instance.KeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Returns true if the provided key was pressed during the last frame but not in the frame before that.
        /// </summary>
        /// <param name="key">An enum value representing the key to test for</param>
        public static bool GetKeyDown(Keys key)
        {
            if (Game.Instance == null)
                return false;
            
            return Game.Instance.KeyboardState.IsKeyDown(key) && !Game.Instance.KeyboardState.WasKeyDown(key);
        }

        /// <summary>
        /// Returns true if the provided key was pressed during the last frame.
        /// </summary>
        /// <param name="key">A string representing the key name</param>
        public static bool GetKey(string key)
        {
            return Game.Instance.KeyboardState.IsKeyDown((Keys) Enum.Parse(typeof(Keys), key));
        }

        /// <summary>
        /// Returns true if the provided key was pressed during the last frame but not in the frame before that.
        /// </summary>
        /// <param name="key">A string representing the key name</param>
        public static bool GetKeyDown(string key)
        {
            return Game.Instance.KeyboardState.IsKeyDown((Keys) Enum.Parse(typeof(Keys), key)) &&
                   !Game.Instance.KeyboardState.WasKeyDown((Keys) Enum.Parse(typeof(Keys), key));
        }

        /// <summary>
        /// Polls a new input frame from the input devices.
        /// </summary>
        public static void Update()
        {
            Game.Instance.CursorGrabbed = false;
        }
    }
}