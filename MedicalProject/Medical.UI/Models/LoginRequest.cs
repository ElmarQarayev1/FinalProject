using System;
using System.ComponentModel.DataAnnotations;

namespace Medical.UI.Models
{
	public class LoginRequest
	{
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

