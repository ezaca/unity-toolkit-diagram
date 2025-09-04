using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams
{
    /// <summary>
    /// Paints a connection connector-to-connector in a VisualElement mesh.
    /// </summary>
    /// <remarks>
    /// Use a class implementing this interface to paint visually the
    /// connections between ports. Add the class with <see
    /// cref="DiagramElement.Connect"/> or <see
    /// cref="DiagramElement.ReconnectPorts"/>.
    /// </remarks>
    public interface IConnectionPainter
    {
        /// <summary>
        /// Paint the shape with the visual connection between these two ports.
        /// </summary>
        /// <remarks>
        /// This method handles the preparation of styles and colors, the begin
        /// of the path, up to the generation of the stroke and/or fill.
        /// </remarks>
        public void PaintConnection(Painter2D painter, PortElement from, PortElement to);

        /// <inheritdoc cref="PaintConnection(Painter2D, PortElement, PortElement)"/>
        public void PaintConnection(Painter2D painter, Vector2 from, Vector2 to);
    }
}