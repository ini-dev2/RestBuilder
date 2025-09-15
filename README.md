# RestBuilder

**RestBuilder** is a C# library designed to simplify HTTP requests in Unity using `UnityWebRequest`. It supports asynchronous requests (GET, POST, PUT, PATCH, DELETE), JSON serialization/deserialization, header configuration, and dynamic URL building.

## Features

- Asynchronous HTTP requests using `UniTask` (`Cysharp.Threading.Tasks`).
- Support for methods: GET, POST, PUT, PATCH, DELETE.
- JSON serialization/deserialization via `Newtonsoft.Json`.
- Flexible URL construction with path and query parameters.
- Configuration of headers and cancellation tokens (`CancellationToken`).
- Error handling through `ApiException`.

## Installation

1. **Dependencies**:
   - Install `Newtonsoft.Json` (available via NuGet or Unity Package Manager; if you prefer not to use Newtonsoft, you can remove the `NewtonsoftSerializer` file).
   - Install `Cysharp.Threading.Tasks` (via NuGet or GitHub: [UniTask](https://github.com/Cysharp/UniTask)).
   - Ensure `UnityWebRequest` is available (built into Unity).

2. **Adding RestBuilder**:
   - Use the `.unitypackage` from the releases page.

3. **Initialization**:
   ```csharp
   using RestBuilder;

   var serializer = new NewtonsoftSerializer();
   var apiService = new ApiService(serializer);
   ```

## Usage

### 1. Performing HTTP Requests

The `ApiService` class provides methods for executing HTTP requests. Example:

```csharp
using RestBuilder;
using Cysharp.Threading.Tasks;

async UniTask GetUserDataAsync()
{
    var serializer = new NewtonsoftSerializer();
    var apiService = new ApiService(serializer);

    try
    {
        var response = await apiService.GetAsync<User>("https://api.example.com/users/123");
        Console.WriteLine($"User name: {response.Name}");
    }
    catch (ApiException ex)
    {
        Console.WriteLine($"Error: {ex.StatusCode}, {ex.Body}");
    }
}
```

### 2. Sending Data

For POST, PUT, and PATCH methods, you can send data:

```csharp
var userData = new { Name = "John", Age = 30 };
var response = await apiService.PostAsync<object, User>("https://api.example.com/users", userData);
```

### 3. Building URLs

The `EndpointBuilder` class allows constructing URLs with dynamic parameters:

```csharp
var builder = new EndpointBuilder("https://api.example.com/users/{userId}/profile");
builder.AddPathParam("userId", 123);
builder.AddQueryParam("format", "json");

or

var builder = new EndpointBuilder("https://api.example.com/users/{userId}/profile")
    .AddPathParam("userId", 123)
    .AddQueryParam("format", "json");

string url = builder.Build(); // https://api.example.com/users/123/profile?format=json
```

### 4. Configuring Headers

Use `RequestOptions` to add headers and a cancellation token:

```csharp
var options = new RequestOptions
{
    Headers = new Dictionary<string, string>
    {
        { "Authorization", "Bearer token123" },
        { "Content-Type", "application/json" }
    }
};
var response = await apiService.GetAsync<User>("https://api.example.com/users/123", options);
```

### 5. Error Handling

HTTP request errors are handled via `ApiException`:

```csharp
try
{
    var response = await apiService.GetAsync<User>("https://api.example.com/invalid");
}
catch (ApiException ex)
{
    Console.WriteLine($"HTTP Error {ex.StatusCode}: {ex.Body}");
}
```

## Comprehensive Example

```csharp
async UniTask FetchUserProfileAsync()
{
    var serializer = new NewtonsoftSerializer();
    var apiService = new ApiService(serializer);

    var builder = new EndpointBuilder("https://api.example.com/users/{userId}/profile");
    builder.AddPathParam("userId", 123);
    builder.AddQueryParam("details", "full");

    var options = new RequestOptions
    {
        Headers = new Dictionary<string, string> { { "Authorization", "Bearer token123" } }
    };

    try
    {
        var profile = await apiService.GetAsync<UserProfile>(builder.Build(), options);
        Console.WriteLine($"Profile: {profile.Name}");
    }
    catch (ApiException ex)
    {
        Console.WriteLine($"Error: {ex.StatusCode}, {ex.Body}");
    }
}
```

## Limitations

- Dependency on `UnityWebRequest`, limiting usage outside of Unity.
- JSON serialization is supported only via `Newtonsoft.Json`. For other formats, implement a custom `ISerializer`.
- Requires external libraries (`UniTask`, `Newtonsoft.Json`).

## License

MIT License. See the [LICENSE](LICENSE) file for details.