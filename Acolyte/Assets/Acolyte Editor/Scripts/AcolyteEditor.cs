using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Acolyte.Editor
{
    public class AcolyteEditor : MonoBehaviour
    {
        [SerializeField]
        private AcolyteScriptSelector selector;

        [SerializeField]
        private AcolyteScriptField scriptField;


        private void Start()
        {
            var scripts = Resources.LoadAll<ScriptAsset>("");

            selector.SetAvailableScripts(scripts);

            selector.OnScriptSelected += HandleScriptSelection;
        }

        private void OnDestroy()
        {
            selector.OnScriptSelected -= HandleScriptSelection;
        }

        private void HandleScriptSelection(ScriptAsset scriptAsset) 
        {
            scriptField.SetScript(scriptAsset);
        }
    }
}