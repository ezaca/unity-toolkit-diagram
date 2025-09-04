using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace EZaca.Diagrams.Internals
{
    /// <summary>
    /// Internal manipulator to paint the connections of a diagram.
    /// </summary>
    public class ConnectionsManipulator : Manipulator, IConnectionsHandler
    {
        private readonly Dictionary<(PortElement from, PortElement to), Connection> connections = new();

        public ConnectionsManipulator() { }

        protected override void RegisterCallbacksOnTarget()
        {
            target.generateVisualContent += OnGenerateContent;
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.generateVisualContent -= OnGenerateContent;
        }

        private void OnGenerateContent(MeshGenerationContext context)
        {
            if (connections.Count is 0)
                return;

            DoGenerateContent(context.painter2D);
        }

        private void DoGenerateContent(Painter2D painter2D)
        {
            foreach (Connection connection in connections.Values)
                connection.painter.PaintConnection(painter2D, connection.from, connection.to);
        }

        public bool Connected(PortElement from, PortElement to)
        {
            return connections.ContainsKey((from, to));
        }

        public IEnumerable<(PortElement from, PortElement to)> Connections()
        {
            return connections.Keys;
        }

        public IEnumerable<(PortElement from, PortElement to)> Connections(PortElement port)
        {
            return connections.Keys.Where(k => k.from == port || k.to == port);
        }

        public void Connect(PortElement from, PortElement to, IConnectionPainter painter)
        {
            connections.Add((from, to), new Connection(from, to, painter));
        }

        public void DisconnectAll()
        {
            connections.Clear();
        }

        public void DisconnectNode(NodeElement node)
        {
            RemoveWhere(conn => conn.from.parentNode == node || conn.to.parentNode == node);
        }

        public void DisconnectPort(PortElement port)
        {
            RemoveWhere(conn => conn.from == port || conn.to == port);
        }

        public void DisconnectPorts(PortElement from, PortElement to)
        {
            connections.Remove((from, to));
        }

        public void ReconnectPorts(PortElement from, PortElement to, IConnectionPainter painter)
        {
            if (connections.TryGetValue((from, to), out Connection conn))
                conn.painter = painter;
            else
                Connect(from, to, painter);
        }

        private void RemoveWhere(Predicate<Connection> test)
        {
            keysToRemove.AddRange(connections.Values.Where(conn => test(conn)));

            if (keysToRemove.Count is 0)
                return;

            foreach (Connection conn in keysToRemove)
                connections.Remove((conn.from, conn.to));
            keysToRemove.Clear();
        }
        private readonly List<Connection> keysToRemove = new();

        private class Connection
        {
            public PortElement from;
            public PortElement to;
            public IConnectionPainter painter;

            public Connection(PortElement from, PortElement to, IConnectionPainter painter)
            {
                this.from = from;
                this.to = to;
                this.painter = painter;
            }
        }
    }
}