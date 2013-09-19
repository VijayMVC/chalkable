REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.discipline.DisciplineType');
REQUIRE('chlk.models.id.DisciplineTypeId');

NAMESPACE('chlk.templates.discipline', function(){
    "use strict";

    /**@class chlk.templates.discipline.AddDisciplineTypeTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/discipline/add-discipline-type-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.discipline.DisciplineType)],
        'AddDisciplineTypeTpl', EXTENDS(chlk.templates.JadeTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.id.DisciplineTypeId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'name',

            [ria.templates.ModelPropertyBind],
            Number, 'score'
        ]);
});