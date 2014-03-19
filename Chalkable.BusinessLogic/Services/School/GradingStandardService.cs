﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Model;
using Chalkable.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;
using Chalkable.StiConnector.Connectors.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface  IGradingStandardService
    {
        IList<GradingStandardInfo> GetGradingStandards(int classId);
        GradingStandardInfo SetGrade(int studentId, int standardId, int classId, int gradingPeriodId, int? alphaGradeId, string note);
    }

    public class GradingStandardService : SisConnectedService, IGradingStandardService
    {
        public GradingStandardService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<GradingStandardInfo> GetGradingStandards(int classId)
        {
            var standardScores = ConnectorLocator.StandardScoreConnector.GetStandardScores(classId, null, null);
            var students = ServiceLocator.PersonService.GetPaginatedPersons(new PersonQuery
                            {
                                ClassId = classId,
                                RoleId = CoreRoles.STUDENT_ROLE.Id
                            });
            var standards = ServiceLocator.StandardService.GetStandardes(classId, null, null);
            var res = new List<GradingStandardInfo>();
            foreach (var standardScore in standardScores)
            {
                var student = students.First(x => x.Id == standardScore.StudentId);
                var standard = standards.First(x => x.Id == standardScore.StandardId);
                res.Add(GradingStandardInfo.Create(standardScore, standard, student));
            }
            return res;
        }

        public GradingStandardInfo SetGrade(int studentId, int standardId, int classId, int gradingPeriodId, int? alphaGradeId, string note)
        {
            var standardScore = new StandardScore
                {
                    SectionId = classId,
                    StudentId = studentId,
                    StandardId = standardId,
                    Note = note,
                    EnteredScoreAlphaGradeId = alphaGradeId,
                    GradingPeriodId = gradingPeriodId
                };
            standardScore = ConnectorLocator.StandardScoreConnector.Update(classId, studentId, standardId, gradingPeriodId, standardScore);
            var standard = ServiceLocator.StandardService.GetStandardById(standardId);
            var student = ServiceLocator.PersonService.GetPerson(studentId);
            return GradingStandardInfo.Create(standardScore, standard, student);
        }
    }



}
