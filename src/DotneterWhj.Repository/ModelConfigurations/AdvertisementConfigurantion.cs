using DotneterWhj.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Repository
{
    class AdvertisementConfigurantion : IEntityTypeConfiguration<Advertisement>
    {
        public void Configure(EntityTypeBuilder<Advertisement> builder)
        {
            builder.ToTable("Advertisement");

            builder.Property(a => a.ImgUrl).HasMaxLength(255).IsRequired().IsUnicode();

            builder.Property(a => a.Remark).HasMaxLength(255).IsRequired();

            builder.Property(a => a.Title).HasMaxLength(255).IsRequired();

            builder.Property(a => a.Url).HasMaxLength(255).IsRequired().IsUnicode();

            builder.Property(a => a.CreateTime).HasColumnType("datetime");
        }
    }
}
