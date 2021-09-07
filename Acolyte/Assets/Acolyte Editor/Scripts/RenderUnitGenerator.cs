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
        private readonly Statement[] statements;
        private readonly List<RenderUnit> lineBuilder = new List<RenderUnit>();

        private RenderConfiguration RenderConfiguration { get { return RenderConfiguration.Current; } }


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
                    colorHex = RenderConfiguration.commentColor
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
            Declexeme currentWord = declexicon.root;

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

        private void ProcessWord(string word, ref Declexeme current)
        {
            if(current.TryGetSubsequentKeyword(language.IsCaseSensitive ? word : word.ToLowerInvariant(), out Keyword keyword))
            {
                AddKeyword(keyword);
                current = keyword;
            }
            // 2 - Tolerated word
            else if(current.IsTolerated(word))
            {
                AddTolerated(word);
            }
            // 3 - Identifier
            else if(current.SubsequentIdentifier != null)
            {
                AddIdentifier(current.SubsequentIdentifier, word);
                current = current.SubsequentIdentifier;
            }
            // 4 - Literal
            else if(current.SubsequentLiteral != null)
            {
                AddLiteral(current.SubsequentLiteral, word);
                current = current.SubsequentLiteral;
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
                colorHex = RenderConfiguration.statementColor
            });
        }

        private void AddKeyword(Keyword keyword) 
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = keyword.token,
                colorHex = RenderConfiguration.keywordColor
            });
        }

        private void AddIdentifier(Identifier identifier, string value)
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = value,
                colorHex = RenderConfiguration.identifierColor
            });
        }

        private void AddLiteral(Literal literal, string value)
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = value,
                colorHex = RenderConfiguration.literalColor
            });
        }

        private void AddValidUndefined(string value)
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = value,
                colorHex = RenderConfiguration.validUndefinedColor
            });
        }

        private void AddTolerated(string value)
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = value,
                colorHex = RenderConfiguration.toleratedColor
            });
        }

        private void AddUnrecognized(string value)
        {
            lineBuilder.Add(new RenderUnit()
            {
                content = value,
                colorHex = RenderConfiguration.unrecognizedColor
            });
        }
    }
}