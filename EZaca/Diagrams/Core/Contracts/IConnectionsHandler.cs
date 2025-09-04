using System.Collections.Generic;

namespace EZaca.Diagrams
{
    internal interface IConnectionsHandler
    {
        /// <summary>
        /// List all connections.
        /// </summary>
        /// <returns>Iterable result in a non-locking way (modifying the
        /// connection list during the iteration does not affect the
        /// iteration).</returns>
        IEnumerable<(PortElement from, PortElement to)> Connections();

        /// <summary>
        /// Tell if two ports are connected in this direction.
        /// </summary>
        bool Connected(PortElement from, PortElement to);

        /// <summary>
        /// Connect two ports in this direction.
        /// </summary>
        void Connect(PortElement from, PortElement to, IConnectionPainter painter);

        /// <summary>
        /// Remove all connections for all nodes and ports.
        /// </summary>
        void DisconnectAll();

        /// <summary>
        /// Remove all the connections for the given port.
        /// </summary>
        void DisconnectPort(PortElement port);

        /// <summary>
        /// Remove all the connections for the given node.
        /// </summary>
        void DisconnectNode(NodeElement node);

        /// <summary>
        /// Remove the connection in this direction.
        /// </summary>
        void DisconnectPorts(PortElement from, PortElement to);

        /// <summary>
        /// Connect two ports or reconnect them with a new painter.
        /// </summary>
        void ReconnectPorts(PortElement from, PortElement to, IConnectionPainter painter);
        IEnumerable<(PortElement from, PortElement to)> Connections(PortElement port);
    }
}