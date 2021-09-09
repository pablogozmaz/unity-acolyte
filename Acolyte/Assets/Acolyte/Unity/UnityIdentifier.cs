using System;


namespace Acolyte
{
    public interface IUnityIdentifier
    {
        IUnityContainerBehaviour ProvideContainer();
    }

    /// <summary>
    /// Identifier word with a Unity object as the invocation parameter.
    /// </summary>
    public class UnityIdentifier<T> : Identifier<T>, IUnityIdentifier where T : class
    {
        private readonly Func<UnityContainerBehaviour<T>> containerProvider;


        public UnityIdentifier(Invocation<T> invocation, Func<UnityContainerBehaviour<T>> containerProvider) : base(invocation)
        {
            this.containerProvider = containerProvider;
        }

        public override void Invoke(string value)
        {
            if(containerProvider.Invoke().TryGetObject(value, out T obj))
                invocation.Invoke(obj);
            else
                invocation.Invoke(null);
        }

        public IUnityContainerBehaviour ProvideContainer() 
        {
            return containerProvider.Invoke();
        }
    }
}