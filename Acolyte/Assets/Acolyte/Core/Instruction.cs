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

    public sealed class InstructionConditional : Instruction
    {
        public struct Data
        {
            public Func<bool> comparison;
            public int failureJumpIndex;
        }

        // public List<Data> data = new List<Data>();

        public Func<bool> comparison;
        public int failureJumpIndex;

        public InstructionConditional() { }

        public override void Execute(ref int index)
        {
            if(!comparison.Invoke())
                index = failureJumpIndex;
        }
    }
}