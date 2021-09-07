using System.Collections;
using System.Collections.Generic;
using Acolyte;


namespace TFM.DynamicProcedures
{
    public class StepScript : Language
    {
        public override string Name => "StepScript";

        public StepScript()
        {
            IsCaseSensitive = false;

            AddStatement<IfElse>();

            AddExpression("profile is expert", IsProfileExpert);
        }

        // Define the desired scope
        public override Declexicon CreateDeclexicon()
        {
            return new StepScriptDeclexicon();
        }

        private bool IsProfileExpert() { return true; }
    }
}