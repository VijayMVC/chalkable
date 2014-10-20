REQUIRE('chlk.models.id.ClassPersonId');
REQUIRE('chlk.models.id.ClassPeriodId');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.ShortAttendanceSummary*/
    CLASS('ShortAttendanceSummary',  [

        Number, 'count',
        [ria.serialize.SerializeProperty('attendancetype')],
        Number, 'attendanceType',

        String, function getAttendanceTypeName(){
            var attType = this.getAttendanceType();
            if(attType == null) return null;
            var converter = new  chlk.converters.attendance.AttendanceTypeToNameConverter();
            return converter.convert(attType);
        },

        [ria.serialize.SerializeProperty('personid')],
        chlk.models.id.SchoolPersonId, 'personId',

        [ria.serialize.SerializeProperty('teacherid')],
        chlk.models.id.SchoolPersonId, 'teacherId',

        [ria.serialize.SerializeProperty('periodid')],
        chlk.models.id.PeriodId, 'periodId',

        [ria.serialize.SerializeProperty('periodorder')],
        Number, 'periodOrder',

        [ria.serialize.SerializeProperty('classname')],
        String, 'className'
    ]);
});
