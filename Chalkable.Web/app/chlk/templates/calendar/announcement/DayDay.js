REQUIRE('chlk.templates.Popup');
REQUIRE('chlk.models.calendar.announcement.DayItem');
NAMESPACE('chlk.templates.calendar.announcement', function () {

    /** @class chlk.templates.calendar.announcement.DayDay*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/DayPopUp.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.DayItem)],
        'DayDay', EXTENDS(chlk.templates.Popup), [
            [ria.templates.ModelPropertyBind],
            Number, 'day',

            [ria.templates.ModelPropertyBind],
            String, 'todayClassName',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.CalendarDayItem), 'calendarDayItems'
        ])
});