using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
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

        private readonly Environment environment;
        private readonly ExecutionRegistry registry;
        private readonly ExecutionRegistry.Recorder registryRecorder;
        private readonly ExecutionSettings settings;

        private StepActionExecution[] actionExecutions;
        private HashSet<StepActionExecution> pendingActionExecutions;

        private Action executionCompletionCallback;


        public StepExecution(Environment environment, Step step, int index, ExecutionRegistry registry, ExecutionSettings settings) 
        {
            this.step = step;
            registryRecorder = new ExecutionRegistry.Recorder(registry, this);
            this.environment = environment;
            this.registry = registry;
            this.settings = settings;

            indexInProcedureExecution = index;

            RunningState = ExecutionRunningState.AwaitingStart;

            // InitializeActionExecutions(step.actions.ToArray());

            // InitializeActionExecutions(actionExecutions, environment, registry, settings);

            // Use script to create and initialize required actions
            
        }

        public void Execute(Action OnStepCompleted)
        {
            Debug.Assert(pendingActionExecutions == null || pendingActionExecutions.Count == 0, "Executing step with pending actions.");

            RunningState = ExecutionRunningState.InProcess;

            pendingActionExecutions = new HashSet<StepActionExecution>();

            executionCompletionCallback = OnStepCompleted;

            registryRecorder.AddEntry(ExecutionRegistry.KeyStepStarted);

            step.Script.Execute((StepAction[] stepActions) =>
            {
                InitializeActionExecutions(stepActions);

                for(int i = 0; i < actionExecutions.Length; i++)
                {
                    pendingActionExecutions.Add(actionExecutions[i]);
                    actionExecutions[i].Execute(HandleActionExecutionCompletion);
                }
            });

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

        private void InitializeActionExecutions(StepAction[] stepActions)
        {
            actionExecutions = new StepActionExecution[stepActions.Length];

            for(int i = 0; i < stepActions.Length; i++)
            {
                StepActionExecution.InitializationParameters parameters = new StepActionExecution.InitializationParameters()
                {
                    environment = environment,
                    stepAction  = stepActions[i],
                    registry    = registry,
                    settings    = settings
                };

                actionExecutions[i] = StepActionExecution.Create(stepActions[i].GetType(), parameters);
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