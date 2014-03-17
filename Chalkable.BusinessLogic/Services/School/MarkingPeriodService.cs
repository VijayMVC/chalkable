using System;
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
    public interface IMarkingPeriodService
    {
        MarkingPeriod Add(int id, int schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays);
        IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods);
        void Delete(int id);
        void DeleteMarkingPeriods(IList<int> ids);
        MarkingPeriod Edit(int id, int schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays);
        IList<MarkingPeriod> Edit(IList<MarkingPeriod> markingPeriods); 
        MarkingPeriod GetMarkingPeriodById(int id);
        MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate = null);
        MarkingPeriod GetNextMarkingPeriodInYear(int markingPeriodId);
        MarkingPeriodClass GetMarkingPeriodClass(int classId, int markingPeriodId);
        MarkingPeriodClass GetMarkingPeriodClass(int markingPeriodClassId);
        IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId);
        MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false);
        bool ChangeWeekDays(IList<int> markingPeriodIds, int weekDays);
        bool ChangeMarkingPeriodsCount(int schoolYearId, int count, out string error);
       
    }

    public class MarkingPeriodService : SchoolServiceBase, IMarkingPeriodService
    {
        public MarkingPeriodService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public MarkingPeriod GetMarkingPeriodById(int id)
        {
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId);
                return da.GetById(id);
            }
        }

        public MarkingPeriod GetLastMarkingPeriod(DateTime? tillDate = null)
        {
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId);
                return  da.GetLast(tillDate ?? Context.NowSchoolTime);
            }
        }

        public MarkingPeriodClass GetMarkingPeriodClass(int classId, int markingPeriodId)
        {
            using (var uow = Read())
            {
                return new MarkingPeriodClassDataAccess(uow, Context.SchoolLocalId)
                    .GetMarkingPeriodClassOrNull(new MarkingPeriodClassQuery
                        {
                            MarkingPeriodId = markingPeriodId,
                            ClassId = classId
                        });
            }
        }

        public IList<MarkingPeriod> GetMarkingPeriods(int? schoolYearId)
        {
            using (var uow = Read())
            {
                return new MarkingPeriodDataAccess(uow, Context.SchoolLocalId).GetMarkingPeriods(schoolYearId);
            }
        }

        public MarkingPeriod GetMarkingPeriodByDate(DateTime date, bool useLastExisting = false)
        {
            using (var uow = Read())
            {
                var da = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId);
                var res = da.GetMarkingPeriod(date);
                if (res != null)
                    return res;
                if (useLastExisting)
                    return GetLastMarkingPeriod(date);
            }
            return null;
        }

        public MarkingPeriod Add(int id, int schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var da = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId);
                var sy = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetById(schoolYearId);
                var mp = UpdateMarkingPeriod(da, sy, startDate, endDate, name, description, weekDays, new MarkingPeriod{Id = id});
                da.Insert(mp);
                uow.Commit();
                return mp;
            }
        }

        private void ValidateMarkingPeriodData(MarkingPeriodDataAccess dataAccess, SchoolYear sy, DateTime startDate,
                                               DateTime endDate, int? id)
        {
            if (startDate > endDate)
                throw new ChalkableException("Invalida date params. StartDate is biger than EndDate");
            if (dataAccess.IsOverlaped(sy.Id, startDate, endDate, id))
                throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_CANT_OVERLAP);
            if (!(sy.StartDate <= startDate && sy.EndDate >= endDate))
                throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_INVALID_SCHOOL_YEAR);
        }

        private MarkingPeriod UpdateMarkingPeriod(MarkingPeriodDataAccess dataAccess, SchoolYear sy, DateTime startDate, DateTime endDate, 
            string name, string description, int weekDays, MarkingPeriod markingPeriod = null)
        {
            int? id = null;
            if (markingPeriod != null)
                id = markingPeriod.Id;
            else markingPeriod = new MarkingPeriod();
            
            ValidateMarkingPeriodData(dataAccess, sy, startDate, endDate, id);

            markingPeriod.Description = description;
            markingPeriod.Name = name;
            markingPeriod.SchoolYearRef = sy.Id;
            markingPeriod.StartDate = startDate;
            markingPeriod.EndDate = endDate;
            markingPeriod.WeekDays = weekDays;
            markingPeriod.SchoolRef = sy.SchoolRef;
            return markingPeriod;
        }
        
        public void Delete(int id)
        {
            DeleteMarkingPeriods(new List<int> { id });
        }

        public void DeleteMarkingPeriods(IList<int> markingPeriodIds)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            using (var uow = Update())
            {
                if (new MarkingPeriodClassDataAccess(uow, null).Exists(markingPeriodIds))
                    throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_ASSIGNED_TO_CLASS);
                new MarkingPeriodDataAccess(uow, Context.SchoolLocalId).DeleteMarkingPeriods(markingPeriodIds);
                uow.Commit();
            }
        }


        public MarkingPeriod Edit(int id, int schoolYearId, DateTime startDate, DateTime endDate, string name, string description, int weekDays)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            
            using (var uow = Update())
            {
                var da = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId);
                var sy = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetById(schoolYearId);
                var mp = da.GetById(id);
                mp = UpdateMarkingPeriod(da, sy, startDate, endDate, name, description, weekDays, mp);
                da.Update(mp);
                uow.Commit();
                return mp;
            }
        }


        //TODO : think how to rewrite this for better performance
        //TODO : needs testing
        public bool ChangeWeekDays(IList<int> markingPeriodIds, int weekDays)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var mpDa = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId);
                if (new DateDataAccess(uow, Context.SchoolLocalId).Exists(markingPeriodIds))
                    throw new ChalkableException("Can't change markingPeriod week days ");
                
                mpDa.ChangeWeekDays(markingPeriodIds, weekDays);
                uow.Commit();
                return true;
            }
        }

        //TODO: remove this method
        public bool ChangeMarkingPeriodsCount(int schoolYearId, int count, out string error)
        {
            throw new NotImplementedException();
            //if (!BaseSecurity.IsAdminEditor(Context))
            //    throw new ChalkableSecurityException();
            //if(!Context.SchoolId.HasValue)
            //    throw new UnassignedUserException();
            //error = null;
            //var year = ServiceLocator.SchoolYearService.GetSchoolYearById(schoolYearId);
            //if (count <= 0)
            //    throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_INVALID_COUNT);

            //var mps = GetMarkingPeriods(year.Id);
            //if (mps.Count != count)
            //{
            //    using (var uow = Update())
            //    {
            //        var mpDa = new MarkingPeriodDataAccess(uow);
            //        var syDa = new SchoolYearDataAccess(uow);
            //        var oldYearEndDate = year.EndDate;
            //        var changeMpCount = Math.Abs(count - mps.Count);
            //        try
            //        {
            //            if (count > mps.Count)
            //            {
            //                const int mpLength = 30;
            //                var endDate = year.StartDate.AddDays(1);
            //                if (mps.Count > 0)
            //                    endDate = mps.Last().EndDate;
            //                var mpsForInsert = new List<MarkingPeriod>();
            //                for (var i = 0; i < changeMpCount; i++)
            //                {
            //                    var startDate = endDate.AddDays(1);
            //                    endDate = endDate.AddDays(mpLength);
            //                    mpsForInsert.Add(UpdateMarkingPeriod(mpDa, year, startDate, endDate, "MP" + (mps.Count + i + 1), "", 62));
            //                }
            //                if (year.EndDate <= endDate)
            //                    year.EndDate = endDate.AddDays(1);
            //                mpDa.Insert(mpsForInsert);
            //            }
            //            else
            //            {
            //                var mpsForDelete = mps.Skip(count).Take(changeMpCount).Select(x => x.Id).ToList();
            //                Delete(mpsForDelete);
            //                year.EndDate = mps[count - 1].EndDate.AddDays(1);
            //            }
            //            if (year.EndDate != oldYearEndDate)
            //                syDa.Update(year);
            //            uow.Commit();
            //        }
            //        catch (Exception e)
            //        {
            //            error = e.Message;
            //        }
            //    }
            //}
            //return true;
        }


        public MarkingPeriodClass GetMarkingPeriodClass(int markingPeriodClassId)
        {
            using (var uow = Read())
            {
                return new MarkingPeriodClassDataAccess(uow, Context.SchoolLocalId).GetById(markingPeriodClassId);
            }
        }
    

        public MarkingPeriod GetNextMarkingPeriodInYear(int markingPeriodId)
        {
            using (var uow = Read())
            {
                return new MarkingPeriodDataAccess(uow, Context.SchoolLocalId).GetNextInYear(markingPeriodId);
            }
        }


        public IList<MarkingPeriod> Add(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var sys = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetAll();
                var mpDa = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId);
                foreach (var markingPeriod in markingPeriods)
                {
                    var sy = sys.First(x => x.Id == markingPeriod.SchoolYearRef);
                    ValidateMarkingPeriodData(mpDa, sy, markingPeriod.StartDate, markingPeriod.EndDate, markingPeriod.Id);
                }
                new MarkingPeriodDataAccess(uow, Context.SchoolLocalId).Insert(markingPeriods);
                uow.Commit();
                return markingPeriods;
            }
        }


        public IList<MarkingPeriod> Edit(IList<MarkingPeriod> markingPeriods)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                var sys = new SchoolYearDataAccess(uow, Context.SchoolLocalId).GetAll();
                var mpDa = new MarkingPeriodDataAccess(uow, Context.SchoolLocalId);
                foreach (var markingPeriod in markingPeriods)
                {
                    var sy = sys.First(x => x.Id == markingPeriod.SchoolYearRef);
                    ValidateMarkingPeriodData(mpDa, sy, markingPeriod.StartDate, markingPeriod.EndDate, markingPeriod.Id);
                }
                new MarkingPeriodDataAccess(uow, Context.SchoolLocalId).Update(markingPeriods);
                uow.Commit();
                return markingPeriods;
            }
        }
    }
}
