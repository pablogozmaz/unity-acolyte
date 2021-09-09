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

            AddStatements(new IfElse("if", "else", "endif"));

            AddExpression("profile is expert", IsProfileExpert);
            AddExpression("booleanExample", ()=> { return true; });
            AddExpression("true", () => { return true; });
            AddExpression("false", () => { return false; });
        }

        // Define the desired scope
        public override Declexicon CreateDeclexicon()
        {
            return new StepScriptDeclexicon();
        }

        private bool IsProfileExpert() { return true; }
    }
}