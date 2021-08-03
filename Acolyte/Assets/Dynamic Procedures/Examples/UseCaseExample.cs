using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures.Examples
{
    public class UseCaseExample : MonoBehaviour
    {
        [SerializeField]
        private Environment environment;

        [SerializeField]
        private Procedure procedure;


        private void Start()
        {
            // Set the environment based on the entity children of the serialized object
            var allEntities = environment.gameObject.GetComponentsInChildren<Entity>();
            environment.SetEntities(allEntities);

            ExecuteProcedure(environment, procedure);
        }

        private void ExecuteProcedure(Environment e, Procedure p)
        {
            ExecutionSettings settings = new ExecutionSettings()
            {
                canAutoComplete = true,
                canStepBackwards = true,
            };

            new ProcedureExecution(e, p, settings);
        }
    }
}