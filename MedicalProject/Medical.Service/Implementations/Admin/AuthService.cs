using System;
using Medical.Core.Entities;
using Medical.Service.Exceptions;
using Medical.Service.Interfaces.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Medical.Service.Dtos.Admin.AuthDtos;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Medical.Service.Dtos.User.AuthDtos;
using Medical.Service.Dtos.User.OrderDtos;
using Microsoft.EntityFrameworkCore;
using Medical.Data;
using Medical.Data.Repositories.Interfaces;
using Medical.Core.Enum;
using Medical.Data.Repositories.Implementations;
using Medical.Service.Dtos.User.AppointmentDtos;

namespace Medical.Service.Implementations.Admin
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly EmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IOrderRepository _orderRepository;
        private readonly IAppointmentRepository _appointmentRepository;


        public AuthService(UserManager<AppUser> userManager, IConfiguration configuration,IMapper mapper,EmailService emailService,AppDbContext appDbContext,IOrderRepository orderRepository,IAppointmentRepository appointmentRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _emailService = emailService;
            _context = appDbContext;
            _orderRepository = orderRepository;
            _appointmentRepository = appointmentRepository;
        }

        //public MemberProfileGetDto GetByIdForUserProfile(string appUserId)
        //{
        //    var user = _userManager.Users.FirstOrDefault(u => u.Id == appUserId);
        //    if (user == null)
        //    {
        //        throw new RestException(StatusCodes.Status404NotFound, "AppUserId", "User not found.");
        //    }

        //    var orders = _orderRepository.GetAll(o => o.AppUser.Id == appUserId && o.Status != OrderStatus.Canceled, "AppUser")
        //       .Select(order => new OrderGetDtoForUserProfile
        //       {
        //           CreatedAt = order.CreatedAt,
        //           TotalPrice = order.OrderItems.Sum(oi => oi.SalePrice * oi.Count),
        //           TotalItemCount = order.OrderItems.Sum(oi => oi.Count),
        //           OrderItems = order.OrderItems.Select(oi => new OrderItemDto
        //           {
        //               MedicineId = oi.MedicineId,
        //               Count = oi.Count,
        //               Price = oi.SalePrice
        //           }).ToList(),
        //           Status = order.Status.ToString()
        //       }).ToList();

        //    var userProfile = new MemberProfileGetDto
        //    {
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        FullName = user.FullName,
        //        HasPassword = _userManager.HasPasswordAsync(user).Result,
        //        IsGoogleUser = _userManager.GetLoginsAsync(user).Result.Any(login => login.LoginProvider == "Google"),
        //        Orders = orders
        //    };

        //    return userProfile;
        //}


        public MemberProfileGetDto GetByIdForUserProfile(string appUserId)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == appUserId);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "AppUserId", "User not found.");
            }

            var orders = _orderRepository.GetAll(o => o.AppUser.Id == appUserId && o.Status != OrderStatus.Canceled, "AppUser")
                .Select(order => new OrderGetDtoForUserProfile
                {
                    CreatedAt = order.CreatedAt,
                   TotalPrice = order.OrderItems.Sum(oi => oi.SalePrice * oi.Count),
                    TotalItemCount = order.OrderItems.Sum(oi => oi.Count),
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        MedicineId = oi.MedicineId,
                        Count = oi.Count,
                        Price = oi.SalePrice
                    }).ToList(),
                    Status = order.Status.ToString()
                }).ToList();

            var appointments = _appointmentRepository.GetAll(a => a.AppUserId == appUserId, "Doctor")
                .Select(appointment => new AppointmentGetDtoForUserProfile
                {
                    DoctorFullName = appointment.Doctor.FullName,
                    Phone = appointment.Phone,
                    Date = appointment.Date.ToString("dd-MM-yyyy HH:mm")
                }).ToList();

            var userProfile = new MemberProfileGetDto
            {
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                HasPassword = _userManager.HasPasswordAsync(user).Result,
                IsGoogleUser = _userManager.GetLoginsAsync(user).Result.Any(login => login.LoginProvider == "Google"),
                Orders = orders??new List<OrderGetDtoForUserProfile>(),
                Appointments = appointments??new List<AppointmentGetDtoForUserProfile>()
            };

            return userProfile;
        }


        public async Task UpdateProfile(MemberProfileEditDto profileEditDto)
        {
            var user = await _userManager.FindByEmailAsync(profileEditDto.Email);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound,"UserName", "User not found.");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new RestException(StatusCodes.Status400BadRequest,"Email", "Email is not confirmed.");
            }

            user.UserName = profileEditDto.UserName;
            user.FullName = profileEditDto.FullName;

            if (_userManager.Users.Any(x => x.Id != user.Id && x.NormalizedEmail == profileEditDto.Email.ToUpper()))
            {
                throw new RestException(StatusCodes.Status400BadRequest,"Email", "Email is already taken.");
            }

            if (!string.IsNullOrEmpty(profileEditDto.NewPassword))
            {
                if (profileEditDto.IsGoogleUser || !profileEditDto.HasPassword)
                {
                    var addPasswordResult = await _userManager.AddPasswordAsync(user, profileEditDto.NewPassword);
                    if (!addPasswordResult.Succeeded)
                    {
                        var errors = string.Join(", ", addPasswordResult.Errors.Select(e => e.Description));
                        throw new RestException(StatusCodes.Status400BadRequest, $"Failed to add new password: {errors}");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(profileEditDto.CurrentPassword))
                    {
                        throw new RestException(StatusCodes.Status400BadRequest, "CurrentPassword","Current password is required.");
                    }

                    var changePasswordResult = await _userManager.ChangePasswordAsync(user, profileEditDto.CurrentPassword, profileEditDto.NewPassword);
                    if (!changePasswordResult.Succeeded)
                    {
                        var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                        throw new RestException(StatusCodes.Status400BadRequest, $"Failed to change password: {errors}");
                    }
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to update profile: {errors}");
            }
        }




        public async Task<string> ForgetPassword(MemberForgetPasswordDto forgetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgetPasswordDto.Email);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Email is not confirmed.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = $"{_configuration["AppSettings:AppBaseUrl"]}/api/resetpassword?email={user.Email}&token={Uri.EscapeDataString(token)}";

          
            var emailTemplate = @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>Password Reset Request</title>
            <style>
                .email-container {
                    font-family: Arial, sans-serif;
                    color: #333;
                    max-width: 600px;
                    margin: auto;
                    padding: 20px;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                    background-color: #f9f9f9;
                }
                .email-header {
                    text-align: center;
                    padding-bottom: 20px;
                }
                .email-body {
                    text-align: left;
                    line-height: 1.6;
                }
                .email-footer {
                    text-align: center;
                    padding-top: 20px;
                    font-size: 12px;
                    color: #999;
                }
            </style>
        </head>
        <body>
            <div class='email-container'>
                <div class='email-header'>
                    <h2>Password Reset Request</h2>
                </div>
                <div class='email-body'>
                    <p>Hello,</p>
                    <p>We received a request to reset your password. Please click the link below to reset your password:</p>
                    <p><a href='{{resetUrl}}'>Reset Your Password</a></p>
                    <p>If you did not request this, please ignore this email.</p>
                </div>
                <div class='email-footer'>
                    <p>&copy; 2024 Hospital. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";

            
            var emailBody = emailTemplate.Replace("{{resetUrl}}", resetUrl);

            var subject = "Password Reset";
            _emailService.Send(user.Email, subject, emailBody);

            return resetUrl;
        }


        public async Task ResetPassword(MemberResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmNewPassword)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "New password and confirm password do not match.");
            }

            var decodedToken = Uri.UnescapeDataString(resetPasswordDto.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to reset password: {errors}");
            }
        }

        public async Task<bool> VerifyEmailAndToken(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Email is not confirmed.");
            }
            var decodedToken = Uri.UnescapeDataString(token);
            var result = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ResetPassword", decodedToken);
            if (!result)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Invalid token.");
            }

            return true;
        }
        
    
         public async Task<string> LoginForUser(MemberLoginDto loginDto)
          {

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                throw new RestException(StatusCodes.Status401Unauthorized, "UserName or Email incorrect!");
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                throw new RestException(StatusCodes.Status401Unauthorized, "Email", "Email not confirmed.");
            }


            var token = await GenerateJwtToken(user);

            return token;
        }


        public async Task<string> Register(MemberRegisterDto registerDto)
        {
            
            if (registerDto.Password != registerDto.ConfirmPassword)
            {
                throw new RestException(StatusCodes.Status400BadRequest,"ConfirmPassword", "Password and ConfirmPassword do not match.");
            }

           
            if (_userManager.Users.Any(u => u.Email.ToLower() == registerDto.Email.ToLower()))
            {
                throw new RestException(StatusCodes.Status400BadRequest,"Email", "Email is already taken.");
            }

          
            if (_userManager.Users.Any(u => u.UserName.ToLower() == registerDto.UserName.ToLower()))
            {
                throw new RestException(StatusCodes.Status400BadRequest, "UserName","UserName is already taken.");
            }

           
            var appUser = new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                IsPasswordResetRequired = false
            };

            
            var result = await _userManager.CreateAsync(appUser, registerDto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to register user: {errors}");
            }

            
            var roleResult = await _userManager.AddToRoleAsync(appUser, "member");
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to assign role: {errors}");
            }

          
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);


            var confirmationUrl = $"{_configuration["AppSettings:AppBaseUrl"]}/api/account/verifyemail?userId={appUser.Id}&token={Uri.EscapeDataString(token)}";


            var emailTemplate = @"
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset='UTF-8'>
            <title>Email Verification</title>
            <style>
                .email-container {
                    font-family: Arial, sans-serif;
                    color: #333;
                    max-width: 600px;
                    margin: auto;
                    padding: 20px;
                    border: 1px solid #ddd;
                    border-radius: 5px;
                    background-color: #f9f9f9;
                }
                .email-header {
                    text-align: center;
                    padding-bottom: 20px;
                }
                .email-body {
                    text-align: left;
                    line-height: 1.6;
                }
                .email-footer {
                    text-align: center;
                    padding-top: 20px;
                    font-size: 12px;
                    color: #999;
                }
            </style>
        </head>
        <body>
            <div class='email-container'>
                <div class='email-header'>
                    <h2>Email Verification</h2>
                </div>
                <div class='email-body'>
                    <p>Hello,</p>
                    <p>Please confirm your email by clicking the link below:</p>
                    <p><a href='{{confirmationUrl}}'>Verify Your Email</a></p>
                    <p>If you did not request this, please ignore this email.</p>
                </div>
                <div class='email-footer'>
                    <p>&copy; 2024 Hospital. All rights reserved.</p>
                </div>
            </div>
        </body>
        </html>";

          
            var emailBody = emailTemplate.Replace("{{confirmationUrl}}", confirmationUrl);

            var subject = "Email Verification";
            _emailService.Send(appUser.Email, subject, emailBody);

            return appUser.Id;
        }

       

        public async Task<bool> IsEmailConfirmedAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            return user.EmailConfirmed;
        }






        public string Create(SuperAdminCreateAdminDto createDto)
        {

            var existingUser = _userManager.FindByNameAsync(createDto.UserName).Result;
            if (existingUser != null)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "UserName", "UserName already taken");
            }

            var user = new AppUser
            {
                UserName = createDto.UserName,
                IsPasswordResetRequired = true,
                FullName=createDto.UserName,

            };
    
            var result = _userManager.CreateAsync(user, createDto.Password).Result;
            if (!result.Succeeded)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Password" ,"Failed to create Admin user.");
            }

            
            var roleResult = _userManager.AddToRoleAsync(user, "Admin").Result;

           
            if (!roleResult.Succeeded)
            {
                throw new RestException(StatusCodes.Status400BadRequest,"UserName", "Failed to assign role to Admin user.");
            }

            
            return user.Id;
        }

        public void Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var result = _userManager.DeleteAsync(user).Result;

            if (!result.Succeeded)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Failed to delete Admin user.");
            }
        }

        public List<AdminGetDto> GetAll(string? search = null)
        {
            
            var users = _userManager.Users.ToList();
          
            var adminUsers = _mapper.Map<List<AdminGetDto>>(users);
           
            var filteredAdminUsers = adminUsers.Where(adminUser =>
            {
                var user = users.FirstOrDefault(u => u.Id == adminUser.Id);
                var roles = _userManager.GetRolesAsync(user).Result;
                return roles.Contains("Admin");
            }).ToList();

           
            if (!string.IsNullOrEmpty(search))
            {
                filteredAdminUsers = filteredAdminUsers
                    .Where(u => u.UserName.Contains(search))
                    .ToList();
            }

            return filteredAdminUsers;
        }

        public PaginatedList<AdminPaginatedGetDto> GetAllByPage(string? search = null, int page = 1, int size = 10)
        {
           
            var users = _userManager.Users.ToList();

          
            var adminUsers = _mapper.Map<List<AdminPaginatedGetDto>>(users);

           
            var filteredAdminUsers = new List<AdminPaginatedGetDto>();

            foreach (var adminUser in adminUsers)
            {
                var user = users.FirstOrDefault(u => u.Id == adminUser.Id);
                var roles = _userManager.GetRolesAsync(user).Result;
                if (roles.Contains("Admin"))
                {
                    filteredAdminUsers.Add(adminUser);
                }
            }

          
            if (!string.IsNullOrEmpty(search))
            {
                filteredAdminUsers = filteredAdminUsers
                    .Where(u => u.UserName.Contains(search))
                    .ToList();
            }

           
            var paginatedResult = PaginatedList<AdminPaginatedGetDto>.Create(filteredAdminUsers.AsQueryable(), page, size);

            return paginatedResult;
        }

        public AdminGetDto GetById(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound,"UserName", "User not found.");
            }

           
            var userDto = _mapper.Map<AdminGetDto>(user);

            return userDto;
        }


       


        public SendingLoginDto Login(AdminLoginDto loginDto)
        {
            AppUser? user = _userManager.FindByNameAsync(loginDto.UserName).Result;

            if (user == null || !_userManager.CheckPasswordAsync(user, loginDto.Password).Result)
            {
                throw new RestException(StatusCodes.Status401Unauthorized, "UserName or Password incorrect!");
            }

            if (user.IsPasswordResetRequired)
            {
               
                string resetToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                return new SendingLoginDto { Token = resetToken, PasswordResetRequired = true };
            }

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim("FullName", user.FullName));

            var roles = _userManager.GetRolesAsync(user).Result;

            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList());

            string secret = _configuration.GetSection("JWT:Secret").Value;

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                issuer: _configuration.GetSection("JWT:Issuer").Value,
                audience: _configuration.GetSection("JWT:Audience").Value,
                expires: DateTime.Now.AddDays(2)
            );

            string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return new SendingLoginDto { Token = tokenStr, PasswordResetRequired = false };
        }




     













        private async Task<string> GenerateJwtToken(AppUser user)
        {
            var claims = new List<Claim>
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim("FullName", user.FullName)
         };

            
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            
            var secret = _configuration.GetSection("JWT:Secret").Value;
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

          
            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("JWT:Issuer").Value,
                audience: _configuration.GetSection("JWT:Audience").Value,
                claims: claims,
                expires: DateTime.Now.AddDays(1),  
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }













        public void Update(string id, AdminUpdateDto updateDto)
        {
            
            var user = _userManager.FindByIdAsync(id).Result;

            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var existingUser = _userManager.FindByNameAsync(updateDto.UserName).Result;
            if (existingUser != null && existingUser.Id != user.Id)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "UserName", "UserName already taken");
            }

            user.UserName = updateDto.UserName;

            if (!string.IsNullOrEmpty(updateDto.CurrentPassword) && !string.IsNullOrEmpty(updateDto.NewPassword))
            {
               
                var passwordCheck = _userManager.CheckPasswordAsync(user, updateDto.CurrentPassword).Result;
                if (!passwordCheck)
                {
                    throw new RestException(StatusCodes.Status400BadRequest,"CurrentPassword", "Current password is incorrect.");
                }

              
                if (updateDto.NewPassword != updateDto.ConfirmPassword)
                {
                    throw new RestException(StatusCodes.Status400BadRequest, "New password and confirm password do not match.");
                }
               
                var changePasswordResult = _userManager.ChangePasswordAsync(user, updateDto.CurrentPassword, updateDto.NewPassword).Result;

                if (!changePasswordResult.Succeeded)
                {
                    var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                    throw new RestException(StatusCodes.Status400BadRequest, $"Failed to change password: {errors}");
                }
                user.IsPasswordResetRequired = false;
            }
           
            var updateResult = _userManager.UpdateAsync(user).Result;

            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to update user: {errors}");
            }
        }













        public async Task UpdatePasswordAsync(AdminUpdateDto updatePasswordDto)
        {
            var user = await _userManager.FindByNameAsync(updatePasswordDto.UserName);
            if (user == null)
            {
                throw new RestException(StatusCodes.Status404NotFound, "User not found.");
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, updatePasswordDto.CurrentPassword);
            if (!passwordCheck)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "Current password is incorrect.");
            }

            if (updatePasswordDto.NewPassword != updatePasswordDto.ConfirmPassword)
            {
                throw new RestException(StatusCodes.Status400BadRequest, "New password and confirm password do not match.");
            }

            var changePasswordResult = await _userManager.ChangePasswordAsync(user, updatePasswordDto.CurrentPassword, updatePasswordDto.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                var errors = string.Join(", ", changePasswordResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to change password: {errors}");
            }

            user.IsPasswordResetRequired = false;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                throw new RestException(StatusCodes.Status400BadRequest, $"Failed to update user: {errors}");
            }
        }
       

    }
}

