using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ProcedUnity
{
    public class ToolModeExpressions
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void AddExpressions()
        {
            StepScript.OnInstanceCreated += (StepScript stepScript) =>
            {
                stepScript.AddExpression("manual mode", () => { return ToolMode.Mode == ToolModes.manualMode; });
                stepScript.AddExpression("electric mode", () => { return ToolMode.Mode == ToolModes.electricMode; });
            };
        }
    }
}