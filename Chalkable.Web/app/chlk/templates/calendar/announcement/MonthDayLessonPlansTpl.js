REQUIRE('chlk.templates.calendar.announcement.MonthDay');

NAMESPACE('chlk.templates.calendar.announcement', function () {

    /** @class chlk.templates.calendar.announcement.MonthDayLessonPlansTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/calendar/announcement/MonthDayLessonPlansPopUp.jade')],
        [ria.templates.ModelBind(chlk.models.calendar.announcement.MonthItem)],
        'MonthDayLessonPlansTpl', EXTENDS(chlk.templates.calendar.announcement.MonthDay), [])
});