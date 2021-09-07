
namespace Acolyte
{
    public sealed class Keyword : Declexeme
    {
        public readonly string token;

        public bool HasInvocation { get { return invocation != null; } }

        private readonly Invocation invocation;


        public Keyword(string token, Invocation invocation = null)
        {
            this.token = token;
            this.invocation = invocation;
        }

        public void Invoke()
        {
            invocation.Invoke();
        }
    }
}