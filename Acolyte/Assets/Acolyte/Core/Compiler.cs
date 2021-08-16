using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace Acolyte
{
    public partial class Script
    {
        private class Compiler
        {
            private readonly Script script;
            private readonly List<Instruction> result = new List<Instruction>();

            private Language Language => script.language;

            private InstructionConditional conditional;


            private Compiler(Script script)
            {
                this.script = script;
            }

            public static Executable Compile(Script script)
            {
                return new Compiler(script).Compile();
            }

            private Executable Compile()
            {
                using(StringReader reader = new StringReader(script.Source))
                {
                    string line;

                    while((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if(IsLineEmptyOrComment(line))
                            continue;

                        string[] words = line.Split(Language.Separator);

                        if(!Language.IsCaseSensitive)
                            words[0] = words[0].ToLowerInvariant();

                        if(!ProcessBuiltInKeywords(words)) 
                        {
                            ProcessLexicon(words);
                        }
                    }
                }

                return result.ToArray();
            }

            private bool ProcessBuiltInKeywords(string[] words)
            {
                if(conditional != null)
                {
                    if(words[0] == "endif")
                    {
                        conditional.failureJumpIndex = result.Count;
                        conditional = null;
                    }

                }
                else
                {
                    if(words[0] == "if")
                    {
                        conditional = new InstructionConditional();
                        result.Add(conditional);
                        conditional.comparison = () => { return 1 == 1; };

                        // Open scope
                        // Save IF encapsulation into scope
                        // When scope is ended, set jump variable to if
                        // If when scope is ended, there is an else, manage jump data..?
                        return true;
                    }
                }
                return false;
            }

            private void ProcessLexicon(string[] words)
            {
                // A line should always start with a matching initial keyword
                if(script.lexicon.root.TryGetSubsequentKeyword(words[0], out Keyword keyword))
                {
                    AddKeyword(keyword);

                    ProcessWordRecursively(keyword, words, 1);

                    // Call end of line on lexicon instance
                    AddInstruction(script.lexicon.EndOfLine);
                }
            }

            private void ProcessWordRecursively(Word current, string[] words, int index)
            {
                if(index >= words.Length) return;

                string word = words[index];

                // 1 - Keywords
                if(current.TryGetSubsequentKeyword(Language.IsCaseSensitive ? word : word.ToLowerInvariant(), out Keyword keyword))
                {
                    AddKeyword(keyword);
                    ProcessWordRecursively(keyword, words, ++index);
                }
                // 2 - Identifiers
                else if(current.SubsequentIdentifier != null)
                {
                    AddIdentifier(current.SubsequentIdentifier, word);
                    ProcessWordRecursively(current.SubsequentIdentifier, words, ++index);
                }
                // 3 - Literals
                else if(current.SubsequentLiteral != null)
                {
                    AddLiteral(current.SubsequentLiteral, word);
                    ProcessWordRecursively(current.SubsequentLiteral, words, ++index);
                }
                else
                {
                    if(Language.SupportsInvalidWords)
                        ProcessWordRecursively(current, words, ++index);
                    else
                        throw new System.Exception("Processed word has no subsequent words: " + word);
                }
            }

            private bool IsLineEmptyOrComment(string line)
            {
                return string.IsNullOrEmpty(line) || line.StartsWith(Language.Comment);
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
                result.Add(new InstructionInvocation(invocation));
            }
        }
    }
}