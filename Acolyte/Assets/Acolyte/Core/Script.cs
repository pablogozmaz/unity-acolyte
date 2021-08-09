using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    /// <summary>
    /// Encapsulates a script for runtime reference and execution.
    /// </summary>
    public sealed partial class Script
    {
        public readonly string source;
        public readonly Language language;

        private readonly Scope scope;

        private Invocation[] executable;


        public Script(string text, Language language, bool compileOnCreation = false)
        {
            source = text;
            this.language = language;

            // Each script contains its own scope
            scope = language.CreateScope();

            if(compileOnCreation)
                Compile();
        }

        public void Compile() 
        {
            executable = Compiler.Compile(this);
        }

        public void Execute(params IExecutionSubcontext[] subcontexts) => Execute(new ExecutionContext(subcontexts));

        public void Execute(ExecutionContext executionContext)
        {
            if(executable != null)
                Compile();

            scope.StartExecution(executionContext);

            foreach(var invocation in executable)
            {
                invocation.Invoke();
            }

            scope.CompleteExecution();
        }
    }
}