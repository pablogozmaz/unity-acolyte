using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    /// <summary>
    /// Encapsulates the execution of a step action. Only the generic StepActionExecution should derive from this class.
    /// </summary>
    public abstract class StepActionExecution : IExecution
    {
        public struct InitializationParameters 
        {
            public Environment       environment;
            public StepAction        stepAction;
            public ExecutionRegistry registry;
            public ExecutionSettings settings;
        }

        public abstract StepAction StepAction { get; }

        public abstract Type StepActionType { get; }

        public string Label { get { return StepAction.TypeLabel; } }

        public abstract bool IsCompleted { get; }

        public abstract void Execute(Action<StepActionExecution> OnCompletion);

        public abstract bool AutoComplete();
        public abstract bool UndoExecution();

        protected abstract void Initialize(InitializationParameters parameters);


        // Force factory use
        protected StepActionExecution() {}

        /// <summary>
        /// Factory method for creating executions based on their implemented Step Action type.
        /// </summary>
        public static StepActionExecution Create(Type stepActionType, InitializationParameters initializationParameters)
        {
            if(StepActionExecutionTypeBinder.TryGetExecutionType(stepActionType, out Type executionType))
            {
                var execution = (StepActionExecution)Activator.CreateInstance(executionType);
                execution.Initialize(initializationParameters);
                return execution;
            }
            else
            {
                Debug.Assert(false, "Could not find execution type in binding for step action type: " + stepActionType);
                return null;
            }
        }
    }

    /// <summary>
    /// Encapsulates the execution of a step action. An implementation for each existing StepAction type is expected.
    /// </summary>
    public abstract class StepActionExecution<T> : StepActionExecution where T : StepAction
    {
        protected struct ExecutionParameters
        {
            public T stepAction;
            public Environment environment;
            public Entity[] entities;
            public ExecutionRegistry.Recorder registryRecorder;
            public ExecutionSettings settings;
        }

        public sealed override StepAction StepAction => parameters.stepAction;
        public sealed override Type StepActionType => typeof(T);

        public sealed override bool IsCompleted => isCompleted;

        protected virtual bool RegisterStartAndCompletion => true;

        protected ExecutionParameters parameters;

        private bool isCompleted;
        private bool isBeingExecuted;

        private Action completionCallback;


        public sealed override void Execute(Action<StepActionExecution> OnCompletion) 
        {
            Debug.Assert(OnCompletion != null);

            isBeingExecuted = true;

            if(RegisterStartAndCompletion)
                parameters.registryRecorder.AddEntry(ExecutionRegistry.KeyActionStarted);

            completionCallback = () =>
            {
                if(RegisterStartAndCompletion)
                    parameters.registryRecorder.AddEntry(ExecutionRegistry.KeyActionCompleted);
                
                SetExecutionCompletedState();
                OnCompletion.Invoke(this);
            };

            Execute(parameters, completionCallback);
        }

        public sealed override bool AutoComplete() 
        {
            Debug.Assert(parameters.settings.canAutoComplete);

            if(isBeingExecuted)
            {
                Debug.Assert(completionCallback != null);

                completionCallback.Invoke(); // Callback will automatically call all relevant completion methods
                return true;
            }

            return false;
        }

        public sealed override bool UndoExecution() 
        {
            if(isBeingExecuted || IsCompleted)
            {
                UndoExecution(parameters);
                completionCallback = null;
                return true;
            }

            return false;
        }

        protected sealed override void Initialize(InitializationParameters initializationParameters)
        {
            Debug.Assert(parameters.stepAction == null, "A step action execution should never be initialized more than once.");

            var stepAction = initializationParameters.stepAction;

            if(stepAction is T castedAction)
            {
                parameters = new ExecutionParameters()
                {
                    stepAction = castedAction,
                    environment = initializationParameters.environment,
                    registryRecorder = new ExecutionRegistry.Recorder(initializationParameters.registry, this),
                    settings = initializationParameters.settings
                };

                if(StepActionRequiresEntities(castedAction))
                {
                    parameters.entities = RetrieveEntities(parameters.environment, castedAction, castedAction.entities);
                }

                InternalInitialization(parameters.environment, castedAction);
            }
            else // This should NEVER happen, earlier assertions should catch
                Debug.Assert(false, "Invalid Step type: " + stepAction.GetType());
        }

        private void SetExecutionCompletedState()
        {
            isBeingExecuted = false;
            isCompleted = true;
            SetExecutionCompletedState(parameters);
            completionCallback = null;
        }

        /// <summary>
        /// Called when initializing the execution, before any object's execution starts.
        /// </summary>
        protected abstract void InternalInitialization(Environment environment, T action);

        /// <summary>
        /// Specifies how to execute the action. Requires invoking the given action for completion callback.
        /// </summary>
        protected abstract void Execute(ExecutionParameters parameters, Action OnCompletion);

        /// <summary>
        /// Undoes any logic performed by the execution, setting all affected data to their expected state before execution.
        /// </summary>
        protected abstract void UndoExecution(ExecutionParameters parameters);

        /// <summary>
        /// Specifies how the data should be upon execution completion of this action.
        /// </summary>
        protected abstract void SetExecutionCompletedState(ExecutionParameters parameters);

        
        private Entity[] RetrieveEntities(Environment environment, StepAction stepAction, List<string> entityIdList)
        {
            Entity[] entities = new Entity[entityIdList.Count];

            for(int i = 0; i < entityIdList.Count; i++)
            {
                if(environment.TryGetEntity(entityIdList[i], out Entity e))
                {
                    entities[i] = e;
                }
                else
                {
                    Debug.Assert(false, "Missing entity from Environment: " + stepAction.entities[i]);
                }
            }

            return entities;
        }

        private bool StepActionRequiresEntities(StepAction stepAction) 
        {
            return stepAction.HasDefaultEntityList && stepAction.entities != null && stepAction.entities.Count > 0;
        }
    }
}