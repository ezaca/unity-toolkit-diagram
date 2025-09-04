namespace EZaca.Diagrams
{
    public static class ExtensionsForDiagram
    {
        public static DragConnectionManipulator AddDragConnectionService(this DiagramElement diagram)
        {
            DragConnectionManipulator manipulator = DragConnectionManipulator.AttachManipulator(diagram, new BasicConnectionPaint());
            manipulator.connect = (a, b) => diagram.Connect(a, b, manipulator.painter);
            return manipulator;
        }
    }
}
