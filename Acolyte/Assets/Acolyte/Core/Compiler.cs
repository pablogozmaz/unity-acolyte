using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace Acolyte
{
    public partial class Script
    {
        private class Compiler
        {
            private readonly string script;
            private readonly Language language;
            private readonly List<Invocation> result;


            private Compiler(string script, Language language)
            {
                this.script = script;
                this.language = language;

                result = new List<Invocation>();
            }

            public static Invocation[] Compile(Script script)
            {
                return new Compiler(script.source, script.language).Compile();
            }

            private Invocation[] Compile()
            {
                using(StringReader reader = new StringReader(script))
                {
                    string line;

                    while((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();

                        if(IsLineEmptyOrComment(line))
                            continue;

                        if(!language.isCaseSensitive)
                            line = line.ToLowerInvariant();

                        string[] words = line.Split(language.separator);

                        // A line should always start with a matching initial keyword
                        if(language.root.TryGetSubsequentKeyword(words[0], out Keyword keyword))
                        {
                            AddKeywordInvocation(keyword);

                            ProcessWordRecursively(keyword, words, 1);
                        }
                    }
                }

                return result.ToArray();
            }

            private void ProcessWordRecursively(Word word, string[] words, int index)
            {
                if(index >= words.Length) return;

                // 1 - Keywords
                if(word.TryGetSubsequentKeyword(words[index], out Keyword keyword))
                {
                    AddKeywordInvocation(keyword);

                    ProcessWordRecursively(keyword, words, ++index);
                }
                // 2 - Identifiers
                else if(word.SubsequentIdentifier != null)
                {
                    AddIdentifierInvocation(word.SubsequentIdentifier, words[index]);

                    ProcessWordRecursively(word.SubsequentIdentifier, words, ++index);
                }
                // 3 - Literals
                else if(word.SubsequentLiteral != null)
                {
                    AddLiteralInvocation(word.SubsequentLiteral, words[index]);

                    ProcessWordRecursively(word.SubsequentLiteral, words, ++index);
                }
                else
                    throw new System.Exception("Processed word has no subsequent words: " + words[index]);
            }

            private bool IsLineEmptyOrComment(string line)
            {
                return string.IsNullOrEmpty(line) || line.StartsWith(language.comment);
            }

            private void AddKeywordInvocation(Keyword keyword)
            {
                if(keyword.HasInvocation)
                {
                    result.Add((ScriptActionContext actionContext) =>
                    {
                        keyword.Invoke(actionContext);
                    });
                }
            }

            private void AddIdentifierInvocation(Identifier identifier, string value)
            {
                result.Add((ScriptActionContext actionContext) =>
                {
                    identifier.Invoke(actionContext, value);
                });
            }

            private void AddLiteralInvocation(Literal literal, string value)
            {
                result.Add((ScriptActionContext actionContext) => 
                {
                    literal.Invoke(actionContext, value);
                });
            }
        }
    }
}