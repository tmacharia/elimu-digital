using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Extensions
{
    public static class Methods
    {
        public static IList<SelectListItem> DaysOfTheWeeks()
        {
            List<SelectListItem> list = new List<SelectListItem>()
            {
                new SelectListItem(){Text="-- Select --",Value=""},
                new SelectListItem(){Text="Monday",Value="1"},
                new SelectListItem(){Text="Tuesday",Value="2"},
                new SelectListItem(){Text="Wednesday",Value="3"},
                new SelectListItem(){Text="Thursday",Value="4"},
                new SelectListItem(){Text="Friday",Value="5"}
            };

            return list;
        }
    }
}
