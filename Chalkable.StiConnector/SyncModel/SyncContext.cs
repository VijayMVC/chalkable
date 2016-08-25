using System;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.StiConnector.SyncModel
{
    public class SyncContext
    {
        public IDictionary<string, long?> TablesToSync { get; private set; }
        public IDictionary<string, Type> Types { get; private set; }
        private IDictionary<string, object> results; 

        private void RegisterType(Type type)
        {
            if (!type.IsSubclassOf(typeof(SyncModel)))
                throw new Exception("Sync model class should be inherited from SyncModel");
            TablesToSync.Add(type.Name, null);
            Types.Add(type.Name, type);
        }
            
        public SyncContext()
        {
            TablesToSync = new Dictionary<string, long?>();
            results = new Dictionary<string, object>();
            Types = new Dictionary<string, Type>();
            
            RegisterType(typeof(School));
            RegisterType(typeof(SchoolOption));
            RegisterType(typeof(User));
            RegisterType(typeof(Address));
            RegisterType(typeof(Person));
            RegisterType(typeof(Student));
            RegisterType(typeof(Staff));
            RegisterType(typeof(StudentSchool));
            RegisterType(typeof(PersonTelephone));
            RegisterType(typeof(GradeLevel));
            RegisterType(typeof(AcadSession));
            RegisterType(typeof(StudentAcadSession));
            RegisterType(typeof(StudentScheduleTerm));
            RegisterType(typeof(Term));
            RegisterType(typeof(GradingPeriod));
            RegisterType(typeof(DayType));
            RegisterType(typeof(CalendarDay));
            RegisterType(typeof(Room));
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
            RegisterType(typeof(StudentCustomAlertDetail));
            RegisterType(typeof(District));
            RegisterType(typeof(StandardizedTest));
            RegisterType(typeof(StandardizedTestComponent));
            RegisterType(typeof(StandardizedTestScoreType));
            RegisterType(typeof(Ethnicity));
            RegisterType(typeof(PersonEthnicity));

            RegisterType(typeof(Language));
            RegisterType(typeof(PersonLanguage));
            RegisterType(typeof(Country));
            RegisterType(typeof(PersonNationality));

            RegisterType(typeof(Homeroom));
            
            RegisterType(typeof(AppSetting));
        }

        public void SetCurrentVersions(IList<SyncVersion> tableVersions)
        {
            foreach (var tableVersion in tableVersions)
            {
                if (TablesToSync.ContainsKey(tableVersion.TableName))
                {
                    TablesToSync[tableVersion.TableName] = tableVersion.Version;
                }
                else
                {
                    throw new Exception($"There is no such table {tableVersion.TableName} registered for sync");   
                }
            }
        }

        public void SetResult(SyncResultBase<SyncModel> result)
        {
            var tName = result.Name;
            results.Add(tName, result);
            TablesToSync[tName] = result.CurrentVersion;
        }

        public SyncResult<T> GetSyncResult<T>() where T : SyncModel
        {
            var tName = typeof (T).Name;
            return (SyncResult<T>)results[tName];
        }

        public SyncResultBase<SyncModel> GetSyncResult(Type type)
        {
            return (SyncResultBase<SyncModel>)results[type.Name];
        }
    }
    
    public interface SyncResultBase<out T> where T : SyncModel
    {
        long CurrentVersion { get; set; }
        string Name { get; }
        int RowCount { get; }
        T[] Updated { get; }
        T[] Deleted { get; }
        T[] All { get; }
    }

    public class SyncResult<T> : SyncResultBase<T> where T : SyncModel
    {
        public T[] Inserted { get; set; }
        public T[] Updated { get; set; }
        public T[] Deleted { get; set; }
        public T[] Rows { get; set; }
        public T[] All
        {
            get
            {
                if (Rows != null)
                    return Rows;
                if (Inserted != null)
                    return Inserted;
                return new T[0];
            }
        }
        public long CurrentVersion { get; set; }
        public string Name => typeof (T).Name;

        public int RowCount
        {
            get 
            { 
                var res = 0;
                if (All != null)
                    res += All.Length;
                if (Updated != null)
                    res += Updated.Length;
                if (Deleted != null)
                    res += Deleted.Length;
                return res;
            }
        }
    }
}
