using System;
namespace Medical.Service.Dtos.Admin.AuthDtos
{
	public class AdminUpdateDto
	{
        
        public string UserName { get; set; }

        
        public string CurrentPassword { get; set; }

        
        public string NewPassword { get; set; }

       
        public string ConfirmPassword { get; set; }

    }
}

