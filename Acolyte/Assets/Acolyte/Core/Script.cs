using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    /// <summary>
    /// Encapsulates a script for runtime reference, compilation and execution.
    /// </summary>
    public sealed partial class Script
    {
        public string Source { get; private set; }

        public readonly Language language;

        private readonly Declexicon declexicon;

        private Executable executable;


        public Script(string source, Language language, bool compileOnCreation = false)
        {
            Source = source;
            this.language = language;

            // Each script contains its own declexicon instance
            declexicon = language.CreateLexicon();

            if(compileOnCreation)
                Compile();
        }

        public void Modify(string source, bool compile = false)
        {
            Source = source;

            if(compile)
                Compile();
        }

        public void Execute<T>(object context, Action<T> callback) where T : class
        {
            if(executable != null)
                Compile();

            declexicon.StartExecution(context);
            executable.Execute();
            declexicon.CompleteExecution(callback);
        }

        private void Compile()
        {
            executable = Compiler.Compile(language, declexicon, Source);
        }
    }
}