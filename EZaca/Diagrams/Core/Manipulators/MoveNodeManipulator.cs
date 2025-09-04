using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    public class MoveNodeManipulator : DragManipulator
    {
        public static MoveNodeManipulator AttachManipulator(NodeElement node)
            => AttachManipulator(node.headerContainer, new MoveNodeManipulator(node));

        private readonly NodeElement node;

        public MoveNodeManipulator(NodeElement node)
        {
            activators.Add(new ManipulatorActivationFilter() { button = MouseButton.MiddleMouse, clickCount = 1, modifiers = EventModifiers.None });
            activators.Add(new ManipulatorActivationFilter() { button = MouseButton.MiddleMouse, clickCount = 1, modifiers = EventModifiers.Control });
            activators.Add(new ManipulatorActivationFilter() { button = MouseButton.MiddleMouse, clickCount = 1, modifiers = EventModifiers.Shift });
            activators.Add(new ManipulatorActivationFilter() { button = MouseButton.MiddleMouse, clickCount = 1, modifiers = EventModifiers.Shift | EventModifiers.Control });
            this.node = node;
        }

        protected override bool CanStartDrag(PointerDownEvent evt)
        {
            target.CapturePointer(evt.pointerId);
            return true;
        }

        protected override void OnDragMove(PointerMoveEvent evt)
        {
            Vector2 oldPosition = default;
            Vector2 newPosition = new(
                node.localBound.x + evt.deltaPosition.x,
                node.localBound.y + evt.deltaPosition.y);

            if (node is not null)
                oldPosition = node.localBound.position;

            node.style.left = newPosition.x;
            node.style.top = newPosition.y;

            if (node is not null && node.diagram is not null)
                node.diagram.OnNodeMoved(node, oldPosition, newPosition);
        }

        protected override void OnEndDrag(PointerUpEvent evt)
        {
            target.ReleaseMouse();
            base.OnEndDrag(evt);
        }
    }
}
