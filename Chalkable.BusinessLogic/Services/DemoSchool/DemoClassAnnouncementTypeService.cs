﻿using System.Collections.Generic;
using System.Linq;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Data.Common.Orm;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
    public class DemoClassClassAnnouncementTypeService : DemoSchoolServiceBase, IClassAnnouncementTypeService
    {
        public DemoClassClassAnnouncementTypeService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }

        public IList<ClassAnnouncementType> GetClassAnnouncementTypes(int classId, bool all = true)
        {
            var cond = new AndQueryCondition { { ClassAnnouncementType.CLASS_REF_FIELD, classId } };
            var res = Storage.ClassAnnouncementTypeStorage.GetAll(cond);
            if (!all)
                res = res.Where(x => x.Percentage > 0).ToList();
            return res;

        }

        public void Add(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            Storage.ClassAnnouncementTypeStorage.Add(classAnnouncementTypes);
        }

        public void Edit(IList<ClassAnnouncementType> classAnnouncementTypes)
        {
            Storage.ClassAnnouncementTypeStorage.Edit(classAnnouncementTypes);
        }

        public void Delete(IList<int> ids)
        {
            Storage.ClassAnnouncementTypeStorage.Delete(ids);
        }
    }
}
