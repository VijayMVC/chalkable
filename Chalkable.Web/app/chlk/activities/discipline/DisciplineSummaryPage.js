REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.discipline.DisciplinesSummary');

NAMESPACE('chlk.activities.discipline', function(){

    /** @class chlk.activities.discipline.DisciplineSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.discipline.DisciplinesSummary)],
        'DisciplineSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), []);
});