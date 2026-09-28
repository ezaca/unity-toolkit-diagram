using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    /// <summary>
    /// A general purpose drag manipulator. Attach to an element to allow drag
    /// it with a pointer.
    /// </summary>
    /// <remarks>
    /// Works in editor and in-game. It uses left and top styles to move.
    /// </remarks>
    public class Dragger : PointerManipulator
    {
        public event Action DragStart;
        public event Action DragMove;
        public event Action DragEnd;

        public bool IsMoving => isMoving;

        private int pointerId;
        private bool isMoving;

        public Dragger()
        {
            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        }

        public void OnPointerDown(PointerDownEvent evt)
        {
            if (!CanStartManipulation(evt))
                return;

            isMoving = true;
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            pointerId = evt.pointerId;

            if (pointerId >= 0)
                target.CapturePointer(evt.pointerId);

            DragStart?.Invoke();
        }

        public void OnPointerMove(PointerMoveEvent evt)
        {
            target.style.left = target.resolvedStyle.left + evt.deltaPosition.x;
            target.style.top = target.resolvedStyle.top + evt.deltaPosition.y;

            if (DragMove is not null)
                target.schedule.Execute(DragMove);
        }

        public void OnPointerUp(PointerUpEvent evt)
        {
            if (!CanStopManipulation(evt))
                return;

            StopDrag();
        }

        public void Cancel()
        {
            StopDrag();
        }

        private void StopDrag()
        {
            if (pointerId >= 0)
                target.ReleasePointer(pointerId);

            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            isMoving = false;

            DragEnd?.Invoke();
        }
    }
}