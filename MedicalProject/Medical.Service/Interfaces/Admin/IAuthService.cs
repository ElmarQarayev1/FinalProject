using System;
using Medical.Service.Dtos.Admin.AuthDtos;
using Medical.Service.Dtos.Admin.CategoryDtos;
using Medical.Service.Dtos.User.AuthDtos;
using Microsoft.AspNetCore.Mvc;

namespace Medical.Service.Interfaces.Admin
{
	public interface IAuthService
	{
        SendingLoginDto Login(AdminLoginDto loginDto);

        string Create(SuperAdminCreateAdminDto createDto);
        PaginatedList<AdminPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10);
        List<AdminGetDto> GetAll(string? search = null);
        AdminGetDto GetById(string id);
        void Update(string id, AdminUpdateDto updateDto);
        void Delete(string id);
        Task UpdatePasswordAsync(AdminUpdateDto updatePasswordDto);


        Task<string> Register(MemberRegisterDto registerDto);

        Task<string> LoginForUser(MemberLoginDto loginDto);

        Task<string> ForgetPassword(MemberForgetPasswordDto forgetPasswordDto);

        Task ResetPassword(MemberResetPasswordDto resetPasswordDto);

        Task<bool> VerifyEmailAndToken(string email, string token);

        Task UpdateProfile(MemberProfileEditDto profileEditDto);

        MemberProfileGetDto GetByIdForUserProfile(string appUserId);





    }
}

