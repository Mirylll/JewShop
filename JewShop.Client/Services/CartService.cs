using System.Net.Http.Json;
using Blazored.LocalStorage;
using JewShop.Shared.Dtos;

namespace JewShop.Client.Services
{
    public class CartService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private const string SessionKey = "cart_session_id";
        
        public event Action? OnChange;

        public CartService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        public async Task<string> GetSessionId()
        {
            var sessionId = await _localStorage.GetItemAsync<string>(SessionKey);
            if (string.IsNullOrEmpty(sessionId))
            {
                sessionId = Guid.NewGuid().ToString();
                await _localStorage.SetItemAsync(SessionKey, sessionId);
            }
            return sessionId;
        }

        public async Task<List<CartItemDto>> GetCartItems()
        {
            var sessionId = await GetSessionId();
            var response = await _http.GetAsync($"api/cart/{sessionId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<CartItemDto>>() ?? new List<CartItemDto>();
            }
            return new List<CartItemDto>();
        }

        public async Task AddToCart(int variantId, int quantity = 1)
        {
            var sessionId = await GetSessionId();
            var dto = new AddToCartDto
            {
                SessionId = sessionId,
                VariantId = variantId,
                Quantity = quantity
            };
            
            await _http.PostAsJsonAsync("api/cart", dto);
            OnChange?.Invoke();
        }

        public async Task UpdateQuantity(int cartItemId, int quantity)
        {
            var dto = new UpdateCartItemDto { Quantity = quantity };
            await _http.PutAsJsonAsync($"api/cart/{cartItemId}", dto);
            OnChange?.Invoke();
        }

        public async Task RemoveFromCart(int cartItemId)
        {
            await _http.DeleteAsync($"api/cart/{cartItemId}");
            OnChange?.Invoke();
        }

        public async Task<int> GetCartCount()
        {
            var items = await GetCartItems();
            return items.Sum(i => i.Quantity);
        }
    }
}