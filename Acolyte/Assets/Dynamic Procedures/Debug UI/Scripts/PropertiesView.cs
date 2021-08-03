using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace TFM.DynamicProcedures.DebugUI
{
    public class PropertiesView : MonoBehaviour
    {
        private Properties properties;

        [SerializeField]
        private TextMeshProUGUI propertyHolderName;

        [SerializeField]
        private TextMeshProUGUI text;


        public void ShowProperties(Properties properties, string propertyHolderName = "<i>NONE</i>")
        {
            if(this.properties != null)
            {
                this.properties.OnShallowChange -= HandleAnyPropertyChange;
            }

            this.properties = properties;

            if(!gameObject.activeSelf) gameObject.SetActive(true);

            this.propertyHolderName.text = "<b>Properties:</b> " + propertyHolderName;

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
                    stringBuilder.Append("<color=orange>").Append(property.Type).Append("</color>");
                    stringBuilder.Append(" <b>").Append(property.Name);
                    stringBuilder.Append("</b> = <color=#5986FA>").Append(property.GetObjectValue()).Append("</color>");
                    stringBuilder.Append("\n");
                }

                text.text = stringBuilder.ToString();
            }
            else
            {
                text.text = "No properties";
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