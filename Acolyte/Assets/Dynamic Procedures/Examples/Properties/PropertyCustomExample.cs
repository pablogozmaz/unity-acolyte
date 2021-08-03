using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures.Examples
{
    [Serializable]
    public struct CustomPropertyData
    {
        public int a;
        public string b;
    }

    public class PropertyCustomExample : Property<CustomPropertyData>
    {
        public event Action<PropertyCustomExample> OnValueChanged;

        public override PropertyType Type => PropertyType.CustomStruct;

        public PropertyCustomExample(string name, CustomPropertyData initialValue) : base(name, initialValue) { }

        public PropertyCustomExample(string name, string valueAsText) : base(name, valueAsText) { }

        protected override void InvokeOnValueChangedDerived()
        {
            OnValueChanged?.Invoke(this);
        }

        protected override bool TryParseValueFromText(string valueAsText, out CustomPropertyData result)
        {
            // Specify how to parse from a string that is expected to be a parseable value
            throw new NotImplementedException();
        }

        // How to add a custom factory for parsing this example property
        private static void FactoryInitializationExample() 
        {
            PropertyParser.AddCustomFactory("example", (string name, string value) => 
            {
                return new PropertyCustomExample(name, value);
            });
        }
    }
}