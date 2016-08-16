using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public interface ISyncModelAdapter
    {
        void Persist(PersistOperationType type, IList<SyncModel> entities);
    }
    public abstract class SyncModelAdapter<TSyncModel> : ISyncModelAdapter where TSyncModel : SyncModel
    {
        protected AdapterLocator Locator { get; }
        protected IServiceLocatorMaster ServiceLocatorMaster => Locator.MasterLocator;
        protected IServiceLocatorSchool ServiceLocatorSchool => Locator.SchoolLocator;

        private readonly Dictionary<PersistOperationType, Action<IList<TSyncModel>>> persistActions = new Dictionary<PersistOperationType, Action<IList<TSyncModel>>>();
        
        public SyncModelAdapter(AdapterLocator locator)
        {
            Locator = locator;
            persistActions.Add(PersistOperationType.Insert, Insert);
            persistActions.Add(PersistOperationType.Update, Update);
            persistActions.Add(PersistOperationType.Delete, Delete);
            persistActions.Add(PersistOperationType.PrepareToDelete, PrepareToDelete);
        }

        
        public void Persist(PersistOperationType type, IList<SyncModel> entities)
        {
            persistActions[type](entities.Cast<TSyncModel>().ToList());
        }

        private void Insert(IList<TSyncModel> entities)
        {
            InsertInternal(entities);
        }

        private void Update(IList<TSyncModel> entities)
        {
            UpdateInternal(entities);
        }

        private void Delete(IList<TSyncModel> entities)
        {
            DeleteInternal(entities);
        }

        private void PrepareToDelete(IList<TSyncModel> entities)
        {
            PrepareToDeleteInternal(entities);
        }

        protected abstract void InsertInternal(IList<TSyncModel> entities);
        protected abstract void UpdateInternal(IList<TSyncModel> entities);
        protected abstract void DeleteInternal(IList<TSyncModel> entities);
        protected abstract void PrepareToDeleteInternal(IList<TSyncModel> entities);
    }

    public class AdapterLocator
    {
        public IServiceLocatorMaster MasterLocator { get; }
        public IServiceLocatorSchool SchoolLocator { get; }

        private Dictionary<Type, ISyncModelAdapter> allAdapters;

        public List<Guid> InsertedUserIds;

        public IDictionary<short, string> GenderMapping {get; }

        public IDictionary<int, string> SpEdStatusMapping { get; }

        public AdapterLocator(IServiceLocatorMaster masterLocator, IServiceLocatorSchool schoolLocator, IEnumerable<Gender> genders, IEnumerable<SpEdStatus> spEdStatuses)
        {
            GenderMapping = genders.ToDictionary(x => x.GenderID, x => x.Code);
            SpEdStatusMapping = spEdStatuses.ToDictionary(x => x.SpEdStatusID, x => x.Name);
            MasterLocator = masterLocator;
            SchoolLocator = schoolLocator;
            allAdapters = new Dictionary<Type, ISyncModelAdapter>
            {
                {typeof(School), new SchoolAdapter(this) },
                {typeof(SchoolOption), new SchoolOptionAdapter(this) },
                {typeof(User), new UserAdapter(this) },
                {typeof(Address), new AddressAdapter(this) },
                {typeof(Person), new PersonAdapter(this) },
                {typeof(Student), new StudentAdapter(this) },
                {typeof(Staff), new StaffAdapter(this) },
                {typeof(StudentSchool), new StudentSchoolAdapter(this) },
                {typeof(PersonTelephone), new PersonTelephoneAdapter(this) },
                {typeof(GradeLevel), new GradeLevelAdapter(this) },
                {typeof(AcadSession), new AcadSessionAdapter(this) },
                {typeof(StudentAcadSession), new StudentAcadSessionAdapter(this) },
                {typeof(StudentScheduleTerm), new StudentScheduleTermAdapter(this) },
                {typeof(Term), new TermAdapter(this) },
                {typeof(GradingPeriod), new GradingPeriodAdapter(this) },
                {typeof(DayType), new DayTypeAdapter(this) },
                {typeof(CalendarDay), new CalendarDayAdapter(this) },
                {typeof(Room), new RoomAdapter(this) },
                {typeof(Course), new CourseAdapter(this) },
                {typeof(StandardSubject), new StandardSubjectAdapter(this) },
                {typeof(Standard), new StandardAdapter(this) },
                {typeof(CourseStandard), new CourseStandardAdapter(this) },
                {typeof(SectionTerm), new SectionTermAdapter(this) },
                {typeof(TimeSlot), new TimeSlotAdapter(this) },
                {typeof(ScheduledSection), new ScheduledSectionAdapter(this) },
                {typeof(AbsenceReason), new AbsenceReasonAdapter(this) },
                {typeof(AbsenceLevelReason), new AbsenceLevelReasonAdapter(this) },
                {typeof(AlphaGrade), new AlphaGradeAdapter(this) },
                {typeof(AlternateScore), new AlternateScoreAdapter(this) },
                {typeof(ScheduledTimeSlot), new ScheduledTimeSlotAdapter(this) },
                {typeof(Infraction), new InfractionAdapter(this) },
                {typeof(ClassroomOption), new ClassroomOptionAdapter(this) },
                {typeof(GradingScale), new GradingScaleAdapter(this) },
                {typeof(GradingScaleRange), new GradingScaleRangeAdapter(this) },
                {typeof(GradingComment), new GradingCommentAdapter(this) },
                {typeof(SectionStaff), new SectionStaffAdapter(this) },
                {typeof(UserSchool), new UserSchoolAdapter(this) },
                {typeof(PersonEmail), new PersonEmailAdapter(this) },
                {typeof(StaffSchool), new StaffSchoolAdapter(this) },
                {typeof(BellSchedule), new BellScheduleAdapter(this) },
                {typeof(ScheduledTimeSlotVariation), new ScheduledTimeSlotVariationAdapter(this) },
                {typeof(SectionTimeSlotVariation), new SectionTimeSlotVariationAdapter(this) },
                {typeof(AttendanceMonth), new AttendanceMonthAdapter(this) },
                {typeof(GradedItem), new GradedItemAdapter(this) },
                {typeof(ActivityAttribute), new ActivityAttributeAdapter(this) },
                {typeof(ContactRelationship), new ContactRelationshipAdapter(this) },
                {typeof(StudentContact), new StudentContactAdapter(this) },
                {typeof(CourseType), new CourseTypeAdapter(this) },
                {typeof(SystemSetting), new SystemSettingAdapter(this) },
                {typeof(StudentCustomAlertDetail), new StudentCustomAlertDetailAdapter(this) },
                {typeof(District), new DistrictAdapter(this) },
                {typeof(StandardizedTest), new StandardizedTestAdapter(this) },
                {typeof(StandardizedTestComponent), new StandardizedTestComponentAdapter(this) },
                {typeof(StandardizedTestScoreType), new StandardizedTestScoreTypeAdapter(this) },
                {typeof(Ethnicity), new EthnicityAdapter(this) },
                {typeof(PersonEthnicity), new PersonEthnicityAdapter(this) },
                {typeof(Language), new LanguageAdapter(this) },
                {typeof(PersonLanguage), new PersonLanguageAdapter(this) },
                {typeof(Country), new CountryAdapter(this) },
                {typeof(PersonNationality), new PersonNationalityAdapter(this) },
                {typeof(Homeroom), new HomeroomAdapter(this) },
                {typeof(AppSetting), new AppSettingAdapter(this) }
            };
        }

        public ISyncModelAdapter GetAdapter(Type type)
        {
            if (allAdapters.ContainsKey(type))
                return allAdapters[type];
            return null;
        }
    }

    public enum PersistOperationType
    {
        Insert = 0,
        Update = 1,
        Delete = 2,
        PrepareToDelete = 3
    }
}
