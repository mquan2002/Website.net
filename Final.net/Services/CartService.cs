using Final.net.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Final.net.Services
{
    public class CartService
    {
        private const string CartSessionKey = "CartSession";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public List<CartItem> GetCartItems()
        {
            var session = _httpContextAccessor.HttpContext?.Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(session))
            {
                return new List<CartItem>();
            }

            try
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(session);
            }
            catch
            {
                return new List<CartItem>();
            }
        }

        public void SaveCartSession(List<CartItem> cart)
        {
            try
            {
                var json = JsonConvert.SerializeObject(cart);
                _httpContextAccessor.HttpContext?.Session.SetString(CartSessionKey, json);
            }
            catch
            {
                // Xử lý lỗi nếu cần
            }
        }
    }
}
