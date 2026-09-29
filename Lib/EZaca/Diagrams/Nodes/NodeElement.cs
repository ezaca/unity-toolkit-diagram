using System.Collections.Generic;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    [UxmlElement(libraryPath = "EZaca.Diagrams")]
    public partial class NodeElement : VisualElement
    {
        [UxmlAttribute]
        public bool Draggable
        {
            get => dragger is not null;
            set => dragger = value ? MakeDraggable() : null;
        }
        private Dragger dragger;

        [UxmlAttribute]
        public bool BringToFrontOnDrag { get; set; }

        public NodeElement()
        {
            AddToClassList("ez", "diagram-node");
            Draggable = true;
        }

        private Dragger MakeDraggable()
        {
            dragger = new Dragger();
            dragger.DragMove += UpdateDiagram;
            
            if (BringToFrontOnDrag)
                dragger.DragStart += BringToFront;

            this.AddManipulator(dragger);
            return dragger;
        }

        private void UpdateDiagram()
        {
            GetFirstAncestorOfType<DiagramElement>().RepaintConnections();
        }

        public DiagramElement FindParentDiagram()
        {
            return GetFirstAncestorOfType<DiagramElement>();
        }

        public IEnumerable<PortElement> FindChildrenPorts()
        {
            return this.Query<PortElement>().Build();
        }
    }
}