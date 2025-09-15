using Newtonsoft.Json;

namespace RestBuilder
{
    public sealed class NewtonsoftSerializer : ISerializer
    {
        public string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj);

        public T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json);
    }
}