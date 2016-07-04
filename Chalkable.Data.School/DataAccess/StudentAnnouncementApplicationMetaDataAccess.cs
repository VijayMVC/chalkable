using System;
using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class StudentAnnouncementApplicationMetaDataAccess : DataAccessBase<StudentAnnouncementApplicationMeta, int>
    {
        public StudentAnnouncementApplicationMetaDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public bool Exists(int announcementApplicationId, int studentId)
        {
            return Exists<StudentAnnouncementApplicationMeta>(BuildConditioins(announcementApplicationId, studentId));
        }

        private QueryCondition BuildConditioins(int announcementApplicationId, int studentId)
        {
            return new AndQueryCondition
            {
                {StudentAnnouncementApplicationMeta.ANNOUNCEMENT_APPLICATION_REF_FIELD, announcementApplicationId},
                {StudentAnnouncementApplicationMeta.STUDENT_REF_FIELD, studentId}
            };
        }

        private DbQuery BuildStudentAnnouncementApplicationMetaSelect()
        {
            var query = new DbQuery();
            var types = new List<Type> { typeof(StudentAnnouncementApplicationMeta), typeof(AnnouncementApplication) };
            query.Sql.AppendFormat(Orm.SELECT_FORMAT, Orm.ComplexResultSetQuery(types), types[0].Name)
                 .Append(" ").AppendFormat(Orm.SIMPLE_JOIN_FORMAT, types[1].Name, nameof(AnnouncementApplication.Id)
                                           , types[0].Name, StudentAnnouncementApplicationMeta.ANNOUNCEMENT_APPLICATION_REF_FIELD);
            query.Sql.Append(" ");
            return query;
        }

        public IList<StudentAnnouncementApplicationMeta> GetStudentAnnouncementApplicationMetaByAnnouncementId(int announcementId)
        {
            var query = BuildStudentAnnouncementApplicationMetaSelect();
            var conds = new AndQueryCondition { { nameof(AnnouncementApplication.AnnouncementRef), announcementId }, {nameof(AnnouncementApplication.Active), true} };
            conds.BuildSqlWhere(query, typeof(AnnouncementApplication).Name);
            return ReadMany<StudentAnnouncementApplicationMeta>(query, true);
        }
    }
}