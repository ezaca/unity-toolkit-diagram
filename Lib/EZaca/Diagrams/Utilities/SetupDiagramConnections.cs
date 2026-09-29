using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams.Extras
{
    [ExecuteAlways]
    public class SetupDiagramConnections : MonoBehaviour
    {
        [Serializable]
        public struct Connection
        {
            [SerializeField] public VisualElementReference<PortElement> start;
            [SerializeField] public VisualElementReference<PortElement> end;
            [SerializeField] public Color color;
        }

        [SerializeField] private PanelRenderer panel;
        [SerializeField] private VisualElementReference<DiagramElement> diagram;
        [SerializeField] private List<Connection> connections;

        private List<DiagramConnection> registeredConnections = new();

        private void Awake()
        {
            if (!panel)
                panel = GetComponent<PanelRenderer>();

            panel.RegisterUIReloadCallback(OnPanelReady);
        }

        private void OnPanelReady(PanelRenderer panelRenderer, VisualElement rootElement, int version)
        {
            diagram.RegisterReferenceResolvedCallback(OnDiagramReady);
        }

        private void OnDiagramReady(DiagramElement diagram)
        {
            ClearRegisteredConnections(diagram);

            foreach (var connection in connections)
            {
                PortElement start = null;
                PortElement end = null;
                connection.start.RegisterReferenceResolvedCallback(value => start = value);
                connection.end.RegisterReferenceResolvedCallback(value => end = value);
                DiagramConnection diagramConnection = new StraightLineConnection(start, end, connection.color);

                if (start is null || end is null)
                    continue;

                diagram.AddConnection(diagramConnection);
                registeredConnections.Add(diagramConnection);
            }
        }

        private void ClearRegisteredConnections(DiagramElement diagram)
        {
            foreach (var oldConnection in registeredConnections)
                diagram.RemoveConnection(oldConnection);

            registeredConnections.Clear();
        }

        private void OnValidate()
        {
            Awake();
        }
    }
}