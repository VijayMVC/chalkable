REQUIRE('chlk.activities.attendance.SeatingChartPage');
REQUIRE('chlk.templates.attendance.ClassAttendanceStatsTpl');
REQUIRE('chlk.templates.classes.ClassProfileAttendanceTpl');
REQUIRE('chlk.templates.calendar.attendance.ClassAttendanceMonthCalendarTpl');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassProfileAttendancePage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfileAttendanceTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.SeatingChartTpl, '', '.attendances-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.ClassAttendanceStatsTpl, null, '.attendances-chart' , ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassProfileAttendancePage', EXTENDS(chlk.activities.attendance.SeatingChartPage), [

            OVERRIDE, function getAttendancesModel(model){
                return model.getClazz().getAttendances();
            }

        ]);
});