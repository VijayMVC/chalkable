REQUIRE('chlk.models.discipline.StudentDisciplineSummary');
REQUIRE('chlk.models.calendar.discipline.StudentDisciplineMonthCalendar');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');

NAMESPACE('chlk.models.student', function(){
    "use strict";

    /**@class chlk.models.student.StudentProfileDisciplineViewData*/
    CLASS('StudentProfileDisciplineViewData',[

        chlk.models.discipline.StudentDisciplineSummary, 'summaryInfo',
        chlk.models.calendar.discipline.StudentDisciplineMonthCalendar, 'disciplineCalendar',
        ArrayOf(chlk.models.schoolYear.MarkingPeriod), 'markingPeriods',

        [[chlk.models.discipline.StudentDisciplineSummary
            , chlk.models.calendar.discipline.StudentDisciplineMonthCalendar
            , ArrayOf(chlk.models.schoolYear.MarkingPeriod)
        ]],
        function $(summaryInfo, disciplineCalendar, markingPeriods){
            BASE();
            this.setSummaryInfo(summaryInfo);
            this.setDisciplineCalendar(disciplineCalendar);
            this.setMarkingPeriods(markingPeriods);
        }
    ]);
});