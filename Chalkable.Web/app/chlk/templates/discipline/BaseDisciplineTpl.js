REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.discipline.DisciplineList');

NAMESPACE('chlk.templates.discipline', function(){

    /** @class chlk.templates.discipline.BaseDisciplineTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.discipline.DisciplineList)],
        'BaseDisciplineTpl', EXTENDS(chlk.templates.JadeTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.Discipline), 'disciplines',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.DisciplineType), 'disciplineTypes'

        ]);
});