REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.discipline.DisciplineSummary');
REQUIRE('chlk.models.discipline.Discipline');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.SchoolYearId');
//REQUIRE('chlk.models.developer.DeveloperInfo');

NAMESPACE('chlk.services', function(){
    "use strict";
    /** @class chlk.services.DisciplineService*/
    CLASS(
        'DisciplineService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.common.ChlkDate, Number, Number]],
            ria.async.Future, function list(date_, start_, count_){
                return this.getPaginatedList('Discipline/List.json', chlk.models.discipline.DisciplineSummary,{
                    date: date_,
                    start: start_,
                    count: count_
                });
            },

            [[chlk.models.id.SchoolYearId, chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            ria.async.Future, function listStudentDiscipline(schoolYearId_, schoolPersonId, date_){
                return this.get('Discipline/ListStudentDiscipline.json', ArrayOf(chlk.models.discipline.Discipline),{
                    schoolYearId: schoolYearId_ ? schoolYearId_.valueOf() : null,
                    schoolPersonId: schoolPersonId.valueOf(),
                    date: date_ ? date_.valueOf() : null
                });
            }
        ]
    );
});

