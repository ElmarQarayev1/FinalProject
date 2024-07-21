using System;
using Medical.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medical.Data.Configuration
{
	public class MedicalimageConfiguration : IEntityTypeConfiguration<MedicineImage>
    {
		
        public void Configure(EntityTypeBuilder<MedicineImage> builder)
        {
            builder.HasOne(x => x.Medicine).WithMany(s => s.MedicineImages).HasForeignKey(x => x.MedicineId);
        }
    }
}

