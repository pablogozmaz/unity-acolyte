using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public interface IExecutionContextGenerator
    {
        ExecutionContext GenerateExecutionContext();
    }

    public interface IExecutionSubcontext {}

    public class ExecutionContext
    {
        private readonly IExecutionSubcontext[] subcontexts;


        public ExecutionContext(params IExecutionSubcontext[] subcontexts)
        {
            this.subcontexts = subcontexts;
        }

        public T GetSubcontext<T>() where T : IExecutionSubcontext
        {
            for(int i = 0; i < subcontexts.Length; i++)
            {
                if(subcontexts[i] is T cast)
                    return cast;
            }

            return default;
        }
    }
}