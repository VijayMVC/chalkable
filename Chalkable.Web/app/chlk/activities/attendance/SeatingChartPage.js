REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.SeatingChartTpl');

NAMESPACE('chlk.activities.attendance', function () {
    "use strict";

    /** @class chlk.activities.attendance.SeatingChartPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.SeatingChartTpl)],
        'SeatingChartPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});