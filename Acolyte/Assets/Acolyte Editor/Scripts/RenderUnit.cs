using System.Collections.Generic;
using System.IO;


namespace Acolyte.Editor
{
    /// <summary>
    /// Encapsulates data for a string as an independent unit for editor display.
    /// </summary>
    public struct RenderUnit
    {
        public string content;
        public string colorHex;


        public static RenderUnit[][] GenerateView(string sourceCode, Language language, Declexicon declexicon)
        {
            IEnumerable<StatementProcessor> statementProcessors = language.StatementProcessors;
            var renderConfiguration = RenderConfiguration.Current;

            List<RenderUnit[]> lines = new List<RenderUnit[]>();
            List<RenderUnit> lineBuilder = new List<RenderUnit>();

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

            #region Local methods
            bool TryProcessAsComment(string line)
            {
                if(line.StartsWith(language.Comment))
                {
                    lineBuilder.Add(new RenderUnit()
                    {
                        content = line,
                        colorHex = renderConfiguration.commentColor
                    });
                    return true;
                }

                return false;
            }

            void ProcessLine(string line)
            {
                if(!ProcessStatements(line))
                {
                    ProcessDeclexicon(line);
                }
            }

            bool ProcessStatements(string line)
            {
                if(statementProcessors == null) return false;

                foreach(var processor in statementProcessors)
                {
                    if(processor.TryGetStatement(line, out var statement))
                    {
                        AddStatementToken(statement.token);

                        string expression = line.Remove(0, statement.token.Length);

                        if(statement.expressionType != null)
                        {
                            if(language.ExpressionExists(expression, statement.expressionType))
                                AddValidUndefined(expression);
                            else
                                AddUnrecognized(expression);
                        }
                        else
                        {
                            AddValidUndefined(expression);
                        }

                        return true;
                    }
                }
                return false;
            }

            void ProcessDeclexicon(string line)
            {
                Declexeme currentWord = declexicon.root;

                string substring;
                int startIndex = 0;

                for(int i = 0; i < line.Length; i++)
                {
                    if(line[i] == language.Separator)
                    {
                        substring = line.Substring(startIndex, i - startIndex);
                        ProcessDeclexeme(substring, ref currentWord);

                        do
                        {
                            AddSeparator();
                            i++;
                        }
                        while(i < line.Length && line[i] == language.Separator);

                        startIndex = i;
                        i--;
                    }
                    else if(i == line.Length - 1)
                    {
                        substring = line.Substring(startIndex);
                        ProcessDeclexeme(substring, ref currentWord);
                    }
                }
            }

            void ProcessDeclexeme(string word, ref Declexeme current)
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
                    bool validIdentifier = true;

                    var container = current.SubsequentIdentifier.ProvideContainer();
                    if(container != null)
                    {
                        validIdentifier = false;
                        foreach(var id in container.GetAllIdentifiers())
                        {
                            if(id == word)
                            {
                                validIdentifier = true;
                                break;
                            }
                        }
                    }

                    if(validIdentifier)
                        AddIdentifier(current.SubsequentIdentifier, word);
                    else
                        AddUnrecognized(word);

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

            void AddSeparator()
            {
                var ru = lineBuilder[lineBuilder.Count - 1];
                ru.content += language.Separator;
                lineBuilder[lineBuilder.Count - 1] = ru;
            }

            void AddStatementToken(string value)
            {
                lineBuilder.Add(new RenderUnit()
                {
                    content = value,
                    colorHex = renderConfiguration.statementColor
                });
            }

            void AddKeyword(Keyword keyword)
            {
                lineBuilder.Add(new RenderUnit()
                {
                    content = keyword.token,
                    colorHex = renderConfiguration.keywordColor
                });
            }

            void AddIdentifier(Identifier identifier, string value)
            {
                lineBuilder.Add(new RenderUnit()
                {
                    content = value,
                    colorHex = renderConfiguration.identifierColor
                });
            }

            void AddLiteral(Literal literal, string value)
            {
                lineBuilder.Add(new RenderUnit()
                {
                    content = value,
                    colorHex = renderConfiguration.literalColor
                });
            }

            void AddValidUndefined(string value)
            {
                lineBuilder.Add(new RenderUnit()
                {
                    content = value,
                    colorHex = renderConfiguration.validUndefinedColor
                });
            }

            void AddTolerated(string value)
            {
                lineBuilder.Add(new RenderUnit()
                {
                    content = value,
                    colorHex = renderConfiguration.toleratedColor
                });
            }

            void AddUnrecognized(string value)
            {
                lineBuilder.Add(new RenderUnit()
                {
                    content = value,
                    colorHex = renderConfiguration.unrecognizedColor
                });
            }
            #endregion
        }
    }
}