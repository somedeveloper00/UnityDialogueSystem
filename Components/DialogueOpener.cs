using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TriInspector;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueOpener : MonoBehaviour
    {
        [TableList]
        public DialoguePathItem[] openOnStart;

        public RectTransform parent;
        
        [PropertyTooltip("Destroys the game object after it's done")]
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
            DialogueManager.Current.GetOrCreateByPath( pathItem.path, pathItem.argvs, parent );
            if ( destroyAfter ) {
                Destroy( gameObject );
            }
        }
        
        static List<string> GetAllDialoguePaths() {
            var r = new List<string>(16);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    var path = type.GetCustomAttribute<DialoguePathAttribute>( true );
                    if ( path != null ) r.Add( path.Path );
                }
            }

            return r;
        }

        [Serializable]
        public class DialoguePathItem
        {
            [Dropdown( nameof(DialogueSystem) + "." +
                       nameof(DialogueOpener) + "." +
                       nameof(GetAllDialoguePaths) )]
            public string path;
            public string argvs;
            [LabelText("Delay (s)"), Min(0)]
            public float delay;
        }
    }
}