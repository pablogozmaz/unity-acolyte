using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Acolyte.Editor
{
    public class AcolyteContextSelectable : MonoBehaviour
    {
        public event Action<string> OnSubmit;

        [SerializeField]
        private TMP_Text textField;


        public void SetOption(string option) 
        {
            textField.text = option;
        }

        public void Submit()
        {
            OnSubmit.Invoke(textField.text);
        }

        private void OnDestroy()
        {
            OnSubmit = null;
        }
    }
}