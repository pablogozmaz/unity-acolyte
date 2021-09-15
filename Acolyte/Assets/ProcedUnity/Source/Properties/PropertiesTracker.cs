using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    /// <summary>
    /// Allows to keep track of a set of properties in a property holder.
    /// </summary>
    public sealed class PropertiesTracker : PropertiesTracker<IProperty> {}

    /// <summary>
    /// Allows to keep track of a set of properties of the specified type in a property holder.
    /// </summary>
    public class PropertiesTracker<T> where T : IProperty
    {
        public event Action<T> OnPropertyFound;
        public event Action<T> OnPropertyRemoved;
        public event Action<T> OnPropertyValueChanged;

        public IEnumerable<T> GetTrackedProperties { get { return trackedProperties; } }

        private Properties properties;

        private HashSet<string> namesToTrack = new HashSet<string>();

        private readonly HashSet<T> trackedProperties = new HashSet<T>();


        public void StartTracking(Properties properties, params string[] namesToTrack)
        {
            Debug.Assert(namesToTrack != null && namesToTrack.Length > 0, "Property tracking attempted with no names to track.");

            StopTracking();

            this.properties = properties;
            this.namesToTrack = new HashSet<string>(namesToTrack);

            foreach(var property in properties.GetAll)
            {
                TryAddProperty(property);
            }

            properties.OnPropertyAdded += TryAddProperty;
            properties.OnPropertyRemoved += TryRemoveProperty;
        }

        public void StopTracking()
        {
            trackedProperties.Clear();

            if(properties != null)
            {
                properties.OnPropertyAdded -= TryAddProperty;
                properties.OnPropertyRemoved -= TryRemoveProperty;
                properties = null;
            }
        }

        private void TryAddProperty(IProperty property)
        {
            if(namesToTrack.Contains(property.Name))
            {
                if(TryCastProperty(property, out T castedProperty))
                {
                    Debug.Assert(PropertyIsStillUntracked(castedProperty, out string failureMessage), failureMessage);

                    trackedProperties.Add(castedProperty);

                    property.OnIValueChanged += InvokeValueChanged;
                    OnPropertyFound?.Invoke(castedProperty);
                }
            }
        }

        private void TryRemoveProperty(IProperty property)
        {
            if(TryCastProperty(property, out T castedProperty))
            {
                if(trackedProperties.Remove(castedProperty))
                {
                    property.OnIValueChanged -= InvokeValueChanged;
                    OnPropertyRemoved?.Invoke(castedProperty);
                }
            }
        }

        private void InvokeValueChanged(IProperty property)
        {
            if(TryCastProperty(property, out T castedProperty))
            {
                OnPropertyValueChanged?.Invoke(castedProperty);
            }
        }

        private bool TryCastProperty(IProperty property, out T castedProperty)
        {
            try
            {
                castedProperty = (T)property;
                return true;
            }
            catch(Exception ex)
            {
                Debug.Assert(false, ex.Message+ "\n"+ex.StackTrace);
            }

            castedProperty = default;

            Debug.LogWarning(GetInvalidTypeWarning(property));
            return false;
        }

        private bool PropertyIsStillUntracked(T property, out string failureMessage) 
        {
            if(trackedProperties.Contains(property)) 
            {
                failureMessage = "Tracker attempted to add a property twice: " + property.Name;
                return false;
            }

            failureMessage = null;
            return true;
        }

        private string GetInvalidTypeWarning(IProperty property) 
        {
            return "Property tracker is tracking invalid type. Expected: " + typeof(T) + " Actual: " + property.GetType();
        }
    }
}