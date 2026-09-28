using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    public delegate void ConnectableConnectDelegate(Vector2 previewLocalPosition, PointerUpEvent pointerUp);

    /// <summary>
    /// Create this manipulator to handle cases when the user drags a connection
    /// from an element to some position of the diagram.
    /// </summary>
    public class Connectable : Manipulator
    {
        /// <summary>
        /// Called when the connection is submitted. It can happen when the
        /// pointer is up, if <see cref="PointerId"/> is greater than zero.
        /// </summary>
        public event ConnectableConnectDelegate Connected;

        /// <summary>
        /// Called when the preview target position is updated.
        /// </summary>
        public event Action<Vector2> PreviewUpdated;

        /// <summary>
        /// Called if the preview was manually cancelled.
        /// </summary>
        public event Action Cancelled;

        protected DiagramElement diagram;
        protected DiagramConnection previewConnection;
        protected VisualElement previewTarget;

        /// <summary>
        /// The node where the preview starts.
        /// </summary>
        public VisualElement Start { get; }

        /// <summary>
        /// Refers to the pointer starting the connection preview. If less than
        /// zero, it was started manually by other means.
        /// </summary>
        public int PointerId { get; }

        /// <summary>
        /// Position where the preview connection ends.
        /// </summary>
        /// <remarks>
        /// Updated when <see cref="PointerId"/> is greater than 0, otherwise it
        /// will stick to the start position.
        /// </remarks>
        public Vector2 PointerPosition { get; private set; }

        /// <summary>
        /// Create a manipulator to handle a connection preview.
        /// </summary>
        /// <param name="start">The first port where the connection is coming
        /// from.</param>
        /// <param name="pointerId">The pointerId from the pointer event, or a
        /// negative number (e.g. -1) to not use pointer.</param>
        /// <param name="startPosition">World position where the preview will
        /// start.</param>
        /// <param name="makeConnection">Produces a new connection from the
        /// start to the preview element.</param>
        /// <remarks>
        /// Usually you would call <see
        /// cref="DiagramElement.StartConnectionPreview"/> rather than creating
        /// a reference of this manipulator.
        /// </remarks>
        public Connectable(
            VisualElement start,
            int pointerId,
            Vector2 startPosition,
            Func<VisualElement, VisualElement, DiagramConnection> makeConnection)
        {
            Start = start;
            PointerId = pointerId;
            PointerPosition = startPosition;
            previewTarget = ProducePreviewTarget();
            previewConnection = makeConnection(start, previewTarget);
        }

        /// <summary>
        /// Cancels the process and remove the preview.
        /// </summary>
        public void Cancel()
        {
            Cancelled?.Invoke();
            target = null;
        }

        /// <summary>
        /// Confirm the preview position and stop the process. Use this to
        /// confirm manually the preview in other way rather than pointer up
        /// event. You may use this if you passed a negative pointerId, or if
        /// you give user other options to confirm (e.g. keyboard Enter key).
        /// </summary>
        public void Submit(PointerUpEvent pointerUp = null)
        {
            Connected?.Invoke(PointerPosition, pointerUp);
            target = null;
            Repaint();
        }

        protected override void RegisterCallbacksOnTarget()
        {
            if (target is not DiagramElement diagramElement)
                throw new NotSupportedException($"{GetType().FullName} supports only {nameof(DiagramElement)}");

            diagram = diagramElement;

            if (PointerId >= 0)
            {
                target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
                target.RegisterCallback<PointerUpEvent>(OnPointerUp);
                target.CapturePointer(PointerId);
            }

            diagram.connectionsConatiner.Add(previewTarget);
            diagram.AddConnection(previewConnection);

            Vector2 startPosition = diagram.connectionsConatiner.WorldToLocal(PointerPosition);
            previewTarget.style.left = startPosition.x;
            previewTarget.style.top = startPosition.y;

            Repaint();
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);

            if (PointerId >= 0)
                target.ReleasePointer(PointerId);

            diagram?.RemoveConnection(previewConnection);
            previewTarget.parent.Remove(previewTarget);
            Repaint();
        }

        private void OnPointerMove(PointerMoveEvent pointerMove)
        {
            PointerPosition = pointerMove.position;
            Vector2 position = diagram.connectionsConatiner.WorldToLocal(PointerPosition);
            previewTarget.style.left = position.x;
            previewTarget.style.top = position.y;
            Repaint();
        }

        private void OnPointerUp(PointerUpEvent pointerUp)
        {
            PointerPosition = pointerUp.position;
            Submit(pointerUp);
        }

        /// <summary>
        /// The dummy to use as preview element target.
        /// </summary>
        private VisualElement ProducePreviewTarget()
        {
            return new()
            {
                name = "PreviewTarget",
                pickingMode = PickingMode.Ignore,
                style =
                {
                    position = Position.Absolute,
                    left = 0,
                    top = 0,
                    width = 0,
                    height = 0,
                }
            };
        }

        /// <summary>
        /// Repaint and notify update.
        /// </summary>
        private void Repaint()
        {
            diagram?.RepaintConnections();
            PreviewUpdated?.Invoke(PointerPosition);
        }
    }
}