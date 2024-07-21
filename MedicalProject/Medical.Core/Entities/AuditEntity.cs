using System;
namespace Medical.Core.Entities
{
	public class AuditEntity:BaseEntity
	{
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; }
    }
}

