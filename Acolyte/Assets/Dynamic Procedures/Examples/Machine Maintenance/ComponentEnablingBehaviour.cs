using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures.Examples
{
    /// <summary>
    /// A property value observer that will set an array of monobehaviours' enabled state equal to a tracked bool property.
    /// </summary>
    public class ComponentEnablingBehaviour : PropertiesBehaviourValueObserver<PropertyBool>
    {
        [SerializeField]
        private MonoBehaviour[] components;


        protected override void ProcessValueChange(PropertyBool property)
        {
            for(int i = 0; i < components.Length; i++)
            {
                components[i].enabled = property.Value;
            }
        }
    }
}