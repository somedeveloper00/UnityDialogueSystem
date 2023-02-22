using UnityEditor;
using UnityEngine;

namespace DialogueSystem
{
    [CustomPropertyDrawer( typeof(DialogueCreator) )]
    public class DialogueCreatorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var prefabProp = property.FindPropertyRelative( nameof(DialogueCreator.prefab) );
            var maxInstancesProp = property.FindPropertyRelative( nameof(DialogueCreator.maxInstances) );
            var maximumReachedBehaviourProp = property.FindPropertyRelative( nameof(DialogueCreator.maximumReachedBehaviour) );
            
            using (new EditorGUI.PropertyScope( position, label, property )) {
                position.height = EditorGUIUtility.singleLineHeight;
                string name = prefabProp.objectReferenceValue == null ? "Empty" : prefabProp.objectReferenceValue.name;
                property.isExpanded = EditorGUI.Foldout( position, property.isExpanded, name, true );

                if ( property.isExpanded ) {
                    EditorGUI.indentLevel++;
                    
                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField( position, prefabProp );
                    
                    position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField( position, maxInstancesProp );
                    
                    if ( maxInstancesProp.intValue == 0 ) maxInstancesProp.intValue = -1;
                    if ( maxInstancesProp.intValue > 1 ) {
                        position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField( position, maximumReachedBehaviourProp );
                    }
                    EditorGUI.indentLevel--;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var maxInstancesProp = property.FindPropertyRelative( nameof(DialogueCreator.maxInstances) );
            return property.isExpanded
                ? maxInstancesProp.intValue > 1
                    ? EditorGUIUtility.singleLineHeight * 4 + EditorGUIUtility.standardVerticalSpacing * 3
                    : EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2
                : EditorGUIUtility.singleLineHeight;
        }
    }
}