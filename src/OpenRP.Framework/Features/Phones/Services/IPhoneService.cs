using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Entities;
using SampSharp.Entities.SAMP;
using System.Collections.Generic;

namespace OpenRP.Framework.Features.Phones.Services
{
    /// <summary>
    /// Reading phones: who carries them, their phone books, texts and call logs.
    /// </summary>
    public interface IPhoneService
    {
        /// <summary>
        /// The loaded phone with this id, or null.
        /// </summary>
        Phone? GetPhone(ulong phoneId);

        /// <summary>
        /// The loaded phone with this number, or null.
        /// </summary>
        Phone? GetPhoneByNumber(uint number);

        bool IsNumberTaken(uint number);

        /// <summary>
        /// The server number with this number, such as 911, or null.
        /// </summary>
        IServerPhoneNumber? GetServerNumber(uint number);

        /// <summary>
        /// The online player whose character has the phone in their inventory, or null.
        /// </summary>
        Player? GetCarrier(Phone phone);

        /// <summary>
        /// The phones in the player's character's inventory.
        /// </summary>
        List<Phone> GetCarriedPhones(Player player);

        bool IsCarrying(Player player, Phone phone);

        /// <summary>
        /// The phone commands like /call use: the first carried phone that's on, otherwise the first one. Null when
        /// the player carries none.
        /// </summary>
        Phone? GetPhoneToUse(Player player);

        /// <summary>
        /// How the phone shows a number: the contact's name when it's in the phone book, the server number's name,
        /// otherwise the number itself.
        /// </summary>
        string GetNumberLabel(Phone phone, uint number);

        /// <summary>
        /// The phone book, by name.
        /// </summary>
        List<PhoneContact> GetContacts(Phone phone);

        /// <summary>
        /// The texts in the phone's memory, newest first.
        /// </summary>
        List<PhoneMessage> GetMessages(Phone phone);

        int GetUnreadMessageCount(Phone phone);

        /// <summary>
        /// The calls in the phone's call log, newest first.
        /// </summary>
        List<PhoneCallLog> GetCallLogs(Phone phone);

        int GetNewMissedCallCount(Phone phone);

        /// <summary>
        /// The last number the phone dialed, or null.
        /// </summary>
        uint? GetLastDialedNumber(Phone phone);

        /// <summary>
        /// A short line such as "2 missed calls, 1 new message", or null when there's nothing new.
        /// </summary>
        string? GetNewsSummary(Phone phone);
    }
}
