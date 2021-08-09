using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TFM.DynamicProcedures.Examples
{
    public class MachineInteractionAction : StepAction
    {
        public override string TypeLabel => "Interaction";

        public override bool HasDefaultEntityList => false;
    }

    public class MachineInteractionActionExecution : StepActionExecution<MachineInteractionAction>
    {
        protected override void InternalInitialization(Environment environment, MachineInteractionAction action)
        {
            throw new NotImplementedException();
        }

        protected override void Execute(ExecutionParameters parameters, Action OnCompletion)
        {
            throw new NotImplementedException();
        }

        protected override void SetExecutionCompletedState(ExecutionParameters parameters)
        {
            throw new NotImplementedException();
        }

        protected override void UndoExecution(ExecutionParameters parameters)
        {
            throw new NotImplementedException();
        }
    }
}