REQUIRE('chlk.activities.attendance.SeatingChartPage');
REQUIRE('chlk.templates.attendance.ClassAttendanceStatsTpl');
REQUIRE('chlk.templates.classes.ClassProfileAttendanceTpl');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassProfileAttendanceSeatingChartPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfileAttendanceTpl)],
        [ria.mvc.PartialUpdateRule(chlk.templates.classes.ClassProfileAttendanceTpl, null, null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.SeatingChartTpl, '', '.attendances-container', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.attendance.ClassAttendanceStatsTpl, null, '.attendances-chart' , ria.mvc.PartialUpdateRuleActions.Replace)],
        'ClassProfileAttendanceSeatingChartPage', EXTENDS(chlk.activities.attendance.SeatingChartPage), [

            OVERRIDE, function getAttendancesModel(model){
                return model.getClazz().getSeatingChart();
            }

        ]);
});