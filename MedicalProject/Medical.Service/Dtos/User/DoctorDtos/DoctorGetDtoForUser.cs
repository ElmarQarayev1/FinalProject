using System;
namespace Medical.Service.Dtos.User.DoctorDtos
{
	public class DoctorGetDtoForUser
	{
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Position { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string FileUrl { get; set; }

    }
}

