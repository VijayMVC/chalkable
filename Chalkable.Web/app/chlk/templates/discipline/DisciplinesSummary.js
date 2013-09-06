REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.discipline.DisciplineSummary');
REQUIRE('chlk.templates.messages.RecipientAutoComplete');

NAMESPACE('chlk.templates.discipline', function(){

    /** @class chlk.templates.discipline.DisciplinesSummary*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/discipline/disciplines-summary.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'DisciplinesSummary', EXTENDS(chlk.templates.PaginatedList), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.DisciplineSummary), 'items'


        ]);
});