REQUIRE('chlk.templates.attendance.SeatingChartTpl');
REQUIRE('chlk.models.attendance.SeatingChart');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.SeatingChartPeopleTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/SeatingChartPeople.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.SeatingChart)],
        'SeatingChartPeopleTpl', EXTENDS(chlk.templates.attendance.SeatingChartTpl), []);
});