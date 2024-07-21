using System;
using Medical.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Medical.Data.Configuration
{
	public class CategoryConfiguration: IEntityTypeConfiguration<Category>
    {
		
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(30).IsRequired(true);

            builder.HasKey(x => x.Id);
        }
    }
}

