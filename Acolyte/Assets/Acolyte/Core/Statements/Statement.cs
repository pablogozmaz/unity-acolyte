using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public abstract class Statement
    {
        public Language Language
        {
            get { return language; }
            set
            {
                if(language == null)
                    language = value;
                else
                    throw new Exception("Language should not be assigned twice to a Statement.");
            }
        }

        private readonly Dictionary<string, StatementProcess> processes = new Dictionary<string, StatementProcess>();

        private Language language;


        public Statement()
        {
            PopulateProcessesDictionary();
        }

        public bool TryGetProcess(string line, out StatementProcess process)
        {
            foreach(var valuePair in processes)
            {
                string token = valuePair.Key.Trim();

                if(line.StartsWith(token))
                {
                    process = valuePair.Value;
                    return true;
                }
            }

            process = default;
            return false;
        }

        private void PopulateProcessesDictionary() 
        {
            foreach(var process in DefineProcesses())
                processes.Add(process.token, process);
        }

        // Require statements to define all their processes
        protected abstract StatementProcess[] DefineProcesses();
    }
}