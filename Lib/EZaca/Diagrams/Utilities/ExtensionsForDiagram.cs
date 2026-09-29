using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    public static class ExtensionsForDiagram
    {
        /// <summary>
        /// Remove all the connections from and to an element.
        /// </summary>
        public static void RemoveConnections(this DiagramElement diagram, VisualElement element)
        {
            diagram.RemoveConnections(item =>
                item.Start == element ||
                item.End == element);
        }

        /// <summary>
        /// Check if there is any connection from or to the element.
        /// </summary>
        public static bool HasConnection(this DiagramElement diagram, VisualElement element)
        {
            return diagram.FindConnection(connection => connection.Start == element || connection.End == element) is not null;
        }

        /// <summary>
        /// Check if any connection match the <paramref name="predicate"/>.
        /// </summary>
        public static bool HasConnection(this DiagramElement diagram, Predicate<DiagramConnection> predicate)
        {
            return diagram.FindConnection(predicate) is not null;
        }

        /// <summary>
        /// Check if there is any connection starting from the element.
        /// </summary>
        public static bool HasConnectionFrom(this DiagramElement diagram, VisualElement start)
        {
            return diagram.FindConnection(connection => connection.Start == start) is not null;
        }

        /// <summary>
        /// Check if there is any connection targeting the element.
        /// </summary>
        public static bool HasConnectionTo(this DiagramElement diagram, VisualElement target)
        {
            return diagram.FindConnection(connection => connection.End == target) is not null;
        }

        /// <summary>
        /// Start a connection preview from a pointer down event.
        /// </summary>
        public static Connectable StartConnectionPreview(
            this DiagramElement diagram,
            PointerDownEvent pointerDownEvent,
            Func<VisualElement, VisualElement, DiagramConnection> makeConnection)
        {
            return diagram.StartConnectionPreview(
                (VisualElement)pointerDownEvent.target,
                pointerDownEvent.pointerId,
                pointerDownEvent.position,
                makeConnection);
        }

        /// <summary>
        /// Start a connection preview from the center of an element. You may
        /// pass a pointer Id to automatically link the manipulator with a
        /// pointer, otherwise you must control it manually.
        /// </summary>
        public static Connectable StartConnectionPreview(
            this DiagramElement diagram,
            VisualElement from,
            int pointerId,
            Func<VisualElement, VisualElement, DiagramConnection> makeConnection)
        {
            return diagram.StartConnectionPreview(
                from,
                pointerId,
                from.worldBound.center,
                makeConnection);
        }
    }
}
