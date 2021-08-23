using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using TMPro;


namespace Acolyte.Editor
{
    /// <summary>
    /// Editing field for script source code.
    /// </summary>
    [ExecuteInEditMode]
    public class AcolyteScriptField : MonoBehaviour
    {
        private const string newLine = "\n";

        private ScriptAsset script;

        [SerializeField]
        private TMP_Text countField;

        [SerializeField]
        private TMP_InputField textField;


        public void SetScript(ScriptAsset script)
        {
            this.script = script;
            RenderScript(script.Script.Source);
        }

        public void SetFontSize(float fontSize)
        {
            textField.pointSize = fontSize;
            countField.fontSize = fontSize;
        }

        public void SetFontMaterial(Material material)
        {
            textField.textComponent.fontMaterial = material;
            countField.fontMaterial = material;
        }

        private void Awake()
        {
#if UNITY_EDITOR
            if(!Application.isPlaying) return;
#endif

            countField.text = "";
            textField.text = "";
        }

        private void RenderScript(string source)
        {
            StringBuilder countBuilder = new StringBuilder();
            StringBuilder sourceBuilder = new StringBuilder();

            using(StringReader reader = new StringReader(source))
            {
                string line;

                int count = 0;
                while((line = reader.ReadLine()) != null)
                {
                    countBuilder.Append(++count).Append(newLine);

                    sourceBuilder.Append(line).Append(newLine);
                }
            }

            countField.text = countBuilder.ToString();
            textField.text = sourceBuilder.ToString();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if(!Application.isPlaying)
            {
                SetFontSize(textField.pointSize);
                SetFontMaterial(textField.textComponent.fontMaterial);
            }
        }
#endif
    }
}