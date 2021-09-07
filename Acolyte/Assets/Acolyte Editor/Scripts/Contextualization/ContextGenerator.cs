using System.Collections;
using System.Collections.Generic;


namespace Acolyte.Editor
{
    public class ContextGenerator
    {
        public struct Parameters
        {
            public string[] line;
            public string word;
            public int wordIndex;
            public Language language;
            public Declexicon declexicon;
        }

        private readonly string[] line;
        private readonly string word;
        private readonly int wordIndex;
        private readonly Language language;
        private readonly Declexicon declexicon;
        private readonly Statement[] statements;

        private readonly List<WordEditContext.IEntry> entries = new List<WordEditContext.IEntry>();


        private ContextGenerator(Parameters parameters) 
        {
            line = parameters.line;
            word = parameters.word;
            wordIndex = parameters.wordIndex;

            language = parameters.language;
            declexicon = parameters.declexicon;
            statements = language.GenerateStatements();
        }

        public static WordEditContext GetContext(Parameters parameters)
        {
            return new ContextGenerator(parameters).GetContext();
        }

        private WordEditContext GetContext()
        {
            entries.Clear();

            ProcessLines();

            return new WordEditContext(entries.ToArray());
        }

        private void ProcessLines()
        {
            if(!ProcessStatements())
            {
                ProcessDeclexicon();
            }
        }

        private bool ProcessStatements()
        {
            if(statements == null) return false;

            return false;
            /*
            foreach(var statement in statements)
            {
                if(statement.TryGetProcess(line, out var process))
                {
                    return true;
                }
            }
            return false;*/
        }

        private void ProcessDeclexicon()
        {
            Declexeme currentWord = declexicon.root;
            HashSet<string> toleratedCache = new HashSet<string>();

            for(int i = 0; i < wordIndex; i++)
            {
                if(!ProcessWord(line[i].Trim(), ref currentWord, toleratedCache))
                {
                    AddEntry(new WordEditContext.Header("Unrecognized"));
                    return;
                }
            }

            AddAvailableDeclarations(currentWord);
        }

        private bool ProcessWord(string word, ref Declexeme currentWord, HashSet<string> toleratedCache)
        {
            if(currentWord.TryGetSubsequentKeyword(language.IsCaseSensitive ? word : word.ToLowerInvariant(), out Keyword keyword))
            {
                currentWord = keyword;
                return true;
            }
            // 2 - Tolerated word
            if(currentWord.IsTolerated(word))
            {
                toleratedCache.Add(word);
                return true;
            }
            // 3 - Identifier
            if(currentWord.SubsequentIdentifier != null)
            {
                currentWord = currentWord.SubsequentIdentifier;
                return true;
            }
            // 4 - Literal
            if(currentWord.SubsequentLiteral != null)
            {
                currentWord = currentWord.SubsequentLiteral;
                return true;
            }
            
            return false;
        }

        private void AddAvailableDeclarations(Declexeme word)
        {
            int keywordCount = word.SubsequentKeywordsCount;
            if(keywordCount > 0)
            {
                AddEntry(new WordEditContext.Header("Keywords ("+ keywordCount + ")"));
                foreach(string keywordToken in word.SubsequentKeywordTokens)
                {
                    AddEntry(new WordEditContext.Selectable(keywordToken));
                }
            }
        }

        private void AddEntry(WordEditContext.IEntry entry)
        {
            entries.Add(entry);
        }
    }
}