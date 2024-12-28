using Zenject;

namespace Match3Test.Untility
{
    /// <summary>
    /// This container can be used to resolve scene-wide dependencies from anywhere.
    /// Since resolving dependencies by hand from the container is actually an anti-pattern,
    /// use this reference only when absolutely needed
    /// </summary>
    public static class SceneDiContainer
    {
        /// <summary>
        /// The value of Container is overwritten by Scene Installers when a scene is loaded
        /// </summary>
        public static DiContainer Container;
    }
}