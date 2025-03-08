using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using OpenRP.Framework.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenRP.Framework.Features.Currencies.Models.DefaultData
{
    public class CurrencyModelDefaultData : IEntityTypeConfiguration<CurrencyModel>
    {
        public void Configure(EntityTypeBuilder<CurrencyModel> builder)
        {
            builder.HasData(
                new CurrencyModel
                {
                    Id = 1
                    , CurrencyCode = "SAD"
                    , Name = "San Andreas Dollar"
                },

                new CurrencyModel
                {
                    Id = 2
                    , CurrencyCode = "CRU"
                    , Name = "Caeroyna Ruble"
                },

                new CurrencyModel
                {
                    Id = 3
                    , CurrencyCode = "USD"
                    , Name = "United States Dollar"
                }
            );
        }
    }
}
