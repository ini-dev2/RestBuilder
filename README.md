# RestBuilder

**RestBuilder** — библиотека на C# для упрощения работы с HTTP-запросами в Unity с использованием `UnityWebRequest`. Поддерживает асинхронные запросы (GET, POST, PUT, PATCH, DELETE), сериализацию/десериализацию JSON, настройку заголовков и динамическое построение URL.

## Возможности

- Асинхронные HTTP-запросы с использованием `UniTask` (`Cysharp.Threading.Tasks`).
- Поддержка методов: GET, POST, PUT, PATCH, DELETE.
- Сериализация/десериализация JSON через `Newtonsoft.Json`.
- Гибкое построение URL с параметрами пути и query-параметрами.
- Настройка заголовков и токенов отмены (`CancellationToken`).
- Обработка ошибок через `ApiException`.

## Установка

1. **Зависимости**:
   - Установите `Newtonsoft.Json` (доступно через NuGet или Unity Package Manager, если не желаете использовать Newtonsoft можете удалить файл NewtonsoftSerializer).
   - Установите `Cysharp.Threading.Tasks` (NuGet или GitHub: [UniTask](https://github.com/Cysharp/UniTask)).
   - Убедитесь, что `UnityWebRequest` доступен (встроен в Unity).

2. **Добавление RestBuilder**:
   используйте .unitypackage на странице релизов

3. **Инициализация**:
   ```csharp
   using RestBuilder;

   var serializer = new NewtonsoftSerializer();
   var apiService = new ApiService(serializer);
   ```

## Использование

### 1. Выполнение HTTP-запросов

Класс `ApiService` предоставляет методы для выполнения HTTP-запросов. Пример:

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
        Console.WriteLine($"Имя пользователя: {response.Name}");
    }
    catch (ApiException ex)
    {
        Console.WriteLine($"Ошибка: {ex.StatusCode}, {ex.Body}");
    }
}
```

### 2. Отправка данных

Для методов POST, PUT, PATCH можно отправлять данные:

```csharp
var userData = new { Name = "John", Age = 30 };
var response = await apiService.PostAsync<object, User>("https://api.example.com/users", userData);
```

### 3. Построение URL

Класс `EndpointBuilder` позволяет формировать URL с динамическими параметрами:

```csharp
var builder = new EndpointBuilder("https://api.example.com/users/{userId}/profile");
builder.AddPathParam("userId", 123);
builder.AddQueryParam("format", "json");
string url = builder.Build(); // https://api.example.com/users/123/profile?format=json
```

### 4. Настройка заголовков

Используйте `RequestOptions` для добавления заголовков и токена отмены:

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

### 5. Обработка ошибок

Ошибки HTTP-запросов обрабатываются через `ApiException`:

```csharp
try
{
    var response = await apiService.GetAsync<User>("https://api.example.com/invalid");
}
catch (ApiException ex)
{
    Console.WriteLine($"Ошибка HTTP {ex.StatusCode}: {ex.Body}");
}
```

## Пример комплексного использования

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
        Console.WriteLine($"Профиль: {profile.Name}");
    }
    catch (ApiException ex)
    {
        Console.WriteLine($"Ошибка: {ex.StatusCode}, {ex.Body}");
    }
}
```

## Ограничения

- Зависимость от `UnityWebRequest`, что ограничивает использование вне Unity.
- Сериализация только в JSON через `Newtonsoft.Json`. Для других форматов реализуйте собственный `ISerializer`.
- Требуется установка внешних библиотек (`UniTask`, `Newtonsoft.Json`).

## Лицензия

MIT License. Подробности см. в файле [LICENSE](LICENSE).
