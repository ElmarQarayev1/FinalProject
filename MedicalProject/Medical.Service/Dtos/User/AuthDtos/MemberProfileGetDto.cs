using System;
using Medical.Service.Dtos.User.OrderDtos;

namespace Medical.Service.Dtos.User.AuthDtos
{
    public class MemberProfileGetDto
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string FullName { get; set; }

        public bool HasPassword { get; set; }

        public bool IsGoogleUser { get; set; }

        public List<OrderGetDtoForUserProfile> Orders = new List<OrderGetDtoForUserProfile>();

    }
}

