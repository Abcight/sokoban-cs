namespace Sokoban.Engine
{
	/// <summary>
	/// A world entity with gameplay code
	/// </summary>
	public class Actor
	{
		/// <summary>
		/// Logic processed by the container once each frame
		/// </summary>
		public virtual void Update() { }
		
		/// <summary>
		/// Logic processed by the container once each frame before rendering
		/// </summary>
		public virtual void Render() { }
	}
}