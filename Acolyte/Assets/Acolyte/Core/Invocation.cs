using System.Collections;

namespace Acolyte
{
    public delegate void Invocation();

    public delegate void Invocation<T>(T parameter);
}