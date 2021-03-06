REQUIRE('chlk.models.attendance.StudentAttendances');
REQUIRE('chlk.models.discipline.StudentDisciplines');
REQUIRE('chlk.models.grading.GradingStat');
REQUIRE('chlk.models.attendance.AttendanceStatBox');
REQUIRE('chlk.models.common.PageWithGrades');

NAMESPACE('chlk.models.admin', function () {
    "use strict";

    /** @class chlk.models.admin.Home*/
    CLASS(
        'Home', EXTENDS(chlk.models.common.PageWithGrades), [
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

            Boolean, 'forGradeLevels',

            String, 'markingPeriodName',


            [[chlk.models.grading.GradeLevelsForTopBar, String, Object, Boolean]],
            function prepareBaseInfo(gradeLvlBarItems, mpName, balance, forGradeLevels_){
                this.setTopData(gradeLvlBarItems);
                this.setMarkingPeriodName(mpName);
                if (forGradeLevels_)
                    this.setForGradeLevels(true);
            }
        ]);
});
