using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;
using System;

namespace OpenRP.Framework.Features.Phones.Entities
{
    public static class PhoneEntities
    {
        public static readonly Guid PhoneType = new Guid("CCD84B27-6AD7-4ABE-8EF7-0D08717CD040");

        public static readonly Guid PhoneContactType = new Guid("973528A8-C347-4339-8154-93264FF91341");

        public static readonly Guid PhoneMessageType = new Guid("BD00F3E4-93E7-4E40-9406-2D1607B63E2E");

        public static readonly Guid PhoneCallLogType = new Guid("6BB977F6-EB49-4A23-BD91-9C85FD5AD488");

        public static EntityId GetPhoneId(int phoneId)
        {
            return RowEntityIds.Get(PhoneType, phoneId);
        }

        public static EntityId GetPhoneContactId(int phoneContactId)
        {
            return RowEntityIds.Get(PhoneContactType, phoneContactId);
        }

        public static EntityId GetPhoneMessageId(int phoneMessageId)
        {
            return RowEntityIds.Get(PhoneMessageType, phoneMessageId);
        }

        public static EntityId GetPhoneCallLogId(int phoneCallLogId)
        {
            return RowEntityIds.Get(PhoneCallLogType, phoneCallLogId);
        }
    }
}
