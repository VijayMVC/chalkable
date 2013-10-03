REQUIRE('chlk.models.people.UserProfileViewData');
REQUIRE('chlk.models.discipline.StudentDisciplineSummary');
REQUIRE('chlk.models.calendar.discipline.StudentDisciplineMonthCalendar');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    /**@class chlk.models.student.StudentProfileDisciplineViewData*/
    CLASS('StudentProfileDisciplineViewData',EXTENDS(chlk.models.people.UserProfileViewData), [

        chlk.models.calendar.discipline.StudentDisciplineMonthCalendar, 'disciplineCalendar',
        ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'markingPeriods',

        chlk.models.discipline.StudentDisciplineSummary, function getSummaryInfo(){
            return this.getUser();
        },

        [[chlk.models.common.Role,
            chlk.models.discipline.StudentDisciplineSummary
            , chlk.models.calendar.discipline.StudentDisciplineMonthCalendar
            , ArrayOf(chlk.models.schoolYear.MarkingPeriod)
        ]],
        function $(role, summaryInfo, disciplineCalendar, markingPeriods){
            BASE(role, summaryInfo);
            this.setDisciplineCalendar(disciplineCalendar);
            this.setMarkingPeriods(markingPeriods);
        }
    ]);
});