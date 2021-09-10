using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    /// <summary>
    /// Contains identified objects for an Identifier declexema.
    /// </summary>
    public interface IIdentifierContainer
    {
        IEnumerable<string> GetAllIdentifiers();
    }

    /// <summary>
    /// Word that is invoked with a parameter identified through a string.
    /// </summary>
    public abstract class Identifier : Declexeme
    {
        public abstract void Invoke(string value);

        public abstract IIdentifierContainer ProvideContainer();
    }
    
    /// <summary>
    /// Word that is invoked with a parameter identified through a string.
    /// </summary>
    public abstract class Identifier<T> : Identifier
    {
        protected readonly Invocation<T> invocation;

        protected Identifier(Invocation<T> invocation)
        {
            this.invocation = invocation;
        }
    }
}