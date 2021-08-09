using System;
using System.Collections.Generic;

namespace Acolyte
{
    public abstract class Scope
    {
        /// <summary>
        /// Root of the language scope's word tree.
        /// </summary>
        public Word root = new Word();

        protected static Func<Scope, Word[]> BuildWordTree;


        public Scope()
        {
            var rootWords = BuildWordTree?.Invoke(this);

            foreach(var word in rootWords)
                root.Then(word);
        }

        public void StartExecution(ExecutionContext executionContext) 
        {
            HandleExecutionStart(executionContext);
        }

        public void CompleteExecution()
        {
            HandleExecutionCompletion();
        }

        protected abstract void HandleExecutionStart(ExecutionContext context);

        protected abstract void HandleExecutionCompletion();
    }
}