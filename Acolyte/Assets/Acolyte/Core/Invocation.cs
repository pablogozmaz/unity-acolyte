namespace Acolyte
{
    /// <summary>
    /// Allows to invoke functions linked to a word with a specific ScriptActionContext.
    /// </summary>
    public delegate void Invocation(ScriptActionContext context);

    /// <summary>
    /// Allows to invoke functions linked to a word with a specific ScriptActionContext.
    /// </summary>
    public delegate void Invocation<T>(ScriptActionContext context, T parameter);
}