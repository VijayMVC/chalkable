﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Chalkable.Common;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentDataAccess : DataAccessBase<Student, int>
    {
        public StudentDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public StudentDetails GetDetailsById(int id, int schoolYearId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@id", id},
                {"@schoolYearId", schoolYearId}
            };
            using (var r = ExecuteStoredProcedureReader("spGetStudentDetails", ps))
            {
                r.Read();
                var res = r.Read<StudentDetails>();
                r.NextResult();
                res.PersonEthnicities = r.ReadList<PersonEthnicity>();
                return res;
            }
        }


        public IList<StudentDetails> GetTeacherStudents(int teacherId, int schoolYearId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@teacherId", teacherId},
                {"@schoolYearId", schoolYearId}
            };
            return ExecuteStoredProcedureList<StudentDetails>("spGetStudentsByTeacher", ps);
        }

        private IList<StudentDetails> ReadStudentDetails(SqlDataReader reader)
        {
            var students = reader.ReadList<StudentDetails>();
            reader.NextResult();
            var personEthnities = reader.ReadList<PersonEthnicity>();

            foreach (var student in students)
                student.PersonEthnicities = personEthnities.Where(x => x.PersonRef == student.Id).ToList();

            return students;
        } 

        public IList<StudentDetails> GetStudents(int classId, int? markingPeriodId, bool? isEnrolled = null)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@classId", classId},
                {"@markingPeriodId", markingPeriodId},
                {"@isEnrolled", isEnrolled}
            };
            using (var reader = ExecuteStoredProcedureReader("spGetStudentsByClass", ps))
            {
                return ReadStudentDetails(reader);
            }
        }

        public PaginatedList<StudentDetails> SearchStudents(int schoolYearId, int? classId, int? teacherId, int? classmatesToId, string filter, bool orderByFirstName, int start, int count, int? markingPeriod)
        {
            var filters = filter?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var ps = new Dictionary<string, object>
            {
                {"@start", start},
                {"@count", count},
                {"@classId", classId},
                {"@teacherId", teacherId},
                {"@classmatesToid", classmatesToId},
                {"@schoolYearId", schoolYearId},
                {"@filter1", filters!=null && filters.Length>0 ? "%"+filters[0]+"%" : null},
                {"@filter2", filters!=null && filters.Length>1 ? "%"+filters[1]+"%" : null},
                {"@filter3", filters!=null && filters.Length>2 ? "%"+filters[2]+"%" : null},
                {"@orderByFirstName", orderByFirstName},
                {"@markingPeriod", markingPeriod}
            };
            return ExecuteStoredProcedurePaginated<StudentDetails>("spSearchStudents", ps, start, count);
        }

        public IList<int> GetEnrollmentStudentsIds(int schoolYearId, int? gradeLevelId)
        {
            var conds = new AndQueryCondition
                {
                    {StudentSchoolYear.SCHOOL_YEAR_REF_FIELD, schoolYearId},
                    {StudentSchoolYear.ENROLLMENT_STATUS_FIELD, StudentEnrollmentStatusEnum.CurrentlyEnrolled}
                };
            if(gradeLevelId.HasValue)
                conds.Add(StudentSchoolYear.GRADE_LEVEL_REF_FIELD, gradeLevelId);
            return SelectMany<StudentSchoolYear>(conds).Select(x => x.StudentRef).ToList();
        }

        public int GetEnrolledStudentsCount()
        {
            var conds = new AndQueryCondition
                {
                    {StudentSchoolYear.ENROLLMENT_STATUS_FIELD, StudentEnrollmentStatusEnum.CurrentlyEnrolled}
                };
            
            return SelectMany<StudentSchoolYear>(conds).Count;
        }
    }
}
