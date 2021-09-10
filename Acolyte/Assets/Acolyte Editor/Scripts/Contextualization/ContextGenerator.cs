using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte.Editor
{
    public class ContextGenerator
    {
        public struct Parameters
        {
            public Language language;
            public Declexicon declexicon;
            public string line;
            public string word;
            public int wordIndex;
        }

        private readonly string line;
        private readonly string[] lineArray;
        private readonly string word;
        private readonly int wordIndex;
        private readonly Language language;
        private readonly Declexicon declexicon;

        private readonly List<WordEditContext.IEntry> entries = new List<WordEditContext.IEntry>();


        private ContextGenerator(Parameters parameters)
        {
            language = parameters.language;
            declexicon = parameters.declexicon;
            line = parameters.line;
            lineArray = line.Split(language.Separator);
            word = parameters.word;
            wordIndex = parameters.wordIndex;
        }

        public static WordEditContext GetContext(Parameters parameters)
        {
            return new ContextGenerator(parameters).GetContext();
        }

        private WordEditContext GetContext()
        {
            entries.Clear();

            ProcessLine();

            return new WordEditContext(entries.ToArray());
        }

        private void ProcessLine()
        {
            if(line.StartsWith(language.Comment))
            {
                AddSelectableEntry("<Comment>", false);
                return;
            }

            bool continueProcess = true;

            ProcessStatements(ref continueProcess);

            if(continueProcess)
            {
                ProcessDeclexicon();
            }
        }

        private void ProcessStatements(ref bool continueProcess)
        {
            var statementProcessors = language.StatementProcessors;

            if(statementProcessors == null)
                return;

            if(wordIndex == 0)
                ProcessStatementTokens(statementProcessors);
            else
                ProcessExpressions(statementProcessors, ref continueProcess);
        }

        private void ProcessStatementTokens(IEnumerable<StatementProcessor> statementProcessors)
        {
            List<string> tokens = new List<string>();

            foreach(var statement in statementProcessors)
                foreach(string token in statement.Tokens)
                    tokens.Add(token);

            AddHeaderEntry("Statements (" + tokens.Count + ")");

            foreach(string token in tokens)
                AddSelectableEntry(token);
        }

        private void ProcessExpressions(IEnumerable<StatementProcessor> statementProcessors, ref bool continueProcess) 
        {
            foreach(var processor in statementProcessors)
            {
                if(processor.TryGetStatement(line, out var statement))
                {
                    if(statement.expressionType != null)
                    {
                        AddHeaderEntry("Expression: " + GetTypeName(statement.expressionType));

                        foreach(var expression in language.GetExpressionsOfType(statement.expressionType))
                        {
                            AddSelectableEntry(expression);
                            continueProcess = false;
                        }
                    }
                }
            }
        }

        private void ProcessDeclexicon()
        {
            Declexeme declexeme = declexicon.root;

            for(int i = 0; i < wordIndex; i++)
            {
                if(!ProcessWord(lineArray[i].Trim(), ref declexeme))
                {
                    AddSelectableEntry("<Unrecognized>", false);
                    return;
                }
            }

            if(!declexeme.IsTolerated(word.Trim()))
                AddAvailableDeclarations(declexeme);
            else
                AddSelectableEntry("<Tolerated>", false);
        }

        private bool ProcessWord(string word, ref Declexeme declexeme)
        {
            if(declexeme.TryGetSubsequentKeyword(language.IsCaseSensitive ? word : word.ToLowerInvariant(), out Keyword keyword))
            {
                declexeme = keyword;
                return true;
            }
            // 2 - Tolerated word
            if(declexeme.IsTolerated(word))
            {
                return true;
            }
            // 3 - Identifier
            if(declexeme.SubsequentIdentifier != null)
            {
                declexeme = declexeme.SubsequentIdentifier;
                return true;
            }
            // 4 - Literal
            if(declexeme.SubsequentLiteral != null)
            {
                declexeme = declexeme.SubsequentLiteral;
                return true;
            }
            
            return false;
        }

        private void AddAvailableDeclarations(Declexeme word)
        {
            AddKeywordEntries(word);

            if(word.SubsequentIdentifier != null)
            {
                AddIdentifierEntries(word.SubsequentIdentifier);
            }
            else if(word.SubsequentLiteral != null)
            {
                AddLiteralEntries();
            }
        }

        private void AddKeywordEntries(Declexeme word) 
        {
            int keywordCount = word.SubsequentKeywordsCount;
            if(keywordCount > 0)
            {
                AddHeaderEntry("Keywords (" + keywordCount + ")");
                foreach(string keywordToken in word.SubsequentKeywordTokens)
                {
                    AddSelectableEntry(keywordToken);
                }
            }
        }

        private void AddIdentifierEntries(Identifier identifier) 
        {
            List<string> ids = new List<string>();

            var container = identifier.ProvideContainer();
            if(container != null)
            {
                foreach(var id in container.GetAllIdentifiers())
                {
                    ids.Add(id);
                }
            }

            AddHeaderEntry("Object identifiers (" + ids.Count + ")");
            if(ids.Count == 0)
            {
                AddSelectableEntry("<No objects found>");
            }
            else
            {
                foreach(string id in ids)
                {
                    AddSelectableEntry(id);
                }
            }
        }

        private void AddLiteralEntries()
        {
            var entry = new WordEditContext.Selectable("<Literal>", false);
            entries.Insert(0, entry);
        }

        private void AddHeaderEntry(string header)
        {
            AddEntry(new WordEditContext.Header(header));
        }

        private void AddSelectableEntry(string selectable, bool interactable = true)
        {
            AddEntry(new WordEditContext.Selectable(selectable, interactable));
        }

        private void AddEntry(WordEditContext.IEntry entry)
        {
            entries.Add(entry);
        }

        /// <summary>
        /// Obtains a friendly type name.
        /// </summary>
        private string GetTypeName(Type type)
        {
            if(type == typeof(int))
                return "int";
            else if(type == typeof(short))
                return "short";
            else if(type == typeof(byte))
                return "byte";
            else if(type == typeof(bool))
                return "boolean";
            else if(type == typeof(long))
                return "long";
            else if(type == typeof(float))
                return "float";
            else if(type == typeof(double))
                return "double";
            else if(type == typeof(decimal))
                return "decimal";
            else if(type == typeof(string))
                return "string";
            else
                return type.Name;
        }
    }
}