using System;
using Medical.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medical.Data.Configuration
{
	public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
		

        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder.Property(x => x.Value).HasMaxLength(30).IsRequired(true);

            builder.HasKey(x => x.Key);
        }
    }
}

