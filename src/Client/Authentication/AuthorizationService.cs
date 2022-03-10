using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BingoLingo.Shared;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace BingoLingo.Client.Authentication;
public class AuthorizationService 
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthorizationService(HttpClient httpClient,
        AuthenticationStateProvider authenticationStateProvider,
        ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
    }

    //public async Task<RegisterResult> Register(RegisterModel registerModel)
    //{
    //    var result = await _httpClient.PostJsonAsync<RegisterResult>("api/accounts", registerModel);

    //    return result;
    //}

    public async Task<LoginResult> Login(LoginResult loginResult)
    {
        await _localStorage.SetItemAsync("authToken", loginResult.Token);
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResult.Name);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResult.Token);

        return loginResult;
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }
}
