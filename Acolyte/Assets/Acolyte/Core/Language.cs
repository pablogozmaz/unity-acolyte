using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public abstract class Language
    {
        public abstract string Name { get; }

        public string Comment { get; protected set; } = "//";
        public char Separator { get; protected set; } = ' ';
        public bool IsCaseSensitive { get; protected set; } = true;

        /// <summary>
        /// Collection of all existing languages.
        /// </summary>
        private static readonly Dictionary<string, Language> languages = new Dictionary<string, Language>();

        private List<Func<IStatement>> commandFactories;

        private Dictionary<Type, IExpression> expressions;


        public Language()
        {
            languages.Add(Name, this);
        }

        public static IEnumerable<Language> GetAllLanguages() => languages.Values;
        
        public static bool TryGetLanguage(string name, out Language language) => languages.TryGetValue(name, out language);

        /// <summary>
        /// Creates a scope instance whose type is the required type for the language.
        /// </summary>
        public abstract Declexicon CreateLexicon();

        public void AddExpression<T>(string expression, Func<T> func) where T : struct 
        {
            if(expressions == null)
                expressions = new Dictionary<Type, IExpression>();

            Expression<T> expr = new Expression<T>();
            expr.Add(expression, func);
            expressions.Add(typeof(T), expr);
        }

        public bool TryGetExpression<T>(string name, out T value) where T : struct
        {
            if(expressions != null && expressions.TryGetValue(typeof(T), out var iexpression))
            {
                if(iexpression is Expression<T> expression)
                {
                    return expression.TryGet(name, out value);
                }
            }

            value = default;
            return false;
        }

        public IEnumerable<string> GetExpressionsOfType(Type type)
        {
            if(expressions.TryGetValue(type, out var expression))
            {
                return expression.GetAllExpressionTokens();
            }
            return null;
        }

        /// <summary>
        /// Add a command factory method that will provide a Command instance to the compiler to process.
        /// </summary>
        public void AddCommandFactory(Func<IStatement> commandFactory)
        {
            if(commandFactories == null)
                commandFactories = new List<Func<IStatement>>();

            commandFactories.Add(commandFactory);
        }

        /// <summary>
        /// Returns an array of Command instances generated from command factories added to this language.
        /// </summary>
        public IStatement[] GenerateCommands() 
        {
            if(commandFactories != null)
            {
                IStatement[] array = new IStatement[commandFactories.Count];

                for(int i = 0; i < commandFactories.Count; i++)
                {
                    array[i] = commandFactories[i].Invoke();
                    array[i].Language = this;
                }

                return array;
            }

            return null;
        }
    }
}