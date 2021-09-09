using System.Collections;
using System.Collections.Generic;


namespace Acolyte.Editor
{
    /// <summary>
    /// Contains all data to generate a contextualizer for a word.
    /// </summary>
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
            public bool Interactable { get; private set; }

            public Selectable(string text, bool interactable = true)
            {
                Text = text;
                Interactable = interactable;
            }
        }

        public int EntriesCount => entries.Length;

        public IEnumerable<IEntry> Entries => entries;

        private readonly IEntry[] entries;


        public WordEditContext(params IEntry[] entries)
        {
            this.entries = entries;
        }
    }
}