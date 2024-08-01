using System;
namespace Medical.Service.Dtos.Admin.AuthDtos
{
	public class SendingLoginDto
	{
        public string? Token { get; set; }

        public bool PasswordResetRequired { get; set; }
    }
}

