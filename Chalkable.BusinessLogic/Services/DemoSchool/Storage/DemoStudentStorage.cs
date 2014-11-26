﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoStudentStorage : BaseDemoIntStorage<Student>
    {
        public DemoStudentStorage(DemoStorage storage) 
            : base(storage, x => x.Id, false)
        {
        }

        public IList<StudentDetails> GetTeacherStudents(int teacherId, int schoolYearId)
        {
            var students = GetAll();
            var markingPeriods = Storage.MarkingPeriodStorage.GetMarkingPeriods(schoolYearId);
            var classPersons = Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery());
            classPersons = classPersons.Where(x => markingPeriods.Any(mp => mp.Id == x.MarkingPeriodRef)).ToList();

            students = students.Where(s => classPersons.Any(cp => cp.PersonRef == s.Id)).ToList();
            var stWithDrawDic = Storage.StudentSchoolYearStorage.GetList(schoolYearId, null)
                                            .GroupBy(x=>x.StudentRef).ToDictionary(x=>x.Key, x=>x.First().IsEnrolled);

            return PrepareStudentDetailsData(students, stWithDrawDic);
        }


        public PaginatedList<StudentDetails> SearStudents(int schoolYearId, int? classId, int? teacherId, string filter, bool orderByFirstName, int start, int count)
        {
            var students = GetAll().AsEnumerable();
            if (!string.IsNullOrEmpty(filter))
            {
                var words = filter.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                students = students.Where(s => words.Any(w => s.FirstName.ToLower().Contains(w) || s.LastName.ToLower().Contains(w)));
            }
            var classTeachers = Storage.ClassTeacherStorage.GetClassTeachers(classId, teacherId);
            var markingPeriods = Storage.MarkingPeriodStorage.GetMarkingPeriods(schoolYearId);
            var stSchoolYears = Storage.StudentSchoolYearStorage.GetList(schoolYearId, null);
            var classPersons = Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery {ClassId = classId});
            classPersons = classPersons.Where(x => classTeachers.Any(y => y.ClassRef == x.ClassRef)).ToList();
            classPersons = classPersons.Where(x => markingPeriods.Any(y => y.Id == x.MarkingPeriodRef)).ToList();

            students = students.Where(x => classPersons.Any(y => y.PersonRef == x.Id));
            students = students.Where(x => stSchoolYears.Any(y => y.StudentRef == x.Id));

            students = orderByFirstName ? students.OrderBy(s => s.FirstName) : students.OrderBy(s => s.LastName);
            var res = students.ToList();
            IDictionary<int, bool> stWithDrawDic = new Dictionary<int, bool>();
            foreach (var student in res)
            {
                var isEnrolled = stSchoolYears.Any(x => x.StudentRef == student.Id && x.IsEnrolled) 
                                && classPersons.Any(x => x.PersonRef == student.Id && x.IsEnrolled);
                if (!stWithDrawDic.ContainsKey(student.Id))
                    stWithDrawDic.Add(student.Id, isEnrolled);
                else stWithDrawDic[student.Id] = isEnrolled;
            }
            return new PaginatedList<StudentDetails>(PrepareStudentDetailsData(res, stWithDrawDic), start / count, count);
        }

        public IList<StudentDetails> GetClassStudents(int classId, int markingPeriodId, bool? isEnrolled)
        {
            var students = GetAll();
            var stWithDrawDic = Storage.ClassPersonStorage.GetClassPersons(new ClassPersonQuery
                    {
                        ClassId = classId,
                        MarkingPeriodId = markingPeriodId,
                        IsEnrolled = isEnrolled
                    }).GroupBy(x=>x.PersonRef).ToDictionary(x=>x.Key, x=>x.First().IsEnrolled);
            students = students.Where(s => stWithDrawDic.ContainsKey(s.Id)).ToList();
            return PrepareStudentDetailsData(students, stWithDrawDic);
        }

        private IList<StudentDetails> PrepareStudentDetailsData(IEnumerable<Student> students, IDictionary<int, bool> stWithDrawDic)
        {
            return students.Select(s => new StudentDetails
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                BirthDate = s.BirthDate,
                Gender = s.Gender,
                HasMedicalAlert = s.HasMedicalAlert,
                IsAllowedInetAccess = s.IsAllowedInetAccess,
                PhotoModifiedDate = s.PhotoModifiedDate,
                SpEdStatus = s.SpEdStatus,
                SpecialInstructions = s.SpecialInstructions,
                IsWithdrawn = stWithDrawDic.ContainsKey(s.Id) && stWithDrawDic[s.Id]
            }).ToList();
        } 
    }
}
