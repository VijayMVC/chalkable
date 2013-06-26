using System;
using System.Collections.Generic;
using Chalkable.BusinessLogic.Security;
using Chalkable.Common;
using Chalkable.Common.Exceptions;
using Chalkable.Data.School.DataAccess;
using Chalkable.Data.School.Model;

namespace Chalkable.BusinessLogic.Services.School
{
    public interface ICalendarDateService
    {
        Date GetCalendarDateByDate(DateTime date);
        Date GetById(Guid id);
        DateTime GetDbDateTime();
        IList<Date> GetDays(Guid markingPeriodId, bool schoolDaysOnly, DateTime? fromDate = null, DateTime? tillDate = null);
        IList<Date> GetLastDays(Guid schoolYearId, bool schoolDaysOnly, DateTime? fromDate, DateTime? tillDate, int count = int.MaxValue);
        void ClearCalendarDates(Guid markingPeriodId);
        bool CanAssignDate(Guid id, Guid sectionId);
        void AssignDate(Guid id, Guid sectionId);
        Date Add(DateTime date, bool schoolDay, Guid? markingPeriodId, Guid? scheduleSectionId, int? sisId);
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
                var res = da.GetDate(new DateQuery {FromDate = date, ToDate = date});
                if (res == null)
                {
                    var mp = ServiceLocator.MarkingPeriodService.GetMarkingPeriodByDate(date);
                    if (mp == null)
                    {
                        res = new Date { DateTime = date };
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

                        var dates = new List<Date>();
                        for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                        {
                            var d = new Date
                            {
                                MarkingPeriodRef = mp.Id,
                                IsSchoolDay = false,
                                DateTime = dt
                            };
                            dates.Add(d);
                            if (((1 << (int)dt.DayOfWeek) & mp.WeekDays) != 0)
                            {
                                d.IsSchoolDay = true;
                                d.ScheduleSectionRef = sections[sectionIndex].Id;
                                sectionIndex = (sectionIndex + 1) % sections.Count;
                            }
                            if (d.DateTime == date) res = d;
                        }
                    }
                    da.Insert(res);
                }
                uow.Commit();
                return res;
            }
        }

        public Date GetById(Guid id)
        {
            using (var uow = Read())
            {
                return new DateDataAccess(uow).GetDate(new DateQuery{Id = id});
            }
        }

        public DateTime GetDbDateTime()
        {
            using (var uow = Read())
            {
                return new DateDataAccess(uow).GetDbDateTime();
            }
        }

        public IList<Date> GetDays(Guid markingPeriodId, bool schoolDaysOnly, DateTime? fromDate = null, DateTime? tillDate = null)
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
                            ToDate = tillDate
                        });

            }
        }

        public IList<Date> GetLastDays(Guid schoolYearId, bool schoolDaysOnly, DateTime? fromDate, DateTime? tillDate, int count = Int32.MaxValue)
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
                return da.GetDates(new DateQuery
                    {
                        SchoolYearId = schoolYearId,
                        FromDate = fromDate,
                        ToDate = tillDate,
                        Count = count,
                        SchoolDaysOnly = schoolDaysOnly
                    });
            }
        }

        public void ClearCalendarDates(Guid markingPeriodId)
        {
            if(!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();
            using (var uow = Update())
            {
                new DateDataAccess(uow).Delete(new DateQuery{ MarkingPeriodId = markingPeriodId});
                uow.Commit();
            }
        }

        public bool CanAssignDate(Guid id, Guid sectionId)
        {
            var cdDate = GetById(id);
            if (!BaseSecurity.IsAdminEditor(Context))
                throw new ChalkableSecurityException();

            var section = ServiceLocator.ScheduleSectionService.GetSectionById(sectionId);
            if (section.MarkingPeriodRef != cdDate.MarkingPeriodRef)
                throw new ChalkableException(ChlkResources.ERR_SECTION_NOT_IN_MARKING_PERIOD_FOR_CURRENT_DAY);

            //TODO : needs attendances and discipline existing
          //  var date = cdDate.Date1;
            //return !Entities.ClassAttendances.Any(x => x.ClassGeneralPeriod.GeneralPeriod.MarkingPeriodRef == section.MarkingPeriodRef && x.Date >= date)
            //    && !Entities.ClassDisciplines.Any(x => x.ClassGeneralPeriod.GeneralPeriod.MarkingPeriodRef == section.MarkingPeriodRef && x.Date >= date);


            throw new NotImplementedException();
        }

        public void AssignDate(Guid id, Guid sectionId)
        {
            if(!CanAssignDate(id,sectionId))
                throw new ChalkableException(ChlkResources.ERR_CANT_REASSIGN_DAY_TO_DIFFERENT_SECTION);

            using (var uow = Update())
            {
                var cdDate = GetById(id);
                cdDate.ScheduleSectionRef = sectionId;
                new DateDataAccess(uow).Update(cdDate);
                uow.Commit();
            }
        }

        public Date Add(DateTime date, bool schoolDay, Guid? markingPeriodId, Guid? scheduleSectionId, int? sisId)
        {
            using (var uow = Update())
            {
                var res = new Date
                    {
                        Id = Guid.NewGuid(),
                        DateTime = date,
                        IsSchoolDay = schoolDay,
                        MarkingPeriodRef = markingPeriodId,
                        ScheduleSectionRef = scheduleSectionId,
                        SisId = sisId
                    };
                new DateDataAccess(uow).Insert(res);
                uow.Commit();
                return res;
            }
        }
    }
}
