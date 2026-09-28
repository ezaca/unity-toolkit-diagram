using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    /// <summary>
    /// A straight line connecting the center of two elements.
    /// </summary>
    public class StraightLineConnection : DiagramConnection
    {
        public Color Color { get; set; }

        public StraightLineConnection(VisualElement start, VisualElement end, Color color) : base(start, end)
        {
            Color = color;
        }

        public override void Paint(MeshGenerationContext context)
        {
            Painter2D painter = context.painter2D;

            Vector3 start = Start.ChangeCoordinatesTo(context.visualElement, Start.localBound.size / 2f);
            Vector3 end = End.ChangeCoordinatesTo(context.visualElement, End.localBound.size / 2f);
            
            painter.strokeColor = Color;
            painter.lineWidth = 2f;
            painter.BeginPath();
            painter.MoveTo(start);
            painter.LineTo(end);
            painter.Stroke();
        }
    }
}