using System;
namespace Medical.Core.Entities
{
	public class Doctor:AuditEntity
	{
		public string Name { get; set; }

		public string Position { get; set; }

		public int DepartmentId { get; set; }

		public string Desc { get; set; }

		public string Address { get; set; }

		public string Email { get; set; }

		public string Phone { get; set; }

        public string ImageName { get; set; }

        public string TwitterUrl { get; set; }

		public string InstagramUrl { get; set; }

		public string VimeoUrl { get; set; }

		public string BehanceUrl { get; set; }

		public string ResilienceSkil { get; set; }

		public string EthicSkil { get; set; }

		public string Compassion { get; set; }

		public Department Department { get; set; }

		public List<Appointment>? Appointments { get; set; }
    }

}

