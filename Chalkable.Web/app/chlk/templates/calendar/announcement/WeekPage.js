REQUIRE('chlk.templates.calendar.announcement.MonthPage');
REQUIRE('chlk.models.calendar.announcement.Week');
NAMESPACE('chlk.templates.calendar.announcement', function () {

    /** @class chlk.templates.calendar.announcement.WeekPage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/WeekPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Week)],
        [chlk.activities.lib.PageClass('calendar')],
        'WeekPage', EXTENDS(chlk.templates.calendar.announcement.MonthPage), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.WeekItem), 'items'
        ])
});