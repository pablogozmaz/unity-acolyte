using System.Collections;
using System.Collections.Generic;
using Acolyte;


namespace ProcedUnity
{
    public class StepScript : Language
    {
        public static event System.Action<StepScript> OnInstanceCreated;

        public override string Name => "StepScript";

        public StepScript()
        {
            IsCaseSensitive = false;

            AddStatements(new IfElse("if", "else", "endif"));

            AddExpression("true", () => { return true; });
            AddExpression("false", () => { return false; });
            AddExpression("profile is expert", IsProfileExpert);

            OnInstanceCreated?.Invoke(this);
        }

        // Define the desired scope
        public override Declexicon CreateDeclexicon()
        {
            return new StepScriptDeclexicon();
        }

        private bool IsProfileExpert() { return true; }
    }
}