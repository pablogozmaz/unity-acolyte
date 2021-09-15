using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace ProcedUnity
{
    public class AutoInitialization : MonoBehaviour
    {
        [SerializeField]
        private Environment environment;

        [SerializeField]
        private Procedure procedure;

        [Space(8)]
        [SerializeField]
        private UnityEvent OnRecompilation;

        private ProcedureExecution execution;


        private void Start()
        {
            StepScriptDeclexicon.environment = environment;

            // Set the environment based on the entity children of the serialized object
            var allEntities = environment.gameObject.GetComponentsInChildren<Entity>();
            environment.SetEntities(allEntities);

            foreach(var step in procedure.steps)
            {
                // Ensure compilation
                var script = step.Script;
                if(script.IsCompiled)
                    script.Compile();
            }

            Acolyte.Script.OnCompilation += HandleScriptRecompilation;

            ExecuteProcedure();
        }

        private void OnDestroy()
        {
            Acolyte.Script.OnCompilation -= HandleScriptRecompilation;
        }

        private void ExecuteProcedure()
        {
            ExecutionSettings settings = new ExecutionSettings()
            {
                canAutoComplete = true,
                canStepBackwards = true,
            };

            execution = new ProcedureExecution(environment, procedure, settings);
        }

        private void HandleScriptRecompilation(Acolyte.Script script) 
        {
            if(execution != null)
            {
                execution.CancelExecution();

                OnRecompilation.Invoke();

                ExecuteProcedure();
            }
        }
    }
}