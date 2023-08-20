using System;
using OpenTK.Mathematics;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
	/// <summary>
	/// A class used for handling the switching cutout animation
	/// </summary>
	public class SwitchTimer
	{
		public float Value;

		private Action action;

		private bool reverse;
		
		/// <summary>
		/// A utility class used for handling the switching cutout animation
		/// </summary>
		/// <param name="endAction">The action to perform when the screen has been cut to black</param>
		/// <param name="reverse">Should the screen be restored instead of cut?</param>
		public SwitchTimer(Action endAction, bool reverse = false)
		{
			action = endAction;
			this.reverse = reverse;
			Renderer.Instance.ScreenCutoutDirection = 0;
			if (reverse)
			{
				Renderer.Instance.ScreenCutoutDirection = 1;
				Value = Renderer.Instance.ScreenCutout;
			}
		}

		public void Update()
		{
			if ((!reverse && Value >= 40.0f) || (reverse && Value == 0))
				action.Invoke();
			Value += ((float)Time.DeltaTime * 25 + Math.Abs(Value) * (float)Time.DeltaTime * 12) * (reverse ? -1 : 1);
			Value = MathHelper.Clamp(Value, 0, 40);
			Renderer.Instance.ScreenCutout = (int) Value;
		}
	}
}