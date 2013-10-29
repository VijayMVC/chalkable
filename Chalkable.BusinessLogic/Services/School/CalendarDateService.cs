using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.Common;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ICalendarDateService
    {
        Date GetCalendarDateByDate(DateTime date);
        DateTime GetDbDateTime();
        IList<Date> GetDays(int markingPeriodId, bool schoolDaysOnly, DateTime? fromDate = null, DateTime? tillDate = null, int count = Int32.MaxValue);
        IList<DateDetails> GetLastDays(int schoolYearId, bool schoolDaysOnly, DateTime? fromDate, DateTime? tillDate, int count = int.MaxValue);
        void ClearCalendarDates(int markingPeriodId);
        bool CanAssignDate(DateTime date, int dateTypeId);
        void AssignDate(DateTime date, int dateTypeId);
        Date Add(DateTime date, bool schoolDay, int schoolYearId, int? dateTypeId);
        void Delete(DateTime date);
    }

    //TODO: needs tests
    public class CalendarDateService : SchoolServiceBase, ICalendarDateService
    {
        public CalendarDateService(IServiceLocatorSchool serviceLocator) : base(serviceLocator)
        {
        }

        public Date GetCalendarDateByDate(DateTime date)
        {
            using (var uow = Update())
            {
                var da = new DateDataAccess(uow);
                var res = da.GetDateOrNull(new DateQuery {FromDate = date, ToDate = date});
                if (res == null)
                {
                    var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(date);
                    var dates = new List<Date>();
                    if (mp == null)
                    {
                        res = new Date { Day = date };
                        dates.Add(res);
                    }
                    else
                    {
                        //TODO: just regenerate for all MP

                        da.Delete(new DateQuery {MarkingPeriodId = mp.Id});

                        var sections = ServiceLocator.ScheduleSectionService.GetSections(mp.Id);
                        if (sections.Count == 0)
                            throw new ChalkableException(ChlkResources.ERR_MARKING_PERIOD_SHOULD_HAVE_SECTION);
                       
                        var sectionIndex = 0;
                        var startDate = mp.StartDate.Date;
                        var endDate = mp.EndDate.Date;

                        for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                        {
                            var d = new Date
                            {
                                SchoolYearRef = mp.SchoolYearRef,
                                IsSchoolDay = false,
                                Day = dt
                            };
                            dates.Add(d);
                            if (((1 << (int)dt.DayOfWeek) & mp.WeekDays) != 0)
                            {
                                d.IsSchoolDay = true;
                                d.DateTypeRef = sections[sectionIndex].Id;
                                sectionIndex = (sectionIndex + 1) % sections.Count;
                            }
                            if (d.Day == date) res = d;
                        }
                    }
                    da.Insert(dates);
                }
                uow.Commit();
                return res;
            }
        }


        public DateTime GetDbDateTime()
        {
            using (var uow = Read())
            {
                return new DateDataAccess(uow).GetDbDateTime();
            }
        }

        public IList<Date> GetDays(int markingPeriodId, bool schoolDaysOnly, DateTime? fromDate = null, DateTime? tillDate = null, int count = Int32.MaxValue)
        {
            var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodById(markingPeriodId);
            GetCalendarDateByDate(mp.EndDate);

            using (var uow = Read())
            {
                return new DateDataAccess(uow)
                    .GetDates(new DateQuery
                        {
                            MarkingPeriodId = markingPeriodId,
                            SchoolDaysOnly = schoolDaysOnly,
                            FromDate = fromDate,
                            ToDate = tillDate,
                            Count = count
                        });

            }
        }

        public IList<DateDetails> GetLastDays(int schoolYearId, bool schoolDaysOnly, DateTime? fromDate, DateTime? tillDate, int count = Int32.MaxValue)
        {
            using (var uow = Read())
            {
                var markingPeriods = ServiceLocator.MarkingPeriodService.GetMarkingPeriods(schoolYearId);
                foreach (var markingPeriod in markingPeriods)
                {
                    if (!((tillDate.HasValue && markingPeriod.StartDate > tillDate)
                          || (fromDate.HasValue && markingPeriod.EndDate > fromDate)))
                    {
                        ServiceLocator.CalendarDateService.GetCalendarDateByDate(markingPeriod.StartDate.AddDays(1)); 
                    }
                }

                var da = new DateDataAccess(uow);
                return da.GetDatesDetails(new DateQuery
                    {
                        SchoolYearId = schoolYearId,
                        FromDate = fromDate,
                        ToDate = tillDate,
                        Count = count,
                        SchoolDaysOnly = schoolDaysOnly
                    });
            }
        }

        public void ClearCalendarDates(int markingPeriodId)
        {
            Delete(new DateQuery{MarkingPeriodId = markingPeriodId});
        }

        public void Delete(DateTime date)
        {
            Delete(new DateQuery { FromDate = date, ToDate = date});
        }
        
        private void Delete(DateQuery query)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new DateDataAccess(uow).Delete(query);
                uow.Commit();
            }
        }

        public bool CanAssignDate(DateTime date, int sectionId)
        {
            var cdDate = GetByDate(date);
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();

            var section = ServiceLocator.ScheduleSectionService.GetSectionById(sectionId);
            if (section.SchoolYearRef != cdDate.SchoolYearRef)
                throw new ChalkableException(ChlkResources.ERR_SECTION_NOT_IN_MARKING_PERIOD_FOR_CURRENT_DAY);
            
            using (var uow = Read())
            {
                var da = new ClassAttendanceDataAccess(uow);
                //TODO : check discipline existing
                //return !da.Exists(section.SchoolYearRef, cdDate.DateTime);   
                return true;
            }
        }

        public void AssignDate(DateTime date, int sectionId)
        {
            if(!CanAssignDate(date, sectionId))
                throw new ChalkableException(ChlkResources.ERR_CANT_REASSIGN_DAY_TO_DIFFERENT_SECTION);

            using (var uow = Update())
            {
                var cdDate = GetByDate(date, uow);
                cdDate.DateTypeRef = sectionId;
                cdDate.IsSchoolDay = true;
                new DateDataAccess(uow).Update(cdDate);
                uow.Commit();
            }
        }

        private Date GetByDate(DateTime date)
        {
            using (var uow = Read())
            {
                return GetByDate(date, uow);
            }
        }
        
        private Date GetByDate(DateTime date, UnitOfWork unitOfWork)
        {
            return new DateDataAccess(unitOfWork).GetDate(new DateQuery {FromDate = date, ToDate = date});
        }

        public Date Add(DateTime date, bool schoolDay, int schoolYearId, int? dateTypeId)
        {
            if (!BaseSecurity.IsDistrict(Context))
                throw new ChalkableSecurityException();
            if (!schoolDay && dateTypeId.HasValue)
                throw new ChalkableException("Incorrect parameters data");

            using (var uow = Update())
            {
                if (dateTypeId.HasValue)
                {
                    //var section = new DateTypeDataAccess(uow).GetById(dateType.Value);
                    //if (!markingPeriodId.HasValue)
                    //    markingPeriodId = section.MarkingPeriodRef;
                    //if (markingPeriodId.Value != section.MarkingPeriodRef)
                    //    throw new ChalkableException(ChlkResources.ERR_SECTION_NOT_IN_MARKING_PERIOD_FOR_CURRENT_DAY);
                }
                
                var res = new Date
                    {
                        Day = date,
                        IsSchoolDay = schoolDay,
                        SchoolYearRef = schoolYearId,
                        DateTypeRef = dateTypeId,
                    };
                new DateDataAccess(uow).Insert(res);
                uow.Commit();
                return res;
            }
        }

 
    }
}
