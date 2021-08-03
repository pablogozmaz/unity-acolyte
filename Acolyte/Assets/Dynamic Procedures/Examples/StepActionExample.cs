using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte;


namespace TFM.DynamicProcedures.Examples
{
    [CreateAssetMenu(fileName = "Example Action", menuName = "Dynamic Procedures/" + "Example Step Action")]
    public class StepActionExample : StepAction
    {
        public override string TypeLabel => "My Testing Action";

        public override bool HasDefaultEntityList => true;

        public Color colorValue;
    }

    public class StepActionExampleExecution : StepActionExecution<StepActionExample>
    {
        private GameObject executionGameObject;

        private IEnumerator coroutine;


        protected override void InternalInitialization(Environment environment, StepActionExample action) {}

        protected override void Execute(ExecutionParameters parameters, Action OnCompletion)
        {
            Debug.Assert(executionGameObject == null);

            executionGameObject = new GameObject("Example test!");

            coroutine = ExecutionCoroutine(parameters.stepAction, OnCompletion);
            StaticCoroutineRunner.Start(coroutine);
        }

        protected override void UndoExecution(ExecutionParameters parameters)
        {
            if(executionGameObject != null)
                UnityEngine.Object.Destroy(executionGameObject);

            if(coroutine != null)
                StaticCoroutineRunner.Stop(coroutine);
        }

        protected override void SetExecutionCompletedState(ExecutionParameters parameters)
        {
            if(executionGameObject != null)
                UnityEngine.Object.Destroy(executionGameObject);

            if(coroutine != null)
                StaticCoroutineRunner.Stop(coroutine);
        }

        private IEnumerator ExecutionCoroutine(StepActionExample stepAction, Action OnCompletion)
        {
            Debug.Log("Hey, I have just been executed.");

            yield return new WaitForSecondsRealtime(2f);

            Debug.Log("Completed!");

            OnCompletion.Invoke();

            coroutine = null;
        }
    }
}