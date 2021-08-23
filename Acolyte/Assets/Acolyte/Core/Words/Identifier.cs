
namespace Acolyte
{
    /// <summary>
    /// Word that is invoked with a parameter identified through a string.
    /// </summary>
    public abstract class Identifier : Word
    {
        public abstract void Invoke(string value);
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