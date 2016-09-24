﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Security.AccessControl;
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

        public Student GetById(int id, int schoolYearId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@id", id},
                {"@schoolYearId", schoolYearId}
            };
            using (var r = ExecuteStoredProcedureReader("spGetStudent", ps))
            {
                r.Read();
                var res = r.Read<Student>();
                return res;
            }
        }

        public IList<Student> GetTeacherStudents(int teacherId, int schoolYearId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@teacherId", teacherId},
                {"@schoolYearId", schoolYearId}
            };
            return ExecuteStoredProcedureList<Student>("spGetStudentsByTeacher", ps);
        }

        public IList<Student> GetStudents(int classId, int? markingPeriodId, bool? isEnrolled = null)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@classId", classId},
                {"@markingPeriodId", markingPeriodId},
                {"@isEnrolled", isEnrolled}
            };
            using (var reader = ExecuteStoredProcedureReader("spGetStudentsByClass", ps))
            {
                return reader.ReadList<Student>();
            }
        }

        public PaginatedList<StudentSchoolsInfo> SearchStudents(int schoolYearId, int? classId, IList<int> schoolIds, int? gradeLevel, int? programId, int? teacherId, int? classmatesToId, string filter, 
            bool orderByFirstName, int start, int count, int? markingPeriod, bool enrolledOnly = false)
        {
            var filters = filter?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var ps = new Dictionary<string, object>
            {
                {"@start", start},
                {"@count", count},
                {"@classId", classId},
                {"@teacherId", teacherId},
                {"@schoolIds", schoolIds},
                {"@gradeLevel", gradeLevel},
                {"@programId", programId},
                {"@classmatesToid", classmatesToId},
                {"@schoolYearId", schoolYearId},
                {"@filter1", filters!=null && filters.Length>0 ? "%"+filters[0]+"%" : null},
                {"@filter2", filters!=null && filters.Length>1 ? "%"+filters[1]+"%" : null},
                {"@filter3", filters!=null && filters.Length>2 ? "%"+filters[2]+"%" : null},
                {"@orderByFirstName", orderByFirstName},
                {"@markingPeriod", markingPeriod},
                {"@enrolledOnly", enrolledOnly }
            };

            return ExecuteStoredProcedurePaginated("spSearchStudentsTmp", ps, ReadStudentInfo,  start, count);
        }

        private static IList<StudentSchoolsInfo> ReadStudentInfo(DbDataReader reader)
        {
            var students = reader.ReadList<StudentSchoolsInfo>();
            reader.NextResult();

            IList<StudentSchoolYear> ssy = new List<StudentSchoolYear>();
            IDictionary <int, IList<Model.School>> stSchool = new Dictionary<int, IList<Model.School>>();
            IList<Model.School> dictionarySchools;
            while (reader.Read())
            {
                var ssyItem = reader.Read<StudentSchoolYear>();
                ssyItem.GradeLevel = reader.Read<GradeLevel>(true);
                ssy.Add(ssyItem);

                var school = reader.Read<Model.School>();
                if (!stSchool.TryGetValue(ssyItem.StudentRef, out dictionarySchools))
                    stSchool.Add(ssyItem.StudentRef, dictionarySchools = new List<Model.School>());
                dictionarySchools.Add(school);
            }

            foreach (var student in students)
            {
                student.StudentSchools = stSchool[student.Id].Distinct().ToList();
                student.GradeLevel = ssy.First(x => x.StudentRef == student.Id).GradeLevel;
            }

            return students;
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

        public StudentDetails GetDetailsById(int id, int schoolYearId)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@id", id},
                {"@schoolYearId", schoolYearId}
            };
            using (var r = ExecuteStoredProcedureReader("spGetStudentDetails", ps))
            {
                return ReadStudentDetails(r);
            }
        }

        public IList<StudentDetails> GetDetailsByClass(int classId, bool? isEnrolled = null)
        {
            IDictionary<string, object> ps = new Dictionary<string, object>
            {
                {"@classId", classId},
                {"@isEnrolled", isEnrolled}
            };
            using (var reader = ExecuteStoredProcedureReader("spGetStudentsDetailsByClass", ps))
            {
                return ReadStudentsDetails(reader);
            }
        }

        private static StudentDetails ReadStudentDetails(DbDataReader reader)
        {
            reader.Read();
            var student = reader.Read<StudentDetails>();
            reader.NextResult();
            student.PersonEthnicities = reader.ReadList<PersonEthnicity>();
            reader.NextResult();
            student.PersonLanguages = reader.ReadList<PersonLanguage>();
            reader.NextResult();
            student.PersonNationalities = reader.ReadList<PersonNationality>();
            reader.NextResult();
            student.StudentSchools = reader.ReadList<StudentSchool>();

            return student;
        }

        private static IList<StudentDetails> ReadStudentsDetails(DbDataReader reader)
        {
            var students = reader.ReadList<StudentDetails>();
            reader.NextResult();
            var personEthnicities = reader.ReadList<PersonEthnicity>();
            reader.NextResult();
            var personLanguages = reader.ReadList<PersonLanguage>();
            reader.NextResult();
            var personNationalities = reader.ReadList<PersonNationality>();
            reader.NextResult();
            var studentSchools = reader.ReadList<StudentSchool>();

            foreach (var student in students)
            {
                student.PersonEthnicities = personEthnicities.Where(x => x.PersonRef == student.Id).ToList();
                student.PersonNationalities = personNationalities.Where(x => x.PersonRef == student.Id).ToList();
                student.PersonLanguages = personLanguages.Where(x => x.PersonRef == student.Id).ToList();
                student.StudentSchools = studentSchools.Where(x => x.StudentRef == student.Id).ToList();
            }

            return students;
        }
    }
}
