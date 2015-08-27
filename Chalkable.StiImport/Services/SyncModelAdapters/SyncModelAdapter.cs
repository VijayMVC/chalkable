using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.StiConnector.SyncModel;

namespace Chalkable.StiImport.Services.SyncModelAdapters
{
    public interface ISyncModelAdapter<out TSyncModel> where TSyncModel : SyncModel
    {
        void Persist(PersistOperationType type, IList entities);
    }
    public abstract class SyncModelAdapter<TSyncModel> : ISyncModelAdapter<TSyncModel> where TSyncModel : SyncModel
    {
        protected AdapterLocator Locator { get; }
        protected IServiceLocatorMaster MasterLocator => Locator.MasterLocator;
        protected IServiceLocatorSchool SchoolLocator => Locator.SchoolLocator;

        private readonly Dictionary<PersistOperationType, Action<IList<TSyncModel>>> persistActions = new Dictionary<PersistOperationType, Action<IList<TSyncModel>>>();
        
        public SyncModelAdapter(AdapterLocator locator)
        {
            Locator = locator;
            persistActions.Add(PersistOperationType.Insert, Insert);
            persistActions.Add(PersistOperationType.Update, Update);
            persistActions.Add(PersistOperationType.Delete, Delete);
        }

        
        public void Persist(PersistOperationType type, IList entities)
        {
            persistActions[type]((IList<TSyncModel>)entities);
        }

        private void Insert(IList<TSyncModel> entities)
        {
            InsertInternal(entities);
        }

        private void Update(IList<TSyncModel> entities)
        {
            if (entities != null)
                UpdateInternal(entities);
        }

        private void Delete(IList<TSyncModel> entities)
        {
            if (entities != null)
                DeleteInternal(entities);
        }

        protected abstract void InsertInternal(IList<TSyncModel> entities);
        protected abstract void UpdateInternal(IList<TSyncModel> entities);
        protected abstract void DeleteInternal(IList<TSyncModel> entities);
    }

    public class AdapterLocator
    {
        public IServiceLocatorMaster MasterLocator { get; }
        public IServiceLocatorSchool SchoolLocator { get; }

        private Dictionary<Type, ISyncModelAdapter<SyncModel>> allAdapters;

        public List<Guid> InsertedUserIds;

        public IDictionary<short, string> GetnderMapping {get; }

        public IDictionary<int, string> SpEdStatusMapping { get; }

        public AdapterLocator(IServiceLocatorMaster masterLocator, IServiceLocatorSchool schoolLocator, IEnumerable<Gender> genders, IEnumerable<SpEdStatus> spEdStatuses)
        {
            GetnderMapping = genders.ToDictionary(x => x.GenderID, x => x.Code);
            SpEdStatusMapping = spEdStatuses.ToDictionary(x => x.SpEdStatusID, x => x.Name);
            MasterLocator = masterLocator;
            SchoolLocator = schoolLocator;
            allAdapters = new Dictionary<Type, ISyncModelAdapter<SyncModel>>
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
            };
        }
        /*
                    
            RegisterType(typeof(Course));
            RegisterType(typeof(StandardSubject));
            RegisterType(typeof(Standard));
            RegisterType(typeof(CourseStandard));
            RegisterType(typeof(SectionTerm));
            RegisterType(typeof(TimeSlot));
            RegisterType(typeof(ScheduledSection));
            RegisterType(typeof(AbsenceReason));
            RegisterType(typeof(AbsenceLevelReason));
            RegisterType(typeof(AlphaGrade));
            RegisterType(typeof(AlternateScore));
            RegisterType(typeof(ScheduledTimeSlot));
            RegisterType(typeof(Gender));
            RegisterType(typeof(SpEdStatus));
            RegisterType(typeof(Infraction));
            RegisterType(typeof(ClassroomOption));
            RegisterType(typeof(GradingScale));
            RegisterType(typeof(GradingScaleRange));
            RegisterType(typeof(GradingComment));
            RegisterType(typeof(SectionStaff));
            RegisterType(typeof(UserSchool));
            RegisterType(typeof(PersonEmail));
            RegisterType(typeof(StaffSchool));
            RegisterType(typeof(BellSchedule));
            RegisterType(typeof(ScheduledTimeSlotVariation));
            RegisterType(typeof(SectionTimeSlotVariation));
            RegisterType(typeof(AttendanceMonth));
            RegisterType(typeof(GradedItem));
            RegisterType(typeof(ActivityAttribute));
            RegisterType(typeof(ContactRelationship));
            RegisterType(typeof(StudentContact));
            RegisterType(typeof(CourseType));
            RegisterType(typeof(SystemSetting));
        */

        public ISyncModelAdapter<SyncModel> GetAdapter(Type type)
        {
            if (allAdapters.ContainsKey(type))
                return allAdapters[type];
            throw new Exception($"can't find adapter for model {type.Name}");
        }
    }

    public enum PersistOperationType
    {
        Insert = 0,
        Update = 1,
        Delete = 2
    }
}
