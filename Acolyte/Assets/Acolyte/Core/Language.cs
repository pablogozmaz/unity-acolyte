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

        public IEnumerable<StatementProcessor> StatementProcessors => statementProcessors;

        private List<StatementProcessor> statementProcessors;

        private Dictionary<Type, IExpressions> expressions;


        /// <summary>
        /// Creates a declexicon instance whose type is the required type for the language.
        /// </summary>
        public abstract Declexicon CreateDeclexicon();

        public void AddStatements(StatementProcessor statementProcessor)
        {
            if(statementProcessors == null)
                statementProcessors = new List<StatementProcessor>();

            statementProcessor.Initialize(this);
            statementProcessors.Add(statementProcessor);
        }

        public void AddExpression<T>(string expression, Func<T> func) where T : struct 
        {
            if(expressions == null)
                expressions = new Dictionary<Type, IExpressions>();

            if(!expressions.TryGetValue(typeof(T), out IExpressions expr))
            {
                expr = new Expressions<T>();
                expressions.Add(typeof(T), expr);
            }

            (expr as Expressions<T>).Add(expression, func);
        }

        public bool TryGetExpression<T>(string name, out T value) where T : struct
        {
            if(expressions != null && expressions.TryGetValue(typeof(T), out var iexpression))
            {
                if(iexpression is Expressions<T> expression)
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

        public bool ExpressionExists<T>(string name) where T : struct
        {
            if(expressions != null && expressions.TryGetValue(typeof(T), out var iexpression))
            {
                return iexpression.Contains(name);
            }
            return false;
        }     

        public bool ExpressionExists(string name, Type type)
        {
            if(expressions != null && expressions.TryGetValue(type, out var iexpression))
            {
                return iexpression.Contains(name);
            }
            return false;
        }
    }
}