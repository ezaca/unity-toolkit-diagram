using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    public class DragConnectionManipulator : DragManipulator
    {
        public static DragConnectionManipulator AttachManipulator(DiagramElement diagram, IConnectionPainter previewPainter)
            => AttachManipulator(diagram, new DragConnectionManipulator(previewPainter));

        public Func<PortElement, bool> canStartDrag;
        public Func<PortElement, PortElement, bool> canDrop;
        public Action<PortElement, PortElement> connect;
        public Action<PortElement> cancel;
        public IConnectionPainter painter;

        private DiagramElement diagram;
        private bool activePreview;
        private PortElement initialPort;
        private PortElement targetPort;
        private Vector2 targetCoord;

        protected override VisualElement actualTarget => diagram.nodesContainer;

        public DragConnectionManipulator(IConnectionPainter previewPainter) : base()
        {
            this.painter = previewPainter;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            if (target is not DiagramElement diagram)
            {
                Debug.LogError($"{nameof(DragConnectionManipulator)} should be registered in a diagram");
                return;
            }

            this.diagram = diagram;
            diagram.connectionsContainer.generateVisualContent += GenerateContent;
            base.RegisterCallbacksOnTarget();
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            diagram.connectionsContainer.generateVisualContent -= GenerateContent;
            base.UnregisterCallbacksFromTarget();
        }

        private void GenerateContent(MeshGenerationContext context)
        {
            if (!activePreview)
                return;

            Vector2 initialCoord = initialPort.center;
            Vector2 finalCoord = targetPort?.center ?? targetCoord;
            painter.PaintConnection(context.painter2D, initialCoord, finalCoord);
        }

        protected override bool CanStartDrag(PointerDownEvent evt)
        {
            return !activePreview
                && evt.target is PortElement port
                && port.allowDepartingConnections
                && canStartDrag?.Invoke(port) != false;
        }

        protected override bool CanDrop(EventBase eventBase, IPointerEvent pointerEvent)
        {
            return eventBase.target is PortElement port
                && port.acceptIncomingConnections
                && canDrop?.Invoke(initialPort, port) != false;
        }

        protected override void OnStartDrag(PointerDownEvent evt)
        {
            PortElement port = (PortElement)evt.target;
            initialPort = port;
            targetPort = port;
            targetCoord = ChangeCoordinate(port);
            activePreview = true;
            Repaint();
        }

        protected override void OnDragMove(PointerMoveEvent evt)
        {
            targetCoord = ChangeCoordinate((VisualElement)evt.currentTarget, evt.localPosition);
            targetPort = CanDrop(evt, evt) ? evt.target as PortElement : null;
            Repaint();
        }

        protected override void OnConfirmDrop(PointerUpEvent evt)
        {
            PortElement port = (PortElement)evt.target;
            targetPort = port;
            connect?.Invoke(initialPort, targetPort);
        }

        protected override void OnCancel(PointerUpEvent evt)
        {
            cancel?.Invoke(initialPort);
        }

        protected override void OnEndDrag(PointerUpEvent evt)
        {
            activePreview = false;
            Repaint();
        }

        private Vector2 ChangeCoordinate(PortElement port)
        {
            return port.ChangeCoordinatesTo(diagram.nodesContainer, port.localBound.size / 2f);
        }

        private Vector2 ChangeCoordinate(VisualElement element, Vector2 coord)
        {
            return element.ChangeCoordinatesTo(diagram.nodesContainer, coord);
        }

        private void Repaint()
        {
            diagram.RepaintConnections();
        }
    }
}
