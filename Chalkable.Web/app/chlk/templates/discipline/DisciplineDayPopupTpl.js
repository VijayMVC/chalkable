REQUIRE('chlk.templates.common.AttendanceDisciplinePopUpTpl');
REQUIRE('chlk.models.discipline.DisciplinePopupViewData');

NAMESPACE('chlk.templates.discipline', function(){

    /** @class chlk.templates.discipline.DisciplineDayPopupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/discipline/StudentDayDiscipline.jade')],
        [ria.templates.ModelBind(chlk.models.discipline.DisciplinePopupViewData)],
        'DisciplineDayPopupTpl', EXTENDS(chlk.templates.common.AttendanceDisciplinePopUpTpl), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.Discipline), 'disciplines',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.DisciplineType), 'disciplineTypes',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [[chlk.models.discipline.Discipline]],
            Object,  function prepareItemForDisciplinesContainer(item){
                var disciplineTypes = this.getDisciplineTypes();
                var itemDisciplineTypes = item.getDisciplineTypes();
                var res = [];
                itemDisciplineTypes.forEach(function(_){
                    if(_.getName())
                        res.push({
                            id: _.getId(),
                            name: _.getName(),
                            visible: true
                        });
                });
                for(var i = 0; i < disciplineTypes.length; i++){
                    if(res.filter(function(_){return _.id == disciplineTypes[i].getId()}).length == 0){
                        res.push({
                            id: disciplineTypes[i].getId(),
                            name: disciplineTypes[i].getName(),
                            visible: false
                        });
                    }
                }
                return res;
            }
        ]);
});