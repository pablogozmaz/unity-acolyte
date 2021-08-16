using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public abstract partial class Language
    {
        public readonly string name;

        public string Comment { get; protected set; } = "//";
        public char Separator { get; protected set; } = ' ';
        public bool IsCaseSensitive { get; protected set; } = true;

        /// <summary>
        /// Are words not found in the language supported or should they stop execution of a line?
        /// </summary>
        public bool SupportsInvalidWords { get; protected set; } = false;

        /// <summary>
        /// Collection of all existing languages.
        /// </summary>
        private static readonly Dictionary<string, Language> languages = new Dictionary<string, Language>();


        public Language(string name)
        {
            this.name = name;
            languages.Add(name, this);
        }

        public static IEnumerable<Language> GetAllLanguages() => languages.Values;
        
        public static bool TryGetLanguage(string name, out Language language) => languages.TryGetValue(name, out language);

        /// <summary>
        /// Creates a scope instance whose type is the required type for the language.
        /// </summary>
        public abstract Lexicon CreateLexicon();
    }
}