using System;


namespace Acolyte
{
    public abstract class Identifier : Word
    {
        public abstract void Invoke(string value);
    }
    
    public abstract class Identifier<T> : Identifier
    {
        protected readonly Invocation<T> invocation;

        protected Identifier(Invocation<T> invocation)
        {
            this.invocation = invocation;
        }
    }

    public sealed class Var : Identifier<object>
    {
        public Var(Invocation<object> invocation) : base(invocation)
        {
            throw new NotImplementedException();
        }

        public override void Invoke(string value)
        {
            throw new NotImplementedException();
        }
    }
}