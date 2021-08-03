using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;


namespace TFM.DynamicProcedures.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(StepAction), true)]
    public class StepActionEditor : UnityEditor.Editor
    {
        // Modified from DoDrawInspectorGUI from Unity Editor.cs source code
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();

            // Loop through properties and create one field (including children) for each top level property.
            SerializedProperty property = serializedObject.GetIterator();
            bool expanded = true;
            while(property.NextVisible(expanded))
            {
                // Hide or show "entities" property based on step action
                if(property.name == "entities")
                {
                    var stepAction = target as StepAction;

                    if(!stepAction.HasDefaultEntityList)
                    {
                        stepAction.entities = null;
                        continue;
                    }
                }

                using(new EditorGUI.DisabledScope("m_Script" == property.propertyPath))
                {
                    EditorGUILayout.PropertyField(property, true);
                }
                expanded = false;
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
    }
}
#endif