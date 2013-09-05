REQUIRE('chlk.models.attendance.StudentAttendances');
REQUIRE('chlk.models.discipline.StudentDisciplines');
REQUIRE('chlk.models.grading.GradingStat');
REQUIRE('chlk.models.attendance.AttendanceStatBox');
REQUIRE('chlk.models.funds.BudgetBalance');

NAMESPACE('chlk.models.admin', function () {
    "use strict";

    /** @class chlk.models.admin.Home*/
    CLASS(
        'Home', [
            ArrayOf(chlk.models.attendance.StudentAttendances), 'attendances',

            ArrayOf(chlk.models.discipline.StudentDisciplines), 'disciplines',

            [ria.serialize.SerializeProperty('gradingstats')],
            ArrayOf(chlk.models.grading.GradingStat), 'gradingStats',

            [ria.serialize.SerializeProperty('notinclassbox')],
            chlk.models.attendance.AttendanceStatBox, 'notInClassBox',

            [ria.serialize.SerializeProperty('absentforday')],
            chlk.models.attendance.AttendanceStatBox, 'absentForDay',

            [ria.serialize.SerializeProperty('absentformp')],
            chlk.models.attendance.AttendanceStatBox, 'absentForMp',

            chlk.models.funds.BudgetBalance, 'budgetBalance',

            String, 'markingPeriodName'
        ]);
});
