using System;

namespace EZaca.Diagrams
{
    /// <summary>
    /// Options for auto-connectable ports.
    /// </summary>
    [Flags]
    public enum PortOptions
    {
        /// <summary>
        /// The port is not connectable.
        /// </summary>
        None = 0,

        /// <summary>
        /// You can drag connections from the port.
        /// </summary>
        AllowDragConnections = 1,

        /// <summary>
        /// You can drop connections in the port.
        /// </summary>
        AllowDropConnections = 2,

        /// <summary>
        /// The port can connect to itself.
        /// </summary>
        AllowSelfConnect = 4,

        Everything = AllowDragConnections | AllowDropConnections | AllowSelfConnect,
    }
}