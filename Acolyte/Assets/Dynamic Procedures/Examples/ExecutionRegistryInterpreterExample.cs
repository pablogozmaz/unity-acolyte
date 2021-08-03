using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


namespace TFM.DynamicProcedures.Examples
{
    public class ExecutionRegistryInterpreterExample : ExecutionRegistryInterpreter
    {
        private static ExecutionRegistryInterpreterExample instance;

        private readonly StringBuilder executionHistory = new StringBuilder();


        public static void DebugHistory() 
        {
            Debug.Log(instance.executionHistory.ToString());
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnSceneLoad() 
        {
            instance = new ExecutionRegistryInterpreterExample();
            instance.Initialize();
        }

        private void Initialize()
        {
            SetupSolvers();

            ExecutionRegistry.OnRegistryCreated += BeginListeningToRegistry;
        }

        private void BeginListeningToRegistry(ExecutionRegistry registry)
        {
            registry.OnUpdated.AddListener(AppendEntryToHistory);
            // Not necessary to stop listening at any point, listeners are automatically removed when registry is closed
        }

        private void AppendEntryToHistory(ExecutionRegistry.Entry entry)
        {
            // In this example, always append datetime for each entry
            executionHistory.Append(entry.dateTime.ToLongTimeString()).Append(" - ");

            // Then solve
            // Solvers are executed inside this instance, so they can access variables and append to execution history themselves
            bool solveSuccess = TrySolve(entry);
            
            if(!solveSuccess)
            {
                // If we want to notify in the history that a solver key is missing
                executionHistory.Append("COULD_NOT_SOLVE_FOR_KEY: " + entry.key);
            }

            // Prepare history for the next entry
            executionHistory.Append("\n");

            // Debug.Log(executionHistory.ToString());
        }

        // Setup all solvers for the per-entry solve logic we want in this interpreter
        // In this case, we will append certain strings to our execution history for each key based on execution data
        private void SetupSolvers()
        {
            AddSolver(ExecutionRegistry.KeyProcedureStarted, (ProcedureExecution execution, ExecutionRegistry.Entry entry) =>
            {
                executionHistory.Append("El usuario ha comenzado el Procedimiento " + execution.procedure.name);
                return true;
            });

            AddSolver(ExecutionRegistry.KeyProcedureCompleted, (ProcedureExecution execution, ExecutionRegistry.Entry entry) =>
            {
                executionHistory.Append("El usuario ha completado el Procedimiento " + execution.procedure.name);
                return true;
            });

            AddSolver(ExecutionRegistry.KeyProcedureCancelled, (ProcedureExecution execution, ExecutionRegistry.Entry entry) =>
            {
                executionHistory.Append("El usuario ha cancelado el Procedimiento " + execution.procedure.name);
                return true;
            });

            AddSolver(ExecutionRegistry.KeyStepStarted, (StepExecution execution, ExecutionRegistry.Entry entry) =>
            {
                executionHistory.Append("El usuario ha comenzado el paso " + execution.step.name);
                return true;
            });

            AddSolver(ExecutionRegistry.KeyStepCompleted, (StepExecution execution, ExecutionRegistry.Entry entry) =>
            {
                executionHistory.Append("El usuario ha completado el paso " + execution.step.name);
                return true;
            });

            AddSolver(ExecutionRegistry.KeyStepAutoComplete, (StepExecution execution, ExecutionRegistry.Entry entry) =>
            {
                executionHistory.Append("--AUTO-COMPLETADO--");
                return true;
            });

            // Example of custom solver for a custom action
            AddSolver(MachineOperationActionExecution.RegistryTestKey,
                (MachineOperationActionExecution execution, ExecutionRegistry.Entry entry) =>
            {
                executionHistory.Append("El usuario ha cambiado el estado de la máquina: " + entry.content);
                return true;
            });
        }
    }
}