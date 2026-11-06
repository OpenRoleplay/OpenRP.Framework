using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRP.Framework.Database.Entities;
using System;

namespace OpenRP.Framework.Database.Extensions
{
    public static class EntityTypeBuilderExtensions
    {
        // Seed rows need fixed values: BaseModel stamps new models with the current time, and a changing
        // timestamp would make every new migration update every seed row again.
        private static readonly DateTime DefaultDataTimestamp = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Seeds these rows like HasData, with fixed CreatedOn/UpdatedOn timestamps. Use this instead of HasData.
        /// </summary>
        public static DataBuilder<TModel> HasDefaultData<TModel>(this EntityTypeBuilder<TModel> builder, params TModel[] rows)
            where TModel : BaseModel
        {
            foreach (TModel row in rows)
            {
                row.CreatedOn = DefaultDataTimestamp;
                row.UpdatedOn = DefaultDataTimestamp;
            }
            return builder.HasData(rows);
        }
    }
}
