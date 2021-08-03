using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures
{
    /// <summary>
    /// Encapsulates the execution of a step and all its actions.
    /// </summary>
    public sealed class StepExecution : IExecution
    {
        /// <summary>
        /// Invoked when the public state of the object is modified.
        /// </summary>
        public event Action OnObjectStateChange;

        public readonly Step step;

        public readonly int indexInProcedureExecution;

        public string Label { get { return step.name; } }

        public bool IsCompleted { get { return RunningState == ExecutionRunningState.Completed; } }

        public ExecutionRunningState RunningState { get; private set; } = ExecutionRunningState.AwaitingStart;

        private readonly StepActionExecution[] actionExecutions;
        private readonly ExecutionRegistry.Recorder registryRecorder;
        private readonly ExecutionSettings settings;

        private HashSet<StepActionExecution> pendingActionExecutions;

        private Action executionCompletionCallback;


        public StepExecution(Environment environment, Step step, int index, ExecutionRegistry registry, ExecutionSettings settings) 
        {
            this.step = step;
            registryRecorder = new ExecutionRegistry.Recorder(registry, this);
            this.settings = settings;

            indexInProcedureExecution = index;

            RunningState = ExecutionRunningState.AwaitingStart;

            actionExecutions = new StepActionExecution[step.actions.Count];
            InitializeActionexecutions(actionExecutions, environment, registry, settings);
        }

        public void Execute(Action OnStepCompleted)
        {
            Debug.Assert(pendingActionExecutions == null || pendingActionExecutions.Count == 0, "Executing step with pending actions.");

            RunningState = ExecutionRunningState.InProcess;

            pendingActionExecutions = new HashSet<StepActionExecution>(actionExecutions);

            executionCompletionCallback = OnStepCompleted;

            registryRecorder.AddEntry(ExecutionRegistry.KeyStepStarted);

            for(int i = 0; i < actionExecutions.Length; i++)
            {
                actionExecutions[i].Execute(HandleActionExecutionCompletion);
            }

            NotifyStateChange();
        }

        public bool AutoComplete() 
        {
            if(RunningState != ExecutionRunningState.InProcess) return false;

            if(!settings.canAutoComplete)
            {
                Debug.Assert(false, "Current execution settings do not allow auto-completion.");
                return false;
            }

            registryRecorder.AddEntry(ExecutionRegistry.KeyStepAutoComplete);

            // Copy to a safe array since the pending actions collection will be modified each iteration
            StepActionExecution[] safeArray = new StepActionExecution[pendingActionExecutions.Count];
            pendingActionExecutions.CopyTo(safeArray);
            foreach(var pendingActionexecution in safeArray)
            {
                pendingActionexecution.AutoComplete();
            }
            return true;
        }

        public void UndoExecution() 
        {
            if(RunningState == ExecutionRunningState.InProcess || RunningState == ExecutionRunningState.Completed)
            {
                for(int i = 0; i < actionExecutions.Length; i++)
                {
                    actionExecutions[i].UndoExecution();
                    RunningState = ExecutionRunningState.AwaitingStart;
                }

                registryRecorder.AddEntry(ExecutionRegistry.KeyStepUndo);

                pendingActionExecutions = null;
            }
        }

        private void InitializeActionexecutions(StepActionExecution[] actionExecutions, 
                                                Environment           environment,
                                                ExecutionRegistry     registry,
                                                ExecutionSettings     settings)
        {
            Debug.Assert(actionExecutions != null);
            Debug.Assert(actionExecutions.Length == step.actions.Count);

            for(int i = 0; i < step.actions.Count; i++)
            {
                StepActionExecution.InitializationParameters parameters = new StepActionExecution.InitializationParameters()
                {
                    environment = environment,
                    stepAction  = step.actions[i],
                    registry    = registry,
                    settings    = settings
                };

                actionExecutions[i] = StepActionExecution.Create(step.actions[i].GetType(), parameters);
            }
        }

        private void HandleActionExecutionCompletion(StepActionExecution execution) 
        {
            Debug.Assert(RunningState == ExecutionRunningState.InProcess);

            bool removalFlag = pendingActionExecutions.Remove(execution);

            Debug.Assert(removalFlag, "A StepActionExecution invoked completion but it could not be removed from pending executions.");

            if(pendingActionExecutions.Count == 0)
            {
                RunningState = ExecutionRunningState.Completed;

                registryRecorder.AddEntry(ExecutionRegistry.KeyStepCompleted);
                executionCompletionCallback.Invoke();
            }

            NotifyStateChange();
        }

        private void NotifyStateChange() =>  OnObjectStateChange?.Invoke();
    }
}