using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    [UxmlElement(libraryPath = "EZaca.Diagrams")]
    public partial class DiagramElement : VisualElement
    {
        public event Action<DiagramConnection> ConnectionAdded;
        public event Action<DiagramConnection> ConnectionRemoved;

        public readonly VisualElement connectionsConatiner;
        private readonly VisualElement nodesContainer;
        private readonly List<DiagramConnection> connections = new();
        private Connectable currentPreview;

        public override VisualElement contentContainer => nodesContainer;

        public DiagramElement()
        {
            nodesContainer = new() { name = "Container" };
            connectionsConatiner = new() { name = "Connections" };

            nodesContainer.StretchToParentSize();
            connectionsConatiner.StretchToParentSize();
            connectionsConatiner.pickingMode = PickingMode.Ignore;

            hierarchy.Add(nodesContainer);
            hierarchy.Add(connectionsConatiner);

            AddToClassList("ez", "diagram");

            connectionsConatiner.generateVisualContent += GenerateVisualContent;
        }

        /// <summary>
        /// Add a connection between nodes.
        /// </summary>
        public void AddConnection(DiagramConnection connection)
        {
            connections.Add(connection);
            ConnectionAdded?.Invoke(connection);
        }

        /// <summary>
        /// Remove a specific connection.
        /// </summary>
        public void RemoveConnection(DiagramConnection connection)
        {
            ConnectionRemoved?.Invoke(connection);
            connections.Remove(connection);
        }

        /// <summary>
        /// Remove all the connections matching a criteria.
        /// </summary>
        public void RemoveConnections(Predicate<DiagramConnection> condition)
        {
            if (ConnectionRemoved is not null)
            {
                foreach (DiagramConnection connection in connections)
                {
                    if (condition(connection))
                        ConnectionRemoved?.Invoke(connection);
                }
            }

            connections.RemoveAll(condition);
        }

        public DiagramConnection FindConnection(Predicate<DiagramConnection> predicate)
        {
            foreach (DiagramConnection connection in connections)
            {
                if (predicate(connection))
                    return connection;
            }
            return null;
        }

        /// <summary>
        /// Call to repaint the connections. It will schedule a <see
        /// cref="VisualElement.MarkDirtyRepaint"/>.
        /// </summary>
        public void RepaintConnections()
        {
            connectionsConatiner.schedule.Execute(() => connectionsConatiner.MarkDirtyRepaint());
        }

        private void GenerateVisualContent(MeshGenerationContext context)
        {
            foreach (DiagramConnection connection in connections)
            {
                connection.Paint(context);
            }
        }

        /// <summary>
        /// Start a connection preview.
        /// </summary>
        /// <param name="from">Start element.</param>
        /// <param name="pointerId">Pointer id to attach the manipulator to a
        /// pointer, or a negative number if you will control it
        /// manually.</param>
        /// <param name="startPosition">Position to start the preview.</param>
        /// <param name="makeConnection">Make a connection from the starting
        /// element to the preview element.</param>
        /// <returns>The connectable manipulator reference.</returns>
        public Connectable StartConnectionPreview(
            VisualElement from,
            int pointerId,
            Vector2 startPosition,
            Func<VisualElement, VisualElement, DiagramConnection> makeConnection)
        {
            if (currentPreview?.target is not null)
                currentPreview.Cancel();

            Connectable connectable = new(from, pointerId, startPosition, makeConnection);
            currentPreview = connectable;
            this.AddManipulator(connectable);
            return connectable;
        }
    }
}