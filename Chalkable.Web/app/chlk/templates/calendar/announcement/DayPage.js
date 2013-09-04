REQUIRE('chlk.templates.calendar.announcement.MonthPage');
REQUIRE('chlk.models.calendar.announcement.Day');
NAMESPACE('chlk.templates.calendar.announcement', function () {

    /** @class chlk.templates.calendar.announcement.DayPage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/dayPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Day)],
        [chlk.activities.lib.PageClass('calendar')],
        'DayPage', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.DayItem), 'items',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassesForTopBar, 'topData',

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