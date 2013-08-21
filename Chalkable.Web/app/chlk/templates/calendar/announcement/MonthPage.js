REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.calendar.announcement.Month');
NAMESPACE('chlk.templates.calendar.announcement', function () {

    /** @class chlk.templates.calendar.announcement.MonthPage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/MonthPage.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.Month)],
        [chlk.activities.lib.PageClass('calendar')],
        'MonthPage', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.Day), 'days',

            [ria.templates.ModelPropertyBind],
            chlk.models.class.ClassesForTopBar, 'topData',

            [ria.templates.ModelPropertyBind],
            Number, 'selectedTypeId',

            [ria.templates.ModelPropertyBind],
            String, 'currentMonth',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'nextMonthDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'prevMonthDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'currentDate'
        ])
});