using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    /// <summary>
    /// Extends this class to add new connection styles.
    /// </summary>
    public abstract class DiagramConnection
    {
        /// <summary>
        /// The element where the connection starts.
        /// </summary>
        public VisualElement Start { get; }

        /// <summary>
        /// The element where the connection ends.
        /// </summary>
        public VisualElement End { get; }

        protected DiagramConnection(VisualElement start, VisualElement end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Paint the connection in the diagram.
        /// </summary>
        /// <param name="context"></param>
        public abstract void Paint(MeshGenerationContext context);
    }
}