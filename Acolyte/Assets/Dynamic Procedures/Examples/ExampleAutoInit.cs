using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Acolyte;


namespace TFM.DynamicProcedures.Examples
{
    public class ExampleAutoInit : MonoBehaviour
    {
        [SerializeField]
        private Environment environment;

        [SerializeField]
        private Procedure procedure;

        [Space(8)]
        [SerializeField]
        private UnityEvent OnProcedureReset;


        private ProcedureExecution execution;


        private void Start()
        {
            // Set the environment based on the entity children of the serialized object
            var allEntities = environment.gameObject.GetComponentsInChildren<Entity>();
            environment.SetEntities(allEntities);

            ExecuteProcedure(environment, procedure);

            Script.OnCompilation += HandleScriptCompilation;
        }

        private void OnDestroy()
        {
            Script.OnCompilation -= HandleScriptCompilation;
        }

        private void ExecuteProcedure(Environment e, Procedure p)
        {
            ExecutionSettings settings = new ExecutionSettings()
            {
                canAutoComplete = true,
                canStepBackwards = true,
            };

            execution = new ProcedureExecution(e, p, settings);
        }

        private void HandleScriptCompilation(Script script) 
        {
            if(execution != null && execution.RunningState != ExecutionRunningState.AwaitingStart)
            {
                execution.CancelExecution();

                ExecuteProcedure(environment, procedure);

                OnProcedureReset.Invoke();
            }
        }
    }
}