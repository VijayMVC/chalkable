REQUIRE('chlk.templates.PaginatedList');
REQUIRE('chlk.models.common.PaginatedList');
REQUIRE('chlk.models.discipline.DisciplineType');

NAMESPACE('chlk.templates.discipline', function(){
    "use strict";

    /** @class chlk.templates.discipline.DisciplineTypeListTpl */
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/discipline/disciplines-type-list.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'DisciplineTypeListTpl', EXTENDS(chlk.templates.PaginatedList),[

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.DisciplineType), 'items'
        ]);
});