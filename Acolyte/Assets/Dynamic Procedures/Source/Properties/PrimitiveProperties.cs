using System.Collections;
using System.Globalization;
using System.Collections.Generic;
using System;

namespace TFM.DynamicProcedures
{
    public sealed class PropertyBool : Property<bool>
    {
        public event Action<PropertyBool> OnValueChanged;

        public override PropertyType Type => PropertyType.Bool;

        public PropertyBool(string name, bool initialValue) : base(name, initialValue) { }

        public PropertyBool(string name, string valueAsText) : base(name, valueAsText) { }

        protected override void InvokeOnValueChangedDerived()
        {
            OnValueChanged?.Invoke(this);
        }

        protected override bool TryParseValueFromText(string valueAsText, out bool result)
        {
            return (bool.TryParse(valueAsText, out result));
        }
    }

    public sealed class PropertyFloat : Property<float>
    {
        public event Action<PropertyFloat> OnValueChanged;

        public override PropertyType Type => PropertyType.Float;

        public PropertyFloat(string name, float initialValue) : base(name, initialValue) { }

        public PropertyFloat(string name, string valueAsText) : base(name, valueAsText) { }

        protected override void InvokeOnValueChangedDerived()
        {
            OnValueChanged?.Invoke(this);
        }

        protected override bool TryParseValueFromText(string valueAsText, out float result)
        {
            return (float.TryParse(valueAsText, NumberStyles.Any, CultureInfo.InvariantCulture, out result));
        }
    }

    public sealed class PropertyInt : Property<int>
    {
        public event Action<PropertyInt> OnValueChanged;

        public override PropertyType Type => PropertyType.Int;

        public PropertyInt(string name, int initialValue) : base(name, initialValue) { }

        public PropertyInt(string name, string valueAsText) : base(name, valueAsText) { }

        protected override void InvokeOnValueChangedDerived()
        {
            OnValueChanged?.Invoke(this);
        }

        protected override bool TryParseValueFromText(string valueAsText, out int result)
        {
            return (int.TryParse(valueAsText, out result));
        }
    }

    public sealed class PropertyString : Property<string>
    {
        public event Action<PropertyString> OnValueChanged;

        public override PropertyType Type => PropertyType.String;

        public PropertyString(string name, string initialValue) : base(name, initialValue) { }

        protected override void InvokeOnValueChangedDerived()
        {
            OnValueChanged?.Invoke(this);
        }

        protected override bool TryParseValueFromText(string valueAsText, out string result)
        {
            result = valueAsText;
            return true;
        }
    }
}