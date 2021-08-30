using System;
using System.Collections;
using System.Collections.Generic;


namespace Acolyte
{
    public abstract class Language
    {
        public abstract string Name { get; }

        public string Comment { get; protected set; } = "//";
        public char Separator => ' ';
        public bool IsCaseSensitive { get; protected set; } = true;

        /// <summary>
        /// Collection of all existing languages.
        /// </summary>
        private static readonly Dictionary<string, Language> languages = new Dictionary<string, Language>();

        private List<Func<IStatement>> statementFactories;

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
        public abstract Declexicon CreateDeclexicon();

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
        /// Add a Statement factory method that will provide a Statement instance to the compiler to process.
        /// </summary>
        public void AddStatementFactory(Func<IStatement> statementFactory)
        {
            if(statementFactories == null)
                statementFactories = new List<Func<IStatement>>();

            statementFactories.Add(statementFactory);
        }

        /// <summary>
        /// Returns an array of Statement instances generated from the factories added to this language.
        /// </summary>
        public IStatement[] GenerateStatements() 
        {
            if(statementFactories != null)
            {
                IStatement[] array = new IStatement[statementFactories.Count];

                for(int i = 0; i < statementFactories.Count; i++)
                {
                    array[i] = statementFactories[i].Invoke();
                    array[i].Language = this;
                }

                return array;
            }

            return null;
        }
    }
}