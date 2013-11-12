REQUIRE('chlk.models.id.ClassAttendanceId');
REQUIRE('chlk.models.id.ClassPersonId');
REQUIRE('chlk.models.id.ClassPeriodId');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.id.AttendanceReasonId');
REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.period.Period');
REQUIRE('chlk.models.attendance.AttendanceReason');
//REQUIRE('chlk.converters.attendance.AttendanceLevelToTypeConverter');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.AttendanceTypeEnum*/
    ENUM(
        'AttendanceTypeEnum', {
            NA: 1,
            PRESENT: 2,
            EXCUSED: 4,
            ABSENT: 8,
            LATE: 16
        });

    /** @class chlk.models.attendance.AttendanceLevelEnum*/
    ENUM('AttendanceLevelEnum',{
        ABSENT_LEVEL: 'A',
        LATE_LEVEL: 'T'
    });

    /** @class chlk.models.attendance.AttendanceTypeMapper*/
    CLASS('AttendanceTypeMapper', [

        function $(){
            BASE();
            this._attLevelEnum = chlk.models.attendance.AttendanceLevelEnum;
            this._attTypeEnum = chlk.models.attendance.AttendanceTypeEnum;
        },
        [[String]],
        chlk.models.attendance.AttendanceTypeEnum, function mapBack(level){
            if(!level) return this._attTypeEnum.PRESENT;
            switch (level){
                case this._attLevelEnum.ABSENT_LEVEL.valueOf() : return this._attTypeEnum.ABSENT;
                case this._attLevelEnum.LATE_LEVEL.valueOf() : return this._attTypeEnum.LATE;
            }
            throw new Exception('Unknown attendance level ');
        },
        [[chlk.models.attendance.AttendanceTypeEnum]],
        String, function map(type){
            switch (type){
                case this._attTypeEnum.ABSENT : return this._attLevelEnum.ABSENT_LEVEL.valueOf();
                case this._attTypeEnum.LATE : return this._attLevelEnum.LATE_LEVEL.valueOf();
                case this._attTypeEnum.PRESENT : return null;
            }
            throw new Exception('Unknown attendance type ');
        }
    ]);

    /** @class chlk.models.attendance.ClassAttendance*/
    CLASS(
        'ClassAttendance', [

            function $(){
                BASE();
                this._studentId = null;
                this._student = null;
                this._attendanceTypeMapper = new chlk.models.attendance.AttendanceTypeMapper();
            },

            chlk.models.id.ClassAttendanceId, 'id',
            chlk.models.common.ChlkDate, 'date',

            //todo change number to AttendanceTypeEnum
            Number, 'type',
            Number, function getType(){
                return this._attendanceTypeMapper.mapBack(this.getLevel()).valueOf();
            },
            [[Number]],
            VOID, function setType(type){
               var level = this._attendanceTypeMapper.map(new chlk.models.attendance.AttendanceTypeEnum(type));
               this.setLevel(level);
            },

            String, 'level',

//            chlk.models.period.Period, 'period',

            chlk.models.people.User, 'student',

            [[chlk.models.people.User]],
            VOID, function setStudent(student){
                this._student = student;
                if(student)
                    this.setStudentId(student.getId());
            },
            chlk.models.people.User, function getStudent(){
                return this._student;
            },

            [ria.serialize.SerializeProperty('studentid')],
            chlk.models.id.SchoolPersonId, 'studentId',

            chlk.models.id.SchoolPersonId, function getStudentId(){
                return this._studentId || (this.getStudent() ? this.getStudent().getId() : null);
            },
            [[chlk.models.id.SchoolPersonId]],
            VOID, function setStudentId(studentId){
                this._studentId = studentId;
            },


            [ria.serialize.SerializeProperty('attendancereasonid')],
            chlk.models.id.AttendanceReasonId, 'attendanceReasonId',

            [ria.serialize.SerializeProperty('attendancereason')],
            chlk.models.attendance.AttendanceReason, 'attendanceReason',

            [ria.serialize.SerializeProperty('classname')],
            String, 'className',

            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId',

            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            String, 'submitType',

            String, 'attendanceReasonDescription'
        ]);
});
