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

        private List<Type> statementTypes;

        private Dictionary<Type, IExpression> expressions;


        /// <summary>
        /// Creates a declexicon instance whose type is the required type for the language.
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

        public void AddStatement<T>() where T : Statement
        {
            if(statementTypes == null)
                statementTypes = new List<Type>();

            var type = typeof(T);

            if(!statementTypes.Contains(type))
                statementTypes.Add(type);
        }

        /// <summary>
        /// Returns an array of Statement instances generated from registered types.
        /// </summary>
        public Statement[] GenerateStatements() 
        {
            if(statementTypes != null)
            {
                Statement[] array = new Statement[statementTypes.Count];

                for(int i = 0; i < statementTypes.Count; i++)
                {
                    array[i] = Activator.CreateInstance(statementTypes[i]) as Statement;
                    array[i].Language = this;
                }

                return array;
            }

            return null;
        }
    }
}