using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using Newtonsoft.Json;

namespace Services
{
    public class GoogleMapsService : IGoogleMapsService
    {
        private readonly string API_Key;

        public GoogleMapsService(string apiKey)
        {
            API_Key = apiKey;
        }

        public async Task<GMapResult> ReverseGeocode(double latitude, double longitude)
        {
            HttpClient client = new HttpClient();
            string requestUrl = "https://maps.googleapis.com/maps/api/geocode/json?latlng=";
            requestUrl += $"{latitude},{longitude}";
            requestUrl += $"&key={API_Key}";

            HttpResponseMessage response = await client.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();

                GMapResult gMap = new GMapResult();

                gMap = JsonConvert.DeserializeObject<GMapResult>(json);

                return gMap;
            }
            else
            {
                return null;
            }
        }
    }
}
