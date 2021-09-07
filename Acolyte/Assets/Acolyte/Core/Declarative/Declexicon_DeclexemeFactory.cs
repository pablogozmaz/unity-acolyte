using System;


namespace Acolyte
{
    public partial class Declexicon
    {
        private interface IDeclexemeFactory
        {
            bool Invoke(Declexicon declexicon, out Declexeme[] declexemes);
        }

        private class DeclexemeFactory<T> : IDeclexemeFactory where T : Declexicon
        {
            public Func<T, Declexeme[]> factoryMethod;

            public DeclexemeFactory(Func<T, Declexeme[]> factoryMethod)
            {
                this.factoryMethod = factoryMethod;
            }

            public bool Invoke(Declexicon declexicon, out Declexeme[] declexemes)
            {
                if(declexicon is T castedDeclexicon)
                {
                    declexemes = factoryMethod.Invoke(castedDeclexicon);
                    return true;
                }
                declexemes = null;
                return false;
            }
        }
    }
}