using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public struct StatementParameters
    {
        public string line;
        public int instructionCount;

        public StatementParameters(string line, int instructionCount)
        {
            this.line = line;
            this.instructionCount = instructionCount;
        }
    }

    public class StatementProcess
    {
        public readonly string token;
        public readonly Func<StatementParameters, Instruction> function;
        /// <summary>
        /// The type returned by the expression interpreted by this process. Should be null if no expression is needed.
        /// </summary>
        public readonly Type expressionType;

        public StatementProcess(string token, Func<StatementParameters, Instruction> function, Type expressionType = null)
        {
            this.token = token;
            this.function = function;
            this.expressionType = expressionType;
        }
    }

    public interface IStatement
    {
        Language Language { get; set; }

        bool TryGetProcess(string line, out StatementProcess process);
    }

    public abstract class Statement<T> : IStatement where T : Statement<T>, new()
    {
        public static Func<T> Factory => () => { return new T(); };

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