using OpenRP.Framework.Features.Phones.Components;
using OpenRP.Framework.Features.Phones.Enums;
using SampSharp.Entities.SAMP;
using System;

namespace OpenRP.Framework.Features.Phones.Services
{
    /// <summary>
    /// Creating phones and changing what's in them. Changes are saved with the next phone sync. Methods that can fail
    /// return the reason, or null.
    /// </summary>
    public interface IPhoneEditorService
    {
        /// <summary>
        /// Creates a phone with a new random 7-digit number and puts it in the player's character's inventory.
        /// </summary>
        (Phone? Phone, string? Error) GivePhone(Player player);

        /// <summary>
        /// Turns the phone on or off. Turning it on beeps for texts that came in while it was off.
        /// </summary>
        void SetPower(Phone phone, bool isOn);

        void SetSilent(Phone phone, bool isSilent);

        /// <summary>
        /// Adds a name to the phone book. Refused when it's full or the number is in it already.
        /// </summary>
        string? AddContact(Phone phone, string name, string numberText);

        string? RenameContact(PhoneContact contact, string name);

        void DeleteContact(PhoneContact contact);

        /// <summary>
        /// Sends a text from the phone. The sender keeps a copy; the receiving phone gets it even when it's off, and
        /// beeps when it's on.
        /// </summary>
        string? SendText(Phone phone, uint number, string text);

        void SetMessageRead(PhoneMessage message);

        void DeleteMessage(PhoneMessage message);

        /// <summary>
        /// Writes a call into the phone's call log, dropping the oldest when it's full.
        /// </summary>
        void AddCallLog(Phone phone, uint otherNumber, PhoneCallLogType type, DateTime startedAt, int durationSeconds);

        /// <summary>
        /// Marks the missed calls as seen, for when the call log is opened.
        /// </summary>
        void SetMissedCallsSeen(Phone phone);
    }
}
