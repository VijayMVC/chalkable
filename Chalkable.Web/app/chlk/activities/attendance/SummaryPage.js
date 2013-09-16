REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.attendance.SummaryPage');

NAMESPACE('chlk.activities.attendance', function () {

    /** @class chlk.activities.attendance.SummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.attendance.SummaryPage)],
        'SummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [ ]);
});