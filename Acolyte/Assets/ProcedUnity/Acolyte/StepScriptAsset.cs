using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte;


namespace ProcedUnity
{
    [CreateAssetMenu(fileName = "StepScript", menuName = "StepScript Asset")]
    public sealed class StepScriptAsset : ScriptAsset<StepScript>
    {
        protected override bool AutoCompileOnInitialization => true;
    }
}