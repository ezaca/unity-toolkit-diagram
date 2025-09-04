using EZaca.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams.Samples
{
    public class Diagram_GetStarted : MonoBehaviour
    {
        private UIDocument document;

        private void OnEnable()
        {
            /**
             * This is a very simple script to showcase the Diagram.
             *
             * We put it in OnEnable because here the UIDocument instantiated
             * the VisualElements.
             *
             * Why not Awake? Elements are not present.
             *
             * Why not Start? Because it executes only once, but the
             * VisualElements are recreated every time the object is enabled.
             *
             * So let's get UIDocument component, its root element, and the
             * DiagramElement itself.
             */

            document = GetComponent<UIDocument>();
            VisualElement root = document.rootVisualElement;
            DiagramElement diagram = root.Q<DiagramElement>();

            /**
             * The ports are PortElement (custom VisualElement) inside the nodes
             * of the diagram.
             */
            PortElement portA = diagram.Q<PortElement>("PortA");
            PortElement portB = diagram.Q<PortElement>("PortB");
            PortElement portC = diagram.Q<PortElement>("PortC");
            PortElement portD = diagram.Q<PortElement>("PortD");
            PortElement portE = diagram.Q<PortElement>("PortE");
            PortElement portF = diagram.Q<PortElement>("PortF");
            PortElement portG = diagram.Q<PortElement>("PortG");

            /**
             * Here we are creating two styles of line. One is for horizontal
             * flows (default) and dark red, the other is vertical in the
             * default color (white).
             */
            BasicConnectionPaint horizontal = new BasicConnectionPaint() {
                color = UIUtility.Gradient(GradientMode.Blend, (Color.maroon, 0.1f), (Color.white, 1f)),
            };

            BasicConnectionPaint vertical = new BasicConnectionPaint() {
                direction = BasicConnectionPaint.Direction.TopDown
            };

            /**
             * Now we connect the ports using the styles.
             */
            diagram.Connect(portA, portB, vertical);
            diagram.Connect(portC, portG, horizontal);
            diagram.Connect(portD, portE, horizontal);
            diagram.Connect(portF, portG, horizontal);

            /**
             *
             * COMMOM PITFALLS
             * ---------------
             * - The nodes present from the start do not act correctly. Check if
             *   you is configuring the nodes already added from the start, and
             *   not only those added later through the subscribable event.
             *
             * - The nodes added later do not act correctly. Check if you are
             *   configuring them in the event for nodes added.
             *
             * - I can't connect doors, even if the connection preview is
             *   working fine. Maybe you forgot to set the direction of the
             *   PortElement to Input, Output, or Both.
             *
             * - Manipulator is not working as expected. Double-check if you
             *   added it to the element it expects you to add. Some of them are
             *   added to the main element, others (e.g. <see
             *   cref="MoveNodeManipulator"/>) must be added to inner children.
             */
        }
    }
}