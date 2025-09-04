using EZaca.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    [UxmlElement]
    public partial class PortElement : VisualElementEx, IAttachToPanelHandler, IDetachFromPanelHandler
    {
        private PortDirection _direction;
        private IDiagramEvents _parentDiagram;
        private INodeEvents _parentNode;

        [UxmlAttribute]
        public PortDirection direction
        {
            get => _direction;
            set => _direction = value;
        }

        public DiagramElement diagram => (DiagramElement)_parentDiagram;
        public NodeElement node => (NodeElement)_parentNode;
        public INodeEvents parentNode => _parentNode;
        public bool acceptIncomingConnections => _direction is PortDirection.Input or PortDirection.Both;
        public bool allowDepartingConnections => _direction is PortDirection.Output or PortDirection.Both;
        public Vector2 center => this.ChangeCoordinatesTo(diagram.connectionsContainer, localBound.size / 2f);

        public PortElement()
        {
            _direction = PortDirection.Both;
        }

        void IAttachToPanelHandler.OnAttachToPanel(AttachToPanelEvent eventData)
        {
            _parentNode = SearchBottomUp<INodeEvents>();
            _parentDiagram = SearchBottomUp<IDiagramEvents>();

            _parentNode?.OnPortAdded(this);
        }

        void IDetachFromPanelHandler.OnDetachFromPanel(DetachFromPanelEvent eventData)
        {
            if (_parentNode is not null)
                _parentNode.OnPortRemoved(this);
            else
                _parentDiagram?.OnPortRemoved(this);
        }
    }
}