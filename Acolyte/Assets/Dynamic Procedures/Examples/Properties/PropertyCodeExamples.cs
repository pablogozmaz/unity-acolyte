using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures.Examples
{
    public partial class CodeExamples
    {
        public static void PropertyCastingExample()
        {
            IProperty property = new PropertyInt("My Int", 2);

            // 1) Using type enum and direct casting
            if(property.Type == PropertyType.Int)
            {
                PropertyInt cast = (PropertyInt)property;
                Debug.Log(cast.Value > 2);
            }

            // 2) Using "is" keyword for casting
            if(property is PropertyInt otherCast)
            {
                Debug.Log(otherCast.Value > 4);
            }
        }

        public static void PropertyValueManipulationExample()
        {
            IProperty property = new PropertyInt("My Int", 2);

            // Using interface's abstract object methods
            property.SetObjectValue(3);
            bool check = property.SetObjectValue("hello"); // Will assert false and return false
            object value = property.GetObjectValue();

            // Using type cast
            if(property is PropertyInt propertyCast)
            {
                // Casted properties offer direct correct type access
                propertyCast.Value = 5;
                int intValue = propertyCast.Value;
            }
        }

        public static void PropertyAbstractComparisonExample()
        {
            IProperty property = new PropertyBool("My Bool", true);

            Debug.Log(property.ValueEquals(true));  // Is true
            Debug.Log(property.ValueEquals(false)); // Is false

            property = new PropertyString("My String", "Value");

            Debug.Log(property.ValueEquals("Value"));     // Is true
            Debug.Log(property.ValueEquals("Not my string.")); // Is false

            property = new PropertyInt("My Int", 3);

            Debug.Log(property.ValueEquals(3)); // Is true
            Debug.Log(property.ValueEquals(4)); // Is false
        }
    }
}