using EZaca.Diagrams.Internals;
using EZaca.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    /// <summary>
    /// The container for diagram elements (nodes, connectors).
    /// </summary>
    [UxmlElement]
    public partial class DiagramElement : VisualElementEx, IAttachToPanelHandler, IDiagramEvents, IConnectionsHandler
    {
        public const string NodesContainerName = "Nodes";
        public const string ConnectionsContainerName = "Connections";

        public VisualElement nodesContainer => _nodesContainer ??= GetOrCreateContainer(NodesContainerName);
        public VisualElement connectionsContainer => _connectionsContainer ??= GetOrCreateContainer(ConnectionsContainerName);
        public override VisualElement contentContainer => nodesContainer;
        public IEnumerable<NodeElement> nodes => _nodes.ToArray();

        public event Action<NodeElement> nodeAdded;
        public event Action<NodeElement> nodeRemoved;

        private readonly List<NodeElement> _nodes = new();
        private IConnectionsHandler _connectionsHandler;
        private VisualElement _nodesContainer;
        private VisualElement _connectionsContainer;

        public DiagramElement()
            : base()
        {
            _connectionsHandler = new ConnectionsManipulator();
        }

        void IAttachToPanelHandler.OnAttachToPanel(AttachToPanelEvent eventData)
        {
            _nodesContainer ??= GetOrCreateNodeContainer();
            _connectionsContainer ??= GetOrCreateConnectionsContainer();
            ResetWithNodesIniside();
        }

        public IEnumerable<(PortElement from, PortElement to)> Connections()
        {
            return _connectionsHandler.Connections();
        }

        public IEnumerable<(PortElement from, PortElement to)> Connections(PortElement port)
        {
            return _connectionsHandler.Connections(port);
        }

        public bool Connected(PortElement from, PortElement to)
        {
            return _connectionsHandler.Connected(from, to);
        }

        public void Connect(PortElement from, PortElement to, IConnectionPainter painter) { _connectionsHandler.Connect(from, to, painter); RepaintConnections(); }

        public void ReconnectPorts(PortElement from, PortElement to, IConnectionPainter painter) { _connectionsHandler.ReconnectPorts(from, to, painter); RepaintConnections(); }

        public void DisconnectPorts(PortElement from, PortElement to) { _connectionsHandler.DisconnectPorts(from, to); RepaintConnections(); }

        public void DisconnectPort(PortElement port) { _connectionsHandler.DisconnectPort(port); RepaintConnections(); }

        public void DisconnectNode(NodeElement node) { _connectionsHandler.DisconnectNode(node); RepaintConnections(); }

        public void DisconnectAll() { _connectionsHandler.DisconnectAll(); RepaintConnections(); }

        void IDiagramEvents.OnNodeAdded(NodeElement node)
        {
            _nodes.Add(node);
            nodeAdded?.Invoke(node);
        }

        void IDiagramEvents.OnNodeRemoved(NodeElement node)
        {
            _nodes.Remove(node);
            nodeRemoved?.Invoke(node);
            DisconnectNode(node);
        }

        void IDiagramEvents.OnPortRemoved(PortElement port)
        {
            DisconnectPort(port);
        }

        public void OnNodeMoved(NodeElement node, Vector2 oldValue, Vector2 newValue)
        {
            RepaintConnections();
        }

        public void RepaintConnections()
        {
            connectionsContainer.MarkDirtyRepaint();
        }

        private void ResetWithNodesIniside()
        {
            _nodes.Clear();
            _nodes.AddRange(this.Query<NodeElement>().Build());
        }

        private VisualElement GetOrCreateNodeContainer()
        {
            if (_nodesContainer is not null)
                return _nodesContainer;

            _nodesContainer = GetOrCreateContainer(NodesContainerName);
            _nodesContainer.SendToBack();
            return _nodesContainer;
        }

        private VisualElement GetOrCreateConnectionsContainer()
        {
            if (_connectionsContainer is not null)
                return _connectionsContainer;

            _connectionsContainer = GetOrCreateContainer(ConnectionsContainerName);
            _connectionsContainer.BringToFront();
            _connectionsContainer.pickingMode = PickingMode.Ignore;

            _connectionsContainer.AddManipulator((Manipulator)_connectionsHandler);

            return _connectionsContainer;
        }

        private VisualElement GetOrCreateContainer(string name)
        {
            if (hierarchy.Children().FirstOrDefault(el => el.name == name) is VisualElement container)
                return container;

            container = new VisualElement();
            container.name = name;
            container.StretchToParentSize();
            hierarchy.Add(container);
            return container;
        }
    }
}