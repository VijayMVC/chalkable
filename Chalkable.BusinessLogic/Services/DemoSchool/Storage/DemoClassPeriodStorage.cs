using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassPeriodStorage:BaseDemoStorage<int, ClassPeriod>
    {
        public DemoClassPeriodStorage(DemoStorage storage) : base(storage)
        {
        }

        public bool Exists(ClassPeriodQuery classPeriodQuery)
        {
            return GetClassPeriods(classPeriodQuery).Count > 0;
        }

        public void Add(ClassPeriod res)
        {
            data.Add(GetNextFreeId(), res);
        }

        public void Add(IList<ClassPeriod> classPeriods)
        {
            foreach (var classPeriod in classPeriods)
            {
                Add(classPeriod);
            }
        }

        public void FullDelete(int periodId, int classId, int dayTypeId)
        {
            
            throw new System.NotImplementedException();
        }

        public IList<ClassPeriod> GetClassPeriods(ClassPeriodQuery classPeriodQuery)
        {

            var classPeriods = data.Select(x => x.Value);

            if (classPeriodQuery.PeriodId.HasValue)
                classPeriods = classPeriods.Where(x => x.PeriodRef == classPeriodQuery.PeriodId);

            if (classPeriodQuery.DateTypeId.HasValue)
                classPeriods = classPeriods.Where(x => x.DayTypeRef == classPeriodQuery.DateTypeId);

            classPeriods = classPeriods.Where(x => classPeriodQuery.ClassIds.Contains(x.ClassRef));

           // if (classPeriodQuery.SchoolYearId.HasValue)
             //   classPeriods = classPeriods.Where( x => x.)

            return classPeriods.ToList();


            /*var conds = new AndQueryCondition();
            var classPeriodTName = "ClassPeriod";
            if (query.PeriodId.HasValue)
                conds.Add(ClassPeriod.PERIOD_REF_FIELD, query.PeriodId);
            if (query.DateTypeId.HasValue)
                conds.Add(ClassPeriod.DAY_TYPE_REF_FIELD, query.DateTypeId);

            FilterBySchool(conds).BuildSqlWhere(dbQuery, classPeriodTName);

            if (query.RoomId.HasValue)
            {
                conds.Add("roomId", query.RoomId);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] in (select [{2}].[{4}] from [{2}] where [{2}].[{3}] = @roomId)"
                    , classPeriodTName, ClassPeriod.CLASS_REF_FIELD, "Class", Class.ROOM_REF_FIELD, Class.ID_FIELD);
            }
            
            if (query.StudentId.HasValue)
            {
                dbQuery.Parameters.Add("studentId", query.StudentId);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] in (select [{2}].[{4}] from [{2}] where [{2}].[{3}]  = @studentId)"
                    , classPeriodTName, ClassPeriod.CLASS_REF_FIELD, "ClassPerson", ClassPerson.PERSON_REF_FIELD, ClassPerson.CLASS_REF_FIELD);
            }
            if (query.TeacherId.HasValue)
            {
                dbQuery.Parameters.Add("teacherId", query.TeacherId);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] in (select [{2}].[{4}] from [{2}] where [{2}].[{3}] = @teacherId)"
                    , classPeriodTName, ClassPeriod.CLASS_REF_FIELD, "Class", Class.TEACHER_REF_FIELD, Class.ID_FIELD);
            }

            if (query.SchoolYearId.HasValue)
            {
                dbQuery.Parameters.Add(Period.SCHOOL_YEAR_REF, query.SchoolYearId);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] = @{1}", "Period", Period.SCHOOL_YEAR_REF);
            }

            if (query.Time.HasValue)
            {
                dbQuery.Parameters.Add("time", query.Time);
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] <= @time and [{0}].[{2}] >= @time"
                    , "Period", Period.START_TIME_FIELD, Period.END_TIME_FIELD);
            }
            if (query.ClassIds != null && query.ClassIds.Count > 0)
            {
                var classIdsParams = new List<string>();
                for (int i = 0; i < query.ClassIds.Count; i++)
                {
                    var classIdParam = "@classId_" + i;
                    classIdsParams.Add(classIdParam);
                    dbQuery.Parameters.Add(classIdParam, query.ClassIds[i]);
                }
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] in ({2})", classPeriodTName
                    , ClassPeriod.CLASS_REF_FIELD,  classIdsParams.JoinString(","));
            }
            return dbQuery;*/

            //
            //if (classPeriodQuery.SchoolYearId.HasValue)
            //    classPeriods = classPeriods.Where(x => x.)

            //
            //if (classPeriodQuery.Time.HasValue)
            //    classPeriods = classPeriods.Where(x => x.)
            //
            //if (classPeriodQuery.TeacherId.HasValue)
            //  classPeriods = classPeriods.Where(x => x.)
            //if (classPeriodQuery.StudentId.HasValue)
            //  classPeriods = classPeriods.Where(x => x.)
            //if (classPeriodQuery.RoomId.HasValue)
            //  classPeriods = classPeriods.Where(x => x.)
            //if (classPeriodQuery.MarkingPeriodId.HasValue)
            //  classPeriods = classPeriods.Where(x => x.)

        }

        public IList<Class> GetAvailableClasses(int periodId)
        {
            var classIds = GetClassPeriods(new ClassPeriodQuery
            {
                PeriodId = periodId
            }).Select(x => x.ClassRef);

            return classIds.Select(classId => Storage.ClassStorage.GetById(classId)).ToList();
        }

        public IList<Room> GetAvailableRooms(int periodId)
        {
            var classIds = GetClassPeriods(new ClassPeriodQuery
            {
                PeriodId = periodId
            }).Select(x => x.ClassRef);


            var rooms = new List<Room>();

            foreach (var classId in classIds)
            {
                var cls = Storage.ClassStorage.GetById(classId);
                if (cls.RoomRef.HasValue)
                    rooms.Add(Storage.RoomStorage.GetById(cls.RoomRef.Value));

            }

            return rooms;
        }

        public override void Setup()
        {
            //todo add class periods
        }
    }
}
