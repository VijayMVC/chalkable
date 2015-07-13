REQUIRE('chlk.activities.lib.TemplatePopup');
REQUIRE('chlk.templates.calendar.announcement.MonthDayLessonPlansTpl');

NAMESPACE('chlk.activities.calendar.announcement', function () {

    /** @class chlk.activities.calendar.announcement.MonthDayLessonPlansPopUp */
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        [chlk.activities.lib.IsHorizontalAxis(false)],
        [chlk.activities.lib.isTopLeftPosition(true)],
        [ria.mvc.ActivityGroup('CalendarPopUp')],
        [ria.mvc.TemplateBind(chlk.templates.calendar.announcement.MonthDayLessonPlansTpl)],
        'MonthDayLessonPlansPopUp', EXTENDS(chlk.activities.lib.TemplatePopup), []);
});