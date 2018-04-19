using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace web.API_s
{
    [Authorize]
    [Route("api/locations")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]

    public class LocationsController : Controller
    {
        private readonly IRepositoryFactory _repos;
        private readonly IGoogleMapsService _gmaps;

        public LocationsController(IRepositoryFactory factory, IGoogleMapsService mapsService)
        {
            _repos = factory;
            _gmaps = mapsService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok(_repos.Locations.List.ToList());
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> Test(double latitude,double longitude)
        {
            var result = await _gmaps.ReverseGeocode(latitude, longitude);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Save(double latitude,double longitude)
        {
            var result = await _gmaps.ReverseGeocode(latitude, longitude);

            if(result == null)
            {
                return this.Error(HttpStatusCode.InternalServerError, "Reverse geo-coding operation failed.");
            }

            var profile = this.GetMyProfile();

            if(profile.Location == null)
            {
                profile.Location = new Location();
            }

            var loc = result.results[0];

            profile.Location.Latitude = latitude;
            profile.Location.Longitude = longitude;
            profile.Location.FullAddress = loc.formatted_address;

            // retreive street & city
            string[] tokens = loc.formatted_address.Split(',');

            if(tokens.Length > 2)
            {
                profile.Location.Street = tokens[0];
                profile.Location.City = tokens[1];
            }
            else if(tokens.Length <= 2)
            {
                profile.Location.City = tokens[0];
            }

            profile = _repos.Profiles.Update(profile);
            _repos.Commit();

            return Ok(profile.Location);
        }

        [HttpPost]
        [Route("update")]
        public IActionResult Update(Location location)
        {
            return Ok(location);
        }
    }
}
