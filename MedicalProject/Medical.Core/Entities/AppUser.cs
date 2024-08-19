using System;
using Microsoft.AspNetCore.Identity;

namespace Medical.Core.Entities
{
	public class AppUser:IdentityUser
	{

        public string FullName { get; set; }

        public bool IsPasswordResetRequired { get; set; } = true;

      
    }
}

