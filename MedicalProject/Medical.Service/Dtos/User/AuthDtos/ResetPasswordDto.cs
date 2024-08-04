using System;
namespace Medical.Service.Dtos.User.AuthDtos
{
	public class ResetPasswordDto
	{
        public string UserId { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}

