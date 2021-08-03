using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures
{
    /// <summary>
    /// Abstract root class for behaviours that will perform a logic process upon properties.
    /// Subclass behaviours should use the generic implementation.
    /// </summary>
    public abstract class PropertiesBehaviour : MonoBehaviour
    {
        [SerializeField]
        protected Properties properties;

        [SerializeField]
        protected string[] propertyNames;

        public void Set(Properties properties, params string[] propertyNames)
        {
            Stop();

            this.properties = properties;
            this.propertyNames = propertyNames;

            if(enabled) Begin(properties, propertyNames);
        }

        protected virtual void OnEnable()
        {
            Stop();
            Begin(properties, propertyNames);
        }

        protected virtual void OnDisable()
        {
            Stop();
        }

        protected abstract void Stop();

        protected abstract void Begin();

        protected abstract void Begin(Properties properties, string[] propertyNames);

        protected virtual void OnValidate()
        {
            if(Application.isPlaying && enabled)
            {
                Stop();
                Begin(properties, propertyNames);
            }
        }
    }

    /// <summary>
    /// Abstract class for behaviours that will perform a logic process upon properties.
    /// </summary>
    public abstract class PropertiesBehaviour<T> : PropertiesBehaviour where T : IProperty
    {
        protected PropertiesTracker<T> propertiesTracker = new PropertiesTracker<T>();

        protected virtual void OnDestroy()
        {
            propertiesTracker.StopTracking();
            propertiesTracker = null;
        }

        protected sealed override void Begin(Properties properties, string[] propertyNames)
        {
            Debug.Assert(enabled);

            if(properties == null || propertyNames == null || propertyNames.Length == 0) return;

            propertiesTracker.StartTracking(properties, propertyNames);

            Begin();
        }
    }
}