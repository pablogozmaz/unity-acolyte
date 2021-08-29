using System.Collections;
using System.Collections.Generic;


namespace Acolyte.Editor
{
    public class WordEditContext
    {
        public interface IEntry
        {
            string Text { get; }
        }

        public struct Header : IEntry
        {
            public string Text { get; private set; }

            public Header(string text)
            {
                Text = text;
            }
        }

        public struct Selectable : IEntry
        {
            public string Text { get; private set; }

            public Selectable(string text)
            {
                Text = text;
            }
        }

        public IEnumerable<IEntry> Entries => entries;

        private IEntry[] entries;


        public WordEditContext(params IEntry[] entries)
        {
            this.entries = entries;
        }
    }
}