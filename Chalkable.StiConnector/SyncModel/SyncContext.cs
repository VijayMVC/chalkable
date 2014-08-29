using System;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.StiConnector.SyncModel
{
    public class SyncContext
    {
        public IDictionary<string, int?> TablesToSync { get; private set; }
        public IDictionary<string, Type> Types { get; private set; }
        private IDictionary<string, object> results; 

        private void RegisterType(Type type)
        {
            TablesToSync.Add(type.Name, null);
            Types.Add(type.Name, type);
        }
            
        public SyncContext()
        {
            TablesToSync = new Dictionary<string, int?>();
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
                    throw new Exception(string.Format("There is no such table {0} registered for sync", tableVersion.TableName));   
                }
            }
        }

        public void SetResult(SyncResultBase result)
        {
            var tName = result.Name;
            results.Add(tName, result);
            TablesToSync[tName] = result.CurrentVersion;
        }
         
        public SyncResult<T> GetSyncResult<T>()
        {
            var tName = typeof (T).Name;
            return (SyncResult<T>)results[tName];
        }
    }

    public abstract class SyncResultBase
    {
        public int CurrentVersion { get; set; }
        public abstract string Name { get; }
        public abstract int RowCount { get; }
    }

    public class SyncResult<T> : SyncResultBase
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
                return Inserted;
            }
        }


        public override string Name { get { return typeof (T).Name; } }

        public override int RowCount
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
