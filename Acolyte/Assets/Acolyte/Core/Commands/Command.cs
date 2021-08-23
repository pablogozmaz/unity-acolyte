using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public interface ICommand
    {
        Language Language { get; set;  }

        bool Process(string line, int instructionCount, out Instruction result);
    }

    public abstract class Command<T> : ICommand where T : Command<T>, new()
    {
        public Language Language 
        {
            get { return language; }
            set
            {
                if(language == null)
                    language = value;
                else
                    throw new Exception("Language should not be assigned twice to a Command.");
            }
        }

        private Language language;

        public static Func<T> Factory
        {
            get
            {
                return () => { return new T(); };
            }
        }

        public abstract bool Process(string line, int instructionCount, out Instruction result);
    }
}