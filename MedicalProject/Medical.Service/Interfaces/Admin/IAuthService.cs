using System;
using Medical.Service.Dtos.Admin.AuthDtos;

namespace Medical.Service.Interfaces.Admin
{
	public interface IAuthService
	{
        string Login(AdminLoginDto loginDto);
    }
}

