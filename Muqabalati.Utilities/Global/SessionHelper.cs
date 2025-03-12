using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Utilities.Global
{
    class SessionHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void SetInSession<T>(string sessionName, T value)
        {
            if (typeof(T) == typeof(int))
            {
                _httpContextAccessor.HttpContext?.Session.SetInt32(sessionName, (int)(object)value);
            }
            else
            {
                _httpContextAccessor.HttpContext?.Session.SetString(sessionName, (string)(object)value);
            }
        }

        public T GetFromSession<T>(string sessionName)
        {
            if (typeof(T) == typeof(int))
            {
                var value = _httpContextAccessor.HttpContext?.Session.GetInt32(sessionName) ?? 0;
                return (T)(object)value;
            }
            else
            {
                var value = _httpContextAccessor.HttpContext?.Session.GetString(sessionName) ?? "";
                return (T)(object)value;
            }
        }
    }
}
