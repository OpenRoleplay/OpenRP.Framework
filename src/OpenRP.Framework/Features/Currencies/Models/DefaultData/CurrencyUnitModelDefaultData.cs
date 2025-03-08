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
    public class CurrencyUnitModelDefaultData : IEntityTypeConfiguration<CurrencyModel>
    {
        public void Configure(EntityTypeBuilder<CurrencyModel> builder)
        {
            builder.HasData(
                // San Andreas Dollar
                new CurrencyUnitModel
                {
                    Id = 1,
                    CurrencyId = 1,
                    UnitValue = 0.01m // Copper
                },
                new CurrencyUnitModel
                {
                    Id = 2,
                    CurrencyId = 1,
                    UnitValue = 0.05m // Nick
                },
                new CurrencyUnitModel
                {
                    Id = 3,
                    CurrencyId = 1,
                    UnitValue = 0.10m // Dime
                },
                new CurrencyUnitModel
                {
                    Id = 4,
                    CurrencyId = 1,
                    UnitValue = 0.25m // Quarter
                },
                new CurrencyUnitModel
                {
                    Id = 5,
                    CurrencyId = 1,
                    UnitValue = 0.50m // Half Dollar
                },
                new CurrencyUnitModel
                {
                    Id = 6,
                    CurrencyId = 1,
                    UnitValue = 1.00m // Dollar Coin
                },
                new CurrencyUnitModel
                {
                    Id = 7,
                    CurrencyId = 1,
                    UnitValue = 2.00m // $2 Bill
                },
                new CurrencyUnitModel
                {
                    Id = 8,
                    CurrencyId = 1,
                    UnitValue = 5.00m // $5 Bill
                },
                new CurrencyUnitModel
                {
                    Id = 9,
                    CurrencyId = 1,
                    UnitValue = 10.00m // $10 Bill
                },
                new CurrencyUnitModel
                {
                    Id = 10,
                    CurrencyId = 1,
                    UnitValue = 20.00m // $20 Bill
                },
                new CurrencyUnitModel
                {
                    Id = 11,
                    CurrencyId = 1,
                    UnitValue = 50.00m // $50 Bill
                },
                new CurrencyUnitModel
                {
                    Id = 12,
                    CurrencyId = 1,
                    UnitValue = 100.00m // $100 Bill
                },
                new CurrencyUnitModel
                {
                    Id = 13,
                    CurrencyId = 1,
                    UnitValue = 500.00m // $500 Bill
                },
                new CurrencyUnitModel
                {
                    Id = 14,
                    CurrencyId = 1,
                    UnitValue = 1000.00m // $1,000 Bill
                },
                new CurrencyUnitModel
                {
                    Id = 15,
                    CurrencyId = 1,
                    UnitValue = 10000.00m // $10,000 Bill
                },
                // Caeroyna Ruble
                new CurrencyUnitModel
                {
                    Id = 16,
                    CurrencyId = 2,
                    UnitValue = 0.01m
                },
                new CurrencyUnitModel
                {
                    Id = 17,
                    CurrencyId = 2,
                    UnitValue = 0.05m
                },
                new CurrencyUnitModel
                {
                    Id = 18,
                    CurrencyId = 2,
                    UnitValue = 0.10m
                },
                new CurrencyUnitModel
                {
                    Id = 19,
                    CurrencyId = 2,
                    UnitValue = 0.50m
                },
                new CurrencyUnitModel
                {
                    Id = 20,
                    CurrencyId = 2,
                    UnitValue = 1.00m
                },
                new CurrencyUnitModel
                {
                    Id = 21,
                    CurrencyId = 2,
                    UnitValue = 2.00m
                },
                new CurrencyUnitModel
                {
                    Id = 22,
                    CurrencyId = 2,
                    UnitValue = 5.00m
                },
                new CurrencyUnitModel
                {
                    Id = 23,
                    CurrencyId = 2,
                    UnitValue = 10.00m
                },
                new CurrencyUnitModel
                {
                    Id = 24,
                    CurrencyId = 2,
                    UnitValue = 50.00m
                },
                new CurrencyUnitModel
                {
                    Id = 25,
                    CurrencyId = 2,
                    UnitValue = 100.00m
                },
                new CurrencyUnitModel
                {
                    Id = 26,
                    CurrencyId = 2,
                    UnitValue = 200.00m
                },
                new CurrencyUnitModel
                {
                    Id = 27,
                    CurrencyId = 2,
                    UnitValue = 500.00m
                },
                new CurrencyUnitModel
                {
                    Id = 28,
                    CurrencyId = 2,
                    UnitValue = 1000.00m
                },
                new CurrencyUnitModel
                {
                    Id = 29,
                    CurrencyId = 2,
                    UnitValue = 5000.00m
                }
            );
        }
    }
}
