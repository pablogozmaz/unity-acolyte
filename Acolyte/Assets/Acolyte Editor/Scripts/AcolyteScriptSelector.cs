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

        public ScriptAsset Script { get; private set; }

        private readonly Dictionary<string, ScriptAsset> dictionary = new Dictionary<string, ScriptAsset>();

        [SerializeField]
        private TMP_Dropdown dropdown;



        public void SetAvailableScripts(ScriptAsset[] scripts)
        {
            dropdown.ClearOptions();
            dictionary.Clear();

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>(scripts.Length);

            foreach(var script in scripts)
            {
                string key = script.name;
                options.Add(new TMP_Dropdown.OptionData(key));
                dictionary.Add(key, script);
            }

            dropdown.AddOptions(options);
        }

        private void Awake()
        {
            dropdown.onValueChanged.AddListener(HandleSelection);
        }

        private void OnDestroy()
        {
            OnScriptSelected = null;
        }

        private void HandleSelection(int index)
        {
            var option = dropdown.options[index];

            if(dictionary.TryGetValue(option.text, out ScriptAsset script))
            {
                Script = script;
                OnScriptSelected?.Invoke(script);
            }
            else
                throw new Exception("Dropdown has incorrect selection value: "+option.text);
        }
    }
}