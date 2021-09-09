using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace TFM.DynamicProcedures.Examples
{
    public class EntityInteractor : Entity
    {
        private class Observer : PropertiesBehaviourValueObserver<PropertyBool>
        {
            public EntityInteractor owner;

            protected override void ProcessValueChange(PropertyBool property)
            {
                Debug.Assert(property.Type == PropertyType.Bool);

                owner.ProcessPropertyValueChange(property);
            }
        }

        // public string InteractionPropertyName { get}

        [SerializeField]
        private Properties targetProperties;

        [SerializeField]
        private string propertyName;

        [Space(8)]
        [SerializeField]
        private UnityEvent OnPropertyTrue;

        [SerializeField]
        private UnityEvent OnPropertyFalse;



        public void OnMouseDown()
        {
            if(!enabled) return;

            if(targetProperties.TryGet(propertyName, out PropertyBool property))
            {
                property.Value = !property.Value;
            }
            else
                Debug.Assert(false, "Property for machine part not found: " + propertyName);
        }

        private void Awake()
        {
            var observer = gameObject.AddComponent<Observer>();
            observer.owner = this;
        }

        private void ProcessPropertyValueChange(PropertyBool property)
        {
            try
            {
                bool state = (bool)property.GetObjectValue();

                if(state)
                    OnPropertyTrue.Invoke();
                else
                    OnPropertyFalse.Invoke();
            }
            catch(Exception ex)
            {
                Debug.Assert(false, ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}