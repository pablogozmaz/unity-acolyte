using System;
using System.Collections.Generic;

namespace Acolyte
{
    public abstract class Declexicon
    {
        /// <summary>
        /// Root of the word tree.
        /// </summary>
        public readonly Word root = new Word();

        protected static Func<Declexicon, Word[]> WordsRequested;


        public Declexicon()
        {
            var rootWords = WordsRequested?.Invoke(this);

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