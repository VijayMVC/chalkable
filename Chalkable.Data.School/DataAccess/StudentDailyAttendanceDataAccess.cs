using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Data.Common;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentDailyAttendanceDataAccess : DataAccessBase<StudentDailyAttendance>
    {
        public StudentDailyAttendanceDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        private const string SET_DAILY_ATTENDANCE_PROC = "spSetDailyAttendance";
        private const string PERSON_ID_PARAM = "personId";
        private const string TIME_IN_PARAM = "timeIn";
        private const string TIME_OUT_PARAM = "timeOut";


        public StudentDailyAttendance SetStudentDailyAttendance(Guid personId, DateTime date, int? timeIn, int? timeOut)
        {
            var parameter = new Dictionary<string, object>()
                {
                    {PERSON_ID_PARAM, personId},
                    {TIME_IN_PARAM, timeIn},
                    {TIME_OUT_PARAM, timeOut},
                    {StudentDailyAttendance.DATE_FIELD_NAME, date}
                };
            using (var reader = ExecuteStoredProcedureReader(SET_DAILY_ATTENDANCE_PROC, parameter))
            {
                reader.Read();
                return reader.Read<StudentDailyAttendanceDetails>();
            }
        }

        private Dictionary<string, object> BuildConditions(StudentDailyAttendanceQuery query)
        {
            var conds = new Dictionary<string, object>();
            if (query.Date.HasValue)
                conds.Add(StudentDailyAttendance.DATE_FIELD_NAME, query.Date);
            if (query.PersonRef.HasValue)
                conds.Add(StudentDailyAttendance.PERSON_REF_FIELD_NAME, query.PersonRef);
            return conds;
        }

        public IList<StudentDailyAttendance> GetDailyAttendances(StudentDailyAttendanceQuery query)
        {
            return SelectMany<StudentDailyAttendance>(BuildConditions(query));
        } 
        public StudentDailyAttendance GetDailyAttendanceOrNull(StudentDailyAttendanceQuery query)
        {
            return SelectOneOrNull<StudentDailyAttendance>(BuildConditions(query));
        } 
    }

    public class StudentDailyAttendanceQuery
    {
        public DateTime? Date { get; set; }
        public Guid? PersonRef { get; set; }
    }
}
