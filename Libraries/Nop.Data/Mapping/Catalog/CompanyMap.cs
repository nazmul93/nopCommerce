using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Data.Mapping.Catalog
{
    public partial class CompanyMap : NopEntityTypeConfiguration<Company>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable(nameof(Company));
            builder.HasKey(company => company.Id);

            builder.Property(company => company.Name).HasMaxLength(400).IsRequired();
            builder.Property(company => company.Description).HasMaxLength(400);

            base.Configure(builder);
        }

        #endregion
    }
}
