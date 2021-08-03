using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures.Examples
{
    /// <summary>
    /// Changes the color of machine parts based on their property's boolean value.
    /// </summary>
    public class MachinePartOperationBehaviour : PropertiesBehaviourValueObserver<PropertyBool>
    {
        [Serializable]
        private struct MachinePartSerializer
        {
            public string propertyName;
            public Renderer machinePartRenderer;
        }

        [SerializeField]
        private MachinePartSerializer[] machineParts = new MachinePartSerializer[0];

        [Space(8)]
        [SerializeField]
        private Color trueColor = Color.white;

        [SerializeField]
        private Color falseColor = Color.white;


        protected override void ProcessValueChange(PropertyBool property)
        {
            for(int i = 0; i < machineParts.Length; i++)
            {
                if(machineParts[i].propertyName == property.Name) 
                {
                    Debug.Assert(property.Type == PropertyType.Bool);

                    try
                    {
                        bool state = (bool)property.GetObjectValue();

                        machineParts[i].machinePartRenderer.material.color = state ? trueColor : falseColor;
                    }
                    catch(Exception ex)
                    {
                        Debug.Assert(false, ex.Message + "\n"+ ex.StackTrace);
                    }
                    break;
                }
            }
        }
    }
}