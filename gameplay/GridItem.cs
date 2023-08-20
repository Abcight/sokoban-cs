using System;
using OpenTK.Mathematics;
using Sokoban.Engine;

namespace Sokoban.Gameplay
{
	/// <summary>
	/// An item that resides on the grid and moves smoothly on it
	/// </summary>
	public class GridItem : Actor
	{
		/// <summary>
		/// Smoothing time
		/// </summary>
		public float MoveSpeed = 10;
		
		/// <summary>
		/// The T component of interpolation function for smoothing the position between points a and b
		/// where the interpolation is described as (1 - T) * a + b * T
		/// </summary>
		public float MoveTime;
		
		/// <summary>
		/// Interpolated position.
		/// </summary>
		public Vector3 SmoothPosition;
		
		/// <summary>
		/// Position on the grid.
		/// </summary>
		public Vector2i GridPosition;
		
		/// <summary>
		/// Position on the grid.
		/// </summary>
		public Vector3 RenderPosition;
		
		// internal smoothing variables
		private DateTime moveStartTime;
		private Vector3 lastSmoothPosition;
		
		/// <summary>
		/// A function for resetting the interpolation time for smoothing.
		/// </summary>
		public void ResetMoveTime()
		{
			lastSmoothPosition = SmoothPosition;
			moveStartTime = DateTime.Now;
		}

		public override void Render()
		{
			MoveTime = (float)(DateTime.Now - moveStartTime).TotalSeconds * MoveSpeed;
			MoveTime = Math.Min(MoveTime, 1);
			SmoothPosition = Vector3.Lerp(lastSmoothPosition, RenderPosition, MoveTime);
		}
	}
}