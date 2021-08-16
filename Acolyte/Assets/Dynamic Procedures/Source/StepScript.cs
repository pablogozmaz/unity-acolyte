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
            IsCaseSensitive = false;
            SupportsInvalidWords = true;
        }

        // Define the desired scope
        public override Lexicon CreateLexicon()
        {
            return new StepScriptLexicon();
        }
    }
}