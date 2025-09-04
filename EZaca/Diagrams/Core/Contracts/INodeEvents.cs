namespace EZaca.Diagrams
{
    /// <summary>
    /// Can receive events for nodes.
    /// </summary>
    public interface INodeEvents
    {
        /// <summary>
        /// Notify the node a port was added as child.
        /// </summary>
        public void OnPortAdded(PortElement port);

        /// <summary>
        /// Notify the node a port was removed from child, all connections
        /// should be removed.
        /// </summary>
        public void OnPortRemoved(PortElement port);
    }
}