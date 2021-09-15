using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace ProcedUnity
{
    /// <summary>
    /// Encapsulates the execution of a Procedure step by step under a specific Environment.
    /// </summary>
    public sealed class ProcedureExecution : IExecution
    {
        public static event Action<ProcedureExecution> OnExecutionCreated;

        public static IEnumerable<ProcedureExecution> GetAllActiveExecutions { get { return activeExecutions; } }

        /// <summary>
        /// Invoked when the public state of the object is modified. Will turn null once the execution is completed or cancelled.
        /// </summary>
        public UnityEvent OnObjectStateChange { get; private set; } = new UnityEvent();

        public readonly Procedure procedure;
        public readonly Environment environment;
        public readonly ExecutionRegistry registry;
        public readonly ExecutionSettings settings;

        public string Label { get { return procedure.name; } }

        public ExecutionRunningState RunningState { get; private set; }

        public StepExecution CurrentStepExecution { get; private set; }

        public int CurrentStepIndex { get; private set; }
        public int StepAmount { get { return stepExecutions != null ? stepExecutions.Length : 0; } }

        private static readonly HashSet<ProcedureExecution> activeExecutions = new HashSet<ProcedureExecution>();

        private ExecutionRegistry.Recorder registryRecorder;

        private StepExecution[] stepExecutions;


        public ProcedureExecution(Environment environment, Procedure procedure, ExecutionSettings settings)
        {
            Debug.Assert(procedure != null);
            Debug.Assert(IsProcedureValidForExecution(procedure, out string failureReason), failureReason);

            this.procedure = procedure;
            this.environment = environment;
            this.settings = settings;
            registry = new ExecutionRegistry();
            registryRecorder = new ExecutionRegistry.Recorder(registry, this);

            RunningState = ExecutionRunningState.AwaitingStart;

            stepExecutions = new StepExecution[procedure.steps.Count];
            for(int i = 0; i < stepExecutions.Length; i++)
                stepExecutions[i] = new StepExecution(environment, procedure.steps[i], i, registry, settings);

            CurrentStepIndex = -1;

            OnExecutionCreated?.Invoke(this);

            activeExecutions.Add(this);
        }

        /// <summary>
        /// Attempt to continue to the next step.
        /// </summary>
        public bool TryStepForward()
        {
            switch(RunningState)
            {
                case ExecutionRunningState.AwaitingStart:
                    StartExecution();
                    return true;

                case ExecutionRunningState.InProcess:
                    return TryContinueToNextStep();

                default:
                    Debug.Assert(false, "Cannot step forward in procedure with the following running state: " + RunningState);
                    return false;
            }
        }

        /// <summary>
        /// Attemp to undo any execution on the current and previous steps, and execute the previous step again.
        /// </summary>
        public bool TryStepBack()
        {
            switch(RunningState)
            {
                case ExecutionRunningState.InProcess:
                    CurrentStepExecution.UndoExecution();
                    int index;

                    if(CurrentStepExecution.IsCompleted)
                    {
                        index = CurrentStepIndex;
                    }
                    else 
                    {
                        index = CurrentStepIndex - 1;
                        CurrentStepIndex = -1;
                    }

                    if(index < 0) index = 0;

                    stepExecutions[index].UndoExecution();
                    ExecuteStep(index);
                    return true;

                default:
                    Debug.Assert(false, "Cannot step back in procedure with the following running state: " + RunningState);
                    return false;
            }
        }

        public void CancelExecution() 
        {
            switch(RunningState)
            {
                case ExecutionRunningState.InProcess:
                case ExecutionRunningState.AwaitingStart:

                    for(int i = CurrentStepIndex; i >= 0; i--)
                    {
                        stepExecutions[i].UndoExecution();
                    }

                    RunningState = ExecutionRunningState.Cancelled;
                    registryRecorder.AddEntry(ExecutionRegistry.KeyProcedureCancelled);

                    NullifyExecution();
                    break;

                default:
                    Debug.Assert(false, "Attempted to cancel a Procedure with an invalid running state: " + RunningState);
                    break;
            }
        }

        private void StartExecution()
        {
            Debug.Assert(RunningState == ExecutionRunningState.AwaitingStart);

            RunningState = ExecutionRunningState.InProcess;
            registryRecorder.AddEntry(ExecutionRegistry.KeyProcedureStarted);
            ExecuteStep(0);
        }

        private bool TryContinueToNextStep() 
        {
            if(CurrentStepExecution.IsCompleted)
            {
                ExecuteStep(CurrentStepIndex + 1);
                return true;
            }
            else
            {
                Debug.LogWarning("Attempting to step forward while the current step in execution is not yet completed.");
                return false;
            }
        }

        private void ExecuteStep(int index) 
        {
            Debug.Assert(RunningState == ExecutionRunningState.InProcess);

            if(index == CurrentStepIndex)
            {
                Debug.LogWarning("Attempted to execute a step with the same index as the current step.");
                return;
            }

            CurrentStepIndex = index;
            CurrentStepExecution = stepExecutions[index];
            CurrentStepExecution.Execute(HandleStepExecutionCompletion);

            NotifyStateChange();
        }

        private void HandleStepExecutionCompletion() 
        {
            if(IsCurrentStepLastStep())
            {
                CompleteExecution();
            }
            else
                NotifyStateChange();
        }

        private void CompleteExecution() 
        {
            RunningState = ExecutionRunningState.Completed;

            registryRecorder.AddEntry(ExecutionRegistry.KeyProcedureCompleted);
            
            NullifyExecution();
        }
       
        /// <summary>
        /// Sets the current execution as invalid for executing any further, cleaning references.
        /// </summary>
        private void NullifyExecution() 
        {
            Debug.Assert(RunningState == ExecutionRunningState.Cancelled || RunningState == ExecutionRunningState.Completed);

            stepExecutions = null;
            registryRecorder = null;

            NotifyStateChange();

            OnObjectStateChange.RemoveAllListeners();
            OnObjectStateChange = null;

            activeExecutions.Remove(this);
        }

        private bool IsProcedureValidForExecution(Procedure procedure, out string failureReason) 
        {
            if(procedure.steps == null || procedure.steps.Count == 0)
            {
                failureReason = "An executed Procedure has no steps. At least one step is required.";
                return false;
            }

            failureReason = null;
            return true;
        }

        private bool IsCurrentStepLastStep() => CurrentStepExecution.indexInProcedureExecution == procedure.steps.Count - 1;
        
        private void NotifyStateChange() => OnObjectStateChange.Invoke();
    }
}