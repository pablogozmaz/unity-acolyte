using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures.Examples
{
    public class MyCustomPropertiesLoop : PropertiesBehaviourLoop<PropertyInt>
    {
        protected override float FrequencyInSeconds => 1f;


        protected override void ProcessBehaviour(IEnumerable<PropertyInt> properties)
        {
            foreach(var property in properties)
            {
                property.Value = Random.Range(50, 100);
            }
        }
    }

    // Example using default property interface
    public class MyCustomPropertiesLoop2 : PropertiesBehaviourLoop<IProperty>
    {
        protected override float FrequencyInSeconds => 1f;


        protected override void ProcessBehaviour(IEnumerable<IProperty> properties)
        {
            foreach(var property in properties)
            {
                Debug.Assert(property.Type == PropertyType.Int || property.Type == PropertyType.Float);

                property.SetObjectValue(Random.Range(50, 100));
            }
        }
    }
}