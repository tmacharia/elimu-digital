using Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IGoogleMapsService
    {
        Task<GMapResult> ReverseGeocode(double latitude, double longitude);
    }
}
