using System;
using System.Collections.Generic;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.DemoSchool.Storage
{
    public class DemoMarkingPeriodStorage
    {
        public MarkingPeriod getById(int id)
        {
            throw new System.NotImplementedException();
        }

        public MarkingPeriod GetLast(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId)
        {
            throw new NotImplementedException();
        }

        public MarkingPeriod GetMarkingPeriod(DateTime date)
        {
            throw new NotImplementedException();
        }

        public void Add(MarkingPeriod mp)
        {
            throw new NotImplementedException();
        }

        public bool IsOverlaped(int id, DateTime startDate, DateTime endDate, int? i)
        {
            throw new NotImplementedException();
        }

        public bool Exists(IList<int> markingPeriodIds)
        {
            throw new NotImplementedException();
        }

        public void DeleteMarkingPeriods(IList<int> markingPeriodIds)
        {
            throw new NotImplementedException();
        }

        public MarkingPeriod GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(MarkingPeriod mp)
        {
            throw new NotImplementedException();
        }

        public void ChangeWeekDays(IList<int> markingPeriodIds, int weekDays)
        {
            throw new NotImplementedException();
        }

        public MarkingPeriod GetNextInYear(int markingPeriodId)
        {
            throw new NotImplementedException();
        }

        public IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods)
        {
            throw new NotImplementedException();
        }

        public IList<MarkingPeriod> Update(IList<MarkingPeriod> markingPeriods)
        {
            throw new NotImplementedException();
        }
    }
}
