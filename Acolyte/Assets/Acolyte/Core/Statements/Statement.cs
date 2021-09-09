using System;


namespace Acolyte
{
    public class Statement
    {
        public delegate Instruction Function(StatementParameters parameters);

        public readonly string token;

        /// <summary>
        /// The type returned by the expression interpreted by this process. Should be null if no expression is needed.
        /// </summary>
        public readonly Type expressionType;

        public readonly Function function;


        public Statement(string token, Function function, Type expressionType = null)
        {
            this.token = token;
            this.function = function;
            this.expressionType = expressionType;
        }
    }

}