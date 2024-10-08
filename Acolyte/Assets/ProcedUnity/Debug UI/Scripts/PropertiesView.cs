﻿using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace ProcedUnity.DebugUI
{
    public class PropertiesView : MonoBehaviour
    {
        private Properties properties;

        [SerializeField]
        private TextMeshProUGUI propertyHolderName;

        [SerializeField]
        private TextMeshProUGUI text;


        public void ShowProperties(Properties properties, string propertyHolderName = "<i>Nothing selected</i>")
        {
            if(this.properties != null)
            {
                this.properties.OnShallowChange -= HandleAnyPropertyChange;
            }

            this.properties = properties;

            if(!gameObject.activeSelf) gameObject.SetActive(true);

            this.propertyHolderName.text = propertyHolderName;

            SetPropertiesText(properties);

            if(properties != null)
            {
                properties.OnShallowChange += HandleAnyPropertyChange;
            }
        }

        private void SetPropertiesText(Properties properties) 
        {
            if(properties != null && properties.Count > 0)
            {
                StringBuilder stringBuilder = new StringBuilder();

                foreach(var property in properties.GetAll)
                {
                    if(!PropertyParser.TryGetPrefix(property.GetType(), out string prefix))
                    {
                        prefix = property.Type.ToString();
                    }

                    stringBuilder.Append("<color=orange>").Append(prefix).Append("</color>");
                    stringBuilder.Append(" <b>").Append(property.Name);
                    stringBuilder.Append("</b> = <color=#5986FA>").Append(property.GetObjectValue()).Append("</color>");
                    stringBuilder.Append("\n");
                }

                text.text = stringBuilder.ToString();
            }
            else
            {
                text.text = "No properties.";
            }
        }

        private void HandleAnyPropertyChange()
        {
            SetPropertiesText(properties);
        }

        private void Start()
        {
            ShowProperties(null);
        }
    }
}