using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace Acolyte
{
    /// <summary>
    /// Compiles a script's source code to an array of executable instructions.
    /// </summary>
    public class Compiler
    {
        private readonly List<Instruction> instructions = new List<Instruction>();

        private readonly Language language;
        private readonly Declexicon declexicon;
        private readonly string source;
        private readonly IStatement[] statements;


        private Compiler(Language language, Declexicon declexicon, string source)
        {
            this.language = language;
            this.declexicon = declexicon;
            this.source = source;

            statements = language.GenerateStatements();
        }

        public static Executable Compile(Language language, Declexicon declexicon, string source)
        {
            return new Compiler(language, declexicon, source).Compile();
        }

        private Executable Compile()
        {
            using(StringReader reader = new StringReader(source))
            {
                string line;

                while((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if(IsLineEmptyOrComment(line))
                        continue;

                    if(!ProcessStatements(line))
                    {
                        string[] words = line.Split(language.Separator);

                        if(!language.IsCaseSensitive)
                            words[0] = words[0].ToLowerInvariant();

                        ProcessDeclexicon(words);
                    }
                }
            }

            return instructions.ToArray();
        }

        private bool ProcessStatements(string line)
        {
            if(statements == null) return false;

            foreach(var statement in statements)
            {
                if(statement.TryGetProcess(line, out var process))
                {
                    Instruction instruction = process.function.Invoke(new StatementParameters(line, instructions.Count));
                    if(instruction != null) // Not all processes produce an instruction
                        instructions.Add(instruction);
                    return true;
                }
            }
            return false;
        }

        private void ProcessDeclexicon(string[] words)
        {
            // A line should always start with a matching initial keyword
            if(declexicon.root.TryGetSubsequentKeyword(words[0], out Keyword keyword))
            {
                AddKeyword(keyword);
                ProcessWordRecursively(keyword, words, 1);

                AddInstruction(declexicon.EndOfLine); // Call end of line on declexicon instance
            }
        }

        private void ProcessWordRecursively(Word current, string[] words, int index)
        {
            if(index >= words.Length) return;

            string word = words[index];
            Word next;

            // 1 - Keyword
            if(current.TryGetSubsequentKeyword(language.IsCaseSensitive ? word : word.ToLowerInvariant(), out Keyword keyword))
            {
                AddKeyword(keyword);
                next = keyword;
            }
            // 2 - Tolerated word
            else if(current.IsTolerated(word))
            {
                next = current;
            }
            // 3 - Identifier
            else if(current.SubsequentIdentifier != null)
            {
                AddIdentifier(current.SubsequentIdentifier, word);
                next = current.SubsequentIdentifier;
            }
            // 4 - Literal
            else if(current.SubsequentLiteral != null)
            {
                AddLiteral(current.SubsequentLiteral, word);
                next = current.SubsequentLiteral;
            }
            else
            {
                throw new System.Exception("Could not process word [" + word+"] in line ["+ Join(words) + "].");
            }

            ProcessWordRecursively(next, words, ++index);
        }

        private bool IsLineEmptyOrComment(string line)
        {
            return string.IsNullOrEmpty(line) || line.StartsWith(language.Comment);
        }

        private void AddKeyword(Keyword keyword)
        {
            if(keyword.HasInvocation)
            {
                AddInstruction(() =>
                {
                    keyword.Invoke();
                });
            }
        }

        private void AddIdentifier(Identifier identifier, string value)
        {
            AddInstruction(() =>
            {
                identifier.Invoke(value);
            });
        }

        private void AddLiteral(Literal literal, string value)
        {
            AddInstruction(() =>
            {
                literal.Invoke(value);
            });
        }

        private void AddInstruction(Invocation invocation)
        {
            instructions.Add(new InstructionInvocation(invocation));
        }

        private string Join(string[] array)
        {
            return string.Join(language.Separator.ToString(), array);
        }
    }
}