using System;
using System.Collections.Generic;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoAlphaGradeStorage:BaseDemoIntStorage<AlphaGrade>
    {
        public DemoAlphaGradeStorage(DemoStorage storage) : base(storage, x => x.Id)
        {
        }

        public override void Setup()
        {
            Add(new List<AlphaGrade>()
            {
                new AlphaGrade()
                {
                    Id = 1,
                    SchoolRef = 1,
                    Name = "A"
                },

                new AlphaGrade()
                {
                    Id = 2,
                    SchoolRef = 1,
                    Name = "B"
                },

                new AlphaGrade()
                {
                    Id = 3,
                    SchoolRef = 1,
                    Name = "C"
                },

                new AlphaGrade()
                {
                    Id = 4,
                    SchoolRef = 1,
                    Name = "D"
                },

                new AlphaGrade()
                {
                    Id = 5,
                    SchoolRef = 1,
                    Name = "F"
                },

                new AlphaGrade()
                {
                    Id = 6,
                    SchoolRef = 1,
                    Name = "E"
                },

                new AlphaGrade()
                {
                    Id = 7,
                    SchoolRef = 1,
                    Name = "S"
                },

                new AlphaGrade()
                {
                    Id = 8,
                    SchoolRef = 1,
                    Name = "N",
                    Description = "No pass"

                },

                new AlphaGrade()
                {
                    Id = 9,
                    SchoolRef = 1,
                    Name = "A+"
                },

                new AlphaGrade()
                {
                    Id = 10,
                    SchoolRef = 1,
                    Name = "A-"
                },

                new AlphaGrade()
                {
                    Id = 11,
                    SchoolRef = 1,
                    Name = "B+"
                },

                new AlphaGrade()
                {
                    Id = 12,
                    SchoolRef = 1,
                    Name = "B-"
                },

                new AlphaGrade()
                {
                    Id = 13,
                    SchoolRef = 1,
                    Name = "C+"
                },

                new AlphaGrade()
                {
                    Id = 14,
                    SchoolRef = 1,
                    Name = "C-"
                },

                new AlphaGrade()
                {
                    Id = 15,
                    SchoolRef = 1,
                    Name = "D+"
                },

                new AlphaGrade()
                {
                    Id = 16,
                    SchoolRef = 1,
                    Name = "D-"
                },

                new AlphaGrade()
                {
                    Id = 17,
                    SchoolRef = 1,
                    Name = "P",
                    Description = "Pass"
                },

                new AlphaGrade()
                {
                    Id = 18,
                    SchoolRef = 1,
                    Name = "Audit",
                },

                new AlphaGrade()
                {
                    Id = 19,
                    SchoolRef = 1,
                    Name = "I",
                    Description = "Incomplete"
                }

            });
        }

        public IList<AlphaGrade> GetForClassStandarts(int classId)
        {
            /*
             * var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(@"select AlphaGrade.* from AlphaGrade
                                       join GradingScaleRange on GradingScaleRange.[{0}] = AlphaGrade.[{1}]
                                       join ClassroomOption on ClassroomOption.[{2}] = GradingScaleRange.[{3}]"
                                     , GradingScaleRange.ALPHA_GRADE_REF_FIELD, AlphaGrade.ID_FIELD
                                     , ClassroomOption.STANDARD_GRADING_SCALE_REF_FIELD,
                                     GradingScaleRange.GRADING_SCALE_REF_FIELD);
            var conds = new AndQueryCondition {{ClassroomOption.ID_FIELD, classId}};
            conds.BuildSqlWhere(dbQuery, "ClassroomOption");
             */
            //throw new NotImplementedException();
            return new List<AlphaGrade>();
        }

        public IList<AlphaGrade> GetForClass(int classId)
        {
            /*var sql = @"select AlphaGrade.* from AlphaGrade
                        join GradingScaleRange on GradingScaleRange.[{0}] = AlphaGrade.[{1}]
                        join Class on Class.[{2}] = GradingScaleRange.[{3}]";
            var dbQuery = new DbQuery();
            dbQuery.Sql.AppendFormat(sql, GradingScaleRange.ALPHA_GRADE_REF_FIELD, AlphaGrade.ID_FIELD
                                     , Class.GRADING_SCALE_REF_FIELD, GradingScaleRange.GRADING_SCALE_REF_FIELD);
            var conds = new AndQueryCondition {{Class.ID_FIELD, classId}};
            conds.BuildSqlWhere(dbQuery, "Class");
            return ReadMany<AlphaGrade>(dbQuery);*/
            //throw new NotImplementedException();
            return new List<AlphaGrade>();
        }
    }
}
