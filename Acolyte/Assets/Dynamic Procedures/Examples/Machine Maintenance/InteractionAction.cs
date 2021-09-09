using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures.Examples
{
    [CreateAssetMenu(fileName = "Machine Operation Action", menuName = "Dynamic Procedures/Machine Operation Action")]
    public class InteractionAction : StepAction
    {
        public override string TypeLabel => "Machine Operation";

        public override bool HasDefaultEntityList => false;

        public string machineId;

        public string interactionControlProperty;
        
        public string[] partsProperties;
    }

    public class InteractionActionExecution : StepActionExecution<InteractionAction>
    {
        public const string RegistryTestKey = "MACHINEOP";

        private Entity machine;

        private HashSet<IProperty> pendingProperties;

        private Action completionCallback;


        protected override void InternalInitialization(Environment environment, InteractionAction action)
        {
            // Obtain our desired entity, since we dont want to rely on default entity list
            if(environment.TryGetEntity(action.machineId, out machine)) {}
            else Debug.Assert(false, "Machine Entity not found");
        }

        protected override void Execute(ExecutionParameters parameters, Action OnCompletion)
        {
            // 1) Save callback reference for later invoke
            completionCallback = OnCompletion;

            // 2) Assert that interaction property is correctly set at this point, to ensure procedure correctness
            if(machine.Properties.TryGet(parameters.stepAction.interactionControlProperty, out PropertyBool property))
            {
                // Assert property is correct at this point
                Debug.Assert(!property.Value, "Interaction control property should be false, but it is true");

                // After assertion, set it to true
                property.Value = true;
            }

            // 3) Find all part properties for the operation to perform assertions and start listening to value changes
            var properties = parameters.stepAction.partsProperties;
            pendingProperties = new HashSet<IProperty>(); // We use a hashset of properties to keep their track

            foreach(var propertyName in properties)
            {
                if(machine.Properties.TryGet(propertyName, out property))
                {
                    Debug.Assert(!property.Value, "All machine parts should be set to false at this point");

                    // Listen to value changes, and add property to pending
                    property.OnValueChanged += HandlePropertyChange;
                    pendingProperties.Add(property);
                }
                else Debug.Assert(false, "Missing property from machine: "+ propertyName);
            }
        }

        private void HandlePropertyChange(PropertyBool property)
        {
            bool isSet = property.Value;
            parameters.registryRecorder.AddEntry(RegistryTestKey, property.Name + " = "+ isSet);

            // Remove from pending if true
            if(isSet)
            {
                bool removalCheck = pendingProperties.Remove(property);
                Debug.Assert(removalCheck); // If a property was changed to true it should always be present in the pending set

                // If no pending properties left, action is completed
                if(pendingProperties.Count == 0)
                {
                    completionCallback.Invoke();
                }

            }
            // Add again to pending if property was set to false
            else
            {
                // If a property was changed to false it should never be in the pending set
                Debug.Assert(!pendingProperties.Contains(property));

                pendingProperties.Add(property);
            }
        }

        protected override void SetExecutionCompletedState(ExecutionParameters parameters)
        {
            // Put all properties in the required completion state, in case they are not
            // If completion was done by interacting, all properties will already be in the correct state
            // If we auto-complete, this process is necessary for procedure correctness
            var properties = parameters.stepAction.partsProperties;
            foreach(var partStateName in properties)
            {
                if(machine.Properties.TryGet(partStateName, out PropertyBool property))
                {
                    // VERY IMPORTANT TO UNSUBSCRIBE
                    property.OnValueChanged -= HandlePropertyChange;
                    property.Value = true;
                }
                else Debug.Assert(false, "Missing property from machine: " + partStateName);
            }

            // Ensure interaction control is removed when completed
            if(machine.Properties.TryGet(parameters.stepAction.interactionControlProperty, out PropertyBool interactionControl))
            {
                interactionControl.Value = false;
            }

            // Clean references
            pendingProperties = null;
            completionCallback = null;
        }

        protected override void UndoExecution(ExecutionParameters parameters)
        {
            // Ensure all properties are in the required initial state
            var properties = parameters.stepAction.partsProperties;
            foreach(var propertyName in properties)
            {
                if(machine.Properties.TryGet(propertyName, out PropertyBool property))
                {
                    // VERY IMPORTANT TO UNSUBSCRIBE
                    property.OnValueChanged -= HandlePropertyChange;
                    property.Value = false;
                }
                else Debug.Assert(false, "Missing property from machine: " + propertyName);
            }

            // Ensure interaction control is removed when completed
            if(machine.Properties.TryGet(parameters.stepAction.interactionControlProperty, out PropertyBool interactionControl))
            {
                interactionControl.Value = false;
            }

            // Clean references
            pendingProperties = null;
            completionCallback = null;
        }
    }
}