
namespace Acolyte
{
    public sealed class Literal : Word
    {
        private readonly Invocation<string> invocation;

        public Literal(Invocation<string> invocation = null)
        {
            this.invocation = invocation;
        }

        public void Invoke(string value)
        {
            invocation.Invoke(value);
        }
    }
}