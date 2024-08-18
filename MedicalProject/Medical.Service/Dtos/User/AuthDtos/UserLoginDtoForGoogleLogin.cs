using System;
namespace Medical.Service.Dtos.User.AuthDtos
{
	public class UserLoginDtoForGoogleLogin
	{
         public  string Email { get; set; }
         public  string? Password { get; set; }      
         public  bool ExternalLogin { get; set; }
        
    }
}

