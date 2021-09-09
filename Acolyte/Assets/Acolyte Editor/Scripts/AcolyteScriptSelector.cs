using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Acolyte.Editor
{
    public class AcolyteScriptSelector : MonoBehaviour
    {
        public event Action<ScriptAsset> OnScriptSelected;

        public ScriptAsset ScriptAsset { get; private set; }

        [SerializeField]
        private AcolyteScriptSelectionOption optionPrefab;

        [SerializeField]
        private TMP_Text scriptNameField;

        private readonly List<AcolyteScriptSelectionOption> options = new List<AcolyteScriptSelectionOption>();


        public static string GetFullScriptLabel(ScriptAsset scriptAsset)
        {
            return scriptAsset.name + " (" + scriptAsset.Script.language.Name + ")";
        }

        public void SetAvailableScripts(ScriptAsset[] scriptAssets)
        {
            int i = 0;
            if(scriptAssets != null)
            {
                for(; i < scriptAssets.Length; i++)
                {
                    if(i >= options.Count)
                        CreateOption();

                    options[i].Set(scriptAssets[i]);
                }
            }

            // Pool
            for(; i < options.Count; i++)
            {
                if(options[i].gameObject.activeSelf)
                    options[i].gameObject.SetActive(false);
            }
        }

        private void Awake()
        {
            scriptNameField.text = "Acolyte Editor";

            optionPrefab.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            OnScriptSelected = null;
        }

        private void HandleSelection(ScriptAsset scriptAsset)
        {
            ScriptAsset = scriptAsset;
            OnScriptSelected?.Invoke(scriptAsset);

            scriptNameField.text = GetFullScriptLabel(scriptAsset);
        }

        private AcolyteScriptSelectionOption CreateOption() 
        {
            var option = Instantiate(optionPrefab, optionPrefab.transform.parent);
            option.OnSelected += (ScriptAsset asset) =>
            {
                HandleSelection(asset);
            };
            options.Add(option);
            return option;
        }
    }
}