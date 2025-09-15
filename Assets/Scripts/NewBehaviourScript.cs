using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using RestBuilder;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private IApiService apiService;
    private ISerializer serializer;
    private string baseUrl = "https://localhost:7126";
    Service service;

    private void Awake()
    {
        serializer = new NewtonsoftSerializer();
        apiService = new ApiService(serializer);

        service = new(apiService, baseUrl);
    }

    private async UniTaskVoid Start()
    {
        var r = await service.AuthorizationPost(new() { email = "admin@mail.ru", password = "adminpassword" });

        var r1 = await service.Get(r.Token);
        Debug.Log(r1);

        var r2 = await service.GetWithPathParam(r.Token, 1);
        Debug.Log(r2);
    }
}

public sealed class Service
{
    private readonly IApiService _apiService;
    private readonly string _baseUrl;

    private string endPointGet = "/api/Modules/GetAllPublic";
    private string endPontGetWithPathParam = "/api/Modules/GetByUser/{userId}";
    private string authEndPoint = "/api/Auth/Login";
    public Service(IApiService apiService, string baseUrl)
    {
        _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
        _baseUrl = baseUrl ?? throw new ArgumentNullException(nameof(baseUrl));
    }

    public async UniTask<DTO> AuthorizationPost(AuthDTO authDTO)
    {
        var url = _baseUrl + new EndpointBuilder(authEndPoint).ToString();
        Debug.Log(url);
        var result = await _apiService.PostAsync<AuthDTO, DTO>(url, authDTO, new()
        {
            Headers =
            {
                { "Content-Type", "application/json" }
            }
        });

        return result;
    }

    public async UniTask<List<DTO>> Get(string token)
    {
        var url = _baseUrl + new EndpointBuilder(endPointGet).ToString();
        Debug.Log(url);
        var result = await _apiService.GetAsync<List<DTO>>(url, new()
        {
            Headers =
            {
                { "Authorization", $"Bearer {token}" },
                { "Content-Type", "application/json" }
            }
        });

        return result;
    }

    public async UniTask<List<DTO>> GetWithPathParam(string token, int pathParam)
    {
        var url = _baseUrl + new EndpointBuilder(endPontGetWithPathParam)
            .AddPathParam("userId", pathParam);
        Debug.Log(url);
        var result = await _apiService.GetAsync<List<DTO>>(url, new()
        {
            Headers =
            {
                { "Authorization", $"Bearer {token}" },
                { "Content-Type", "application/json" },
            }
        });

        return result;
    }
}

public class DTO
{
    [JsonProperty("id")]
    public string Id;

    [JsonProperty("token")]
    public string Token;
}

public class AuthDTO
{
    [JsonProperty("email")]
    public string email;

    [JsonProperty("password")]
    public string password;
}