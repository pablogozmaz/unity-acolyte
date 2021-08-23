using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte;


namespace TFM.DynamicProcedures
{
    [CreateAssetMenu(fileName = "StepScript", menuName = "StepScript Asset")]
    public sealed class StepScriptAsset : ScriptAsset<StepScript>
    {
        protected override bool autoCompileOnInitialization => true;
    }
}