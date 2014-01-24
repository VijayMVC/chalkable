REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.discipline.SetDisciplineListModel');

NAMESPACE('chlk.templates.discipline',function(){
   "use strict";
    /**@class chlk.templates.discipline.DisciplineTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/empty.jade')],
        [ria.templates.ModelBind(chlk.models.discipline.SetDisciplineModel)],
        'DisciplineTpl', EXTENDS(chlk.templates.ChlkTemplate),[

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassPeriodId, 'classPeriodId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassPersonId, 'classPersonId',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            String, 'description',

            [ria.templates.ModelPropertyBind],
            String, 'disciplineTypeIds',

            [ria.templates.ModelPropertyBind],
            Number, 'time'
    ]);
});