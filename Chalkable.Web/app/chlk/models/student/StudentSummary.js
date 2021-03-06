REQUIRE('chlk.models.people.PersonSummary');
REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.common.HoverBox');

REQUIRE('chlk.models.common.AttendanceHoverBoxItem');
REQUIRE('chlk.models.common.DisciplineHoverBoxItem');
REQUIRE('chlk.models.student.StudentGradesHoverBoxItem');

REQUIRE('chlk.models.attendance.ClassAttendance');

NAMESPACE('chlk.models.student', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.student.StudentSummary*/
    CLASS(
        'StudentSummary', EXTENDS(chlk.models.people.PersonSummary), IMPLEMENTS(ria.serialize.IDeserializable), [

            function $(){
                BASE();
                this._attendanceTypeMapper = new chlk.models.attendance.AttendanceTypeMapper();
            },

            OVERRIDE, VOID, function deserialize(raw){
                BASE(raw);
                this.gradeLevelNumber = SJX.fromValue(raw.gradelevelnumber, Number);
                this.currentClassName = SJX.fromValue(raw.currentclassname, String);
                this.currentAttendanceLevel = SJX.fromValue(raw.currentattendancelevel, String);
                this.maxPeriodNumber = SJX.fromValue(raw.maxperiodnumber, Number);

                this.classesSection = SJX.fromArrayOfDeserializables(raw.classessection, chlk.models.classes.Class);

                this.attendanceBox = SJX.fromDeserializable(raw.attendancebox, chlk.models.common.HoverBox.OF(chlk.models.common.AttendanceHoverBoxItem));
                this.disciplineBox = SJX.fromDeserializable(raw.disciplinebox, chlk.models.common.HoverBox.OF(chlk.models.common.DisciplineHoverBoxItem));
                this.gradesBox = SJX.fromDeserializable(raw.gradesbox, chlk.models.common.HoverBox.OF(chlk.models.student.StudentGradesHoverBoxItem));
            },

            Number, 'gradeLevelNumber',

            String, 'currentClassName',

            String, 'currentAttendanceLevel',

            Boolean, 'ableViewTranscript',

            READONLY, Number, 'currentAttendanceType',
            Number, function getCurrentAttendanceType(){
                return this._attendanceTypeMapper.map(this.getCurrentAttendanceLevel()).valueOf();
            },

            Number, 'maxPeriodNumber',


            chlk.models.common.HoverBox.OF(chlk.models.common.AttendanceHoverBoxItem), 'attendanceBox',

            chlk.models.common.HoverBox.OF(chlk.models.common.DisciplineHoverBoxItem), 'disciplineBox',

            chlk.models.common.HoverBox.OF(chlk.models.student.StudentGradesHoverBoxItem), 'gradesBox',

            ArrayOf(chlk.models.classes.Class), 'classesSection'

        ]);
});
