REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.discipline.DisciplineSummary');
REQUIRE('chlk.models.discipline.Discipline');
REQUIRE('chlk.models.discipline.SetDisciplineListModel');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.id.ClassPeriodId');
REQUIRE('chlk.models.id.ClassPersonId');
REQUIRE('chlk.models.discipline.StudentDisciplineSummary');
REQUIRE('chlk.models.discipline.DisciplinePopupViewData');
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

            [[chlk.models.common.ChlkDate, String]],
            ria.async.Future, function summary(date_, gradeLevelsIds_){
                return this.get('Discipline/Summary.json', chlk.models.discipline.AdminDisciplineSummary,{
                    date: date_,
                    gradeLevelsIds: gradeLevelsIds_
                });
            },

            [[chlk.models.id.SchoolYearId, chlk.models.id.SchoolPersonId, chlk.models.common.ChlkDate]],
            ria.async.Future, function listStudentDiscipline(schoolYearId_, schoolPersonId, date_){
                return this.get('Discipline/ListStudentDiscipline.json', chlk.models.discipline.DisciplinePopupViewData,{
                    schoolYearId: schoolYearId_ ? schoolYearId_.valueOf() : null,
                    schoolPersonId: schoolPersonId.valueOf(),
                    date: date_ ? date_.valueOf() : null
                });
            },

            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate, Number, Number]],
            ria.async.Future, function getClassDisciplines(classId, date_, start_,count_){
                return this.get('Discipline/ClassList.json', ArrayOf(chlk.models.discipline.Discipline),{
                    classId: classId && classId.valueOf(),
                    date: date_ && date_.toStandardFormat(),
                    start: start_,
                    count: count_
                });
            },

            [[chlk.models.discipline.SetDisciplineListModel]],
            ria.async.Future, function setDisciplines(disciplines){
                return this.post('Discipline/SetClassDiscipline.json', Boolean,{
                    disciplines: disciplines.getPostData()
                });
            },

            [[chlk.models.discipline.SetDisciplineModel]],
            ria.async.Future, function setDiscipline(discipline){
                return this.post('Discipline/SetClassDiscipline.json', Boolean,{
                    discipline: discipline.getPostData()
                });
            },


            [[chlk.models.id.SchoolPersonId, chlk.models.id.MarkingPeriodId]],
            ria.async.Future, function getStudentDisciplineSummary(personId, markingPeriodId){
                return this.get('Discipline/StudentDisciplineSummary.json', chlk.models.discipline.StudentDisciplineSummary ,{
                    personId: personId && personId.valueOf(),
                    markingPeriodId: markingPeriodId && markingPeriodId.valueOf()
                });
            }

        ]
    );
});

