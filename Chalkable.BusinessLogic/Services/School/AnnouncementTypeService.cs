using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface IAnnouncementTypeService
    {
        IList<AnnouncementType> GetAnnouncementTypes(bool? gradable);
        AnnouncementType GetAnnouncementTypeById(int id);
        AnnouncementType GetAnnouncementTypeBySystemType(SystemAnnouncementType type);
        IList<ClassAnnouncementType> GetClassAnnouncementTypes(int classId, bool all = true);
        IList<ClassAnnouncementType> GetClassAnnouncementTypes(int type, int? classId);
    }
    public class AnnouncementTypeService : SchoolServiceBase, IAnnouncementTypeService
    {
        public AnnouncementTypeService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public IList<AnnouncementType> GetAnnouncementTypes(bool? gradable)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementTypeDataAccess(uow);
                return PrepareAnnouncementTypes(da.GetList(gradable));
            }
        }


        private IList<AnnouncementType> PrepareAnnouncementTypes(IList<AnnouncementType> types)
        {
            if (BaseSecurity.IsAdminViewer(Context))
            {
                types.First(x => x.SystemType == SystemAnnouncementType.Admin).CanCreate = true;
            }
            if (Context.Role == CoreRoles.TEACHER_ROLE)
            {
                types.First(x => x.SystemType == SystemAnnouncementType.Standard).CanCreate = true;
                types.First(x => x.SystemType == SystemAnnouncementType.BookReport).CanCreate = true;
                types.First(x => x.SystemType == SystemAnnouncementType.Test).CanCreate = true;
                types.First(x => x.SystemType == SystemAnnouncementType.TermPaper).CanCreate = true;
                types.First(x => x.SystemType == SystemAnnouncementType.Quiz).CanCreate = true;
                types.First(x => x.SystemType == SystemAnnouncementType.Project).CanCreate = true;
                types.First(x => x.SystemType == SystemAnnouncementType.Midterm).CanCreate = true;
                types.First(x => x.SystemType == SystemAnnouncementType.HW).CanCreate = true;
                types.First(x => x.SystemType == SystemAnnouncementType.Final).CanCreate = true;
                types.First(x => x.SystemType == SystemAnnouncementType.Essay).CanCreate = true;
            }
            return types;
        }

        public AnnouncementType GetAnnouncementTypeById(int id)
        {
            using (var uow = Read())
            {
                var da = new AnnouncementTypeDataAccess(uow);
                return da.GetById(id);
            }
        }

        public AnnouncementType GetAnnouncementTypeBySystemType(SystemAnnouncementType type)
        {
            return GetAnnouncementTypeById((int)type);
        }


        public IList<ClassAnnouncementType> GetClassAnnouncementTypes(int classId, bool all = true)
        {
            using (var uow = Read())
            {
                var cond = new AndQueryCondition{{ClassAnnouncementType.CLASS_REF_FIELD, classId}};
                var res = new ClassAnnouncementTypeDataAccess(uow).GetAll(cond);
                if (res.Count == 0)
                    res = BuildClassAnnouncementTypes(classId);
                if (!all)
                    res = res.Where(x => x.Percentage > 0).ToList();
                return res;
            }
        }

        private IList<ClassAnnouncementType> BuildClassAnnouncementTypes(int classId)
        {
            using (var uow = Update())
            {
                var da = new ClassAnnouncementTypeDataAccess(uow);
                var annTypes = PrepareAnnouncementTypes(new AnnouncementTypeDataAccess(uow).GetAll());
                var c = new ClassDataAccess(uow, Context.SchoolLocalId).GetById(classId);
                if(c.TeacherRef != Context.UserLocalId)
                    throw new ChalkableSecurityException();
                var res = annTypes.Select(x => new ClassAnnouncementType
                    {
                        AnnouncementTypeRef = x.Id,
                        ClassRef = classId,
                        Description = x.Description,
                        Gradable = x.Gradable,
                        Name = x.Name,
                        Percentage = x.Percentage
                    }).ToList();
                da.Insert(res);
                uow.Commit();
                return res;
            }
        }


        public IList<ClassAnnouncementType> GetClassAnnouncementTypes(int type, int? classId)
        {
            using (var uow = Read())
            {
                var conds = new AndQueryCondition {{ClassAnnouncementType.ANNOUNCEMENT_TYPE_REF, type}};
                if (classId.HasValue)
                    conds = new AndQueryCondition {{ClassAnnouncementType.CLASS_REF_FIELD, classId}};
                var res = new ClassAnnouncementTypeDataAccess(uow).GetAll(conds);
                //if (res.Count == 0)
                //    res = BuildClassAnnouncementTypes(classId);
                return res;

            }
            throw new System.NotImplementedException();
        }
    }
}
