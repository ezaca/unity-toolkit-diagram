using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    /// <summary>
    /// A diagram node port that can be connected to other ports.
    /// </summary>
    [UxmlElement("Port", libraryPath = "EZaca.Diagrams")]
    public partial class PortElement : VisualElement
    {
        [Tooltip("Options to allow auto-connect ports")]
        [UxmlAttribute]
        public PortOptions Options { get; set; }

        [Tooltip("Color of the connection")]
        [UxmlAttribute]
        public Color ConnectionColor { get; set; }

        /// <summary>
        /// Called when after the connection is added to the diagram.
        /// </summary>
        public event Action<DiagramConnection> Connected;

        /// <summary>
        /// Custom checks to test if the ports can be connected.
        /// </summary>
        /// <remarks>
        /// This event is called for the outcoming connection, and never to the
        /// incoming connection.</remarks>
        public Func<PortElement, PortElement, bool> canConnect;

        /// <summary>
        /// Create a custom preview connection.
        /// </summary>
        /// <remarks>
        /// Set it to null to use a simple connection based in <see
        /// cref="ConnectionColor"/>.
        /// </remarks>
        public Func<VisualElement, VisualElement, DiagramConnection> previewConnection;

        /// <summary>
        /// Create a custom connection when two ports are connected.
        /// </summary>
        /// <remarks>
        /// Called in the outcoming port. Set it to null to use a simple
        /// connection based in <see cref="ConnectionColor"/>.
        /// </remarks>
        public Func<VisualElement, VisualElement, DiagramConnection> makeConnection;

        public PortElement()
        {
            AddToClassList("ez", "diagram-port");

            Options = PortOptions.Everything;
            ConnectionColor = Color.white;

            RegisterCallback<PointerDownEvent>(OnPointerDown, CallbackOptions.TrickleDown);
        }

        private void OnPointerDown(PointerDownEvent pointerDownEvent)
        {
            if (!Options.HasFlag(PortOptions.AllowDragConnections))
                return;

            DiagramElement diagram = FindDiagram();

            if (diagram is null)
            {
                Debug.LogError($"{nameof(PortElement)} must be a direct or indirect children of a {nameof(DiagramElement)} to work correctly");
                return;
            }

            pointerDownEvent.StopPropagation();

            Connectable connectable = diagram.StartConnectionPreview(
                pointerDownEvent,
                previewConnection ?? makeConnection ?? MakeStraightConnection);

            connectable.Connected += ConfirmConnection;
        }

        private static List<VisualElement> pickedElements = new();

        private void ConfirmConnection(Vector2 previewLocalPosition, PointerUpEvent pointerUp)
        {
            DiagramElement diagram = FindDiagram();

            pickedElements ??= new();
            pickedElements.Clear();

            panel.PickAll(pointerUp.position, pickedElements);
            VisualElement target = pickedElements.LastOrDefault(item => item is PortElement);

            if (target is not PortElement targetPort)
                return;

            if (this == target && !Options.HasFlag(PortOptions.AllowSelfConnect))
                return;

            if (!targetPort.Options.HasFlag(PortOptions.AllowDropConnections))
                return;

            if (canConnect?.Invoke(this, targetPort) == false)
                return;

            DiagramConnection connection = (makeConnection ?? MakeStraightConnection).Invoke(this, target);
            diagram.AddConnection(connection);
            Connected?.Invoke(connection);
        }

        private DiagramElement FindDiagram()
        {
            return GetFirstAncestorOfType<DiagramElement>();
        }

        private DiagramConnection MakeStraightConnection(VisualElement from, VisualElement to)
        {
            return new StraightLineConnection(from, to, ConnectionColor);
        }
    }
}