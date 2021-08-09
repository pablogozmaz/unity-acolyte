using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte;


namespace TFM.DynamicProcedures
{
    public partial class StepScript : Language
    {
        public static readonly StepScript instance = new StepScript();


        private StepScript() : base("StepScript")
        {
            isCaseSensitive = false;
            supportsInvalidWords = true;
        }

        public override Scope CreateScope()
        {
            return new StepScriptScope();
        }
    }
}