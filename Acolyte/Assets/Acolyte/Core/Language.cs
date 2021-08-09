using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public abstract class Language
    {
        public readonly string name;

        public string comment = "//";
        public char separator = ' ';
        public bool isCaseSensitive = true;

        /// <summary>
        /// Are words not found in the language supported or should they stop execution of a line?
        /// </summary>
        public bool supportsInvalidWords = false;

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

        public abstract Scope CreateScope();
    }
}