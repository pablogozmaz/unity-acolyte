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
        public static event Action<Script> OnCompilation;

        public string Source { get; private set; }

        public readonly Language language;

        public readonly Declexicon declexicon;

        public bool IsCompiled => executable != null;

        private Executable executable;


        public Script(string source, Language language, bool compileOnCreation = false)
        {
            Source = source;
            this.language = language;

            // Each script contains its own declexicon instance
            declexicon = language.CreateDeclexicon();

            if(compileOnCreation)
                Compile();
        }

        public void Modify(string source, bool compile = false)
        {
            Source = source;

            if(compile || IsCompiled)
                Compile();
        }

        public void Execute<T>(object context, Action<T> callback) where T : class
        {
            if(!IsCompiled)
                Compile();

            declexicon.StartExecution(context);
            executable.Execute();
            declexicon.CompleteExecution(callback);
        }

        private void Compile()
        {
            executable = Compiler.Compile(language, declexicon, Source);
            OnCompilation?.Invoke(this);
        }
    }
}