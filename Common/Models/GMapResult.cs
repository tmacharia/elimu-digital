using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class GMapResult
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }
    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
    }
    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }
}
