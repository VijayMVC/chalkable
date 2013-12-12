REQUIRE('chlk.templates.Popup');
REQUIRE('chlk.models.discipline.DisciplinePopupViewData');

NAMESPACE('chlk.templates.discipline', function(){

    /** @class chlk.templates.discipline.DisciplinePopupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/discipline/CalendarDayDisciplineView.jade')],
        [ria.templates.ModelBind(chlk.models.discipline.DisciplinePopupViewData)],
        'DisciplinePopupTpl', EXTENDS(chlk.templates.Popup), [

            [ria.templates.ModelPropertyBind],
            chlk.models.discipline.DisciplineList, 'disciplineList',

            [ria.templates.ModelPropertyBind],
            String, 'controller',
            [ria.templates.ModelPropertyBind],
            String, 'action',
            [ria.templates.ModelPropertyBind],
            String, 'params',


            ArrayOf(chlk.models.discipline.Discipline), function getDisciplines(){
                return this.getDisciplineList().getDisciplines();
            },
            ArrayOf(chlk.models.discipline.DisciplineType), function getDisciplineTypes(){
                return this.getDisciplineList().getDisciplineTypes();
            },
            chlk.models.common.ChlkDate, function getDate(){return this.getDisciplineList().getDate();},

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