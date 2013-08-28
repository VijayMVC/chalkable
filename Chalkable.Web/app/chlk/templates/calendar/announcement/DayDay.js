REQUIRE('chlk.templates.Popup');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.calendar.announcement.CalendarDayItem');
NAMESPACE('chlk.templates.calendar.announcement', function () {

    /** @class chlk.templates.calendar.announcement.DayDay*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/DayPeriodPopUp.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.CalendarDayItem)],
        'DayDay', EXTENDS(chlk.templates.Popup), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.AnnouncementClassPeriod), 'announcementClassPeriods',

            [ria.templates.ModelPropertyBind],
            chlk.models.period.Period, 'period',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date'
        ])
});