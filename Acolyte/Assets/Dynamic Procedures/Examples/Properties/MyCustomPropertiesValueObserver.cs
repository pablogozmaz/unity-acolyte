using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures.Examples
{
    public class MyCustomPropertiesValueObserver : PropertiesBehaviourValueObserver<PropertyInt>
    {
        protected override void ProcessValueChange(PropertyInt property)
        {
            property.Value += 100;
        }
    }

    // Example using default interface and manual casting
    public class MyCustomPropertiesValueObserver2 : PropertiesBehaviourValueObserver<IProperty>
    {
        protected override void ProcessValueChange(IProperty property)
        {
            try
            {
                int currentValue = (int)property.GetObjectValue();
                property.SetObjectValue(currentValue + 100);
            }
            catch(Exception ex)
            {
                Debug.Assert(false, ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}