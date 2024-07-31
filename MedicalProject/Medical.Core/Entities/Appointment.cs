using System;
using System.ComponentModel.DataAnnotations;

namespace Medical.Core.Entities
{
	public class Appointment:BaseEntity
	{
        public string? AppUserId { get; set; }

        public int DoctorId { get; set; }
           
        public string FullName { get; set; }

        public string Phone { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ModifiedAt { get; set; }

        public AppUser? AppUser { get; set; }

        public Doctor Doctor { get; set; }

        public DateTime Date { get; set; }

    }
}

