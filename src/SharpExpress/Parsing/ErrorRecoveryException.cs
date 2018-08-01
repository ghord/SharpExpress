using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpExpress.Parsing
{
    /// <summary>
    /// Exception that unrolls the stack for parser recovery
    /// </summary>
    class ErrorRecoveryException : Exception
    {
        public ErrorRecoveryException(ErrorRecoveryScope scope)
        {
            if (scope == null)
                throw new ArgumentNullException(nameof(scope));

            Scope = scope;
        }

        public ErrorRecoveryScope Scope { get; }
    }
}
