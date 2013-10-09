REQUIRE('chlk.models.discipline.Discipline');
REQUIRE('chlk.models.discipline.DisciplineType');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.discipline', function(){
    "use strict";

    /** @class chlk.models.discipline.DisciplineList*/
    CLASS(
        'DisciplineList',[

            ArrayOf(chlk.models.discipline.Discipline), 'disciplines',

            ArrayOf(chlk.models.discipline.DisciplineType), 'disciplineTypes',

            chlk.models.common.ChlkDate, 'date',

            [[ArrayOf(chlk.models.discipline.Discipline), ArrayOf(chlk.models.discipline.DisciplineType), chlk.models.common.ChlkDate]],
            function $(disciplines, disciplineTypes_, date_){
                BASE();
                if(disciplines)
                    this.setDisciplines(disciplines);
                if(disciplineTypes_)
                    this.setDisciplineTypes(disciplineTypes_);
                if(date_)
                    this.setDate(date_);
            }
    ]);
});