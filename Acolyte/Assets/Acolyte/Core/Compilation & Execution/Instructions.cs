using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    /// <summary>
    /// Base class for an executable instruction, resulting from compiling a script.
    /// </summary>
    public abstract class Instruction
    {
        public abstract void Execute(ref int index);
    }

    /// <summary>
    /// Instruction that invokes an invocation.
    /// </summary>
    public sealed class InstructionInvocation : Instruction
    {
        private readonly Invocation invocation;

        public InstructionInvocation(Invocation invocation)
        {
            this.invocation = invocation;
        }

        public override void Execute(ref int index)
        {
            invocation.Invoke();
        }
    }

    public class InstructionJump : Instruction
    {
        public int jumpIndex;


        public override void Execute(ref int index)
        {
            index = jumpIndex;
        }
    }

    public sealed class InstructionConditional : InstructionJump
    {
        public Func<bool> comparison;


        public override void Execute(ref int index)
        {
            if(comparison.Invoke())
                index = jumpIndex;
        }
    }
}