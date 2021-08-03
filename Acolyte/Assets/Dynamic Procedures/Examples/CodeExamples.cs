using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFM.DynamicProcedures.Examples
{
    /// <summary>
    /// Contains several methods to showcase different code examples.
    /// </summary>
    public partial class CodeExamples
    {
        public static Procedure ProcedureCreationExample()
        {
            Procedure procedure = ScriptableObject.CreateInstance<Procedure>();
            procedure.name = "My example procedure";

            Step step = new Step()
            {
                name = "My step"
            };

            StepActionExample stepAction = ScriptableObject.CreateInstance<StepActionExample>();
            stepAction.name = "My step action";
            stepAction.colorValue = Color.red;

            step.actions = new List<StepAction>
            {
                stepAction
            };

            procedure.steps.Add(step);

            return procedure;
        }
    }
}