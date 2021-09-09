using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public abstract class StatementProcessor
    {
        /// <summary>
        /// The language this statement has been assigned to.
        /// </summary>
        public Language Language => language;

        public int StatementsCount => statements.Count;

        public IEnumerable<string> Tokens => statements.Keys;

        private Language language;

        private readonly Dictionary<string, Statement> statements = new Dictionary<string, Statement>();


        public void Initialize(Language language)
        {
            if(this.language != null)
                throw new Exception("Statement was initialized twice.");

            this.language = language;

            PopulateStatementDictionary();
        }

        public bool TryGetStatement(string line, out Statement statement)
        {
            foreach(var valuePair in statements)
            {
                string token = valuePair.Key;

                if(line.StartsWith(token))
                {
                    statement = valuePair.Value;
                    return true;
                }
            }

            statement = default;
            return false;
        }

        private void PopulateStatementDictionary() 
        {
            foreach(var process in DefineStatements())
                statements.Add(process.token, process);
        }

        // Require processors to define all their statements
        protected abstract Statement[] DefineStatements();
    }
}