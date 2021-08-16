using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures.Examples
{
    public class MachinePartInteractor : MonoBehaviour
    {
        public string PropertyName { get { return propertyName; } }

        [SerializeField]
        private Properties machineProperties;

        [SerializeField]
        private string propertyName;


        public void OnMouseDown()
        {
            if(!enabled) return;

            if(machineProperties.TryGet(propertyName, out PropertyBool property))
            {
                property.Value = !property.Value;
            }
            else
                Debug.Assert(false, "Property for machine part not found: "+ propertyName);
        }
    }
}