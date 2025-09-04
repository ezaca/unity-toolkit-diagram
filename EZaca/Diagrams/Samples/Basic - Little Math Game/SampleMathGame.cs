using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams.Samples
{
    public class SampleMathGame : MonoBehaviour
    {
        [SerializeField] private UIDocument document;
        [SerializeField] private Color unresolvedColor;
        [SerializeField] private Color correctColor;
        [SerializeField] private Color wrongColor;
        private DiagramElement diagram;
        private NodeElement value10;
        private NodeElement value20;
        private NodeElement value2;
        private NodeElement add;
        private NodeElement multiply;
        private NodeElement result;
        private VisualElement resultFinalValue;
        private Dictionary<NodeElement, int> valuesToInt;

        private void OnEnable()
        {
            // References, references...
            diagram = document.rootVisualElement.Q<DiagramElement>();
            value10 = document.rootVisualElement.Q<NodeElement>("Value10");
            value20 = document.rootVisualElement.Q<NodeElement>("Value20");
            value2 = document.rootVisualElement.Q<NodeElement>("Value2");
            add = document.rootVisualElement.Q<NodeElement>("Add");
            multiply = document.rootVisualElement.Q<NodeElement>("Multiply");
            result = document.rootVisualElement.Q<NodeElement>("Result");
            resultFinalValue = result.Q<VisualElement>("FinalValue");

            // Value nodes to their actual value. For sample it's sufficient,
            // but for a game you may want it to be reflected in the node value
            // through a configurable way.
            valuesToInt = new()
            {
                { value10, 10 },
                { value20, 20 },
                { value2, 2 },
            };

            // Configure the diagram connections
            DragConnectionManipulator drag = diagram.AddDragConnectionService();

            drag.canDrop = (from, to) => from.parentNode != to.parentNode; // Should not link node to itself (infinite loop)

            drag.connect = (from, to) =>
            {
                // One input to one output, no more. Then check result.
                diagram.DisconnectPort(from);
                diagram.DisconnectPort(to);
                diagram.Connect(from, to, drag.painter);
                CheckResult();
            };

        }

        /// <summary>
        /// Check if the math expression was resolved to the correct result,
        /// wrong result, or if it's still pending.
        /// </summary>
        private void CheckResult()
        {
            bool? hasResult = EvaluateResult();
            resultFinalValue.style.backgroundColor = hasResult switch
            {
                null => unresolvedColor,
                true => correctColor,
                false => wrongColor,
            };
        }

        /// <summary>
        /// Evaluate the result.
        /// </summary>
        /// <returns>True is correct answer, false is wrong answer, null is
        /// because no operation is bound to result.</returns>
        private bool? EvaluateResult()
        {
            const bool WrongAnswer = true;
            bool? NoResultYet = null;

            (PortElement from, PortElement to) resultConnection = diagram.Connections(result.ports[0]).FirstOrDefault();

            // Result node is not connected
            if (resultConnection.from is null)
                return NoResultYet;

            // Answer are wrong
            //  - No value is the result
            //  - Operators should not be tested: if no operator is connected to
            //    result, it could happen of one operator being connected to
            //    other in infinite loop
            if (resultConnection.from.name.StartsWith("Value"))
                return WrongAnswer;

            // Here there is a connection to an operation, and no infinite loop
            // possible. Evalute the expression.
            int resultingValue = ResolveValue();
            return resultingValue == 240;
        }

        /// <summary>
        /// Evaluate the expression and return its actual value. Here it is the
        /// risk of infinite loop (actually stack overflow), because no check is
        /// made for interconnected nodes.
        /// </summary>
        private int ResolveValue()
        {
            // Get all the components. Output ports, input ports, and
            // connections to the input ports.
            PortElement addPortA = InputPortA(add);
            PortElement addPortB = InputPortB(add);
            PortElement addPortResult = Port(add);

            PortElement multiplyPortA = InputPortA(multiply);
            PortElement multiplyPortB = InputPortB(multiply);
            PortElement multiplyPortResult = Port(multiply);

            PortElement finalPort = Port(result);
            (PortElement finalConn, PortElement _) = diagram.Connections(finalPort).FirstOrDefault();

            (PortElement addConnA, _) = diagram.Connections(addPortA).FirstOrDefault();
            (PortElement addConnB, _) = diagram.Connections(addPortB).FirstOrDefault();

            (PortElement multiplyConnA, _) = diagram.Connections(multiplyPortA).FirstOrDefault();
            (PortElement multiplyConnB, _) = diagram.Connections(multiplyPortB).FirstOrDefault();

            // Here we get the actual return of this method
            bool endsWithAdd = finalConn == addPortResult;
            bool endsWithMultiply = finalConn == multiplyPortResult;

            if (endsWithAdd)
                return ResolveSum();
            else if (endsWithMultiply)
                return ResolveMultiply();
            else
                return 0;

            // ----------------------------------------------------------------
            // The methods below help to resolve the sum and multiplication in
            // correct order without doubling the code.

            /// Get the inputs of the Add node and returns the sum. I didn't use
            /// parameters as we would hard code below anyway. Feel free to
            /// refactor if it becomes more readable.
            int ResolveSum()
            {
                int a = ResolvePortValue(addConnA);
                int b = ResolvePortValue(addConnB);
                return a + b;
            }

            /// Get the inputs of the Multiply node and returns the
            /// multiplication. See "ResolveSum" for additional information on
            /// parametrization.
            int ResolveMultiply()
            {
                int a = ResolvePortValue(multiplyConnA);
                int b = ResolvePortValue(multiplyConnB);
                return a * b;
            }

            /// Resolve the value connected to an input port, which can be a
            /// Value, Add or Multiply node, or none.
            int ResolvePortValue(PortElement port)
            {
                string nodeName = port?.node.name;
                return port is null ? 0 :
                    nodeName.StartsWith("Value") ? valuesToInt[port.node] :
                    nodeName == multiply.name ? ResolveMultiply() :
                    nodeName == add.name ? ResolveSum() :
                    throw new NotImplementedException($"Operation for '{nodeName}' not implemented");
            }
        }

        private PortElement Port(NodeElement element)
        {
            return element.ports.FirstOrDefault(p => p.name == "Port"); // Find the reference: One way to get the port
        }

        private PortElement InputPortA(NodeElement element)
        {
            return element.Q<PortElement>("PortA"); // Find the element: Another way to get the port
        }

        private PortElement InputPortB(NodeElement element)
        {
            return element.Q<PortElement>("PortB");
        }

        private void Reset()
        {
            document = GetComponent<UIDocument>();
        }
    }
}