using System;


namespace Acolyte
{
    /// <summary>
    /// Encapsulates a word tree and its invocable functions.
    /// </summary>
    public abstract class InvocationScope
    {
        public readonly Word[] rootwords;


        public InvocationScope()
        {
            rootwords = Build();

            if(rootwords == null || rootwords.Length == 0)
                throw new Exception("An inovcation scope requires adding at least one word.");
        }

        /// <summary>
        /// Creates the word tree structure for this scope returning all words subsequent to the root word.
        /// </summary>
        protected abstract Word[] Build();
    }
}