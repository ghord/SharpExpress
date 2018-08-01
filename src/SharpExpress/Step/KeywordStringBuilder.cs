using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Step
{
    public class KeywordStringBuilder
    {
        private TrieNode root_ = new TrieNode();
        private TrieNode current_;
        private StringBuilder builder_ = new StringBuilder();
        const int AlphabetSize = 38;

        class TrieNode
        {
            public TrieNode()
            {
                Children = new TrieNode[AlphabetSize];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static int GetIndex(char character)
            {
                if (character == '_')
                    return 0;
                if (character == '-')
                    return 1;

                if (character >= 'A' && character <= 'Z')
                    return character - 'A' + 2;

                return character - '0' + 28;
            }

            public string InternedValue { get; set; }
            public TrieNode[] Children { get; }

            public TrieNode GetOrCreateChild(char character)
            {
                var idx = GetIndex(character);
                if (Children[idx] == null)
                {
                    Children[idx] = new TrieNode();
                }

                return Children[idx];
            }
        }

        public KeywordStringBuilder()
        {
            current_ = root_;
        }

        public void Append(char character)
        {
            builder_.Append(character);
            current_ = current_.GetOrCreateChild(character);
        }

        public override string ToString()
        {
            if(current_.InternedValue == null)
            {
                current_.InternedValue = string.Intern(builder_.ToString());
            }

            return current_.InternedValue;
        }
   
        public void Clear()
        {
            builder_.Clear();
            current_ = root_;
        }
    }
}
