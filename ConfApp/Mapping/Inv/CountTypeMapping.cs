using Domain.Objects.Shop;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfApp.Mapping.Inv
{
    public class CountTypeMapping : IEntityTypeConfiguration<CountType>         
    {           

                public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<CountType> builder)
                {
                    builder.ToTable("CountTypes");
                
                    builder.HasKey(x => x.Id);
                    builder.HasIndex(x => x.Id);
                    builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
                
                }
    }           
                
}