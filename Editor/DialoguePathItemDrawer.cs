using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    [CustomPropertyDrawer( typeof(DialogueOpener.DialoguePathItem) )]
    public class DialoguePathItemDrawer : PropertyDrawer
    {
        static List<string> DialoguePaths;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var pathProp = property.FindPropertyRelative( nameof(DialogueOpener.DialoguePathItem.path) );
            var argvsProp = property.FindPropertyRelative( nameof(DialogueOpener.DialoguePathItem.argvs) );
            var delayProp = property.FindPropertyRelative( nameof(DialogueOpener.DialoguePathItem.delay) );

            using (new EditorGUI.PropertyScope( position, label, property )) {
                position.height = EditorGUIUtility.singleLineHeight;

                using (var check = new EditorGUI.ChangeCheckScope()) {
                    var result = EditorGUI.Popup( position,
                        new( pathProp.displayName, pathProp.tooltip ),
                        DialoguePaths.IndexOf( pathProp.stringValue ), DialoguePaths.Select( p => new GUIContent( p ) ).ToArray() );
                    if ( check.changed ) {
                        pathProp.stringValue = DialoguePaths[result];
                    }
                }
                
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField( position, argvsProp );
                
                position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField( position, delayProp );
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
        }
        
        [InitializeOnLoadMethod]
        internal static void CacheAllDialoguePaths() {
            DialoguePaths = new(16);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    if ( !type.IsSubclassOf( typeof(Dialogue) ) || type.IsAbstract ) continue;
                    DialoguePaths.Add( DialogueManager.GetPath( type ) );
                }
            }
        }

    }
}