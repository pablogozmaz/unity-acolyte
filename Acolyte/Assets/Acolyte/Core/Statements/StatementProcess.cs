using System;


namespace Acolyte
{
    public class StatementProcess
    {
        public delegate Instruction Function(StatementParameters parameters);

        public readonly string token;
        public readonly Function function;
        /// <summary>
        /// The type returned by the expression interpreted by this process. Should be null if no expression is needed.
        /// </summary>
        public readonly Type expressionType;

        public StatementProcess(string token, Function function, Type expressionType = null)
        {
            this.token = token;
            this.function = function;
            this.expressionType = expressionType;
        }
    }

}