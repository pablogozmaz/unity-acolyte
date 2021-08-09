using System.Collections;
using System.Collections.Generic;

namespace Acolyte
{
    public interface IExecutable
    {
        bool Invoke();
    }

    public class ExecutableInvocation : IExecutable
    {
        private Invocation invocation;

        public ExecutableInvocation(Invocation invocation)
        {
            this.invocation = invocation;
        }

        public bool Invoke()
        {
            invocation.Invoke();

            return true;
        }
    }

    public class ExecutableEndOfLine : IExecutable
    {
        public bool Invoke()
        {
            return false;
        }
    }
}