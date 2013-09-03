REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.calendar.announcement.MonthPage');

NAMESPACE('chlk.activities.calendar.announcement', function () {

    /** @class chlk.activities.calendar.announcement.MonthPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('calendar')],
        [ria.mvc.TemplateBind(chlk.templates.calendar.announcement.MonthPage)],
        'MonthPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.calendar.announcement.MonthItem), 'items',

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
        ]);
});