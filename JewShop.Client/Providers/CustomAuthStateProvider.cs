using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;

namespace JewShop.Client.Providers
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;

        public CustomAuthStateProvider(ILocalStorageService localStorage, HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string token = await _localStorage.GetItemAsStringAsync("authToken");

            var identity = new ClaimsIdentity();
            _http.DefaultRequestHeaders.Authorization = null;

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    token = token.Replace("\"", "");
                    identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                catch
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    identity = new ClaimsIdentity();
                }
            }

            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }

        public void NotifyUserAuthentication(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            
            // IMPORTANT: Set Authorization header for HttpClient
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("\"", ""));
            
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            
            // IMPORTANT: Clear Authorization header
            _http.DefaultRequestHeaders.Authorization = null;
            
            NotifyAuthenticationStateChanged(authState);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            var claims = new List<Claim>();
            foreach (var kvp in keyValuePairs)
            {
                var value = kvp.Value;
                var valueString = value.ToString();
                
                // Fix Role Claim Type
                var claimType = kvp.Key;
                if (string.Equals(claimType, "role", StringComparison.OrdinalIgnoreCase) || 
                    string.Equals(claimType, "roles", StringComparison.OrdinalIgnoreCase))
                {
                    claimType = ClaimTypes.Role;
                }

                // Handle Array (Multiple Roles)
                if (value is JsonElement element && element.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in element.EnumerateArray())
                    {
                        claims.Add(new Claim(claimType, item.ToString()));
                    }
                }
                else
                {
                    // Handle Single Value
                    // Try to detect complex object (array string representation) if needed, 
                    // but usually direct string or JsonElement is enough.
                    if (valueString.Trim().StartsWith("[") && valueString.Trim().EndsWith("]"))
                    {
                         try 
                         {
                             var parsedRoles = JsonSerializer.Deserialize<string[]>(valueString);
                             if (parsedRoles != null)
                             {
                                 foreach (var role in parsedRoles)
                                 {
                                     claims.Add(new Claim(claimType, role));
                                 }
                                 continue;
                             }
                         } 
                         catch {} // Not a JSON array, treat as string
                    }
                    
                    claims.Add(new Claim(claimType, valueString));
                }
            }
            return claims;
        }

        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }    
            return Convert.FromBase64String(base64.Replace('-', '+').Replace('_', '/'));
        }
    }
}