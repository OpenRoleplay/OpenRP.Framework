using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Radios.Entities
{
    public static class RadioEntities
    {
        /// <summary>
        /// The key in a handheld radio item's additional data that holds its radio id: [RADIO_ID=x].
        /// </summary>
        public const string RadioIdKey = "RADIO_ID";

        public static readonly Guid RadioType = new Guid("5E0C2F7A-3B19-4D6E-9A41-7C82D0E5B163");

        public static readonly Guid RadioFrequencyType = new Guid("A1D74E28-6F0B-4C93-8E57-2B9F14C6D380");

        public static readonly Guid RadioFrequencyFactionType = new Guid("3C9B61E4-D825-47A0-B316-E4F07A92C5D1");

        public static EntityId GetRadioId(int radioId)
        {
            return RowEntityIds.Get(RadioType, radioId);
        }

        public static EntityId GetRadioFrequencyId(int radioFrequencyId)
        {
            return RowEntityIds.Get(RadioFrequencyType, radioFrequencyId);
        }

        public static EntityId GetRadioFrequencyFactionId(int radioFrequencyFactionId)
        {
            return RowEntityIds.Get(RadioFrequencyFactionType, radioFrequencyFactionId);
        }
    }
}
