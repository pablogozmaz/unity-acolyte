using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public partial class Script
    {
        /// <summary>
        /// Array of instructions that can be executed.
        /// </summary>
        private class Executable
        {
            private readonly Instruction[] instructions;

            public Executable(Instruction[] instructions)
            {
                this.instructions = instructions;
            }

            public static implicit operator Executable(Instruction[] instructions) => new Executable(instructions);

            public void Execute()
            {
                for(int i = 0; i < instructions.Length; i++)
                {
                    instructions[i].Execute(ref i);
                }
            }
        }
    }
}