using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public class Language
    {
        public readonly string name;

        /// <summary>
        /// Root of the language's word tree.
        /// </summary>
        public readonly Word root = new Word();

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


        public Language(string name, params InvocationScope[] scopes)
        {
            languages.Add(name, this);

            this.name = name;

            if(scopes != null)
                foreach(var scope in scopes)
                    AddScope(scope);
        }

        public static IEnumerable<Language> GetAllLanguages() => languages.Values;
        
        public static bool TryGetLanguage(string name, out Language language) => languages.TryGetValue(name, out language);
        
        public void AddScope(InvocationScope scope)
        {
            foreach(var word in scope.rootwords)
                root.Then(word);
        }
    }
}