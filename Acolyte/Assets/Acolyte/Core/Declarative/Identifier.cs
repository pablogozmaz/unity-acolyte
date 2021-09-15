using System;
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

    public interface IIdentifierContainer<T> : IIdentifierContainer
    {
        bool TryGetObject(string identifier, out T obj);
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
    public class Identifier<T> : Identifier where T : class
    {
        protected readonly Invocation<T> invocation;

        private readonly Func<IIdentifierContainer<T>> containerProvider;


        public Identifier(Invocation<T> invocation, Func<IIdentifierContainer<T>> containerProvider)
        {
            this.invocation = invocation;
            this.containerProvider = containerProvider;
        }

        public override void Invoke(string value)
        {
            if(containerProvider.Invoke().TryGetObject(value, out T obj))
                invocation.Invoke(obj);
            else
                invocation.Invoke(null);
        }

        public override IIdentifierContainer ProvideContainer()
        {
            return containerProvider.Invoke();
        }
    }
}