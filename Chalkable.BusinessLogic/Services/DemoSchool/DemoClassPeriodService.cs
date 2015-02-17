﻿using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.BusinessLogic.Services.DemoSchool.Common;
using Chalkable.BusinessLogic.Services.DemoSchool.Storage;
using Chalkable.BusinessLogic.Services.School;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool
{
  
    public class DemoClassPeriodService : DemoSchoolServiceBase, IClassPeriodService
    {
        public DemoClassPeriodService(IServiceLocatorSchool serviceLocator, DemoStorage storage) : base(serviceLocator, storage)
        {
        }
        
        public void Add(IList<ClassPeriod> classPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            Storage.ClassPeriodStorage.Add(classPeriods);
        }

        public void Delete(IList<ClassPeriod> classPeriods)
        {
            throw new NotImplementedException();
        }

        public Class CurrentClassForTeacher(int personId, DateTime dateTime)
        {
            return Storage.ClassStorage.GetById(DemoSchoolConstants.AlgebraClassId);
        }

        public IList<ScheduleItem> GetSchedule(int? teacherId, int? studentId, int? classId, DateTime @from, DateTime to)
        {
            var syId = ServiceLocator.SchoolYearService.GetCurrentSchoolYear().Id;
            return Storage.ClassPeriodStorage.GetSchedule(syId, teacherId, studentId, classId, from, to);
        }
    }
}
