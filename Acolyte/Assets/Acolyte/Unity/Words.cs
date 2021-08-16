using System;


namespace Acolyte
{
    public class UnityIdentifier<T> : Identifier<T> where T : class
    {
        private readonly Func<UnityContainerBehaviour<T>> containerProvider;

        public UnityIdentifier(Invocation<T> invocation, Func<UnityContainerBehaviour<T>> containerProvider) : base(invocation)
        {
            this.containerProvider = containerProvider;
        }

        public override void Invoke(string value)
        {
            var tracker = containerProvider.Invoke();

            if(tracker.TryGetObject(value, out T obj))
            {
                invocation.Invoke(obj);
            }
        }
    }
}