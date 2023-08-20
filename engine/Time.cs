using System;
using System.Collections.Generic;

namespace Sokoban.Engine
{
	/// <summary>
	/// A helper class for measuring real world time values
	/// </summary>
	public static class Time
	{
		/// <summary>
		/// The time since the last frame
		/// </summary>
		public static double DeltaTime;
		
		/// <summary>
		/// The total time the game has been running for
		/// </summary>
		public static double TotalTime;
		
		/// <summary>
		/// The exact time now
		/// </summary>
		public static double ExactTime;
		
		/// <summary>
		/// Whether or not the timeflow should be paused
		/// </summary>
		public static bool Pause;

		// Time of the previous tick
		private static DateTime lastTickTime = DateTime.Now;

		public static void RecordTick()
		{
			var delta = (DateTime.Now - lastTickTime).TotalSeconds;
			lastTickTime = DateTime.Now;
			Time.DeltaTime = delta;
			Time.TotalTime += Time.DeltaTime;
			Time.ExactTime = DateTime.Now.TimeOfDay.TotalSeconds;
		}
	}
}