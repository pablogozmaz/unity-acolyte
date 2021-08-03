using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


namespace TFM.DynamicProcedures.DebugUI
{
    public class EntityView : MonoBehaviour
    {
        private Entity entity;

        [SerializeField]
        private TextMeshProUGUI entityNameField;

        [SerializeField]
        private TextMeshProUGUI entityIdField;

        [Space(8)]
        [SerializeField]
        private PropertiesView propertiesView;


        public void SetEntity(Entity entity)
        {
            this.entity = entity;

            Activate(true);

            entityNameField.text = entity.name;
            entityIdField.text = entity.id;
        }

        public void Activate(bool doActivate)
        {
            if(gameObject.activeSelf != doActivate) gameObject.SetActive(doActivate);
        }

        public void ShowProperties()
        {
            propertiesView.ShowProperties(entity.Properties, entity.name);
        }
    }
}