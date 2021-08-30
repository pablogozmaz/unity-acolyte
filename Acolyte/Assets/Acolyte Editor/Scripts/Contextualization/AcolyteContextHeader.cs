using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace Acolyte.Editor
{
    public class AcolyteContextHeader : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text textField;


        public void SetHeader(string value)
        {
            textField.text = value;

            if(!gameObject.activeSelf)
                gameObject.SetActive(true);
        }
    }
}