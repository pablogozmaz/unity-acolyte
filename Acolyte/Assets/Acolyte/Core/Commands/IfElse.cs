using System.Collections;
using System.Collections.Generic;



namespace Acolyte
{
    public class IfElse : Command<IfElse>
    {
        private readonly Stack<InstructionConditional> conditionals = new Stack<InstructionConditional>();


        public override bool Process(string line, int instructionCount, out Instruction result)
        {
            if(line.StartsWith("if"))
            {
                string expression = line.Substring(2).Trim();

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
                result = conditional;

                return true;
            }
            else if(line.StartsWith("endif"))
            {
                if(conditionals.Count > 0)
                {
                    conditionals.Pop().failureJumpIndex = instructionCount;
                }
                else
                    throw new System.Exception("Invalid endif");
            }

            result = null;
            return false;
        }
    }
}