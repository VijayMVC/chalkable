REQUIRE('chlk.templates.calendar.announcement.MonthPage');
REQUIRE('chlk.models.calendar.announcement.Day');
NAMESPACE('chlk.templates.calendar.announcement', function () {

    /** @class chlk.templates.calendar.announcement.DayPage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/dayPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Day)],
        [chlk.activities.lib.PageClass('calendar')],
        'DayPage', EXTENDS(chlk.templates.calendar.announcement.MonthPage), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.DayItem), 'items',

            Boolean, 'notMainCalendar',

            String, 'controllerName',

            String, 'actionName',

            Array, 'params'
        ])
});