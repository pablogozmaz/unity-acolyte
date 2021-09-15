using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    public class Interactor : MonoBehaviour
    {
        public abstract class Strategy : ScriptableObject
        {
            public abstract void HandlePropertyValue(Interactor interactor, InteractionState value);
        }

        public string PropertyName => propertyName;

        public InteractionState Property { get; private set; }

        [HideInInspector]
        public Tool requiredTool;

        [SerializeField]
        private Properties targetProperties;

        [SerializeField]
        private string propertyName;

        [SerializeField]
        private Strategy strategy;


        private void Start()
        {
            if(targetProperties.TryGet(propertyName, out InteractionState property))
            {
                Property = property;
                ProcessPropertyValueChange(property);
                property.OnValueChanged += ProcessPropertyValueChange;
            }
            else
                Debug.Assert(false, "Missing property "+propertyName+" from "+targetProperties.name);
        }

        private void OnDestroy()
        {
            if(Property != null)
                Property.OnValueChanged -= ProcessPropertyValueChange;
        }

        private void ProcessPropertyValueChange(InteractionState property)
        {
            strategy.HandlePropertyValue(this, property);
        }

        private void OnMouseDown()
        {
            if(!enabled) return;

            var value = Property.Value;

            switch(value)
            {
                default:
                    return;

                case InteractionStates.Interactable:
                    Property.Value = requiredTool == Tool.Current ? InteractionStates.Interacted : InteractionStates.InteractedIncorrect;
                    break;
            }
        }
    }
}