using System;
using System.ComponentModel.DataAnnotations;

namespace Medical.Service.Dtos.Admin.AuthDtos
{
	public class MemberRegisterDto
	{
        
        public string UserName { get; set; }
        
        public string Email { get; set; }
      
        public string FullName { get; set; }
            
        public string Password { get; set; }
             
        public string ConfirmPassword { get; set; }
    }
}

