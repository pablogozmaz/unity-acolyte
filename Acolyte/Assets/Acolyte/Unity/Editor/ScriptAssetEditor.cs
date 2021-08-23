#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Acolyte
{
    [CustomEditor(typeof(ScriptAsset), true)]
    public class ScriptAssetEditor : UnityEditor.Editor
    {
        private SerializedProperty sourceTextField;

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.UpdateIfRequiredOrScript();

            sourceTextField = serializedObject.FindProperty("source");

            GUIContent content = new GUIContent(sourceTextField.stringValue);

            GUIStyle style = GUI.skin.textField;
            Vector2 size = style.CalcSize(content);

            var str = sourceTextField.stringValue;
            sourceTextField.stringValue = EditorGUILayout.TextArea(str, style, GUILayout.Height(size.y));

            serializedObject.ApplyModifiedProperties();
            EditorGUI.EndChangeCheck();
        }
    }
}
#endif