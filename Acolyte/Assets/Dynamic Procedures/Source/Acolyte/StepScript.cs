using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Acolyte;


namespace TFM.DynamicProcedures
{
    public class StepScript : Language
    {
        public override string Name => "StepScript";

        public StepScript()
        {
            IsCaseSensitive = false;

            AddCommandFactory(IfElse.Factory);

            AddExpression("profile is expert", IsProfileExpert);
        }

        // Define the desired scope
        public override Lexicon CreateLexicon()
        {
            return new StepScriptLexicon();
        }

        private bool IsProfileExpert() { return true; }
    }
}