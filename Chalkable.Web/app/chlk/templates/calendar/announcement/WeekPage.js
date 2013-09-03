REQUIRE('chlk.templates.calendar.announcement.MonthPage');
REQUIRE('chlk.models.calendar.announcement.Week');
NAMESPACE('chlk.templates.calendar.announcement', function () {

    /** @class chlk.templates.calendar.announcement.WeekPage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/WeekPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Week)],
        [chlk.activities.lib.PageClass('calendar')],
        'WeekPage', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.WeekItem), 'items',
            [ria.templates.ModelPropertyBind],
            chlk.models.class.ClassesForTopBar, 'topData',

            [ria.templates.ModelPropertyBind],
            Number, 'selectedTypeId',

            [ria.templates.ModelPropertyBind],
            String, 'currentTitle',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'nextDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'prevDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'currentDate'
        ])
});