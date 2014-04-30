REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.discipline.ClassDisciplinesViewData');

NAMESPACE('chlk.templates.discipline',function(){
   "use strict";
    /**@class chlk.templates.discipline.ClassDisciplinesTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/discipline/ClassDisciplinesView.jade')],
        [ria.templates.ModelBind(chlk.models.discipline.ClassDisciplinesViewData)],
        'ClassDisciplinesTpl', EXTENDS(chlk.templates.discipline.BaseDisciplineTpl),[

            function getSelectedItemId(){return this.getModel().getSelectedItemId();},

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassesForTopBar, 'topData',

            [ria.templates.ModelPropertyBind],
            Boolean, 'byLastName',

            [ria.templates.ModelPropertyBind],
            String, 'filter',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ablePostDiscipline',

            String, function getTotalText(){
                var disciplines = this.getDisciplines();
                var res = disciplines.length + ' ' + Msg.Student(disciplines.length != 1);
                return res;
            },

            Object, function getRedirectData(){
                return{
                    controller: 'discipline',
                    action: 'classList',
                    params: JSON.stringify([this.getTopData().getSelectedItemId().valueOf(), this.getDate().toStandardFormat()])
                }
            },

            [[chlk.models.discipline.Discipline]],
            String, function getSelectedTypeIds(discipline){
                var selectedDisciplineTypes = discipline.getDisciplineTypes();
                var res = [];
                for(var i = 0; i < selectedDisciplineTypes.length; i++){
                    res.push(selectedDisciplineTypes[i].getId().valueOf());
                }
                return res.join(',');
            },

            [[chlk.models.discipline.Discipline]],
            Array, function getGroupDisciplineTypes(discipline){
                var res = [];
                var disciplineTypes = this.getDisciplineTypes();
                var selectedDisciplineTypes = discipline.getDisciplineTypes();
                for(var i = 0; i < disciplineTypes.length; i++){
                    res.push({
                        disciplineTypeData: disciplineTypes[i],
                        selected: selectedDisciplineTypes.filter(function(item){return item.getId() == disciplineTypes[i].getId();}).length > 0
                    });
                }

                var half = Math.ceil(res.length / 2);
                if(res.length < 6)
                    half = 5;

                return [res.slice(0, half)
                        , res.slice(half)
                    ];
            }
    ]);
});