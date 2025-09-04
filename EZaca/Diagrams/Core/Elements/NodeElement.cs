using EZaca.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    /// <summary>
    /// The node inside the diagram.
    /// </summary>
    [UxmlElement]
    public partial class NodeElement : VisualElementEx, IAttachToPanelHandler, IDetachFromPanelHandler, INodeEvents
    {
        public const string BodyElementName = "NodeBody";
        public const string HeaderElementName = "NodeHeader";
        public const string TitleElementName = "NodeTitle";

        private readonly List<PortElement> _ports = new();
        private IDiagramEvents _parentDiagram;
        private VisualElement _headerContainer;
        private Label _headerTitleLabel;
        private VisualElement _bodyContainer;
        private string _initialTitle;
        private MoveNodeManipulator _moveNodeManipulator;

        public IReadOnlyList<PortElement> ports => _ports;
        public IDiagramEvents diagram => _parentDiagram;

        [UxmlAttribute]
        public string title
        {
            get => _initialTitle;
            set
            {
                _initialTitle = value;
                if (_headerTitleLabel is not null)
                    _headerTitleLabel.text = _initialTitle;
            }
        }

        [UxmlAttribute]
        public bool movable
        {
            get => _moveNodeManipulator is not null;
            set
            {
                bool wasMovable = _moveNodeManipulator is not null;
                bool nowIsMovable = value;
                if (wasMovable == value)
                    return;

                if (!nowIsMovable && _moveNodeManipulator is not null)
                    _moveNodeManipulator.target = null;

                _moveNodeManipulator = nowIsMovable ? new MoveNodeManipulator(this) : null;
                if (_moveNodeManipulator is not null && _headerTitleLabel is not null)
                    _headerTitleLabel.AddManipulator(_moveNodeManipulator);
            }
        }

        public VisualElement bodyContainer
        {
            get => _bodyContainer ??= GetOrCreateBody(this);
            set => _bodyContainer = value;
        }

        public VisualElement headerContainer
        {
            get
            {
                if (_headerContainer is not null)
                    return _headerContainer;
                _headerContainer = GetOrCreateHeader(this);
                _headerTitleLabel = GetOrCreateTitle(_headerContainer);
                _headerTitleLabel.text = _initialTitle;
                return _headerContainer;
            }
            set => _headerContainer = value;
        }

        public override VisualElement contentContainer => bodyContainer;

        public NodeElement()
        {
            _initialTitle = "Node";
            _moveNodeManipulator = new MoveNodeManipulator(this);
        }

        void IAttachToPanelHandler.OnAttachToPanel(AttachToPanelEvent eventData)
        {
            _bodyContainer = GetOrCreateBody(this);
            _headerContainer = GetOrCreateHeader(this);
            _headerTitleLabel = GetOrCreateTitle(_headerContainer);
            _headerTitleLabel.text = _initialTitle;

            ResetWithPortsInside();
            _parentDiagram = SearchBottomUp<IDiagramEvents>();
            _parentDiagram?.OnNodeAdded(this);
        }

        void IDetachFromPanelHandler.OnDetachFromPanel(DetachFromPanelEvent eventData)
        {
            if (_parentDiagram is null)
                return;
            _parentDiagram?.OnNodeRemoved(this);
            _parentDiagram = null;
        }

        void INodeEvents.OnPortAdded(PortElement port)
        {
            _ports.Add(port);
        }

        void INodeEvents.OnPortRemoved(PortElement port)
        {
            _ports.Remove(port);
            _parentDiagram?.OnPortRemoved(port);
        }

        private void ResetWithPortsInside()
        {
            _ports.Clear();
            _ports.AddRange(this.Query<PortElement>().Build());
        }

        private static VisualElement GetOrCreateBody(VisualElement parent)
        {
            VisualElement body = GetOrCreateElement<VisualElement>(parent, BodyElementName, true);
            body.BringToFront();
            return body;
        }

        private static VisualElement GetOrCreateHeader(VisualElement parent)
        {
            VisualElement header = GetOrCreateElement<VisualElement>(parent, HeaderElementName, true);
            header.SendToBack();
            return header;
        }

        private Label GetOrCreateTitle(VisualElement parent)
        {
            Label title = GetOrCreateElement<Label>(parent, TitleElementName, false);

            if (_moveNodeManipulator is not null)
                title.AddManipulator(_moveNodeManipulator);

            return title;
        }

        private static TElement GetOrCreateElement<TElement>(VisualElement parent, string name, bool directChild) where TElement : VisualElement, new()
        {
            TElement element = directChild
                ? parent.hierarchy.Children().FirstOrDefault(el => el is TElement && el.name == name) as TElement
                : parent.Q<TElement>(name);

            if (element is not null)
                return element;

            element = new TElement();
            element.name = name;

            if (name is HeaderElementName)
                parent.hierarchy.Insert(0, element);
            else
                parent.hierarchy.Add(element);
                

            return element;
        }
    }
}