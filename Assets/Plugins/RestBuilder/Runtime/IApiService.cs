using Cysharp.Threading.Tasks;

namespace RestBuilder
{
    public interface IApiService
    {
        public UniTask<TResponse> DeleteAsync<TResponse>(string endpoint, RequestOptions options = null);
        public UniTask<TResponse> GetAsync<TResponse>(string endpoint, RequestOptions options = null);
        public UniTask<TResponse> PatchAsync<TRequest, TResponse>(string endpoint, TRequest data, RequestOptions options = null);
        public UniTask<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, RequestOptions options = null);
        public UniTask<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, RequestOptions options = null);

        public UniTask DeleteAsync(string endpoint, RequestOptions options = null);
        public UniTask PatchAsync<TRequest>(string endpoint, TRequest data, RequestOptions options = null);
        public UniTask PostAsync<TRequest>(string endpoint, TRequest data, RequestOptions options = null);
        public UniTask PutAsync<TRequest>(string endpoint, TRequest data, RequestOptions options = null);

        public UniTask<byte[]> GetBytesAsync(string endpoint, RequestOptions options = null);
    }
}