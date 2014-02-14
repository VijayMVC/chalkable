REQUIRE('ria.templates.IConverter');
REQUIRE('chlk.models.attendance.ClassAttendance');

NAMESPACE('chlk.converters.attendance', function () {

    /** @class chlk.converters.attendance.AttendanceLevelToTypeConverter */
    CLASS(
        'AttendanceLevelToTypeConverter', IMPLEMENTS(ria.templates.IConverter.OF(Object, chlk.models.attendance.AttendanceTypeEnum)), [

            [[Object]],
            chlk.models.attendance.AttendanceTypeEnum, function convert(level) {
                VALIDATE_ARG('level', String, level);
                var attLvEnum = chlk.models.attendance.AttendanceLevelEnum;
                var attTypeEnum = chlk.models.attendance.AttendanceTypeEnum;
                if(!level) return attTypeEnum.PRESENT;
                switch (level){
                    case attLvEnum.ABSENT_LEVEL : return attTypeEnum.ABSENT;
                    case attLvEnum.LATE_LEVEL : return attTypeEnum.LATE;
                }
                throw new Exception('Unknown attendance level ');
            }
        ])
});