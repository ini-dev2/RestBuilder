using System.Threading;
using System.Collections.Generic;

namespace RestBuilder
{
    public sealed class RequestOptions
    {
        public Dictionary<string, string> Headers { get; set; } = new();
        public CancellationToken CancellationToken { get; set; } = default;
    }
}