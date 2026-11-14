using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Jurisdictions.Entities
{
    public static class JurisdictionEntities
    {
        public static readonly Guid JurisdictionType = new Guid("8E2AB852-5A0D-49BE-A67F-9933830DCDA3");

        public static readonly Guid FactionTaxRegistrationType = new Guid("89E2188C-0C54-49B9-922C-8D1F1298895E");

        public static readonly Guid JurisdictionSettingType = new Guid("66348A08-D2C4-4977-A51A-DBB2513A0425");

        public static EntityId GetJurisdictionId(int jurisdictionId)
        {
            return RowEntityIds.Get(JurisdictionType, jurisdictionId);
        }

        public static EntityId GetFactionTaxRegistrationId(int factionTaxRegistrationId)
        {
            return RowEntityIds.Get(FactionTaxRegistrationType, factionTaxRegistrationId);
        }
    }
}
