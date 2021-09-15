using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    [CreateAssetMenu(fileName = "Interaction StepAction", menuName = "Dynamic Procedures/Interaction StepAction")]
    public class InteractionAction : StepAction
    {
        public override string TypeLabel => "Interaction";

        public override bool HasDefaultEntityList => false;

        public Interactor[] interactors;

        public Tool tool;
    }

    public class InteractionActionExecution : StepActionExecution<InteractionAction>
    {
        public const string RegistryKey = "State set";

        private static Event incorrectCompletionEvent = new Event("Interaction_Incorrect");

        private Interactor[] Interactors => parameters.stepAction.interactors;

        private HashSet<IProperty> pendingProperties;
        private Action completionCallback;
        private bool done;

        protected override void InternalInitialization(Environment environment, InteractionAction action) {}

        protected override void Execute(ExecutionParameters parameters, Action OnCompletion)
        {
            done = false;

            if(Interactors == null) return;

            pendingProperties = new HashSet<IProperty>();

            foreach(var interactor in Interactors)
            {
                // Set tool
                interactor.requiredTool = parameters.stepAction.tool;

                // Handle property
                var property = interactor.Property;
                pendingProperties.Add(property);
                property.Value = InteractionStates.Interactable;
                property.OnValueChanged += HandlePropertyChange;
            }

            completionCallback = OnCompletion;
        }

        protected override void SetExecutionCompletedState(ExecutionParameters parameters)
        {
            if(done || Interactors == null) return;

            foreach(var interactor in Interactors)
            {
                var property = interactor.Property;
                property.OnValueChanged -= HandlePropertyChange;
                property.Value = InteractionStates.Interacted;
            }

            pendingProperties.Clear();
        }

        protected override void UndoExecution(ExecutionParameters parameters)
        {
            if(Interactors == null) return;

            foreach(var interactor in Interactors)
            {
                var property = interactor.Property;
                property.OnValueChanged -= HandlePropertyChange;
                property.Value = InteractionStates.NonInteractable;
            }

            pendingProperties.Clear();
        }

        private void HandlePropertyChange(InteractionState property)
        {
            InteractionStates value = property.Value;
            parameters.registryRecorder.AddEntry(RegistryKey, property.Name + " = " + value);

            if((int)value > 1)
            {
                if(value == InteractionStates.InteractedIncorrect)
                {
                    incorrectCompletionEvent.Invoke();
                }

                bool removalCheck = pendingProperties.Remove(property);
                Debug.Assert(removalCheck); // If a property was changed to true it should always be present in the pending set

                // If no pending properties left, action is completed
                if(pendingProperties.Count == 0)
                {
                    done = true;
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
    }
}