using System;
using System.Linq;
using System.Collections.Generic;

namespace RestBuilder
{
    public sealed class EndpointBuilder
    {
        private readonly string _template;
        private readonly Dictionary<string, object> _pathParams = new();
        private readonly Dictionary<string, object> _queryParams = new();

        public EndpointBuilder(string template)
        {
            _template = template ?? throw new ArgumentNullException(nameof(template));
        }

        public EndpointBuilder AddPathParam(string key, object value)
        {
            var placeholder = $"{{{key}}}";
            if (!_template.Contains(placeholder))
                throw new ArgumentException($"Template does not contain placeholder {placeholder}");

            _pathParams[key] = value;
            return this;
        }

        public EndpointBuilder AddQueryParam(string key, object value)
        {
            _queryParams[key] = value;
            return this;
        }

        public string Build()
        {
            var url = _template;
            foreach (var kv in _pathParams)
                url = url.Replace($"{{{kv.Key}}}", kv.Value.ToString());

            if (_queryParams.Count > 0)
            {
                var query = string.Join("&", _queryParams.Select(kv => $"{kv.Key}={kv.Value}"));
                url += (url.Contains("?") ? "&" : "?") + query;
            }

            return url;
        }

        public override string ToString() => Build();
    }
}