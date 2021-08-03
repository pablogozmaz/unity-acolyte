#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace TFM.DynamicProcedures.Editor
{
    [CustomEditor(typeof(PropertiesBehaviour), true), CanEditMultipleObjects]
    public class PropertiesBehaviourEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();

            // Flag for subclass editors
            bool shouldDrawSubclassNotif = false;

            // Loop through properties and create one field (including children) for each top level property.
            SerializedProperty property = serializedObject.GetIterator();
            bool expanded = true;
            while(property.NextVisible(expanded))
            {
                if(property.name == "propertyNames")
                {
                    DrawPropertyNames(property);
                    shouldDrawSubclassNotif = true; // If next iteration happens, editor is for subclass
                    continue;
                }

                if(shouldDrawSubclassNotif)
                {
                    DrawSubclassNotif();
                    shouldDrawSubclassNotif = false;
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

        private void DrawPropertyNames(SerializedProperty property) 
        {
            Debug.Assert(property.isArray);

            List<int> indexesToDelete = new List<int>();

            int i = 0;
            for(; i < property.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                var arrayElement = property.GetArrayElementAtIndex(i);
                string str = GUILayout.TextField(arrayElement.stringValue);
                arrayElement.stringValue = str;

                if(GUILayout.Button("Delete", GUILayout.Width(80))) 
                {
                    int index = i;
                    indexesToDelete.Add(index);
                }
                EditorGUILayout.EndHorizontal();
            }

            foreach(int index in indexesToDelete)
            {
                property.DeleteArrayElementAtIndex(index);
            }

            if(GUILayout.Button("+"))
            {
                property.InsertArrayElementAtIndex(i);
            }

            EditorGUILayout.Separator();
        }

        private void DrawSubclassNotif()
        {
            GUIStyle style = GUI.skin.label;
            style.fontSize = 13;
            style.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField("Subclass Fields", style);
        }
    }
}
#endif