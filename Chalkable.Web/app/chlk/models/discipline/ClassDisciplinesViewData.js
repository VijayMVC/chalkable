REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.discipline.Discipline');

NAMESPACE('chlk.models.discipline', function(){
   "use strict";
    /**@class chlk.models.discipline.ClassDisciplinesViewData*/

    CLASS('ClassDisciplinesViewData', EXTENDS(chlk.models.common.PageWithClasses), [

        ArrayOf(chlk.models.discipline.Discipline), 'disciplines',

        ArrayOf(chlk.models.discipline.DisciplineType), 'disciplineTypes',

        chlk.models.common.ChlkDate, 'date',

        Boolean, 'byLastName',

        String, 'filter',

        Boolean, 'ablePostDiscipline',

        [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId
            , ArrayOf(chlk.models.discipline.Discipline)
            , ArrayOf(chlk.models.discipline.DisciplineType)
            ,chlk.models.common.ChlkDate, Boolean, Boolean
        ]],
        function $(classes_, classId_, disciplines_, disciplineTypes_, date_, byLastName_, isAblePostDiscipline_){
            BASE(classes_, classId_);
            if(disciplines_)
                this.setDisciplines(disciplines_);
            if(disciplineTypes_)
                this.setDisciplineTypes(disciplineTypes_);
            this.setDate(date_ || chlk.models.common.ChlkDate(getDate()));
            this.setByLastName(byLastName_ || false);
            this.setAblePostDiscipline(isAblePostDiscipline_ || false);
        }
    ]);
});