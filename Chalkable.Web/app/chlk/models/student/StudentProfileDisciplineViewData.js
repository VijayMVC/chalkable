REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.discipline.StudentDisciplineSummary');
REQUIRE('chlk.models.calendar.discipline.StudentDisciplineMonthCalendar');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    /**@class chlk.models.student.StudentProfileDisciplineViewData*/
    CLASS('StudentProfileDisciplineViewData',EXTENDS(chlk.models.people.UserProfileViewData.OF(chlk.models.discipline.StudentDisciplineSummary)), [

        chlk.models.calendar.discipline.StudentDisciplineMonthCalendar, 'disciplineCalendar',

        chlk.models.discipline.StudentDisciplineSummary, function getSummaryInfo(){return this.getUser();},

        [[chlk.models.common.Role,
            chlk.models.discipline.StudentDisciplineSummary
            , chlk.models.calendar.discipline.StudentDisciplineMonthCalendar
            , ArrayOf(chlk.models.people.Claim)
        ]],
        function $(role, summaryInfo, disciplineCalendar, claims_){
            BASE(role, summaryInfo, claims_);
            this.setDisciplineCalendar(disciplineCalendar);
        }
    ]);
});