using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    /// <summary>
    /// Categorizes a unit of meaning in a language.
    /// Each word forms a tree structure with other subsequent words.
    /// </summary>
    public class Word
    {
        public Identifier SubsequentIdentifier { get; private set; }
        public Literal SubsequentLiteral { get; private set; }

        private Dictionary<string, Keyword> subsequentKeywords;

        private static HashSet<string> tolerated;


        public bool TryGetSubsequentKeyword(string text, out Keyword keyword)
        {
            if(subsequentKeywords == null)
            {
                keyword = null;
                return false;
            }

            return subsequentKeywords.TryGetValue(text, out keyword);
        }

        public void Then(Word word)
        {
            if(word is Keyword keyword)
                Then(keyword);
            else if(word is Literal literal)
                Then(literal);
            else if(word is Identifier identifier)
                Then(identifier);
        }

        public void Then(Keyword word)
        {
            if(subsequentKeywords == null)
                subsequentKeywords = new Dictionary<string, Keyword>();

            subsequentKeywords.Add(word.text, word);
        }

        public void Then(Identifier identifier)
        {
            if(SubsequentIdentifier != null)
                throw new Exception("Only a single identifier can follow a word.");

            SubsequentIdentifier = identifier;
        }

        public void Then(Literal literal)
        {
            if(SubsequentLiteral != null)
                throw new Exception("Only a single literal can follow a word.");

            SubsequentLiteral = literal;
        }

        /// <summary>
        /// Add a string as a tolerated word whose processing will be ignored, passing to the next word.
        /// </summary>
        public void Tolerate(string word)
        {
            if(tolerated == null)
                tolerated = new HashSet<string>();
            else if(tolerated.Contains(word))
                return;
            tolerated.Add(word);
        }

        public bool IsTolerated(string word)
        {
            if(tolerated == null) return false;

            return tolerated.Contains(word);
        }
    }
}