REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.discipline.DisciplineSummary');
REQUIRE('chlk.models.discipline.PaginatedListByDateModel');

NAMESPACE('chlk.templates.discipline', function(){

    /** @class chlk.templates.discipline.DisciplinesSummary*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/discipline/disciplines-summary.jade')],
        [ria.templates.ModelBind(chlk.models.discipline.PaginatedListByDateModel)],
        'DisciplinesSummary', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'items',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date'

        ]);
});