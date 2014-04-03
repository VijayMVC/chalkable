﻿using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoClassPeriodStorage:BaseDemoStorage<int, ClassPeriod>
    {
        private int index = 0;
        public DemoClassPeriodStorage(DemoStorage storage) : base(storage)
        {
        }

        public bool Exists(ClassPeriodQuery classPeriodQuery)
        {
            throw new System.NotImplementedException();
        }

        public void Add(ClassPeriod res)
        {
            data.Add(index++, res);
        }

        public void Add(IList<ClassPeriod> classPeriods)
        {
            foreach (var classPeriod in classPeriods)
            {
                Add(classPeriod);
            }
        }

        public void FullDelete(int periodId, int classId, int dayTypeId)
        {
            throw new System.NotImplementedException();
        }

        public IList<ClassPeriod> GetClassPeriods(ClassPeriodQuery classPeriodQuery)
        {
            throw new System.NotImplementedException();
        }

        public IList<Class> GetAvailableClasses(int periodId)
        {
            throw new System.NotImplementedException();
        }

        public IList<Room> GetAvailableRooms(int periodId)
        {
            throw new System.NotImplementedException();
        }
    }
}
