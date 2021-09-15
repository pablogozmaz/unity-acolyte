using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    /// <summary>
    /// Component that wraps a collection of properties.
    /// </summary>
    public sealed class Properties : MonoBehaviour
    {
        public event Action OnDestruction;

        public event Action<IProperty> OnPropertyAdded;
        public event Action<IProperty> OnPropertyRemoved;

        /// <summary>
        /// Invoked when there is a change in the properties collection without notifying of details.
        /// </summary>
        public event Action OnShallowChange;

        public int Count { get { return properties.Count; } }

        public IEnumerable<IProperty> GetAll { get { return properties.Values; } }

        [SerializeField]
        private string propertyInitializationText;

        private readonly Dictionary<string, IProperty> properties = new Dictionary<string, IProperty>();


        public bool TryGet(string name, out IProperty property)
        {
            return properties.TryGetValue(name, out property);
        }

        public bool TryGet<T>(string name, out T property) where T : IProperty
        {
            if(properties.TryGetValue(name, out IProperty p))
            {
                try
                {
                    property = (T)p;
                    return true;
                }
                catch(Exception ex)
                {
                    Debug.Assert(false, "Failed property type cast for type:"+typeof(T) + "\n"+ex.StackTrace);
                }
            }

            property = default;
            return false;
        }

        public void Add(IProperty property) 
        {
            if(property == null)
            {
                Debug.Assert(false, "Attempted to add a null property to "+name);
                return;
            }

            Debug.Assert(!properties.ContainsKey(property.Name), "Cannot join properties with the same name.");

            property.OnIValueChanged += InvokeShallowChange;

            properties.Add(property.Name, property);

            OnShallowChange?.Invoke();
            OnPropertyAdded?.Invoke(property);
        }

        public bool RemoveProperty(string name)
        {
            if(properties.TryGetValue(name, out IProperty property))
            {
                property.OnIValueChanged -= InvokeShallowChange;
                properties.Remove(name);
                OnShallowChange?.Invoke();
                OnPropertyRemoved?.Invoke(property);
                return true;
            }

            return false;
        }

        private void Awake()
        {
            if(propertyInitializationText != null)
            {
                PropertyParser.ParseAndAppend(propertyInitializationText, this);
                propertyInitializationText = null; // Clean
            }
        }

        private void OnDestroy()
        {
            OnDestruction?.Invoke();
        }

        private void InvokeShallowChange(IProperty p) => OnShallowChange?.Invoke();
    }
}