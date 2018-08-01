using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    class ErrorRecoveryScope
    {
        public ErrorRecoveryScope(TokenKind syncTokenKind, string syncTokenText = null)
        {
            SyncTokenKind = syncTokenKind;
            SyncTokenText = syncTokenText;
        }

        public string SyncTokenText { get; }
        public TokenKind SyncTokenKind { get; }

        public bool CanRecover(TokenKind kind, string text)
        {
            if (kind != SyncTokenKind)
                return false;

            if (SyncTokenText != null && !SyncTokenText.Equals(text, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
