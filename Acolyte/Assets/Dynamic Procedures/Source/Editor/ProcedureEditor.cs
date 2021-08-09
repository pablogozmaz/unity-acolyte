#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace TFM.DynamicProcedures.Editor
{
    [CustomEditor(typeof(Procedure))]
    public class ProcedureEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();

            // Loop through properties and create one field (including children) for each top level property.
            SerializedProperty property = serializedObject.GetIterator();
            bool expanded = true;
            while(property.NextVisible(expanded))
            {
                if(property.name == "infoEntries")
                {
                    EditorGUILayout.Space();
                    DrawInfoEntries(property);
                    continue;
                }

                if(property.name == "steps")
                {
                    EditorGUILayout.Space();
                    DrawSteps(property);
                    continue;
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

        private void DrawSteps(SerializedProperty property) 
        {
            Debug.Assert(property.isArray);

            List<int> indexesToDelete = new List<int>();

            int i = 0;
            for(; i < property.arraySize; i++)
            {
                EditorGUILayout.LabelField("Step "+(i+1).ToString());

                var stepProperty = property.GetArrayElementAtIndex(i);

                DrawStepHeader (stepProperty, out bool shouldDelete);
                DrawInfoEntries(stepProperty.FindPropertyRelative("infoEntries"), 30);
                DrawStepActions(stepProperty);
                DrawStepScript(stepProperty);

                if(shouldDelete)
                {
                    int index = i;
                    indexesToDelete.Add(index);
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }

            foreach(int index in indexesToDelete)
            {
                property.DeleteArrayElementAtIndex(index);
            }

            if(GUILayout.Button("Add new step"))
            {
                property.InsertArrayElementAtIndex(i);
            }

            EditorGUILayout.Separator();
        }

        private void DrawStepHeader(SerializedProperty stepProperty, out bool shouldDelete) 
        {
            EditorGUILayout.BeginHorizontal();

            SerializedProperty stepName = stepProperty.FindPropertyRelative("name");

            stepName.stringValue = GUILayout.TextField(stepName.stringValue);

            if(GUILayout.Button("Delete", GUILayout.Width(80)))
            {
                string message = "Are you sure you want to delete '" + stepName.stringValue + "'?";
                shouldDelete = EditorUtility.DisplayDialog("Procedure configuration", message, "Yes", "Cancel");
            }
            else
                shouldDelete = false;

            EditorGUILayout.EndHorizontal();
        }

        private void DrawInfoEntries(SerializedProperty entries, int leftMargin = 0) 
        {
            Debug.Assert(entries.isArray);

            // Header
            EditorGUILayout.BeginHorizontal();
            if(leftMargin > 0) GUILayout.Space(leftMargin);
            EditorGUILayout.LabelField("Info Entries");
            EditorGUILayout.EndHorizontal();

            // Keep track of indexes of elements to delete after draw
            List<int> indexesToDelete = new List<int>();

            int i = 0;
            for(; i < entries.arraySize; i++)
            {
                SerializedProperty entry = entries.GetArrayElementAtIndex(i);
                DrawInfoEntry(entry, i, leftMargin, out bool shouldDeleteEntry);

                if(shouldDeleteEntry)
                    indexesToDelete.Add(i);

                if(i < entries.arraySize - 1) 
                    EditorGUILayout.Space();
            }

            foreach(int index in indexesToDelete)
            {
                entries.DeleteArrayElementAtIndex(index);
            }

            // Add new entry button
            EditorGUILayout.BeginHorizontal();
            if(leftMargin > 0) GUILayout.Space(leftMargin);
            if(GUILayout.Button("Add new info entry"))
            {
                entries.InsertArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawInfoEntry(SerializedProperty entry, int index, int leftMargin, out bool shouldDelete) 
        {
            EditorGUILayout.BeginHorizontal();
            if(leftMargin > 0) GUILayout.Space(leftMargin);

            SerializedProperty header = entry.FindPropertyRelative("header");
            EditorGUILayout.LabelField((index + 1).ToString(), GUILayout.Width(16));
            header.stringValue = EditorGUILayout.TextField(header.stringValue);

            shouldDelete = GUILayout.Button("Delete", GUILayout.Width(80));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if(leftMargin > 0) GUILayout.Space(leftMargin);
            SerializedProperty body = entry.FindPropertyRelative("body");
            body.stringValue = EditorGUILayout.TextArea(body.stringValue);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawStepActions(SerializedProperty stepProperty) 
        {
            SerializedProperty stepActions = stepProperty.FindPropertyRelative("actions");

            Debug.Assert(stepActions.isArray);

            // Header
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(30);
            EditorGUILayout.LabelField("Actions");
            EditorGUILayout.EndHorizontal();

            // Keep track of indexes of elements to delete after draw
            List<int> indexesToDelete = new List<int>();

            int i = 0;
            for(; i < stepActions.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();

                GUILayout.Space(30);

                SerializedProperty action = stepActions.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(action, new GUIContent(""));

                if(GUILayout.Button("Delete", GUILayout.Width(80)))
                {
                    int index = i;
                    indexesToDelete.Add(index);
                }

                EditorGUILayout.EndHorizontal();
            }

            // Delete required elements
            foreach(int index in indexesToDelete)
            {
                stepActions.DeleteArrayElementAtIndex(index);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(30);
            if(GUILayout.Button("Add new action"))
            {
                stepActions.InsertArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawStepScript(SerializedProperty stepProperty)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(30);
            var script = stepProperty.FindPropertyRelative("scriptAsset");
            EditorGUILayout.PropertyField(script);
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif