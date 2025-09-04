using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    public abstract class DragManipulator : PointerManipulator
    {
        public DragManipulator()
        {
            activators.Clear();
            activators.Add(new ManipulatorActivationFilter() { button = MouseButton.LeftMouse, clickCount = 1, modifiers = EventModifiers.None });
            activators.Add(new ManipulatorActivationFilter() { button = MouseButton.LeftMouse, clickCount = 1, modifiers = EventModifiers.Control });
            activators.Add(new ManipulatorActivationFilter() { button = MouseButton.LeftMouse, clickCount = 1, modifiers = EventModifiers.Shift });
            activators.Add(new ManipulatorActivationFilter() { button = MouseButton.LeftMouse, clickCount = 1, modifiers = EventModifiers.Shift | EventModifiers.Control });
        }

        protected virtual VisualElement actualTarget => target;

        protected static TManipulator AttachManipulator<TManipulator>(VisualElement target, TManipulator manipulator)
            where TManipulator : IManipulator
        {
            target.AddManipulator(manipulator);
            return manipulator;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            actualTarget.RegisterCallback<PointerDownEvent>(DoPointerDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            actualTarget.UnregisterCallback<PointerDownEvent>(DoPointerDown);
        }

        private void DoPointerDown(PointerDownEvent evt)
        {
            if (!CanStartManipulation(evt) || !CanStartDrag(evt))
                return;

            actualTarget.RegisterCallback<PointerMoveEvent>(DoPointerMove);
            actualTarget.RegisterCallback<PointerUpEvent>(DoPointerUp);
            OnStartDrag(evt);
        }

        private void DoPointerMove(PointerMoveEvent evt)
        {
            OnDragMove(evt);
        }

        private void DoPointerUp(PointerUpEvent evt)
        {
            if (!CanStopManipulation(evt))
                return;

            actualTarget.UnregisterCallback<PointerMoveEvent>(DoPointerMove);
            actualTarget.UnregisterCallback<PointerUpEvent>(DoPointerUp);

            if (CanDrop(evt, evt))
                OnConfirmDrop(evt);
            else
                OnCancel(evt);
            
            OnEndDrag(evt);
        }

        /// <summary>
        /// Called on pointer down, with correct activators and validation <see
        /// cref="CanStartDrag"/>.
        /// </summary>
        protected virtual void OnStartDrag(PointerDownEvent evt) { }

        /// <summary>
        /// Called on move pointer after drag, with no further validation.
        /// </summary>
        protected virtual void OnDragMove(PointerMoveEvent evt) { }

        /// <summary>
        /// Tell if drag can be started on pointer down.
        /// </summary>
        protected virtual bool CanStartDrag(PointerDownEvent evt) => true;

        /// <summary>
        /// Tell if is over valid drop site. Can be called by derivative classes
        /// on move, or by this class on pointer up.
        /// </summary>
        protected virtual bool CanDrop(EventBase eventBase, IPointerEvent pointerEvent) => true;

        /// <summary>
        /// Called on pointer up, after validation of <see cref="CanDrop"/>.
        /// </summary>
        protected virtual void OnConfirmDrop(PointerUpEvent evt) { }

        /// <summary>
        /// Called on pointer up, if fails to validate <see cref="CanDrop"/>.
        /// </summary>
        protected virtual void OnCancel(PointerUpEvent evt) { }

        /// <summary>
        /// Called on pointer up, after validate or reject drop are called.
        /// </summary>
        protected virtual void OnEndDrag(PointerUpEvent evt) { }
    }
}
