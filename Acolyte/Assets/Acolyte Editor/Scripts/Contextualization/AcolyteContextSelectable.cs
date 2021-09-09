using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace Acolyte.Editor
{
    public class AcolyteContextSelectable : MonoBehaviour
    {
        public event Action<string> OnSubmit;

        [SerializeField]
        private TMP_Text textField;

        [SerializeField]
        private Selectable selectable;


        public void SetOption(string option, bool interactable)
        {
            textField.text = option;

            if(selectable != null)
                selectable.interactable = interactable;

            if(!gameObject.activeSelf)
                gameObject.SetActive(true);
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