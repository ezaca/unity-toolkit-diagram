namespace EZaca.Diagrams
{
    /// <summary>
    /// Direction of a port, which can receive a connection from other, issue a connection to other,
    /// both, or neither.
    /// </summary>
    public enum PortDirection
    {
        /// <summary>
        /// Connections are disabled.
        /// </summary>
        None,

        /// <summary>
        /// Can receive a connection from other port to this.
        /// </summary>
        Input,

        /// <summary>
        /// Can issue a connection from this to other port.
        /// </summary>
        Output,

        /// <summary>
        /// Can receive and issue connection from the same port.
        /// </summary>
        Both,
    }
}