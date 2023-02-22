using UnityEditor;

namespace DialogueSystem
{
    [CustomEditor( typeof(DialogueManager) )]
    public class DialogueManagerEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }
    }

    [CustomEditor( typeof(DialogueFocusManager) )]
    public class DialogueFocusManagerEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }
    }

    [CustomEditor( typeof(DialogueOpener) )]
    public class DialogueOpenerEditor : Editor
    {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }
    }
}