REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.converters.attendance.AttendanceTypeToNameConverter');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.ClassAttendanceTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/StudentAttendance.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.ClassAttendance)],
        'ClassAttendanceTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassAttendanceId, 'id',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            Number, 'type',

            [ria.templates.ModelPropertyBind],
            Boolean, 'absentPreviousDay',

            [ria.templates.ModelPropertyBind],
            String, 'level',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            String, function getTypeName(){
                return this.getModel().getTypeName();
            },

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'student',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AttendanceReasonId, 'attendanceReasonId',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceReason, 'attendanceReason',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            Boolean, 'needPresent',

            String, function getSubmitFormActionName(){
                return 'setAttendanceProfile';
            },

            function getAttendanceReasonsByType(type){
                var attLevelReasons = this.getReasons();
                if(!attLevelReasons) return [];
                return attLevelReasons.filter(function(item){
                    return type == chlk.models.attendance.AttendanceTypeEnum.LATE.valueOf() ?
                        item.hasLevel('T') : item.hasLevel('A') || item.hasLevel('AO') ||
                            item.hasLevel('H') || item.hasLevel('HO');
                });
            },
        ]);
});