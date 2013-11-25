REQUIRE('chlk.templates.discipline.BaseDisciplineTpl');
REQUIRE('chlk.models.discipline.DisciplineList');

NAMESPACE('chlk.templates.discipline', function(){

    /** @class chlk.templates.discipline.SetDisciplineDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/discipline/set-discipline-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.discipline.DisciplineList)],
        'SetDisciplineDialog', EXTENDS(chlk.templates.discipline.BaseDisciplineTpl), []);
});