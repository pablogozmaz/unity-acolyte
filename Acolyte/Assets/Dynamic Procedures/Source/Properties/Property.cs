using System;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures
{
    public enum PropertyType 
    {
        Bool,
        Float,
        Int,
        String,
        CustomStruct
    }

    /// <summary>
    /// Encapsulation of a strongly typed named value.
    /// </summary>
    public interface IProperty
    {
        event Action<IProperty> OnIValueChanged;

        PropertyType Type { get; }

        string Name { get; }

        object GetObjectValue();

        bool SetObjectValue(object value);

        bool ValueEquals(object value);
    }

    /// <summary>
    /// Encapsulation of a strongly typed named value.
    /// <para>
    /// The T parameter MUST be a primitive or struct
    /// (accessing the value as reference wich allows internal modification will likely lead to bugs and lack of correctness).
    /// </para>
    /// </summary>
    public abstract class Property<T> : IProperty
    {
        public event Action<IProperty> OnIValueChanged;

        public abstract PropertyType Type { get; }

        public string Name { get; protected set; }

        public T Value 
        {
            get { return value; }
            set
            {
                if(Equals(this.value, value)) return;

                this.value = value;
                InvokeOnValueChanged();
            }
        }

        private T value;


        public Property(string name, T initialValue) 
        {
            Name = name;
            value = initialValue;
        }

        public Property(string name, string valueAsText)
        {
            Name = name;
            if(TryParseValueFromText(valueAsText, out T result))
            {
                value = result;
            }
            else
            {
                value = default;
                Debug.Assert(false, "Could not parse Property value from text: "+valueAsText+" ("+ name+")");
            }
        }

        public object GetObjectValue() => value;

        public bool SetObjectValue(object value)
        {
            if(value is T newValue)
            {
                Value = newValue;
                return true;
            }
            Debug.Assert(false, "Property value set from object is incorrect type: "+value.GetType()+" expected: "+typeof(T));
            return false;
        }

        public bool ValueEquals(object value)
        {
            Debug.Assert(value is T, "Invalid Type in Property comparison: " + typeof(T)+" vs "+value.GetType());

            return Equals(this.value, value);
        }

        protected abstract void InvokeOnValueChangedDerived();

        protected abstract bool TryParseValueFromText(string valueAsText, out T result);

        protected void InvokeOnValueChanged()
        {
            OnIValueChanged?.Invoke(this);
            InvokeOnValueChangedDerived();
        }
    }
}