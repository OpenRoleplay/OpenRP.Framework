using OpenRP.Framework.Database.Models;
using OpenRP.Framework.Shared.BaseManager.Entities;
using SampSharp.Entities;

namespace OpenRP.Framework.Features.Jurisdictions.Components
{
    /// <summary>
    /// A place with its own rules: a country, a state or a city. Its parent is the jurisdiction it lies in.
    /// </summary>
    public class Jurisdiction : Component, IBaseDataComponent, IChangeable, IDeletable
    {
        private readonly JurisdictionModel _jurisdictionModel;
        private bool _hasChanges;
        private bool _isDeleted;

        public Jurisdiction(JurisdictionModel jurisdictionModel)
        {
            _jurisdictionModel = jurisdictionModel;
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

        public JurisdictionModel GetRawJurisdictionModel()
        {
            return _jurisdictionModel;
        }

        public ulong GetId()
        {
            return _jurisdictionModel.Id;
        }

        public string GetName()
        {
            return _jurisdictionModel.Name;
        }

        public void SetName(string name)
        {
            _jurisdictionModel.Name = name;
            ProcessChanges();
        }

        /// <summary>
        /// The two-letter tag, for example SA. A country's is used on licence plates.
        /// </summary>
        public string GetTwoLetterTag()
        {
            return _jurisdictionModel.TwoLetterTag;
        }

        public string GetThreeLetterTag()
        {
            return _jurisdictionModel.ThreeLetterTag;
        }

        public string GetFourLetterTag()
        {
            return _jurisdictionModel.FourLetterTag;
        }

        /// <summary>
        /// The three tags together, for example SA/SAN/USSA.
        /// </summary>
        public string GetTags()
        {
            return $"{GetTwoLetterTag()}/{GetThreeLetterTag()}/{GetFourLetterTag()}";
        }

        public void SetTags(string twoLetterTag, string threeLetterTag, string fourLetterTag)
        {
            _jurisdictionModel.TwoLetterTag = twoLetterTag;
            _jurisdictionModel.ThreeLetterTag = threeLetterTag;
            _jurisdictionModel.FourLetterTag = fourLetterTag;
            ProcessChanges();
        }

        /// <summary>
        /// The jurisdiction this one lies in, or null for a country.
        /// </summary>
        public ulong? GetParentJurisdictionId()
        {
            return _jurisdictionModel.ParentJurisdictionId;
        }

        public bool IsCountry()
        {
            return !_jurisdictionModel.ParentJurisdictionId.HasValue;
        }

        public void SetParentJurisdictionId(ulong? parentJurisdictionId)
        {
            _jurisdictionModel.ParentJurisdictionId = parentJurisdictionId;
            ProcessChanges();
        }

        /// <summary>
        /// The currency of its own, or null when it uses its parent's.
        /// </summary>
        public ulong? GetCurrencyId()
        {
            return _jurisdictionModel.CurrencyId;
        }

        public void SetCurrencyId(ulong? currencyId)
        {
            _jurisdictionModel.CurrencyId = currencyId;
            ProcessChanges();
        }

        /// <summary>
        /// The faction that governs it, or null when its parent's government governs it.
        /// </summary>
        public ulong? GetGovernmentFactionId()
        {
            return _jurisdictionModel.GovernmentFactionId;
        }

        public void SetGovernmentFactionId(ulong? governmentFactionId)
        {
            _jurisdictionModel.GovernmentFactionId = governmentFactionId;
            ProcessChanges();
        }
    }
}
