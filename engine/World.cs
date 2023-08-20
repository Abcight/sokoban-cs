using System.Collections.Generic;

namespace Sokoban.Engine
{
    /// <summary>
    /// A logical compound of actors
    /// </summary>
    public class World
    {
        public readonly List<Actor> Actors = new();
        
        /// <summary>
        /// Performs an update call for each of the stored actors
        /// </summary>
        public void Update()
        {
            foreach(var actor in Actors)
                actor.Update();
        }

        /// <summary>
        /// Performs a render call for each of the stored actors
        /// </summary>
        public void Render()
        {
            foreach(var actor in Actors)
                actor.Render();
        }
    }
}