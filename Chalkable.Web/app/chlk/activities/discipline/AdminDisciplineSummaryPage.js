REQUIRE('chlk.activities.attendance.BaseSummaryPage');
REQUIRE('chlk.templates.discipline.AdminDisciplinesSummaryTpl');

NAMESPACE('chlk.activities.discipline', function(){

    /** @class chlk.activities.discipline.AdminDisciplineSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.discipline.AdminDisciplinesSummaryTpl)],
        'AdminDisciplineSummaryPage', EXTENDS(chlk.activities.attendance.BaseSummaryPage), []);
});