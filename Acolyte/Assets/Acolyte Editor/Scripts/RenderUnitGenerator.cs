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
        private readonly List<RenderUnit> lineBuilder = new List<RenderUnit>();
        private readonly string separatorStr;


        private RenderUnitGenerator(string sourceCode, Language language)
        {
            this.sourceCode = sourceCode;
            this.language = language;
            declexicon = language.CreateLexicon();
            separatorStr = language.Separator.ToString();
        }

        public static RenderUnit[][] GenerateRender(string sourceCode, Language language)
        {
            return new RenderUnitGenerator(sourceCode, language).GetScriptRenderables();
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
            Word currentWord = declexicon.root;

            string substring;
            int startIndex = 0;

            for(int i = 0; i < line.Length; i++)
            {
                if(line[i] == language.Separator)
                {
                    substring = line.Substring(startIndex, i - startIndex);
                    ProcessWord(substring, ref currentWord);
                    AddSeparator();
                    startIndex = i + 1;
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
            
            /*lineBuilder.Add(new RenderUnit() 
            {
                content = separatorStr
            });*/
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