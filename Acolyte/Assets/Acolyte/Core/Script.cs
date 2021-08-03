using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public sealed partial class Script
    {
        public readonly string source;
        public readonly Language language;

        private Invocation[] executable;


        public Script(string text, Language language, bool compile = false)
        {
            source = text;
            this.language = language;

            if(compile)
                Compile();
        }

        public void Execute(ScriptActionContext actionContext)
        {
            if(executable != null)
                Compile();

            foreach(var invocation in executable)
                invocation.Invoke(actionContext);
        }

        private void Compile()
        {
            executable = Compiler.Compile(this);
        }
    }
}