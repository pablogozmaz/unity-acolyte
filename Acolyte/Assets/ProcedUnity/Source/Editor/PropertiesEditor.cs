#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace ProcedUnity.UnityConfiguration
{
    [CustomEditor(typeof(Properties))]
    public class PropertiesEditor : UnityEditor.Editor
    {
        private SerializedProperty propertyInitializationText;


        public override bool RequiresConstantRepaint()
        {
            return Application.isPlaying;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();

            DrawDisabledScriptField();

            if(!Application.isPlaying)
            {
                DrawInitializationTextArea();
            }
            else 
            {
                DrawUneditablePropertyLabels();
            }

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }

        private void DrawDisabledScriptField() 
        {
            var script = serializedObject.FindProperty("m_Script");
            using(new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(script, true);
            }
        }

        private void DrawInitializationTextArea() 
        {
            propertyInitializationText = serializedObject.FindProperty("propertyInitializationText");

            GUIContent content = new GUIContent(propertyInitializationText.stringValue);

            GUIStyle style = GUI.skin.textField;
            Vector2 size = style.CalcSize(content);

            var str = propertyInitializationText.stringValue;
            propertyInitializationText.stringValue = EditorGUILayout.TextArea(str, style, GUILayout.Height(size.y));
        }

        private void DrawUneditablePropertyLabels() 
        {
            var properties = target as Properties;

            foreach(var p in properties.GetAll)
            {
                EditorGUILayout.LabelField(p.Type.ToString().ToLower() + " " + p.Name + " = " + p.GetObjectValue());
            }
        }
    }
}
#endif