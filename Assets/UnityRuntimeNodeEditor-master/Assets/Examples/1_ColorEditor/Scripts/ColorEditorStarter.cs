using UnityEngine;

namespace RuntimeNodeEditor.Eggode
{
    public class ColorEditorStarter : MonoBehaviour
    {
        public RectTransform editorHolder;
        public ColorNodeEditor colorEditor;

        private void Start()
        {   
            var graph = colorEditor.CreateGraph<NodeGraph>(editorHolder);
            colorEditor.StartEditor(graph);
        }
    }
}