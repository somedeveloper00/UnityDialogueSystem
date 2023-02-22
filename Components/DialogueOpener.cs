using System;
using System.Collections;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueOpener : MonoBehaviour
    {
        public DialoguePathItem[] openOnStart;
        public RectTransform parentTransform;
        
        [Tooltip("Destroys the game object after it's done")]
        public bool destroyGameObjectAfterwards = true;

        void Start() {
            for ( int i = 0; i < openOnStart.Length; i++ ) {
                var path = openOnStart[i];
                StartCoroutine( openPathItem( path, (i == openOnStart.Length - 1) && destroyGameObjectAfterwards ) );
            }
        }

        IEnumerator openPathItem(DialoguePathItem pathItem, bool destroyAfter) {
            if (pathItem.delay > 0)
                yield return new WaitForSecondsRealtime( pathItem.delay );
            DialogueManager.Current.GetOrCreateByPath( pathItem.path, pathItem.argvs, parentTransform );
            if ( destroyAfter ) {
                Destroy( gameObject );
            }
        }
        
        [Serializable]
        public class DialoguePathItem
        {
            public string path;
            public string argvs;
            [InspectorName("Delay (s)"), Min(0)]
            public float delay;
        }
    }
}