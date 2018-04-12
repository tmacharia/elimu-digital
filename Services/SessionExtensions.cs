using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public static class SessionExtensions
    {

        public static T Get<T>(this ISession session,string key) where T : class
        {
            string content = session.GetString(key);
            
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
        }
        public static void Set<T>(this ISession session,string key, T value) where T: class
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
    }
}
