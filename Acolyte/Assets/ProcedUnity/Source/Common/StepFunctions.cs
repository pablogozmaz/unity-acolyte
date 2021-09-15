using System;
using System.Collections;
using System.Collections.Generic;



namespace ProcedUnity
{
    public class StepFunctions : StepAction
    {
        public override string TypeLabel => "Functions";

        public override bool HasDefaultEntityList => false;

        public List<Action> functions = new List<Action>();
    }

    public class StepFunctionsExecution : StepActionExecution<StepFunctions>
    {
        protected override bool RegisterStartAndCompletion => false;


        protected override void InternalInitialization(Environment environment, StepFunctions action) {}

        protected override void Execute(ExecutionParameters parameters, Action OnCompletion)
        {
            foreach(var function in parameters.stepAction.functions)
            {
                function.Invoke();
            }

            OnCompletion.Invoke();
        }

        protected override void SetExecutionCompletedState(ExecutionParameters parameters)
        {
        }

        protected override void UndoExecution(ExecutionParameters parameters)
        {
        }
    }
}