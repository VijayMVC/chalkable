REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.discipline.DisciplineList');
REQUIRE('chlk.models.discipline.Discipline');
REQUIRE('chlk.models.discipline.DisciplineType');
REQUIRE('chlk.models.common.ChlkDate');


NAMESPACE('chlk.templates.discipline', function(){

    /** @class chlk.templates.discipline.SetDisciplineDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/discipline/set-discipline-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.discipline.DisciplineList)],
        'SetDisciplineDialog', EXTENDS(chlk.templates.JadeTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.Discipline), 'disciplines',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.DisciplineType), 'disciplineTypes'

        ]);
});