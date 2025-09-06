using EZaca.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EZaca.Diagrams.Samples
{
    public class Diagram_GetStartedForEditor : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        [MenuItem("Help/Samples/EZaca.Diagrams/Get Started", priority = 300)]
        public static void ShowExample()
        {
            Diagram_GetStartedForEditor wnd = GetWindow<Diagram_GetStartedForEditor>();
            wnd.titleContent = new GUIContent("Diagram_GetStartedForEditor");
        }

        public void CreateGUI()
        {
            VisualElement document = m_VisualTreeAsset.Instantiate();
            document.StretchToParentSize();
            rootVisualElement.Add(document);
            
            DiagramElement diagram = rootVisualElement.Q<DiagramElement>();
            diagram.AddToClassList(EditorGUIUtility.isProSkin ? "dark" : "light");

            DragConnectionManipulator dragToConnect = diagram.AddDragConnectionService();

            dragToConnect.canStartDrag = from =>
            {
                dragToConnect.painter = new BasicConnectionPaint()
                {
                    color = UIUtility.Gradient(Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f))
                };
                return true;
            };

            dragToConnect.connect = (from, target) =>
            {
                diagram.DisconnectPort(target);
                diagram.Connect(from, target, dragToConnect.painter);
            };
        }
    }
}