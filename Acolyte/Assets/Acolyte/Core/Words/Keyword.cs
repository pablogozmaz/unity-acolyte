
namespace Acolyte
{
    public sealed class Keyword : Word
    {
        public readonly string text;

        public bool HasInvocation { get { return invocation != null; } }

        private readonly Invocation invocation;


        public Keyword(string text, Invocation invocation = null)
        {
            this.text = text;
            this.invocation = invocation;
        }

        public void Invoke()
        {
            invocation.Invoke();
        }
    }
}