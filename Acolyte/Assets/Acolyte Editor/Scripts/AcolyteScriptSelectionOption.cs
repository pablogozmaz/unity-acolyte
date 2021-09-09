using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Acolyte.Editor
{
    public class AcolyteScriptSelectionOption : MonoBehaviour
    {
        public event Action<ScriptAsset> OnSelected;

        private ScriptAsset scriptAsset;

        [SerializeField]
        private TMP_Text textField;


        public void Set(ScriptAsset scriptAsset)
        {
            if(!gameObject.activeSelf)
                gameObject.SetActive(true);

            this.scriptAsset = scriptAsset;
            textField.text = AcolyteScriptSelector.GetFullScriptLabel(scriptAsset);
        }
        
        public void Select()
        {
            if(scriptAsset != null)
                OnSelected.Invoke(scriptAsset);
        }

        private void OnDestroy()
        {
            OnSelected = null;
        }
    }
}