using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DAL.Models
{
    public class AppUser : IdentityUser
    {
        [DefaultValue(Models.AccountType.None)]
        public AccountType AccountType { get; set; }

        public int AccountId { get; set; }
    }
}
