using System;
using System.Collections;
using System.Collections.Generic;



namespace Acolyte
{
    public sealed class IfElse : Statement<IfElse>
    {
        private readonly Stack<InstructionConditional> conditionals = new Stack<InstructionConditional>();

        protected override StatementProcess[] DefineProcesses()
        {
            return new StatementProcess[] 
            {
                new StatementProcess("if", IfProcess, typeof(bool)),
                new StatementProcess("endif", EndIfProcess)
            };
        }

        private Instruction IfProcess(StatementParameters parameters) 
        {
            string expression = parameters.line.Substring(2).Trim();

            // Create a conditional whose comparison is based on the value of the boolean expression
            var conditional = new InstructionConditional
            {
                comparison = () =>
                {
                    if(Language.TryGetExpression(expression, out bool value))
                        return value;
                    return false;
                }
            };

            conditionals.Push(conditional);
            return conditional;
        }

        private Instruction EndIfProcess(StatementParameters parameters)
        {
            if(conditionals.Count > 0)
            {
                conditionals.Pop().failureJumpIndex = parameters.instructionCount;
            }
            else
                throw new Exception("Invalid endif");

            return null;
        }
    }
}