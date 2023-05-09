using UnityEditor;

namespace DialogueSystem.Editor {
    [CustomEditor( typeof(DialogueManager) )]
    public class DialogueManagerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }
    }

    [CustomEditor( typeof(DialogueFocusManager) )]
    public class DialogueFocusManagerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }
    }

    [CustomEditor( typeof(DialogueOpener) )]
    public class DialogueOpenerEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
        }
    }
}