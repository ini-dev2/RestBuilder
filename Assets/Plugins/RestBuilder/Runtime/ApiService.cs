using System.Text;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System;

namespace RestBuilder
{
    public sealed class ApiService : IApiService
    {
        private readonly ISerializer _serializer;

        public ApiService(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public UniTask<TResponse> GetAsync<TResponse>(string endpoint, RequestOptions options = null)
            => SendRequest<TResponse>(UnityWebRequest.Get(endpoint), options);

        public UniTask<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, RequestOptions options = null)
            => SendRequestWithBody<TRequest, TResponse>("POST", endpoint, data, options);

        public UniTask<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, RequestOptions options = null)
            => SendRequestWithBody<TRequest, TResponse>("PUT", endpoint, data, options);

        public UniTask<TResponse> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data, RequestOptions options = null)
            => SendRequestWithBody<TRequest, TResponse>("PATCH", endpoint, data, options);

        public UniTask<TResponse> DeleteAsync<TResponse>(string endpoint, RequestOptions options = null)
            => SendRequest<TResponse>(UnityWebRequest.Delete(endpoint), options);

        public UniTask PostAsync<TRequest>(string endpoint, TRequest data, RequestOptions options = null)
            => SendRequestWithBody("POST", endpoint, data, options);

        public UniTask PutAsync<TRequest>(string endpoint, TRequest data, RequestOptions options = null)
            => SendRequestWithBody("PUT", endpoint, data, options);

        public UniTask PatchAsync<TRequest>(string endpoint, TRequest data, RequestOptions options = null)
            => SendRequestWithBody("PATCH", endpoint, data, options);

        public UniTask DeleteAsync(string endpoint, RequestOptions options = null)
            => SendRequest(UnityWebRequest.Delete(endpoint), options);

        public UniTask<byte[]> GetBytesAsync(string endpoint, RequestOptions options = null)
            => SendRequestForBytes(UnityWebRequest.Get(endpoint), options);

        private async UniTask SendRequest(UnityWebRequest request, RequestOptions options)
        {
            try
            {
                ApplyHeaders(request, options);

                request.downloadHandler ??= new DownloadHandlerBuffer();

                await request.SendWebRequest().WithCancellation(options?.CancellationToken ?? default);

                if (request.result != UnityWebRequest.Result.Success)
                    throw new ApiException(request.responseCode, request.downloadHandler?.text ?? request.error ?? string.Empty);
            }
            finally
            {
                request.Dispose();
            }
        }

        private async UniTask<TResponse> SendRequest<TResponse>(UnityWebRequest request, RequestOptions options)
        {
            try
            {
                ApplyHeaders(request, options);
                request.downloadHandler ??= new DownloadHandlerBuffer();

                await request.SendWebRequest().WithCancellation(options?.CancellationToken ?? default);

                if (request.result != UnityWebRequest.Result.Success)
                    throw new ApiException(request.responseCode, request.downloadHandler?.text ?? request.error ?? string.Empty);

                var json = request.downloadHandler.text;
                if (string.IsNullOrWhiteSpace(json))
                    return default;

                return _serializer.Deserialize<TResponse>(json);
            }
            finally
            {
                request.Dispose();
            }
        }

        private async UniTask<byte[]> SendRequestForBytes(UnityWebRequest request, RequestOptions options)
        {
            try
            {
                ApplyHeaders(request, options);
                request.downloadHandler = request.downloadHandler ?? new DownloadHandlerBuffer();

                await request.SendWebRequest().WithCancellation(options?.CancellationToken ?? default);

                if (request.result != UnityWebRequest.Result.Success)
                    throw new ApiException(request.responseCode, request.error);

                return request.downloadHandler.data;
            }
            finally
            {
                request.Dispose();
            }
        }

        private UniTask SendRequestWithBody<TRequest>(string method, string endpoint, TRequest data, RequestOptions options)
        {
            var request = BuildBodyRequest(method, endpoint, data);
            return SendRequest(request, options);
        }

        private UniTask<TResponse> SendRequestWithBody<TRequest, TResponse>(string method, string endpoint, TRequest data, RequestOptions options)
        {
            var request = BuildBodyRequest(method, endpoint, data);
            return SendRequest<TResponse>(request, options);
        }

        private UnityWebRequest BuildBodyRequest<TRequest>(string method, string endpoint, TRequest data)
        {
            var json = _serializer.Serialize(data);
            var bodyRaw = Encoding.UTF8.GetBytes(json);

            var request = new UnityWebRequest(endpoint, method)
            {
                uploadHandler = new UploadHandlerRaw(bodyRaw),
                downloadHandler = new DownloadHandlerBuffer()
            };

            return request;
        }

        private void ApplyHeaders(UnityWebRequest request, RequestOptions options)
        {
            var headers = options?.Headers;
            if (headers == null) return;

            foreach (var kv in headers)
            {
                if (string.IsNullOrEmpty(kv.Key)) continue;
                request.SetRequestHeader(kv.Key, kv.Value ?? string.Empty);
            }
        }
    }
}
