using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


namespace Acolyte.Editor
{
    public class RenderUnitGenerator
    {
        private readonly string sourceCode;
        private readonly Language language;
        private readonly Declexicon declexicon;
        private readonly IStatement[] statements;
        private readonly List<RenderUnit> lineBuilder = new List<RenderUnit>();


        private RenderUnitGenerator(string sourceCode, Language language, Declexicon declexicon)
        {
            this.sourceCode = sourceCode;
            this.language = language;
            this.declexicon = declexicon;
            statements = language.GenerateStatements();
        }

        public static RenderUnit[][] GenerateRender(string sourceCode, Language language, Declexicon declexicon)
        {
            return new RenderUnitGenerator(sourceCode, language, declexicon).GetScriptRenderables();
        }

        private RenderUnit[][] GetScriptRenderables()
        {
            List<RenderUnit[]> lines = new List<RenderUnit[]>();

            using(StringReader reader = new StringReader(sourceCode))
            {
                string line;

                while((line = reader.ReadLine()) != null)
                {
                    if(!TryProcessAsComment(line))
                    {
                        ProcessLine(line);
                    }

                    lines.Add(lineBuilder.ToArray());
                    lineBuilder.Clear();
                }
            }

            return lines.ToArray();
        }

        private bool TryProcessAsComment(string line)
        {
            if(line.StartsWith(language.Comment))
            {
                lineBuilder.Add(new RenderUnit()
                {
                    content = line,
                    colorHex = "#A3A3A3"
                });
                return true;
            }

            return false;
        }

        private void ProcessLine(string line)
        {
            if(!ProcessStatements(line))
            {
                ProcessDeclexicon(line);
            }
        }

        private bool ProcessStatements(string line)
        {
            if(statements == null) return false;

            foreach(var statement in statements)
            {
                if(statement.TryGetProcess(line, out var process))
                {
                    AddStatementToken(process.token);

                    AddValidUndefined(line.Remove(0, process.token.Length));

                    return true;
                }
            }
            return false;
        }

        private void ProcessDeclexicon(string line)
        {
            Word currentWord = declexicon.root;

            string substring;
            int startIndex = 0;

            for(int i = 0; i < line.Length; i++)
            {
                if(line[i] == language.Separator)
                {
                    substring = line.Substring(startIndex, i - startIndex);
                    ProcessWord(substring, ref currentWord);

                    do
                    {
                        AddSeparator();
                        i++;
                    } 
                    while(line[i] == language.Separator);

                    startIndex = i;
                    i--;
                }
                else if(i == line.Length - 1)
                {
                    substring = line.Substring(startIndex);
                    ProcessWord(substring, ref currentWord);
                }
            }
        }

        private void ProcessWord(string word, ref Word currentWord)
        {
            if(currentWord.TryGetSubsequentKeyword(language.IsCaseSensitive ? word : word.ToLowerInvariant(), out Keyword keyword))
            {
                AddKeyword(keyword);
                currentWord = keyword;
            }
            // 2 - Tolerated word
            else if(currentWord.IsTolerated(word))
            {
                AddTolerated(word);
            }
            // 3 - Identifier
            else if(currentWord.SubsequentIdentifier != null)
            {
                AddIdentifier(currentWord.SubsequentIdentifier, word);
                currentWord = currentWord.SubsequentIdentifier;
            }
            // 4 - Literal
            else if(currentWord.SubsequentLiteral != null)
            {
                AddLiteral(currentWord.SubsequentLiteral, word);
                currentWord = currentWord.SubsequentLiteral;
            }
            else
            {
                AddUnrecognized(word);
            }
        }

        private void AddSeparator()
        {
            var ru = lineBuilder[lineBuilder.Count - 1];
            ru.content += language.Separator;
            lineBuilder[lineBuilder.Count - 1] = ru;
        }

        private void AddStatementToken(string value)
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = value,
                colorHex = "#51A3F5"
            });
        }

        private void AddKeyword(Keyword keyword) 
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = keyword.text,
                colorHex = "#51A3F5"
            });
        }

        private void AddIdentifier(Identifier identifier, string value)
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = value,
                colorHex = "#CCA88E"
            });
        }

        private void AddLiteral(Literal literal, string value)
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = value,
                colorHex = "#CCA88E"
            });
        }

        private void AddValidUndefined(string value)
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = value,
                colorHex = "#FFFFFF"
            });
        }

        private void AddTolerated(string value)
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = value,
                colorHex = "#FFFFFF"
            });
        }

        private void AddUnrecognized(string value)
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = value,
                colorHex = "#CC1F37"
            });
        }
    }
}