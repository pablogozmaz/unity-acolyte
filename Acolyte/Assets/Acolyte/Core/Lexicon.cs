using System;
using System.Collections.Generic;

namespace Acolyte
{
    public abstract class Lexicon
    {
        /// <summary>
        /// Root of the word tree.
        /// </summary>
        public Word root = new Word(); // Each scope has its own instanced word tree

        protected static Func<Lexicon, Word[]> OnRequestWords;


        public Lexicon()
        {
            var rootWords = OnRequestWords?.Invoke(this);

            foreach(var word in rootWords)
                root.Then(word);
        }

        public void StartExecution(object context)
        {
            HandleExecutionStart(context);
        }

        public void CompleteExecution<T>(Action<T> callback) where T : class
        {
            HandleExecutionCompletion(callback);
        }

        public abstract void EndOfLine();

        protected abstract void HandleExecutionStart(object context);

        protected abstract void HandleExecutionCompletion<T>(Action<T> callback) where T : class;
    }
}