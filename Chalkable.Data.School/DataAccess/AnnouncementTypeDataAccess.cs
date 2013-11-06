using System.Collections.Generic;
using Chalkable.Data.Common;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.Model;

namespace Chalkable.Data.School.DataAccess
{
    public class AnnouncementTypeDataAccess : DataAccessBase<AnnouncementType, int>
    {
        public AnnouncementTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<AnnouncementType> GetList(bool? gradeble)
        {
            var conds = new AndQueryCondition();
            if (gradeble.HasValue)
                conds.Add(AnnouncementType.GRADABLE_FIELD_NAME, gradeble.Value);
            return SelectMany<AnnouncementType>(conds);
        }
    }

    public class ClassAnnouncementTypeDataAccess : DataAccessBase<ClassAnnouncementType, int>
    {
        public ClassAnnouncementTypeDataAccess(UnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IList<ClassAnnouncementType> GetClassAnnouncementTypes(int announcementTypeId, int? classId, int? teacherId)
        {
            var conds = new AndQueryCondition { { ClassAnnouncementType.ANNOUNCEMENT_TYPE_REF, announcementTypeId } };
            if (classId.HasValue)
                conds = new AndQueryCondition { { ClassAnnouncementType.CLASS_REF_FIELD, classId } };
            var dbQuery = Orm.SimpleSelect<ClassAnnouncementType>(conds);
            if (teacherId.HasValue)
            {
                dbQuery.Parameters.Add(Class.TEACHER_REF_FIELD, teacherId);
                var tName = typeof (ClassAnnouncementType).Name;
                dbQuery.Sql.AppendFormat(" and [{0}].[{1}] in (select {3} from [{2}] where  [{2}].[{4}] = @{4})"
                                         , tName, ClassAnnouncementType.CLASS_REF_FIELD, "Class", Class.ID_FIELD,
                                         Class.TEACHER_REF_FIELD);
            }
            return ReadMany<ClassAnnouncementType>(dbQuery);
        } 
    }
}
