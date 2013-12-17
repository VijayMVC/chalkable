REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.attendance.AttendanceDaySummary');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.AdminAttendanceDayTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/AdminSummaryDay.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.AttendanceDaySummary)],
        'AdminAttendanceDayTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AdminAttendanceStatItem), 'attendancesStats',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'studentsAbsentWholeDay',

            [ria.templates.ModelPropertyBind],
            Number, 'studentsCountAbsentWholeDay',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'absentStudents',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'excusedStudents',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.User), 'lateStudents',

            function getSortedBlocksInfo(){
                var array = [[1, this.getStudentsAbsentWholeDay() ? this.getStudentsAbsentWholeDay().length : 0],
                    [2, this.getExcusedStudents() ? this.getExcusedStudents().length : 0],
                    [3, this.getAbsentStudents() ? this.getAbsentStudents().length : 0],
                    [4, this.getLateStudents() ? this.getLateStudents().length : 0]
                ];
                return array.sort(function(a,b){
                    return a[1] < b[1];
                })
            }
        ])
});