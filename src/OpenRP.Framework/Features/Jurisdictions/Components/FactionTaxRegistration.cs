using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Jurisdictions.Components
{
    /// <summary>
    /// A faction registered in a jurisdiction, where it pays taxes.
    /// </summary>
    public class FactionTaxRegistration : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly FactionTaxRegistrationModel _registrationModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public FactionTaxRegistration(FactionTaxRegistrationModel registrationModel)
        {
            _registrationModel = registrationModel;
        }

        public bool HasChanges()
        {
            return _hasChanges;
        }

        public void ProcessChanges(bool processChanges = true)
        {
            _hasChanges = processChanges;
        }

        public bool IsDeleted()
        {
            return _isDeleted;
        }

        public void ProcessDeletion(bool processDeletion = true)
        {
            _isDeleted = processDeletion;
        }

        public FactionTaxRegistrationModel GetRawFactionTaxRegistrationModel()
        {
            return _registrationModel;
        }

        public ulong GetId()
        {
            return _registrationModel.Id;
        }

        public ulong GetFactionId()
        {
            return _registrationModel.FactionId;
        }

        public ulong GetJurisdictionId()
        {
            return _registrationModel.JurisdictionId;
        }
    }
}
