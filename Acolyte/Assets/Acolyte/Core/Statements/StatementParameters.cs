
namespace Acolyte
{
    public struct StatementParameters
    {
        public string line;
        public int instructionCount;

        public StatementParameters(string line, int instructionCount)
        {
            this.line = line;
            this.instructionCount = instructionCount;
        }
    }
}