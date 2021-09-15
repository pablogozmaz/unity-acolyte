using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace ProcedUnity
{
    public class Alert : MonoBehaviour
    {
        public static Alert Instance => instance;

        private static Alert instance;

        [SerializeField]
        private GameObject root;

        [SerializeField]
        private TMP_Text textField;


        public static void ShowAlert(string alert)
        {
            if(instance == null)
            {
                Debug.LogWarning("There is no enabled Alert object on scene.");
                return;
            }

            instance.textField.text = alert.Replace("_", " ");

            if(!instance.root.activeSelf)
                instance.root.SetActive(true);
        }

        private void Awake()
        {
            root.SetActive(false);
        }

        private void OnEnable()
        {
            instance = this;
        }

        private void OnDisable()
        {
            if(instance == this)
            {
                instance = null;
            }
        }
    }
}