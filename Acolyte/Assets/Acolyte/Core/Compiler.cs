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
            private readonly List<Invocation> result = new List<Invocation>();

            private Language Language => script.language;
            private Scope Scope => script.scope;


            private Compiler(Script script)
            {
                this.script = script;
            }

            public static Invocation[] Compile(Script script)
            {
                return new Compiler(script).Compile();
            }

            private Invocation[] Compile()
            {
                using(StringReader reader = new StringReader(script.source))
                {
                    string line;

                    while((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if(IsLineEmptyOrComment(line))
                            continue;

                        string[] words = line.Split(Language.separator);

                        if(!Language.isCaseSensitive)
                            words[0] = words[0].ToLowerInvariant();

                        // A line should always start with a matching initial keyword
                        if(Scope.root.TryGetSubsequentKeyword(words[0], out Keyword keyword))
                        {
                            InvokeKeyword(keyword);

                            ProcessWordRecursively(keyword, words, 1);
                        }
                    }
                }

                return result.ToArray();
            }

            private void ProcessWordRecursively(Word current, string[] words, int index)
            {
                if(index >= words.Length) return;

                string word = words[index];

                // 1 - Keywords
                if(current.TryGetSubsequentKeyword(Language.isCaseSensitive ? word : word.ToLowerInvariant(), out Keyword keyword))
                {
                    InvokeKeyword(keyword);
                    ProcessWordRecursively(keyword, words, ++index);
                }
                // 2 - Identifiers
                else if(current.SubsequentIdentifier != null)
                {
                    InvokeIdentifier(current.SubsequentIdentifier, word);
                    ProcessWordRecursively(current.SubsequentIdentifier, words, ++index);
                }
                // 3 - Literals
                else if(current.SubsequentLiteral != null)
                {
                    InvokeLiteral(current.SubsequentLiteral, word);
                    ProcessWordRecursively(current.SubsequentLiteral, words, ++index);
                }
                else
                    throw new System.Exception("Processed word has no subsequent words: " + word);
            }

            private bool IsLineEmptyOrComment(string line)
            {
                return string.IsNullOrEmpty(line) || line.StartsWith(Language.comment);
            }

            private void InvokeKeyword(Keyword keyword)
            {
                if(keyword.HasInvocation)
                {
                    result.Add(() =>
                    {
                        keyword.Invoke();
                    });
                }
            }

            private void InvokeIdentifier(Identifier identifier, string value)
            {
                result.Add(() =>
                {
                    identifier.Invoke(value);
                });
            }

            private void InvokeLiteral(Literal literal, string value)
            {
                result.Add(() =>
                {
                    literal.Invoke(value);
                });
            }
        }
    }
}