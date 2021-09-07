using System;
using System.Collections.Generic;

namespace Acolyte
{
    public abstract partial class Declexicon
    {
        /// <summary>
        /// Root of the declarative tree.
        /// </summary>
        public readonly Declexeme root = new Declexeme();

        protected static Func<Declexicon, Declexeme[]> WordsRequested;

        private static readonly List<IDeclexemeFactory> declFactories = new List<IDeclexemeFactory>();


        public Declexicon()
        {
            foreach(var declFactory in declFactories)
            {
                if(declFactory.Invoke(this, out Declexeme[] declexemes))
                {
                    AddDeclexemes(declexemes);
                }
            }
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

        protected static void AddDeclexemeFactory<T>(Func<T, Declexeme[]> factoryMethod) where T : Declexicon
        {
            declFactories.Add(new DeclexemeFactory<T>(factoryMethod));
        }

        protected void AddDeclexemes(Declexeme[] declexemes)
        {
            foreach(var declexeme in declexemes)
                root.Then(declexeme);
        }

        protected abstract void HandleExecutionStart(object context);

        protected abstract void HandleExecutionCompletion<T>(Action<T> callback) where T : class;
    }
}