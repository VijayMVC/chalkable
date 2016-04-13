using System.Collections.Generic;
using System.Linq;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public class PersonTelephoneAdapter : SyncModelAdapter<PersonTelephone>
    {
        private const string DESCR_WORK = "Work";
        private const string DESCR_CELL = "cell";
        public PersonTelephoneAdapter(AdapterLocator locator) : base(locator)
        {

        }

        protected override void InsertInternal(IList<PersonTelephone> entities)
        {
            var ps = new List<Phone>();
            foreach (var pt in entities)
            {
                var type = PhoneType.Home;
                if (pt.Description == DESCR_WORK)
                    type = PhoneType.Work;
                if (pt.Description == DESCR_CELL)
                    type = PhoneType.Mobile;
                if (!string.IsNullOrEmpty(pt.FormattedTelephoneNumber))
                {
                    ps.Add(new Phone
                    {
                        Value = pt.FormattedTelephoneNumber,
                        DigitOnlyValue = pt.TelephoneNumber,
                        PersonRef = pt.PersonID,
                        IsPrimary = pt.IsPrimary,
                        Type = type
                    });
                }
            }
            ServiceLocatorSchool.PhoneService.AddPhones(ps);
        }

        protected override void UpdateInternal(IList<PersonTelephone> entities)
        {
            IList<Phone> ps = new List<Phone>();
            foreach (var pt in entities)
            {
                var type = PhoneType.Home;
                if (pt.Description == DESCR_WORK)
                    type = PhoneType.Work;
                if (pt.Description == DESCR_CELL)
                    type = PhoneType.Mobile;
                if (!string.IsNullOrEmpty(pt.FormattedTelephoneNumber))
                {
                    ps.Add(new Phone
                    {
                        DigitOnlyValue = pt.TelephoneNumber,
                        PersonRef = pt.PersonID,
                        IsPrimary = pt.IsPrimary,
                        Type = type,
                        Value = pt.FormattedTelephoneNumber
                    });
                }
            }
            ServiceLocatorSchool.PhoneService.EditPhones(ps);
        }

        protected override void DeleteInternal(IList<PersonTelephone> entities)
        {
            var phones = entities.Select(x => new Phone
            {
                DigitOnlyValue = x.TelephoneNumber,
                PersonRef = x.PersonID,
                IsPrimary = x.IsPrimary,
                Value = x.FormattedTelephoneNumber
            }).ToList();
            ServiceLocatorSchool.PhoneService.Delete(phones);
        }
    }
}