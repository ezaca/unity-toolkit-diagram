using UnityEngine;

namespace EZaca.Diagrams
{
    public interface IDiagramEvents
    {
        /// <summary>
        /// Notify diagram a new node is added.
        /// </summary>
        public void OnNodeAdded(NodeElement node);

        /// <summary>
        /// Notify diagram a node was removed and should be disconnected.
        /// </summary>
        public void OnNodeRemoved(NodeElement node);

        /// <summary>
        /// Notify diagram a port was removed and should be disconnected.
        /// </summary>
        public void OnPortRemoved(PortElement port);

        /// <summary>
        /// Notify diagram the node was moved from a position to another.
        /// </summary>
        public void OnNodeMoved(NodeElement node, Vector2 oldValue, Vector2 newValue);
    }
}