using System;
using System.Collections;
using System.Collections.Generic;



namespace Acolyte
{
    /// <summary>
    /// Statement processor for If/Else statements.
    /// </summary>
    public sealed class IfElse : StatementProcessor
    {
        private readonly Stack<InstructionJump> instructions = new Stack<InstructionJump>();

        private readonly string ifToken = "if ";
        private readonly string elseToken = "else";
        private readonly string endifToken = "endif";


        public IfElse(string ifToken, string elseToken, string endifToken)
        {
            this.ifToken = ifToken.Trim() + " ";
            this.elseToken = elseToken;
            this.endifToken = endifToken;
        }

        protected override Statement[] DefineStatements()
        {
            return new Statement[]
            {
                new Statement(ifToken, IfFunction, typeof(bool)),
                new Statement(elseToken, ElseFunction),
                new Statement(endifToken, EndIfFunction),
            };
        }

        private Instruction IfFunction(StatementParameters parameters) 
        {
            string expression = parameters.line.Substring(ifToken.Length).Trim();

            // Create a conditional whose comparison is based on the value of the boolean expression
            var conditional = new InstructionConditional
            {
                comparison = () =>
                {
                    Language.TryGetExpression(expression, out bool value);
                    return !value; // Jump when comparison is false
                }
            };

            instructions.Push(conditional);
            return conditional;
        }

        private Instruction EndIfFunction(StatementParameters parameters)
        {
            if(instructions.Count > 0)
                instructions.Pop().jumpIndex = parameters.instructionCount - 1;
            else
                throw new Exception("Invalid endif");

            return null;
        }

        private Instruction ElseFunction(StatementParameters parameters)
        {
            if(instructions.Count > 0)
            {
                var lastInstruction = instructions.Pop();
                lastInstruction.jumpIndex = parameters.instructionCount;

                var jump = new InstructionJump();

                instructions.Push(jump);
                return jump;
            }
            else
                throw new Exception("Invalid else");
        }
    }
}